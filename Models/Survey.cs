using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Survey
    {
        public Survey()
        {
            QuestionSurveys = new HashSet<QuestionSurvey>();
            Questions = new HashSet<Question>();
            ResponseResults = new HashSet<ResponseResult>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? SurveyTypeId { get; set; }
        public string Status { get; set; }

        public virtual SurveyType SurveyType { get; set; }
        public virtual ICollection<QuestionSurvey> QuestionSurveys { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<ResponseResult> ResponseResults { get; set; }
    }
}
