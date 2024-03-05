using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Extensions.Queryable;
using Unite.Data.Context.Repositories;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Genome.Transcriptomics;
using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Images.Enums;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Indices.Entities;
using Unite.Indices.Entities.Donors;
using Unite.Mapping;

using CNV = Unite.Data.Entities.Genome.Variants.CNV;
using SSM = Unite.Data.Entities.Genome.Variants.SSM;
using SV = Unite.Data.Entities.Genome.Variants.SV;

namespace Unite.Donors.Indices.Services;

public class DonorIndexCreationService
{
    private record GenomicStats(int NumberOfGenes, int NumberOfSsms, int NumberOfCnvs, int NumberOfSvs);

    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    private readonly DonorsRepository _donorsRepository;
    private readonly SpecimensRepository _specimensRepository;


    public DonorIndexCreationService(IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _donorsRepository = new DonorsRepository(dbContextFactory);
        _specimensRepository = new SpecimensRepository(dbContextFactory);
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

        var index = DonorIndexMapper.CreateFrom<DonorIndex>(donor);

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
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Donor>()
            .AsNoTracking()
            .IncludeClinicalData()
            .IncludeTreatments()
            .IncludeProjects()
            .IncludeStudies()
            .FirstOrDefault(donor => donor.Id == donorId);
    }


    private ImageIndex[] CreateImageIndices(int donorId, DateOnly? diagnosisDate)
    {
        var images = LoadImages(donorId);

        var indices = images.Select(image => CreateImageIndex(image, diagnosisDate));

        return indices.Any() ? indices.ToArray() : null;
    }

    private static ImageIndex CreateImageIndex(Image image, DateOnly? diagnosisDate)
    {
        return ImageIndexMapper.CreateFrom<ImageIndex>(image, diagnosisDate);
    }

    private Image[] LoadImages(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var imageIds = _donorsRepository.GetRelatedImages([donorId]).Result;

        return dbContext.Set<Image>()
            .AsNoTracking()
            .Include(image => image.MriImage)
            .Where(image => imageIds.Contains(image.Id))
            .ToArray();
    }


    private SpecimenIndex[] CreateSpecimenIndices(int donorId, DateOnly? diagnosisDate)
    {
        var specimens = LoadSpecimens(donorId);

        var indices = specimens.Select(specimen => CreateSpecimenIndex(specimen, diagnosisDate));

        return indices.Any() ? indices.ToArray() : null;
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen, DateOnly? diagnosisDate)
    {
        var index =  SpecimenIndexMapper.CreateFrom<SpecimenIndex>(specimen, diagnosisDate);

        index.Analyses = CreateAnalysisIndices(specimen.Id, diagnosisDate);

        return index;
    }

    private Specimen[] LoadSpecimens(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var specimenIds = _donorsRepository.GetRelatedSpecimens([donorId]).Result;

        return dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeMaterial()
            .IncludeLine()
            .IncludeOrganoid()
            .IncludeXenograft()
            .IncludeMolecularData()
            .IncludeInterventions()
            .IncludeDrugScreenings()
            .Where(specimen => specimenIds.Contains(specimen.Id))
            .ToArray();
    }


    private AnalysisIndex[] CreateAnalysisIndices(int specimenId, DateOnly? diagnosisDate)
    {
        var analyses = LoadAnalyses(specimenId);

        var indices = analyses.Select(analysis => CreateAnalysisIndex(analysis, diagnosisDate));

        return indices.Any() ? indices.ToArray() : null;
    }

    private static AnalysisIndex CreateAnalysisIndex(AnalysedSample analysis, DateOnly? diagnosisDate)
    {
        return AnalysisIndexMapper.CreateFrom<AnalysisIndex>(analysis, diagnosisDate);
    }

    private AnalysedSample[] LoadAnalyses(int specimenId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<AnalysedSample>()
            .AsNoTracking()
            .Include(analysedSample => analysedSample.Analysis)
            .Where(analysedSample => analysedSample.TargetSampleId == specimenId)
            .ToArray();
    }


    private DataIndex CreateDataIndex(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var specimenIds = _donorsRepository.GetRelatedSpecimens([donorId]).Result;

        var index = new DataIndex();

        index.Donors = true;

        index.Clinical = dbContext.Set<ClinicalData>()
            .AsNoTracking()
            .Where(clinical => clinical.DonorId == donorId)
            .Any();

        index.Treatments = dbContext.Set<Treatment>()
            .AsNoTracking()
            .Where(treatment => treatment.DonorId == donorId)
            .Any();

        index.Mris = dbContext.Set<Image>()
            .AsNoTracking()
            .Where(image => image.DonorId == donorId)
            .Where(image => image.TypeId == ImageType.MRI)
            .Any();

        index.Cts = dbContext.Set<Image>()
            .AsNoTracking()
            .Where(image => image.DonorId == donorId)
            .Where(image => image.TypeId == ImageType.CT)
            .Any();

        index.Materials = dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == SpecimenType.Xenograft)
            .Any();

        index.MaterialsMolecular = dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeMolecularData()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == SpecimenType.Material)
            .Where(specimen => specimen.MolecularData != null)
            .Any();

        index.Lines = dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == SpecimenType.Line)
            .Any();

        index.LinesMolecular = dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeMolecularData()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == SpecimenType.Line)
            .Where(specimen => specimen.MolecularData != null)
            .Any();

        index.LinesInterventions = dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeInterventions()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == SpecimenType.Line)
            .Where(specimen => specimen.Interventions != null && specimen.Interventions.Any())
            .Any();

        index.LinesDrugs = dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeDrugScreenings()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == SpecimenType.Line)
            .Where(specimen => specimen.DrugScreenings != null && specimen.DrugScreenings.Any())
            .Any();

        index.Organoids = dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == SpecimenType.Organoid)
            .Any();

        index.OrganoidsMolecular = dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeMolecularData()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == SpecimenType.Organoid)
            .Where(specimen => specimen.MolecularData != null)
            .Any();

        index.OrganoidsInterventions = dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeInterventions()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == SpecimenType.Organoid)
            .Where(specimen => specimen.Interventions != null && specimen.Interventions.Any())
            .Any();

        index.OrganoidsDrugs = dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeDrugScreenings()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == SpecimenType.Organoid)
            .Where(specimen => specimen.DrugScreenings != null && specimen.DrugScreenings.Any())
            .Any();

        index.Xenografts = dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == SpecimenType.Xenograft)
            .Any();

        index.XenograftsMolecular = dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeMolecularData()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == SpecimenType.Xenograft)
            .Where(specimen => specimen.MolecularData != null)
            .Any();

        index.XenograftsInterventions = dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeInterventions()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == SpecimenType.Xenograft)
            .Where(specimen => specimen.Interventions != null && specimen.Interventions.Any())
            .Any();

        index.XenograftsDrugs = dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeDrugScreenings()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == SpecimenType.Xenograft)
            .Where(specimen => specimen.DrugScreenings != null && specimen.DrugScreenings.Any())
            .Any();
        
        index.Ssms = CheckVariants<SSM.Variant, SSM.VariantEntry>(specimenIds);

        index.Cnvs = CheckVariants<CNV.Variant, CNV.VariantEntry>(specimenIds);

        index.Svs = CheckVariants<SV.Variant, SV.VariantEntry>(specimenIds);

        index.GeneExp = CheckGeneExp(specimenIds);

        index.GeneExpSc = false;

        return index;
    }

    private GenomicStats LoadGenomicStats(int donorId)
    {
        var specimenIds = _donorsRepository.GetRelatedSpecimens([donorId]).Result;

        var ssmIds = _specimensRepository.GetRelatedVariants<SSM.Variant>(specimenIds).Result;
        var cnvIds = _specimensRepository.GetRelatedVariants<CNV.Variant>(specimenIds).Result;
        var svIds = _specimensRepository.GetRelatedVariants<SV.Variant>(specimenIds).Result;
        var geneIds = _specimensRepository.GetVariantRelatedGenes(specimenIds).Result;

        return new GenomicStats(geneIds.Length, ssmIds.Length, cnvIds.Length, svIds.Length);
    }

    /// <summary>
    /// Checks if variants data of given type is available for given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen identifiers.</param>
    /// <typeparam name="TVariant">Variant type.</typeparam>
    /// <typeparam name="TVariantEntry">Variant occurrence type.</typeparam>
    /// <returns>'true' if variants data exists or 'false' otherwise.</returns>
    private bool CheckVariants<TVariant, TVariantEntry>(int[] specimenIds)
        where TVariant : Variant
        where TVariantEntry : VariantEntry<TVariant>
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<TVariantEntry>()
            .AsNoTracking()
            .Where(entry => specimenIds.Contains(entry.AnalysedSample.TargetSampleId))
            .Select(entry => entry.EntityId)
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
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<BulkExpression>()
            .AsNoTracking()
            .Any(expression => specimenIds.Contains(expression.AnalysedSample.TargetSampleId));
    }
}
