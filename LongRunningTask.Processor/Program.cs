using MassTransit;
using LongRunningTask.Infraestructure.Queues;
using LongRunningTask.Domain;
using LongRunningTask.Domain.Interfaces;
using LongRunningTask.Domain.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Bind ThreadDelayConfiguration from appsettings
builder.Services.Configure<ThreadDelayConfiguration>(
    builder.Configuration.GetSection("ThreadDelayConfiguration"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPublisher, Publisher>();

builder.Services.AddScoped<StringProcessor>();
builder.Services.AddSingleton<IDelayProvider, ThreadDelayProvider>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<RequestJobConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("queues", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();