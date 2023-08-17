using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PsychologicalCounseling.Models;
using PsychologicalCounseling.Services;

namespace PsychologicalCounseling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;
        private readonly IDrawnatalchart _drawnatal;
        public ProfilesController(PsychologicalCouselingContext context, IDrawnatalchart drawnatal)
        {
            _context = context;
            _drawnatal = drawnatal;
        }

        // GET: api/Profiles
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfiles()
        {
            return await _context.Profiles.ToListAsync();
        }

        // GET: api/Profiles/5
        [HttpGet("getbyid")]

        public async Task<ActionResult> GetProfile(int id)
        {
            var all = _context.Profiles.AsQueryable();

            all = _context.Profiles.Where(us => us.Id.Equals(id));
            var result = all.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        [HttpGet("lovecompatibility")]

        public async Task<ActionResult> LoveCompatibility(int customerid, int profileid)
        {
            var customer = _context.Customers.Where(a => a.Id == customerid).FirstOrDefault();
            var profile = _context.Profiles.Where(a => a.Id == profileid).FirstOrDefault();
            if(customer == null || profile == null) return StatusCode(400, new { StatusCode = 400, message = "Thông tin bị thiếu hoặc sai dữ liệu!" });
            int zodiacofcustomerid = (int)customer.ZodiacId;
            int zodiacofprofileid = (int)profile.ZodiacId;

            var zodiacofcustomer = _context.Zodiacs.Where(a => a.Id == zodiacofcustomerid).FirstOrDefault();
            var zodiacofprofile = _context.Zodiacs.Where(a => a.Id == zodiacofprofileid).FirstOrDefault();

            var detailzodiacofcustomer = zodiacofcustomer.DescriptionDetail;
            var detailzodiacofprofile = zodiacofprofile.DescriptionDetail;


            var tileresult = CompairZodiac(customerid, profileid); //% tỉ lệ hoà hợp







            return Ok(new { StatusCode = 200, Message = "Load successful", compatibility = tileresult ,zodiaccustomer = detailzodiacofcustomer, zodiacprofile = detailzodiacofprofile  });
        }

        [HttpGet("getbyidcustomer")]
      
        public async Task<ActionResult> GetProfileByIdCustomer(int id, int pagesize = 5, int pagenumber = 1)
        {
            var all = _context.Profiles.AsQueryable();
            all = _context.Profiles.Where(us => us.CustomerId.Equals(id));
            var result = all.ToList();
            var paging = result.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();

            double totalpage1 = (double)result.Count() / pagesize;
            totalpage1 = Math.Ceiling(totalpage1);
            return Ok(new { StatusCode = 200, Message = "Load successful", data = paging, totalpage = totalpage1 });
        }

        // PUT: api/Profiles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> PutProfile(Profile profile)
        {

            try
            {
               
               


                var pro = await _context.Profiles.FindAsync(profile.Id);
                if (pro == null)
                {
                    return NotFound();
                }
                DateTime daten;
                int zodiacid = -1;
                if (profile.Dob != null)
                {
                    daten = (DateTime)profile.Dob;
                    zodiacid = zodiacsign(daten);
                }
                pro.Name = profile.Name == null ? pro.Name : profile.Name;
                pro.ImageUrl = profile.ImageUrl == null ? pro.ImageUrl : profile.ImageUrl;
                pro.Dob = profile.Dob == null ? pro.Dob : profile.Dob;
                pro.BirthPlace = profile.BirthPlace == null ? pro.BirthPlace : profile.BirthPlace;
                pro.Latitude = profile.Latitude == null ? pro.Latitude : profile.Latitude;
                pro.Longitude = profile.Longitude == null ? pro.Longitude : profile.Longitude;
                pro.Gender = profile.Gender == null ? pro.Gender : profile.Gender;
                pro.Status = profile.Status == null ? pro.Status : profile.Status;
                pro.CustomerId = profile.CustomerId == null ? pro.CustomerId : profile.CustomerId;
                pro.ZodiacId = zodiacid < 0 ? pro.ZodiacId : zodiacid;
                pro.PlanetId = profile.PlanetId == null ? pro.PlanetId : profile.PlanetId;
                pro.HouseId = profile.HouseId == null ? pro.HouseId : profile.HouseId;


                DateTime date = (DateTime)pro.Dob;
                var datenew = date.ToShortDateString();
                var birthchart = _drawnatal.GetChartLinkFirebase(DateTime.Parse(datenew), double.Parse(pro.Longitude), double.Parse(pro.Latitude));
                pro.BirthChart = birthchart;


                _context.Profiles.Update(pro);
                await _context.SaveChangesAsync();

 




                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // POST: api/Profiles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]
 
        public async Task<ActionResult<Profile>> PostProfile(Profile profile)
        {
            try
            {
                DateTime date = (DateTime)profile.Dob;
                var datenew = date.ToShortDateString();
                DateTime daten = (DateTime)profile.Dob;
                var zodiacid = zodiacsign(daten);


                var birthchart = _drawnatal.GetChartLinkFirebase(DateTime.Parse(datenew), double.Parse(profile.Longitude), double.Parse(profile.Latitude));


                var pro = new Profile();
                {
                    pro.Name = profile.Name;
                    pro.ImageUrl = profile.ImageUrl;
                    pro.BirthChart = birthchart;
                    pro.Dob = profile.Dob;
                    pro.BirthPlace = profile.BirthPlace;
                    pro.Latitude = profile.Latitude;
                    pro.Longitude = profile.Longitude;
                    pro.Gender = profile.Gender;
                    pro.Status = "active";
                    pro.CustomerId = profile.CustomerId;
                    pro.ZodiacId = zodiacid < 0 ? profile.ZodiacId : zodiacid;
                    pro.PlanetId = profile.PlanetId;
                    pro.HouseId = profile.HouseId;
                        

                }
                _context.Profiles.Add(pro);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // DELETE: api/Profiles/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }

            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Content = "The Profile was delete successfully!!" });
        }

        private bool ProfileExists(int id)
        {
            return _context.Profiles.Any(e => e.Id == id);
        }

        private Models.Customer GetAccount_byEmailCustomer(string email)
        {
            var account = _context.Customers.Where(a => a.Email.ToUpper() == email.ToUpper()).FirstOrDefault();

            if (account == null)
            {
                return null;
            }

            return account;
        }
        private int zodiacsign(DateTime dob)
        {
            int zodiacid = 0;
            var month = dob.Month;
            var day = dob.Day;
            if (((month == 3) && (day >= 21 || day <= 31)) || ((month == 4) && (day >= 01 || day <= 20)))
            {
                zodiacid = 1;
                return zodiacid;
            }
            if (((month == 4) && (day >= 21 || day <= 31)) || ((month == 5) && (day >= 01 || day <= 20)))
            {
                zodiacid = 2;
                return zodiacid;
            }
            if (((month == 5) && (day >= 21 || day <= 31)) || ((month == 6) && (day >= 01 || day <= 21)))
            {
                zodiacid = 3;
                return zodiacid;
            }
            if (((month == 6) && (day >= 22 || day <= 31)) || ((month == 7) && (day >= 01 || day <= 22)))
            {
                zodiacid = 4;
            }
            if (((month == 7) && (day >= 23 || day <= 31)) || ((month == 8) && (day >= 01 || day <= 22)))
            {
                zodiacid = 5;
                return zodiacid;
            }
            if (((month == 8) && (day >= 23 || day <= 31)) || ((month == 9) && (day >= 01 || day <= 22)))
            {
                zodiacid = 6;
                return zodiacid;
            }
            if (((month == 9) && (day >= 23 || day <= 31)) || ((month == 10) && (day >= 01 || day <= 23)))
            {
                zodiacid = 7;
                return zodiacid;
            }
            if (((month == 10) && (day >= 24 || day <= 31)) || ((month == 11) && (day >= 01 || day <= 22)))
            {
                zodiacid = 8;
                return zodiacid;
            }
            if (((month == 11) && (day >= 23 || day <= 31)) || ((month == 12) && (day >= 01 || day <= 21)))
            {
                zodiacid = 9;
                return zodiacid;
            }
            if (((month == 12) && (day >= 22 || day <= 31)) || ((month == 1) && (day >= 01 || day <= 19)))
            {
                zodiacid = 10;
                return zodiacid;
            }
            if (((month == 1) && (day >= 20 || day <= 31)) || ((month == 2) && (day >= 01 || day <= 18)))
            {
                zodiacid = 11;
                return zodiacid;
            }
            if (((month == 2) && (day >= 19 || day <= 31)) || ((month == 3) && (day >= 01 || day <= 20)))
            {
                zodiacid = 12;
                return zodiacid;
            }


            return zodiacid;
        }

        private string CompairZodiac(int customerid, int profileid)
        {
            double compairdigit = 0;
            var customer = _context.Customers.Where(a => a.Id == customerid).FirstOrDefault();
            var profile = _context.Profiles.Where(a => a.Id == profileid).FirstOrDefault();

            int zodiacofcustomer = (int)customer.ZodiacId;
            int zodiacofprofile = (int)profile.ZodiacId;
            int[,] matrix;
            matrix = new int[13, 13];
            matrix[1, 1] = 50;
            matrix[1, 2] = 38;
            matrix[1, 3] = 83;
            matrix[1, 4] = 42;
            matrix[1, 5] = 97;
            matrix[1, 6] = 63;
            matrix[1, 7] = 85;
            matrix[1, 8] = 50;
            matrix[1, 9] = 78;
            matrix[1, 10] = 47;
            matrix[1, 11] = 93;
            matrix[1, 12] = 67;

            matrix[2, 1] = 38;
            matrix[2, 2] = 65;
            matrix[2, 3] = 33;
            matrix[2, 4] = 97;
            matrix[2, 5] = 73;
            matrix[2, 6] = 90;
            matrix[2, 7] = 65;
            matrix[2, 8] = 88;
            matrix[2, 9] = 98;          //  11
            matrix[2, 10] = 58;        // 9
            matrix[2, 11] = 30;    // 10
            matrix[2, 12] = 85;    // 12

            matrix[3, 1] = 83;
            matrix[3, 2] = 33;
            matrix[3, 3] = 60;
            matrix[3, 4] = 65;
            matrix[3, 5] = 88;
            matrix[3, 6] = 68;
            matrix[3, 7] = 93;
            matrix[3, 8] = 28;
            matrix[3, 10] = 85;        // 9
            matrix[3, 11] = 60;    // 10
            matrix[3, 9] = 68;          //  11                  
            matrix[3, 12] = 53;    // 12

            matrix[4, 1] = 42;
            matrix[4, 2] = 97;
            matrix[4, 3] = 65;
            matrix[4, 4] = 75;
            matrix[4, 5] = 35;
            matrix[4, 6] = 90;
            matrix[4, 7] = 43;
            matrix[4, 8] = 94;
            matrix[4, 10] = 27;        // 9
            matrix[4, 11] = 53;    // 10
            matrix[4, 9] = 83;          //  11                  
            matrix[4, 12] = 98;    // 12

            matrix[5, 1] = 97;
            matrix[5, 2] = 73;
            matrix[5, 3] = 88;
            matrix[5, 4] = 35;
            matrix[5, 5] = 45;
            matrix[5, 6] = 35;
            matrix[5, 7] = 97;
            matrix[5, 8] = 58;
            matrix[5, 10] = 68;        // 9
            matrix[5, 11] = 93;    // 10
            matrix[5, 9] = 35;          //  11                  
            matrix[5, 12] = 38;    // 12

            matrix[6, 1] = 63;
            matrix[6, 2] = 90;
            matrix[6, 3] = 68;
            matrix[6, 4] = 90;
            matrix[6, 5] = 35;
            matrix[6, 6] = 65;
            matrix[6, 7] = 68;
            matrix[6, 8] = 88;
            matrix[6, 10] = 30;        // 9
            matrix[6, 11] = 48;    // 10
            matrix[6, 9] = 95;          //  11                  
            matrix[6, 12] = 88;    // 12

            matrix[7, 1] = 85;
            matrix[7, 2] = 65;
            matrix[7, 3] = 93;
            matrix[7, 4] = 43;
            matrix[7, 5] = 97;
            matrix[7, 6] = 68;
            matrix[7, 7] = 75;
            matrix[7, 8] = 35;
            matrix[7, 10] = 90;        // 9
            matrix[7, 11] = 73;    // 10
            matrix[7, 9] = 55;          //  11                  
            matrix[7, 12] = 88;    // 12

            matrix[8, 1] = 50;
            matrix[8, 2] = 88;
            matrix[8, 3] = 28;
            matrix[8, 4] = 94;
            matrix[8, 5] = 58;
            matrix[8, 6] = 88;
            matrix[8, 7] = 35;
            matrix[8, 8] = 80;
            matrix[8, 10] = 73;        // 9
            matrix[8, 11] = 28;    // 10
            matrix[8, 9] = 95;          //  11                  
            matrix[8, 12] = 97;    // 12


            matrix[9, 1] = 78;
            matrix[9, 2] = 58;
            matrix[9, 3] = 85;
            matrix[9, 4] = 27;
            matrix[9, 5] = 68;
            matrix[9, 6] = 30;
            matrix[9, 7] = 90;
            matrix[9, 8] = 73;
            matrix[9, 10] = 60;        // 9
            matrix[9, 11] = 75;    // 10
            matrix[9, 9] = 68;          //  11                  
            matrix[9, 12] = 88;    // 12

            matrix[10, 1] = 93;
            matrix[10, 2] = 30;
            matrix[10, 3] = 60;
            matrix[10, 4] = 53;
            matrix[10, 5] = 93;
            matrix[10, 6] = 48;
            matrix[10, 7] = 73;
            matrix[10, 8] = 73;

            matrix[10, 10] = 90;        // 9
            matrix[10, 11] = 68;    // 10
            matrix[10, 9] = 60;          //  11                  
            matrix[10, 12] = 45;    // 12

            matrix[11, 1] = 47;
            matrix[11, 2] = 98;
            matrix[11, 3] = 68;
            matrix[11, 4] = 83;
            matrix[11, 5] = 35;
            matrix[11, 6] = 95;
            matrix[11, 7] = 55;
            matrix[11, 8] = 95;

            matrix[11, 10] = 68;        // 9
            matrix[11, 11] = 60;    // 10
            matrix[11, 9] = 75;          //  11                  
            matrix[11, 12] = 63;    // 12

            matrix[12, 1] = 67;
            matrix[12, 2] = 85;
            matrix[12, 3] = 53;
            matrix[12, 4] = 98;
            matrix[12, 5] = 38;
            matrix[12, 6] = 88;
            matrix[12, 7] = 88;
            matrix[12, 8] = 97;

            matrix[12, 10] = 45;        // 9
            matrix[12, 11] = 63;    // 10
            matrix[12, 9] = 88;          //  11                  
            matrix[12, 12] = 60;    // 12


            compairdigit = matrix[zodiacofcustomer, zodiacofprofile];

            return compairdigit.ToString() +"%";
        }
    }
}
