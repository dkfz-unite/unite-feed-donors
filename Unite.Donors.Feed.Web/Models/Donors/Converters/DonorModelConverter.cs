using Unite.Essentials.Extensions;

namespace Unite.Donors.Feed.Web.Models.Donors.Converters;

public class DonorModelConverter
{
    private readonly Base.Converters.ClinicalDataModelConverter _clinicalDataModelConverter = new();
    private readonly Base.Converters.TreatmentModelConverter _treatmentModelConverter = new();

    public Data.Models.DonorModel Convert(DonorModel source)
    {
        var target = new Data.Models.DonorModel();

        Map(source, target);

        return target;
    }


    private void Map(in DonorModel source, in Data.Models.DonorModel target)
    {
        target.ReferenceId = source.Id;
        target.MtaProtected = source.MtaProtected;
        target.Projects = source.Projects;
        target.Studies = source.Studies;
        target.ClinicalData = _clinicalDataModelConverter.Convert(source.ClinicalData);
        target.Treatments = source.Treatments?.Select(_treatmentModelConverter.Convert).ToArrayOrNull();
    }
}
