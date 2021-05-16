using System.Linq;
using Unite.Data.Entities.Clinical;
using Unite.Data.Services;

namespace Unite.Donors.Feed.Donors.Data.Repositories
{
    internal class TumourLocalizationRepository
    {
        private readonly UniteDbContext _dbContext;


        public TumourLocalizationRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public TumourLocalization FindOrCreate(string value)
        {
            return Find(value) ?? Create(value);
        }

        public TumourLocalization Find(string value)
        {
            var localization = _dbContext.TumourLocalizations.FirstOrDefault(localization =>
                localization.Value == value
            );

            return localization;
        }

        public TumourLocalization Create(string value)
        {
            var localization = new TumourLocalization
            {
                Value = value
            };

            _dbContext.TumourLocalizations.Add(localization);
            _dbContext.SaveChanges();

            return localization;
        }
    }
}
