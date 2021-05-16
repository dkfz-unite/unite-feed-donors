using System.Linq;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;
using Unite.Donors.Feed.Donors.Data.Models;

namespace Unite.Donors.Feed.Donors.Data.Repositories
{
    internal class DonorRepository
    {
        private readonly UniteDbContext _dbContext;


        public DonorRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Donor Find(DonorModel donorModel)
        {
            return Find(donorModel.ReferenceId);
        }

        public Donor Create(DonorModel donorModel)
        {
            var donor = new Donor();

            donor.ReferenceId = donorModel.ReferenceId;

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


        private Donor Find(string referenceId)
        {
            var donor = _dbContext.Donors.FirstOrDefault(donor =>
                donor.ReferenceId == referenceId
            );

            return donor;
        }


        private void Map(DonorModel source, Donor target)
        {
            target.MtaProtected = source.MtaProtected;
        }
    }
}
