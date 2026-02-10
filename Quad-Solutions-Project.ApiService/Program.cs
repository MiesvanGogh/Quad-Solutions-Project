using Microsoft.OpenApi;
using Quad_Solutions_Project.ApiClient;
using Quad_Solutions_Project.BusnLogic.Services;
using Quad_Solutions_Project.Models;
using Quad_Solutions_Project.Models.Interfaces;

const string OpenTdbBaseUrl = "https://opentdb.com/";

var builder = WebApplication.CreateBuilder(args);

// ─── Dependency Injection ───────────────────────────────────────────

// Typed HttpClient for the Open Trivia Database
builder.Services.AddHttpClient<IOpenTdbClient, OpenTdbClient>(client =>
{
    client.BaseAddress = new Uri(OpenTdbBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
});

// HttpClient for your own server API (Blazor frontend calls backend)
builder.Services.AddHttpClient("ServerAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7326");
});

// AnswerStore is a singleton
builder.Services.AddSingleton<AnswerService>();

// TriviaService is transient
builder.Services.AddTransient<ITriviaService, TriviaService>();

// Controllers + API Explorer
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger / OpenAPI documentation
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TriviaApp Intermediate API",
        Version = "v1",
        Description =
            "A proxy API that sits between the frontend and the Open Trivia Database. " +
            "Correct answers are stored server-side and never exposed until the user commits to a choice."
    });
});

// 🔹 Aspire health checks (REQUIRED)
builder.Services.AddHealthChecks();

var app = builder.Build();

// ─── Middleware ─────────────────────────────────────────────────────

// Development-only: Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors(policy =>
    policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

// Map controllers
app.MapControllers();

// 🔹 Health endpoint for Aspire
app.MapHealthChecks("/health");

await app.RunAsync();
