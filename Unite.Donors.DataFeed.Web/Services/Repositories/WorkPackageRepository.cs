using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class WorkPackageRepository : Repository<WorkPackage>
    {
        public WorkPackageRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        protected override void Map(in WorkPackage source, ref WorkPackage target)
        {
            target.Name = source.Name;
        }
    }
}
