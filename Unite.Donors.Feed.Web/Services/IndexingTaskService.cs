using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;

namespace Unite.Donors.Feed.Web.Services
{
    public class IndexingTaskService
    {
        private const int BUCKET_SIZE = 1000;

        private readonly UniteDbContext _dbContext;


        public IndexingTaskService(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Creates only donor indexing tasks for all existing donors
        /// </summary>
        public void CreateTasks()
        {
            IterateDonors(donor => true, donors =>
            {
                CreateDonorIndexingTasks(donors);
            });
        }

        /// <summary>
        /// Creates only donor indexing tasks for all donors with given identifiers
        /// </summary>
        /// <param name="donorIds">Identifiers of donors</param>
        public void CreateTasks(IEnumerable<int> donorIds)
        {
            IterateDonors(donor => donorIds.Contains(donor.Id), donors =>
            {
                CreateDonorIndexingTasks(donors);
            });
        }

        /// <summary>
        /// Populates all types of indexing tasks for for donors with given identifiers
        /// </summary>
        /// <param name="donorIds">Identifiers of donors</param>
        public void PopulateTasks(IEnumerable<int> donorIds)
        {
            IterateDonors(donor => true, donors =>
            {
                CreateDonorIndexingTasks(donors);
                CreateMutationIndexingTasks(donors);
                CreateSpecimenIndexingTasks(donors);
            });
        }


        private void CreateDonorIndexingTasks(IEnumerable<int> donorIds)
        {
            var tasks = donorIds
                .Select(donorId => new Task
                {
                    TypeId = TaskType.Indexing,
                    TargetTypeId = TaskTargetType.Donor,
                    Target = donorId.ToString(),
                    Date = DateTime.UtcNow
                })
                .ToArray();

            _dbContext.Tasks.AddRange(tasks);
            _dbContext.SaveChanges();
        }

        private void CreateMutationIndexingTasks(IEnumerable<int> donorIds)
        {
            var mutationIds = _dbContext.MutationOccurrences
                .Where(mutationOccurrence => donorIds.Contains(mutationOccurrence.AnalysedSample.Sample.Specimen.DonorId))
                .Select(mutationOccurrence => mutationOccurrence.MutationId)
                .Distinct()
                .ToArray();

            var tasks = mutationIds
                .Select(mutationId => new Task
                {
                    TypeId = TaskType.Indexing,
                    TargetTypeId = TaskTargetType.Mutation,
                    Target = mutationId.ToString(),
                    Date = DateTime.UtcNow
                })
                .ToArray();

            _dbContext.Tasks.AddRange(tasks);
            _dbContext.SaveChanges();
        }

        private void CreateSpecimenIndexingTasks(IEnumerable<int> donorIds)
        {
            var specimenIds = _dbContext.Specimens
                .Where(specimen => donorIds.Contains(specimen.DonorId))
                .Select(specimen => specimen.Id)
                .Distinct()
                .ToArray();

            var tasks = specimenIds
                .Select(specimenId => new Task
                {
                    TypeId = TaskType.Indexing,
                    TargetTypeId = TaskTargetType.Specimen,
                    Target = specimenId.ToString(),
                    Date = DateTime.UtcNow
                })
                .ToArray();

            _dbContext.Tasks.AddRange(tasks);
            _dbContext.SaveChanges();
        }

        private void IterateDonors(Expression<Func<Donor, bool>> condition, Action<IEnumerable<int>> handler)
        {
            var position = 0;

            var donors = Enumerable.Empty<int>();

            do
            {
                donors = _dbContext.Donors
                    .Where(condition)
                    .Skip(position)
                    .Take(BUCKET_SIZE)
                    .Select(donor => donor.Id)
                    .ToArray();

                handler.Invoke(donors);

                position += donors.Count();

            }
            while (donors.Count() == BUCKET_SIZE);
        }
    }
}
