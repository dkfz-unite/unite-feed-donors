using Unite.Data.Context;
using Unite.Data.Entities.Donors;

namespace Unite.Donors.Feed.Data.Donors.Repositories;

internal class ProjectDonorRepository
{
    private readonly DomainDbContext _dbContext;


    public ProjectDonorRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ProjectDonor FindOrCreate(int donorId, string projectName)
    {
        return Find(donorId, projectName) ?? Create(donorId, projectName);
    }

    public ProjectDonor Find(int donorId, string projectName)
    {
        var entity = _dbContext.Set<ProjectDonor>()
            .FirstOrDefault(entity =>
                entity.DonorId == donorId &&
                entity.Project.Name == projectName
            );

        return entity;
    }

    public ProjectDonor Create(int donorId, string projectName)
    {
        var entity = new ProjectDonor
        {
            DonorId = donorId,
            Project = GetProject(projectName)
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public IEnumerable<ProjectDonor> CreateOrUpdate(int donorId, IEnumerable<string> projectNames)
    {
        RemoveRedundant(donorId, projectNames);

        var created = CreateMissing(donorId, projectNames);

        return created;
    }

    public IEnumerable<ProjectDonor> CreateMissing(int donorId, IEnumerable<string> projectNames)
    {
        var entitiesToAdd = new List<ProjectDonor>();

        foreach (var projectName in projectNames)
        {
            var entity = Find(donorId, projectName);

            if (entity == null)
            {
                entity = new ProjectDonor
                {
                    DonorId = donorId,
                    Project = GetProject(projectName)
                };

                entitiesToAdd.Add(entity);
            }
        }

        if (entitiesToAdd.Any())
        {
            _dbContext.AddRange(entitiesToAdd);
            _dbContext.SaveChanges();
        }

        return entitiesToAdd;
    }

    public void RemoveRedundant(int donorId, IEnumerable<string> projectNames)
    {
        var entitiesToRemove = _dbContext.Set<ProjectDonor>()
            .Where(entity => entity.DonorId == donorId && !projectNames.Contains(entity.Project.Name))
            .ToArray();

        if (entitiesToRemove.Any())
        {
            _dbContext.RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }
    }


    private Project GetProject(string name)
    {
        var project = _dbContext.Set<Project>().FirstOrDefault(project =>
            project.Name == name
        );

        if (project == null)
        {
            project = new Project { Name = name };

            _dbContext.Add(project);
            _dbContext.SaveChanges();
        }

        return project;
    }
}
