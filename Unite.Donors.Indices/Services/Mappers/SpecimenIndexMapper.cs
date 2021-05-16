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
        public void Map(in Specimen sample, SpecimenIndex index)
        {
            if (sample == null)
            {
                return;
            }

            index.Id = sample.Id;
            index.ReferenceId = sample.ReferenceId;

            index.Tissue = CreateFrom(sample.Tissue);
            index.CellLine = CreateFrom(sample.CellLine);
            index.MolecularData = CreateFrom(sample.MolecularData);
        }


        private TissueIndex CreateFrom(in Tissue tissue)
        {
            if (tissue == null)
            {
                return null;
            }

            var index = new TissueIndex();

            index.Type = tissue.TypeId?.ToDefinitionString();
            index.TumourType = tissue.TumourTypeId?.ToDefinitionString();
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

            index.Name = cellLine.Name;
            index.Type = cellLine.TypeId?.ToDefinitionString();
            index.Species = cellLine.SpeciesId?.ToDefinitionString();

            index.DepositorName = cellLine?.Info.DepositorName;
            index.DepositorEstablishment = cellLine?.Info.DepositorEstablishment;
            index.EstablishmentDate = cellLine?.Info.EstablishmentDate;
            index.PublicationId = cellLine?.Info.PublicationId;
            index.AtccId = cellLine?.Info.AtccId;
            index.ExPasyId = cellLine?.Info.ExPasyId;

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
            index.MethylationSubtype = molecularData.MethylationSubtypeId?.ToDefinitionString();
            index.GcimpMethylation = molecularData.GcimpMethylation;

            return index;
        }
    }
}
