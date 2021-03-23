using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class StudyDonorRepository : Repository<StudyDonor>
    {
        public StudyDonorRepository(UniteDbContext database) : base(database)
        {
        }

        protected override void Map(in StudyDonor source, ref StudyDonor target)
        {
            target.StudyId = source.Study?.Id ?? source.StudyId;
            target.DonorId = source.Donor?.Id ?? source.DonorId;
        }
    }
}
