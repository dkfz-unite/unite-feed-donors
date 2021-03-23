using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class TherapyRepository : Repository<Therapy>
    {
        public TherapyRepository(UniteDbContext database) : base(database)
        {
        }

        protected override void Map(in Therapy source, ref Therapy target)
        {
            target.Name = source.Name;
            target.Description = source.Description;
        }
    }
}
