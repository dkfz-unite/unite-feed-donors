using Microsoft.AspNetCore.Mvc.ModelBinding;
using Unite.Essentials.Tsv;

namespace Unite.Donors.Feed.Web.Models.Donors.Binders;

public class TreatmentsTsvModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);

        var tsv = await reader.ReadToEndAsync();

        var map = new ClassMap<TreatmentStandaloneModel>()
            .Map(entity => entity.DonorId, "donor_id")
            .Map(entity => entity.Therapy, "therapy")
            .Map(entity => entity.Details, "details")
            .Map(entity => entity.StartDate, "start_date")
            .Map(entity => entity.StartDay, "start_day")
            .Map(entity => entity.EndDate, "end_date")
            .Map(entity => entity.DurationDays, "duration_days")
            .Map(entity => entity.Results, "results");

        var model = TsvReader.Read(tsv, map).ToArray();

        bindingContext.Result = ModelBindingResult.Success(model);
    }
}

