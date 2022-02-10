using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Mutations;
using Unite.Data.Extensions;
using Unite.Data.Utilities.Mutations;
using Unite.Indices.Entities.Basic.Genome;
using Unite.Indices.Entities.Basic.Genome.Mutations;

namespace Unite.Donors.Indices.Services.Mappers
{
    internal class MutationIndexMapper
    {
        internal void Map(in Mutation mutation, MutationIndex index)
        {
            if (mutation == null)
            {
                return;
            }

            index.Id = mutation.Id;
            index.Code = mutation.Code;
            index.Chromosome = mutation.ChromosomeId.ToDefinitionString();
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

                index.Consequences = CreateFrom(affectedTranscript.Consequences);

                index.Transcript = CreateFrom(affectedTranscript.Transcript);

                return index;

            }).ToArray();

            return indices;
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
            index.Biotype = transcript.Biotype?.Value;
            index.Chromosome = transcript.ChromosomeId.ToDefinitionString();
            index.Start = transcript.Start;
            index.End = transcript.End;
            index.Strand = transcript.Strand;

            index.EnsemblId = transcript.Info?.EnsemblId;

            index.Gene = CreateFrom(transcript.Gene);
            index.Protein = CreateFrom(transcript.Protein);

            return index;
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
            index.Biotype = gene.Biotype?.Value;
            index.Chromosome = gene.ChromosomeId.ToDefinitionString();
            index.Start = gene.Start;
            index.End = gene.End;
            index.Strand = gene.Strand;

            index.EnsemblId = gene.Info?.EnsemblId;

            return index;
        }

        private static ProteinIndex CreateFrom(in Protein protein)
        {
            if (protein == null)
            {
                return null;
            }

            var index = new ProteinIndex();

            index.Id = protein.Id;
            index.Symbol = protein.Symbol;
            index.Start = protein.Start;
            index.End = protein.End;
            index.Length = protein.Length;

            index.EnsemblId = protein.Info?.EnsemblId;

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
