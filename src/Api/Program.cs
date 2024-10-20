using Api.ConsoleExecutor;
using Api.Injections;
using Api.Modules;

await ConsoleExecutor.RunViaConsoleProcess("docker-compose", "up -d");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddCors();

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

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