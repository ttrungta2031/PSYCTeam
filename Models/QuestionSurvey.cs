using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class QuestionSurvey
    {
        public int Id { get; set; }
        public int? QuestionId { get; set; }
        public int? SurveyId { get; set; }

        public virtual Question Question { get; set; }
        public virtual Survey Survey { get; set; }
    }
}
