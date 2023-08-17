using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Question
    {
        public Question()
        {
            OptionQuestions = new HashSet<OptionQuestion>();
            QuestionSurveys = new HashSet<QuestionSurvey>();
            ResultSurveys = new HashSet<ResultSurvey>();
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public int? SurveyId { get; set; }

        public virtual Survey Survey { get; set; }
        public virtual ICollection<OptionQuestion> OptionQuestions { get; set; }
        public virtual ICollection<QuestionSurvey> QuestionSurveys { get; set; }
        public virtual ICollection<ResultSurvey> ResultSurveys { get; set; }
    }
}
