using MassTransit;
using LongRunningTask.Infraestructure.Queues;
using LongRunningTask.Domain;
using LongRunningTask.Domain.Interfaces;
using LongRunningTask.Domain.Configuration;
using LongRunningTask.Infraestructure.Configuration;
using LongRunningTask.Infraestructure.Repositories.Entites;
using Microsoft.EntityFrameworkCore;
using LongRunningTask.Infraestructure.Repositories;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

var builder = WebApplication.CreateBuilder(args);

// Bind ThreadDelayConfiguration from appsettings
builder.Services.Configure<ThreadDelayConfiguration>(
    builder.Configuration.GetSection("ThreadDelayConfiguration"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
        .WithOrigins(
            "http://localhost:5173"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
    );
});

builder.Services.AddDbContext<JobDbContext>(options =>
     options.UseNpgsql(
        builder.Configuration.GetConnectionString("JobsDb")));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterDomain();
builder.Services.RegisterQueuePublisher();
builder.Services.RegisterQueueConsumer(typeof(RequestJobConsumer), typeof(CancelJobConsumer));
builder.Services.AddScoped<IJobRepository, JobRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.MapGet("/process/running", async (IJobRepository jobRepository, ILogger<Program> logger) =>
    {
        var job = await jobRepository.GetByStatus(Job.PENDING_STATUS, Job.PROCESSING_STATUS);
        logger.LogInformation("Found running job with ProcessId: {ProcessId}", job?.ProcessId);
        return Results.Ok(new { job?.ProcessId });
    }
);

app.Run();