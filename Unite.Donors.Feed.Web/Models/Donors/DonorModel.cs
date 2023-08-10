namespace Unite.Donors.Feed.Web.Models.Donors;

public class DonorModel
{
    private string _id;
    private bool? _mtaProtected;
    private string[] _projects;
    private string[] _studies;

    public string Id { get => _id?.Trim(); set => _id = value; }
    public bool? MtaProtected { get => _mtaProtected; set => _mtaProtected = value; }
    public string[] Projects { get => Trim(_projects); set => _projects = value; }
    public string[] Studies { get => Trim(_studies); set => _studies = value; }

    public ClinicalDataModel ClinicalData { get; set; }
    public TreatmentModel[] Treatments { get; set; }


    private static string[] Trim(string[] array)
    {
        return array?.Select(value => value.Trim()).ToArray();
    }
}
