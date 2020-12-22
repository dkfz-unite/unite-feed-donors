using Microsoft.Extensions.Logging;
using Unite.Data.Entities;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class StudyRepository : Repository<Study>
    {
        public StudyRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        public Study Find(string name)
        {
            var study = Find(study =>
                study.Name == name);

            return study;
        }

        protected override void Map(in Study source, ref Study target)
        {
            target.Name = source.Name;
        }
    }
}
