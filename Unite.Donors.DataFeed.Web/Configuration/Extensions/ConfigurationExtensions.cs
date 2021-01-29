using System.Collections.Generic;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Unite.Data.Services;
using Unite.Data.Services.Configuration.Options;
using Unite.Donors.DataFeed.Domain.Resources;
using Unite.Donors.DataFeed.Domain.Resources.Validation;
using Unite.Donors.DataFeed.Domain.Validation;
using Unite.Donors.DataFeed.Web.Configuration.Options;
using Unite.Donors.DataFeed.Web.HostedServices;
using Unite.Donors.DataFeed.Web.Services;
using Unite.Donors.DataFeed.Web.Services.Indices;
using Unite.Indices.Entities.Donors;
using Unite.Indices.Services;
using Unite.Indices.Services.Configuration.Options;

namespace Unite.Donors.DataFeed.Web.Configuration.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddValidation();

            services.AddTransient<UniteDbContext>();

            services.AddTransient<IDataFeedService, DataFeedService>();
            services.AddTransient<DonorIndexCreationService>();
            services.AddTransient<IIndexingService<DonorIndex>, DonorIndexingService>();
            services.AddTransient<ITaskProcessingService, TaskProcessingService>();

            services.AddHostedService<IndexingHostedService>();
        }

        private static void AddOptions(this IServiceCollection services)
        {
            services.AddTransient<IndexingOptions>();
            services.AddTransient<IMySqlOptions, MySqlOptions>();
            services.AddTransient<IElasticOptions, ElasticOptions>();
        }

        private static void AddValidation(this IServiceCollection services)
        {
            services.AddTransient<IValidator<IEnumerable<Donor>>, DonorsValidator>();

            services.AddTransient<IValidationService, ValidationService>();
        }
    }
}
