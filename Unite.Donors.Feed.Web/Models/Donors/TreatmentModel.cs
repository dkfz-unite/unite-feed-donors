using System.Text.Json.Serialization;

namespace Unite.Donors.Feed.Web.Models.Donors;

public class TreatmentModel
{
    private string _therapy;
    private string _details;
    private DateOnly? _startDate;
    private int? _startDay;
    private DateOnly? _endDate;
    private int? _durationDays;
    private string _results;

    [JsonPropertyName("therapy")]
    public string Therapy { get => _therapy?.Trim(); set => _therapy = value; }
    [JsonPropertyName("details")]
    public string Details { get => _details?.Trim(); set => _details = value; }
    [JsonPropertyName("start_date")]
    public DateOnly? StartDate { get => _startDate; set => _startDate = value; }
    [JsonPropertyName("start_day")]
    public int? StartDay { get => _startDay; set => _startDay = value; }
    [JsonPropertyName("end_date")]
    public DateOnly? EndDate { get => _endDate; set => _endDate = value; }
    [JsonPropertyName("duration_days")]
    public int? DurationDays { get => _durationDays; set => _durationDays = value; }
    [JsonPropertyName("results")]
    public string Results { get => _results?.Trim(); set => _results = value; }
}
