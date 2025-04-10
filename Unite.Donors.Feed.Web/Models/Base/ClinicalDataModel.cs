using System.Text.Json.Serialization;
using Unite.Data.Entities.Donors.Clinical.Enums;

namespace Unite.Donors.Feed.Web.Models.Base;

public class ClinicalDataModel
{
    private Sex? _sex;
    private DateOnly? _enrollmentDate;
    private int? _enrollmentAge;
    private string _diagnosis;
    private string _primarySite;
    private string _localization;
    private bool? _vitalStatus;
    private DateOnly? _vitalStatusChangeDate;
    private int? _vitalStatusChangeDay;
    private bool? _progressionStatus;
    private DateOnly? _progressionStatusChangeDate;
    private int? _progressionStatusChangeDay;
    private bool? _steroidsReactive;
    private int? _kps;


    [JsonPropertyName("sex")]
    public Sex? Sex { get => _sex; set => _sex = value; }

    [JsonPropertyName("enrollment_date")]
    public DateOnly? EnrollmentDate { get => _enrollmentDate; set => _enrollmentDate = value; }

    [JsonPropertyName("enrollment_age")]
    public int? EnrollmentAge { get => _enrollmentAge; set => _enrollmentAge = value; }

    [JsonPropertyName("diagnosis")]
    public string Diagnosis { get => _diagnosis?.Trim(); set => _diagnosis = value; }

    [JsonPropertyName("primary_site")]
    public string PrimarySite { get => _primarySite?.Trim(); set => _primarySite = value; }

    [JsonPropertyName("localization")]
    public string Localization { get => _localization?.Trim(); set => _localization = value; }

    [JsonPropertyName("vital_status")]
    public bool? VitalStatus { get => _vitalStatus; set => _vitalStatus = value; }

    [JsonPropertyName("vital_status_change_date")]
    public DateOnly? VitalStatusChangeDate { get => _vitalStatusChangeDate; set => _vitalStatusChangeDate = value; }

    [JsonPropertyName("vital_status_change_day")]
    public int? VitalStatusChangeDay { get => _vitalStatusChangeDay; set => _vitalStatusChangeDay = value; }

    [JsonPropertyName("progression_status")]
    public bool? ProgressionStatus { get => _progressionStatus; set => _progressionStatus = value; }

    [JsonPropertyName("progression_status_change_date")]
    public DateOnly? ProgressionStatusChangeDate { get => _progressionStatusChangeDate; set => _progressionStatusChangeDate = value; }

    [JsonPropertyName("progression_status_change_day")]
    public int? ProgressionStatusChangeDay { get => _progressionStatusChangeDay; set => _progressionStatusChangeDay = value; }

    [JsonPropertyName("steroids_reactive")]
    public bool? SteroidsReactive { get => _steroidsReactive; set => _steroidsReactive = value; }

    [JsonPropertyName("kps")]
    public int? Kps { get => _kps; set => _kps = value; }
}
