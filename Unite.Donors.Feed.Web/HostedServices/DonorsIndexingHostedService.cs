using Unite.Donors.Feed.Web.Configuration.Options;
using Unite.Donors.Feed.Web.Handlers;

namespace Unite.Donors.Feed.Web.HostedServices;

public class DonorsIndexingHostedService : BackgroundService
{
    private readonly DonorsIndexingOptions _options;
    private readonly DonorsIndexingHandler _handler;
    private readonly ILogger _logger;

    public DonorsIndexingHostedService(
        DonorsIndexingOptions options,
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

        // Delay 5 seconds to let the web api start working
        await Task.Delay(5000, cancellationToken);

        try
        {
            _handler.Prepare();
        }
        catch (Exception exception)
        {
            LogError(exception);
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _handler.Handle(_options.BucketSize);
            }
            catch (Exception exception)
            {
                LogError(exception);
            }
            finally
            {
                await Task.Delay(10000, cancellationToken);
            }
        }
    }

    private void LogError(Exception exception)
    {
        _logger.LogError(exception.Message);

        if (exception.InnerException != null)
        {
            _logger.LogError(exception.InnerException.Message);
        }
    }
}
