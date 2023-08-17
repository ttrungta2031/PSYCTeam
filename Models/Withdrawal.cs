using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Withdrawal
    {
        public int Id { get; set; }
        public DateTime? DateCreate { get; set; }
        public string FullName { get; set; }
        public string BankName { get; set; }
        public string BankNumber { get; set; }
        public int? RequestAmount { get; set; }
        public int? ActualWithdrawal { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int? WalletId { get; set; }

        public virtual Wallet Wallet { get; set; }
    }
}
