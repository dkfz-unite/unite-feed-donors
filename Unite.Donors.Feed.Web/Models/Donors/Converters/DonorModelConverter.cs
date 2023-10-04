namespace Unite.Donors.Feed.Web.Models.Donors.Converters;

public class DonorModelConverter
{
    public Data.Donors.Models.DonorModel Convert(DonorModel source)
    {
        var donorModel = new Data.Donors.Models.DonorModel();

        Map(source, donorModel);

        if (source.ClinicalData != null)
        {
            donorModel.ClinicalData = new Data.Donors.Models.ClinicalDataModel();

            Map(source.ClinicalData, donorModel.ClinicalData);
        }

        if (source.Treatments != null)
        {
            donorModel.Treatments = source.Treatments.Select(treatment =>
            {
                var treatmentModel = new Data.Donors.Models.TreatmentModel();

                Map(treatment, treatmentModel);

                return treatmentModel;

            }).ToArray();
        }

        return donorModel;
    }

    private static void Map(DonorModel source, Data.Donors.Models.DonorModel target)
    {
        target.ReferenceId = source.Id;
        target.MtaProtected = source.MtaProtected;

        target.WorkPackages = source.Projects;
        target.Studies = source.Studies;
    }

    private static void Map(ClinicalDataModel source, Data.Donors.Models.ClinicalDataModel target)
    {
        target.Gender = source.Gender;
        target.Age = source.Age;
        target.Diagnosis = source.Diagnosis;
        target.DiagnosisDate = FromDateTime(source.DiagnosisDate);
        target.PrimarySite = source.PrimarySite;
        target.Localization = source.Localization;
        target.VitalStatus = source.VitalStatus;
        target.VitalStatusChangeDate = FromDateTime(source.VitalStatusChangeDate);
        target.VitalStatusChangeDay = source.VitalStatusChangeDay;
        target.ProgressionStatus = source.ProgressionStatus;
        target.ProgressionStatusChangeDate = FromDateTime(source.ProgressionStatusChangeDate);
        target.ProgressionStatusChangeDay = source.ProgressionStatusChangeDay;
        target.KpsBaseline = source.KpsBaseline;
        target.SteroidsBaseline = source.SteroidsBaseline;
    }

    private static void Map(TreatmentModel source, Data.Donors.Models.TreatmentModel target)
    {
        target.Therapy = source.Therapy;
        target.Details = source.Details;
        target.StartDate = FromDateTime(source.StartDate);
        target.StartDay = source.StartDay;
        target.EndDate = FromDateTime(source.EndDate);
        target.DurationDays = source.DurationDays;
        target.Results = source.Results;
    }

    private static DateOnly? FromDateTime(DateTime? date)
    {
        return date != null
            ? DateOnly.FromDateTime(date.Value)
            : null;
    }
}
