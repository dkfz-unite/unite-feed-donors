using System;

namespace Unite.Donors.DataFeed.Web.Configuration
{
    public static class EnvironmentConfig
    {
        private static string _defaultSqlHost = "localhost";
        private static string _defaultSqlDatabase = "unite";
        private static string _defaultSqlUser = "root";
        private static string _defaultSqlPassword = "Long-p@55w0rd";

        private static string _defaultElasticHost = "http://localhost:9200";
        private static string _defaultElasticUser = "elastic";
        private static string _defaultElasticPassword = "Long-p@55w0rd";

        private static string _defaultIndexingInterval = "60";
        private static string _defaultIndexingBucketSize = "300";


        public static string SqlHost => GetEnvironmentVariable("UNITE_SQL_HOST", _defaultSqlHost);
        public static string SqlDatabase = GetEnvironmentVariable("UNITE_SQL_DATABASE", _defaultSqlDatabase);
        public static string SqlUser => GetEnvironmentVariable("UNITE_SQL_USER", _defaultSqlUser);
        public static string SqlPassword = GetEnvironmentVariable("UNITE_SQL_PASSWORD", _defaultSqlPassword);

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
