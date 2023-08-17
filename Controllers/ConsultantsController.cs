using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PsychologicalCounseling.Models;

namespace PsychologicalCounseling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultantsController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public ConsultantsController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/Consultants
        [HttpGet("Getallconsultant")]

        public IActionResult GetAllList(string search)



        {
            var result = (from s in _context.Consultants
                          where s.Status == "active"
                          select new
                          {
                              Id = s.Id,
                              FullName = s.FullName,
                              ImageUrl = s.ImageUrl,
                              Email = s.Email,
                              Address = s.Address,
                              Dob = s.Dob,
                              Gender = s.Gender,
                              Phone = s.Phone,
                              Status = s.Status,
                              Experience = s.Experience,
                              Rating = s.Rating,
                              Specialization = s.Specializations.Select(a => a.SpecializationType.Name)
                          }).ToList();
            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Consultants
                          where s.FullName.Contains(search) && s.Status == "active"  || s.Experience.ToString().Contains(search) && s.Status == "active" || s.Status.Contains(search) && s.Status == "active" || s.Rating.ToString().Contains(search) && s.Status == "active" || s.Specializations.Select(a => a.SpecializationType.Name).Contains(search) && s.Status == "active"
                          select new
                          {
                              Id = s.Id,
                              FullName = s.FullName,
                              ImageUrl = s.ImageUrl,
                              Email = s.Email,
                              Address = s.Address,
                              Dob = s.Dob,
                              Gender = s.Gender,
                              Phone = s.Phone,
                              Status = s.Status,
                              Experience = s.Experience,
                              Rating = s.Rating,
                              Specialization = s.Specializations.Select(a => a.SpecializationType.Name)

                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }




        [HttpGet("Getallconsultantbyadmin")]

        public IActionResult GetAllListByAdmin(string search)
        {
            var result = (from s in _context.Consultants

                          select new
                          {
                              Id = s.Id,
                              FullName = s.FullName,
                              ImageUrl = s.ImageUrl,
                              Email = s.Email,
                              Address = s.Address,
                              Dob = s.Dob,
                              Gender = s.Gender,
                              Phone = s.Phone,
                              Status = s.Status,
                              Experience = s.Experience,
                              Rating = s.Rating,
                              Flag = s.SlotBookings.Where(s => s.Status == "overdue").Count(),
                              //  SlotBooking =s.SlotBookings
                              // Flag = s.SlotBookings.Where(a => Formatdayver3(a.DateSlot, a.TimeStart) && a.Status == "booked").Count()

                          }).ToList();
            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Consultants
                          where s.FullName.Contains(search) || s.Experience.ToString().Contains(search) || s.Status.Contains(search) || s.Rating.ToString().Contains(search)
                          select new
                          {
                              Id = s.Id,
                              FullName = s.FullName,
                              ImageUrl = s.ImageUrl,
                              Email = s.Email,
                              Address = s.Address,
                              Dob = s.Dob,
                              Gender = s.Gender,
                              Phone = s.Phone,
                              Status = s.Status,
                              Experience = s.Experience,
                              Rating = s.Rating,
                              Flag = s.SlotBookings.Where(s => s.Status == "overdue").Count(),
                              // SlotBooking = s.SlotBookings
                              // Flag = s.SlotBookings.Where(a => Formatdayver3(a.DateSlot, a.TimeStart) == true && a.Status == "booked").Select(a => a.SlotId).Count()

                          }).ToList();
            }



            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }



        [HttpGet("Getconsultantbyspecial")]

        public IActionResult GetConsultantBySpecial(string search)



        {
            var result = (from s in _context.Specializations

                          select new
                          {
                              SpecialName = s.SpecializationType.Name,
                              ConsultantId = s.ConsultantId,
                              Name = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Email = s.Consultant.Email,
                              Address = s.Consultant.Address,
                              Dob = s.Consultant.Dob,
                              Gender = s.Consultant.Gender,
                              Phone = s.Consultant.Phone,
                              Experience = s.Consultant.Experience,
                              Rating = s.Consultant.Rating

                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Specializations
                          where s.SpecializationType.Name.Contains(search)
                          select new
                          {
                              SpecialName = s.SpecializationType.Name,
                              ConsultantId = s.ConsultantId,
                              Name = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Email = s.Consultant.Email,
                              Address = s.Consultant.Address,
                              Dob = s.Consultant.Dob,
                              Gender = s.Consultant.Gender,
                              Phone = s.Consultant.Phone,
                              Experience = s.Consultant.Experience,
                              Rating = s.Consultant.Rating

                          }).ToList();
            }


            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }



        public class Infoconsultant
        {

            public int Id { get; set; }
            public string FullName { get; set; }
            public string ImageUrl { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public DateTime? Dob { get; set; }
            public string Gender { get; set; }
            public string Phone { get; set; }
            public string Status { get; set; }
            public int? Experience { get; set; }
            public double? Rating { get; set; }
            public string Specialization { get; set; }

        }





        // GET: api/Consultants/5
        [HttpGet("getbyid")]

        public async Task<ActionResult> GetConsultant(int id)
        {
            var consu = _context.Consultants.Where(a => a.Id == id).FirstOrDefault();
            if (consu == null) return StatusCode(404, new { StatusCode = 404, Message = "Không tìm thấy thông tin Consultant" });

            var special = (from s in _context.Specializations
                           where s.ConsultantId == id
                           select new
                           {
                               Id = s.Id,
                               Specname = s.SpecializationType.Name,
                               SpecializationTypeId = s.SpecializationTypeId

                           }).ToList();
            Infoconsultant info = new Infoconsultant();
            

            info.Id = consu.Id;
            info.FullName = consu.FullName;
            info.ImageUrl = consu.ImageUrl;
            info.Email = consu.Email;
            info.Address = consu.Address;
            info.Dob = consu.Dob;
            info.Gender = consu.Gender;
            info.Phone = consu.Phone;
            info.Status = consu.Status;
            info.Experience = consu.Experience;
            info.Rating = consu.Rating;


        
        
        info.Specialization = "0";
            if(special.Count > 0) {
                info.Specialization = "";
                foreach (var item in special)
            {
                info.Specialization = info.Specialization + item.Specname +", ";

            }

                info.Specialization = info.Specialization.Remove(info.Specialization.Length - 2, 2);
            }


            List<Infoconsultant> listinfo = new List<Infoconsultant>()
            {
                new Infoconsultant{
                Id = info.Id,
                FullName = info.FullName,
            ImageUrl = info.ImageUrl,
            Email = info.Email,
            Address = info.Address,
            Dob = info.Dob,
            Gender = info.Gender,
            Phone = info.Phone,
           Status = info.Status,
            Experience = info.Experience,
            Rating = info.Rating,
            Specialization = info.Specialization
        }
            }.ToList();



            return Ok(new { StatusCode = 200, Message = "Load successful", Data = listinfo });
        }


        [HttpGet("getbyidv2")]

        public async Task<ActionResult> GetConsultantV2(int id)
        {
            var consu = _context.Consultants.Where(a => a.Id == id).FirstOrDefault();
            if (consu == null) return StatusCode(404, new { StatusCode = 404, Message = "Không tìm thấy thông tin Consultant" });

            var special = (from s in _context.Specializations
                           where s.ConsultantId == id
                           select new
                           {
                               Id = s.Id,
                               Specname = s.SpecializationType.Name,
                               SpecializationTypeId = s.SpecializationTypeId

                           }).ToList();
            Infoconsultant info = new Infoconsultant();


            info.Id = consu.Id;
            info.FullName = consu.FullName;
            info.ImageUrl = consu.ImageUrl;
            info.Email = consu.Email;
            info.Address = consu.Address;
            info.Dob = consu.Dob;
            info.Gender = consu.Gender;
            info.Phone = consu.Phone;
            info.Status = consu.Status;
            info.Experience = consu.Experience;
            info.Rating = consu.Rating;




            info.Specialization = "0";
            if (special.Count > 0)
            {
                info.Specialization = "";
                foreach (var item in special)
                {
                    info.Specialization = info.Specialization + item.Specname + ", ";

                }

                info.Specialization = info.Specialization.Remove(info.Specialization.Length - 2, 2);
            }


            List<Infoconsultant> listinfo = new List<Infoconsultant>()
            {
                new Infoconsultant{
                Id = info.Id,
                FullName = info.FullName,
            ImageUrl = info.ImageUrl,
            Email = info.Email,
            Address = info.Address,
            Dob = info.Dob,
            Gender = info.Gender,
            Phone = info.Phone,
           Status = info.Status,
            Experience = info.Experience,
            Rating = info.Rating,
            Specialization = info.Specialization
        }
            }.ToList();



            return Ok(new { StatusCode = 200, Message = "Load successful", data = info });
        }





        [HttpGet("getfeedbackbyid")]

        public async Task<ActionResult> GetFeedBackConsultant(int id)
        {
            var consu = _context.Consultants.Where(a => a.Id == id).FirstOrDefault();
            if (consu == null) return StatusCode(404, new { StatusCode = 404, Message = "Không tìm thấy thông tin Consultant" });

            var feedback = (from s in _context.Bookings
                           where s.ConsultantId == id && s.Feedback != null
                           select new
                           {
                               Id = s.Id,
                               Feedback = s.Feedback,
                               Rate = s.Rate,
                               DateCreate = s.DateBooking,
                               CustomerName = s.Customer.Fullname,
                               BookingId = s.Id

                           }).ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful" , data = feedback});
        }


        [HttpGet("checkpasswalletbyid")]

        public async Task<ActionResult> CheckPassWalletById(int id)
        {
            var all = _context.Consultants.AsQueryable();

            all = _context.Consultants.Where(us => us.Id.Equals(id));
            var result = all.ToList();
            if (result.Count <1)
            {
                return StatusCode(404, new { StatusCode = 404, Message = "Không tìm thấy thông tin Consultant" });
            }
            var wallet = _context.Wallets.Where(a => a.ConsultantId == id).FirstOrDefault();
           int checkpasswallet = 0;

            if (!string.IsNullOrEmpty(wallet.PassWord))
            {
                checkpasswallet = 1;
            }
          
            return Ok(new { StatusCode = 200, Message = "Load successful", data = checkpasswallet });
        }

        [HttpGet("getfeedbackbyconsuid")]

        public async Task<ActionResult> GetFeedbackByConsultant(int id)
        {
            var all = _context.Bookings.AsQueryable();
            all = _context.Bookings.Where(a => a.ConsultantId == id);
            var feedback = all.Where(a => a.Feedback !=null).ToList();
           



            return Ok(new { StatusCode = 200, Message = "Load successful", data = feedback });
        }



        // PUT: api/Consultants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]
   
        public async Task<IActionResult> PutConsultant(Consultant con)
        {

            try
            {
                var consultant = GetAccount_byEmailConsultant(con.Email);
             //   var consu = await _context.Consultants.FindAsync(con.Id);
                if (consultant == null)
                {
                    return NotFound();
                }

                consultant.FullName = con.FullName == null ? consultant.FullName : con.FullName; 
                consultant.Email = con.Email == null ? consultant.Email : consultant.Email; 
                consultant.AvartarUrl = con.AvartarUrl == null ? consultant.AvartarUrl : con.AvartarUrl;
                consultant.ImageUrl = con.ImageUrl == null ? consultant.ImageUrl : con.ImageUrl;
                consultant.Address = con.Address == null ? consultant.Address : con.Address; 
                consultant.Dob = con.Dob == null ? consultant.Dob : con.Dob; 
                consultant.Gender = con.Gender == null ? consultant.Gender : con.Gender; 
                consultant.Phone = con.Phone == null ? consultant.Phone : con.Phone; 
                consultant.Status = con.Status == null ? consultant.Status : consultant.Status; 
                consultant.Experience = con.Experience == null ? consultant.Experience : consultant.Experience; 
                consultant.Rating = con.Rating == null ? consultant.Rating : consultant.Rating; 



                _context.Consultants.Update(consultant);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // POST: api/Consultants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]
        [Authorize]

        public async Task<ActionResult<Consultant>> PostConsultant(Consultant con)
        {
            try
            {
                var consu = new Consultant();
                {
                    consu.FullName = con.FullName;
                    consu.Email = con.Email;
                    consu.ImageUrl = con.ImageUrl;
                    consu.AvartarUrl = consu.AvartarUrl;
                    consu.Address = con.Address;
                    consu.Dob = con.Dob;
                    consu.Gender = con.Gender;
                    consu.Phone = con.Phone;
                    consu.Status = con.Status;
                    consu.Experience = con.Experience;
                    consu.Rating = con.Rating;
   
                }
                _context.Consultants.Add(consu);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // DELETE: api/Consultants/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteConsultant(int id)
        {
            var consu = await _context.Consultants.FindAsync(id);
            if (consu == null)
            {
                return NotFound();
            }
            if (consu.Status == "active")
                consu.Status = "inactive";
            else
                consu.Status = "active";
            
            _context.Consultants.Update(consu);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Content = "The Consultant was deleted successfully!!" });
        }


        [HttpDelete("BanUnban")]
        [Authorize]
        public async Task<IActionResult> BanUnbanConsultant(int id)
        {
            var consu = await _context.Consultants.FindAsync(id);
            if (consu == null)
            {
                return NotFound();
            }
            if (consu.Status != "banned") { 

                consu.Status = "banned";
                _context.Consultants.Update(consu);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Content = "The Consultant was banned successfully!!" });
            }
            else {
                consu.Status = "active";
                _context.Consultants.Update(consu);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Content = "The Consultant has unban successfully!!" });
            }
            _context.Consultants.Update(consu);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Content = "The Consultant was deleted successfully!!" });
        }
        private Models.User GetAccount_byEmail(string email)
        {
            var account = _context.Users.Where(a => a.Email.ToUpper() == email.ToUpper()).FirstOrDefault();

            if (account == null)
            {
                return null;
            }

            return account;
        }
        private bool ConsultantExists(int id)
        {
            return _context.Consultants.Any(e => e.Id == id);
        }


        private Models.Consultant GetAccount_byEmailConsultant(string email)
        {
            var account = _context.Consultants.Where(a => a.Email.ToUpper() == email.ToUpper()).FirstOrDefault();

            if (account == null)
            {
                return null;
            }

            return account;
        }

        private bool Formatdayver3(DateTime? day, string time)
        {
            // string format = "dd/MM/yyyy";
            //   DateTime dayne = DateTime.Now.AddHours(-24);
            /* DateTime daytest = DateTime.UtcNow.AddHours(7);
             var abtest = daytest.ToShortDateString();
             string abctest = "18:00:00";
             var testresult = abtest + " "+ abctest;
             //10/27/2022 18:00:00
             DateTime testresult2 = DateTime.ParseExact(testresult, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);*/
            //   Console.WriteLine(testresult, testresult2);
            DateTime timenow = DateTime.UtcNow.AddHours(7);

            var time1 = day?.ToShortDateString();
            string timestart = time;
            var result = time1 + " " + timestart;
            //10/6/2022 09:00:00
            DateTime resultnew = DateTime.ParseExact(result, "M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            resultnew = resultnew.AddMinutes(36);
            //  timestart   compare timenow
            //vd:    30/10/2022 15:00:00    compare     27/10/2022 4:30:00     true  nhận về slot active và appointment
            if (resultnew.CompareTo(timenow) == 1) return false;
            return true;

        }
    }
}
