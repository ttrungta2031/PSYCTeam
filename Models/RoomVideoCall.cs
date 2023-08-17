using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class RoomVideoCall
    {
        public int Id { get; set; }
        public string ChanelName { get; set; }
        public string Token { get; set; }
        public int? SlotId { get; set; }

        public virtual SlotBooking Slot { get; set; }
    }
}
