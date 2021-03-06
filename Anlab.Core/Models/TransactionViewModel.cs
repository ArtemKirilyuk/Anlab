using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Anlab.Jobs.MoneyMovement
{
    public class TransactionViewModel
    {
        public TransactionViewModel()
        {
            TransactionDate = DateTime.UtcNow; //DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            Transfers = new List<TransferViewModel>(2);
        }
        public string MerchantTrackingNumber { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Source { get; set; } = "ANLAB";
        public string SourceType { get; set; } = "Recharge";

        public IList<TransferViewModel> Transfers { get; set; }
    }

    public class TransferViewModel
    {
        public Decimal Amount { get; set; }
        public string Chart { get; set; }
        public string Account { get; set; }
        public string SubAccount { get; set; }
        public string ObjectCode { get; set; }
        
        public string Description { get; set; }
        
        public string Direction { get; set; }// Debit or Credit Code associated with the transaction. = ['Credit', 'Debit'],
    }
}
