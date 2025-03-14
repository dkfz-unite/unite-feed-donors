using Unite.Data.Entities.Donors;
using Unite.Indices.Entities.Basic.Projects;

namespace Unite.Donors.Indices.Services.Mapping;

public class ProjectIndexMapper
{
    /// <summary>
    /// Creates an index from the entity. Returns null if entity is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in Project entity) where T : ProjectIndex, new()
    {
        if (entity == null)
        {
            return null;
        }

        var index = new T();

        Map(entity, index);

        return index;
    }

    /// <summary>
    /// Maps entity to index. Does nothing if either entity or index is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="index">Index.</param>
    public static void Map(in Project entity, ProjectIndex index)
    {
        if (entity == null || index == null)
        {
            return;
        }

        index.Id = entity.Id;
        index.Name = entity.Name;
    }
}
