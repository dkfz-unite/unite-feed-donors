namespace Unite.Donors.Feed.Web.Models.Base.Converters;

public class TreatmentModelsConverter
{
    public Data.Models.TreatmentModel Convert(in TreatmentModel source)
    {
        if (source == null)
        {
            return null;
        }

        return new Data.Models.TreatmentModel
        {
            Therapy = source.Therapy,
            Details = source.Details,
            StartDate = source.StartDate,
            StartDay = source.StartDay,
            EndDate = source.EndDate,
            DurationDays = source.DurationDays,
            Results = source.Results
        };
    }

    public Data.Models.TreatmentModel[] Convert(in TreatmentModel[] source)
    {
        if (source == null)
        {
            return null;
        }

        return source.Select(model => Convert(model)).ToArray();
    }
}
