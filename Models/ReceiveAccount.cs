using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class ReceiveAccount
    {
        public ReceiveAccount()
        {
            Deposits = new HashSet<Deposit>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string QrCode { get; set; }
        public string PhoneNumber { get; set; }
        public string BankName { get; set; }
        public string BankNumber { get; set; }
        public DateTime? DateCreate { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Deposit> Deposits { get; set; }
    }
}
