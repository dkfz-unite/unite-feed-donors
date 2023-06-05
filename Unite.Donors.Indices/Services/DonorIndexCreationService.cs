using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Donors.Clinical;
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

        var index = new DonorIndex();

        _donorIndexMapper.Map(donor, index);

        index.Images = CreateImageIndices(donor.Id, diagnosisDate);
        index.Specimens = CreateSpecimenIndices(donor.Id, diagnosisDate);
        index.Data = CreateDataIndex(donor.Id);
        
        var stats = LoadGenomicStats(donor.Id);
        
        index.NumberOfGenes = stats.NumberOfGenes;
        index.NumberOfSsms = stats.NumberOfSsms;
        index.NumberOfCnvs = stats.NumberOfCnvs;
        index.NumberOfSvs = stats.NumberOfSvs;

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


    private DataIndex CreateDataIndex(int donorId)
    {
        var specimenIds = LoadSpecimenIds(donorId);

        var index = new DataIndex();

        index.Clinical = _dbContext.Set<ClinicalData>()
            .Where(clinical => clinical.DonorId == donorId)
            .Any();

        index.Treatments = _dbContext.Set<Treatment>()
            .Where(treatment => treatment.DonorId == donorId)
            .Any();

        index.Mris = _dbContext.Set<Image>()
            .Include(image => image.MriImage)
            .Where(image => image.DonorId == donorId)
            .Where(image => image.MriImage != null)
            .Any();

        index.Tissues = _dbContext.Set<Specimen>()
            .Include(specimen => specimen.Tissue)
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.Tissue != null)
            .Any();

        index.Cells = _dbContext.Set<Specimen>()
            .Include(specimen => specimen.CellLine)
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.CellLine != null)
            .Any();

        index.Organoids = _dbContext.Set<Specimen>()
            .Include(specimen => specimen.Organoid)
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.Organoid != null)
            .Any();

        index.Xenografts = _dbContext.Set<Specimen>()
            .Include(specimen => specimen.Xenograft)
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.Xenograft != null)
            .Any();

        index.Ssms = CheckVariants<SSM.Variant, SSM.VariantOccurrence>(specimenIds);

        index.Cnvs = CheckVariants<CNV.Variant, CNV.VariantOccurrence>(specimenIds);

        index.Svs = CheckVariants<SV.Variant, SV.VariantOccurrence>(specimenIds);

        index.GeneExp = CheckGeneExp(specimenIds);

        return index;
    }


    private record GenomicStats(int NumberOfGenes, int NumberOfSsms, int NumberOfCnvs, int NumberOfSvs);

    private GenomicStats LoadGenomicStats(int donorId)
    {
        var specimenIds = LoadSpecimenIds(donorId);

        var ssmIds = LoadVariantIds<SSM.Variant, SSM.VariantOccurrence>(specimenIds);
        var cnvIds = LoadVariantIds<CNV.Variant, CNV.VariantOccurrence>(specimenIds);
        var svIds = LoadVariantIds<SV.Variant, SV.VariantOccurrence>(specimenIds);
        var ssmGeneIds = LoadGeneIds<SSM.Variant, SSM.AffectedTranscript>(ssmIds);
        var cnvGeneIds = LoadGeneIds<CNV.Variant, CNV.AffectedTranscript>(cnvIds, affectedTranscript => affectedTranscript.Variant.TypeId != CNV.Enums.CnvType.Neutral);
        var svGeneIds = LoadGeneIds<SV.Variant, SV.AffectedTranscript>(svIds);
        var geneIds = Array.Empty<int>().Union(ssmGeneIds).Union(cnvGeneIds).Union(svGeneIds).ToArray();

        return new GenomicStats(geneIds.Length, ssmIds.Length, cnvIds.Length, svIds.Length);
    }

    /// <summary>
    /// Loads identifiers of specimens associated with given donor.
    /// </summary>
    /// <param name="donorId">Donor identifier.</param>
    /// <returns>Array of specimens identifiers.</returns>
    private int[] LoadSpecimenIds(int donorId)
    {
        var ids = _dbContext.Set<Specimen>()
            .Where(specimen => specimen.DonorId == donorId)
            .Select(specimen => specimen.Id)
            .Distinct()
            .ToArray();

        return ids;
    }

    /// <summary>
    /// Loads identifiers of genes affected by given variants.
    /// </summary>
    /// <param name="variantIds">Varians identifiers.</param>
    /// <param name="filter">Affected transcript filter.</param>
    /// <typeparam name="TVariant">Variant type.</typeparam>
    /// <typeparam name="TAffectedTranscript">Variant affected transcript type.</typeparam>
    /// <returns>Array of genes identifiers.</returns>
    private int[] LoadGeneIds<TVariant, TAffectedTranscript>(long[] variantIds, Expression<Func<TAffectedTranscript, bool>> filter = null)
        where TVariant : Variant
        where TAffectedTranscript : VariantAffectedFeature<TVariant, Data.Entities.Genome.Transcript>
    {
        Expression<Func<TAffectedTranscript, bool>> selectorPredicate = (affectedTranscript => variantIds.Contains(affectedTranscript.VariantId));
        Expression<Func<TAffectedTranscript, bool>> filterPredicate = filter ?? (affectedTranscript => true);

        var ids = _dbContext.Set<TAffectedTranscript>()
            .Where(selectorPredicate)
            .Where(filterPredicate)
            .Select(affectedTranscript => affectedTranscript.Feature.GeneId.Value)
            .Distinct()
            .ToArray();

        return ids;
    }

    /// <summary>
    /// Loads identifiers of variants of given type occurring in given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimens identifiers.</param>
    /// <param name="filter">Variant occurrence filter.</param>
    /// <typeparam name="TVariant">Variant type.</typeparam>
    /// <typeparam name="TVariantOccurrence">Variant occurrence type.</typeparam>
    /// <returns>Array of variants identifiers.</returns>
    private long[] LoadVariantIds<TVariant, TVariantOccurrence>(int[] specimenIds, Expression<Func<TVariantOccurrence, bool>> filter = null)
        where TVariant : Variant
        where TVariantOccurrence : VariantOccurrence<TVariant>
    {
        Expression<Func<TVariantOccurrence, bool>> selectorPredicate = (occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId));
        Expression<Func<TVariantOccurrence, bool>> filterPredicate = filter ?? (occurrence => true);

        var ids = _dbContext.Set<TVariantOccurrence>()
            .Where(selectorPredicate)
            .Where(filterPredicate)
            .Select(occurrence => occurrence.VariantId)
            .Distinct()
            .ToArray();

        return ids;
    }

    /// <summary>
    /// Checks if variants data of given type is available for given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen identifiers.</param>
    /// <typeparam name="TVariant">Variant type.</typeparam>
    /// <typeparam name="TVariantOccurrence">Variant occurrence type.</typeparam>
    /// <returns>'true' if variants data exists or 'false' otherwise.</returns>
    private bool CheckVariants<TVariant, TVariantOccurrence>(int[] specimenIds)
        where TVariant : Variant
        where TVariantOccurrence : VariantOccurrence<TVariant>
    {
        return _dbContext.Set<TVariantOccurrence>()
            .Where(occurrence => specimenIds.Contains(occurrence.AnalysedSample.Sample.SpecimenId))
            .Select(occurrence => occurrence.VariantId)
            .Distinct()
            .Any();
    }

    /// <summary>
    /// Checks if gene expression data is available for given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen identifiers.</param>
    /// <returns>'true' if gene expression data exists or 'false' otherwise.</returns>
    private bool CheckGeneExp(int[] specimenIds)
    {
        return _dbContext.Set<GeneExpression>()
            .Any(expression => specimenIds.Contains(expression.AnalysedSample.Sample.SpecimenId));
    }
}
