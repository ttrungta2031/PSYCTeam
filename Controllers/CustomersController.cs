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
    public class CustomersController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;
        private readonly IDrawnatalchart _drawnatal;
        public CustomersController(PsychologicalCouselingContext context, IDrawnatalchart drawnatal)
        {
            _context = context;
            _drawnatal = drawnatal;
        }

        // GET: api/Customers

        [HttpGet("Getallcustomer")]
        public IActionResult GetAllList(string search)
        {
            var result = (from s in _context.Customers
                          select new
                          {
                              Id = s.Id,
                              Fullname=s.Fullname,
                              ImageUrl =s.ImageUrl,
                              Birthchart = s.Birthchart,
                              Longtitude = s.Longitude,
                              Latitude=s.Latitude,
                              Email =s.Email,
                              Address =s.Address,
                              Dob =s.Dob,
                              HourBirth =s.HourBirth,
                              MinuteBirth =s.MinuteBirth,
                              SecondBirth =s.SecondBirth,
                              Gender =s.Gender,
                              Phone =s.Phone,
                              Status =s.Status
                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Customers
                          where s.Fullname.Contains(search) || s.Status.Contains(search) || s.Email.Contains(search)
                          select new
                          {
                              Id = s.Id,
                              Fullname = s.Fullname,
                              ImageUrl = s.ImageUrl,
                              Birthchart = s.Birthchart,
                              Longtitude = s.Longitude,
                              Latitude = s.Latitude,
                              Email = s.Email,
                              Address = s.Address,
                              Dob = s.Dob,
                              HourBirth = s.HourBirth,
                              MinuteBirth = s.MinuteBirth,
                              SecondBirth = s.SecondBirth,
                              Gender = s.Gender,
                              Phone = s.Phone,
                              Status = s.Status
                          }).ToList();
            }
          
            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }
        // GET: api/Customers/5
        [HttpGet("getbyid")]

        public async Task<ActionResult> GetCustomer(int id)
        {
            var all = _context.Customers.AsQueryable();

            all = _context.Customers.Where(us => us.Id.Equals(id));
            var result = all.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }



        [HttpGet("getbyemail")]
 
        public async Task<ActionResult> GetCustomerByEmail(string email)
        {
            var all = _context.Customers.AsQueryable();

            all = _context.Customers.Where(us => us.Email.Equals(email));
            var result = all.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }
        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]  
        public async Task<IActionResult> PutCustomer(Customer cus)
        {

            try
            {
                var custom = GetAccount_byEmailCustomer(cus.Email);
              //  var customer = await _context.Customers.FindAsync(cus.Id);
                if (custom == null)
                {
                    return NotFound();
                }
                DateTime daten;
                int zodiacid =-1;
                if (cus.Dob != null) { 
                 daten = (DateTime)cus.Dob;         
                 zodiacid = zodiacsign(daten);
                }


                custom.Fullname = cus.Fullname == null ? custom.Fullname : cus.Fullname; 
                custom.ImageUrl = cus.ImageUrl == null ? custom.ImageUrl : cus.ImageUrl;             
                custom.Longitude = cus.Longitude == null ? custom.Longitude : cus.Longitude.ToString();
                custom.Latitude = cus.Latitude == null ? custom.Latitude : cus.Latitude.ToString();
                custom.Email = cus.Email == null ? custom.Email : cus.Email; 
                custom.Address = cus.Address == null ? custom.Address : cus.Address;
                custom.Dob = cus.Dob == null ? custom.Dob : cus.Dob; 
                custom.HourBirth = cus.HourBirth == null ? custom.HourBirth : cus.HourBirth; 
                custom.MinuteBirth = cus.MinuteBirth == null ? custom.MinuteBirth : cus.MinuteBirth; 
                custom.SecondBirth = cus.SecondBirth == null ? custom.SecondBirth : cus.SecondBirth; 
                custom.Gender = cus.Gender == null ? custom.Gender : cus.Gender; 
                custom.Phone = cus.Phone == null ? custom.Phone : cus.Phone;
                custom.Status = cus.Status == null ? custom.Status : custom.Status;
                custom.ZodiacId = zodiacid < 0 ? custom.ZodiacId : zodiacid;
                DateTime date = (DateTime)custom.Dob;
                var datenew = date.ToShortDateString();
                var birthchart = _drawnatal.GetChartLinkFirebase(DateTime.Parse(datenew), double.Parse(custom.Longitude), double.Parse(custom.Latitude));

                custom.Birthchart = birthchart == null ? "No" : birthchart; 


                _context.Customers.Update(custom);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]
        [Authorize]

        public async Task<ActionResult<Customer>> PostCustomer(Customer cus)
        {
            try
            {
                DateTime date = (DateTime)cus.Dob;
                var datenew = date.ToShortDateString();

                var zodiacid = zodiacsign(date);
                var birthchart = this._drawnatal.GetChartLinkFirebase(DateTime.Parse(datenew), double.Parse(cus.Longitude), double.Parse(cus.Latitude));

                var customer = new Customer();
                {
                    customer.Fullname = cus.Fullname;
                    customer.ImageUrl = cus.ImageUrl;
                    customer.Email = cus.Email;
                    customer.Birthchart = birthchart;
                    customer.Address = cus.Address;
                    customer.Latitude = cus.Latitude;
                    customer.Longitude = cus.Longitude;
                    customer.Dob = cus.Dob;
                    customer.HourBirth = cus.HourBirth;
                    customer.MinuteBirth = cus.MinuteBirth;
                    customer.SecondBirth = cus.SecondBirth;
                    customer.Gender = cus.Gender;
                    customer.Phone = cus.Phone;
                    customer.Status = cus.Status;
                    customer.ZodiacId = zodiacid;
                }
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            if (customer.Status == "active")
                customer.Status = "inactive";
            else
                customer.Status = "active";

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Content = "The Customer was deleted successfully!!" });
        }

        [HttpDelete("BanUnban")]
        [Authorize]
        public async Task<IActionResult> BanorUnbanCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            if (customer.Status != "banned") {
                customer.Status = "banned";
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Content = "The Customer was banned successfully!!" });
            }
            else { 
                customer.Status = "active";
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Content = "The Customer was unban successfully!!" });
            }
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Content = "The Customer was deleted successfully!!" });
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
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
    }
}
