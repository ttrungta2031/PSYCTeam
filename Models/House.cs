using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class House
    {
        public House()
        {
            PlanetHouses = new HashSet<PlanetHouse>();
            Profiles = new HashSet<Profile>();
            ZodiacHouses = new HashSet<ZodiacHouse>();
        }

        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Element { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
        public string Maincontent { get; set; }

        public virtual ICollection<PlanetHouse> PlanetHouses { get; set; }
        public virtual ICollection<Profile> Profiles { get; set; }
        public virtual ICollection<ZodiacHouse> ZodiacHouses { get; set; }
    }
}
