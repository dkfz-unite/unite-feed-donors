
using Unite.Donors.Feed.Web.Models.Donors;

namespace Unite.Donors.Feed.Web.Models.Converters;

public class TreatmentStandaloneModelConverter
{
    public TreatmentStandaloneModel Convert(TreatmentTsvModel source)
    {
        var donorModel = new TreatmentStandaloneModel();

        Map(source, donorModel);

        return donorModel;
    }

    private static void Map(TreatmentTsvModel source, TreatmentStandaloneModel target)
    {
        target.DonorId = source.DonorId;
        target.Therapy = source.Therapy;
        target.Details = source.Details;
        target.StartDate = source.StartDate;
        target.StartDay = source.StartDay;
        target.EndDate = source.EndDate;
        target.DurationDays = source.DurationDays;
        target.Results = source.Results;
    }
}
