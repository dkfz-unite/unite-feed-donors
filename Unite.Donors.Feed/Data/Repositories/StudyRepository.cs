using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Donors;

namespace Unite.Donors.Feed.Data.Repositories;

public class StudyRepository
{
    private readonly DomainDbContext _dbContext;

    
    public StudyRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Study Find(string name)
    {
        return _dbContext.Set<Study>().AsNoTracking().FirstOrDefault(entity =>
            entity.Name == name
        );
    }

    public Study FindOrCreate(string name)
    {
        var entity = Find(name);

        if (entity == null)
        {
            entity = new Study()
            {
                Name = name
            };

            _dbContext.Add(entity);
            _dbContext.SaveChanges();
        }

        return entity;
    }

    public IEnumerable<Study> CreateMissing(IEnumerable<string> names)
    {
        var entitiesToAdd = new List<Study>();

        foreach (var name in names)
        {
            var entity = Find(name);

            if (entity == null)
            {
                entity = new Study() { Name = name };

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
}
