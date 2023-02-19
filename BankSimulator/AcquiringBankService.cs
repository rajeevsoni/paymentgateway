using BankSimulator.Models;

namespace BankSimulator
{
    public class AcquiringBankService : IBankService
    {
        private static List<string> notAllowedCards= new List<string>() { "1234-5678-1234-5678", "1111-2222-3333-4444" };

        public async Task<TransactionResponse> ProcessTransaction(TransactionRequest transactionRequest)
        {
            // fake response with some delay
            await Task.Delay(10);
            TransactionResponse transactionResponse = new TransactionResponse();

            if(notAllowedCards.Contains(transactionRequest.CardNumber))
            {
                transactionResponse.TransactionStatus = TransactionStatus.failed;
            }
            else
            {
                transactionResponse.TransactionStatus = TransactionStatus.success;
            }
            return transactionResponse;
        }
    }
}