using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class DeviceToken
    {
        public int Id { get; set; }
        public DateTime? Datechange { get; set; }
        public string FcmToken { get; set; }
        public int? CustomerId { get; set; }
        public int? ConsultantId { get; set; }
    }
}
