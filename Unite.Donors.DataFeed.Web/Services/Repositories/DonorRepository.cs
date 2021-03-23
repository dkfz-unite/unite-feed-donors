using System.Linq;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class DonorRepository : Repository<Donor>
    {
        private readonly PrimarySiteRepository _primariSiteRepository;

        public DonorRepository(UniteDbContext database) : base(database)
        {
            _primariSiteRepository = new PrimarySiteRepository(database);
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

        private PrimarySite GetOrCreatePrimarySite(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var entity = _primariSiteRepository.Entities.FirstOrDefault(primarySite =>
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
