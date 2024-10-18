using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Donors.Feed.Web.Models.Donors;

namespace Unite.Donors.Feed.Web.Submissions.Repositories;

public class TreatmentsSubmissionRepository : CacheRepository<TreatmentsModel[]>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "don_trt";

    public TreatmentsSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
