using FluentValidation;
using Unite.Data.Context.Configuration.Extensions;
using Unite.Data.Context.Configuration.Options;
using Unite.Data.Context.Services.Tasks;
using Unite.Cache.Configuration.Options;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Web.Configuration.Options;
using Unite.Donors.Feed.Web.Handlers;
using Unite.Donors.Feed.Web.Workers;
using Unite.Donors.Feed.Web.Models;
using Unite.Donors.Feed.Web.Services;
using Unite.Donors.Indices.Services;
using Unite.Indices.Context.Configuration.Extensions;
using Unite.Indices.Context.Configuration.Options;
using Unite.Donors.Feed.Web.Models.Donors;
using Unite.Donors.Feed.Web.Models.Donors.Validators;
using Unite.Donors.Feed.Web.Submissions;
using Unite.Donors.Feed.Web.Handlers.Submission;

namespace Unite.Donors.Feed.Web.Configuration.Extensions;

public static class ConfigurationExtensions
{
    public static void Configure(this IServiceCollection services)
    {
        var sqlOptions = new SqlOptions();

        services.AddOptions();
        services.AddDatabase();
        services.AddDatabaseFactory(sqlOptions);
        services.AddRepositories();
        services.AddIndexServices();
        services.AddValidation();

        services.AddTransient<DonorsWriter>();
        services.AddTransient<DonorsRemover>();
        services.AddTransient<TreatmentsWriter>();

        services.AddTransient<DonorsSubmissionService>();
        services.AddTransient<TasksProcessingService>();

        services.AddTransient<SubmissionTaskService>();
        services.AddTransient<DonorIndexingTasksService>();

        // Indexing hosted service
        services.AddHostedService<DonorsIndexingWorker>();
        services.AddTransient<DonorsIndexingOptions>();
        services.AddTransient<DonorsIndexingHandler>();
        services.AddTransient<DonorIndexCreator>();
        services.AddTransient<DonorIndexRemover>();

        // Submissions hosted services
        services.AddHostedService<SubmissionsWorker>();
        services.AddTransient<DonorsSubmissionHandler>();
        services.AddTransient<TreatmentsSubmissionHandler>();
    }


    private static IServiceCollection AddOptions(this IServiceCollection services)
    {
        services.AddTransient<ApiOptions>();
        services.AddTransient<ISqlOptions, SqlOptions>();
        services.AddTransient<IMongoOptions, MongoOptions>();
        services.AddTransient<IElasticOptions, ElasticOptions>();

        return services;
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddTransient<IValidator<DonorModel[]>, DonorDataModelsValidator>();
        services.AddTransient<IValidator<TreatmentsModel[]>, TreatmentsDataModelsValidator>();

        return services;
    }
}
