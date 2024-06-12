using Unite.Donors.Feed.Web.Configuration.Options;
using Unite.Donors.Feed.Web.Handlers;
using Unite.Essentials.Extensions;

namespace Unite.Donors.Feed.Web.Workers;

public class DonorsIndexingWorker : BackgroundService
{
    private readonly DonorsIndexingOptions _options;
    private readonly DonorsIndexingHandler _handler;
    private readonly ILogger _logger;

    public DonorsIndexingWorker(
        DonorsIndexingOptions options,
        DonorsIndexingHandler handler,
        ILogger<DonorsIndexingWorker> logger)
    {
        _options = options;
        _handler = handler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Indexing worker started");

        cancellationToken.Register(() => _logger.LogInformation("Indexing worker stopped"));

        // Delay 5 seconds to let the web api start working
        await Task.Delay(5000, cancellationToken);

        try
        {
            await _handler.Prepare();
        }
        catch (Exception exception)
        {
            _logger.LogError("{error}", exception.GetShortMessage());
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await _handler.Handle(_options.BucketSize);
            }
            catch (Exception exception)
            {
                _logger.LogError("{error}", exception.GetShortMessage());
            }
            finally
            {
                await Task.Delay(10000, cancellationToken);
            }
        }
    }
}
