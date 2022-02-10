using System.Collections.Generic;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;

namespace Unite.Donors.Feed.Web.Services
{
    public abstract class IndexingTaskService<T, TKey> : TaskService
        where T : class
    {

        protected IndexingTaskService(DomainDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Creates only target type indexing tasks for all existing entities of target type
        /// </summary>
        public abstract void CreateTasks();

        /// <summary>
        /// Creates only target type indexing tasks for all entities of target type with given identifiers
        /// </summary>
        /// <param name="keys">Identifiers of entities</param>
        public abstract void CreateTasks(IEnumerable<TKey> keys);

        /// <summary>
        /// Populates all types of indexing tasks for entities of target type with given identifiers
        /// </summary>
        /// <param name="keys">Identifiers of entities</param>
        public abstract void PopulateTasks(IEnumerable<TKey> keys);


        protected abstract IEnumerable<int> LoadRelatedDonors(IEnumerable<TKey> keys);
        protected abstract IEnumerable<int> LoadRelatedImages(IEnumerable<TKey> keys);
        protected abstract IEnumerable<int> LoadRelatedSpecimens(IEnumerable<TKey> keys);
        protected abstract IEnumerable<long> LoadRelatedMutations(IEnumerable<TKey> keys);
        protected abstract IEnumerable<int> LoadRelatedGenes(IEnumerable<TKey> keys);


        protected virtual void CreateDonorIndexingTasks(IEnumerable<TKey> keys)
        {
            var donorIds = LoadRelatedDonors(keys);

            CreateTasks(TaskType.Indexing, TaskTargetType.Donor, donorIds);
        }

        protected virtual void CreateImageIndexingTasks(IEnumerable<TKey> keys)
        {
            var imageIds = LoadRelatedImages(keys);

            CreateTasks(TaskType.Indexing, TaskTargetType.Image, imageIds);
        }

        protected virtual void CreateSpecimenIndexingTasks(IEnumerable<TKey> keys)
        {
            var specimenIds = LoadRelatedSpecimens(keys);

            CreateTasks(TaskType.Indexing, TaskTargetType.Specimen, specimenIds);
        }

        protected virtual void CreateMutationIndexingTasks(IEnumerable<TKey> keys)
        {
            var mutationIds = LoadRelatedMutations(keys);

            CreateTasks(TaskType.Indexing, TaskTargetType.Mutation, mutationIds);
        }

        protected virtual void CreateGeneIndexingTasks(IEnumerable<TKey> keys)
        {
            var geneIds = LoadRelatedGenes(keys);

            CreateTasks(TaskType.Indexing, TaskTargetType.Gene, geneIds);
        }
    }
}
