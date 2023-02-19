using Azure.Identity;
using Azure.Storage.Queues;
using Microsoft.Extensions.Azure;
using PaymentGateway.Data.Repository;
using PaymentGatewayAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IPaymentDetailsRepository, PaymentDetailsRepository>();
builder.Services.AddTransient<IPaymentGatewayService, PaymentGatewayService>();
var queueURI = builder.Configuration["AZURE_STORAGE_QUEUE_URI"];
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
