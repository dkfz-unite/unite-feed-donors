namespace Unite.Donors.Feed.Web.Models.Converters;

public class TreatmentDataFlatModelsConverter
{
    private readonly Base.Converters.TreatmentModelsConverter _treatmentModelsConverter = new();

    public Data.Models.DonorModel[] Convert(in TreatmentDataFlatModel[] source)
    {
        if (source == null)
            return null;

        return source
            .GroupBy(model => model.DonorId)
            .Select(group => new Data.Models.DonorModel
                {
                    ReferenceId = group.Key,
                    Treatments = _treatmentModelsConverter.Convert(group.ToArray())
                })
            .ToArray();
    }
}
