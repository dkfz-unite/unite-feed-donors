using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class TherapyRepository : Repository<Therapy>
    {
        public TherapyRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        protected override void Map(in Therapy source, ref Therapy target)
        {
            target.Name = source.Name;
            target.Description = source.Description;
        }
    }
}
