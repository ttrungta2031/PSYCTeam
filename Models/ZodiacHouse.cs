using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class ZodiacHouse
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? ZodiacId { get; set; }
        public int? HouseId { get; set; }

        public virtual House House { get; set; }
        public virtual Zodiac Zodiac { get; set; }
    }
}
