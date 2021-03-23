using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class TreatmentRepository : Repository<Treatment>
    {
        public TreatmentRepository(UniteDbContext database) : base(database)
        {
        }

        protected override void Map(in Treatment source, ref Treatment target)
        {
            target.DonorId = source.Donor?.Id ?? source.DonorId;
            target.TherapyId = source.Therapy?.Id ?? source.TherapyId;

            target.Details = source.Details;
            target.StartDate = source.StartDate;
            target.EndDate = source.EndDate;
            target.Results = source.Results;
        }
    }
}
