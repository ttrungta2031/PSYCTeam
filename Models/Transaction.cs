using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Transaction
    {
        public int Id { get; set; }
        public DateTime? DateCreate { get; set; }
        public int? WalletId { get; set; }
        public int PaymentId { get; set; }
        public string Description { get; set; }

        public virtual Payment Payment { get; set; }
        public virtual Wallet Wallet { get; set; }
    }
}
