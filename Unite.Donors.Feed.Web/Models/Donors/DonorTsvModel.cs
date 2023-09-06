using Unite.Data.Entities.Donors.Clinical.Enums;

namespace Unite.Donors.Feed.Web.Models.Donors;

public class DonorTsvModel
{
    public string Id { get; set; }
    public bool? MtaProtected { get; set; }

    public string WorkPackages { get; set; }
    public string Studies { get; set; }

    // Clinical data
    public Gender? Gender { get; set; }
    public int? Age { get; set; }
    public string Diagnosis { get; set; }
    public DateTime? DiagnosisDate { get; set; }
    public string PrimarySite { get; set; }
    public string Localization { get; set; }
    public bool? VitalStatus { get; set; }
    public DateTime? VitalStatusChangeDate { get; set; }
    public int? VitalStatusChangeDay { get; set; }
    public bool? ProgressionStatus { get; set; }
    public DateTime? ProgressionStatusChangeDate { get; set; }
    public int? ProgressionStatusChangeDay { get; set; }
    public int? KpsBaseline { get; set; }
    public bool? SteroidsBaseline { get; set; }

    // Treatments data
    public string Therapy { get; set; }
    public string Details { get; set; }
    public DateTime? StartDate { get; set; }
    public int? StartDay { get; set; }
    public DateTime? EndDate { get; set; }
    public int? DurationDays { get; set; }
    public string Results { get; set; }
}
