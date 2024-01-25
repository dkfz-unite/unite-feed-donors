using System.Text.Json.Serialization;

namespace Unite.Donors.Feed.Web.Models;

public class TreatmentsDataModel
{
    private string _donorId;

    [JsonPropertyName("donor_id")]
    public string DonorId { get => _donorId?.Trim(); set => _donorId = value; }

    [JsonPropertyName("data")]
    public Base.TreatmentModel[] Data { get; set; }
}
