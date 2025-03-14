using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Extensions.Queryable;
using Unite.Data.Context.Repositories;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Genome.Analysis.Dna;
using Unite.Data.Entities.Genome.Analysis.Rna;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Images.Enums;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Analysis.Drugs;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Donors.Indices.Services.Mapping;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities;
using Unite.Indices.Entities.Donors;

using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SSM = Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;

namespace Unite.Donors.Indices.Services;

public class DonorIndexCreator
{
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    private readonly DonorsRepository _donorsRepository;
    private readonly SpecimensRepository _specimensRepository;


    public DonorIndexCreator(IDbContextFactory<DomainDbContext> dbContextFactory)
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
            return null;

        return CreateDonorIndex(donor);
    }

    private DonorIndex CreateDonorIndex(Donor donor)
    {
        var diagnosisDate = donor.ClinicalData?.DiagnosisDate;

        var index = DonorIndexMapper.CreateFrom<DonorIndex>(donor);

        index.Images = CreateImageIndices(donor.Id, diagnosisDate);
        index.Specimens = CreateSpecimenIndices(donor.Id, diagnosisDate);
        index.Stats = CreateStatsIndex(donor.Id);
        index.Data = CreateDataIndex(donor.Id);

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

        return images.Select(image => CreateImageIndex(image, diagnosisDate)).ToArrayOrNull();
    }

    private static ImageIndex CreateImageIndex(Image image, DateOnly? diagnosisDate)
    {
        return ImageNavIndexMapper.CreateFrom<ImageIndex>(image, diagnosisDate);
    }

    private Image[] LoadImages(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var imageIds = _donorsRepository.GetRelatedImages([donorId]).Result;

        return dbContext.Set<Image>()
            .AsNoTracking()
            .Where(image => imageIds.Contains(image.Id))
            .ToArray();
    }


    private SpecimenIndex[] CreateSpecimenIndices(int donorId, DateOnly? diagnosisDate)
    {
        var specimens = LoadSpecimens(donorId);

        return specimens.Select(specimen => CreateSpecimenIndex(specimen, diagnosisDate)).ToArrayOrNull();
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen, DateOnly? diagnosisDate)
    {
        var index =  SpecimenNavIndexMapper.CreateFrom<SpecimenIndex>(specimen, diagnosisDate);

        index.Samples = CreateSampleIndices(specimen.Id, diagnosisDate);

        return index;
    }

    private Specimen[] LoadSpecimens(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var specimenIds = _donorsRepository.GetRelatedSpecimens([donorId]).Result;

        return dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(specimen => specimenIds.Contains(specimen.Id))
            .ToArray();
    }


    private SampleIndex[] CreateSampleIndices(int specimenId, DateOnly? diagnosisDate)
    {
        var samples = LoadSamples(specimenId);

        return samples.Select(sample => CreateSampleIndex(sample, diagnosisDate)).ToArrayOrNull();
    }

    private SampleIndex CreateSampleIndex(Sample sample, DateOnly? diagnosisDate)
    {
        var index = SampleIndexMapper.CreateFrom<SampleIndex>(sample, diagnosisDate);

        var ssm = CheckSampleVariants<SSM.Variant, SSM.VariantEntry>(sample.Id);
        var cnv = CheckSampleVariants<CNV.Variant, CNV.VariantEntry>(sample.Id);
        var sv = CheckSampleVariants<SV.Variant, SV.VariantEntry>(sample.Id);
        var meth = CheckSampleMethylation(sample.Id);
        var exp = CheckSampleGeneExp(sample.Id);
        var expSc = CheckSampleGeneExpSc(sample.Id);

        if (ssm || cnv || sv || meth || exp || expSc)
        {
            index.Data = new Unite.Indices.Entities.Basic.Analysis.SampleDataIndex
            {
                Ssm = ssm,
                Cnv = cnv,
                Sv = sv,
                Meth = meth,
                Exp = exp,
                ExpSc = expSc
            };
        }

        index.Resources = sample.Resources?.Select(resource => ResourceIndexMapper.CreateFrom<Unite.Indices.Entities.Basic.Analysis.ResourceIndex>(resource)).ToArray();

        return index;
    }

    private Sample[] LoadSamples(int specimenId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(sample => sample.Analysis)
            .Include(sample => sample.Resources)
            .Where(sample => sample.SpecimenId == specimenId)
            .Where(sample => sample.SsmEntries.Any() || sample.CnvEntries.Any() || sample.SvEntries.Any() || sample.GeneExpressions.Any() || sample.Resources.Any())
            .ToArray();
    }

    private bool CheckSampleVariants<TVariant, TVariantEntry>(int sampleId)
        where TVariant : Variant
        where TVariantEntry : VariantEntry<TVariant>
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<TVariantEntry>()
            .AsNoTracking()
            .Any(entity => entity.SampleId == sampleId);
    }

    private bool CheckSampleMethylation(int sampleId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<SampleResource>()
            .AsNoTracking()
            .Any(resource => resource.SampleId == sampleId && resource.Type == "dna-meth");
    }

    private bool CheckSampleGeneExp(int sampleId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<GeneExpression>()
            .AsNoTracking()
            .Any(expression => expression.SampleId == sampleId);
    }

    private bool CheckSampleGeneExpSc(int sampleId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<SampleResource>()
            .AsNoTracking()
            .Any(resource => resource.SampleId == sampleId && resource.Type == "rnasc-exp");
    }


    private StatsIndex CreateStatsIndex(int donorId)
    {
        var specimenIds = _donorsRepository.GetRelatedSpecimens([donorId]).Result;

        var geneIds = _specimensRepository.GetVariantRelatedGenes(specimenIds).Result;
        var ssmIds = _specimensRepository.GetRelatedVariants<SSM.Variant>(specimenIds).Result;
        var cnvIds = _specimensRepository.GetRelatedVariants<CNV.Variant>(specimenIds).Result;
        var svIds = _specimensRepository.GetRelatedVariants<SV.Variant>(specimenIds).Result;
        
        return new StatsIndex
        {
            Genes = geneIds.Length,
            Ssms = ssmIds.Length,
            Cnvs = cnvIds.Length,
            Svs = svIds.Length
        };
    }

    private DataIndex CreateDataIndex(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var specimenIds = _donorsRepository.GetRelatedSpecimens([donorId]).Result;

        return new DataIndex
        {
            Donors = true,
            Clinical = CheckClinicalData(donorId),
            Treatments = CheckTreatments(donorId),
            Mris = CheckImages(donorId, ImageType.MRI),
            Cts = CheckImages(donorId, ImageType.CT),
            Materials = CheckSpecimens(donorId, SpecimenType.Material),
            MaterialsMolecular = CheckMolecularData(donorId, SpecimenType.Material),
            Lines = CheckSpecimens(donorId, SpecimenType.Line),
            LinesMolecular = CheckMolecularData(donorId, SpecimenType.Line),
            LinesInterventions = CheckInterventions(donorId, SpecimenType.Line),
            LinesDrugs = CheckDrugScreenings(donorId, SpecimenType.Line),
            Organoids = CheckSpecimens(donorId, SpecimenType.Organoid),
            OrganoidsMolecular = CheckMolecularData(donorId, SpecimenType.Organoid),
            OrganoidsInterventions = CheckInterventions(donorId, SpecimenType.Organoid),
            OrganoidsDrugs = CheckDrugScreenings(donorId, SpecimenType.Organoid),
            Xenografts = CheckSpecimens(donorId, SpecimenType.Xenograft),
            XenograftsMolecular = CheckMolecularData(donorId, SpecimenType.Xenograft),
            XenograftsInterventions = CheckInterventions(donorId, SpecimenType.Xenograft),
            XenograftsDrugs = CheckDrugScreenings(donorId, SpecimenType.Xenograft),
            Ssms = CheckVariants<SSM.Variant, SSM.VariantEntry>(specimenIds),
            Cnvs = CheckVariants<CNV.Variant, CNV.VariantEntry>(specimenIds),
            Svs = CheckVariants<SV.Variant, SV.VariantEntry>(specimenIds),
            Meth = CheckMethylation(specimenIds),
            Exp = CheckGeneExp(specimenIds),
            ExpSc = CheckGeneExpSc(specimenIds)
        };
    }


    private bool CheckClinicalData(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<ClinicalData>()
            .AsNoTracking()
            .Where(clinical => clinical.DonorId == donorId)
            .Any();
    }

    private bool CheckTreatments(int donorId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Treatment>()
            .AsNoTracking()
            .Where(treatment => treatment.DonorId == donorId)
            .Any();
    }

    private bool CheckImages(int donorId, ImageType type)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Image>()
            .AsNoTracking()
            .Where(image => image.DonorId == donorId)
            .Where(image => image.TypeId == type)
            .Any();
    }

    private bool CheckSpecimens(int donorId, SpecimenType type)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Specimen>()
            .AsNoTracking()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == type)
            .Any();
    }

    private bool CheckMolecularData(int donorId, SpecimenType type)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeMolecularData()
            .Where(specimen => specimen.DonorId == donorId)
            .Where(specimen => specimen.TypeId == type)
            .Where(specimen => specimen.MolecularData != null)
            .Any();
    }

    private bool CheckInterventions(int donorId, SpecimenType type)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Intervention>()
            .AsNoTracking()
            .Where(intervention => intervention.Specimen.DonorId == donorId)
            .Where(intervention => intervention.Specimen.TypeId == type)
            .Any();
    }

    private bool CheckDrugScreenings(int donorId, SpecimenType type)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<DrugScreening>()
            .AsNoTracking()
            .Where(entry => entry.Sample.Specimen.DonorId == donorId)
            .Where(entry => entry.Sample.Specimen.TypeId == type)
            .Any();
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
            .Any(entry => specimenIds.Contains(entry.Sample.SpecimenId));
    }

    /// <summary>
    /// Checks if methylation data is available for given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen identifiers.</param>
    /// <returns>'true' if methylation data exists or 'false' otherwise.</returns>
    private bool CheckMethylation(int[] specimenIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<SampleResource>()
            .AsNoTracking()
            .Any(resource => specimenIds.Contains(resource.Sample.SpecimenId) && resource.Type == "dna-meth");
    }

    /// <summary>
    /// Checks if bulk gene expression data is available for given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen identifiers.</param>
    /// <returns>'true' if gene expression data exists or 'false' otherwise.</returns>
    private bool CheckGeneExp(int[] specimenIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<GeneExpression>()
            .AsNoTracking()
            .Any(expression => specimenIds.Contains(expression.Sample.SpecimenId));
    }

    /// <summary>
    /// Checks if single cell gene expression data is available for given specimens.
    /// </summary>
    /// <param name="specimenIds">Specimen identifiers.</param>
    /// <returns>'true' if single cell gene expression data exists or 'false' otherwise.</returns>
    private bool CheckGeneExpSc(int[] specimenIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<SampleResource>()
            .AsNoTracking()
            .Any(resource => specimenIds.Contains(resource.Sample.SpecimenId) && resource.Type == "rnasc-exp");
    }
}
