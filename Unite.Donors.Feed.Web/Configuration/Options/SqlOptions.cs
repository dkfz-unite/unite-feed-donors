using System;
using Unite.Data.Services.Configuration.Options;

namespace Unite.Donors.DataFeed.Web.Configuration.Options
{
    public class SqlOptions: ISqlOptions
    {
        public string Host => Environment.GetEnvironmentVariable("UNITE_SQL_HOST");
        public string Database => Environment.GetEnvironmentVariable("UNITE_SQL_DATABASE");
        public string User => Environment.GetEnvironmentVariable("UNITE_SQL_USER");
        public string Password => Environment.GetEnvironmentVariable("UNITE_SQL_PASSWORD");
    }
}
