using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Data.Extensions;
using Unite.Indices.Entities.Basic.Donors;
using Unite.Indices.Entities.Basic.Donors.Clinical;

namespace Unite.Donors.Indices.Services.Mappers;

internal class DonorIndexMapper
{
    internal void Map(in Donor donor, DonorIndex index)
    {
        if (donor == null)
        {
            return;
        }

        index.Id = donor.Id;
        index.ReferenceId = donor.ReferenceId;
        index.MtaProtected = donor.MtaProtected;

        index.ClinicalData = CreateFrom(donor.ClinicalData);
        index.Treatments = CreateFrom(donor.Treatments, donor.ClinicalData?.DiagnosisDate);
        index.WorkPackages = CreateFrom(donor.DonorWorkPackages);
        index.Studies = CreateFrom(donor.DonorStudies);
    }

    private static ClinicalDataIndex CreateFrom(in ClinicalData clinicalData)
    {
        if (clinicalData == null)
        {
            return null;
        }

        var index = new ClinicalDataIndex();

        index.Gender = clinicalData.GenderId?.ToDefinitionString();
        index.Age = clinicalData.Age;
        index.Diagnosis = clinicalData.Diagnosis;
        index.PrimarySite = clinicalData.PrimarySite?.Value;
        index.Localization = clinicalData.Localization?.Value;
        index.VitalStatus = clinicalData.VitalStatus;
        index.VitalStatusChangeDay = clinicalData.VitalStatusChangeDate.RelativeFrom(clinicalData.DiagnosisDate) ?? clinicalData.VitalStatusChangeDay;
        index.ProgressionStatus = clinicalData.ProgressionStatus;
        index.ProgressionStatusChangeDay = clinicalData.ProgressionStatusChangeDate.RelativeFrom(clinicalData.DiagnosisDate) ?? clinicalData.ProgressionStatusChangeDay;
        index.KpsBaseline = clinicalData.KpsBaseline;
        index.SteroidsBaseline = clinicalData.SteroidsBaseline;

        return index;
    }

    private static TreatmentIndex[] CreateFrom(in IEnumerable<Treatment> treatments, DateOnly? diagnosisDate)
    {
        if (treatments == null || !treatments.Any())
        {
            return null;
        }

        var indices = treatments.Select(treatment =>
        {
            var index = new TreatmentIndex();

            index.Therapy = treatment.Therapy.Name;
            index.Details = treatment.Details;
            index.StartDay = treatment.StartDate.RelativeFrom(diagnosisDate) ?? treatment.StartDay;
            index.DurationDays = treatment.EndDate.RelativeFrom(treatment.StartDate) ?? treatment.DurationDays;
            index.Results = treatment.Results;

            return index;

        }).ToArray();

        return indices;
    }

    private static WorkPackageIndex[] CreateFrom(in IEnumerable<WorkPackageDonor> workPackageDonors)
    {
        if (workPackageDonors == null || !workPackageDonors.Any())
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
        if (studyDonors == null || !studyDonors.Any())
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
