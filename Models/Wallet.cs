using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Wallet
    {
        public Wallet()
        {
            Deposits = new HashSet<Deposit>();
            Transactions = new HashSet<Transaction>();
            Withdrawals = new HashSet<Withdrawal>();
        }

        public int Id { get; set; }
        public int? Crab { get; set; }
        public DateTime? HistoryTrans { get; set; }
        public string PassWord { get; set; }
        public int? ConsultantId { get; set; }
        public int? CustomerId { get; set; }
        public string IsAdmin { get; set; }

        public virtual Consultant Consultant { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<Deposit> Deposits { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Withdrawal> Withdrawals { get; set; }
    }
}
