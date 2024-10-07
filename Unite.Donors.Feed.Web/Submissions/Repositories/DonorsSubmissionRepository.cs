using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Donors.Feed.Web.Models.Donors;

namespace Unite.Donors.Feed.Web.Submissions.Repositories;

public class DonorsSubmissionRepository : CacheRepository<DonorModel[]>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "don";

    public DonorsSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
