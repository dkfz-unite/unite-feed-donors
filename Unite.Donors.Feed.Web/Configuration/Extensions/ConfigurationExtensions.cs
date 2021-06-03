using System.Collections.Generic;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Unite.Data.Services;
using Unite.Data.Services.Configuration.Options;
using Unite.Donors.DataFeed.Web.Configuration.Options;
using Unite.Donors.DataFeed.Web.HostedServices;
using Unite.Donors.DataFeed.Web.Models.Donors;
using Unite.Donors.DataFeed.Web.Models.Donors.Validators;
using Unite.Donors.DataFeed.Web.Models.Validation;
using Unite.Donors.Feed.Data.Donors;
using Unite.Donors.Feed.Web.Handlers;
using Unite.Donors.Feed.Web.Services;
using Unite.Donors.Indices.Services;
using Unite.Indices.Entities.Donors;
using Unite.Indices.Services;
using Unite.Indices.Services.Configuration.Options;

namespace Unite.Donors.DataFeed.Web.Configuration.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void Configure(this IServiceCollection services)
        {
            AddOptions(services);
            AddDatabases(services);
            AddValidation(services);
            AddServices(services);
            AddHostedServices(services);
        }

        private static void AddOptions(IServiceCollection services)
        {
            services.AddTransient<ISqlOptions, SqlOptions>();
            services.AddTransient<IElasticOptions, ElasticOptions>();
            services.AddTransient<IndexingOptions>();
        }

        private static void AddDatabases(IServiceCollection services)
        {
            services.AddTransient<UniteDbContext>();
        }

        private static void AddValidation(IServiceCollection services)
        {
            services.AddTransient<IValidator<IEnumerable<DonorModel>>, DonorModelsValidator>();

            services.AddTransient<IValidationService, ValidationService>();
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddTransient<DonorDataWriter>();
            
            services.AddTransient<TaskProcessingService>();
            services.AddTransient<IndexingTaskService>();
            services.AddTransient<IIndexCreationService<DonorIndex>, DonorIndexCreationService>();
            services.AddTransient<IIndexingService<DonorIndex>, DonorIndexingService>();
        }

        private static void AddHostedServices(IServiceCollection services)
        {
            services.AddHostedService<IndexingHostedService>();

            services.AddTransient<IndexingHandler>();
        }
    }
}
