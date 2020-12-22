using Microsoft.Extensions.Logging;
using Unite.Data.Entities;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class WorkPackageRepository : Repository<WorkPackage>
    {
        public WorkPackageRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        public WorkPackage Find(string name)
        {
            var workPackage = Find(workPackage =>
                workPackage.Name == name);

            return workPackage;
        }

        protected override void Map(in WorkPackage source, ref WorkPackage target)
        {
            target.Name = source.Name;
        }
    }
}
