namespace Unite.Donors.Feed.Web.Configuration.Options;

public class ProjectsIndexingOptions
{
    /// <summary>
    /// Indexing bucket size
    /// </summary>
    public int BucketSize
    {
        get
        {
            var option = Environment.GetEnvironmentVariable("UNITE_PROJECTS_INDEXING_BUCKET_SIZE");
            var size = int.Parse(option);

            return size;
        }
    }
}
