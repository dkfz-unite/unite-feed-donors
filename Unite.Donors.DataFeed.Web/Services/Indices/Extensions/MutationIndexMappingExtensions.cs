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
            index.Name = mutation.Name;
            index.Chromosome = mutation.ChromosomeId?.ToDefinitionString();
            index.Contig = mutation.Contig?.Value;
            index.SequenceType = mutation.SequenceTypeId.ToDefinitionString();
            index.Position = mutation.Position;
            index.Type = mutation.TypeId.ToDefinitionString();

            index.Gene = CreateFrom(mutation.Gene);
        }

        private static GeneIndex CreateFrom(in Gene gene)
        {
            if (gene is null)
            {
                return null;
            }

            var index = new GeneIndex();

            index.Id = gene.Id;
            index.Name = gene.Name;

            return index;
        }
    }
}
