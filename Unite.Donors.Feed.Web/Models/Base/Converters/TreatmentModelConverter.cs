namespace Unite.Donors.Feed.Web.Models.Base.Converters;

public class TreatmentModelConverter
{
    public Data.Models.TreatmentModel Convert(TreatmentModel source)
    {
        if (source == null)
            return null;

        var target = new Data.Models.TreatmentModel();

        Map(source, ref target);

        return target;
    }

    private static void Map(in TreatmentModel source, ref Data.Models.TreatmentModel target)
    {
        target.Therapy = source.Therapy;
        target.Details = source.Details;
        target.StartDate = source.StartDate;
        target.StartDay = source.StartDay;
        target.EndDate = source.EndDate;
        target.DurationDays = source.DurationDays;
        target.Results = source.Results;
    }
}
