using WSBInvestmentPredictor.Expenses.API.Cqrs;
using WSBInvestmentPredictor.Expenses.Application;
using WSBInvestmentPredictor.Expenses.Domain.Interfaces;
using WSBInvestmentPredictor.Expenses.Infrastructure.Repositories;
using WSBInvestmentPredictor.Expenses.Shared.Cqrs.Commands;
using WSBInvestmentPredictor.Technology.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:7236",
                "https://wsbinvestmentpredictor-frontend-g6gegxf5gdhnbpe8.polandcentral-01.azurewebsites.net",
                "https://wsbinvestmentpredictor.azurewebsites.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Rejestracja CQRS
builder.Services.AddExpensesApplication();

// Register repositories
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WSB Investment Predictor API V1");
        c.RoutePrefix = "swagger";
    });

    // Production specific middleware
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseErrorHandling();

app.MapCqrsEndpoints(typeof(AddTransactions).Assembly, "AllowFrontend");

app.Use(async (context, next) =>
{
    await next();

    Console.WriteLine("--- RESPONSE HEADERS ---");
    foreach (var h in context.Response.Headers)
        Console.WriteLine($"{h.Key}: {h.Value}");
});

app.Run();

public partial class Program
{ }