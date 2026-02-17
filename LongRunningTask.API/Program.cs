using LongRunningTask.Infraestructure.Configuration;
using LongRunningTask.Domain.Configuration;
using LongRunningTask.Domain;
using LongRunningTask.API.Models;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var messageGroup = app.MapGroup("/api/message");
messageGroup.MapPost("/", (StringProcessor processor, StringProcessRequest request) => processor.Process(request.Input));
messageGroup.MapHub<SignalRCharacterHub>("/responsehub");

app.Run();
