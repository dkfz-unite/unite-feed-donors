using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Clinical;
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
            var clinicalData = _dbContext.ClinicalData
                .Include(clinicalData => clinicalData.PrimarySite)
                .Include(clinicalData => clinicalData.Localization)
                .FirstOrDefault(clinicalData =>
                    clinicalData.DonorId == donorId
                );

            return clinicalData;
        }

        public ClinicalData Create(int donorId, ClinicalDataModel clinicalDataModel)
        {
            var clinicalData = new ClinicalData
            {
                DonorId = donorId
            };

            Map(clinicalDataModel, clinicalData);

            _dbContext.ClinicalData.Add(clinicalData);
            _dbContext.SaveChanges();

            return clinicalData;
        }

        public void Update(ClinicalData clinicalData, ClinicalDataModel clinicalDataModel)
        {
            Map(clinicalDataModel, clinicalData);

            _dbContext.ClinicalData.Update(clinicalData);
            _dbContext.SaveChanges();
        }


        private void Map(ClinicalDataModel clinicalDataModel, ClinicalData clinicalData)
        {
            clinicalData.GenderId = clinicalDataModel.Gender;
            clinicalData.Age = clinicalDataModel.Age;
            clinicalData.Diagnosis = clinicalDataModel.Diagnosis;
            clinicalData.PrimarySite = GetPrimarySite(clinicalDataModel.PrimarySite);
            clinicalData.Localization = GetLocalization(clinicalDataModel.Localization);
            clinicalData.VitalStatus = clinicalDataModel.VitalStatus;
            clinicalData.VitalStatusChangeDay = clinicalDataModel.VitalStatusChangeDay;
            clinicalData.KpsBaseline = clinicalDataModel.KpsBaseline;
            clinicalData.SteroidsBaseline = clinicalDataModel.SteroidsBaseline;
        }

        private TumorPrimarySite GetPrimarySite(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var primarySite = _dbContext.TumorPrimarySites.FirstOrDefault(primarySite =>
                primarySite.Value == value
            );

            if (primarySite == null)
            {
                primarySite = new TumorPrimarySite { Value = value };

                _dbContext.TumorPrimarySites.Add(primarySite);
                _dbContext.SaveChanges();
            }

            return primarySite;
        }

        private TumorLocalization GetLocalization(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var localization = _dbContext.TumorLocalizations.FirstOrDefault(localization =>
                localization.Value == value
            );

            if (localization == null)
            {
                localization = new TumorLocalization { Value = value };

                _dbContext.TumorLocalizations.Add(localization);
                _dbContext.SaveChanges();
            }

            return localization;
        }
    }
}
