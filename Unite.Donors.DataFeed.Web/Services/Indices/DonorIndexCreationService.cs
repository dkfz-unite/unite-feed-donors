using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Donors.DataFeed.Web.Services.Indices.Extensions;
using Unite.Indices.Entities.Donors;

namespace Unite.Donors.DataFeed.Web.Services.Indices
{
    public class DonorIndexCreationService
    {
        private readonly UniteDbContext _database;


        public DonorIndexCreationService(UniteDbContext database)
        {
            _database = database;
        }

        public DonorIndex CreateIndex(string donorId)
        {
            var index = CreateDonorIndex(donorId);

            return index;
        }


        private Donor LoadDonor(string donorId)
        {
            var donor = _database.Donors
                .Include(donor => donor.PrimarySite)
                .Include(donor => donor.ClinicalData)
                    .ThenInclude(clinicalData => clinicalData.Localization)
                .Include(donor => donor.Treatments)
                    .ThenInclude(treatment => treatment.Therapy)
                .Include(donor => donor.DonorWorkPackages)
                    .ThenInclude(workPackageDonor => workPackageDonor.WorkPackage)
                .Include(donor => donor.DonorStudies)
                    .ThenInclude(studyDonor => studyDonor.Study)
                .FirstOrDefault(donor => donor.Id == donorId);

            return donor;
        }

        private DonorIndex CreateDonorIndex(string donorId)
        {
            var donor = LoadDonor(donorId);

            if(donor is null)
            {
                return null;
            }

            var index = new DonorIndex();

            index.MapFrom(donor);

            index.Mutations = CreateMutationIndices(donor.Id);

            return index;
        }


        private Mutation[] LoadMutations(string donorId)
        {
            var mutations = _database.MutationOccurrences
                .Include(mutationOccurrence => mutationOccurrence.Mutation)
                    .ThenInclude(mutation => mutation.Gene)
                .Include(mutationOccurrence => mutationOccurrence.Mutation)
                    .ThenInclude(mutation => mutation.Contig)
                .Where(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.DonorId == donorId)
                .Select(mutationOccurrence => mutationOccurrence.Mutation)
                .Distinct() // TODO: Insure Distinc works as expected
                .ToArray();

            return mutations;
        }

        private MutationIndex[] CreateMutationIndices(string donorId)
        {
            var mutations = LoadMutations(donorId);

            if(mutations is null)
            {
                return null;
            }

            var indices = mutations
                .Select(mutation => CreateMutationIndex(mutation, donorId))
                .ToArray();

            return indices;
        }

        private MutationIndex CreateMutationIndex(in Mutation mutation, string donorId)
        {
            if(mutation is null)
            {
                return null;
            }

            var index = new MutationIndex();

            index.MapFrom(mutation);

            index.Samples = CreateSampleIndices(donorId, mutation.Id);

            return index;
        }


        private AnalysedSample[] LoadSamples(string donorId, int mutationId)
        {
            var samples = _database.AnalysedSamples
                .Include(analysedSample => analysedSample.Sample)
                .Include(analysedSample => analysedSample.Analysis)
                    .ThenInclude(analysis => analysis.File)
                .Include(analysedSample => analysedSample.MatchedSamples)
                    .ThenInclude(matchedSample => matchedSample.Matched)
                        .ThenInclude(analysedSample => analysedSample.Sample)
                .Where(analysedSample =>
                    analysedSample.Sample.DonorId == donorId &&
                    analysedSample.MutationOccurrences.Any(mutationOccurrence => mutationOccurrence.MutationId == mutationId))
                .ToArray();

            return samples;
        }

        private AnalysedSampleIndex[] CreateSampleIndices(string donorId, int mutationId)
        {
            var samples = LoadSamples(donorId, mutationId);

            if (samples is null)
            {
                return null;
            }

            var indices = samples
                .Select(analysedSample => CreateSampleIndex(analysedSample))
                .ToArray();

            return indices;
        }

        private AnalysedSampleIndex CreateSampleIndex(AnalysedSample analysedSample)
        {
            if (analysedSample is null)
            {
                return null;
            }

            var index = new AnalysedSampleIndex();

            index.MapFrom(analysedSample);

            return index;
        }
    }
}
