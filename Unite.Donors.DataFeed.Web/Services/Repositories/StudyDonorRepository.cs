using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class StudyDonorRepository : Repository<StudyDonor>
    {
        public StudyDonorRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        protected override void Map(in StudyDonor source, ref StudyDonor target)
        {
            target.Study = source.Study;
            target.Donor = source.Donor;
        }
    }
}
