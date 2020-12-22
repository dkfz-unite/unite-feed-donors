using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Unite.Donors.DataFeed.Web.Configuration.Filters
{
    public class DefaultActionFilter: IActionFilter
    {
        private readonly ILogger _logger;

        public DefaultActionFilter(ILogger<DefaultActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var method = context.HttpContext.Request.Method;
            var urlBase = context.HttpContext.Request.PathBase;
            var url = context.HttpContext.Request.Path;

			AddStopwatchData(context.HttpContext);

			_logger.LogInformation("{0}:{1}{2}", method, urlBase.ToString(), url.ToString());
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
			var stopwatch = GetStopwatchData(context.HttpContext);

			if (stopwatch != null)
			{
				_logger.LogInformation($"Executed. Elapsed {stopwatch.ElapsedMilliseconds}ms");
			}
			else
			{
				_logger.LogInformation("Executed");
			}
		}

		private void AddStopwatchData(HttpContext context)
		{
			var stopwatch = new Stopwatch();

			stopwatch.Start();

			context.Items.Add("stopwatch", stopwatch);
		}

		private Stopwatch GetStopwatchData(HttpContext context)
		{
			try
			{
				var stopwatchItem = context.Items["stopwatch"];

				if (stopwatchItem != null)
				{
					var stopwatch = (Stopwatch)stopwatchItem;

					stopwatch.Stop();

					return stopwatch;
				}
				else
				{
					return null;
				}
			}
			catch
			{
				return null;
			}
		}
	}
}
