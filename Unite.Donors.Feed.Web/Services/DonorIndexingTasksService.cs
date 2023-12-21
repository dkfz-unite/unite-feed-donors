using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Donors;

using CNV = Unite.Data.Entities.Genome.Variants.CNV;
using SSM = Unite.Data.Entities.Genome.Variants.SSM;
using SV = Unite.Data.Entities.Genome.Variants.SV;

namespace Unite.Donors.Feed.Web.Services;

public class DonorIndexingTasksService : IndexingTaskService<Donor, int>
{
    protected override int BucketSize => 1000;

    private readonly DonorsRepository _donorsRepository;

    public DonorIndexingTasksService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        _donorsRepository = new DonorsRepository(dbContextFactory);
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
        return _donorsRepository.GetRelatedImages(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedSpecimens(IEnumerable<int> keys)
    {
        return _donorsRepository.GetRelatedSpecimens(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedGenes(IEnumerable<int> keys)
    {
        return _donorsRepository.GetRelatedGenes(keys).Result;
    }

    protected override IEnumerable<long> LoadRelatedSsms(IEnumerable<int> keys)
    {
        return _donorsRepository.GetRelatedVariants<SSM.Variant>(keys).Result;
    }

    protected override IEnumerable<long> LoadRelatedCnvs(IEnumerable<int> keys)
    {
        return _donorsRepository.GetRelatedVariants<CNV.Variant>(keys).Result;
    }

    protected override IEnumerable<long> LoadRelatedSvs(IEnumerable<int> keys)
    {
        return _donorsRepository.GetRelatedVariants<SV.Variant>(keys).Result;
    }
}
