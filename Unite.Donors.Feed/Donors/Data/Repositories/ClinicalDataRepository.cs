using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Clinical;
using Unite.Data.Services;
using Unite.Donors.Feed.Donors.Data.Models;

namespace Unite.Donors.Feed.Donors.Data.Repositories
{
    internal class ClinicalDataRepository
    {
        private readonly UniteDbContext _dbContext;
        private readonly TumourPrimarySiteRepository _primarySiteRepository;
        private readonly TumourLocalizationRepository _localizationRepository;


        public ClinicalDataRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _primarySiteRepository = new TumourPrimarySiteRepository(dbContext);
            _localizationRepository = new TumourLocalizationRepository(dbContext);
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
            var clinicalData = new ClinicalData();

            clinicalData.DonorId = donorId;

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
            clinicalData.DiagnosisDate = clinicalDataModel.DiagnosisDate;
            clinicalData.PrimarySite = GetPrimarySite(clinicalDataModel.PrimarySite);
            clinicalData.Localization = GetLocalization(clinicalDataModel.Localization);
            clinicalData.VitalStatus = clinicalDataModel.VitalStatus;
            clinicalData.VitalStatusChangeDate = clinicalDataModel.VitalStatusChangeDate;
            clinicalData.KpsBaseline = clinicalDataModel.KpsBaseline;
            clinicalData.SteroidsBaseline = clinicalDataModel.SteroidsBaseline;
        }

        private TumourPrimarySite GetPrimarySite(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return _primarySiteRepository.FindOrCreate(value);
        }

        private TumourLocalization GetLocalization(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return _localizationRepository.FindOrCreate(value);
        }
    }
}
