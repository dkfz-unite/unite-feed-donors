namespace Unite.Donors.Feed.Web.Models.Donors.Converters;

public class TreatmentsModelConverter
{
    private readonly Base.Converters.TreatmentModelConverter _treatmentModelConverter = new();

    public Data.Models.DonorModel Convert(TreatmentsModel source)
    {
        var target = new Data.Models.DonorModel();

        Map(source, target);

        return target;
    }


    private void Map(in TreatmentsModel source, in Data.Models.DonorModel target)
    {
        target.ReferenceId = source.DonorId;
        target.Treatments = source.Entries.Select(_treatmentModelConverter.Convert).ToArray();
    }
}
