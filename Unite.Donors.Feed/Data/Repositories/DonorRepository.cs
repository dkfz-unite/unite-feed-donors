using Unite.Data.Context;
using Unite.Data.Entities.Donors;
using Unite.Donors.Feed.Data.Models;

namespace Unite.Donors.Feed.Data.Repositories;

internal class DonorRepository
{
    private readonly DomainDbContext _dbContext;


    public DonorRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Donor Find(int id)
    {
        return _dbContext.Set<Donor>()
            .FirstOrDefault(entity =>
                entity.Id == id
            );
    }

    public Donor Find(DonorModel model)
    {
        var entity = _dbContext.Set<Donor>()
            .FirstOrDefault(entity =>
                entity.ReferenceId == model.ReferenceId
            );

        return entity;
    }

    public Donor Create(DonorModel model)
    {
        var entity = new Donor
        {
            ReferenceId = model.ReferenceId
        };

        Map(model, entity);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public void Update(Donor entity, DonorModel model)
    {
        Map(model, entity);

        _dbContext.Update(entity);
        _dbContext.SaveChanges();
    }

    public void Delete(Donor entity)
    {
        _dbContext.Remove(entity);
        _dbContext.SaveChanges();
    }


    private void Map(DonorModel source, Donor target)
    {
        target.MtaProtected = source.MtaProtected;
    }
}
