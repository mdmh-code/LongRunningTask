using MassTransit;
using LongRunningTask.Infraestructure.Queues;
using LongRunningTask.Domain;
using LongRunningTask.Domain.Interfaces;
using LongRunningTask.Domain.Configuration;
using LongRunningTask.Infraestructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Bind ThreadDelayConfiguration from appsettings
builder.Services.Configure<ThreadDelayConfiguration>(
    builder.Configuration.GetSection("ThreadDelayConfiguration"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterDomain();
builder.Services.RegisterQueuePublisher();
builder.Services.RegisterQueueConsumer<RequestJobConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();