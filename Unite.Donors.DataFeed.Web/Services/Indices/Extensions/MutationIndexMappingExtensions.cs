using Unite.Data.Entities.Extensions;
using Unite.Data.Entities.Mutations;
using Unite.Indices.Entities.Basic.Mutations;

namespace Unite.Donors.DataFeed.Web.Services.Indices.Extensions
{
    public static class MutationIndexMappingExtensions
    {
        public static void MapFrom(this MutationIndex index, in Mutation mutation)
        {
            if (mutation is null)
            {
                return;
            }

            index.Id = mutation.Id;
            index.Code = mutation.Code;
            index.Chromosome = mutation.ChromosomeId.ToDefinitionString();
            index.SequenceType = mutation.SequenceTypeId.ToDefinitionString();
            index.Start = mutation.Start;
            index.End = mutation.End;
            index.Type = mutation.TypeId.ToDefinitionString();
            index.Ref = mutation.ReferenceBase;
            index.Alt = mutation.AlternateBase;
        }
    }
}
