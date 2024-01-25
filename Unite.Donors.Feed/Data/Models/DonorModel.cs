namespace Unite.Donors.Feed.Data.Models;

public class DonorModel
{
    public string ReferenceId { get; set; }

    public bool? MtaProtected { get; set; }

    public string[] WorkPackages { get; set; }
    public string[] Studies { get; set; }

    public ClinicalDataModel ClinicalData { get; set; }
    public TreatmentModel[] Treatments { get; set; }
}
