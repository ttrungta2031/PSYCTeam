using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Booking
    {
        public Booking()
        {
            SlotBookings = new HashSet<SlotBooking>();
        }

        public int Id { get; set; }
        public double? Price { get; set; }
        public int? Rate { get; set; }
        public string Feedback { get; set; }
        public string Duration { get; set; }
        public int? ConsultantId { get; set; }
        public DateTime? DateBooking { get; set; }
        public int PaymentId { get; set; }
        public int? CustomerId { get; set; }
        public string Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual ICollection<SlotBooking> SlotBookings { get; set; }
    }
}
