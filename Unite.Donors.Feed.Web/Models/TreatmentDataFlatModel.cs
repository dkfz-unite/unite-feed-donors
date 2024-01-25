using System.Text.Json.Serialization;
using Unite.Donors.Feed.Web.Models.Base;

namespace Unite.Donors.Feed.Web.Models;

public class TreatmentDataFlatModel : TreatmentModel
{
    private string _donorId;

    [JsonPropertyName("donor_id")]
    public string DonorId { get => _donorId?.Trim(); set => _donorId = value; }
}
