using AspNetCoreRateLimit;
using Azure.Identity;
using Azure.Storage.Queues;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Azure;
using PaymentGateway.Data.Models;
using PaymentGateway.Data.Repository;
using PaymentGatewayAPI.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<PaymentRequestValidator>());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var queueURI = builder.Configuration["PaymentRequestQueueURI"];
builder.Services.AddAzureClients(builder =>
{
    builder.AddClient<QueueClient, QueueClientOptions>((options, _, _) =>
    {
        options.MessageEncoding = QueueMessageEncoding.Base64;
        var credential = new DefaultAzureCredential();
        var queueUri = new Uri(queueURI);
        return new QueueClient(queueUri, credential, options);
    });
});

builder.Services.AddTransient<IPaymentGatewayService, PaymentGatewayService>();

builder.Services.AddTransient<IPaymentDetailsRepository, PaymentDetailsRepository>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Real-IP";
    options.GeneralRules = new List<RateLimitRule>
        {
            new RateLimitRule
            {
                Endpoint = "*",
                Period = "10s",
                Limit = 3
            }
        };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpMetrics();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapMetrics();

app.UseIpRateLimiting();

app.Run();

public partial class Program { }
