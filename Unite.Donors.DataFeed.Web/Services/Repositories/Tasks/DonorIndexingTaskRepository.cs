using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Tasks;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories.Tasks
{
    public class DonorIndexingTaskRepository : Repository
	{
		public DonorIndexingTaskRepository(UniteDbContext database, ILogger logger) : base(database, logger)
		{
		}

		public void AddRange(params string[] donors)
		{
			if (donors == null || donors.Length < 1)
			{
				return;
			}

			var entities = donors.Select(donorId =>
			{
				var entity = new DonorIndexingTask();
				entity.DonorId = donorId;
				entity.Date = DateTime.UtcNow;

				return entity;
			});

			_database.DonorIndexingTasks.AddRange(entities);
			_database.SaveChanges();
		}
	}
}
