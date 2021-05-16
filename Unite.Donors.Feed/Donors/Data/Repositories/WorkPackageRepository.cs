using System.Linq;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.Feed.Donors.Data.Repositories
{
    internal class WorkPackageRepository
    {
        private readonly UniteDbContext _dbContext;


        public WorkPackageRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public WorkPackage FindOrCreate(string name)
        {
            return Find(name) ?? Create(name);
        }

        public WorkPackage Find(string name)
        {
            var workPcakge = _dbContext.WorkPackages.FirstOrDefault(workPackage =>
                workPackage.Name == name
            );

            return workPcakge;
        }

        public WorkPackage Create(string name)
        {
            var workPackage = new WorkPackage
            {
                Name = name
            };

            _dbContext.WorkPackages.Add(workPackage);
            _dbContext.SaveChanges();

            return workPackage;
        }
    }
}
