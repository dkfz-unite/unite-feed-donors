using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Donors.Clinical;

namespace Unite.Donors.Feed.Data.Repositories;

public class TherapyRepository
{
    private readonly DomainDbContext _dbContext;


    public TherapyRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Therapy Find(string name)
    {
        return _dbContext.Set<Therapy>().AsNoTracking().FirstOrDefault(entity =>
            entity.Name == name
        );
    }

    public Therapy FindOrCreate(string name)
    {
        var entity = Find(name);

        if (entity == null)
        {
            entity = new Therapy()
            {
                Name = name
            };

            _dbContext.Add(entity);
            _dbContext.SaveChanges();
        }

        return entity;
    }

    public IEnumerable<Therapy> CreateMissing(IEnumerable<string> names)
    {
        var entitiesToAdd = new List<Therapy>();

        foreach (var name in names)
        {
            var entity = Find(name);

            if (entity == null)
            {
                entity = new Therapy() { Name = name };

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
