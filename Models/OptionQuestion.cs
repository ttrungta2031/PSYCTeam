using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class OptionQuestion
    {
        public OptionQuestion()
        {
            ResultSurveys = new HashSet<ResultSurvey>();
        }

        public int Id { get; set; }
        public string OptionText { get; set; }
        public string Type { get; set; }
        public int? QuestionId { get; set; }

        public virtual Question Question { get; set; }
        public virtual ICollection<ResultSurvey> ResultSurveys { get; set; }
    }
}
