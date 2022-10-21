using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Services;
using Unite.Data.Services.Tasks;

using CNV = Unite.Data.Entities.Genome.Variants.CNV;
using SSM = Unite.Data.Entities.Genome.Variants.SSM;
using SV = Unite.Data.Entities.Genome.Variants.SV;

namespace Unite.Donors.Feed.Web.Services;

public class DonorIndexingTasksService : IndexingTaskService<Donor, int>
{
    protected override int BucketSize => 1000;


    public DonorIndexingTasksService(DomainDbContext dbContext) : base(dbContext)
    {
    }


    public override void CreateTasks()
    {
        IterateEntities<Donor, int>(donor => true, donor => donor.Id, donors =>
        {
            CreateDonorIndexingTasks(donors);
        });
    }

    public override void CreateTasks(IEnumerable<int> keys)
    {
        IterateEntities<Donor, int>(donor => keys.Contains(donor.Id), donor => donor.Id, donors =>
        {
            CreateDonorIndexingTasks(donors);
        });
    }

    public override void PopulateTasks(IEnumerable<int> keys)
    {
        IterateEntities<Donor, int>(donor => keys.Contains(donor.Id), donor => donor.Id, donors =>
        {
            CreateDonorIndexingTasks(donors);
            CreateImageIndexingTasks(donors);
            CreateSpecimenIndexingTasks(donors);
            CreateVariantIndexingTasks(donors);
            CreateGeneIndexingTasks(donors);
        });
    }


    protected override IEnumerable<int> LoadRelatedDonors(IEnumerable<int> keys)
    {
        return keys;
    }

    protected override IEnumerable<int> LoadRelatedImages(IEnumerable<int> keys)
    {
        var imageIds = _dbContext.Set<Image>()
            .Where(image => keys.Contains(image.DonorId))
            .Select(image => image.Id)
            .Distinct()
            .ToArray();

        return imageIds;
    }

    protected override IEnumerable<int> LoadRelatedSpecimens(IEnumerable<int> keys)
    {
        var specimenIds = _dbContext.Set<Specimen>()
            .Where(specimen => keys.Contains(specimen.DonorId))
            .Select(specimen => specimen.Id)
            .Distinct()
            .ToArray();

        return specimenIds;
    }

    protected override IEnumerable<int> LoadRelatedGenes(IEnumerable<int> keys)
    {
        var ssmAffectedGeneIds = _dbContext.Set<SSM.AffectedTranscript>()
            .Where(affectedTranscript => affectedTranscript.Variant.Occurrences.Any(occurrence =>
                keys.Contains(occurrence.AnalysedSample.Sample.Specimen.DonorId)))
            .Where(affectedTranscript => affectedTranscript.Feature.GeneId != null)
            .Select(affectedTranscript => affectedTranscript.Feature.GeneId.Value)
            .Distinct()
            .ToArray();

        var cnvAffectedGeneIds = _dbContext.Set<SSM.AffectedTranscript>()
            .Where(affectedTranscript => affectedTranscript.Variant.Occurrences.Any(occurrence =>
                keys.Contains(occurrence.AnalysedSample.Sample.Specimen.DonorId)))
            .Where(affectedTranscript => affectedTranscript.Feature.GeneId != null)
            .Select(affectedTranscript => affectedTranscript.Feature.GeneId.Value)
            .Distinct()
            .ToArray();

        var svAffectedGeneIds = _dbContext.Set<SSM.AffectedTranscript>()
            .Where(affectedTranscript => affectedTranscript.Variant.Occurrences.Any(occurrence =>
                keys.Contains(occurrence.AnalysedSample.Sample.Specimen.DonorId)))
            .Where(affectedTranscript => affectedTranscript.Feature.GeneId != null)
            .Select(affectedTranscript => affectedTranscript.Feature.GeneId.Value)
            .Distinct()
            .ToArray();

        var geneIds = ssmAffectedGeneIds.Union(cnvAffectedGeneIds).Union(svAffectedGeneIds).ToArray();

        return geneIds;
    }

    protected override IEnumerable<long> LoadRelatedMutations(IEnumerable<int> keys)
    {
        var variantIds = _dbContext.Set<SSM.VariantOccurrence>()
            .Where(occurrence => keys.Contains(occurrence.AnalysedSample.Sample.Specimen.DonorId))
            .Select(occurrence => occurrence.VariantId)
            .Distinct()
            .ToArray();

        return variantIds;
    }

    protected override IEnumerable<long> LoadRelatedCopyNumberVariants(IEnumerable<int> keys)
    {
        var variantIds = _dbContext.Set<CNV.VariantOccurrence>()
           .Where(occurrence => keys.Contains(occurrence.AnalysedSample.Sample.Specimen.DonorId))
           .Select(occurrence => occurrence.VariantId)
           .Distinct()
           .ToArray();

        return variantIds;
    }

    protected override IEnumerable<long> LoadRelatedStructuralVariants(IEnumerable<int> keys)
    {
        var variantIds = _dbContext.Set<SV.VariantOccurrence>()
           .Where(occurrence => keys.Contains(occurrence.AnalysedSample.Sample.Specimen.DonorId))
           .Select(occurrence => occurrence.VariantId)
           .Distinct()
           .ToArray();

        return variantIds;
    }
}
