using FluentValidation;
using Unite.Data.Context.Configuration.Extensions;
using Unite.Data.Context.Configuration.Options;
using Unite.Data.Context.Services.Tasks;
using Unite.Donors.Feed.Data;
using Unite.Donors.Feed.Web.Configuration.Options;
using Unite.Donors.Feed.Web.Handlers;
using Unite.Donors.Feed.Web.HostedServices;
using Unite.Donors.Feed.Web.Models;
using Unite.Donors.Feed.Web.Models.Validators;
using Unite.Donors.Feed.Web.Services;
using Unite.Donors.Indices.Services;
using Unite.Indices.Context.Configuration.Extensions;
using Unite.Indices.Context.Configuration.Options;

namespace Unite.Donors.Feed.Web.Configuration.Extensions;

public static class ConfigurationExtensions
{
    public static void Configure(this IServiceCollection services)
    {
        var sqlOptions = new SqlOptions();

        services.AddOptions();
        services.AddDatabase();
        services.AddDatabaseFactory(sqlOptions);
        services.AddIndexServices();
        services.AddValidation();

        services.AddTransient<DonorsDataWriter>();
        services.AddTransient<TreatmentsDataWriter>();

        services.AddTransient<DonorIndexingTasksService>();
        services.AddTransient<TasksProcessingService>();

        services.AddHostedService<DonorsIndexingHostedService>();
        services.AddTransient<DonorsIndexingOptions>();
        services.AddTransient<DonorsIndexingHandler>();
        services.AddTransient<DonorIndexCreationService>();
    }


    private static IServiceCollection AddOptions(this IServiceCollection services)
    {
        services.AddTransient<ApiOptions>();
        services.AddTransient<ISqlOptions, SqlOptions>();
        services.AddTransient<IElasticOptions, ElasticOptions>();

        return services;
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddTransient<IValidator<DonorDataModel[]>, DonorDataModelsValidator>();
        services.AddTransient<IValidator<TreatmentsDataModel[]>, TreatmentsDataModelsValidator>();
        services.AddTransient<IValidator<TreatmentDataFlatModel[]>, TreatmentDataFlatModelsValidator>();

        return services;
    }
}
