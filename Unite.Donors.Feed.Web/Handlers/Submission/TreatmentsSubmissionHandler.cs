using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Web.Services;
using Unite.Donors.Feed.Web.Submissions;

namespace Unite.Donors.Feed.Web.Handlers.Submission;

public class TreatmentsSubmissionHandler
{
    private readonly TreatmentsWriter _treatmentDataWriter;
     private readonly DonorIndexingTasksService _donorIndexingTaskService;
    private readonly DonorSubmissionService _donorSubmissionService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly Models.Donors.Converters.TreatmentsModelConverter _treatmentModelConverter;

    private readonly ILogger _logger;

 public TreatmentsSubmissionHandler
           (TreatmentsWriter treatmentsWriter,
           DonorIndexingTasksService donorIndexingTasksService,
           DonorSubmissionService donorSubmissionService,
           TasksProcessingService tasksProcessingService,
           ILogger<TreatmentsSubmissionHandler> logger)
    {
        _treatmentDataWriter = treatmentsWriter;
        _donorIndexingTaskService = donorIndexingTasksService;
        _donorSubmissionService = donorSubmissionService;
        _taskProcessingService = tasksProcessingService;
        _logger = logger;

        _treatmentModelConverter = new Models.Donors.Converters.TreatmentsModelConverter ();

    }

    public void Handle()
    {
        ProcessSubmissionTasks();
    }

     private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(SubmissionTaskType.DON_TRT, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed Treatments data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
    private void ProcessSubmission(string submissionId)
    {
        var submittedData = _donorSubmissionService.FindTreatmentsSubmission(submissionId);
        var convertedData = _treatmentModelConverter.Convert(submittedData);

        _treatmentDataWriter.SaveData(convertedData, out var audit);

        _donorIndexingTaskService.PopulateTasks(audit.Donors);

        _donorSubmissionService.DeleteTreatmentssSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }


}