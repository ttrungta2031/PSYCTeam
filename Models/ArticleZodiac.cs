using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class ArticleZodiac
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDay { get; set; }
        public string ContentNews { get; set; }
        public int? Articleid { get; set; }
        public int? Zodiacid { get; set; }

        public virtual Article Article { get; set; }
        public virtual Zodiac Zodiac { get; set; }
    }
}
