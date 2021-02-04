using Unite.Data.Entities.Epigenetics.Enums;

namespace Unite.Donors.DataFeed.Domain.Resources
{
    public class EpigeneticsData
    {
        public GeneExpressionSubtype? GeneExpressionSubtype { get; set; }
        public IDHStatus? IdhStatus { get; set; }
        public IDHMutation? IdhMutation { get; set; }
        public MethylationStatus? MethylationStatus { get; set; }
        public MethylationSubtype? MethylationSubtype { get; set; }
        public bool? GcimpMethylation { get; set; }

        public void Sanitise()
        {

        }
    }
}
