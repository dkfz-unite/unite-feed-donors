using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Donors;

using SSM = Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;

namespace Unite.Donors.Feed.Web.Services;

public class ProjectIndexingTasksService : IndexingTaskService<Project, int>
{
    protected override int BucketSize => 1000;

    private readonly ProjectsRepository _projectsRepository;

    public ProjectIndexingTasksService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        _projectsRepository = new ProjectsRepository(dbContextFactory);
    }
    

    public override void CreateTasks()
    {
        IterateEntities<Project, int>(project => true, project => project.Id, projects =>
        {
            CreateProjectIndexingTasks(projects);
        });
    }

    public override void CreateTasks(IEnumerable<int> keys)
    {
        IterateEntities<Project, int>(project => keys.Contains(project.Id), project => project.Id, projects =>
        {
            CreateProjectIndexingTasks(projects);
        });
    }

    public override void PopulateTasks(IEnumerable<int> keys)
    {
        IterateEntities<Project, int>(project => keys.Contains(project.Id), project => project.Id, projects =>
        {
            CreateProjectIndexingTasks(projects);
        });
    }


    protected override IEnumerable<int> LoadRelatedProjects(IEnumerable<int> keys)
    {
        return keys;
    }

    protected override IEnumerable<int> LoadRelatedDonors(IEnumerable<int> keys)
    {
        return _projectsRepository.GetRelatedDonors(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedImages(IEnumerable<int> keys)
    {
        return _projectsRepository.GetRelatedImages(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedSpecimens(IEnumerable<int> keys)
    {
        return _projectsRepository.GetRelatedSpecimens(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedGenes(IEnumerable<int> keys)
    {
        return _projectsRepository.GetRelatedGenes(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedSsms(IEnumerable<int> keys)
    {
        return _projectsRepository.GetRelatedVariants<SSM.Variant>(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedCnvs(IEnumerable<int> keys)
    {
        return _projectsRepository.GetRelatedVariants<CNV.Variant>(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedSvs(IEnumerable<int> keys)
    {
        return _projectsRepository.GetRelatedVariants<SV.Variant>(keys).Result;
    }
}
