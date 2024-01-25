namespace Unite.Donors.Feed.Web.Models.Converters;

public class DonorDataModelsConverter
{
    private readonly Base.Converters.ClinicalDataModelsConverter _clinicalDataModelsConverter = new();
    private readonly Base.Converters.TreatmentModelsConverter _treatmentModelsConverter = new();

    public Data.Models.DonorModel Convert(in DonorDataModel model)
    {
        if (model == null)
            return null;

        return new Data.Models.DonorModel
        {
            ReferenceId = model.Id,
            MtaProtected = model.MtaProtected,
            WorkPackages = model.Projects,
            Studies = model.Studies,
            ClinicalData = _clinicalDataModelsConverter.Convert(model.ClinicalData),
            Treatments = _treatmentModelsConverter.Convert(model.Treatments)
        };
    }

    public Data.Models.DonorModel[] Convert(in DonorDataModel[] models)
    {
        if (models == null)
            return null;

        return models.Select(model => Convert(model)).ToArray();
    }
}
