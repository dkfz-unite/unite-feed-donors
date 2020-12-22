using Microsoft.Extensions.Logging;
using Unite.Data.Entities;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class WorkPackageDonorRepository : Repository<WorkPackageDonor>
    {
        public WorkPackageDonorRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        public WorkPackageDonor Find(int workPackageId, string donorId)
        {
            var workpackageDonor = Find(workpackageDonor =>
                    workpackageDonor.WorkPackageId == workPackageId &&
                    workpackageDonor.DonorId == donorId);

            return workpackageDonor;
        }

        protected override void Map(in WorkPackageDonor source, ref WorkPackageDonor target)
        {
            target.WorkPackage = source.WorkPackage;
            target.Donor = source.Donor;
        }
    }
}
