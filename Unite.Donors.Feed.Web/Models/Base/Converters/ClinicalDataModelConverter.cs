namespace Unite.Donors.Feed.Web.Models.Base.Converters;

public class ClinicalDataModelConverter
{
    public Data.Models.ClinicalDataModel Convert(ClinicalDataModel source)
    {
        if (source == null)
            return null;

        var target = new Data.Models.ClinicalDataModel();

        Map(source, ref target);

        return target;
    }

    
    private static void Map(in ClinicalDataModel source, ref Data.Models.ClinicalDataModel target)
    {
        target.Sex = source.Sex;
        target.EnrollmentDate = source.EnrollmentDate;
        target.EnrollmentAge = source.EnrollmentAge;
        target.Diagnosis = source.Diagnosis;
        target.PrimarySite = source.PrimarySite;
        target.Localization = source.Localization;
        target.VitalStatus = source.VitalStatus;
        target.VitalStatusChangeDate = source.VitalStatusChangeDate;
        target.VitalStatusChangeDay = source.VitalStatusChangeDay;
        target.ProgressionStatus = source.ProgressionStatus;
        target.ProgressionStatusChangeDate = source.ProgressionStatusChangeDate;
        target.ProgressionStatusChangeDay = source.ProgressionStatusChangeDay;
        target.SteroidsReactive = source.SteroidsReactive;
        target.Kps = source.Kps;
    }
}
