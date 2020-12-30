using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities;
using Unite.Data.Entities.Cells;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Extensions;
using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Samples;
using Unite.Data.Services;
using Unite.Indices.Entities;
using Unite.Indices.Entities.Donors;

namespace Unite.Donors.DataFeed.Web.Services.Indices
{
    public class IndexCreationService : IIndexCreationService
    {
        private readonly UniteDbContext _database;


        public IndexCreationService(UniteDbContext database)
        {
            _database = database;
        }


        public DonorIndex CreateIndex(string donorId)
        {
            var index = CreateDonorIndex(donorId);

            return index;
        }


        private DonorIndex CreateDonorIndex(string donorId)
        {
            var donor = LoadDonor(donorId);

            if(donor == null)
            {
                return null;
            }

            var index = new DonorIndex();

            index.Id = donor.Id;
            index.Origin = donor.Origin;
            index.MtaProtected = donor.MtaProtected;
            index.PrimarySite = donor.PrimarySite?.Value;
            index.Diagnosis = donor.Diagnosis;
            index.DiagnosisDate = donor.DiagnosisDate;

            index.ClinicalData = CreateClinicalDataIndex(donor.Id);
            index.Treatments = CreateTreatmentIndices(donor.Id);
            index.WorkPackages = CreateWorkPackageIndices(donor.Id);
            index.Studies = CreateStudyIndices(donor.Id);
            index.Samples = CreateSampleIndices(donor.Id);
            index.CellLines = CreateCellLineIndices(donor.Id);

            return index;
        }

        private Donor LoadDonor(string donorId)
        {
            var donor = _database.Donors
                .Include(donor => donor.PrimarySite)
                .FirstOrDefault(donor => donor.Id == donorId);

            return donor;
        }


        private ClinicalDataIndex CreateClinicalDataIndex(string donorId)
        {
            var clinicalData = LoadClinicalData(donorId);

            if(clinicalData == null)
            {
                return null;
            }

            var index = new ClinicalDataIndex();

            index.Gender = clinicalData.GenderId.ToDefinitionString();
            index.Age = clinicalData.Age;
            index.AgeCategory = clinicalData.AgeCategoryId?.ToDefinitionString();
            index.Localization = clinicalData.Localization?.Value;
            index.VitalStatus = clinicalData.VitalStatusId?.ToDefinitionString();
            index.VitalStatusChangeDate = clinicalData.VitalStatusChangeDate;
            index.SurvivalDays = clinicalData.SurvivalDays;
            index.ProgressionDate = clinicalData.ProgressionDate;
            index.ProgressionFreeDays = clinicalData.ProgressionFreeDays;
            index.RelapseDate = clinicalData.RelapseDate;
            index.RelapseFreeDays = clinicalData.RelapseFreeDays;
            index.KpsBaseline = clinicalData.KpsBaseline;
            index.SteroidsBaseline = clinicalData.SteroidsBaseline;

            return index;
        }

        private ClinicalData LoadClinicalData(string donorId)
        {
            var clinicalData = _database.ClinicalData
                .Include(clinicalData => clinicalData.Localization)
                .FirstOrDefault(clinicalData =>
                    clinicalData.DonorId == donorId);

            return clinicalData;
        }


        private TreatmentIndex[] CreateTreatmentIndices(string donorId)
        {
            var treatments = LoadTreatments(donorId);

            if (!treatments.Any())
            {
                return null;
            }

            var indices = treatments.Select(treatment =>
            {
                var index = new TreatmentIndex();

                index.Therapy = CreateTherapyIndex(treatment.Therapy);
                index.Details = treatment.Details;
                index.Results = treatment.Results;

                return index;
            })
            .ToArray();

            return indices;
        }

        private Treatment[] LoadTreatments(string donorId)
        {
            var treatments = _database.Treatments
                .Include(treatment => treatment.Therapy)
                .Where(treatment =>
                    treatment.DonorId == donorId)
                .ToArray();

            return treatments;
        }

        private TherapyIndex CreateTherapyIndex(in Therapy therapy)
        {
            if(therapy == null)
            {
                return null;
            }

            var index = new TherapyIndex();

            index.Id = therapy.Id;
            index.Name = therapy.Name;

            return index;
        }


        private WorkPackageIndex[] CreateWorkPackageIndices(string donorId)
        {
            var workPackages = LoadWorkPackages(donorId);

            if (!workPackages.Any())
            {
                return null;
            }

            var indices = workPackages.Select(workPackage =>
            {
                var index = new WorkPackageIndex();

                index.Id = workPackage.WorkPackage.Id;
                index.Name = workPackage.WorkPackage.Name;

                return index;
            })
            .ToArray();

            return indices;
        }

        private WorkPackageDonor[] LoadWorkPackages(string donorId)
        {
            var workPackages = _database.WorkPackageDonors
                .Include(workPackageDonor => workPackageDonor.WorkPackage)
                .Where(workPackageDonor =>
                    workPackageDonor.DonorId == donorId)
                .ToArray();

            return workPackages;
        }


        private StudyIndex[] CreateStudyIndices(string donorId)
        {
            var studies = LoadStudies(donorId);

            if (!studies.Any())
            {
                return null;
            }

            var indices = studies.Select(study =>
            {
                var index = new StudyIndex();

                index.Id = study.Study.Id;
                index.Name = study.Study.Name;

                return index;
            })
            .ToArray();

            return indices;
        }

        private StudyDonor[] LoadStudies(string donorId)
        {
            var studies = _database.StudyDonors
                .Include(studyDonor => studyDonor.Study)
                .Where(studyDonor =>
                    studyDonor.DonorId == donorId)
                .ToArray();

            return studies;
        }


        private SampleIndex[] CreateSampleIndices(string donorId, int? cellLineId = null)
        {
            var samples = LoadSamples(donorId, cellLineId);

            if (!samples.Any())
            {
                return null;
            }

            var indices = samples.Select(sample =>
            {
                var index = new SampleIndex();

                index.Id = sample.Id;
                index.Name = sample.Name;
                index.Link = sample.Link;
                index.Type = sample.TypeId?.ToDefinitionString();
                index.Subtype = sample.SubtypeId?.ToDefinitionString();

                index.Mutations = CreateMutationIndices(sample.Id);

                return index;
            })
            .ToArray();

            return indices;
        }

        private Sample[] LoadSamples(string donorId, int? cellLineId = null)
        {
            var samples = _database.Samples
                .Where(sample =>
                    sample.DonorId == donorId &&
                    sample.CellLineId == cellLineId)
                .ToArray();

            return samples;
        }


        private CellLineIndex[] CreateCellLineIndices(string donorId)
        {
            var cellLines = LoadCellLines(donorId);

            if (!cellLines.Any())
            {
                return null;
            }

            var indices = cellLines.Select(cellLine =>
            {
                var index = new CellLineIndex();

                index.Id = cellLine.Id;
                index.Name = cellLine.Name;
                index.Type = cellLine.TypeId?.ToDefinitionString();
                index.Species = cellLine.SpeciesId?.ToDefinitionString();
                index.GeneExpressionSubtype = cellLine.GeneExpressionSubtypeId?.ToDefinitionString();
                index.IdhStatus = cellLine.IdhStatusId?.ToDefinitionString();
                index.IdhMutation = cellLine.IdhMutationId?.ToDefinitionString();
                index.MethylationStatus = cellLine.MethylationStatusId?.ToDefinitionString();
                index.MethylationSubtype = cellLine.MethylationSubtypeId?.ToDefinitionString();
                index.GcimpMethylation = cellLine.GcimpMethylation;

                index.Parent = CreateCellLineBaseIndex(cellLine.Parent);
                index.Children = CreateCellLineBaseIndices(cellLine.Childern);
                index.Samples = CreateSampleIndices(donorId, cellLine.Id);
                
                return index;
            })
            .ToArray();

            return indices;
        }

        private CellLineBaseIndex CreateCellLineBaseIndex(in CellLine cellLine)
        {
            if(cellLine == null)
            {
                return null;
            }

            var index = new CellLineBaseIndex();

            index.Id = cellLine.Id;
            index.Name = cellLine.Name;
            index.Type = cellLine.TypeId?.ToDefinitionString();
            index.Species = cellLine.SpeciesId?.ToDefinitionString();
            index.GeneExpressionSubtype = cellLine.GeneExpressionSubtypeId?.ToDefinitionString();
            index.IdhStatus = cellLine.IdhStatusId?.ToDefinitionString();
            index.IdhMutation = cellLine.IdhMutationId?.ToDefinitionString();
            index.MethylationStatus = cellLine.MethylationStatusId?.ToDefinitionString();
            index.MethylationSubtype = cellLine.MethylationSubtypeId?.ToDefinitionString();
            index.GcimpMethylation = cellLine.GcimpMethylation;

            return index;
        }

        private CellLineBaseIndex[] CreateCellLineBaseIndices(in IEnumerable<CellLine> cellLines)
        {
            if(cellLines == null)
            {
                return null;
            }

            var indices = cellLines.Select(cellLine =>
            {
                var index = CreateCellLineBaseIndex(cellLine);

                return index;
            })
            .ToArray();

            return indices;
        }

        private CellLine[] LoadCellLines(string donorId)
        {
            var cellLines = _database.CellLines
                .Include(cellLine => cellLine.Parent)
                .Include(cellLine => cellLine.Childern)
                .Where(cellLine => cellLine.DonorId == donorId)
                .ToArray();

            return cellLines;
        }


        private MutationIndex[] CreateMutationIndices(int sampleId)
        {
            var mutations = LoadMutations(sampleId);

            if (!mutations.Any())
            {
                return null;
            }

            var indices = mutations.Select(mutation =>
            {
                var index = new MutationIndex();

                index.Id = mutation.Id;
                index.Code = mutation.Code;
                index.Name = mutation.Name;
                index.Chromosome = mutation.ChromosomeId?.ToDefinitionString();
                index.Contig = mutation.Contig?.Value;
                index.SequenceType = mutation.SequenceTypeId.ToDefinitionString();
                index.Position = mutation.Position;
                index.Type = mutation.TypeId.ToDefinitionString();

                index.Gene = CreateGeneIndex(mutation.Gene);

                return index;
            })
            .ToArray();

            return indices;
        }

        private Mutation[] LoadMutations(int sampleId)
        {
            var mutations = _database.SampleMutations
                .Include(sampleMutation => sampleMutation.Mutation)
                .Include(sampleMutation => sampleMutation.Mutation.Gene)
                .Include(sampleMutation => sampleMutation.Mutation.Contig)
                .Where(sampleMutation => sampleMutation.SampleId == sampleId)
                .Select(sampleMutation => sampleMutation.Mutation)
                .ToArray();

            return mutations;
        }

        private GeneIndex CreateGeneIndex(in Gene gene)
        {
            if(gene == null)
            {
                return null;
            }

            var index = new GeneIndex();

            index.Id = gene.Id;
            index.Name = gene.Name;

            return index;
        }
    }
}
