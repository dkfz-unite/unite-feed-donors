using CNV = Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using SM = Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using SV = Unite.Data.Entities.Omics.Analysis.Dna.Sv;

namespace Unite.Donors.Indices.Services.Extensions;

internal static class VariantExtensions
{
    public static Data.Entities.Omics.Analysis.Dna.Effect GetMostSevereEffect(this SM.Variant variant)
    {
        return variant.AffectedTranscripts?.GetMostSevere().Effects.GetMostSevere();
    }

    public static SM.AffectedTranscript GetMostSevere(this ICollection<SM.AffectedTranscript> entries)
    {
        return entries.OrderBy(entry => entry.Effects.GetMostSevere().Severity).First();
    }

    public static Data.Entities.Omics.Analysis.Dna.Effect GetMostSevere(this ICollection<Data.Entities.Omics.Analysis.Dna.Effect> entries)
    {
        return entries.OrderBy(effect => effect.Severity).First();
    }
}
