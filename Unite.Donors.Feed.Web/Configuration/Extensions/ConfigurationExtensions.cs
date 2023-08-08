using FluentValidation;
using Unite.Data.Services;
using Unite.Data.Services.Configuration.Options;
using Unite.Data.Services.Tasks;
using Unite.Donors.Feed.Data.Donors;
using Unite.Donors.Feed.Web.Configuration.Options;
using Unite.Donors.Feed.Web.Handlers;
using Unite.Donors.Feed.Web.HostedServices;
using Unite.Donors.Feed.Web.Services.Donors;
using Unite.Donors.Feed.Web.Services.Donors.Validators;
using Unite.Donors.Indices.Services;
using Unite.Indices.Entities.Donors;
using Unite.Indices.Services;
using Unite.Indices.Services.Configuration.Options;
using Unite.Donors.Feed.Web.Services;

namespace Unite.Donors.Feed.Web.Configuration.Extensions;

public static class ConfigurationExtensions
{
    public static void Configure(this IServiceCollection services)
    {
        services.AddTransient<ApiOptions>();
        services.AddTransient<ISqlOptions, SqlOptions>();
        services.AddTransient<IElasticOptions, ElasticOptions>();

        services.AddTransient<IValidator<DonorModel[]>, DonorModelsValidator>();

        services.AddTransient<DomainDbContext>();
        services.AddTransient<DonorDataWriter>();

        services.AddTransient<DonorIndexingTasksService>();
        services.AddTransient<TasksProcessingService>();

        services.AddHostedService<DonorsIndexingHostedService>();
        services.AddTransient<DonorsIndexingOptions>();
        services.AddTransient<DonorsIndexingHandler>();
        services.AddTransient<IIndexCreationService<DonorIndex>, DonorIndexCreationService>();
        services.AddTransient<IIndexingService<DonorIndex>, DonorsIndexingService>();

    }
}
