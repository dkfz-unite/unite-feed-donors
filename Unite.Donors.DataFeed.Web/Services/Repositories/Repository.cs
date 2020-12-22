using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Services.Repositories
{
    public abstract class Repository
    {
		protected readonly UniteDbContext _database;
		protected readonly ILogger _logger;


		public Repository(UniteDbContext database, ILogger logger)
		{
			_database = database;
			_logger = logger;
		}
	}

    public abstract class Repository<T> : Repository
		where T : class, new()
    {
        protected DbSet<T> Entities
        {
            get
            {
				return _database.Set<T>();
			}
        }


		protected Repository(UniteDbContext database, ILogger logger) : base(database, logger)
		{
		}


		public virtual T Find(Expression<Func<T, bool>> predicate)
        {
			var query = Entities.AsQueryable();

			query = Include(query);

			var entity = query.FirstOrDefault(predicate);

			return entity;
        }

		public virtual T Add(in T model)
        {
			var entity = new T();

			Map(model, ref entity);

			Entities.Add(entity);

			_database.SaveChanges();

			return entity;
        }

		public virtual void Update(ref T entity, in T model)
        {
			Map(model, ref entity);

			Entities.Update(entity);

			_database.SaveChanges();
        }


		protected virtual IQueryable<T> Include(IQueryable<T> query)
        {
			return query;
        }

		protected abstract void Map(in T source, ref T target);
    }
}
