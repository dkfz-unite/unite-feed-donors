using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class ClinicalDataRepository : Repository<ClinicalData>
    {
        private readonly Repository<Localization> _localizationRepository;

        public ClinicalDataRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
            _localizationRepository = new LocalizationRepository(database, logger);
        }

        protected override void Map(in ClinicalData source, ref ClinicalData target)
        {
            target.DonorId = source.DonorId;
            target.GenderId = source.GenderId;
            target.Age = source.Age;
            target.AgeCategoryId = source.AgeCategoryId;
            target.Localization = GetOrCreateLocalization(source.Localization?.Value);
            target.VitalStatusId = source.VitalStatusId;
            target.VitalStatusChangeDate = source.VitalStatusChangeDate;
            target.SurvivalDays = source.SurvivalDays;
            target.ProgressionDate = source.ProgressionDate;
            target.ProgressionFreeDays = source.ProgressionFreeDays;
            target.RelapseDate = source.RelapseDate;
            target.RelapseFreeDays = source.RelapseFreeDays;
            target.KpsBaseline = source.KpsBaseline;
            target.SteroidsBaseline = source.SteroidsBaseline;
        }

        private Localization GetOrCreateLocalization(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var entity = _localizationRepository.Find(localization =>
                localization.Value == value
            );

            if (entity == null)
            {
                var localization = new Localization();
                localization.Value = value;

                entity = _localizationRepository.Add(localization);
            }

            return entity;
        }
    }
}
