using BankSimulator.Models;

namespace BankSimulator
{
    public interface IBankService
    {
        Task<TransactionResponse> ProcessTransaction(TransactionRequest transactionRequest);
    }
}
