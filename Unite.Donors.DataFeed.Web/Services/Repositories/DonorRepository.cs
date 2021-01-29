using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class DonorRepository : Repository<Donor>
    {
        private readonly PrimarySiteRepository _primariSiteRepository;

        public DonorRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
            _primariSiteRepository = new PrimarySiteRepository(database, logger);
        }

        protected override void Map(in Donor source, ref Donor target)
        {
            target.Id = source.Id;
            target.Origin = source.Origin;
            target.MtaProtected = source.MtaProtected;
            target.PrimarySite = GetOrCreatePrimarySite(source.PrimarySite?.Value);
            target.Diagnosis = source.Diagnosis;
            target.DiagnosisDate = source.DiagnosisDate;
        }

        protected override IQueryable<Donor> Include(IQueryable<Donor> query)
        {
            var includeQuery = query
                .Include(donor => donor.PrimarySite);

            return includeQuery;
        }

        private PrimarySite GetOrCreatePrimarySite(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var entity = _primariSiteRepository.Find(primarySite =>
                primarySite.Value == value
            );

            if(entity == null)
            {
                var primarySite = new PrimarySite();
                primarySite.Value = value;

                entity = _primariSiteRepository.Add(primarySite);
            }

            return entity;
        }
    }
}
