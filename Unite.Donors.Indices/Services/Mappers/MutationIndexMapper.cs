using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Extensions;
using Unite.Data.Utilities.Mutations;
using Unite.Indices.Entities.Basic.Mutations;

namespace Unite.Donors.Indices.Services.Mappers
{
    public class MutationIndexMapper
    {
        public void Map(in Mutation mutation, MutationIndex index)
        {
            if (mutation == null)
            {
                return;
            }

            index.Id = mutation.Id;
            index.Code = mutation.Code;
            index.Chromosome = mutation.ChromosomeId.ToDefinitionString();
            index.SequenceType = mutation.SequenceTypeId.ToDefinitionString();
            index.Start = mutation.Start;
            index.End = mutation.End;
            index.Type = mutation.TypeId.ToDefinitionString();
            index.Ref = mutation.ReferenceBase;
            index.Alt = mutation.AlternateBase;

            index.AffectedTranscripts = CreateFrom(mutation.AffectedTranscripts);
        }


        private static AffectedTranscriptIndex[] CreateFrom(in IEnumerable<AffectedTranscript> affectedTranscripts)
        {
            if (affectedTranscripts == null || !affectedTranscripts.Any())
            {
                return null;
            }

            var indices = affectedTranscripts.Select(affectedTranscript =>
            {
                var index = new AffectedTranscriptIndex();

                index.Id = affectedTranscript.Id;

                index.AminoAcidChange = AAChangeCodeGenerator.Generate(
                    affectedTranscript.ProteinStart,
                    affectedTranscript.ProteinEnd,
                    affectedTranscript.AminoAcidChange
                );

                index.CodonChange = CodonChangeCodeGenerator.Generate(
                    affectedTranscript.CDSStart,
                    affectedTranscript.CDSEnd,
                    affectedTranscript.CodonChange
                );

                index.Gene = CreateFrom(affectedTranscript.Gene);
                index.Transcript = CreateFrom(affectedTranscript.Transcript);
                index.Consequences = CreateFrom(affectedTranscript.Consequences);

                return index;

            }).ToArray();

            return indices;
        }

        private static GeneIndex CreateFrom(in Gene gene)
        {
            if (gene == null)
            {
                return null;
            }

            var index = new GeneIndex();

            index.Id = gene.Id;
            index.Symbol = gene.Symbol;
            index.Strand = gene.Strand;
            index.Biotype = gene.Biotype?.Value;

            index.EnsemblId = gene.Info?.EnsemblId;

            return index;
        }

        private static TranscriptIndex CreateFrom(in Transcript transcript)
        {
            if (transcript == null)
            {
                return null;
            }

            var index = new TranscriptIndex();

            index.Id = transcript.Id;
            index.Symbol = transcript.Symbol;
            index.Strand = transcript.Strand;
            index.Biotype = transcript.Biotype?.Value;

            index.EnsemblId = transcript.Info?.EnsemblId;

            return index;
        }

        private static ConsequenceIndex[] CreateFrom(in IEnumerable<AffectedTranscriptConsequence> consequences)
        {
            if (consequences == null || !consequences.Any())
            {
                return null;
            }

            var indices = consequences.Select(affectedTranscriptConsequence =>
            {
                var index = new ConsequenceIndex();

                index.Type = affectedTranscriptConsequence.Consequence.TypeId.ToDefinitionString();
                index.Impact = affectedTranscriptConsequence.Consequence.ImpactId.ToDefinitionString();
                index.Severity = affectedTranscriptConsequence.Consequence.Severity;

                return index;

            }).ToArray();

            return indices;
        }
    }
}
