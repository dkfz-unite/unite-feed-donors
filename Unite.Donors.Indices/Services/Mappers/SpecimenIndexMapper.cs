using Unite.Data.Entities.Specimens;
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
        // index.ReferenceId - resolved by property getter
        // index.Type - resolved by property getter
        index.CreationDay = specimen.CreationDate.RelativeFrom(diagnosisDate) ?? specimen.CreationDay;

        index.Tissue = CreateFromTissue(specimen);
        index.Cell = CreateFromCellLine(specimen);
        index.Organoid = CreateFromOrganoid(specimen, specimen.CreationDate);
        index.Xenograft = CreateFromXenograft(specimen, specimen.CreationDate);
    }


    private static TissueIndex CreateFromTissue(in Specimen specimen)
    {
        if (specimen.Tissue == null)
        {
            return null;
        }

        var index = new TissueIndex();

        index.ReferenceId = specimen.Tissue.ReferenceId;

        index.Type = specimen.Tissue.TypeId?.ToDefinitionString();
        index.TumorType = specimen.Tissue.TumorTypeId?.ToDefinitionString();
        index.Source = specimen.Tissue.Source?.Value;

        index.MolecularData = CreateFrom(specimen.MolecularData);

        return index;
    }

    private static CellLineIndex CreateFromCellLine(in Specimen specimen)
    {
        if (specimen.CellLine == null)
        {
            return null;
        }

        var index = new CellLineIndex();

        index.ReferenceId = specimen.CellLine.ReferenceId;

        index.Species = specimen.CellLine.SpeciesId?.ToDefinitionString();
        index.Type = specimen.CellLine.TypeId?.ToDefinitionString();
        index.CultureType = specimen.CellLine.CultureTypeId?.ToDefinitionString();

        index.Name = specimen.CellLine.Info?.Name;
        index.DepositorName = specimen.CellLine.Info?.DepositorName;
        index.DepositorEstablishment = specimen.CellLine.Info?.DepositorEstablishment;
        index.EstablishmentDate = specimen.CellLine.Info?.EstablishmentDate;

        index.PubMedLink = specimen.CellLine.Info?.PubMedLink;
        index.AtccLink = specimen.CellLine.Info?.AtccLink;
        index.ExPasyLink = specimen.CellLine.Info?.ExPasyLink;

        index.MolecularData = CreateFrom(specimen.MolecularData);
        index.DrugScreenings = CreateFrom(specimen.DrugScreenings);

        return index;
    }

    private static OrganoidIndex CreateFromOrganoid(in Specimen specimen, DateOnly? specimenCreationDate)
    {
        if (specimen.Organoid == null)
        {
            return null;
        }

        var index = new OrganoidIndex();

        index.ReferenceId = specimen.Organoid.ReferenceId;
        index.ImplantedCellsNumber = specimen.Organoid.ImplantedCellsNumber;
        index.Tumorigenicity = specimen.Organoid.Tumorigenicity;
        index.Medium = specimen.Organoid.Medium;

        index.MolecularData = CreateFrom(specimen.MolecularData);
        index.DrugScreenings = CreateFrom(specimen.DrugScreenings);
        index.Interventions = CreateFrom(specimen.Organoid.Interventions, specimenCreationDate);

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

    private static XenograftIndex CreateFromXenograft(in Specimen specimen, DateOnly? specimenCreationDate)
    {
        if (specimen.Xenograft == null)
        {
            return null;
        }

        var index = new XenograftIndex();

        index.ReferenceId = specimen.Xenograft.ReferenceId;

        index.MouseStrain = specimen.Xenograft.MouseStrain;
        index.GroupSize = specimen.Xenograft.GroupSize;
        index.ImplantType = specimen.Xenograft.ImplantTypeId?.ToDefinitionString();
        index.TissueLocation = specimen.Xenograft.TissueLocationId?.ToDefinitionString();
        index.ImplantedCellsNumber = specimen.Xenograft.ImplantedCellsNumber;
        index.Tumorigenicity = specimen.Xenograft.Tumorigenicity;
        index.TumorGrowthForm = specimen.Xenograft.TumorGrowthFormId?.ToDefinitionString();
        index.SurvivalDaysFrom = specimen.Xenograft.SurvivalDaysFrom;
        index.SurvivalDaysTo = specimen.Xenograft.SurvivalDaysTo;

        index.MolecularData = CreateFrom(specimen.MolecularData);
        index.DrugScreenings = CreateFrom(specimen.DrugScreenings);
        index.Interventions = CreateFrom(specimen.Xenograft.Interventions, specimenCreationDate);

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
