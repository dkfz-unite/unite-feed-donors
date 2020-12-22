using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Tasks;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories.Tasks
{
    public class MutationIndexingTaskRepository : Repository
	{
		public MutationIndexingTaskRepository(UniteDbContext database, ILogger logger) : base(database, logger)
		{
		}

		public void AddRange(params int[] mutations)
		{
			if (mutations == null || mutations.Length < 1)
			{
				return;
			}

			var entities = mutations.Select(mutationId =>
			{
				var entity = new MutationIndexingTask();
				entity.MutationId = mutationId;
				entity.Date = DateTime.UtcNow;

				return entity;
			});

			_database.MutationIndexingTasks.AddRange(entities);
			_database.SaveChanges();
		}
	}
}
