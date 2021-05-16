using System.Linq;
using Unite.Data.Entities.Clinical;
using Unite.Data.Services;

namespace Unite.Donors.Feed.Donors.Data.Repositories
{
    internal class TumourPrimarySiteRepository
    {
        private readonly UniteDbContext _dbContext;


        public TumourPrimarySiteRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public TumourPrimarySite FindOrCreate(string value)
        {
            return Find(value) ?? Create(value);
        }

        public TumourPrimarySite Find(string value)
        {
            var primarySite = _dbContext.TumourPrimarySites.FirstOrDefault(primarySite =>
                primarySite.Value == value
            );

            return primarySite;
        }

        public TumourPrimarySite Create(string value)
        {
            var primarySite = new TumourPrimarySite
            {
                Value = value
            };

            _dbContext.TumourPrimarySites.Add(primarySite);
            _dbContext.SaveChanges();

            return primarySite;
        }
    }
}
