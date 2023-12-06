using FluentValidation;
using Unite.Donors.Feed.Data.Donors;
using Unite.Donors.Feed.Web.Configuration.Options;
using Unite.Donors.Feed.Web.Handlers;
using Unite.Donors.Feed.Web.HostedServices;
using Unite.Donors.Indices.Services;
using Unite.Donors.Feed.Web.Services;
using Unite.Donors.Feed.Web.Models.Donors;
using Unite.Donors.Feed.Web.Models.Donors.Validators;
using Unite.Data.Context.Configuration.Options;
using Unite.Indices.Context.Configuration.Options;
using Unite.Data.Context.Configuration.Extensions;
using Unite.Data.Context.Services.Tasks;
using Unite.Indices.Context.Configuration.Extensions;
using Unite.Indices.Context;
using Unite.Indices.Entities.Donors;

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

        services.AddTransient<DonorDataWriter>();

        services.AddTransient<DonorIndexingTasksService>();
        services.AddTransient<TasksProcessingService>();

        services.AddHostedService<DonorsIndexingHostedService>();
        services.AddTransient<DonorsIndexingOptions>();
        services.AddTransient<DonorsIndexingHandler>();
        services.AddTransient<DonorIndexCreationService>();
        services.AddTransient<IIndexService<DonorIndex>, DonorsIndexService>();
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
        services.AddTransient<IValidator<DonorModel[]>, DonorModelsValidator>();

        return services;
    }
}
