using AppointmentManagementService.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Load Configuration
var configuration = builder.Configuration;

// Add DbContext
builder.Services.AddDatabaseContext(configuration);

// Add services from the Dependency Injection Library
builder.Services.AddApplicationServices();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Automatically apply migrations on startup
    app.Services.InitializeDatabase();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
