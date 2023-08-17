using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class ResponseResult
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string DetailResult { get; set; }
        public int? CustomerId { get; set; }
        public int? SurveyId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Survey Survey { get; set; }
    }
}
