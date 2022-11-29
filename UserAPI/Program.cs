using HealthChecks.System;
using HealthChecks.UI.Client;
using HealthChecks.UI.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using UserAPI.Authorization;
using UserAPI.Data;
using UserAPI.Filter;
using UserAPI.Helpers;
using UserAPI.Models;
using UserAPI.Services;
using UserAPI.Services.Interfaces;
using UserAPI.Utility;

var builder = WebApplication.CreateBuilder(args);

// configure DI for application services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<ValidateEntityExistsAttribute<User>>();

builder.Services.AddDbContext<DbContextClass>();
builder.Services.AddHealthChecks()
    .AddSqlServer(StaticConfigurationManager.AppSetting["ConnectionStrings:DefaultConnection"])
    .AddDiskStorageHealthCheck(delegate (DiskStorageOptions diskStorageOptions)
    {
        diskStorageOptions.AddDrive(@"C:\", minimumFreeMegabytes: 5000);
    }, name: "C Drive", HealthStatus.Degraded);

builder.Services.AddHealthChecksUI().AddInMemoryStorage();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// configure strongly typed settings object
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    options.SuppressModelStateInvalidFilter = true;
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();

app.UseHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.UseHealthChecksUI(delegate (Options options)
{
    options.UIPath = "/hc-ui";
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
