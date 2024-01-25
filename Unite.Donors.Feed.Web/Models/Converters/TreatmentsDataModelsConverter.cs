namespace Unite.Donors.Feed.Web.Models.Converters;

public class TreatmentsDataModelsConverter
{
    private readonly Base.Converters.TreatmentModelsConverter _treatmentModelsConverter = new();

    public Data.Models.DonorModel Convert(in TreatmentsDataModel source)
    {
        if (source == null)
            return null;

        return new Data.Models.DonorModel
        {
            ReferenceId = source.DonorId,
            Treatments = _treatmentModelsConverter.Convert(source.Data)
        };
    }

    public Data.Models.DonorModel[] Convert(in TreatmentsDataModel[] source)
    {
        if (source == null)
            return null;

        return source.Select(model => Convert(model)).ToArray();
    }
}
