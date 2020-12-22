using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;
using Unite.Donors.DataFeed.Web.Services.Audit;
using Unite.Donors.DataFeed.Web.Services.Extensions;
using Unite.Donors.DataFeed.Web.Services.Repositories;
using Unite.Donors.DataFeed.Web.Services.Repositories.Tasks;

namespace Unite.Donors.DataFeed.Web.Services
{
    public class DataFeedService : IDataFeedService
    {
        private readonly ILogger _logger;
        private readonly UniteDbContext _database;

        private readonly DonorRepository _donorRepository;
        private readonly ClinicalDataRepository _clinicalDataRepository;
        private readonly TherapyRepository _therapyRepository;
        private readonly TreatmentRepository _treatmentRepository;
        private readonly WorkPackageRepository _workPackageRepository;
        private readonly WorkPackageDonorRepository _workPackageDonorRepository;
        private readonly StudyRepository _studyRepository;
        private readonly StudyDonorRepository _studyDonorRepository;

        private readonly DonorIndexingTaskRepository _donorIndexingTaskRepository;
        private readonly MutationIndexingTaskRepository _mutationIndexingTaskRepository;


        public DataFeedService(
            UniteDbContext database,
            ILogger<DataFeedService> logger)
        {
            _logger = logger;
            _database = database;

            _donorRepository = new DonorRepository(database, logger);
            _clinicalDataRepository = new ClinicalDataRepository(database, logger);
            _therapyRepository = new TherapyRepository(database, logger);
            _treatmentRepository = new TreatmentRepository(database, logger);
            _workPackageRepository = new WorkPackageRepository(database, logger);
            _workPackageDonorRepository = new WorkPackageDonorRepository(database, logger);
            _studyRepository = new StudyRepository(database, logger);
            _studyDonorRepository = new StudyDonorRepository(database, logger);

            _donorIndexingTaskRepository = new DonorIndexingTaskRepository(database, logger);
            _mutationIndexingTaskRepository = new MutationIndexingTaskRepository(database, logger);            
        }


        public void ProcessDonors(IEnumerable<Domain.Resources.Donor> donors)
        {
            var totalDonors = donors.Count();

            _logger.LogInformation($"Processing {totalDonors} donors");

            var donorsToIndex = new HashSet<string>();
            var mutationsToIndex = new HashSet<int>();
            var audit = new UploadAudit();

            foreach (var donorResource in donors)
            {
                donorResource.Sanitize();

                var donorModel = donorResource.ToEntity();
                var donor = CreateOrUpdate(donorModel, ref audit);

                if (donorResource.ClinicalData != null)
                {
                    var clinicalDataModel = donorResource.ClinicalData.ToEntity(donor.Id);
                    var clinicalData = CreateOrUpdate(clinicalDataModel, ref audit);
                }

                if (donorResource.Treatments != null)
                {
                    foreach(var treatmentResource in donorResource.Treatments)
                    {
                        var therapyModel = treatmentResource.GetTherapy();
                        var therapy = GetOrCreate(therapyModel, ref audit);

                        var treatmentModel = treatmentResource.ToEntity();
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

                        var workPackageDonorModel = new WorkPackageDonor();
                        workPackageDonorModel.WorkPackage = workPackage;
                        workPackageDonorModel.Donor = donor;
                        
                        var workPackageDonor = GetOrCreate(workPackageDonorModel, ref audit);
                    }
                }

                if(donorResource.Studies != null)
                {
                    foreach(var studyName in donorResource.Studies)
                    {
                        var studyModel = new Study() { Name = studyName };
                        var study = GetOrCreate(studyModel, ref audit);

                        var studyDonorModel = new StudyDonor();
                        studyDonorModel.Study = study;
                        studyDonorModel.Donor = donor;

                        var studyDonor = GetOrCreate(studyDonorModel, ref audit);
                    }
                }

                donorsToIndex.Add(donor.Id);
                mutationsToIndex.AddRange(GetDonorMutations(donor.Id));
            }

            _donorIndexingTaskRepository.AddRange(donorsToIndex.ToArray());
            _mutationIndexingTaskRepository.AddRange(mutationsToIndex.ToArray());

            _logger.LogInformation(audit.ToString());
        }


        private Donor CreateOrUpdate(in Donor donor, ref UploadAudit audit)
        {
            var entity = _donorRepository.Find(donor.Id);

            if (entity == null)
            {
                entity = _donorRepository.Add(donor);

                audit.DonorsCreated++;
            }
            else
            {
                _donorRepository.Update(ref entity, donor);

                audit.DonorsUpdated++;
            }

            return entity;
        }

        private ClinicalData CreateOrUpdate(in ClinicalData clinicalData, ref UploadAudit audit)
        {
            var entity = _clinicalDataRepository.Find(clinicalData.DonorId);

            if (entity == null)
            {
                entity = _clinicalDataRepository.Add(clinicalData);

                audit.ClinicalDataCreated++;
            }
            else
            {
                _clinicalDataRepository.Update(ref entity, clinicalData);

                audit.ClinicalDataUpdated++;
            }

            return entity;
        }

        private Therapy GetOrCreate(in Therapy therapy, ref UploadAudit audit)
        {
            var entity = _therapyRepository.Find(therapy.Name);

            if (entity == null)
            {
                entity = _therapyRepository.Add(therapy);

                audit.TherapiesCreated++;
            }

            return entity;
        }

        private Treatment CreateOrUpdate(in Treatment treatment, ref UploadAudit audit)
        {
            var entity = _treatmentRepository.Find(
                treatment.Donor.Id,
                treatment.Therapy.Id,
                treatment.StartDate,
                treatment.EndDate);

            if(entity == null)
            {
                entity = _treatmentRepository.Add(treatment);

                audit.TreatmentsCreated++;
            }
            else
            {
                _treatmentRepository.Update(ref entity, treatment);

                audit.TreatmentsUpdated++;
            }

            return entity;
        }

        private WorkPackage GetOrCreate(in WorkPackage workPackage, ref UploadAudit audit)
        {
            var entity = _workPackageRepository.Find(workPackage.Name);

            if (entity == null)
            {
                entity = _workPackageRepository.Add(workPackage);

                audit.WorkPackagesCreated++;
            }

            return entity;
        }

        private WorkPackageDonor GetOrCreate(in WorkPackageDonor workPackageDonor, ref UploadAudit audit)
        {
            var entity = _workPackageDonorRepository.Find(
                workPackageDonor.WorkPackage.Id,
                workPackageDonor.Donor.Id);

            if (entity == null)
            {
                entity = _workPackageDonorRepository.Add(workPackageDonor);

                audit.WorkPackagesAssociated++;
            }

            return entity;
        }

        private Study GetOrCreate(in Study study, ref UploadAudit audit)
        {
            var entity = _studyRepository.Find(study.Name);

            if(entity == null)
            {
                entity = _studyRepository.Add(study);

                audit.StudiesCreated++;
            }

            return entity;
        }

        private StudyDonor GetOrCreate(in StudyDonor studyDonor, ref UploadAudit audit)
        {
            var entity = _studyDonorRepository.Find(
                studyDonor.Study.Id,
                studyDonor.Donor.Id);

            if(entity == null)
            {
                entity = _studyDonorRepository.Add(studyDonor);

                audit.StudiesAssociated++;
            }

            return entity;
        }


        private IEnumerable<int> GetDonorMutations(string donorId)
        {
            var mutations = _database.SampleMutations
                .Include(sampleMutaton => sampleMutaton.Sample)
                .Where(sampleMutation => sampleMutation.Sample.DonorId == donorId)
                .Select(sampleMutation => sampleMutation.MutationId)
                .Distinct();

            return mutations;
        }
    }
}
