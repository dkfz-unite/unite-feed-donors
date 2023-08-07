
using Unite.Donors.Feed.Web.Services.Donors;

namespace Unite.Donors.Feed.Web.Services.Treatments.Converters;

public class TreatmentModelConverter
{
    public TreatmentModel Convert(TreatmentTsvModel source)
    {
        var donorModel = new TreatmentModel();

        Map(source, donorModel);

        return donorModel;
    }

    private static void Map(TreatmentTsvModel source, TreatmentModel target)
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
