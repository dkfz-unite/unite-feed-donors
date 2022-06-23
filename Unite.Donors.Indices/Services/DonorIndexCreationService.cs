using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome.Mutations;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Services;
using Unite.Data.Services.Extensions;
using Unite.Donors.Indices.Services.Mappers;
using Unite.Indices.Entities.Donors;
using Unite.Indices.Services;

namespace Unite.Donors.Indices.Services;

public class DonorIndexCreationService : IIndexCreationService<DonorIndex>
{
    private readonly DomainDbContext _dbContext;
    private readonly DonorIndexMapper _donorIndexMapper;
    private readonly ImageIndexMapper _imageIndexMapper;
    private readonly SpecimenIndexMapper _specimenIndexMapper;
    private readonly MutationIndexMapper _mutationIndexMapper;



    public DonorIndexCreationService(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _donorIndexMapper = new DonorIndexMapper();
        _imageIndexMapper = new ImageIndexMapper();
        _specimenIndexMapper = new SpecimenIndexMapper();
        _mutationIndexMapper = new MutationIndexMapper();
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

        index.NumberOfImages = index.Images
            .Select(image => image.Id)
            .Distinct()
            .Count();

        index.NumberOfSpecimens = index.Specimens
            .Select(specimen => specimen.Id)
            .Distinct()
            .Count();

        index.NumberOfMutations = index.Specimens
            .SelectMany(specimen => specimen.Mutations)
            .Select(mutation => mutation.Id)
            .Distinct()
            .Count();

        index.NumberOfGenes = index.Specimens
            .SelectMany(specimen => specimen.Mutations)
            .Where(mutation => mutation.AffectedTranscripts != null)
            .SelectMany(mutation => mutation.AffectedTranscripts)
            .Select(affectedTranscript => affectedTranscript.Transcript.Gene.Id)
            .Distinct()
            .Count();

        return index;
    }

    private Donor LoadDonor(int donorId)
    {
        var donor = _dbContext.Set<Donor>()
            .IncludeClinicalData()
            .IncludeTreatments()
            .IncludeWorkPackages()
            .IncludeStudies()
            .FirstOrDefault(donor => donor.Id == donorId);

        return donor;
    }


    private ImageIndex[] CreateImageIndices(int donorId, DateTime? diagnosisDate)
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

    private ImageIndex CreateImageIndex(Image image, DateTime? diagnosisDate)
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


    private SpecimenIndex[] CreateSpecimenIndices(int donorId, DateTime? diagnosisDate)
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

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen, DateTime? diagnosisDate)
    {
        var index = new SpecimenIndex();

        _specimenIndexMapper.Map(specimen, index, diagnosisDate);

        index.Mutations = CreateMutationIndices(specimen.Id);

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
            .Where(specimen => specimen.DonorId == donorId)
            .ToArray();

        return specimens;
    }


    private MutationIndex[] CreateMutationIndices(int specimenId)
    {
        var mutations = LoadMutations(specimenId);

        if (mutations == null)
        {
            return null;
        }

        var indices = mutations
            .Select(mutation => CreateMutationIndex(mutation))
            .ToArray();

        return indices;
    }

    private MutationIndex CreateMutationIndex(Mutation mutation)
    {
        var index = new MutationIndex();

        _mutationIndexMapper.Map(mutation, index);

        return index;
    }

    private Mutation[] LoadMutations(int specimenId)
    {
        var mutationIds = _dbContext.Set<MutationOccurrence>()
            .Where(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.SpecimenId == specimenId)
            .Select(mutationOccurrence => mutationOccurrence.MutationId)
            .Distinct()
            .ToArray();

        var mutations = _dbContext.Set<Mutation>()
            .IncludeAffectedTranscripts()
            .Where(mutation => mutationIds.Contains(mutation.Id))
            .ToArray();

        return mutations;
    }
}
