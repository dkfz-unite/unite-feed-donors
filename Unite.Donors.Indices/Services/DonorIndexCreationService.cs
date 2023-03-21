using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome.Transcriptomics;
using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Services;
using Unite.Data.Services.Extensions;
using Unite.Donors.Indices.Services.Mappers;
using Unite.Indices.Entities.Donors;
using Unite.Indices.Services;

using CNV = Unite.Data.Entities.Genome.Variants.CNV;
using SSM = Unite.Data.Entities.Genome.Variants.SSM;
using SV = Unite.Data.Entities.Genome.Variants.SV;

namespace Unite.Donors.Indices.Services;

public class DonorIndexCreationService : IIndexCreationService<DonorIndex>
{
    private readonly DomainDbContext _dbContext;
    private readonly DonorIndexMapper _donorIndexMapper;
    private readonly ImageIndexMapper _imageIndexMapper;
    private readonly SpecimenIndexMapper _specimenIndexMapper;


    public DonorIndexCreationService(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _donorIndexMapper = new DonorIndexMapper();
        _imageIndexMapper = new ImageIndexMapper();
        _specimenIndexMapper = new SpecimenIndexMapper();
    }


    public DonorIndex CreateIndex(object key)
    {
        var donorId = (int)key;

        return CreateDonorIndex(donorId);
    }


    private DonorIndex CreateDonorIndex(int donorId)
    {
        var donor = LoadDonor(donorId);

        if (donor == null)
        {
            return null;
        }

        var index = CreateDonorIndex(donor);

        return index;
    }

    private DonorIndex CreateDonorIndex(Donor donor)
    {
        var diagnosisDate = donor.ClinicalData?.DiagnosisDate;

        var stats = LoadGenomicStats(donor.Id);

        var index = new DonorIndex();

        _donorIndexMapper.Map(donor, index);

        index.Images = CreateImageIndices(donor.Id, diagnosisDate);
        index.Specimens = CreateSpecimenIndices(donor.Id, diagnosisDate);
        index.NumberOfImages = index.Images?.Length ?? 0;
        index.NumberOfSpecimens = index.Specimens?.Length ?? 0;
        index.NumberOfGenes = stats.NumberOfGenes;
        index.NumberOfMutations = stats.NumberOfMutations;
        index.NumberOfCopyNumberVariants = stats.NumberOfCopyNumberVariants;
        index.NumberOfStructuralVariants = stats.NumberOfStructuralVariants;
        index.HasGeneExpressions = stats.HasGeneExpressions;

        return index;
    }

    private Donor LoadDonor(int donorId)
    {
        var donor = _dbContext.Set<Donor>()
            .IncludeClinicalData()
            .IncludeTreatments()
            .IncludeProjects()
            .IncludeStudies()
            .FirstOrDefault(donor => donor.Id == donorId);

        return donor;
    }


    private ImageIndex[] CreateImageIndices(int donorId, DateOnly? diagnosisDate)
    {
        var images = LoadImages(donorId);

        if (images == null)
        {
            return null;
        }

        var indices = images
            .Select(image => CreateImageIndex(image, diagnosisDate))
            .ToArray();

        return indices;
    }

    private ImageIndex CreateImageIndex(Image image, DateOnly? diagnosisDate)
    {
        var index = new ImageIndex();

        _imageIndexMapper.Map(image, index, diagnosisDate);

        return index;
    }

    private Image[] LoadImages(int donorId)
    {
        var images = _dbContext.Set<Image>()
            .Include(image => image.MriImage)
            .Where(image => image.DonorId == donorId)
            .ToArray();

        return images;
    }


    private SpecimenIndex[] CreateSpecimenIndices(int donorId, DateOnly? diagnosisDate)
    {
        var specimens = LoadSpecimens(donorId);

        if (specimens == null)
        {
            return null;
        }

        var indices = specimens
            .Select(specimen => CreateSpecimenIndex(specimen, diagnosisDate))
            .ToArray();

        return indices;
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen, DateOnly? diagnosisDate)
    {
        var index = new SpecimenIndex();

        _specimenIndexMapper.Map(specimen, index, diagnosisDate);

        return index;
    }

    private Specimen[] LoadSpecimens(int donorId)
    {
        var specimens = _dbContext.Set<Specimen>()
            .IncludeTissue()
            .IncludeCellLine()
            .IncludeOrganoid()
            .IncludeXenograft()
            .IncludeMolecularData()
            .IncludeDrugScreeningData()
            .Where(specimen => specimen.DonorId == donorId)
            .ToArray();

        return specimens;
    }


    private record GenomicStats(int NumberOfGenes, int NumberOfMutations, int NumberOfCopyNumberVariants, int NumberOfStructuralVariants, bool HasGeneExpressions);

    private GenomicStats LoadGenomicStats(int donorId)
    {
        var specimenIds = LoadSpecimenIds(donorId);
        var ssmIds = LoadVariantIds<SSM.Variant, SSM.VariantOccurrence>(specimenIds);
        var cnvIds = LoadVariantIds<CNV.Variant, CNV.VariantOccurrence>(specimenIds);
        var svIds = LoadVariantIds<SV.Variant, SV.VariantOccurrence>(specimenIds);
        var ssmGeneIds = LoadGeneIds<SSM.Variant, SSM.AffectedTranscript>(ssmIds);
        var cnvGeneIds = LoadGeneIds<CNV.Variant, CNV.AffectedTranscript>(ssmIds);
        var svGeneIds = LoadGeneIds<SV.Variant, SV.AffectedTranscript>(ssmIds);
        var geneIds = ssmGeneIds.Union(cnvGeneIds).Union(svGeneIds).ToArray();
        var hasGeneExpressions = CheckGeneExpressions(specimenIds);

        return new GenomicStats(geneIds.Length, ssmIds.Length, cnvIds.Length, svIds.Length, hasGeneExpressions);
    }

    private int[] LoadSpecimenIds(int donorId)
    {
        var ids = _dbContext.Set<Specimen>()
            .Where(specimen => specimen.DonorId == donorId)
            .Select(specimen => specimen.Id)
            .Distinct()
            .ToArray();

        return ids;
    }

    private int[] LoadGeneIds<TVariant, TAffectedTranscript>(long[] variantIds)
        where TVariant : Variant
        where TAffectedTranscript : VariantAffectedFeature<TVariant, Data.Entities.Genome.Transcript>
    {
        var ids = _dbContext.Set<TAffectedTranscript>()
            .Where(affectedTranscript => variantIds.Contains(affectedTranscript.VariantId))
            .Select(affectedTranscript => affectedTranscript.Feature.GeneId.Value)
            .Distinct()
            .ToArray();

        return ids;
    }

    private long[] LoadVariantIds<TVariant, TVariantOccurrence>(int[] specimenIds)
        where TVariant : Variant
        where TVariantOccurrence : VariantOccurrence<TVariant>
    {
        var ids = _dbContext.Set<TVariantOccurrence>()
            .Where(occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId))
            .Select(occurrence => occurrence.VariantId)
            .Distinct()
            .ToArray();

        return ids;
    }

    private bool CheckGeneExpressions(int[] specimenIds)
    {
        var hasExpressions = _dbContext.Set<GeneExpression>()
            .Any(expression => specimenIds.Contains(expression.AnalysedSample.Sample.SpecimenId));

        return hasExpressions;
    }
}
