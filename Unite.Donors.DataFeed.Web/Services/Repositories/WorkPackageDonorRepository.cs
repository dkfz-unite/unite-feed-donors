using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class WorkPackageDonorRepository : Repository<WorkPackageDonor>
    {
        public WorkPackageDonorRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        protected override void Map(in WorkPackageDonor source, ref WorkPackageDonor target)
        {
            target.WorkPackage = source.WorkPackage;
            target.Donor = source.Donor;
        }
    }
}
