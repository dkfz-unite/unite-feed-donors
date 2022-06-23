using System.Linq.Expressions;
using Unite.Indices.Entities.Donors;
using Unite.Indices.Services;
using Unite.Indices.Services.Configuration.Options;

namespace Unite.Donors.Indices.Services;

public class DonorsIndexingService : IndexingService<DonorIndex>
{
    protected override string DefaultIndex
    {
        get { return "donors"; }
    }

    protected override Expression<Func<DonorIndex, object>> IdProperty
    {
        get { return (donor) => donor.Id; }
    }

    public DonorsIndexingService(IElasticOptions options) : base(options)
    {
    }
}
