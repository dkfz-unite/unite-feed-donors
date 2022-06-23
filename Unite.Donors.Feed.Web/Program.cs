using FluentValidation.AspNetCore;
using Unite.Donors.Feed.Web.Configuration.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Logging.AddConsole();

builder.Services.Configure();

builder.Services.AddControllers(options => options.AddMvcOptions())
                .AddJsonOptions(options => options.AddJsonOptions())
                .AddFluentValidation();


var app = builder.Build();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();