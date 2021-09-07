using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unite.Donors.Feed.Web.Configuration.Options;
using Unite.Donors.Feed.Web.Handlers;

namespace Unite.Donors.Feed.Web.HostedServices
{
    public class DonorsIndexingHostedService : BackgroundService
    {
        private readonly IndexingOptions _options;
        private readonly DonorsIndexingHandler _handler;
        private readonly ILogger _logger;

        public DonorsIndexingHostedService(
            IndexingOptions options,
            DonorsIndexingHandler handler,
            ILogger<DonorsIndexingHostedService> logger)
        {
            _options = options;
            _handler = handler;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
			_logger.LogInformation("Indexing service started");

            cancellationToken.Register(() => _logger.LogInformation("Indexing service stopped"));

			while (!cancellationToken.IsCancellationRequested)
			{
                try
                {
                    _handler.Handle(_options.BucketSize);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.Message);

                    if (exception.InnerException != null)
                    {
                        _logger.LogError(exception.InnerException.Message);
                    }
                }
                finally
                {
                    await Task.Delay(_options.Interval, cancellationToken);
                }
			}
		}
    }
}
