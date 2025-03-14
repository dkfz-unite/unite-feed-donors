using System.Text.Json.Serialization;
using Unite.Donors.Feed.Web.Models.Base;

namespace Unite.Donors.Feed.Web.Models.Donors;

public class DonorModel
{
    private string _id;
    private bool? _mtaProtected;
    private string[] _projects = ["Other"];
    private string[] _studies;


    [JsonPropertyName("id")]
    public string Id { get => _id?.Trim(); set => _id = value; }

    [JsonPropertyName("mta")]
    public bool? MtaProtected { get => _mtaProtected; set => _mtaProtected = value; }

    [JsonPropertyName("projects")]
    public string[] Projects { get => Trim(_projects); set => _projects = value; }

    [JsonPropertyName("studies")]
    public string[] Studies { get => Trim(_studies); set => _studies = value; }


    [JsonPropertyName("clinical_data")]
    public ClinicalDataModel ClinicalData { get; set; }

    [JsonPropertyName("treatments")]
    public TreatmentModel[] Treatments { get; set; }


    private static string[] Trim(string[] array)
    {
        return array?.Select(value => value.Trim()).ToArray();
    }
}
