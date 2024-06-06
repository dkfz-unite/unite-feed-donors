using System.Text.Json.Serialization;

namespace Unite.Donors.Feed.Web.Models.Donors;

public class TreatmentsModel
{
    private string _donorId;

    [JsonPropertyName("donor_id")]
    public string DonorId { get => _donorId?.Trim(); set => _donorId = value; }

    [JsonPropertyName("entries")]
    public Base.TreatmentModel[] Entries { get; set; }
}
