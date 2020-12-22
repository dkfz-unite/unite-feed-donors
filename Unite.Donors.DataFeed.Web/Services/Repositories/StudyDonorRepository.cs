using Microsoft.Extensions.Logging;
using Unite.Data.Entities;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class StudyDonorRepository : Repository<StudyDonor>
    {
        public StudyDonorRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        public StudyDonor Find(int studyId, string donorId)
        {
            var studyDonor = Find(studyDonor =>
                studyDonor.StudyId == studyId &&
                studyDonor.DonorId == donorId);

            return studyDonor;
        }

        protected override void Map(in StudyDonor source, ref StudyDonor target)
        {
            target.Study = source.Study;
            target.Donor = source.Donor;
        }
    }
}
