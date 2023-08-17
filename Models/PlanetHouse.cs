using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class PlanetHouse
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? PlanetId { get; set; }
        public int? HouseId { get; set; }

        public virtual House House { get; set; }
        public virtual Planet Planet { get; set; }
    }
}
