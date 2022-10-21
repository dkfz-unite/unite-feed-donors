using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Extensions;
using Unite.Data.Utilities.Mutations;
using Unite.Indices.Entities.Basic.Genome;
using Unite.Indices.Entities.Basic.Genome.Variants;

using CNV = Unite.Data.Entities.Genome.Variants.CNV;
using SSM = Unite.Data.Entities.Genome.Variants.SSM;
using SV = Unite.Data.Entities.Genome.Variants.SV;

namespace Unite.Donors.Indices.Services.Mappers;

internal class VariantIndexMapper
{
    internal void Map(in Variant entity, VariantIndex index)
    {
        if (entity == null)
        {
            return;
        }

        if (entity is SSM.Variant mutation)
        {
            Map(mutation, index);
        }
        else if (entity is CNV.Variant copyNumberVariant)
        {
            Map(copyNumberVariant, index);
        }
        else if (entity is SV.Variant structuralVariant)
        {
            Map(structuralVariant, index);
        }
    }

    internal void Map(in SSM.Variant entity, VariantIndex index)
    {
        if (entity == null)
        {
            return;
        }

        index.Mutation = CreateFrom(entity);
        index.AffectedTranscripts = CreateFrom(entity.AffectedTranscripts);
    }

    internal void Map(in CNV.Variant entity, VariantIndex index)
    {
        if (entity == null)
        {
            return;
        }

        index.CopyNumberVariant = CreateFrom(entity);
        index.AffectedTranscripts = CreateFrom(entity.AffectedTranscripts);
    }

    internal void Map(in SV.Variant entity, VariantIndex index)
    {
        if (entity == null)
        {
            return;
        }

        index.StructuralVariant = CreateFrom(entity);
        index.AffectedTranscripts = CreateFrom(entity.AffectedTranscripts);
    }


    private static MutationIndex CreateFrom(in SSM.Variant entity)
    {
        if (entity == null)
        {
            return null;
        }

        var index = new MutationIndex();

        index.Id = entity.Id;
        index.Chromosome = entity.ChromosomeId.ToDefinitionString();
        index.Start = entity.Start;
        index.End = entity.End;
        index.Type = entity.TypeId.ToDefinitionString();
        index.Ref = entity.ReferenceBase;
        index.Alt = entity.AlternateBase;

        return index;
    }

    private static CopyNumberVariantIndex CreateFrom(in CNV.Variant entity)
    {
        if (entity == null)
        {
            return null;
        }

        var index = new CopyNumberVariantIndex();

        index.Id = entity.Id;
        index.Chromosome = entity.ChromosomeId.ToDefinitionString();
        index.Start = entity.Start;
        index.End = entity.End;
        index.SvType = entity.SvTypeId.ToDefinitionString();
        index.CnaType = entity.CnaTypeId.ToDefinitionString();
        index.Loh = entity.Loh;
        index.HomoDel = entity.HomoDel;
        index.C1Mean = entity.C1Mean;
        index.C2Mean = entity.C2Mean;
        index.TcnMean = entity.TcnMean;
        index.C1 = entity.C1;
        index.C2 = entity.C2;
        index.Tcn = entity.Tcn;
        index.DhMax = entity.DhMax;

        return index;
    }

    private static StructuralVariantIndex CreateFrom(in SV.Variant entity)
    {
        if (entity == null)
        {
            return null;
        }

        var index = new StructuralVariantIndex();

        index.Id = entity.Id;
        index.Chromosome = entity.ChromosomeId.ToDefinitionString();
        index.Start = entity.Start;
        index.End = entity.End;
        index.NewChromosome = entity.NewChromosomeId.ToDefinitionString();
        index.NewStart = entity.NewStart;
        index.NewEnd = entity.NewEnd;
        index.Type = entity.TypeId.ToDefinitionString();
        index.Ref = entity.ReferenceBase;
        index.Alt = entity.AlternateBase;

        return index;
    }

    private static AffectedTranscriptIndex[] CreateFrom(in IEnumerable<SSM.AffectedTranscript> entities)
    {
        if (entities == null || !entities.Any())
        {
            return null;
        }

        var indices = entities.Select(entity =>
        {
            var index = new AffectedTranscriptIndex();

            index.Gene = CreateFrom(entity.Feature.Gene);
            index.Feature = CreateFrom(entity.Feature);
            index.Consequences = CreateFrom(entity.Consequences);

            index.AminoAcidChange = AAChangeCodeGenerator.Generate(
                entity.ProteinStart,
                entity.ProteinEnd,
                entity.AminoAcidChange
            );

            index.CodonChange = CodonChangeCodeGenerator.Generate(
                entity.CDSStart,
                entity.CDSEnd,
                entity.CodonChange
            );

            return index;

        }).ToArray();

        return indices;
    }

    private static AffectedTranscriptIndex[] CreateFrom(in IEnumerable<CNV.AffectedTranscript> entities)
    {
        if (entities == null || !entities.Any())
        {
            return null;
        }

        var indices = entities.Select(entity =>
        {
            var index = new AffectedTranscriptIndex();

            index.Gene = CreateFrom(entity.Feature.Gene);
            index.Feature = CreateFrom(entity.Feature);
            index.Consequences = CreateFrom(entity.Consequences);

            index.OverlapBpNumber = entity.OverlapBpNumber;
            index.OverlapPercentage = entity.OverlapPercentage;
            index.Distance = entity.Distance;

            return index;

        }).ToArray();

        return indices;
    }

    private static AffectedTranscriptIndex[] CreateFrom(in IEnumerable<SV.AffectedTranscript> entities)
    {
        if (entities == null || !entities.Any())
        {
            return null;
        }

        var indices = entities.Select(entity =>
        {
            var index = new AffectedTranscriptIndex();

            index.Gene = CreateFrom(entity.Feature.Gene);
            index.Feature = CreateFrom(entity.Feature);
            index.Consequences = CreateFrom(entity.Consequences);

            index.OverlapBpNumber = entity.OverlapBpNumber;
            index.OverlapPercentage = entity.OverlapPercentage;
            index.Distance = entity.Distance;

            return index;

        }).ToArray();

        return indices;
    }

    private static ConsequenceIndex[] CreateFrom(in IEnumerable<Consequence> entities)
    {
        if (entities == null || !entities.Any())
        {
            return null;
        }

        var indices = entities.Select(entity =>
        {
            var index = new ConsequenceIndex();

            index.Type = entity.Type;
            index.Impact = entity.Impact;
            index.Severity = entity.Severity;

            return index;

        }).ToArray();

        return indices;
    }

    private static GeneIndex CreateFrom(in Gene entity)
    {
        if (entity == null)
        {
            return null;
        }

        var index = new GeneIndex();

        index.Id = entity.Id;
        index.Symbol = entity.Symbol;
        index.Biotype = entity.Biotype;
        index.Chromosome = entity.ChromosomeId.ToDefinitionString();
        index.Start = entity.Start;
        index.End = entity.End;
        index.Strand = entity.Strand;

        index.EnsemblId = entity.Info?.EnsemblId;

        return index;
    }

    private static TranscriptIndex CreateFrom(in Transcript entity)
    {
        if (entity == null)
        {
            return null;
        }

        var index = new TranscriptIndex();

        index.Id = entity.Id;
        index.Symbol = entity.Symbol;
        index.Biotype = entity.Biotype;
        index.Chromosome = entity.ChromosomeId.ToDefinitionString();
        index.Start = entity.Start;
        index.End = entity.End;
        index.Strand = entity.Strand;

        index.EnsemblId = entity.Info?.EnsemblId;

        index.Protein = CreateFrom(entity.Protein);

        return index;
    }

    private static ProteinIndex CreateFrom(in Protein entity)
    {
        if (entity == null)
        {
            return null;
        }

        var index = new ProteinIndex();

        index.Id = entity.Id;
        index.Symbol = entity.Symbol;
        index.Start = entity.Start;
        index.End = entity.End;
        index.Length = entity.Length;

        index.EnsemblId = entity.Info?.EnsemblId;

        return index;
    }
}
