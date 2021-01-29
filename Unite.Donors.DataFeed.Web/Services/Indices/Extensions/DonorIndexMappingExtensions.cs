using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Extensions;
using Unite.Indices.Entities.Basic.Donors;

namespace Unite.Donors.DataFeed.Web.Services.Indices.Extensions
{
    public static class DonorIndexMappingExtensions
    {
        public static void MapFrom(this DonorIndex index, in Donor donor)
        {
            if (donor is null)
            {
                return;
            }

            index.Id = donor.Id;
            index.Diagnosis = donor.Diagnosis;
            index.DiagnosisDate = donor.DiagnosisDate;
            index.PrimarySite = donor.PrimarySite?.Value;
            index.Origin = donor.Origin;
            index.MtaProtected = donor.MtaProtected;

            index.ClinicalData = CreateFrom(donor.ClinicalData);
            index.Treatments = CreateFrom(donor.Treatments);
            index.WorkPackages = CreateFrom(donor.DonorWorkPackages);
            index.Studies = CreateFrom(donor.DonorStudies);
        }

        private static ClinicalDataIndex CreateFrom(in ClinicalData clinicalData)
        {
            if (clinicalData is null)
            {
                return null;
            }

            var index = new ClinicalDataIndex();

            index.Gender = clinicalData.GenderId?.ToDefinitionString();
            index.Age = clinicalData.Age;
            index.AgeCategory = clinicalData.AgeCategoryId?.ToDefinitionString();
            index.Localization = clinicalData.Localization?.Value;
            index.VitalStatus = clinicalData.VitalStatusId?.ToDefinitionString();
            index.VitalStatusChangeDate = clinicalData.VitalStatusChangeDate;
            index.SurvivalDays = clinicalData.SurvivalDays;
            index.ProgressionDate = clinicalData.ProgressionDate;
            index.ProgressionFreeDays = clinicalData.ProgressionFreeDays;
            index.RelapseDate = clinicalData.RelapseDate;
            index.RelapseFreeDays = clinicalData.RelapseFreeDays;
            index.KpsBaseline = clinicalData.KpsBaseline;
            index.SteroidsBaseline = clinicalData.SteroidsBaseline;

            return index;
        }

        private static TreatmentIndex[] CreateFrom(in IEnumerable<Treatment> treatments)
        {
            if (treatments is null)
            {
                return null;
            }

            var indices = treatments.Select(treatment =>
            {
                var index = new TreatmentIndex();

                index.Therapy = treatment.Therapy.Name;
                index.Details = treatment.Details;
                index.StartDate = treatment.StartDate;
                index.EndDate = treatment.EndDate;
                index.Results = treatment.Results;

                return index;

            }).ToArray();

            return indices;
        }

        private static WorkPackageIndex[] CreateFrom(in IEnumerable<WorkPackageDonor> workPackageDonors)
        {
            if (workPackageDonors is null)
            {
                return null;
            }

            var indices = workPackageDonors.Select(workPackageDonor =>
            {
                var index = new WorkPackageIndex();

                index.Id = workPackageDonor.WorkPackage.Id;
                index.Name = workPackageDonor.WorkPackage.Name;

                return index;

            }).ToArray();

            return indices;
        }

        private static StudyIndex[] CreateFrom(in IEnumerable<StudyDonor> studyDonors)
        {
            if (studyDonors is null)
            {
                return null;
            }

            var indices = studyDonors.Select(studyDonor =>
            {
                var index = new StudyIndex();

                index.Id = studyDonor.Study.Id;
                index.Name = studyDonor.Study.Name;

                return index;

            }).ToArray();

            return indices;
        }
    }
}
