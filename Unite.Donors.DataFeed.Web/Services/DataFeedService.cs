using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Epigenetics;
using Unite.Data.Entities.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;
using Unite.Donors.DataFeed.Domain.Resources.Extensions;
using Unite.Donors.DataFeed.Web.Services.Audit;
using Unite.Donors.DataFeed.Web.Services.Repositories;

namespace Unite.Donors.DataFeed.Web.Services
{
    public class DataFeedService : IDataFeedService
    {
        private readonly UniteDbContext _database;
        private readonly Repository<Donor> _donorRepository;
        private readonly Repository<ClinicalData> _clinicalDataRepository;
        private readonly Repository<EpigeneticsData> _epigeneticsDataRepository;
        private readonly Repository<Therapy> _therapyRepository;
        private readonly Repository<Treatment> _treatmentRepository;
        private readonly Repository<WorkPackage> _workPackageRepository;
        private readonly Repository<WorkPackageDonor> _workPackageDonorRepository;
        private readonly Repository<Study> _studyRepository;
        private readonly Repository<StudyDonor> _studyDonorRepository;
        private readonly ILogger _logger;


        public DataFeedService(
            UniteDbContext database,
            ILogger<DataFeedService> logger)
        {
            _logger = logger;
            _database = database;

            _donorRepository = new DonorRepository(database);
            _clinicalDataRepository = new ClinicalDataRepository(database);
            _epigeneticsDataRepository = new EpigeneticsDataRepository(database);
            _therapyRepository = new TherapyRepository(database);
            _treatmentRepository = new TreatmentRepository(database);
            _workPackageRepository = new WorkPackageRepository(database);
            _workPackageDonorRepository = new WorkPackageDonorRepository(database);
            _studyRepository = new StudyRepository(database);
            _studyDonorRepository = new StudyDonorRepository(database);           
        }


        public void ProcessDonors(IEnumerable<Domain.Resources.Donor> donors)
        {
            var totalDonors = donors.Count();

            _logger.LogInformation($"Processing {totalDonors} donors");

            var audit = new UploadAudit();

            foreach (var donorResource in donors)
            {
                donorResource.Sanitize();

                var transaction = _database.Database.BeginTransaction();

                var donorModel = donorResource.GetDonor();
                var donor = CreateOrUpdate(donorModel, ref audit);

                if (donorResource.ClinicalData != null)
                {
                    var clinicalDataModel = donorResource.ClinicalData.GetClinicalData(donor.Id);
                    var clinicalData = CreateOrUpdate(clinicalDataModel, ref audit);
                }

                if (donorResource.EpigeneticsData != null)
                {
                    var epigeneticsDataModel = donorResource.EpigeneticsData.GetEpigeneticsData(donor.Id);
                    var epigeneticsData = CreateOrUpdate(epigeneticsDataModel, ref audit);
                }

                if (donorResource.Treatments != null)
                {
                    foreach(var treatmentResource in donorResource.Treatments)
                    {
                        var therapyModel = treatmentResource.GetTherapy();
                        var therapy = GetOrCreate(therapyModel, ref audit);

                        var treatmentModel = treatmentResource.GetTreatment();
                        treatmentModel.Donor = donor;
                        treatmentModel.Therapy = therapy;

                        var treatment = CreateOrUpdate(treatmentModel, ref audit);
                    }
                }

                if(donorResource.WorkPackages != null)
                {
                    foreach(var workPackageName in donorResource.WorkPackages)
                    {
                        var workPackageModel = new WorkPackage() { Name = workPackageName };
                        var workPackage = GetOrCreate(workPackageModel, ref audit);

                        var workPackageDonorModel = new WorkPackageDonor() { WorkPackage = workPackage, Donor = donor };                        
                        var workPackageDonor = GetOrCreate(workPackageDonorModel, ref audit);
                    }
                }

                if(donorResource.Studies != null)
                {
                    foreach(var studyName in donorResource.Studies)
                    {
                        var studyModel = new Study() { Name = studyName };
                        var study = GetOrCreate(studyModel, ref audit);

                        var studyDonorModel = new StudyDonor() { Study = study, Donor = donor };
                        var studyDonor = GetOrCreate(studyDonorModel, ref audit);
                    }
                }

                audit.Donors.Add(donor.Id);

                transaction.Commit();
            }

            PopulateTasks(audit.Donors);

            _logger.LogInformation(audit.ToString());
        }


        private Donor CreateOrUpdate(in Donor model, ref UploadAudit audit)
        {
            var donorId = model.Id;

            var entity = _donorRepository.Entities
                .Include(entity => entity.PrimarySite)
                .FirstOrDefault(entity =>
                    entity.Id == donorId
            );

            if (entity == null)
            {
                entity = _donorRepository.Add(model);

                audit.DonorsCreated++;
            }
            else
            {
                _donorRepository.Update(ref entity, model);

                audit.DonorsUpdated++;
            }

            return entity;
        }

        private ClinicalData CreateOrUpdate(in ClinicalData model, ref UploadAudit audit)
        {
            var donorId = model.DonorId;

            var entity = _clinicalDataRepository.Entities
                .Include(entity => entity.Localization)
                .FirstOrDefault(entity =>
                    entity.DonorId == donorId
            );

            if (entity == null)
            {
                entity = _clinicalDataRepository.Add(model);

                audit.ClinicalDataCreated++;
            }
            else
            {
                _clinicalDataRepository.Update(ref entity, model);

                audit.ClinicalDataUpdated++;
            }

            return entity;
        }

        private EpigeneticsData CreateOrUpdate(in EpigeneticsData model, ref UploadAudit audit)
        {
            var donorId = model.DonorId;

            var entity = _epigeneticsDataRepository.Entities
                .FirstOrDefault(entity =>
                    entity.DonorId == donorId
            );

            if (entity == null)
            {
                entity = _epigeneticsDataRepository.Add(model);

                audit.EpigeneticsDataCreated++;
            }
            else
            {
                _epigeneticsDataRepository.Update(ref entity, model);

                audit.EpigeneticsDataUpdated++;
            }

            return entity;
        }

        private Therapy GetOrCreate(in Therapy model, ref UploadAudit audit)
        {
            var name = model.Name;

            var entity = _therapyRepository.Entities
                .FirstOrDefault(entity =>
                    entity.Name == name
            );

            if (entity == null)
            {
                entity = _therapyRepository.Add(model);

                audit.TherapiesCreated++;
            }

            return entity;
        }

        private Treatment CreateOrUpdate(in Treatment model, ref UploadAudit audit)
        {
            var donorId = model.Donor.Id;
            var therapyId = model.Therapy.Id;
            var startDate = model.StartDate;

            var entity = _treatmentRepository.Entities
                .FirstOrDefault(entity =>
                    entity.DonorId == donorId &&
                    entity.TherapyId == therapyId &&
                    entity.StartDate == startDate
            );

            if(entity == null)
            {
                entity = _treatmentRepository.Add(model);

                audit.TreatmentsCreated++;
            }
            else
            {
                _treatmentRepository.Update(ref entity, model);

                audit.TreatmentsUpdated++;
            }

            return entity;
        }

        private WorkPackage GetOrCreate(in WorkPackage model, ref UploadAudit audit)
        {
            var name = model.Name;

            var entity = _workPackageRepository.Entities
                .FirstOrDefault(entity =>
                    entity.Name == name
            );

            if (entity == null)
            {
                entity = _workPackageRepository.Add(model);

                audit.WorkPackagesCreated++;
            }

            return entity;
        }

        private WorkPackageDonor GetOrCreate(in WorkPackageDonor model, ref UploadAudit audit)
        {
            var donorId = model.Donor.Id;
            var workPackageId = model.WorkPackage.Id;
            

            var entity = _workPackageDonorRepository.Entities
                .FirstOrDefault(entity =>
                    entity.DonorId == donorId &&
                    entity.WorkPackageId == workPackageId
            );

            if (entity == null)
            {
                entity = _workPackageDonorRepository.Add(model);

                audit.WorkPackagesAssociated++;
            }

            return entity;
        }

        private Study GetOrCreate(in Study model, ref UploadAudit audit)
        {
            var name = model.Name;

            var entity = _studyRepository.Entities
                .FirstOrDefault(entity =>
                    entity.Name == name
            );

            if(entity == null)
            {
                entity = _studyRepository.Add(model);

                audit.StudiesCreated++;
            }

            return entity;
        }

        private StudyDonor GetOrCreate(in StudyDonor model, ref UploadAudit audit)
        {
            var donorId = model.Donor.Id;
            var studyId = model.Study.Id;

            var entity = _studyDonorRepository.Entities
                .FirstOrDefault(entity =>
                    entity.DonorId == donorId &&
                    entity.StudyId == studyId
            );

            if(entity == null)
            {
                entity = _studyDonorRepository.Add(model);

                audit.StudiesAssociated++;
            }

            return entity;
        }


        private void PopulateTasks(IEnumerable<string> donorIds)
        {
            var donorIndexingTasks = donorIds
                .Select(donorId => CreateTask(TaskTargetType.Donor, donorId))
                .ToArray();

            var mutationIndexingTasks = donorIds
                .SelectMany(donorId => GetDonorMutations(donorId))
                .Distinct()
                .Select(mutationId => CreateTask(TaskTargetType.Mutation, mutationId))
                .ToArray();

            _database.Tasks.AddRange(donorIndexingTasks);
            _database.Tasks.AddRange(mutationIndexingTasks);
            _database.SaveChanges();
        }

        private IEnumerable<int> GetDonorMutations(string donorId)
        {
            var mutations = _database.MutationOccurrences
                .Where(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.DonorId == donorId)
                .Select(mutationOccurrence => mutationOccurrence.MutationId)
                .Distinct()
                .ToArray();

            return mutations;
        }

        private Task CreateTask(TaskTargetType targetType, object target)
        {
            var task = new Task()
            {
                TypeId = TaskType.Indexing,
                TargetTypeId = targetType,
                Target = target.ToString(),
                Data = null,
                Date = DateTime.UtcNow
            };

            return task;
        }
    }
}
