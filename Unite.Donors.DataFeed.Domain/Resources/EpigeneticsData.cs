using Unite.Data.Entities.Epigenetics.Enums;

namespace Unite.Donors.DataFeed.Domain.Resources
{
    public class EpigeneticsData
    {
        public GeneExpressionSubtype? GeneExpressionSubtypeId { get; set; }
        public IDHStatus? IdhStatusId { get; set; }
        public IDHMutation? IdhMutationId { get; set; }
        public MethylationStatus? MethylationStatusId { get; set; }
        public MethylationSubtype? MethylationSubtypeId { get; set; }
        public bool? GcimpMethylation { get; set; }

        public void Sanitise()
        {

        }
    }
}
