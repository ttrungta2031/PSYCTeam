using System;
using System.Collections.Generic;

#nullable disable

namespace PsychologicalCounseling.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Bookings = new HashSet<Booking>();
            Notifications = new HashSet<Notification>();
            Orders = new HashSet<Order>();
            Profiles = new HashSet<Profile>();
            ResponseResults = new HashSet<ResponseResult>();
            ResultSurveys = new HashSet<ResultSurvey>();
            Wallets = new HashSet<Wallet>();
        }

        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Birthchart { get; set; }
        public string ImageUrl { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime? Dob { get; set; }
        public string HourBirth { get; set; }
        public string MinuteBirth { get; set; }
        public string SecondBirth { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public int? ZodiacId { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Profile> Profiles { get; set; }
        public virtual ICollection<ResponseResult> ResponseResults { get; set; }
        public virtual ICollection<ResultSurvey> ResultSurveys { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
