using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;
using Unite.Donors.DataFeed.Domain.Resources.Extensions;
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

        private readonly Repository<Donor> _donorRepository;
        private readonly Repository<ClinicalData> _clinicalDataRepository;
        private readonly Repository<Therapy> _therapyRepository;
        private readonly Repository<Treatment> _treatmentRepository;
        private readonly Repository<WorkPackage> _workPackageRepository;
        private readonly Repository<WorkPackageDonor> _workPackageDonorRepository;
        private readonly Repository<Study> _studyRepository;
        private readonly Repository<StudyDonor> _studyDonorRepository;

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

                var donorModel = donorResource.GetDonor();
                var donor = CreateOrUpdate(donorModel, ref audit);

                if (donorResource.ClinicalData != null)
                {
                    var clinicalDataModel = donorResource.ClinicalData.GetClinicalData(donor.Id);
                    var clinicalData = CreateOrUpdate(clinicalDataModel, ref audit);
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

                donorsToIndex.Add(donor.Id);
                mutationsToIndex.AddRange(GetDonorMutations(donor.Id));
            }

            _donorIndexingTaskRepository.AddRange(donorsToIndex.ToArray());
            _mutationIndexingTaskRepository.AddRange(mutationsToIndex.ToArray());

            _logger.LogInformation(audit.ToString());
        }


        private Donor CreateOrUpdate(in Donor donor, ref UploadAudit audit)
        {
            var donorId = donor.Id;

            var entity = _donorRepository.Find(donor =>
                donor.Id == donorId
            );

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
            var donorId = clinicalData.DonorId;

            var entity = _clinicalDataRepository.Find(clinicalData =>
                clinicalData.DonorId == donorId
            );

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
            var name = therapy.Name;

            var entity = _therapyRepository.Find(therapy =>
                therapy.Name == name
            );

            if (entity == null)
            {
                entity = _therapyRepository.Add(therapy);

                audit.TherapiesCreated++;
            }

            return entity;
        }

        private Treatment CreateOrUpdate(in Treatment treatment, ref UploadAudit audit)
        {
            var donorId = treatment.DonorId;
            var therapyId = treatment.TherapyId;
            var startDate = treatment.StartDate;

            var entity = _treatmentRepository.Find(treatment =>
                treatment.DonorId == donorId &&
                treatment.TherapyId == therapyId &&
                treatment.StartDate == startDate
            );

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
            var name = workPackage.Name;

            var entity = _workPackageRepository.Find(workPackage =>
                workPackage.Name == name
            );

            if (entity == null)
            {
                entity = _workPackageRepository.Add(workPackage);

                audit.WorkPackagesCreated++;
            }

            return entity;
        }

        private WorkPackageDonor GetOrCreate(in WorkPackageDonor workPackageDonor, ref UploadAudit audit)
        {
            var workPackageId = workPackageDonor.WorkPackageId;
            var donorId = workPackageDonor.DonorId;

            var entity = _workPackageDonorRepository.Find(workPackageDonor =>
                workPackageDonor.WorkPackageId == workPackageId &&
                workPackageDonor.DonorId == donorId
            );

            if (entity == null)
            {
                entity = _workPackageDonorRepository.Add(workPackageDonor);

                audit.WorkPackagesAssociated++;
            }

            return entity;
        }

        private Study GetOrCreate(in Study study, ref UploadAudit audit)
        {
            var name = study.Name;

            var entity = _studyRepository.Find(study =>
                study.Name == name
            );

            if(entity == null)
            {
                entity = _studyRepository.Add(study);

                audit.StudiesCreated++;
            }

            return entity;
        }

        private StudyDonor GetOrCreate(in StudyDonor studyDonor, ref UploadAudit audit)
        {
            var studyId = studyDonor.StudyId;
            var donorId = studyDonor.DonorId;

            var entity = _studyDonorRepository.Find(studyDonor =>
                studyDonor.StudyId == studyId &&
                studyDonor.DonorId == donorId
            );

            if(entity == null)
            {
                entity = _studyDonorRepository.Add(studyDonor);

                audit.StudiesAssociated++;
            }

            return entity;
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
    }
}
