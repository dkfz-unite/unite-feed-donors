using System.Collections.Generic;
using System.Linq;
using Unite.Data.Entities;
using Unite.Data.Entities.Extensions;
using Unite.Data.Entities.Mutations;
using Unite.Indices.Entities.Basic;
using Unite.Indices.Entities.Basic.Mutations;

namespace Unite.Donors.DataFeed.Web.Services.Indices.Extensions
{
    public static class SampleIndexMappingExtensions
    {
        public static void MapFrom(this AnalysedSampleIndex index, in AnalysedSample analysedSample)
        {
            if (analysedSample is null)
            {
                return;
            }

            index.MapFrom(analysedSample.Sample);

            index.Analysis = CreateFrom(analysedSample.Analysis);
            index.MatchedSamples = CreateFrom(analysedSample.MatchedSamples);
        }

        public static void MapFrom(this SampleIndex index, in Sample sample)
        {
            if (sample is null)
            {
                return;
            }

            index.Id = sample.Id;
            index.Name = sample.Name;
            index.Type = sample.TypeId?.ToDefinitionString();
            index.Subtype = sample.SubtypeId?.ToDefinitionString();
            index.Date = sample.Date;
        }

        private static AnalysisIndex CreateFrom(in Analysis analysis)
        {
            if (analysis is null)
            {
                return null;
            }

            var index = new AnalysisIndex();

            index.Id = analysis.Id;
            index.Name = analysis.Name;
            index.Type = analysis.TypeId?.ToDefinitionString();
            index.Date = analysis.Date;

            index.File = CreateFrom(analysis.File);

            return index;
        }

        private static FileIndex CreateFrom(in File file)
        {
            if (file is null)
            {
                return null;
            }

            var index = new FileIndex();

            index.Id = file.Id;
            index.Name = file.Name;
            index.Link = file.Link;
            index.Created = file.Created;
            index.Updated = file.Updated;

            return index;
        }

        private static SampleIndex[] CreateFrom(in IEnumerable<MatchedSample> matchedSamples)
        {
            if (matchedSamples is null)
            {
                return null;
            }

            var indices = matchedSamples.Select(matchedSample =>
            {
                var index = new SampleIndex();

                index.MapFrom(matchedSample.Matched.Sample);

                return index;

            }).ToArray();

            return indices;
        }
    }
}
