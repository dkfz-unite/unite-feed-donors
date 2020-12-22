using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class LocalizationRepository : Repository<Localization>
    {
        public LocalizationRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        protected override void Map(in Localization source, ref Localization target)
        {
            target.Value = source.Value;
        }
    }
}
