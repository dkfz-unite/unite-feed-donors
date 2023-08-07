namespace Unite.Donors.Feed.Web.Services.Donors;

public class TreatmentTsvModel
{
    public string DonorId { get; set; }
    public string Therapy { get; set; }
    public string Details { get; set; }
    public DateTime? StartDate { get; set; }
    public int? StartDay { get; set; }
    public DateTime? EndDate { get; set; }
    public int? DurationDays { get; set; }
    public string Results { get; set; }
}

