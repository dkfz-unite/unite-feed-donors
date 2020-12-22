namespace Unite.Donors.DataFeed.Web.Services
{
    public interface ITaskProcessingService
    {
        void ProcessIndexingTasks(int bucketSize);
    }
}
