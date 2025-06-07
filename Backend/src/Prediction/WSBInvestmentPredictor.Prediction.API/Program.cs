using FluentValidation;
using MediatR;
using WSBInvestmentPredictor.Prediction.Application;
using WSBInvestmentPredictor.Prediction.Application.Common.Behaviors;
using WSBInvestmentPredictor.Prediction.Application.FeatureEngeneering;
using WSBInvestmentPredictor.Prediction.Application.Validators;
using WSBInvestmentPredictor.Prediction.Domain.Interfaces;
using WSBInvestmentPredictor.Prediction.Infrastructure.MarketData;
using WSBInvestmentPredictor.Prediction.Infrastructure.Prediction;
using WSBInvestmentPredictor.Prediction.MarketData;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("https://localhost:7236")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Rejestracja CQRS
builder.Services.AddPredictionApplication();

// Rejestracja FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(TrainModelCommandValidator).Assembly);

// Rejestracja globalnego pipeline do walidacji
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddScoped<IStockPredictorService, StockPredictorService>();
builder.Services.AddSingleton<ISp500TickerProvider, Sp500CsvTickerProvider>();
builder.Services.AddSingleton<MarketDataBuilder>();
builder.Services.AddHttpClient<IPolygonClient, PolygonClient>();

var app = builder.Build();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();