using HackerNewsApi.Services;
using Microsoft.OpenApi.Models;

//DI Container setup
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add HTTP client
builder.Services.AddHttpClient<IHackerNewsService, HackerNewsService>();

// Add memory caching
builder.Services.AddMemoryCache();

// Register services - *adds dependency injection to services*
// Meaning - when IHackerNewsService is requested, provides instance of HackerNewsService
// add scope - new instance per request
builder.Services.AddScoped<IHackerNewsService, HackerNewsService>();

// Source for Cors setup:
// https://stackoverflow.com/questions/31942037/how-to-enable-cors-in-asp-net-core

// Add CORS - *add Cors policy to allow Angular app access*
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Hacker News API", 
        Version = "v1",
        Description = "API for fetching newest stories from Hacker News"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS - *add to middleware
app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();

// public program class - f or testing
public partial class Program { }
