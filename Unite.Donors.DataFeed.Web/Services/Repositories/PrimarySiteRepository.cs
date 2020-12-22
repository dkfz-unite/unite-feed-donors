using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class PrimarySiteRepository : Repository<PrimarySite>
    {
        public PrimarySiteRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        protected override void Map(in PrimarySite source, ref PrimarySite target)
        {
            target.Value = source.Value;
        }
    }
}
