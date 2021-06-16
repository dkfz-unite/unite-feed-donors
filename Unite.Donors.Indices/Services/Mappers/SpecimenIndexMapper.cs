using Unite.Data.Entities.Molecular;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Cells;
using Unite.Data.Entities.Specimens.Tissues;
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
            index.MolecularData = CreateFrom(specimen.MolecularData);
        }


        private TissueIndex CreateFrom(in Tissue tissue)
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

        private CellLineIndex CreateFrom(in CellLine cellLine)
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
            index.PassageNumber = cellLine.PassageNumber;

            index.Name = cellLine.Info?.Name;
            index.DepositorName = cellLine.Info?.DepositorName;
            index.DepositorEstablishment = cellLine.Info?.DepositorEstablishment;
            index.EstablishmentDate = cellLine.Info?.EstablishmentDate;

            index.PubMedLink = cellLine.Info?.PubMedLink;
            index.AtccLink = cellLine.Info?.AtccLink;
            index.ExPasyLink = cellLine.Info?.ExPasyLink;

            return index;
        }

        private MolecularDataIndex CreateFrom(in MolecularData molecularData)
        {
            if (molecularData == null)
            {
                return null;
            }

            var index = new MolecularDataIndex();

            index.GeneExpressionSubtype = molecularData.GeneExpressionSubtypeId?.ToDefinitionString();
            index.IdhStatus = molecularData.IdhStatusId?.ToDefinitionString();
            index.IdhMutation = molecularData.IdhMutationId?.ToDefinitionString();
            index.MethylationStatus = molecularData.MethylationStatusId?.ToDefinitionString();
            index.MethylationType = molecularData.MethylationTypeId?.ToDefinitionString();
            index.GcimpMethylation = molecularData.GcimpMethylation;

            return index;
        }
    }
}
