using Unite.Data.Entities.Base;
using Unite.Indices.Entities.Basic.Analysis;

namespace Unite.Donors.Indices.Services.Mapping;

public class ResourceIndexMapper
{
    /// <summary>
    /// Creates an index from the entity. Returns null if entity is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in SampleResource entity) where T : ResourceIndex, new()
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
    public static void Map(in SampleResource entity, ResourceIndex index)
    {
        if (entity == null || index == null)
        {
            return;
        }

        index.Id = entity.Id;
        index.Name = entity.Name;
        index.Type = entity.Type;
        index.Format = entity.Format;
        index.Archive = entity.Archive;
        index.Url = entity.Url;
    }
}
