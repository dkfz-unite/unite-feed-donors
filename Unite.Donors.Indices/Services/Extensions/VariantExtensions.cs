using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SSM = Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;

namespace Unite.Donors.Indices.Services.Extensions;

internal static class VariantExtensions
{
    public static Data.Entities.Genome.Analysis.Dna.Effect GetMostSevereEffect(this SSM.Variant variant)
    {
        return variant.AffectedTranscripts?.GetMostSevere().Effects.GetMostSevere();
    }

    public static SSM.AffectedTranscript GetMostSevere(this ICollection<SSM.AffectedTranscript> entries)
    {
        return entries.OrderBy(entry => entry.Effects.GetMostSevere().Severity).First();
    }

    public static Data.Entities.Genome.Analysis.Dna.Effect GetMostSevere(this ICollection<Data.Entities.Genome.Analysis.Dna.Effect> entries)
    {
        return entries.OrderBy(effect => effect.Severity).First();
    }
}
