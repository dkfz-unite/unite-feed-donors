using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.Feed.Data.Donors.Repositories
{
    internal class WorkPackageDonorRepository
    {
        private readonly UniteDbContext _dbContext;


        public WorkPackageDonorRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
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
                WorkPackage = GetWorkPackage(workPackageName)
            };

            _dbContext.WorkPackageDonors.Add(workPackageDonor);
            _dbContext.SaveChanges();

            return workPackageDonor;
        }

        public IEnumerable<WorkPackageDonor> CreateMissing(int donorId, IEnumerable<string> workPackageNames)
        {
            var workPackageDonorsToAdd = new List<WorkPackageDonor>();

            foreach (var workPackageName in workPackageNames)
            {
                var workPackageDonor = Find(donorId, workPackageName);

                if (workPackageDonor == null)
                {
                    workPackageDonor = new WorkPackageDonor
                    {
                        DonorId = donorId,
                        WorkPackage = GetWorkPackage(workPackageName)
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


        private WorkPackage GetWorkPackage(string name)
        {
            var workPackage = _dbContext.WorkPackages.FirstOrDefault(workPackage =>
                workPackage.Name == name
            );

            if (workPackage == null)
            {
                workPackage = new WorkPackage { Name = name };

                _dbContext.WorkPackages.Add(workPackage);
                _dbContext.SaveChanges();
            }

            return workPackage;
        }
    }
}
