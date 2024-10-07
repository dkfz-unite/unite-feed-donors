using Unite.Essentials.Extensions;
using Unite.Donors.Feed.Web.Handlers.Submission;

namespace Unite.Donors.Feed.Web.Workers;

public class SubmissionsWorker : BackgroundService
{
    private readonly DonorsSubmissionHandler _donorsSubmissionHandler;
    private readonly TreatmentsSubmissionHandler _treatmentsSubmissionHandler;
    private readonly ILogger _logger;

    public SubmissionsWorker(
        DonorsSubmissionHandler donorsSubmissionHandler,
        TreatmentsSubmissionHandler treatmentsSubmissionHandler,
        ILogger<SubmissionsWorker> logger)
    {
        _donorsSubmissionHandler = donorsSubmissionHandler;
        _treatmentsSubmissionHandler = treatmentsSubmissionHandler;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Submissions worker started");

        stoppingToken.Register(() => _logger.LogInformation("Submissions worker stopped"));

        // Delay 5 seconds to let the web api start working
        await Task.Delay(5000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _donorsSubmissionHandler.Handle();
                _treatmentsSubmissionHandler.Handle();
            }
            catch (Exception exception)
            {
                _logger.LogError("{error}", exception.GetShortMessage());
            }
            finally
            {
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
