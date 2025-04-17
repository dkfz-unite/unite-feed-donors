using Unite.Data.Entities.Specimens;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Basic.Specimens;

namespace Unite.Donors.Indices.Services.Mapping;

public class SpecimenNavIndexMapper
{
    /// <summary>
    /// Creates an index from the entity. Returns null if entity is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="enrollmentDate">Enrollment date (anchor date for calculation of relative days).</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in Specimen entity, DateOnly? enrollmentDate) where T : SpecimenNavIndex, new()
    {
        if (entity == null)
        {
            return null;
        }

        var index = new T();

        Map(entity, index, enrollmentDate);

        return index;
    }

    /// <summary>
    /// Maps entity to index. Does nothing if either entity or index is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="index">Index.</param>
    /// <param name="enrollmentDate">Enrollment date (anchor date for calculation of relative days).</param> 
    public static void Map(in Specimen entity, SpecimenNavIndex index, DateOnly? enrollmentDate)
    {
        if (entity == null || index == null)
        {
            return;
        }

        index.Id = entity.Id;
        index.ReferenceId = entity.ReferenceId;
        index.Type = entity.TypeId.ToDefinitionString();
    }
}
