using Unite.Donors.Feed.Web.Models.Donors;

namespace Unite.Donors.Feed.Web.Models.Converters;

public class TreatmentStandaloneModelConverter
{
    public Data.Donors.Models.DonorModel Convert(TreatmentStandaloneModel source)
    {
        var donorModel = new Data.Donors.Models.DonorModel();

        Map(source, donorModel);

        return donorModel;
    }

    private static void Map(TreatmentStandaloneModel source, Data.Donors.Models.DonorModel target)
    {
        target.ReferenceId = source.DonorId;

        var treatmentModel = new Data.Donors.Models.TreatmentModel();

        Map(source, treatmentModel);

        target.Treatments = new Data.Donors.Models.TreatmentModel[] { treatmentModel };
    }

    private static void Map(TreatmentStandaloneModel source, Data.Donors.Models.TreatmentModel target)
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
