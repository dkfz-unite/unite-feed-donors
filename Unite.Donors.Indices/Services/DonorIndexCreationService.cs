﻿using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Specimens;
using Unite.Data.Services;
using Unite.Donors.Indices.Services.Mappers;
using Unite.Indices.Entities.Donors;
using Unite.Indices.Services;

namespace Unite.Donors.Indices.Services
{
    public class DonorIndexCreationService : IIndexCreationService<DonorIndex>
    {
        private readonly UniteDbContext _dbContext;
        private readonly DonorIndexMapper _donorIndexMapper;
        private readonly MutationIndexMapper _mutationIndexMapper;
        private readonly SpecimenIndexMapper _specimenIndexMapper;


        public DonorIndexCreationService(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _donorIndexMapper = new DonorIndexMapper();
            _mutationIndexMapper = new MutationIndexMapper();
            _specimenIndexMapper = new SpecimenIndexMapper();
        }


        public DonorIndex CreateIndex(object key)
        {
            var donorId = (int)key;

            return CreateDonorIndex(donorId);
        }


        private DonorIndex CreateDonorIndex(int donorId)
        {
            var donor = LoadDonor(donorId);

            var index = CreateDonorIndex(donor);

            return index;
        }

        private DonorIndex CreateDonorIndex(Donor donor)
        {
            if(donor == null)
            {
                return null;
            }

            var index = new DonorIndex();

            _donorIndexMapper.Map(donor, index);

            index.Mutations = CreateMutationIndices(donor.Id);

            index.NumberOfSpecimens = index.Mutations
                .SelectMany(mutation => mutation.Specimens)
                .Select(specimen => specimen.Id)
                .Distinct()
                .Count();

            index.NumberOfMutations = index.Mutations
                .Select(mutation => mutation.Id)
                .Distinct()
                .Count();

            index.NumberOfGenes = index.Mutations
                .SelectMany(mutation => mutation.AffectedTranscripts)
                .Select(affectedTranscript => affectedTranscript.Gene.Id)
                .Distinct()
                .Count();

            return index;
        }

        private Donor LoadDonor(int donorId)
        {
            var donor = _dbContext.Donors
                .Include(donor => donor.ClinicalData)
                    .ThenInclude(clinicalData => clinicalData.PrimarySite)
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


        private MutationIndex[] CreateMutationIndices(int donorId)
        {
            var mutations = LoadMutations(donorId);

            if(mutations == null)
            {
                return null;
            }

            var indices = mutations
                .Select(CreateMutationIndex)
                .ToArray();

            return indices;
        }

        private MutationIndex CreateMutationIndex(Mutation mutation)
        {
            var index = new MutationIndex();

            _mutationIndexMapper.Map(mutation, index);

            index.Specimens = CreateSpecimenIndices(mutation);

            return index;
        }

        private Mutation[] LoadMutations(int donorId)
        {
            var mutationIds = _dbContext.MutationOccurrences
                .Where(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.Specimen.DonorId == donorId)
                .Select(mutationOccurrence => mutationOccurrence.MutationId)
                .Distinct()
                .ToArray();

            var mutations = _dbContext.Mutations
                .Include(mutation => mutation.AffectedTranscripts)
                    .ThenInclude(affectedTranscript => affectedTranscript.Gene)
                        .ThenInclude(gene => gene.Info)
                .Include(mutation => mutation.AffectedTranscripts)
                    .ThenInclude(affectedTranscript => affectedTranscript.Gene)
                        .ThenInclude(gene => gene.Biotype)
                .Include(mutation => mutation.AffectedTranscripts)
                    .ThenInclude(affectedTranscript => affectedTranscript.Transcript)
                        .ThenInclude(transcript => transcript.Info)
                .Include(mutation => mutation.AffectedTranscripts)
                    .ThenInclude(affectedTranscript => affectedTranscript.Transcript)
                        .ThenInclude(transcript => transcript.Biotype)
                .Include(mutation => mutation.AffectedTranscripts)
                    .ThenInclude(affectedTranscript => affectedTranscript.Consequences)
                        .ThenInclude(affectedTranscriptConsequence => affectedTranscriptConsequence.Consequence)
                .Include(mutation => mutation.MutationOccurrences.Where(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.Specimen.DonorId == donorId))
                    .ThenInclude(mutationOccurrence => mutationOccurrence.AnalysedSample)
                        .ThenInclude(analysedSample => analysedSample.Sample)
                            .ThenInclude(sample => sample.Specimen)
                .Where(mutation => mutationIds.Contains(mutation.Id))
                .ToArray();

            return mutations;
        }


        private SpecimenIndex[] CreateSpecimenIndices(Mutation mutation)
        {
            var specimens = mutation.MutationOccurrences
                 .Select(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.Specimen)
                 .ToArray();

            if(specimens == null)
            {
                return null;
            }

            var indices = specimens
                .Select(CreateSpecimenIndex)
                .ToArray();

            return indices;
        }

        private SpecimenIndex CreateSpecimenIndex(Specimen specimen)
        {
            var index = new SpecimenIndex();

            _specimenIndexMapper.Map(specimen, index);

            return index;
        }
    }
}
