using BankSimulator.Models;
using System.Transactions;

namespace BankSimulator.Tests
{
    [TestClass]
    public class AcquiringBankSeriveTests
    {
        private IBankService _bankService;

        [TestInitialize]
        public void TestInitialize()
        {
            _bankService = new AcquiringBankService();
        }

        [TestMethod]
        public async Task TransactionStatus_Failed()
        {
            TransactionRequest transactionRequest = new TransactionRequest(Guid.NewGuid(), Guid.NewGuid(),"1111-2222-3333-4444",10,21,"User1",321,"USD",213);
            TransactionResponse  response =  await _bankService.ProcessTransaction(transactionRequest);
            Assert.IsTrue(response.TransactionStatus == Models.TransactionStatus.failed);
        }

        [TestMethod]
        public async Task TransactionStatus_Success()
        {
            TransactionRequest transactionRequest = new TransactionRequest(Guid.NewGuid(), Guid.NewGuid(), "1132-4322-3783-9564", 10, 21, "User1", 321, "USD", 213);
            TransactionResponse response = await _bankService.ProcessTransaction(transactionRequest);
            Assert.IsTrue(response.TransactionStatus == Models.TransactionStatus.success);
        }
    }
}