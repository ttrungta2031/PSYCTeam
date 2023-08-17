using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Article
    {
        public Article()
        {
            ArticleZodiacs = new HashSet<ArticleZodiac>();
        }

        public int Id { get; set; }
        public string UrlBanner { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDay { get; set; }
        public string ContentNews { get; set; }
        public string Status { get; set; }

        public virtual ICollection<ArticleZodiac> ArticleZodiacs { get; set; }
    }
}
