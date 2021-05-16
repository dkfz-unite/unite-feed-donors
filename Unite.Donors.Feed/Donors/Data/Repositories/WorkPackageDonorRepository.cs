using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.Feed.Donors.Data.Repositories
{
    internal class WorkPackageDonorRepository
    {
        private readonly UniteDbContext _dbContext;
        private readonly WorkPackageRepository _workPackageRepository;


        public WorkPackageDonorRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _workPackageRepository = new WorkPackageRepository(dbContext);
        }

        public WorkPackageDonor FindOrCreate(int donorId, string workPackageName)
        {
            return Find(donorId, workPackageName) ?? Create(donorId, workPackageName);
        }

        public WorkPackageDonor Find(int donorId, string workPackageName)
        {
            var workPackageDonor = _dbContext.WorkPackageDonors.FirstOrDefault(workPackageDonor =>
                workPackageDonor.DonorId == donorId &&
                workPackageDonor.WorkPackage.Name == workPackageName
            );

            return workPackageDonor;
        }

        public WorkPackageDonor Create(int donorId, string workPackageName)
        {
            var workPackageDonor = new WorkPackageDonor
            {
                DonorId = donorId,
                WorkPackage = _workPackageRepository.FindOrCreate(workPackageName)
            };

            _dbContext.WorkPackageDonors.Add(workPackageDonor);
            _dbContext.SaveChanges();

            return workPackageDonor;
        }

        public IEnumerable<WorkPackageDonor> CreateMissing(int donorId, IEnumerable<string> workPackageNames)
        {
            var workPackageDonorsToAdd = new List<WorkPackageDonor>();

            foreach(var workPackageName in workPackageNames)
            {
                var workPackageDonor = Find(donorId, workPackageName);

                if(workPackageDonor == null)
                {
                    workPackageDonor = new WorkPackageDonor
                    {
                        DonorId = donorId,
                        WorkPackage = _workPackageRepository.FindOrCreate(workPackageName)
                    };

                    workPackageDonorsToAdd.Add(workPackageDonor);
                }
            }

            if (workPackageDonorsToAdd.Any())
            {
                _dbContext.WorkPackageDonors.AddRange(workPackageDonorsToAdd);
                _dbContext.SaveChanges();
            }

            return workPackageDonorsToAdd;
        }
    }
}
