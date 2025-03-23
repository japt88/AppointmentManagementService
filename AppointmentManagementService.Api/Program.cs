using AppointmentManagementService.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});


// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "3.0.0", 
        Title = "Clinic API",
        Description = "API for generating appointments"
    });
});


// Load Configuration
var configuration = builder.Configuration;

// Add DbContext
builder.Services.AddDatabaseContext(configuration);

// Add services from the Dependency Injection Library
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseCors("AllowAll"); // Apply CORS policy


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Automatically apply migrations on startup
    app.Services.InitializeDatabase();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic API");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
