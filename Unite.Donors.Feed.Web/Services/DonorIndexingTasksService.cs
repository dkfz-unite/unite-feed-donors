using System;
using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome.Mutations;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Services;

namespace Unite.Donors.Feed.Web.Services
{
    public class DonorIndexingTasksService : IndexingTaskService<Donor, int>
    {
        protected override int BucketSize => 1000;


        public DonorIndexingTasksService(DomainDbContext dbContext) : base(dbContext)
        {
        }


        public override void CreateTasks()
        {
            IterateEntities<Donor, int>(donor => true, donor => donor.Id, donors =>
            {
                CreateDonorIndexingTasks(donors);
            });
        }

        public override void CreateTasks(IEnumerable<int> keys)
        {
            IterateEntities<Donor, int>(donor => keys.Contains(donor.Id), donor => donor.Id, donors =>
            {
                CreateDonorIndexingTasks(donors);
            });
        }

        public override void PopulateTasks(IEnumerable<int> keys)
        {
            IterateEntities<Donor, int>(donor => keys.Contains(donor.Id), donor => donor.Id, donors =>
            {
                CreateDonorIndexingTasks(donors);
                CreateImageIndexingTasks(donors);
                CreateSpecimenIndexingTasks(donors);
                CreateMutationIndexingTasks(donors);
                CreateGeneIndexingTasks(donors);
            });
        }


        protected override IEnumerable<int> LoadRelatedDonors(IEnumerable<int> keys)
        {
            return keys;
        }

        protected override IEnumerable<int> LoadRelatedImages(IEnumerable<int> keys)
        {
            var imageIds = _dbContext.Set<Image>()
                .Where(image => keys.Contains(image.DonorId))
                .Select(image => image.Id)
                .Distinct()
                .ToArray();

            return imageIds;
        }

        protected override IEnumerable<int> LoadRelatedSpecimens(IEnumerable<int> keys)
        {
            var specimenIds = _dbContext.Set<Specimen>()
                .Where(specimen => keys.Contains(specimen.DonorId))
                .Select(specimen => specimen.Id)
                .Distinct()
                .ToArray();

            return specimenIds;
        }

        protected override IEnumerable<int> LoadRelatedGenes(IEnumerable<int> keys)
        {
            var mutationIds = _dbContext.Set<MutationOccurrence>()
                .Where(mutationOccurrence => keys.Contains(mutationOccurrence.AnalysedSample.Sample.Specimen.DonorId))
                .Select(mutationOccurrence => mutationOccurrence.MutationId)
                .Distinct()
                .ToArray();

            var geneIds = _dbContext.Set<AffectedTranscript>()
                .Where(affectedTranscript => mutationIds.Contains(affectedTranscript.Id))
                .Where(affectedTranscript => affectedTranscript.Transcript.GeneId != null)
                .Select(affectedTranscript => affectedTranscript.Transcript.GeneId.Value)
                .Distinct()
                .ToArray();

            return geneIds;
        }

        protected override IEnumerable<long> LoadRelatedMutations(IEnumerable<int> keys)
        {
            var mutationIds = _dbContext.Set<MutationOccurrence>()
                .Where(mutationOccurrence => keys.Contains(mutationOccurrence.AnalysedSample.Sample.Specimen.DonorId))
                .Select(mutationOccurrence => mutationOccurrence.MutationId)
                .Distinct()
                .ToArray();

            return mutationIds;
        }
    }
}
