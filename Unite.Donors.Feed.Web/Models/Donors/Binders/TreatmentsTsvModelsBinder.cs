using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Unite.Essentials.Tsv;

namespace Unite.Donors.Feed.Web.Models.Donors.Binders;

public class TreatmentsTsvModelsBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);

        var tsv = await reader.ReadToEndAsync();

        var map = new ClassMap<TreatmentModel>()
            .Map(entity => entity.DonorId, "donor_id")
            .Map(entity => entity.Therapy, "therapy")
            .Map(entity => entity.Details, "details")
            .Map(entity => entity.StartDate, "start_date")
            .Map(entity => entity.StartDay, "start_day")
            .Map(entity => entity.EndDate, "end_date")
            .Map(entity => entity.DurationDays, "duration_days")
            .Map(entity => entity.Results, "results");

        var treatmentModels = TsvReader.Read(tsv, map).ToArray();

        var treatmentsModels = treatmentModels
            .GroupBy(treatment => treatment.DonorId)
            .Select(group => new TreatmentsModel
            {
                DonorId = group.Key,
                Entries = group.ToArray()
            })
            .ToArray();

        bindingContext.Result = ModelBindingResult.Success(treatmentsModels);
    }
}

public class TreatmentModel : Base.TreatmentModel
{
    private string _donorId;

    [JsonPropertyName("donor_id")]
    public string DonorId { get => _donorId?.Trim(); set => _donorId = value; }
}
