using System.Text.Json.Serialization;
using Unite.Data.Entities.Donors.Clinical.Enums;

namespace Unite.Donors.Feed.Web.Models.Donors;

public class ClinicalDataModel
{
    private Gender? _gender;
    private int? _age;
    private string _diagnosis;
    private DateTime? _diagnosisDate;
    private string _primarySite;
    private string _localization;
    private bool? _vitalStatus;
    private DateTime? _vitalStatusChangeDate;
    private int? _vitalStatusChangeDay;
    private bool? _progressionStatus;
    private DateTime? _progressionStatusChangeDate;
    private int? _progressionStatusChangeDay;
    private int? _kpsBaseline;
    private bool? _steroidsBaseline;

    [JsonPropertyName("Sex")]
    public Gender? Gender { get => _gender; set => _gender = value; }
    [JsonPropertyName("age")]
    public int? Age { get => _age; set => _age = value; }
    [JsonPropertyName("diagnosis")]
    public string Diagnosis { get => _diagnosis?.Trim(); set => _diagnosis = value; }
    [JsonPropertyName("diagnosis_date")]
    public DateTime? DiagnosisDate { get => _diagnosisDate; set => _diagnosisDate = value; }
    [JsonPropertyName("primary_site")]
    public string PrimarySite { get => _primarySite?.Trim(); set => _primarySite = value; }
    [JsonPropertyName("localization")]
    public string Localization { get => _localization?.Trim(); set => _localization = value; }
    [JsonPropertyName("vital_status")]
    public bool? VitalStatus { get => _vitalStatus; set => _vitalStatus = value; }
    [JsonPropertyName("vital_status_change_date")]
    public DateTime? VitalStatusChangeDate { get => _vitalStatusChangeDate; set => _vitalStatusChangeDate = value; }
    [JsonPropertyName("vital_status_change_day")]
    public int? VitalStatusChangeDay { get => _vitalStatusChangeDay; set => _vitalStatusChangeDay = value; }
    [JsonPropertyName("progression_status")]
    public bool? ProgressionStatus { get => _progressionStatus; set => _progressionStatus = value; }
    [JsonPropertyName("progression_status_change_date")]
    public DateTime? ProgressionStatusChangeDate { get => _progressionStatusChangeDate; set => _progressionStatusChangeDate = value; }
    [JsonPropertyName("progression_status_change_day")]
    public int? ProgressionStatusChangeDay { get => _progressionStatusChangeDay; set => _progressionStatusChangeDay = value; }
    [JsonPropertyName("kps_baseline")]
    public int? KpsBaseline { get => _kpsBaseline; set => _kpsBaseline = value; }
    [JsonPropertyName("steroids_baseline")]
    public bool? SteroidsBaseline { get => _steroidsBaseline; set => _steroidsBaseline = value; }
}
