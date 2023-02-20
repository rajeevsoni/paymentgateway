using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Newtonsoft.Json;
using PaymentGateway.Data;
using PaymentGateway.Data.Models;
using PaymentGateway.Data.Repository;
using PaymentGatewayAPI.Services;
using System;
using System.Net;
using System.Text;

namespace PaymentGatewayAPI.IntegrationTests
{
    [TestClass]
    public class PaymentsControllerTest
    {
        private static TestContext _testContext;
        private static WebApplicationFactory<Program> _factory;

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            _testContext = testContext;
            _factory = new WebApplicationFactory<Program>();
            _factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("https_port", "5001").UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<PaymentRequestValidator>());
                    services.AddTransient<IPaymentGatewayService, MockPaymentGatewayservice>();
                    services.AddTransient<IPaymentDetailsRepository, PaymentDetailsRepository>();
                    services.AddMemoryCache();
                    services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
                    services.AddInMemoryRateLimiting();
                    services.Configure<IpRateLimitOptions>(options =>
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
                });
            });

        }

        [TestMethod]
        public async Task GetPaymentDetails_ForOk()
        {
            Console.WriteLine(_testContext.TestName);

            var paymentId = Guid.NewGuid().ToString();
            var client = _factory.CreateClient();
            var response = await client.GetAsync($"payments/{paymentId}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PaymentDetails>(jsonString);
            Assert.IsNotNull(result);

            await Task.Delay(10000);
        }

        [TestMethod]
        public async Task GetPaymentDetails_ForNotFound()
        {
            Console.WriteLine(_testContext.TestName);

            var paymentId = "00000000-0000-0000-0000-000000000000";
            var client = _factory.CreateClient();
            var response = await client.GetAsync($"payments/{paymentId}");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            await Task.Delay(10000);
        }

        [TestMethod]
        public async Task GetPaymentDetails_ForRateLimit_Reached()
        {
            Console.WriteLine(_testContext.TestName);

            var paymentId = "00000000-0000-0000-0000-000000000000";
            var client = _factory.CreateClient();
            var response1 = await client.GetAsync($"payments/{paymentId}");
            var response2 = await client.GetAsync($"payments/{paymentId}");
            var response3 = await client.GetAsync($"payments/{paymentId}");
            var response4 = await client.GetAsync($"payments/{paymentId}");

            Assert.AreEqual(response4.StatusCode, HttpStatusCode.TooManyRequests);

            await Task.Delay(10000);
        }

        [TestMethod]
        public async Task PostPayment_ForAccepted()
        {
            Console.WriteLine(_testContext.TestName);
            var client = _factory.CreateClient();

            PaymentRequest paymentRequest = new PaymentRequest();
            paymentRequest.Cvv = 123;
            paymentRequest.Amount = 144;
            paymentRequest.CardNumber = "4018-0611-4504-5864";
            paymentRequest.ExpiryMonth = 10;
            paymentRequest.ExpiryYear = 2025;
            paymentRequest.Name = "User1";
            paymentRequest.CurrencyCode = "USD";
            
            var json = JsonConvert.SerializeObject(paymentRequest);

            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await client.PostAsync("payments", stringContent);

            Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);
            await Task.Delay(10000);
        }

        [TestMethod]
        public async Task PostPayment_ForBadRequest()
        {
            Console.WriteLine(_testContext.TestName);
            var client = _factory.CreateClient();

            PaymentRequest paymentRequest = new PaymentRequest();
            paymentRequest.Cvv = 1;
            paymentRequest.Amount = 144;
            paymentRequest.CardNumber = "1111-1111-1111-1111";
            paymentRequest.ExpiryMonth = 10;
            paymentRequest.ExpiryYear = 2025;
            paymentRequest.Name = "User1";
            paymentRequest.CurrencyCode = "USD";

            var json = JsonConvert.SerializeObject(paymentRequest);

            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await client.PostAsync("payments", stringContent);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            await Task.Delay(10000);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _factory.Dispose();
        }
    }

    public class MockPaymentGatewayservice : IPaymentGatewayService
    {
        public async Task<PaymentDetails> GetPaymentDetails(Guid paymentId)
        {
            await Task.Delay(10);
            if (paymentId.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                return null;
            }
            else
            {
                return new PaymentDetails(Guid.NewGuid(), Guid.NewGuid(), PaymentStatus.success, 213, "USD");
            }
        }

        public async Task<PaymentResponse> SubmitPaymentRequest(PaymentRequest paymentRequest)
        {
            await Task.Delay(10);
            PaymentResponse paymentResponse = new PaymentResponse();
            paymentResponse.PaymentId = Guid.NewGuid().ToString();
            return paymentResponse;
        }
    }

}