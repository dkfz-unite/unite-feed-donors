using System;
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

        public Treatment Find(string donorId, int therapyId, DateTime? startDate, DateTime? endDate)
        {
            var treatment = Find(treatment =>
                treatment.DonorId == donorId &&
                treatment.TherapyId == therapyId &&
                treatment.StartDate == startDate &&
                treatment.EndDate == endDate);

            return treatment;
        }

        protected override void Map(in Treatment source, ref Treatment target)
        {
            target.Donor = source.Donor;
            target.Therapy = source.Therapy;
            target.Details = source.Details;
            target.StartDate = source.StartDate;
            target.EndDate = source.EndDate;
        }
    }
}
