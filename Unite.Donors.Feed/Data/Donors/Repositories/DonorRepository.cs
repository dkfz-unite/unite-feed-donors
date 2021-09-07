using System.Linq;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;
using Unite.Donors.Feed.Data.Donors.Models;

namespace Unite.Donors.Feed.Data.Donors.Repositories
{
    internal class DonorRepository
    {
        private readonly DomainDbContext _dbContext;


        public DonorRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Donor Find(DonorModel donorModel)
        {
            var donor = _dbContext.Donors.FirstOrDefault(donor =>
                donor.ReferenceId == donorModel.ReferenceId
            );

            return donor;
        }

        public Donor Create(DonorModel donorModel)
        {
            var donor = new Donor
            {
                ReferenceId = donorModel.ReferenceId
            };

            Map(donorModel, donor);

            _dbContext.Donors.Add(donor);
            _dbContext.SaveChanges();

            return donor;
        }

        public void Update(Donor donor, DonorModel donorModel)
        {
            Map(donorModel, donor);

            _dbContext.Donors.Update(donor);
            _dbContext.SaveChanges();
        }


        private void Map(DonorModel source, Donor target)
        {
            target.MtaProtected = source.MtaProtected;
        }
    }
}
