using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class ZodiacPlanet
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? ZodiacId { get; set; }
        public int? PlanetId { get; set; }

        public virtual Planet Planet { get; set; }
        public virtual Zodiac Zodiac { get; set; }
    }
}
