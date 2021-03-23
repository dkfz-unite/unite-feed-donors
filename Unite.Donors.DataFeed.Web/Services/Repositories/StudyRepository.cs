using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class StudyRepository : Repository<Study>
    {
        public StudyRepository(UniteDbContext database) : base(database)
        {
        }

        protected override void Map(in Study source, ref Study target)
        {
            target.Name = source.Name;
        }
    }
}
