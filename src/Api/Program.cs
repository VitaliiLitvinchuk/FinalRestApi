using Api.ConsoleExecutor;
using Api.Injections;
using Api.Modules;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

await ConsoleExecutor.RunViaConsoleProcess("docker-compose", "up -d");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
});

builder.Services.AddCors();

builder.Services.UseInjections(builder.Configuration);
builder.Services.SetupServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

await app.InitializeDb();

app.MapControllers();

app.UseHttpsRedirection();

await app.RunAsync();

public partial class Program;