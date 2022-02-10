using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors.Clinical;
using Unite.Data.Services;
using Unite.Donors.Feed.Data.Donors.Models;

namespace Unite.Donors.Feed.Data.Donors.Repositories
{
    internal class ClinicalDataRepository
    {
        private readonly DomainDbContext _dbContext;


        public ClinicalDataRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public ClinicalData Find(int donorId)
        {
            var entity = _dbContext.Set<ClinicalData>()
                .Include(entity => entity.PrimarySite)
                .Include(entity => entity.Localization)
                .FirstOrDefault(entity =>
                    entity.DonorId == donorId
                );

            return entity;
        }

        public ClinicalData Create(int donorId, ClinicalDataModel model)
        {
            var entity = new ClinicalData
            {
                DonorId = donorId
            };

            Map(model, ref entity);

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public void Update(ClinicalData entity, ClinicalDataModel model)
        {
            Map(model, ref entity);

            _dbContext.Update(entity);
            _dbContext.SaveChanges();
        }


        private void Map(in ClinicalDataModel model, ref ClinicalData entity)
        {
            entity.GenderId = model.Gender;
            entity.Age = model.Age;
            entity.Diagnosis = model.Diagnosis;
            entity.DiagnosisDate = model.DiagnosisDate;
            entity.PrimarySite = GetPrimarySite(model.PrimarySite);
            entity.Localization = GetLocalization(model.Localization);
            entity.VitalStatus = model.VitalStatus;
            entity.VitalStatusChangeDate = model.VitalStatusChangeDate;
            entity.VitalStatusChangeDay = model.VitalStatusChangeDay;
            entity.KpsBaseline = model.KpsBaseline;
            entity.SteroidsBaseline = model.SteroidsBaseline;
        }

        private TumorPrimarySite GetPrimarySite(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var entity = _dbContext.Set<TumorPrimarySite>()
                .FirstOrDefault(entity =>
                    entity.Value == value
                );

            if (entity == null)
            {
                entity = new TumorPrimarySite { Value = value };

                _dbContext.Add(entity);
                _dbContext.SaveChanges();
            }

            return entity;
        }

        private TumorLocalization GetLocalization(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var entity = _dbContext.Set<TumorLocalization>()
                .FirstOrDefault(entity =>
                    entity.Value == value
                );

            if (entity == null)
            {
                entity = new TumorLocalization { Value = value };

                _dbContext.Add(entity);
                _dbContext.SaveChanges();
            }

            return entity;
        }
    }
}
