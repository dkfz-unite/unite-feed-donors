using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
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
    private readonly VariantIndexMapper _variantIndexMapper;



    public DonorIndexCreationService(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _donorIndexMapper = new DonorIndexMapper();
        _imageIndexMapper = new ImageIndexMapper();
        _specimenIndexMapper = new SpecimenIndexMapper();
        _variantIndexMapper = new VariantIndexMapper();
    }


    public DonorIndex CreateIndex(object key)
    {
        var donorId = (int)key;

        return CreateDonorIndex(donorId);
    }


    private DonorIndex CreateDonorIndex(int donorId)
    {
        var donor = LoadDonor(donorId);

        var index = CreateDonorIndex(donor);

        return index;
    }

    private DonorIndex CreateDonorIndex(Donor donor)
    {
        if (donor == null)
        {
            return null;
        }

        var index = new DonorIndex();

        var diagnosisDate = donor.ClinicalData?.DiagnosisDate;

        _donorIndexMapper.Map(donor, index);

        index.Images = CreateImageIndices(donor.Id, diagnosisDate);

        index.Specimens = CreateSpecimenIndices(donor.Id, diagnosisDate);

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

        index.Variants = CreateVariantIndices(specimen.Id);

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


    private VariantIndex[] CreateVariantIndices(int specimenId)
    {
        var mutations = LoadVariants<SSM.Variant, SSM.VariantOccurrence>(specimenId);
        var copyNumberVariants = LoadVariants<CNV.Variant, CNV.VariantOccurrence>(specimenId);
        var structuralVariants = LoadVariants<SV.Variant, SV.VariantOccurrence>(specimenId);

        var indices = new List<VariantIndex>();

        if (mutations != null)
        {
            indices.AddRange(mutations.Select(variant => CreateVariantIndex(variant)));
        }

        if (copyNumberVariants != null)
        {
            indices.AddRange(copyNumberVariants.Select(variant => CreateVariantIndex(variant)));
        }

        if (structuralVariants != null)
        {
            indices.AddRange(structuralVariants.Select(variant => CreateVariantIndex(variant)));
        }

        return indices.Any() ? indices.ToArray() : null;
    }

    private VariantIndex CreateVariantIndex<TVariant>(TVariant variant)
        where TVariant : Variant
    {
        var index = new VariantIndex();

        _variantIndexMapper.Map(variant, index);

        return index;
    }

    private TVariant[] LoadVariants<TVariant, TVariantOccurrence>(int specimenId)
        where TVariant : Variant
        where TVariantOccurrence : VariantOccurrence<TVariant>
    {
        var variantIds = _dbContext.Set<TVariantOccurrence>()
            .Where(occurrence => occurrence.AnalysedSample.Sample.SpecimenId == specimenId)
            .Select(occurrence => occurrence.VariantId)
            .Distinct()
            .ToArray();

        var variants = _dbContext.Set<TVariant>()
            .IncludeAffectedTranscripts()
            .Where(variant => variantIds.Contains(variant.Id))
            .ToArray();

        return variants;
    }
}
