using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Donors;
using Unite.Data.Services;

namespace Unite.Donors.Feed.Data.Donors.Repositories
{
    internal class WorkPackageDonorRepository
    {
        private readonly DomainDbContext _dbContext;


        public WorkPackageDonorRepository(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public WorkPackageDonor FindOrCreate(int donorId, string workPackageName)
        {
            return Find(donorId, workPackageName) ?? Create(donorId, workPackageName);
        }

        public WorkPackageDonor Find(int donorId, string workPackageName)
        {
            var entity = _dbContext.Set<WorkPackageDonor>()
                .FirstOrDefault(entity =>
                    entity.DonorId == donorId &&
                    entity.WorkPackage.Name == workPackageName
                );

            return entity;
        }

        public WorkPackageDonor Create(int donorId, string workPackageName)
        {
            var entity = new WorkPackageDonor
            {
                DonorId = donorId,
                WorkPackage = GetWorkPackage(workPackageName)
            };

            _dbContext.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public IEnumerable<WorkPackageDonor> CreateOrUpdate(int donorId, IEnumerable<string> workPackageNames)
        {
            RemoveRedundant(donorId, workPackageNames);

            var created = CreateMissing(donorId, workPackageNames);

            return created;
        }

        public IEnumerable<WorkPackageDonor> CreateMissing(int donorId, IEnumerable<string> workPackageNames)
        {
            var entitiesToAdd = new List<WorkPackageDonor>();

            foreach (var workPackageName in workPackageNames)
            {
                var entity = Find(donorId, workPackageName);

                if (entity == null)
                {
                    entity = new WorkPackageDonor
                    {
                        DonorId = donorId,
                        WorkPackage = GetWorkPackage(workPackageName)
                    };

                    entitiesToAdd.Add(entity);
                }
            }

            if (entitiesToAdd.Any())
            {
                _dbContext.AddRange(entitiesToAdd);
                _dbContext.SaveChanges();
            }

            return entitiesToAdd;
        }

        public void RemoveRedundant(int donorId, IEnumerable<string> workPackageNames)
        {
            var entitiesToRemove = _dbContext.Set<WorkPackageDonor>()
                .Where(entity => entity.DonorId == donorId && !workPackageNames.Contains(entity.WorkPackage.Name))
                .ToArray();

            if (entitiesToRemove.Any())
            {
                _dbContext.RemoveRange(entitiesToRemove);
                _dbContext.SaveChanges();
            }
        }


        private WorkPackage GetWorkPackage(string name)
        {
            var workPackage = _dbContext.WorkPackages.FirstOrDefault(workPackage =>
                workPackage.Name == name
            );

            if (workPackage == null)
            {
                workPackage = new WorkPackage { Name = name };

                _dbContext.Add(workPackage);
                _dbContext.SaveChanges();
            }

            return workPackage;
        }
    }
}
