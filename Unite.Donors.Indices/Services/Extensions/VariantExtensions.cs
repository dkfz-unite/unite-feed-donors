using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SM = Unite.Data.Entities.Genome.Analysis.Dna.Sm;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;

namespace Unite.Donors.Indices.Services.Extensions;

internal static class VariantExtensions
{
    public static Data.Entities.Genome.Analysis.Dna.Effect GetMostSevereEffect(this SM.Variant variant)
    {
        return variant.AffectedTranscripts?.GetMostSevere().Effects.GetMostSevere();
    }

    public static SM.AffectedTranscript GetMostSevere(this ICollection<SM.AffectedTranscript> entries)
    {
        return entries.OrderBy(entry => entry.Effects.GetMostSevere().Severity).First();
    }

    public static Data.Entities.Genome.Analysis.Dna.Effect GetMostSevere(this ICollection<Data.Entities.Genome.Analysis.Dna.Effect> entries)
    {
        return entries.OrderBy(effect => effect.Severity).First();
    }
}
