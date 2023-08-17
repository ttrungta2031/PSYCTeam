using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class ProductType
    {
        public int Id { get; set; }
        public int? ItemId { get; set; }
        public int? ZodiacId { get; set; }
        public string Status { get; set; }

        public virtual Product Item { get; set; }
        public virtual Zodiac Zodiac { get; set; }
    }
}
