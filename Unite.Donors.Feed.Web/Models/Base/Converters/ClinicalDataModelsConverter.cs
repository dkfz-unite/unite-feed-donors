namespace Unite.Donors.Feed.Web.Models.Base.Converters;

public class ClinicalDataModelsConverter
{
    public Data.Models.ClinicalDataModel Convert(in ClinicalDataModel source)
    {
        if (source == null)
        {
            return null;
        }

        return new Data.Models.ClinicalDataModel
        {
            Gender = source.Gender,
            Age = source.Age,
            Diagnosis = source.Diagnosis,
            DiagnosisDate = source.DiagnosisDate,
            PrimarySite = source.PrimarySite,
            Localization = source.Localization,
            VitalStatus = source.VitalStatus,
            VitalStatusChangeDate = source.VitalStatusChangeDate,
            VitalStatusChangeDay = source.VitalStatusChangeDay,
            ProgressionStatus = source.ProgressionStatus,
            ProgressionStatusChangeDate = source.ProgressionStatusChangeDate,
            ProgressionStatusChangeDay = source.ProgressionStatusChangeDay,
            KpsBaseline = source.KpsBaseline,
            SteroidsBaseline = source.SteroidsBaseline
        };
    }

    public Data.Models.ClinicalDataModel[] Convert(in ClinicalDataModel[] source)
    {
        if (source == null)
        {
            return null;
        }

        return source.Select(model => Convert(model)).ToArray();
    }
}
