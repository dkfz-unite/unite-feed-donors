using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class TreatmentRepository : Repository<Treatment>
    {
        public TreatmentRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        protected override void Map(in Treatment source, ref Treatment target)
        {
            target.Donor = source.Donor;
            target.Therapy = source.Therapy;
            target.Details = source.Details;
            target.StartDate = source.StartDate;
            target.EndDate = source.EndDate;
            target.Results = source.Results;
        }
    }
}
