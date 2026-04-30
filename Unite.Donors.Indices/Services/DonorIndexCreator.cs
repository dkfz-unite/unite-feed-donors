using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Repositories.Extensions.Queryable;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Images.Enums;
using Unite.Data.Entities.Omics.Analysis;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Analysis.Drugs;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Donors.Indices.Services.Mapping;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities;
using Unite.Indices.Entities.Donors;

using CNV = Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using SM = Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using SV = Unite.Data.Entities.Omics.Analysis.Dna.Sv;

namespace Unite.Donors.Indices.Services;

public class DonorIndexCreator
{
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    private readonly DonorsRepository _donorsRepository;
    private readonly SpecimensRepository _specimensRepository;
    private readonly SamplesRepository _samplesRepository;


    public DonorIndexCreator(IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _donorsRepository = new DonorsRepository(dbContextFactory);
        _specimensRepository = new SpecimensRepository(dbContextFactory);
        _samplesRepository = new SamplesRepository(dbContextFactory);
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
        var enrollmentDate = donor.ClinicalData?.EnrollmentDate;

        var index = DonorIndexMapper.CreateFrom<DonorIndex>(donor);

        index.Images = CreateImageIndices(donor.Id, enrollmentDate);
        index.Specimens = CreateSpecimenIndices(donor.Id, enrollmentDate);
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


    private ImageIndex[] CreateImageIndices(int donorId, DateOnly? enrollmentDate)
    {
        var images = LoadImages(donorId);

        return images.Select(image => CreateImageIndex(image, enrollmentDate)).ToArrayOrNull();
    }

    private static ImageIndex CreateImageIndex(Image image, DateOnly? enrollmentDate)
    {
        return ImageNavIndexMapper.CreateFrom<ImageIndex>(image, enrollmentDate);
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


    private SpecimenIndex[] CreateSpecimenIndices(int donorId, DateOnly? enrollmentDate)
    {
        var specimens = LoadSpecimens(donorId);

        return specimens.Select(specimen => CreateSpecimenIndex(specimen, enrollmentDate)).ToArrayOrNull();
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen, DateOnly? enrollmentDate)
    {
        var index =  SpecimenNavIndexMapper.CreateFrom<SpecimenIndex>(specimen, enrollmentDate);

        index.Samples = CreateSampleIndices(specimen.Id, enrollmentDate);

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


    private SampleIndex[] CreateSampleIndices(int specimenId, DateOnly? enrollmentDate)
    {
        var samples = LoadSamples(specimenId);

        return samples.Select(sample => CreateSampleIndex(sample, enrollmentDate)).ToArrayOrNull();
    }

    private SampleIndex CreateSampleIndex(Sample sample, DateOnly? enrollmentDate)
    {
        var index = SampleIndexMapper.CreateFrom<SampleIndex>(sample, enrollmentDate);

        var availability = _samplesRepository.HasRelatedOmicsResources(sample.Id).Result;

        if (availability != null)
        {
            index.Data = new Unite.Indices.Entities.Basic.Analysis.SampleDataIndex
            {
                Sm = availability.Sm,
                Cnv = availability.Cnv,
                Sv = availability.Sv,
                Cnvp = availability.Cnvp,
                Meth = availability.Meth,
                Exp = availability.GeneExp,
                ExpSc = availability.GeneExpSc,
                Prot = availability.ProtExp
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
            .Where(sample =>
                sample.SmEntries.Any() ||
                sample.CnvEntries.Any() ||
                sample.SvEntries.Any() ||
                sample.GeneExpressions.Any() ||
                sample.ProteinExpressions.Any() ||
                sample.Resources.Any())
            .ToArray();
    }

    private StatsIndex CreateStatsIndex(int donorId)
    {
        var specimenIds = _donorsRepository.GetRelatedSpecimens([donorId]).Result;

        var geneIds = _specimensRepository.GetVariantRelatedGenes(specimenIds).Result;
        var smIds = _specimensRepository.GetRelatedVariants<SM.Variant>(specimenIds).Result;
        var cnvIds = _specimensRepository.GetRelatedVariants<CNV.Variant>(specimenIds).Result;
        var svIds = _specimensRepository.GetRelatedVariants<SV.Variant>(specimenIds).Result;
        
        return new StatsIndex
        {
            Genes = geneIds.Length,
            Sms = smIds.Length,
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
            Mrs = CheckImages(donorId, ImageType.MR),
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
            Sms = _specimensRepository.HaveVariants<SM.VariantEntry, SM.Variant>(specimenIds).Result,
            Cnvs = _specimensRepository.HaveVariants<CNV.VariantEntry, CNV.Variant>(specimenIds).Result,
            Svs = _specimensRepository.HaveVariants<SV.VariantEntry, SV.Variant>(specimenIds).Result,
            Cnvps = _specimensRepository.HaveProfiles(specimenIds).Result,
            Meth = _specimensRepository.HaveMethylation(specimenIds).Result,
            Exp = _specimensRepository.HaveGeneExpressions(specimenIds).Result,
            ExpSc = _specimensRepository.HaveGeneExpressionsPerCells(specimenIds).Result,
            Prot = _specimensRepository.HaveProteinExpressions(specimenIds).Result
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
}
