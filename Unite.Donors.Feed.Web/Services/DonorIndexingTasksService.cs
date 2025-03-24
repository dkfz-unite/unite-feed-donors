using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Donors;

using SSM = Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;

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
            CreateProjectIndexingTasks(donors);
            CreateDonorIndexingTasks(donors);
            CreateImageIndexingTasks(donors);
            CreateSpecimenIndexingTasks(donors);
            CreateVariantIndexingTasks(donors);
            CreateGeneIndexingTasks(donors);
        });
    }


    protected override IEnumerable<int> LoadRelatedProjects(IEnumerable<int> keys)
    {
        return _donorsRepository.GetRelatedProjects(keys).Result;
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

    protected override IEnumerable<int> LoadRelatedSsms(IEnumerable<int> keys)
    {
        return _donorsRepository.GetRelatedVariants<SSM.Variant>(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedCnvs(IEnumerable<int> keys)
    {
        return _donorsRepository.GetRelatedVariants<CNV.Variant>(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedSvs(IEnumerable<int> keys)
    {
        return _donorsRepository.GetRelatedVariants<SV.Variant>(keys).Result;
    }
}
