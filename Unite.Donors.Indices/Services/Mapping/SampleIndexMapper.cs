using Unite.Data.Entities.Genome.Analysis;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Basic.Analysis;

namespace Unite.Donors.Indices.Services.Mapping;

public class SampleIndexMapper
{
    /// <summary>
    /// Creates an index from the entity. Returns null if entity is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="diagnosisDate">Diagnosis date (anchor date for calculation of relative days).</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in Sample entity, DateOnly? diagnosisDate) where T : SampleIndex, new()
    {
        if (entity == null)
        {
            return null;
        }

        var index = new T();

        Map(entity, index, diagnosisDate);

        return index;
    }

    /// <summary>
    /// Maps entity to index. Does nothing if either entity or index is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="index">Index.</param>
    /// <param name="diagnosisDate">Diagnosis date (anchor date for calculation of relative days).</param>
    public static void Map(in Sample entity, SampleIndex index, DateOnly? diagnosisDate)
    {
        if (entity == null || index == null)
        {
            return;
        }

        index.Id = entity.Id;
        index.AnalysisType = entity.Analysis.TypeId.ToDefinitionString();
        index.AnalysisDay = entity.Analysis.Day ?? entity.Analysis.Date?.RelativeFrom(diagnosisDate);
        index.Genome = entity.Genome;
        index.Purity = entity.Purity;
        index.Ploidy = entity.Ploidy;
        index.Cells = entity.Cells;
    }
}
