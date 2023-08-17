using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Notification
    {
        public int Id { get; set; }
        public DateTime? DateCreate { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int? ConsultantId { get; set; }
        public string Status { get; set; }
        public int? CustomerId { get; set; }
        public bool? IsAdmin { get; set; }

        public virtual Consultant Consultant { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
