using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Data.Services;
using Unite.Donors.Feed.Data.Donors.Models;

namespace Unite.Donors.Feed.Data.Donors.Repositories
{
    internal class TreatmentRepository
    {
        private readonly DomainDbContext _dbContext;

        public TreatmentRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Treatment Find(int donorId, TreatmentModel model)
        {
            var entity = _dbContext.Set<Treatment>()
                .Include(entity => entity.Therapy)
                .FirstOrDefault(entity =>
                    entity.DonorId == donorId &&
                    entity.Therapy.Name == model.Therapy &&
                    entity.StartDate == model.StartDate &&
                    entity.StartDay == model.StartDay
                );

            return entity;
        }

        public Treatment Create(int donorId, TreatmentModel model)
        {
            var entity = new Treatment
            {
                DonorId = donorId
            };

            Map(model, ref entity);

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public void Update(Treatment entity, TreatmentModel model)
        {
            Map(model, ref entity);

            _dbContext.Update(entity);
            _dbContext.SaveChanges();
        }


        private void Map(in TreatmentModel model, ref Treatment entity)
        {
            entity.Therapy = GetTherapy(model.Therapy);
            entity.Details = model.Details;
            entity.StartDate = model.StartDate;
            entity.StartDay = model.StartDay;
            entity.EndDate = model.EndDate;
            entity.DurationDays = model.DurationDays;
            entity.ProgressionStatus = model.ProgressionStatus;
            entity.ProgressionStatusChangeDate = model.ProgressionStatusChangeDate;
            entity.ProgressionStatusChangeDay = model.ProgressionStatusChangeDay;
            entity.Results = model.Results;
        }

        private Therapy GetTherapy(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var entity = _dbContext.Set<Therapy>()
                .FirstOrDefault(entity =>
                    entity.Name == name
                );

            if (entity == null)
            {
                entity = new Therapy { Name = name };

                _dbContext.Add(entity);
                _dbContext.SaveChanges();
            }

            return entity;
        }
    }
}
