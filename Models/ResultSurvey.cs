using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class ResultSurvey
    {
        public int Id { get; set; }
        public int? OptionQuestionId { get; set; }
        public int? QuestionId { get; set; }
        public int? CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual OptionQuestion OptionQuestion { get; set; }
        public virtual Question Question { get; set; }
    }
}
