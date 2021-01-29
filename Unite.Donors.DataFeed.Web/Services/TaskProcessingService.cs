using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Unite.Data.Services;
using Unite.Donors.DataFeed.Web.Services.Indices;
using Unite.Indices.Entities.Donors;
using Unite.Indices.Services;

namespace Unite.Donors.DataFeed.Web.Services
{
    public class TaskProcessingService : ITaskProcessingService
    {
        private readonly UniteDbContext _database;
        private readonly DonorIndexCreationService _indexCreationService;
        private readonly IIndexingService<DonorIndex> _indexingService;
        private readonly ILogger _logger;

        public TaskProcessingService(
            UniteDbContext database,
            DonorIndexCreationService indexCreationService,
            IIndexingService<DonorIndex> indexingService,
            ILogger<TaskProcessingService> logger)
        {
            _database = database;
            _indexCreationService = indexCreationService;
            _indexingService = indexingService;
            _logger = logger;
        }

        public void ProcessIndexingTasks(int bucketSize)
        {
			while (_database.DonorIndexingTasks.Any())
			{
				var tasks = _database.DonorIndexingTasks
                    .OrderBy(task => task.Date)
                    .Take(bucketSize)
                    .ToArray();

				var indices = new List<DonorIndex>();

				foreach (var task in tasks)
				{
                    try
                    {
                        var index = _indexCreationService.CreateIndex(task.DonorId);

                        if (index != null)
                        {
                            indices.Add(index);
                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, $"Could not create index fro donor '{task.DonorId}'");
                    }
                    
				}

                _logger.LogInformation($"Starting to index {indices.Count()} donors");

                _indexingService.IndexMany(indices);

                _database.DonorIndexingTasks.RemoveRange(tasks);

                _database.SaveChanges();

                _logger.LogInformation($"Indexing of {indices.Count()} donors completed");
			}
		}
    }
}
