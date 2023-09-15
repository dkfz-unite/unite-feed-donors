using System.Text.Json.Serialization;

namespace Unite.Donors.Feed.Web.Models.Donors;

public class DonorModel
{
    private string _id;
    private bool? _mtaProtected;
    private string[] _projects;
    private string[] _studies;

    [JsonPropertyName("id")]
    public string Id { get => _id?.Trim(); set => _id = value; }
    [JsonPropertyName("mta")]
    public bool? MtaProtected { get => _mtaProtected; set => _mtaProtected = value; }
    [JsonPropertyName("work_packages")]
    public string[] Projects { get => Trim(_projects); set => _projects = value; }
    [JsonPropertyName("studies")]
    public string[] Studies { get => Trim(_studies); set => _studies = value; }

    public ClinicalDataModel ClinicalData { get; set; }
    public TreatmentModel[] Treatments { get; set; }

    private static string[] Trim(string[] array)
    {
        return array?.Select(value => value.Trim()).ToArray();
    }
}
