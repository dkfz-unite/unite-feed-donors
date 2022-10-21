using Unite.Data.Extensions;

namespace Unite.Donors.Feed.Web.Services.Donors;

public class DonorModel
{
    private string _id;
    private bool? _mtaProtected;

    public string Id { get => _id?.Trim(); set => _id = value; }
    public bool? MtaProtected { get => _mtaProtected; set => _mtaProtected = value; }

    public ClinicalDataModel ClinicalData { get; set; }

    public TreatmentModel[] Treatments { get; set; }
    public string[] Projects { get; set; }
    public string[] Studies { get; set; }
}
