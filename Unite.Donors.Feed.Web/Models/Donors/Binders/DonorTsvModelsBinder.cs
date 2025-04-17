using Microsoft.AspNetCore.Mvc.ModelBinding;
using Unite.Essentials.Tsv;
using Unite.Essentials.Tsv.Converters;

namespace Unite.Donors.Feed.Web.Models.Donors.Binders;

public class DonorTsvModelsBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);

        var tsv = await reader.ReadToEndAsync();
        var arrayConverter = new ArrayConverter();

        var map = new ClassMap<DonorModel>()
            .Map(entity => entity.Id, "id")
            .Map(entity => entity.MtaProtected, "mta")
            .Map(entity => entity.Studies, "studies", arrayConverter)
            .Map(entity => entity.Projects, "projects", arrayConverter)
            .Map(entity => entity.ClinicalData.Sex, "sex")
            .Map(entity => entity.ClinicalData.EnrollmentAge, "age")
            .Map(entity => entity.ClinicalData.Diagnosis, "diagnosis")
            .Map(entity => entity.ClinicalData.EnrollmentDate, "diagnosis_date")
            .Map(entity => entity.ClinicalData.PrimarySite, "primary_site")
            .Map(entity => entity.ClinicalData.Localization, "localization")
            .Map(entity => entity.ClinicalData.VitalStatus, "vital_status")
            .Map(entity => entity.ClinicalData.VitalStatusChangeDate, "vital_status_change_date")
            .Map(entity => entity.ClinicalData.VitalStatusChangeDay, "vital_status_change_day")
            .Map(entity => entity.ClinicalData.ProgressionStatus, "progression_status")
            .Map(entity => entity.ClinicalData.ProgressionStatusChangeDate, "progression_status_change_date")
            .Map(entity => entity.ClinicalData.ProgressionStatusChangeDay, "progression_status_change_day")
            .Map(entity => entity.ClinicalData.Kps, "kps_baseline")
            .Map(entity => entity.ClinicalData.SteroidsReactive, "steroids_baseline");

        var model = TsvReader.Read(tsv, map).ToArray();

        bindingContext.Result = ModelBindingResult.Success(model);
    }
}

internal class ArrayConverter : IConverter<string[]>
{
    public object Convert(string value, string row)
    {
        if (string.IsNullOrEmpty(value))
            return null;
        
        return value.Split(',', StringSplitOptions.RemoveEmptyEntries);
    }

    public string Convert(object value, object row)
    {
        throw new NotImplementedException();
    }
}
