using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unite.Data.Entities.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;

namespace Unite.Donors.Feed.Web.Services
{
    public abstract class TaskService
    {
        protected readonly DomainDbContext _dbContext;

        protected abstract int BucketSize { get; }


        protected TaskService(DomainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        protected void CreateTasks<T>(
            TaskType type,
            TaskTargetType targetType,
            IEnumerable<T> keys)
        {
            var tasks = keys
                .Select(key => new Task
                {
                    TypeId = type,
                    TargetTypeId = targetType,
                    Target = key.ToString(),
                    Date = DateTime.UtcNow
                })
                .ToArray();

            _dbContext.AddRange(tasks);
            _dbContext.SaveChanges();
        }


        protected void IterateEntities<T, TKey>(
            Expression<Func<T, bool>> condition,
            Expression<Func<T, TKey>> selector,
            Action<IEnumerable<TKey>> handler)
            where T : class
        {
            var position = 0;

            var entities = Enumerable.Empty<TKey>();

            do
            {
                entities = _dbContext.Set<T>()
                    .Where(condition)
                    .Skip(position)
                    .Take(BucketSize)
                    .Select(selector)
                    .ToArray();

                handler.Invoke(entities);

                position += entities.Count();

            }
            while (entities.Count() == BucketSize);
        }
    }
}
