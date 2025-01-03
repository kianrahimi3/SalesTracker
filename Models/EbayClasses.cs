using System.Security.Cryptography.X509Certificates;

namespace SalesTrackerMVC.Models
{
    public class EbayClasses
    {
    }

    public class Transactions
    {
        public string href { get; set; }
        public string next { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
        public List<Transaction> transactions { get; set; }
        public int total { get; set; }
    }

    public class Transaction {
        public string transactionId { get; set; }
        public string orderId { get; set; }
        public string salesRecordReference { get; set; }
        public Buyer buyer { get; set; }
        public string transactionType { get; set; }
        public Amount amount { get; set; }
        public string bookingEntry { get; set; }
        public string transactionDate { get; set; }
        public string transactionStatus { get; set; }
        public string transactionMemo { get; set; }
        public string paymentsEntity { get; set; }
    }

    public class Buyer
    {
        public string username { get; set; }
    }

    public class Amount
    {
        public string value { get; set; }
        public string currency { get; set; }
    }
}
