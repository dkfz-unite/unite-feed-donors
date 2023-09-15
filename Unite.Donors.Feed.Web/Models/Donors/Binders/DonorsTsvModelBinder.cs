using Microsoft.AspNetCore.Mvc.ModelBinding;
using Unite.Essentials.Tsv;

namespace Unite.Donors.Feed.Web.Models.Donors.Binders;

public class DonorsTsvModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);

        var tsv = await reader.ReadToEndAsync();

        var map = new ClassMap<DonorModel>()
            .Map(entity => entity.Id, "id")
            .Map(entity => entity.MtaProtected, "mta")
            .Map(entity => entity.Studies, "studies")
            .Map(entity => entity.Projects, "work_packages")
            .Map(entity => entity.ClinicalData.Gender, "gender")
            .Map(entity => entity.ClinicalData.Age, "age")
            .Map(entity => entity.ClinicalData.Diagnosis, "diagnosis")
            .Map(entity => entity.ClinicalData.DiagnosisDate, "diagnosis_date")
            .Map(entity => entity.ClinicalData.PrimarySite, "primary_site")
            .Map(entity => entity.ClinicalData.Localization, "localization")
            .Map(entity => entity.ClinicalData.VitalStatus, "vital_status")
            .Map(entity => entity.ClinicalData.VitalStatusChangeDate, "vital_status_change_date")
            .Map(entity => entity.ClinicalData.VitalStatusChangeDay, "vital_status_change_day")
            .Map(entity => entity.ClinicalData.ProgressionStatus, "progression_status")
            .Map(entity => entity.ClinicalData.ProgressionStatusChangeDate, "progression_status_change_date")
            .Map(entity => entity.ClinicalData.ProgressionStatusChangeDay, "progression_status_change_day")
            .Map(entity => entity.ClinicalData.KpsBaseline, "kps_baseline")
            .Map(entity => entity.ClinicalData.SteroidsBaseline, "steroids_baseline");

        var model = TsvReader.Read(tsv, map).ToArray();

        bindingContext.Result = ModelBindingResult.Success(model);
    }
}

