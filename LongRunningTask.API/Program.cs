using LongRunningTask.Infraestructure.Configuration;
using LongRunningTask.Domain.Configuration;
using LongRunningTask.Domain;
using LongRunningTask.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Bind ThreadDelayConfiguration from appsettings
builder.Services.Configure<ThreadDelayConfiguration>(
    builder.Configuration.GetSection("ThreadDelayConfiguration"));

// Allow CORS for localhost (React/Vite dev server)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy => policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:8080",
                "ws://localhost:5173"
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
builder.Services.RegisterInfraestructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseCors("AllowLocalhost");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var messageGroup = app.MapGroup("api/message");

messageGroup.MapPost("/", (StringProcessor processor, CharacterEmmiter emmiter, StringProcessRequest request) =>
{
    var processedMessage = processor.Process(request.Message);
    emmiter.EmitCharacters(processedMessage);
    return Results.Accepted();
}
);

messageGroup.MapHub<SignalRCharacterHub>("/responsehub");

app.Run();
