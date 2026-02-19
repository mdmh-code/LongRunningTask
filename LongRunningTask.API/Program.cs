using MassTransit;
using LongRunningTask.Infraestructure.Configuration;
using LongRunningTask.Infraestructure.Queues;
using LongRunningTask.Domain.Configuration;
using LongRunningTask.Domain;
using LongRunningTask.API.Models;
using LongRunningTask.Infraestructure;
using LongRunningTask.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Allow CORS for localhost (React/Vite dev server)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy => policy
            .WithOrigins(
                "http://web:5173",
                "http://api:8080",
                "ws://web:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterDomain();
builder.Services.RegisterQueuePublisher();
builder.Services.RegisterQueueConsumer(typeof(ResponseJobConsumer));
builder.Services.RegisterAsyncMessaging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseCors("AllowLocalhost");

app.UseAuthorization();

app.MapControllers();

var messageGroup = app.MapGroup("api/message");

messageGroup.MapPost("/", async (IPublisher publisher, StringProcessRequest request) =>
    {
        var processId = Guid.NewGuid();
        await publisher.Publish(new RequestJob("123", processId, request.Message));
        return Results.Accepted(null, new { processId = processId.ToString() });
    }
);

messageGroup.MapPost("/cancel", async (IPublisher publisher, StringCancelRequest request) =>
    {
        await publisher.Publish(new CancelJob(request.processId));
        return Results.Accepted(null, new { processId = request.processId.ToString() });
    }
);

messageGroup.MapHub<SignalRCharacterHub>("/responsehub");

app.Run();
