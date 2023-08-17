using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class SurveyType
    {
        public SurveyType()
        {
            Surveys = new HashSet<Survey>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Survey> Surveys { get; set; }
    }
}
