using PermutationGeneratorAPI.Services;
using PermutationGeneratorAPI.Services.Interfaces;
using PermutationGeneratorAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PermutationSettings>(
    builder.Configuration.GetSection("PermutationSettings"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Permutation Generator API", 
        Version = "v1",
        Description = "API למחולל קומבינציות - מחשב את כל הקומבינציות האפשריות עבור n מספרים"
    });
});

builder.Services.AddSingleton<IPermutationAlgorithmService, PermutationAlgorithmService>();

builder.Services.AddSingleton<ISessionManagerService, SessionManagerService>();

builder.Services.AddSingleton<INextPermutationService, NextPermutationService>();

builder.Services.AddScoped<IPermutationBusinessLogicService, PermutationBusinessLogicService>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
    ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularClient");

app.MapControllers();

app.Run();
