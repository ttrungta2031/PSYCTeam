using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Specialization
    {
        public int Id { get; set; }
        public int? ConsultantId { get; set; }
        public int? SpecializationTypeId { get; set; }

        public virtual Consultant Consultant { get; set; }
        public virtual SpecializationType SpecializationType { get; set; }
    }
}
