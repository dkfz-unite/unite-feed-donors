using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unite.Donors.DataFeed.Web.Configuration.Options;
using Unite.Donors.DataFeed.Web.Services;

namespace Unite.Donors.DataFeed.Web.HostedServices
{
    public class IndexingHostedService : BackgroundService
    {
        private readonly IndexingOptions _options;
        private readonly ITaskProcessingService _taskProcessingService;
        private readonly ILogger _logger;

        public IndexingHostedService(
            IndexingOptions options,
            ITaskProcessingService taskProcessingService,
            ILogger<IndexingHostedService> logger)
        {
            _options = options;
            _taskProcessingService = taskProcessingService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
			_logger.LogInformation("Processing service started");

            cancellationToken.Register(() => _logger.LogInformation("Processing service stopped"));

			while (!cancellationToken.IsCancellationRequested)
			{
                _taskProcessingService.ProcessIndexingTasks(_options.BucketSize);

				await Task.Delay(_options.Interval, cancellationToken);
			}
		}
    }
}
