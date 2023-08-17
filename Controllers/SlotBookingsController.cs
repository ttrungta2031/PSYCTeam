using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PsychologicalCounseling.Models;

namespace PsychologicalCounseling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class SlotBookingsController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;
        private readonly IAgoraProvider _agoraProvider;
        public SlotBookingsController(PsychologicalCouselingContext context, IAgoraProvider agoraProvider)
        {
            _context = context;
            _agoraProvider = agoraProvider;
        }

        // GET: api/SlotBookings



        [HttpGet("GetSlotBookingByDateAndConsultanid")]
        public IActionResult GetAllList(string date, int consultantid)
        {
            // string format = "dd/MM/yyyy";
            //   DateTime dayne = DateTime.Now.AddHours(-24);
           /* DateTime daytest = DateTime.UtcNow.AddHours(7);
            var abtest = "10/6/2022";
            string abctest = "09:00:00";
            var testresult = abtest + " " + abctest;
            //10/6/2022 09:00:00
            //10/27/2022 18:00:00
            DateTime testresult2 = DateTime.ParseExact(testresult, "M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
               Console.WriteLine(testresult, testresult2);*/



            var result = (from s in _context.SlotBookings
                          where s.Status == "active" 
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId


                          }).ToList();
            if (!string.IsNullOrEmpty(date) && consultantid == 0)
            {
                result = (from s in _context.SlotBookings
                          where s.DateSlot.ToString().Contains(date)  && s.Status == "active"
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId

                          }).ToList();
            }
           else if (consultantid > 0 && string.IsNullOrEmpty(date))
            {
                result = (from s in _context.SlotBookings
                          where s.ConsultantId == consultantid && s.Status == "active" 
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId

                          }).ToList();
            }
            else if (!string.IsNullOrEmpty(date) && consultantid > 0 )
            {
                result = (from s in _context.SlotBookings
                          where s.DateSlot.ToString().Contains(date) && s.ConsultantId == consultantid && s.Status == "active"
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId

                          }).ToList();
            }
            var sort2 = result.Where(s => Formatday(s.DateSlot, s.TimeStart));
            var sort = sort2.OrderBy(x => DateTime.Parse(x.TimeStart)).ToList();

          // var paging = sort.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();



            return Ok(new { StatusCode = 200, Message = "Load successful", data = sort });
        }


        [HttpGet("GetSlotBookingByDateAndConsultanidv2")]
        public IActionResult GetSlotBookingByDateAndConsultanidv2(string date, int consultantid)
        {
            // string format = "dd/MM/yyyy";
            //   DateTime dayne = DateTime.Now.AddHours(-24);
            /* DateTime daytest = DateTime.UtcNow.AddHours(7);
             var abtest = "10/6/2022";
             string abctest = "09:00:00";
             var testresult = abtest + " " + abctest;
             //10/6/2022 09:00:00
             //10/27/2022 18:00:00
             DateTime testresult2 = DateTime.ParseExact(testresult, "M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                Console.WriteLine(testresult, testresult2);*/



            var result = (from s in _context.SlotBookings
                          where s.Status == "active" || s.Status =="booked"
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId


                          }).ToList();
            if (!string.IsNullOrEmpty(date) && consultantid == 0)
            {
                result = (from s in _context.SlotBookings
                          where s.DateSlot.ToString().Contains(date) && s.Status == "active" || s.DateSlot.ToString().Contains(date) && s.Status == "booked"
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId

                          }).ToList();
            }
            else if (consultantid > 0 && string.IsNullOrEmpty(date))
            {
                result = (from s in _context.SlotBookings
                          where s.ConsultantId == consultantid && s.Status == "active" || s.ConsultantId == consultantid && s.Status == "booked"
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId

                          }).ToList();
            }
            else if (!string.IsNullOrEmpty(date) && consultantid > 0)
            {
                result = (from s in _context.SlotBookings
                          where s.DateSlot.ToString().Contains(date) && s.ConsultantId == consultantid && s.Status == "active" || s.DateSlot.ToString().Contains(date) && s.ConsultantId == consultantid && s.Status == "booked"
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId

                          }).ToList();
            }
            var sort2 = result.Where(s => Formatday(s.DateSlot, s.TimeStart));
            var sort = sort2.OrderBy(x => DateTime.Parse(x.TimeStart)).ToList();

            // var paging = sort.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();



            return Ok(new { StatusCode = 200, Message = "Load successful", data = sort });
        }




        [HttpGet("GetSlotLiveStreamByDateAndConsultanid")]
        public IActionResult GetAllListLiveStream(string date, int consultantid)
        {
            // string format = "dd/MM/yyyy";

            var result = (from s in _context.SlotBookings
                          where s.Status == "livestream"
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              Description = s.Description,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId


                          }).ToList();
            if (!string.IsNullOrEmpty(date) && consultantid == 0)
            {
                result = (from s in _context.SlotBookings
                          where s.DateSlot.ToString().Contains(date) && s.Status == "livestream"
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              Description = s.Description,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId

                          }).ToList();
            }
            else if (consultantid > 0 && string.IsNullOrEmpty(date))
            {
                result = (from s in _context.SlotBookings
                          where s.ConsultantId == consultantid && s.Status == "livestream"
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              Description = s.Description,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId

                          }).ToList();
            }
            else if (!string.IsNullOrEmpty(date) && consultantid > 0)
            {
                result = (from s in _context.SlotBookings
                          where s.DateSlot.ToString().Contains(date) && s.ConsultantId == consultantid && s.Status == "livestream"
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              Description = s.Description,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId

                          }).ToList();
            }

            var sort2 = result.Where(s => Formatdayver2(s.DateSlot, s.TimeStart));
            var sort = sort2.OrderBy(x => DateTime.Parse(x.TimeStart)).ToList();

            // var paging = sort.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();



            return Ok(new { StatusCode = 200, Message = "Load successful", data = sort });
        }








        [HttpGet("GetAppointmentByCustomerid")]
        public IActionResult GetAppointmentByCustomerid(string date, int customerid, int pagesize = 5, int pagenumber = 1)
        {
            DateTime dayne = DateTime.Now.AddHours(7);
            var result = (from s in _context.SlotBookings
                          where s.Booking.CustomerId == customerid && s.Status == "booked" 
                          select new
                          {
                              Id = s.SlotId,

                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId


                          }).ToList();
            if (string.IsNullOrEmpty(date) && customerid > 0)
            {
                result = (from s in _context.SlotBookings
                          where s.Booking.CustomerId == customerid && s.Status == "booked" 
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId

                          }).ToList();


              
            }
            else if (!string.IsNullOrEmpty(date) && customerid > 0)
            {
                result = (from s in _context.SlotBookings
                          where s.Booking.CustomerId == customerid && s.DateSlot.ToString().Contains(date) && s.Status == "booked"
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId

                          }).ToList();

              
            }

            /*foreach (var item in result.ToList())
            {
                if (Formatday(item.TimeStart))
                {
                    result.Remove(item);
                }

            }*/
            var sort2 = result.Where(s => Formatdayver3(s.DateSlot, s.TimeStart));
            var sort = sort2.OrderBy(x => DateTime.Parse(x.TimeStart)).ToList();
           // var sort = result.OrderBy(x => DateTime.Parse(x.TimeStart)).ToList();
            var paging = sort.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();

            double totalpage1 = (double)sort.Count() / pagesize;
            totalpage1 = Math.Ceiling(totalpage1);
            return Ok(new { StatusCode = 200, Message = "Load successful", data = paging , totalpage = totalpage1});
           // return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }


        [HttpGet("GetHistoryByCustomerid")]
        public IActionResult GetHistoryByCustomerid(string date, int customerid, int pagesize = 5, int pagenumber = 1)
        {
            // string format = "dd/MM/yyyy";
            DateTime day = DateTime.Now;
            var result = (from s in _context.SlotBookings
                          where s.Booking.CustomerId == customerid && s.Status != "booked" 
                          select new
                          {
                              Id = s.SlotId,

                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId,
                              Rating = s.Booking.Rate,
                              Feedback = s.Booking.Feedback


                          }).ToList();

            if (string.IsNullOrEmpty(date) && customerid > 0)
            {
                result = (from s in _context.SlotBookings
                          where s.Booking.CustomerId == customerid && s.Status != "booked"
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId,
                              Rating = s.Booking.Rate,
                              Feedback = s.Booking.Feedback

                          }).ToList();
           
                
              
            }
            else if (!string.IsNullOrEmpty(date) && customerid > 0)
            {
                result = (from s in _context.SlotBookings
                          where s.Booking.CustomerId == customerid && s.DateSlot.ToString().Contains(date) && s.Status != "booked" 
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ConsultantName = s.Consultant.FullName,
                              ImageUrl = s.Consultant.ImageUrl,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId,
                              Rating = s.Booking.Rate,
                              Feedback = s.Booking.Feedback

                          }).ToList();

               
            }

         /*   foreach (var item in result.ToList())
            {
                if (Formatday(item.TimeStart) == false)
                {
                    result.Remove(item);
                }

            }*/

            var sort = result.OrderBy(x => DateTime.Parse(x.TimeStart)).ToList();

            var paging = sort.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
            double totalpage1 = (double)sort.Count() / pagesize;
            totalpage1 = Math.Ceiling(totalpage1);
            return Ok(new { StatusCode = 200, Message = "Load successful", data = paging , totalpage = totalpage1});
         //   return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }









        [HttpGet("GetAppointmentByConsultantid")]
        public IActionResult GetAppointmentByConsultantid(string date, int consultantid)
        {
            // string format = "dd/MM/yyyy";
            DateTime day = DateTime.Now;
            var result = (from s in _context.SlotBookings
                          where s.ConsultantId == consultantid && s.Status == "booked"
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ImageUrl = s.Booking.Customer.ImageUrl,
                              CustomerName = s.Booking.Customer.Fullname,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId


                          }).ToList();

            if (!string.IsNullOrEmpty(date) && consultantid >= 0)
            {
                result = (from s in _context.SlotBookings
                          where s.ConsultantId == consultantid && s.DateSlot.ToString().Contains(date) && s.Status == "booked" 
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ImageUrl = s.Booking.Customer.ImageUrl,
                              CustomerName = s.Booking.Customer.Fullname,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId

                          }).ToList();

              
            }
            /*  foreach (var item in result.ToList())
              {
                  if (Formatday(item.TimeStart))
                  {
                      result.Remove(item);
                  }

              }*/
            var sort2 = result.Where(s => Formatdayver3(s.DateSlot, s.TimeStart));
            var sort = sort2.OrderBy(x => DateTime.Parse(x.TimeStart)).ToList();
            return Ok(new { StatusCode = 200, Message = "Load successful", data = sort });

          //  return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }


        [HttpGet("GetHistoryByConsultantid")]
        public IActionResult GetHistoryByConsultantid(string date, int consultantid)
        {
            // string format = "dd/MM/yyyy";

/*            DateTime day = DateTime.Now; // DateTime
            var gio = day.Hour;
            var phut = day.Minute;
            var noww = day.TimeOfDay; //TimeSpan
            string timecurrent = DateTime.Now.ToString("HH:mm");

            var timecurrent2 = DateTime.Parse(timecurrent);

*/
          
            var result = (from s in _context.SlotBookings
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ImageUrl = s.Booking.Customer.ImageUrl,
                              CustomerName = s.Booking.Customer.Fullname,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId


                          }).ToList();

            if (!string.IsNullOrEmpty(date) && consultantid > 0)
            {
                result = (from s in _context.SlotBookings       //LINQ        // giờ đang 8:13 >     truyền list có thằng mà dưới (< 8h13). nhận 
                          where s.ConsultantId == consultantid && s.DateSlot.ToString().Contains(date) && s.Status == "cancel" || s.ConsultantId == consultantid && s.DateSlot.ToString().Contains(date) && s.Status == "overdue" || s.ConsultantId == consultantid && s.DateSlot.ToString().Contains(date) && s.Status == "success"
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              ImageUrl = s.Booking.Customer.ImageUrl,
                              CustomerName = s.Booking.Customer.Fullname,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId
                              
                          }).ToList();

                var sort = result.OrderBy(x => DateTime.Parse(x.TimeStart)).ToList();
                return Ok(new { StatusCode = 200, Message = "Load successful", data = sort });
            }
            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });

        }

        // GET: api/SlotBookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SlotBooking>> GetSlotBooking(int id)
        {
            var slotBooking = await _context.SlotBookings.FindAsync(id);

            if (slotBooking == null)
            {
                return NotFound();
            }

            return slotBooking;
        }

        [HttpGet("getdetailslotbookingv2")]

        public async Task<ActionResult> GetDetailSlotBookingByIdV2(int id)
        {
            //   var all = _context.RoomVideoCalls.AsQueryable();
            var slotBooking = await _context.SlotBookings.FindAsync(id);
            //   all = _context.RoomVideoCalls.Where(us => us.SlotId.Equals(id));
            var result = (from s in _context.SlotBookings
                          where s.SlotId == id
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Date = s.DateSlot,
                              Rate = s.Booking.Rate == null ? 0 : s.Booking.Rate,
                              Feedback = s.Booking.Feedback,
                              Price = s.Price,       
                              ImageUrlCustomer = s.Booking.Customer.ImageUrl,
                              CustomerName = s.Booking.Customer.Fullname,
                              ReasonCustomer = s.ReasonOfCustomer,
                              ReasonConsultant = s.ReasonOfConsultant,
                              Status = s.Status


                          }).ToList();
            if (slotBooking == null)
            {
                return StatusCode(404, new { StatusCode = 404, Message = "Notfound!!" });
            }


            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }






        [HttpGet("getdetailslotbooking")]

        public async Task<ActionResult> GetDetailSlotBookingById(int id)
        {
            //   var all = _context.RoomVideoCalls.AsQueryable();
            var slotBooking = await _context.SlotBookings.FindAsync(id);
            //   all = _context.RoomVideoCalls.Where(us => us.SlotId.Equals(id));
            var result = (from s in _context.SlotBookings
                          where s.SlotId == id
                          select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Date = s.DateSlot,
                              Rate = s.Booking.Rate,
                              Feedback = s.Booking.Feedback,
                              Price = s.Price,
                              Description = s.Description,
                              ImageUrlCustomer = s.Booking.Customer.ImageUrl,
                              CustomerName = s.Booking.Customer.Fullname,
                              ImageUrlConsultant = s.Consultant.ImageUrl,
                              ConsultantName = s.Consultant.FullName,
                              ReasonCustomer = s.ReasonOfCustomer,
                              ReasonConsultant = s.ReasonOfConsultant,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId


                          }).ToList();
            if (slotBooking == null)
            {
                return StatusCode(404, new { StatusCode = 404, Message = "Notfound!!" });
            }
          
               
                return Ok(new { StatusCode = 200, Message = "Load successful", data = result });    
        }


        public class DetailCustomer
        {
            public int? SlotId { get; set; }
            public string CustomerName { get; set; }
            public string UrlImageCustomer { get; set; }
            public int? price { get; set; }
            public string TimeStart { get; set; }
            public string TimeEnd { get; set; }
            public DateTime? DateOfSlot { get; set; }
            public string ResultSurvey { get; set; }
            public string DiscChart { get; set; }
            public string UrlBirthChart { get; set; }


        }
        [HttpGet("detailcustomerbyslotid")]

        public async Task<ActionResult> DetailCustomerBySlotId(int id)
        {
            //   var all = _context.RoomVideoCalls.AsQueryable();
       //     var slotBooking = await _context.SlotBookings.FindAsync(id);

            //   all = _context.RoomVideoCalls.Where(us => us.SlotId.Equals(id));
            //   
            var slotBooking = _context.SlotBookings.Where(a => a.SlotId == id).FirstOrDefault();
            if (slotBooking == null)
            {
                return StatusCode(404, new { StatusCode = 404, Message = "Slot Notfound!!" });
            }
            var booking = _context.Bookings.Where(a => a.Id == slotBooking.BookingId).FirstOrDefault();
            if (booking == null)
            {
                return StatusCode(404, new { StatusCode = 404, Message = " Booking Notfound!!" });
            }
            var customer = _context.Customers.Where(a => a.Id == booking.CustomerId).FirstOrDefault();
            int flagmaxresult = 0;

            var resultidmax = _context.ResponseResults.Where(a => a.CustomerId == customer.Id).Any() ;
          if(resultidmax ==true)
            {
                flagmaxresult = _context.ResponseResults.Where(a => a.CustomerId == customer.Id).Max(i => i.Id);
            }

           
            var result = _context.ResponseResults.Where(a => a.CustomerId == customer.Id && a.Id == flagmaxresult).FirstOrDefault();
            
            var birthchart = _context.Customers.Where(a => a.Id == customer.Id).FirstOrDefault();



            DetailCustomer detail = new DetailCustomer();
            {
                detail.SlotId = id;
                detail.CustomerName = customer.Fullname;
                detail.UrlImageCustomer = customer.ImageUrl;
                detail.price = slotBooking.Price;
                detail.TimeStart = slotBooking.TimeStart;
                detail.TimeEnd = slotBooking.TimeEnd;
                detail.DateOfSlot = slotBooking.DateSlot;
                detail.UrlBirthChart = birthchart.Birthchart;

            }


            if(result != null)
            {
                detail.ResultSurvey = result.DetailResult;
                detail.DiscChart = result.Description;
            }
            if (result == null)
            {
                detail.ResultSurvey = "Chưa có thông tin";
                detail.DiscChart = "Chưa có thông tin";
            }





            List<DetailCustomer> listdetail = new List<DetailCustomer>()
            {
                new DetailCustomer
                {
                    SlotId = detail.SlotId,
                    CustomerName = detail.CustomerName,
                UrlImageCustomer = detail.UrlImageCustomer,
              price = detail.price,
               TimeStart = detail.TimeStart,
                  TimeEnd = detail.TimeEnd,
               DateOfSlot = detail.DateOfSlot,
              UrlBirthChart = detail.UrlBirthChart,
              ResultSurvey = detail.ResultSurvey,
             DiscChart = detail.DiscChart,
        }
            }.ToList();

   



            return Ok(new { StatusCode = 200, Message = "Load successful", data = listdetail });
        }


        [HttpGet("getroomslotbooking")]

        public async Task<ActionResult> GetRoomSlotBookingById(int id)
        {
         //   var all = _context.RoomVideoCalls.AsQueryable();

         //   all = _context.RoomVideoCalls.Where(us => us.SlotId.Equals(id));
           var result = (from s in _context.RoomVideoCalls
                         where s.SlotId == id
                      select new
                      {
                          Id = s.Id,
                          ChanelName = s.ChanelName,
                          Token = s.Token,
                      }).ToList();
          //  string appid = "249ed20e39a7470f9e7ed035b2fa4022";
            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // PUT: api/SlotBookings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSlotBooking(int id, SlotBooking slotBooking)
        {
            if (id != slotBooking.SlotId)
            {
                return BadRequest();
            }

            _context.Entry(slotBooking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SlotBookingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SlotBookings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]

        public async Task<ActionResult<Zodiac>> PostSlot(SlotBooking slotbooking)
        {
            try
            {
                DateTime ts = DateTime.Parse(slotbooking.TimeStart);
                DateTime t = ts.AddMinutes(30);
                var  result = (from s in _context.SlotBookings
                          where s.ConsultantId == slotbooking.ConsultantId && s.DateSlot == slotbooking.DateSlot && s.Status == "active" || s.ConsultantId == slotbooking.ConsultantId && s.DateSlot == slotbooking.DateSlot && s.Status == "booked" || s.ConsultantId == slotbooking.ConsultantId && s.DateSlot == slotbooking.DateSlot && s.Status == "livestream"
                               select new
                          {
                              Id = s.SlotId,
                              TimeStart = s.TimeStart,
                              TimeEnd = s.TimeEnd,
                              Price = s.Price,
                              DateSlot = s.DateSlot,
                              Status = s.Status,
                              ConsultantId = s.ConsultantId,
                              BookingId = s.BookingId
                              
                          }).ToList();

                foreach (var item in result)
                {
                    
                    DateTime timestart = DateTime.Parse(item.TimeStart);
                    DateTime timeend = DateTime.Parse(item.TimeEnd);
                    if(ts <= timeend && ts >= timestart)
                    {
                        return StatusCode(409, new { StatusCode = 409, Message = "Khoảng thời gian đã được tạo. Vui lòng chọn khoảng thời gian khác!" });
                    }
                    
                }

                var consultant = _context.Consultants.Where(a => a.Id == slotbooking.ConsultantId).FirstOrDefault();
                int pricelevel = 0;
                if(consultant.Experience == 1)
                {
                    pricelevel = 30;

                }
                if (consultant.Experience == 2)
                {
                    pricelevel = 40;

                }
                if (consultant.Experience == 3)
                {
                    pricelevel = 50;

                }
                if (consultant.Experience == 4)
                {
                    pricelevel =65;

                }
                if (consultant.Experience == 5)
                {
                    pricelevel = 90;
                }



                var slot = new SlotBooking();
                {
                    slot.TimeStart = ts.TimeOfDay.ToString();
                    slot.TimeEnd = t.TimeOfDay.ToString();
                    slot.DateSlot = slotbooking.DateSlot;
                    slot.Price = pricelevel;
                    slot.Status = "active";
                    slot.ConsultantId = slotbooking.ConsultantId;
                }
                _context.SlotBookings.Add(slot);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }





        [HttpPost("createlivestream")]

        public async Task<ActionResult<Zodiac>> PostSlotLiveStream(SlotBooking slotbooking)
        {
            try
            {


                var consultant = _context.Consultants.Where(a => a.Id == slotbooking.ConsultantId).FirstOrDefault();
                if(consultant.Experience < 2)
                {
                    return StatusCode(400, new { StatusCode = 400, Message = "Lưu ý: cấp độ từ 2 trở lên mới có thể tạo cuộc hẹn livestream!" });
                }



                DateTime ts = DateTime.Parse(slotbooking.TimeStart);
                DateTime t = ts.AddMinutes(120);
                var result = (from s in _context.SlotBookings
                              where s.ConsultantId == slotbooking.ConsultantId && s.DateSlot == slotbooking.DateSlot && s.Status == "active" || s.ConsultantId == slotbooking.ConsultantId && s.DateSlot == slotbooking.DateSlot && s.Status == "booked" || s.ConsultantId == slotbooking.ConsultantId && s.DateSlot == slotbooking.DateSlot && s.Status == "livestream"
                              select new
                              {
                                  Id = s.SlotId,
                                  TimeStart = s.TimeStart,
                                  TimeEnd = s.TimeEnd,
                                  Price = s.Price,
                                  DateSlot = s.DateSlot,
                                  Status = s.Status,
                                  ConsultantId = s.ConsultantId,
                                  BookingId = s.BookingId

                              }).ToList();

                foreach (var item in result)
                {

                    DateTime timestart = DateTime.Parse(item.TimeStart);
                    DateTime timeend = DateTime.Parse(item.TimeEnd);
                    if (ts <= timeend && ts >= timestart)
                    {
                        return StatusCode(409, new { StatusCode = 409, Message = "Khoảng thời gian đã được tạo. Vui lòng chọn khoảng thời gian khác!" });
                    }

                }
                



                var slot = new SlotBooking();
                {
                    slot.TimeStart = ts.TimeOfDay.ToString();
                    slot.TimeEnd = t.TimeOfDay.ToString();
                    slot.DateSlot = slotbooking.DateSlot;
                    slot.Description = slotbooking.Description;
                    slot.Price = 0;
                    slot.Status = "livestream";
                    slot.ConsultantId = slotbooking.ConsultantId;
                }
                _context.SlotBookings.Add(slot);
                await _context.SaveChangesAsync();


                var slotidnew = _context.SlotBookings.Max(it => it.SlotId);


                string chanelname = "SLOTLIVESTREAM_" + slotidnew;
                chanelname = chanelname.Replace(" ", "");
                var token = _agoraProvider.GenerateToken(chanelname, 0.ToString(), 0);

                var roomvideocall = new RoomVideoCall();
                {
                    roomvideocall.ChanelName = chanelname;
                    roomvideocall.SlotId = slotidnew;
                    roomvideocall.Token = token;
                }
                _context.RoomVideoCalls.Add(roomvideocall);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }



        // DELETE: api/SlotBookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSlotBooking(int id)
        {
            var slotBooking = await _context.SlotBookings.FindAsync(id);
            if (slotBooking == null)
            {
                return NotFound();
            }

            _context.SlotBookings.Remove(slotBooking);
            await _context.SaveChangesAsync();

            return StatusCode(200, new { StatusCode = 200, Message = "Delete successful" });
        }


        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateSlotBooking(SlotBooking slot)
        {
            try
            {
                var slotbooking = await _context.SlotBookings.FindAsync(slot.SlotId);
                if (slotbooking == null)
                {
                    return NotFound();
                }
                DateTime ts = DateTime.Parse(slot.TimeStart);
                DateTime t = ts.AddMinutes(30);
                var result = (from s in _context.SlotBookings
                              where s.ConsultantId == slotbooking.ConsultantId && s.DateSlot == slotbooking.DateSlot
                              select new
                              {
                                  Id = s.SlotId,
                                  TimeStart = s.TimeStart,
                                  TimeEnd = s.TimeEnd,
                                  Price = s.Price,
                                  DateSlot = s.DateSlot,
                                  Status = s.Status,
                                  ConsultantId = s.ConsultantId,
                                  BookingId = s.BookingId

                              }).ToList();

                foreach (var item in result)
                {

                    DateTime timestart = DateTime.Parse(item.TimeStart);
                    DateTime timeend = DateTime.Parse(item.TimeEnd);
                    if (ts <= timeend && ts >= timestart)
                    {
                        return Ok(new { StatusCode = 200, Message = "Khoảng thời gian đã được tạo. Vui lòng chọn khoảng thời gian khác!" });
                    }

                }


                slotbooking.TimeStart = ts.TimeOfDay.ToString() == null ? slotbooking.TimeStart : ts.TimeOfDay.ToString();
                slotbooking.TimeEnd = t.TimeOfDay.ToString() == null ? slotbooking.TimeEnd : t.TimeOfDay.ToString();       
           
                                     
                _context.SlotBookings.Update(slotbooking);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        [HttpPut("confirmvideocall")]
        [Authorize]
        public async Task<IActionResult> ConfirmSuccessVideoCall(int id)
        {

            var slot = await _context.SlotBookings.FindAsync(id);
            if (slot == null)
            {
                return StatusCode(400, new { StatusCode = 400, Message = "The id of slot notfound!!" });
            }
            var wallet = _context.Wallets.Where(a => a.ConsultantId == slot.ConsultantId).FirstOrDefault();
            var consultant = _context.Consultants.Where(a => a.Id == slot.ConsultantId).FirstOrDefault();
            //  var walletadmin = _context.Wallets.Where(a => a.IsAdmin == "admin").FirstOrDefault();
            var booking = _context.Bookings.Where(a => a.Id == slot.BookingId).FirstOrDefault();
            if (slot.Status != "booked") return StatusCode(400, new { StatusCode = 400, Message = "The id of slot notfound or slot not booked!!" });

            if (slot.Status == "booked") { 
                slot.Status = "success";

                wallet.Crab = (int?)(wallet.Crab + slot.Price * 100 / 100);

                wallet.HistoryTrans = DateTime.Now.AddHours(7);
                wallet.ConsultantId = slot.ConsultantId;
                _context.Wallets.Update(wallet);
                DateTime dateslot = (DateTime)slot.DateSlot;
              //  DateTime dateslot = (DateTime)slotbooking.DateSlot;
                string newString = dateslot.ToString("dd/MM/yyyy");
                var payment = new Payment();
                {

                    payment.Among = slot.Price;
                    payment.Status = "confirm";

                }
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();          
                var paymentnew = _context.Payments.Max(it => it.Id);

                var trans = new Transaction();
                {
                    trans.PaymentId = paymentnew;
                    trans.DateCreate = DateTime.Now.AddHours(7);              
                    trans.WalletId = wallet.Id;
                    trans.Description = "+" + slot.Price + " GEM cho slot: " + slot.TimeStart + " Ngày: " + newString;
                }

                _context.Transactions.Add(trans);



              //  walletadmin.Crab = (int?)(walletadmin.Crab + slot.Price * 20 / 100);
              //  walletadmin.HistoryTrans = DateTime.Now.AddHours(7);
              //  _context.Wallets.Update(walletadmin);



                booking.Status = "success";
                _context.Bookings.Update(booking);

                await _context.SaveChangesAsync();





            }
            else slot.Status = slot.Status;
            _context.SlotBookings.Update(slot);
            await _context.SaveChangesAsync();



            //check uplevel
            var resultsuccess = (from s in _context.SlotBookings    
                      where s.ConsultantId == slot.ConsultantId && s.Status == "success"
                      select new
                      {
                          Id = s.SlotId
                      }).ToList();

            var countsuccess = resultsuccess.Count();
            if(countsuccess == 20 && consultant.Experience <2)
            {
                consultant.Experience = 2;
                var notifi = new Models.Notification();
                {
                    notifi.ConsultantId = consultant.Id;
                    notifi.DateCreate = DateTime.Now.AddHours(7);
                    notifi.IsAdmin = false;
                    notifi.Type = "Uplevel";
                    notifi.Status = "notseen";
                    notifi.Description = "Chúc mừng Bạn đã nâng cấp lên LV2!";

                }
                _context.Notifications.Add(notifi);
                await _context.SaveChangesAsync();
            }
            else if(countsuccess == 50 && consultant.Experience < 3)
                 {
                consultant.Experience = 3;
                var notifi = new Models.Notification();
                {
                    notifi.ConsultantId = consultant.Id;
                    notifi.DateCreate = DateTime.Now.AddHours(7);
                    notifi.IsAdmin = false;
                    notifi.Type = "Uplevel";
                    notifi.Status = "notseen";
                    notifi.Description = "Chúc mừng Bạn đã nâng cấp lên LV3!";

                }
                _context.Notifications.Add(notifi);
                await _context.SaveChangesAsync();
            }
            else if (countsuccess == 100 && consultant.Experience < 4)
                 {
                consultant.Experience = 4;
                var notifi = new Models.Notification();
                {
                    notifi.ConsultantId = consultant.Id;
                    notifi.DateCreate = DateTime.Now.AddHours(7);
                    notifi.IsAdmin = false;
                    notifi.Type = "Uplevel";
                    notifi.Status = "notseen";
                    notifi.Description = "Chúc mừng Bạn đã nâng cấp lên LV4!";

                }
                _context.Notifications.Add(notifi);
                await _context.SaveChangesAsync();
            } else if (countsuccess == 200 && consultant.Experience < 5)
            {
                consultant.Experience = 5;
                var notifi = new Models.Notification();
                {
                    notifi.ConsultantId = consultant.Id;
                    notifi.DateCreate = DateTime.Now.AddHours(7);
                    notifi.IsAdmin = false;
                    notifi.Type = "Uplevel";
                    notifi.Status = "notseen";
                    notifi.Description = "Chúc mừng Bạn đã nâng cấp lên LV5!";

                }
                _context.Notifications.Add(notifi);
                await _context.SaveChangesAsync();
            }
            _context.Consultants.Update(consultant);
            await _context.SaveChangesAsync();








            return Ok(new { StatusCode = 200, Content = "The SlotBooking was Confirm successfully!!" });
        }


        [HttpPut("confirmlivestream")]
        [Authorize]
        public async Task<IActionResult> ConfirmSuccessLiveStream(int id)
        {

            var slot = await _context.SlotBookings.FindAsync(id);
            if (slot == null)
            {
                return StatusCode(400, new { StatusCode = 400, Message = "The id of slot notfound!!" });
            }
            var wallet = _context.Wallets.Where(a => a.ConsultantId == slot.ConsultantId).FirstOrDefault();

            if (slot.Status != "livestream") return StatusCode(400, new { StatusCode = 400, Message = "The id of slot notfound or slot not booked!!" });

            if (slot.Status == "livestream")
            {
                slot.Status = "livestreamover";             
                DateTime dateslot = (DateTime)slot.DateSlot;
          //      DateTime dateslot = (DateTime)slotbooking.DateSlot;
                string newString = dateslot.ToString("dd/MM/yyyy");

                var payment = new Payment();
                {

                    payment.Among = 0;
                    payment.Status = "confirm";

                }
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                var paymentnew = _context.Payments.Max(it => it.Id);



                var trans = new Transaction();
                {
                    trans.PaymentId = paymentnew;
                    trans.DateCreate = DateTime.Now.AddHours(7);
                    trans.WalletId = wallet.Id;
                    trans.Description = "Slot LiveStream: " + slot.TimeStart + " Ngày: " + newString + " Kết thúc";
                }

                _context.Transactions.Add(trans);
                await _context.SaveChangesAsync();




            }
            else slot.Status = slot.Status;
            _context.SlotBookings.Update(slot);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Content = "The Slot LiveStream was Confirm successfully!!" });
        }






        [HttpPut("cancelbyconsultant")]
 
        public async Task<IActionResult> CancelByConsultant(int id, string reason)
        {
          
            var slot = await _context.SlotBookings.FindAsync(id);
            if (slot == null)
            {
                return StatusCode(400, new { StatusCode = 400, Message = "The id of slot notfound!!" });
            }

         

            var booking = _context.Bookings.Where(a => a.Id == slot.BookingId).FirstOrDefault();
            var wallet = _context.Wallets.Where(a => a.CustomerId == booking.CustomerId).FirstOrDefault();

            if (slot.Status != "booked") return StatusCode(400, new { StatusCode = 400, Message = "The id of slot notfound or slot not booked!!" });
            DateTime dateslot = (DateTime)slot.DateSlot;
            string newString = dateslot.ToString("dd/MM/yyyy");
            if (slot.Status == "booked")
            {







                slot.Status = "cancel";
                slot.ReasonOfConsultant = " Lý do: " + reason + "(" + " Ngày: " + newString +" " +slot.TimeStart +")" ;

                _context.SlotBookings.Update(slot);











                wallet.Crab = (int?)(wallet.Crab + slot.Price);
                wallet.HistoryTrans = DateTime.Now.AddHours(7);
                wallet.CustomerId = slot.Booking.CustomerId;
                _context.Wallets.Update(wallet);

                var payment = new Payment();
                {

                    payment.Among = slot.Price;
                    payment.Status = "confirm";

                }
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                var paymentnew = _context.Payments.Max(it => it.Id);



                var trans = new Transaction();
                {
                    trans.PaymentId = paymentnew;
                    trans.DateCreate = DateTime.Now.AddHours(7);
                    trans.WalletId = wallet.Id;
                    trans.Description = "+" + slot.Price + " GEM cho slot: " + slot.TimeStart + " Ngày: " + newString + ". Chuyên gia đã hủy cuộc hẹn này vì lý do: " + reason;
                }

                _context.Transactions.Add(trans);


                var notifi = new Models.Notification();
                {
                    notifi.CustomerId = booking.CustomerId;
                    notifi.DateCreate = DateTime.Now.AddHours(7);
                    notifi.IsAdmin = false;
                    notifi.Type = "cancel";
                    notifi.Status = "notseen";
                    notifi.Description = slot.TimeStart + " Ngày: " + newString + " đã bị hủy!";

                }
                _context.Notifications.Add(notifi);




                slot.DateSlot = DateTime.Now.AddHours(7);
                _context.SlotBookings.Update(slot);
                await _context.SaveChangesAsync();



               




                return Ok(new { StatusCode = 200, Content = "The SlotBooking was Cancel successfully!!" });

            }
            

            return Ok(new { StatusCode = 409, Content = "The SlotBooking was Cancel Failed!!" });
        }




        [HttpPut("cancelbycustomer")]

        public async Task<IActionResult> CancelByCustomer(int id, string reason)
        {

            var slot = await _context.SlotBookings.FindAsync(id);
            if (slot == null)
            {
                return StatusCode(400, new { StatusCode = 400, Message = "The id of slot notfound!!" });
            }

            var consutan = _context.Consultants.Where(us => us.Id == slot.ConsultantId).FirstOrDefault();
            var user = _context.Users.Where(us => us.Email.ToUpper() == consutan.Email.ToUpper()).FirstOrDefault();
            var booking = _context.Bookings.Where(a => a.Id == slot.BookingId).FirstOrDefault();
            var wallet = _context.Wallets.Where(a => a.CustomerId == booking.CustomerId).FirstOrDefault();

            if (slot.Status != "booked") return StatusCode(400, new { StatusCode = 400, Message = "The id of slot notfound or slot not booked!!" });
            DateTime dateslot = (DateTime)slot.DateSlot;
            string newString = dateslot.ToString("dd/MM/yyyy");
            if (slot.Status == "booked")
            {


                slot.Status = "cancel";
                slot.ReasonOfCustomer = " Lý do: " + reason + "(" + " Ngày: " + newString + " " + slot.TimeStart + ")";

                _context.SlotBookings.Update(slot);

                wallet.Crab = (int?)(wallet.Crab + slot.Price);
                wallet.HistoryTrans = DateTime.Now.AddHours(7);
                wallet.CustomerId = slot.Booking.CustomerId;
                _context.Wallets.Update(wallet);

                var payment = new Payment();
                {

                    payment.Among = slot.Price;
                    payment.Status = "confirm";

                }
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                var paymentnew = _context.Payments.Max(it => it.Id);





                var trans = new Transaction();
                {
                    trans.PaymentId = paymentnew;
                    trans.DateCreate = DateTime.Now.AddHours(7);
                    trans.WalletId = wallet.Id;
                    trans.Description = "+" + slot.Price + " GEM cho slot: " + slot.TimeStart + " Ngày: " + newString + ". Bạn đã hủy cuộc hẹn này vì lý do: " + reason;
                }

                _context.Transactions.Add(trans);



                /* FirebaseApp.Create(new AppOptions()
                 {
                     Credential = GoogleCredential.FromFile("private_key.json")
                 });*/

                var notifi = new Models.Notification();
                {
                    notifi.ConsultantId = slot.ConsultantId;
                    notifi.DateCreate = DateTime.Now.AddHours(7);
                    notifi.IsAdmin = false;
                    notifi.Type = "cancel";
                    notifi.Status = "notseen";
                    notifi.Description = slot.TimeStart + " Ngày: " + newString + " đã bị hủy!";

                }
                _context.Notifications.Add(notifi);
                await _context.SaveChangesAsync();




                if (FirebaseApp.DefaultInstance == null)
                {
                   FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("private_key.json")
                    });
                }

                //   var fcmToken = "fKkLgEfjJC5RiR0xaHFGDn:APA91bGL9WuFwTRMGkXdd4wYcv3iQ4W-wIkkLrpC3vpnDwhrYF9rdLcmqJWIsn3CUcud2rGW4bCf0YjY0PKNlZVWZ89HSh7ibfCgLqnClgZnaZERwow4j15uquxqwKxlS1iu3agYrZ1v";
                //  fcm cua web: var fcmToken = "fKkLgEfjJC5RiR0xaHFGDn:APA91bGL9WuFwTRMGkXdd4wYcv3iQ4W-wIkkLrpC3vpnDwhrYF9rdLcmqJWIsn3CUcud2rGW4bCf0YjY0PKNlZVWZ89HSh7ibfCgLqnClgZnaZERwow4j15uquxqwKxlS1iu3agYrZ1v";
                //  eEE7zdTlTbuXcwuX32NQu2: APA91bFVAUk78aqB0 - Xgzdy7qi5 - wwLgRLXxIbQcuijxALJTfpegEkj - WdwT_OskwSQgh7 - kUTclXZ2YUg_Plf4S1VM3rLxlYdVZtE2 - LqYtWgAX9Tp8XnOmoLA1l4FDLzREk2sFEMmD
                // fcmtoken moi cai thiet bi
                var fcmToken = user.FcmToken;
                var message = new Message()
                {
                    Data = new Dictionary<string, string>()
                {
                    {"Mydatav1","PSYCteamv1" },
                },
                    Token = fcmToken,
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Title = "Slot bị hủy",
                        Body = slot.TimeStart + " Ngày: " + newString + " đã bị hủy!"
                    }
                };
                await FirebaseMessaging.DefaultInstance.SendAsync(message);





                slot.DateSlot = DateTime.Now.AddHours(7);
                _context.SlotBookings.Update(slot);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 200, Content = "The SlotBooking was Cancel successfully!!" });

            }

            return StatusCode(409, new { StatusCode = 409, Message = "The SlotBooking was Cancel Failed!!" });
       //     return Ok(new { StatusCode = 409, Content = "The SlotBooking was Cancel Failed!!" });
        }





        [HttpPut("remove")]
        [Authorize]
        public async Task<IActionResult> RemoveSlotBooking(int id)
        {
            var us = await _context.SlotBookings.FindAsync(id);
            if (us == null)
            {
                return NotFound();
            }
            if (us.Status == "active" || us.Status =="livestream")
                us.Status = "inactive";
            else
            {
                return StatusCode(409, new { StatusCode = 409, Message = "Remove Fail because it not support!" });
            }
            _context.SlotBookings.Update(us);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Content = "The SlotBooking was removed successfully!!" });
        }
        private bool SlotBookingExists(int id)
        {
            return _context.SlotBookings.Any(e => e.SlotId == id);
        }

        private bool Formatday(DateTime? day ,string time)
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
              //  timestart   compare timenow
              //vd:    30/10/2022 15:00:00    compare     27/10/2022 4:30:00     true  nhận về slot active và appointment
            if(resultnew.CompareTo(timenow) == 1) return true;
            return false;



            //   DateTime utcDateTime = DateTime.UtcNow.AddHours(7);

            //  string vnTimeZoneKey = "SE Asia Standard Time";
            // TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            //string timecurrent = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone).ToString("HH:mm");
            /*  string timecurrent = DateTime.Now.AddHours(7).ToString("HH:mm");
              var timenew = string.Format("{0:t}",time);
              var timecurrent2 =  DateTime.Parse(timecurrent);

              var timestart = DateTime.Parse(timenew);         
              Console.WriteLine(timestart);
              Console.WriteLine(timecurrent2);
              if (timestart.CompareTo(timecurrent2) == 1) // thoi gian bat dau lớn hơn thời gian hiện tại.Appointment lấy, History Remove
                  return true;

              return false;*/


        }

        private bool Formatdayver2(DateTime? day, string time)
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
            resultnew= resultnew.AddHours(2);
            //  timestart   compare timenow
            //vd:    30/10/2022 15:00:00    compare     27/10/2022 4:30:00     true  nhận về slot active và appointment
            if (resultnew.CompareTo(timenow) == 1) return true;
            return false;



            //   DateTime utcDateTime = DateTime.UtcNow.AddHours(7);

            //  string vnTimeZoneKey = "SE Asia Standard Time";
            // TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            //string timecurrent = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone).ToString("HH:mm");
            /*  string timecurrent = DateTime.Now.AddHours(7).ToString("HH:mm");
              var timenew = string.Format("{0:t}",time);
              var timecurrent2 =  DateTime.Parse(timecurrent);

              var timestart = DateTime.Parse(timenew);         
              Console.WriteLine(timestart);
              Console.WriteLine(timecurrent2);
              if (timestart.CompareTo(timecurrent2) == 1) // thoi gian bat dau lớn hơn thời gian hiện tại.Appointment lấy, History Remove
                  return true;

              return false;*/


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
            if (resultnew.CompareTo(timenow) == 1) return true;
            return false;



            //   DateTime utcDateTime = DateTime.UtcNow.AddHours(7);

            //  string vnTimeZoneKey = "SE Asia Standard Time";
            // TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            //string timecurrent = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone).ToString("HH:mm");
            /*  string timecurrent = DateTime.Now.AddHours(7).ToString("HH:mm");
              var timenew = string.Format("{0:t}",time);
              var timecurrent2 =  DateTime.Parse(timecurrent);

              var timestart = DateTime.Parse(timenew);         
              Console.WriteLine(timestart);
              Console.WriteLine(timecurrent2);
              if (timestart.CompareTo(timecurrent2) == 1) // thoi gian bat dau lớn hơn thời gian hiện tại.Appointment lấy, History Remove
                  return true;

              return false;*/


        }

        private bool Formatdayver4(DateTime? day, string time)
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
            DateTime resultnew = DateTime.ParseExact(result, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            resultnew = resultnew.AddMinutes(36);
            //  timestart   compare timenow
            //vd:    30/10/2022 15:00:00    compare     27/10/2022 4:30:00     true  nhận về slot active và appointment
            if (resultnew.CompareTo(timenow) == 1) return true;
            return false;



            //   DateTime utcDateTime = DateTime.UtcNow.AddHours(7);

            //  string vnTimeZoneKey = "SE Asia Standard Time";
            // TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            //string timecurrent = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone).ToString("HH:mm");
            /*  string timecurrent = DateTime.Now.AddHours(7).ToString("HH:mm");
              var timenew = string.Format("{0:t}",time);
              var timecurrent2 =  DateTime.Parse(timecurrent);

              var timestart = DateTime.Parse(timenew);         
              Console.WriteLine(timestart);
              Console.WriteLine(timecurrent2);
              if (timestart.CompareTo(timecurrent2) == 1) // thoi gian bat dau lớn hơn thời gian hiện tại.Appointment lấy, History Remove
                  return true;

              return false;*/


        }

        /*DateTime? _convertStringToDateTime(String time)
        {
            DateTime? _dateTime;
            try
            {
                _dateTime =  DateTime.Parse(time).ToString("hh:mm");
            }
            catch (e) { }
            return _dateTime;
        }*/
    }
}
