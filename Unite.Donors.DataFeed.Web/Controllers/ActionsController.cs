using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ActionsController : Controller
    {
        private readonly UniteDbContext _database;
        private readonly ILogger _logger;

        public ActionsController(UniteDbContext database, ILogger<ActionsController> logger)
        {
            _database = database;
            _logger = logger;
        }

        [HttpPost]
        public ActionResult Index()
        {
            try
            {
                CreateIndexingTasks();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
            }

            return Ok();
        }

        private void CreateIndexingTasks()
        {
            var donors = _database.Donors
                .Select(donor => donor.Id)
                .ToArray();

            var tasks = donors
                .Select(CreateTask)
                .ToArray();


            _database.Tasks.AddRange(tasks);
            _database.SaveChanges();
        }

        private Task CreateTask(string donorId)
        {
            var task = new Task()
            {
                TypeId = TaskType.Indexing,
                TargetTypeId = TaskTargetType.Donor,
                Target = donorId,
                Data = null,
                Date = DateTime.UtcNow
            };

            return task;
        }
    }
}
