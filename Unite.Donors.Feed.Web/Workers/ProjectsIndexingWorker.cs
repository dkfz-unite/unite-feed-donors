using Unite.Donors.Feed.Web.Configuration.Options;
using Unite.Donors.Feed.Web.Handlers;
using Unite.Essentials.Extensions;

namespace Unite.Donors.Feed.Web.Workers;

public class ProjectsIndexingWorker : BackgroundService
{
    private readonly ProjectsIndexingOptions _options;
    private readonly ProjectsIndexingHandler _handler;
    private readonly ILogger _logger;

    public ProjectsIndexingWorker(
        ProjectsIndexingOptions options,
        ProjectsIndexingHandler handler,
        ILogger<ProjectsIndexingWorker> logger)
    {
        _options = options;
        _handler = handler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Projects indexing worker started");

        cancellationToken.Register(() => _logger.LogInformation("Projects indexing worker stopped"));

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
