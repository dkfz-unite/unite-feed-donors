namespace Unite.Donors.DataFeed.Web.Configuration.Options
{
    public class IndexingOptions
    {
        /// <summary>
        /// Indexing interval in milliseconds
        /// </summary>
        public int Interval
        {
            get
            {
                var option = EnvironmentConfig.IndexingInterval;
                var seconds = int.Parse(option);

                return seconds * 1000;
            }
        }

        /// <summary>
        /// Indexing bucket size
        /// </summary>
        public int BucketSize
        {
            get
            {
                var option = EnvironmentConfig.IndexingBucketSize;
                var size = int.Parse(option);

                return size;
            }
        }
    }
}