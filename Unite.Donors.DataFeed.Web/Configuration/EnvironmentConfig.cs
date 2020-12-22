using System;

namespace Unite.Donors.DataFeed.Web.Configuration
{
    public static class EnvironmentConfig
    {
        private static string _defaultMySqlHost = "localhost";
        private static string _defaultMySqlDatabase = "unite";
        private static string _defaultMySqlUser = "root";
        private static string _defaultMySqlPassword = "Long-p@55w0rd";

        private static string _defaultElasticHost = "http://localhost:9200";
        private static string _defaultElasticUser = "elastic";
        private static string _defaultElasticPassword = "Long-p@55w0rd";

        private static string _defaultIndexingInterval = "60";
        private static string _defaultIndexingBucketSize = "300";


        public static string MySqlHost => GetEnvironmentVariable("UNITE_MYSQL_HOST", _defaultMySqlHost);
        public static string MySqlDatabase = GetEnvironmentVariable("UNITE_MYSQL_DATABASE", _defaultMySqlDatabase);
        public static string MySqlUser => GetEnvironmentVariable("UNITE_MYSQL_USER", _defaultMySqlUser);
        public static string MySqlPassword = GetEnvironmentVariable("UNITE_MYSQL_PASSWORD", _defaultMySqlPassword);

        public static string ElasticHost => GetEnvironmentVariable("UNITE_ELASTIC_HOST", _defaultElasticHost);
        public static string ElasticUser => GetEnvironmentVariable("UNITE_ELASTIC_USER", _defaultElasticUser);
        public static string ElasticPassword => GetEnvironmentVariable("UNITE_ELASTIC_PASSWORD", _defaultElasticPassword);

        public static string IndexingInterval => GetEnvironmentVariable("UNITE_INDEXING_INTERVAL", _defaultIndexingInterval);
        public static string IndexingBucketSize => GetEnvironmentVariable("UNITE_INDEXING_BUCKET_SIZE", _defaultIndexingBucketSize);


        private static string GetEnvironmentVariable(string variable, string defaultValue = null)
        {
            var value = Environment.GetEnvironmentVariable(variable);

            return value ?? defaultValue;
        }
    }
}
