using Unite.Cache.Configuration.Options;
using Unite.Data.Context.Services;

namespace Unite.Donors.Feed.Web.Configuration.Options;

public class MongoOptions : IMongoOptions
{
    public string Host => Environment.GetEnvironmentVariable("UNITE_MONGO_HOST");
    public string Port => Environment.GetEnvironmentVariable("UNITE_MONGO_PORT");
    public string User => Environment.GetEnvironmentVariable("UNITE_MONGO_USER");
    public string Password => Environment.GetEnvironmentVariable("UNITE_MONGO_PASSWORD");
}

