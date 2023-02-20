using BankSimulator;
using BankSimulator.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PaymentGateway.Data;
using PaymentGateway.Data.Repository;
using System;
using System.Threading.Tasks;

namespace PaymentsProcessor
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IBankService, AcquiringBankService>();
            builder.Services.AddTransient<IPaymentDetailsRepository, PaymentDetailsRepository>();
        }
    }

    public class PaymentRequestProcessor
    {
        private readonly IBankService _bankService;
        private readonly IPaymentDetailsRepository _paymentDetailsRepository;
        private readonly Guid dummyMerchantId = Guid.Parse("a633e636-5931-4375-ac7f-1ebdd4903165");

        PaymentRequestProcessor(IBankService bankService, IPaymentDetailsRepository paymentDetailsRepository)
        {
            _bankService = bankService;
            _paymentDetailsRepository = paymentDetailsRepository;
        }

        [FunctionName("PaymentRequestProcessor")]
        public async Task Run([QueueTrigger("PaymentRequestQueue", Connection = "PaymentRequestQueueURI")]PaymentRequestQueueItem paymentRequestQueueItem, ILogger log)
        {
            log.LogInformation($"PaymentRequestProcessor completed for paymentId : {paymentRequestQueueItem.PaymentId}");

            PaymentDetails paymentDetails = new PaymentDetails(paymentRequestQueueItem.PaymentId,
                dummyMerchantId,
                PaymentStatus.pending,
                paymentRequestQueueItem.PaymentRequest.Amount,
                paymentRequestQueueItem.PaymentRequest.CurrencyCode);

            await _paymentDetailsRepository.AddpaymentDetails(paymentDetails);
            log.LogInformation($"Payment details added with Pending status for paymentId : {paymentRequestQueueItem.PaymentId}");

            TransactionRequest transactionRequest = new TransactionRequest(paymentRequestQueueItem.PaymentId,
                dummyMerchantId,
                paymentRequestQueueItem.PaymentRequest.CardNumber,
                paymentRequestQueueItem.PaymentRequest.ExpiryMonth,
                paymentRequestQueueItem.PaymentRequest.ExpiryYear,
                paymentRequestQueueItem.PaymentRequest.Name,
                paymentRequestQueueItem.PaymentRequest.Amount,
                paymentRequestQueueItem.PaymentRequest.CurrencyCode,
                paymentRequestQueueItem.PaymentRequest.Cvv);

            log.LogInformation($"Bank request for transaction ID {transactionRequest.TransactionId} initiated");
            TransactionResponse transactionResponse = await _bankService.ProcessTransaction(transactionRequest);
            log.LogInformation($"Bank request for transaction ID {transactionRequest.TransactionId} Completed");

            paymentDetails.UpdatePaymentStatus(GetPaymentStatus(transactionResponse.TransactionStatus));
            await _paymentDetailsRepository.UpdatepaymentDetails(paymentDetails);
            log.LogInformation($"Payment status set to {paymentDetails.PaymentStatus} for payment ID {paymentDetails.PaymentId}");

            log.LogInformation($"Processing completed for payment ID {paymentDetails.PaymentId}");
        }

        private PaymentStatus GetPaymentStatus(TransactionStatus transactionStatus)
        {
            switch (transactionStatus)
            {
                case TransactionStatus.pending: return PaymentStatus.pending;
                case TransactionStatus.success: return PaymentStatus.success;
                default: return PaymentStatus.failed;
            }
        }
    }
}
