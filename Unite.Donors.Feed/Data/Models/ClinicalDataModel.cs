using Unite.Data.Entities.Donors.Clinical.Enums;

namespace Unite.Donors.Feed.Data.Models;

public class ClinicalDataModel
{
    public Sex? Sex { get; set; }
    public DateOnly? EnrollmentDate { get; set; }
    public int? EnrollmentAge { get; set; }
    public string Diagnosis { get; set; }
    public string PrimarySite { get; set; }
    public string Localization { get; set; }
    public bool? VitalStatus { get; set; }
    public DateOnly? VitalStatusChangeDate { get; set; }
    public int? VitalStatusChangeDay { get; set; }
    public bool? ProgressionStatus { get; set; }
    public DateOnly? ProgressionStatusChangeDate { get; set; }
    public int? ProgressionStatusChangeDay { get; set; }
    public bool? SteroidsReactive { get; set; }
    public int? Kps { get; set; }
}
