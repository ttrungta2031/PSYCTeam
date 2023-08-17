using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Admin
    {
        public Admin()
        {
            Shops = new HashSet<Shop>();
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime Dob { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Shop> Shops { get; set; }
    }
}
