using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Donors.Feed.Web.Models.Donors;
// using Unite.Donors.Feed.Web.Models.Base;

namespace Unite.Donors.Feed.Web.Submissions.Repositories;

public class TreatmentsSubmissionRepository : CacheRepository<TreatmentsModel[]>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "don_treat";

    public TreatmentsSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
