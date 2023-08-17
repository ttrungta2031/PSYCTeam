using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Feedback
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int? Vote { get; set; }
        public string Status { get; set; }
        public int? CustomerId { get; set; }
        public int? ConsultantId { get; set; }

        public virtual Consultant Consultant { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
