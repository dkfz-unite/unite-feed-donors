using System.Linq;
using Unite.Data.Entities.Clinical;
using Unite.Data.Services;

namespace Unite.Donors.Feed.Donors.Data.Repositories
{
    internal class TherapyRepository
    {
        private readonly UniteDbContext _dbContext;


        public TherapyRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Therapy FindOrCreate(string name)
        {
            return Find(name) ?? Create(name);
        }

        public Therapy Find(string name)
        {
            var therapy = _dbContext.Therapies.FirstOrDefault(therapy =>
                therapy.Name == name
            );

            return therapy;
        }

        public Therapy Create(string name)
        {
            var therapy = new Therapy
            {
                Name = name
            };

            _dbContext.Therapies.Add(therapy);
            _dbContext.SaveChanges();

            return therapy;
        }
    }
}
