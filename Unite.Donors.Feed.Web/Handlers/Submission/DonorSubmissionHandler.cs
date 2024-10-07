using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Web.Services;
using Unite.Donors.Feed.Web.Submissions;

namespace Unite.Donors.Feed.Web.Handlers.Submission;

public class DonorSubmissionHandler
{
    private readonly DonorsWriter _donorDataWriter;
    private readonly DonorIndexingTasksService _donorIndexingTaskService;
    private readonly DonorSubmissionService _donorSubmissionService;
    private readonly TasksProcessingService _taskProcessingService;

    private readonly Models.Donors.Converters.DonorModelConverter _donorModelConverter;

    private readonly ILogger _logger;

    public DonorSubmissionHandler
           (DonorsWriter donorDataWriter,
           DonorIndexingTasksService donorIndexingTasksService,
           DonorSubmissionService donorSubmissionService,
           TasksProcessingService tasksProcessingService,
           ILogger<DonorSubmissionHandler> logger)
    {
        _donorDataWriter = donorDataWriter;
        _donorIndexingTaskService = donorIndexingTasksService;
        _donorSubmissionService = donorSubmissionService;
        _taskProcessingService = tasksProcessingService;
        _logger = logger;

        _donorModelConverter = new Models.Donors.Converters.DonorModelConverter ();
    }


    public void Handle()
    {
        ProcessSubmissionTasks();
    }

    private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(SubmissionTaskType.DON, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed Donors data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedData = _donorSubmissionService.FindDonorSubmission(submissionId);
        var convertedData = _donorModelConverter.Convert(submittedData);

        _donorDataWriter.SaveData(convertedData, out var audit);

        _donorIndexingTaskService.PopulateTasks(audit.Donors);

        _donorSubmissionService.DeleteDonorSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}