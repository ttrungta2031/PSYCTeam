using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Shop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ConsultantId { get; set; }
    }
}
