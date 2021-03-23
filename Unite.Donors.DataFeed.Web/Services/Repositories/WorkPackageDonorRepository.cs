using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class WorkPackageDonorRepository : Repository<WorkPackageDonor>
    {
        public WorkPackageDonorRepository(UniteDbContext database) : base(database)
        {
        }

        protected override void Map(in WorkPackageDonor source, ref WorkPackageDonor target)
        {
            target.WorkPackageId = source.WorkPackage?.Id ?? source.WorkPackageId;
            target.DonorId = source.Donor?.Id ?? source.DonorId;
        }
    }
}
