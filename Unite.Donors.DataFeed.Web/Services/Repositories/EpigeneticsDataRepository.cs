using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Epigenetics;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public class EpigeneticsDataRepository : Repository<EpigeneticsData>
    {
        public EpigeneticsDataRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
        }

        protected override void Map(in EpigeneticsData source, ref EpigeneticsData target)
        {
            target.DonorId = source.DonorId;
            target.GeneExpressionSubtypeId = source.GeneExpressionSubtypeId;
            target.IdhStatusId = source.IdhStatusId;
            target.IdhMutationId = source.IdhMutationId;
            target.MethylationStatusId = source.MethylationStatusId;
            target.MethylationSubtypeId = source.MethylationSubtypeId;
            target.GcimpMethylation = source.GcimpMethylation;
        }
    }
}
