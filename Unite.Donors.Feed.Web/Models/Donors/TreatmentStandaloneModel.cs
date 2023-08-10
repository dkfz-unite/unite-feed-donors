namespace Unite.Donors.Feed.Web.Services.Donors;

public class TreatmentStandaloneModel : TreatmentModel
{
    private string _donorId;

    public string DonorId { get => _donorId?.Trim(); set => _donorId = value; }
}
