using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Zodiac
    {
        public Zodiac()
        {
            ArticleZodiacs = new HashSet<ArticleZodiac>();
            DailyHoroscopes = new HashSet<DailyHoroscope>();
            ProductTypes = new HashSet<ProductType>();
            Profiles = new HashSet<Profile>();
            ZodiacHouses = new HashSet<ZodiacHouse>();
            ZodiacPlanets = new HashSet<ZodiacPlanet>();
        }

        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string DescriptionShort { get; set; }
        public string DescriptionDetail { get; set; }

        public virtual ICollection<ArticleZodiac> ArticleZodiacs { get; set; }
        public virtual ICollection<DailyHoroscope> DailyHoroscopes { get; set; }
        public virtual ICollection<ProductType> ProductTypes { get; set; }
        public virtual ICollection<Profile> Profiles { get; set; }
        public virtual ICollection<ZodiacHouse> ZodiacHouses { get; set; }
        public virtual ICollection<ZodiacPlanet> ZodiacPlanets { get; set; }
    }
}
