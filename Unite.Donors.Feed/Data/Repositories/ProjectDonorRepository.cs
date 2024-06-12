using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Donors;

namespace Unite.Donors.Feed.Data.Repositories;

internal class ProjectDonorRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly ProjectRepository _projectRepository;


    public ProjectDonorRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _projectRepository = new ProjectRepository(dbContext);
    }


    public ProjectDonor Find(int donorId, string name)
    {
        var project = _projectRepository.Find(name);

        if (project == null)
            return null;

        return _dbContext.Set<ProjectDonor>().AsNoTracking().FirstOrDefault(entity =>
            entity.DonorId == donorId &&
            entity.ProjectId == project.Id
        );
    }

    public IEnumerable<ProjectDonor> CreateAll(int donorId, IEnumerable<string> names)
    {
        var entities = new List<ProjectDonor>();

        foreach (var name in names)
        {
            var projectId = _projectRepository.FindOrCreate(name).Id;

            var entity = new ProjectDonor()
            {
                DonorId = donorId,
                ProjectId = projectId
            };

            entities.Add(entity);
        }

        if (entities.Any())
        {
            _dbContext.AddRange(entities);
            _dbContext.SaveChanges();
        }

        return entities;
    }

    public IEnumerable<ProjectDonor> RecreateAll(int donorId, IEnumerable<string> names)
    {
        RemoveAll(donorId);

        return CreateAll(donorId, names);
    }


    private void RemoveAll(int donorId)
    {
        var entities = _dbContext.Set<ProjectDonor>()
            .Where(entity => entity.DonorId == donorId)
            .ToArray();

        if (entities.Any())
        {
            _dbContext.RemoveRange(entities);
            _dbContext.SaveChanges();
        }
    }
}
