using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class SpecializationType
    {
        public SpecializationType()
        {
            Specializations = new HashSet<Specialization>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Specialization> Specializations { get; set; }
    }
}
