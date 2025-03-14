using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Donors;

namespace Unite.Donors.Feed.Data.Repositories;

public class ProjectRepository
{
    private readonly DomainDbContext _dbContext;


    public ProjectRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Project Find(int id)
    {
        return _dbContext.Set<Project>().AsNoTracking().FirstOrDefault(entity =>
            entity.Id == id
        );
    }

    public Project Find(string name)
    {
        return _dbContext.Set<Project>().AsNoTracking().FirstOrDefault(entity =>
            entity.Name == name
        );
    }

    public Project FindOrCreate(string name)
    {
        var entity = Find(name);

        if (entity == null)
        {
            entity = new Project()
            {
                Name = name
            };

            _dbContext.Add(entity);
            _dbContext.SaveChanges();
        }

        return entity;
    }

    public IEnumerable<Project> CreateMissing(IEnumerable<string> names)
    {
        var entitiesToAdd = new List<Project>();

        foreach (var name in names)
        {
            var entity = Find(name);

            if (entity == null)
            {
                entity = new Project() { Name = name };

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

    public void Delete(params int[] ids)
    {
        var projects = _dbContext.Set<Project>()
            .AsNoTracking()
            .Where(entity => ids.Contains(entity.Id))
            .ToArray();

        _dbContext.RemoveRange(projects);
        _dbContext.SaveChanges();
    }
}
