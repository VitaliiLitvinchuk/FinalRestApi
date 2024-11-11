using Api.ConsoleExecutor;
using Api.Injections;
using Api.Modules;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
    options.Filters.Add<ValidationExceptionFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

builder.Services.UseInjections(builder.Configuration);
builder.Services.SetupServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await ConsoleExecutor.RunViaConsoleProcess("docker-compose", "up -d");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost");

await app.InitializeDb();

app.MapControllers();

app.UseHttpsRedirection();

await app.RunAsync();

public partial class Program;