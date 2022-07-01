using System.Text.Json.Serialization;
using Unite.Data.Entities.Donors.Clinical.Enums;

namespace Unite.Donors.Feed.Web.Services.Donors;

public class ClinicalDataModel
{
    [JsonPropertyName("Sex")]
    public Gender? Gender { get; set; }
    public int? Age { get; set; }
    public string Diagnosis { get; set; }
    public DateOnly? DiagnosisDate { get; set; }
    public string PrimarySite { get; set; }
    public string Localization { get; set; }
    public bool? VitalStatus { get; set; }
    public DateOnly? VitalStatusChangeDate { get; set; }
    public int? VitalStatusChangeDay { get; set; }
    public bool? ProgressionStatus { get; set; }
    public DateOnly? ProgressionStatusChangeDate { get; set; }
    public int? ProgressionStatusChangeDay { get; set; }
    public int? KpsBaseline { get; set; }
    public bool? SteroidsBaseline { get; set; }


    public void Sanitise()
    {
        Diagnosis = Diagnosis?.Trim();
        PrimarySite = PrimarySite?.Trim();
        Localization = Localization?.Trim();
    }
}
