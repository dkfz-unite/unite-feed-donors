using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Molecular;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Cells;
using Unite.Data.Entities.Specimens.Organoids;
using Unite.Data.Entities.Specimens.Tissues;
using Unite.Data.Entities.Specimens.Xenografts;
using Unite.Data.Extensions;
using Unite.Indices.Entities.Basic.Molecular;
using Unite.Indices.Entities.Basic.Specimens;

namespace Unite.Donors.Indices.Services.Mappers
{
    public class SpecimenIndexMapper
    {
        public void Map(in Specimen specimen, SpecimenIndex index)
        {
            if (specimen == null)
            {
                return;
            }

            index.Id = specimen.Id;

            index.Tissue = CreateFrom(specimen.Tissue);
            index.CellLine = CreateFrom(specimen.CellLine);
            index.Organoid = CreateFrom(specimen.Organoid);
            index.Xenograft = CreateFrom(specimen.Xenograft);
            index.MolecularData = CreateFrom(specimen.MolecularData);
        }


        private static TissueIndex CreateFrom(in Tissue tissue)
        {
            if (tissue == null)
            {
                return null;
            }

            var index = new TissueIndex();

            index.ReferenceId = tissue.ReferenceId;

            index.Type = tissue.TypeId?.ToDefinitionString();
            index.TumorType = tissue.TumorTypeId?.ToDefinitionString();
            index.Source = tissue.Source?.Value;
            index.ExtractionDate = tissue.ExtractionDate;

            return index;
        }

        private static CellLineIndex CreateFrom(in CellLine cellLine)
        {
            if (cellLine == null)
            {
                return null;
            }

            var index = new CellLineIndex();

            index.ReferenceId = cellLine.ReferenceId;

            index.Species = cellLine.SpeciesId?.ToDefinitionString();
            index.Type = cellLine.TypeId?.ToDefinitionString();
            index.CultureType = cellLine.CultureTypeId?.ToDefinitionString();

            index.Name = cellLine.Info?.Name;
            index.DepositorName = cellLine.Info?.DepositorName;
            index.DepositorEstablishment = cellLine.Info?.DepositorEstablishment;
            index.EstablishmentDate = cellLine.Info?.EstablishmentDate;

            index.PubMedLink = cellLine.Info?.PubMedLink;
            index.AtccLink = cellLine.Info?.AtccLink;
            index.ExPasyLink = cellLine.Info?.ExPasyLink;

            return index;
        }

        private static OrganoidIndex CreateFrom(in Organoid organoid)
        {
            if (organoid == null)
            {
                return null;
            }

            var index = new OrganoidIndex();

            index.ReferenceId = organoid.ReferenceId;
            index.ImplantedCellsNumber = organoid.ImplantedCellsNumber;
            index.Tumorigenicity = organoid.Tumorigenicity;
            index.Medium = organoid.Medium;

            index.Interventions = CreateFrom(organoid.Interventions);

            return index;
        }

        private static OrganoidInterventionIndex[] CreateFrom(in IEnumerable<OrganoidIntervention> interventions)
        {
            if (interventions == null || !interventions.Any())
            {
                return null;
            }

            var indices = interventions.Select(intervention =>
            {
                var index = new OrganoidInterventionIndex();

                index.Type = intervention.Type.Name;
                index.Details = intervention.Details;
                index.StartDay = intervention.StartDay;
                index.DurationDays = intervention.DurationDays;
                index.Results = intervention.Results;

                return index;

            }).ToArray();

            return indices;
        }

        private static XenograftIndex CreateFrom(in Xenograft xenograft)
        {
            if (xenograft == null)
            {
                return null;
            }

            var index = new XenograftIndex();

            index.ReferenceId = xenograft.ReferenceId;

            index.MouseStrain = xenograft.MouseStrain;
            index.GroupSize = xenograft.GroupSize;
            index.ImplantType = xenograft.ImplantTypeId?.ToDefinitionString();
            index.TissueLocation = xenograft.TissueLocationId?.ToDefinitionString();
            index.ImplantedCellsNumber = xenograft.ImplantedCellsNumber;
            index.Tumorigenicity = xenograft.Tumorigenicity;
            index.TumorGrowthForm = xenograft.TumorGrowthFormId?.ToDefinitionString();
            index.SurvivalDaysFrom = xenograft.SurvivalDaysFrom;
            index.SurvivalDaysTo = xenograft.SurvivalDaysTo;

            index.Interventions = CreateFrom(xenograft.Interventions);

            return index;
        }

        private static XenograftInterventionIndex[] CreateFrom(in IEnumerable<XenograftIntervention> interventions)
        {
            if (interventions == null || !interventions.Any())
            {
                return null;
            }

            var indices = interventions.Select(intervention =>
            {
                var index = new XenograftInterventionIndex();

                index.Type = intervention.Type.Name;
                index.Details = intervention.Details;
                index.StartDay = intervention.StartDay;
                index.DurationDays = intervention.DurationDays;
                index.Results = intervention.Results;

                return index;

            }).ToArray();

            return indices;
        }

        private static MolecularDataIndex CreateFrom(in MolecularData molecularData)
        {
            if (molecularData == null)
            {
                return null;
            }

            var index = new MolecularDataIndex();

            index.MgmtStatus = molecularData.MgmtStatusId?.ToDefinitionString();
            index.IdhStatus = molecularData.IdhStatusId?.ToDefinitionString();
            index.IdhMutation = molecularData.IdhMutationId?.ToDefinitionString();
            index.GeneExpressionSubtype = molecularData.GeneExpressionSubtypeId?.ToDefinitionString();
            index.MethylationSubtype = molecularData.MethylationSubtypeId?.ToDefinitionString();
            index.GcimpMethylation = molecularData.GcimpMethylation;

            return index;
        }
    }
}
