using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Tasks;
using Unite.Data.Services;

namespace Unite.Donors.DataFeed.Web.Controllers
{
    [Route("api/[controller]")]
    public class IndicesController : Controller
    {
        private readonly UniteDbContext _database;
        private readonly ILogger _logger;

        public IndicesController(UniteDbContext database, ILogger<IndicesController> logger)
        {
            _database = database;
            _logger = logger;
        }

        [HttpPost]
        public ActionResult Post()
        {
            var donors = _database.Donors
                .Select(donor => donor.Id)
                .ToArray();

            var tasks = donors
                .Select(CreateTask)
                .ToArray();

            _logger.LogInformation($"Adding indexing tasks for {donors.Length} donors");

            _database.DonorIndexingTasks.AddRange(tasks);
            _database.SaveChanges();

            return Ok();
        }

        private DonorIndexingTask CreateTask(string donorId)
        {
            var task = new DonorIndexingTask()
            {
                DonorId = donorId,
                Date = DateTime.UtcNow
            };

            return task;
        }
    }
}
