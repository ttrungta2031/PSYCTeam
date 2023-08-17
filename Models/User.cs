using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Email { get; set; }
        public string IsAdmin { get; set; }
        public string Firebaseid { get; set; }
        public string FcmToken { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
    }
}
