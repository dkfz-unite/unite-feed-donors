using System.Collections.Generic;
using Unite.Data.Entities.Clinical;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;
using Unite.Donors.Feed.Data.Donors.Models;
using Unite.Donors.Feed.Data.Donors.Models.Audit;
using Unite.Donors.Feed.Data.Donors.Repositories;

namespace Unite.Donors.Feed.Data.Donors
{
    public class DonorDataWriter : DataWriter<DonorModel, DonorsUploadAudit>
    {
        private readonly DonorRepository _donorRepository;
        private readonly ClinicalDataRepository _clinicalDataRepository;
        private readonly TreatmentRepository _treatmentRepository;
        private readonly WorkPackageDonorRepository _workPackageDonorRepository;
        private readonly StudyDonorRepository _studyDonorRepository;


        public DonorDataWriter(UniteDbContext dbContext) : base(dbContext)
        {
            _donorRepository = new DonorRepository(dbContext);
            _clinicalDataRepository = new ClinicalDataRepository(dbContext);
            _treatmentRepository = new TreatmentRepository(dbContext);
            _workPackageDonorRepository = new WorkPackageDonorRepository(dbContext);
            _studyDonorRepository = new StudyDonorRepository(dbContext);
        }


        protected override void ProcessModel(DonorModel donorModel, ref DonorsUploadAudit audit)
        {
            var donor = CreateorUpdateDonor(donorModel, ref audit);

            if (donorModel.ClinicalData != null)
            {
                CreateOrUpdateClinicalData(donor.Id, donorModel.ClinicalData, ref audit);
            }

            if (donorModel.Treatments != null)
            {
                foreach (var treatmentModel in donorModel.Treatments)
                {
                    CreateOrUpdateTreatment(donor.Id, treatmentModel, ref audit);
                }
            }

            if (donorModel.WorkPackages != null)
            {
                CreateMissingWorkpackages(donor.Id, donorModel.WorkPackages, ref audit);
            }

            if (donorModel.Studies != null)
            {
                CreateMissingStudies(donor.Id, donorModel.Studies, ref audit);
            }
        }


        private Donor CreateorUpdateDonor(DonorModel donorModel, ref DonorsUploadAudit audit)
        {
            var donor = _donorRepository.Find(donorModel);

            if (donor == null)
            {
                donor = _donorRepository.Create(donorModel);

                audit.DonorsCreated++;
                audit.Donors.Add(donor.Id);
            }
            else
            {
                _donorRepository.Update(donor, donorModel);

                audit.DonorsUpdated++;
                audit.Donors.Add(donor.Id);
            }

            return donor;
        }

        private ClinicalData CreateOrUpdateClinicalData(int donorId, ClinicalDataModel clinicalDataModel, ref DonorsUploadAudit audit)
        {
            var clinicalData = _clinicalDataRepository.Find(donorId);

            if (clinicalData == null)
            {
                _clinicalDataRepository.Create(donorId, clinicalDataModel);
                audit.ClinicalDataEntriesCreated++;
            }
            else
            {
                _clinicalDataRepository.Update(clinicalData, clinicalDataModel);
                audit.ClinicalDataEntriesUpdated++;
            }

            return clinicalData;
        }

        private Treatment CreateOrUpdateTreatment(int donorId, TreatmentModel treatmentModel, ref DonorsUploadAudit audit)
        {
            var treatment = _treatmentRepository.Find(donorId, treatmentModel);

            if (treatment == null)
            {
                _treatmentRepository.Create(donorId, treatmentModel);
                audit.TreatmentsCreated++;
            }
            else
            {
                _treatmentRepository.Update(treatment, treatmentModel);
                audit.TreatmentsUpdated++;
            }

            return treatment;
        }

        private IEnumerable<WorkPackageDonor> CreateMissingWorkpackages(int donorId, IEnumerable<string> workPackageNames, ref DonorsUploadAudit audit)
        {
            var workPackageDonors = _workPackageDonorRepository.CreateMissing(donorId, workPackageNames);

            audit.WorkPackagesAssociated++;

            return workPackageDonors;
        }

        private IEnumerable<StudyDonor> CreateMissingStudies(int donorId, IEnumerable<string> studyNames, ref DonorsUploadAudit audit)
        {
            var studyDonors = _studyDonorRepository.CreateMissing(donorId, studyNames);

            audit.StudiesAssociated++;

            return studyDonors;
        }
    }
}
