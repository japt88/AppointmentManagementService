using AppointmentManagementService.Api.Authentication;
using AppointmentManagementService.Api.Filters;
using AppointmentManagementService.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelFilter>();  
});

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

    // Define security scheme
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer {token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    // Apply security globally
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            securityScheme, new string[] { }
        }
    });
});


// Load Configuration
var configuration = builder.Configuration;

// Add DbContext
builder.Services.AddDatabaseContext(configuration);


// Register services
builder.Services.AddSingleton<TokenService>();

// Add services from the Dependency Injection Library
builder.Services.AddApplicationServices();

var app = builder.Build();

//app.UseCors("AllowAll"); // Apply CORS policy


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

app.UseAuthentication(); // Enable Authentication

app.UseAuthorization();

app.MapControllers();

app.Run();
