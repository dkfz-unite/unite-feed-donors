using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Cells;
using Unite.Data.Entities.Specimens.Organoids;
using Unite.Data.Entities.Specimens.Tissues;
using Unite.Data.Entities.Specimens.Xenografts;
using Unite.Data.Extensions;
using Unite.Indices.Entities.Basic.Specimens;

using OrganoidIntervention = Unite.Data.Entities.Specimens.Organoids.Intervention;
using XenograftIntervention = Unite.Data.Entities.Specimens.Xenografts.Intervention;

namespace Unite.Donors.Indices.Services.Mappers;

internal class SpecimenIndexMapper
{
    internal void Map(in Specimen specimen, SpecimenIndex index, DateOnly? diagnosisDate)
    {
        if (specimen == null)
        {
            return;
        }

        index.Id = specimen.Id;
        index.ParentId = specimen.ParentId;
        index.CreationDay = specimen.CreationDate.RelativeFrom(diagnosisDate) ?? specimen.CreationDay;

        index.Tissue = CreateFrom(specimen.Tissue);
        index.CellLine = CreateFrom(specimen.CellLine);
        index.Organoid = CreateFrom(specimen.Organoid, specimen.CreationDate);
        index.Xenograft = CreateFrom(specimen.Xenograft, specimen.CreationDate);

        index.MolecularData = CreateFrom(specimen.MolecularData);
        index.DrugScreenings = CreateFrom(specimen.DrugScreenings);
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

    private static OrganoidIndex CreateFrom(in Organoid organoid, DateOnly? specimenCreationDate)
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

        index.Interventions = CreateFrom(organoid.Interventions, specimenCreationDate);

        return index;
    }

    private static OrganoidInterventionIndex[] CreateFrom(in IEnumerable<OrganoidIntervention> interventions, DateOnly? specimenCreationDate)
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
            index.StartDay = intervention.StartDate.RelativeFrom(specimenCreationDate) ?? intervention.StartDay;
            index.DurationDays = intervention.EndDate.RelativeFrom(intervention.StartDate) ?? intervention.DurationDays;
            index.Results = intervention.Results;

            return index;

        }).ToArray();

        return indices;
    }

    private static XenograftIndex CreateFrom(in Xenograft xenograft, DateOnly? specimenCreationDate)
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

        index.Interventions = CreateFrom(xenograft.Interventions, specimenCreationDate);

        return index;
    }

    private static XenograftInterventionIndex[] CreateFrom(in IEnumerable<XenograftIntervention> interventions, DateOnly? specimenCreationDate)
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
            index.StartDay = intervention.StartDate.RelativeFrom(specimenCreationDate) ?? intervention.StartDay;
            index.DurationDays = intervention.EndDate.RelativeFrom(intervention.StartDate) ?? intervention.DurationDays;
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

    private static DrugScreeningIndex[] CreateFrom(in IEnumerable<DrugScreening> screenings)
    {
        if (screenings == null || !screenings.Any())
        {
            return null;
        }

        var indices = screenings.Select(screening =>
        {
            var index = new DrugScreeningIndex();

            index.Dss = screening.Dss;
            index.DssSelective = screening.DssSelective;
            index.Gof = screening.Gof;
            index.Drug = screening.Drug.Name;
            index.MinConcentration = screening.MinConcentration;
            index.MaxConcentration = screening.MaxConcentration;
            index.AbsIC25 = screening.AbsIC25;
            index.AbsIC50 = screening.AbsIC50;
            index.AbsIC75 = screening.AbsIC75;

            return index;

        }).ToArray();

        return indices;
    }
}
