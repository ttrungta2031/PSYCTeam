using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Payment
    {
        public int Id { get; set; }
        public double? Among { get; set; }
        public string Status { get; set; }
        public int? CustomerId { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}
