using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Donors.Feed.Data.Models;

namespace Unite.Donors.Feed.Data.Repositories;

public class TreatmentRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly TherapyRepository _therapyRepository;

    public TreatmentRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _therapyRepository = new TherapyRepository(dbContext);
    }


    public Treatment Find(int donorId, TreatmentModel model)
    {
        var therapy = _therapyRepository.Find(model.Therapy);

        if (therapy == null)
            return null;

        return _dbContext.Set<Treatment>().AsNoTracking().FirstOrDefault(entity =>
            entity.DonorId == donorId &&
            entity.TherapyId == therapy.Id &&
            entity.StartDate == model.StartDate &&
            entity.StartDay == model.StartDay
        );
    }

    public IEnumerable<Treatment> CreateAll(int donorId, IEnumerable<TreatmentModel> models)
    {
        var entities = new List<Treatment>();

        foreach (var model in models)
        {
            var therapyId = _therapyRepository.FindOrCreate(model.Therapy).Id;

            var entity = new Treatment()
            {
                DonorId = donorId,
                TherapyId = therapyId
            };

            Map(model, ref entity);

            entities.Add(entity);
        }

        if (entities.Any())
        {
            _dbContext.AddRange(entities);
            _dbContext.SaveChanges();
        }

        return entities;
    }

    public IEnumerable<Treatment> RecreateAll(int donorId, IEnumerable<TreatmentModel> models)
    {
        RemoveAll(donorId);

        return CreateAll(donorId, models);
    }


    private void RemoveAll(int donorId)
    {
        var entities = _dbContext.Set<Treatment>()
            .Where(entity => entity.DonorId == donorId)
            .ToList();

        if (entities.Any())
        {
            _dbContext.RemoveRange(entities);
            _dbContext.SaveChanges();
        }
    }
    
    private static void Map(in TreatmentModel model, ref Treatment entity)
    {
        entity.Details = model.Details;
        entity.StartDate = model.StartDate;
        entity.StartDay = model.StartDay;
        entity.EndDate = model.EndDate;
        entity.DurationDays = model.DurationDays;
        entity.Results = model.Results;
    }
}
