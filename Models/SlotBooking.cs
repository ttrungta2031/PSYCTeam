using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class SlotBooking
    {
        public int SlotId { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public int? Price { get; set; }
        public DateTime? DateSlot { get; set; }
        public string ReasonOfCustomer { get; set; }
        public string ReasonOfConsultant { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int? BookingId { get; set; }
        public int? ConsultantId { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual Consultant Consultant { get; set; }
        public virtual RoomVideoCall RoomVideoCall { get; set; }
    }
}
