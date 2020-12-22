using Unite.Data.Services.Configuration.Options;

namespace Unite.Donors.DataFeed.Web.Configuration.Options
{
    public class MySqlOptions: IMySqlOptions
    {
        public string Host => EnvironmentConfig.MySqlHost;
        public string Database => EnvironmentConfig.MySqlDatabase;
        public string User => EnvironmentConfig.MySqlUser;
        public string Password => EnvironmentConfig.MySqlPassword;
    }
}
