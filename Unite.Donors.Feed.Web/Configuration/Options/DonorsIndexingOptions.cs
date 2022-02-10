using System;

namespace Unite.Donors.Feed.Web.Configuration.Options
{
    public class DonorsIndexingOptions
    {
        /// <summary>
        /// Indexing interval in milliseconds
        /// </summary>
        public int Interval
        {
            get
            {
                var option = Environment.GetEnvironmentVariable("UNITE_INDEXING_INTERVAL");
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
                var option = Environment.GetEnvironmentVariable("UNITE_INDEXING_BUCKET_SIZE");
                var size = int.Parse(option);

                return size;
            }
        }
    }
}
