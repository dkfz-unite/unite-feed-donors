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
            .Map(entity => entity.MtaProtected, "mta_protected")
            .Map(entity => entity.ClinicalData.Diagnosis, "diagnosis");

        var model = TsvReader.Read(tsv, map).ToArray();

        bindingContext.Result = ModelBindingResult.Success(model);
    }
}

