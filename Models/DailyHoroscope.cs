using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class DailyHoroscope
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? Date { get; set; }
        public string Context { get; set; }
        public string Job { get; set; }
        public string Affection { get; set; }
        public int? LuckyNumber { get; set; }
        public string GoodTime { get; set; }
        public string Color { get; set; }
        public string ShouldThing { get; set; }
        public string ShouldNotThing { get; set; }
        public int? ZodiacId { get; set; }

        public virtual Zodiac Zodiac { get; set; }
    }
}
