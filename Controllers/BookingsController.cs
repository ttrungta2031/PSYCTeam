using System;
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
using PsychologicalCounseling.Services;

//using agora.rtc;
namespace PsychologicalCounseling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;
        private readonly IAgoraProvider _agoraProvider;
        // private IAgoraRtcEngine rtc_engine_ = null;
        private readonly ISendNotiService _sendnoti;

        public BookingsController(PsychologicalCouselingContext context, IAgoraProvider agoraProvider, ISendNotiService sendnoti)
        {
            _context = context;
            _agoraProvider = agoraProvider;
            _sendnoti = sendnoti;

            //     rtc_engine_ = rtc_engine;
        }

        // GET: api/Bookings

        [HttpGet("Gethistorybooking")]
        [Authorize]
        public IActionResult GetAllListHistory(int id, string date)
        {
            var result = (from s in _context.Bookings
                          select new
                          {
                              Id = s.Id,
                              ỈmageUrl = s.Customer.ImageUrl,
                              TimeStart = s.Customer.Fullname,
                              Price =s.Price,
                              Feedback = s.Feedback,
                             Duration = s.Duration,
                              DateBooking = DateTime.Now.AddHours(7),
                             PaymentId = s.PaymentId,
                                ConsultantId = s.ConsultantId,
                             CustomerId = s.CustomerId,

            Status = s.Status
                          }).ToList();

            if (id>=0)
            {
                result = (from s in _context.Bookings
                          where s.ConsultantId.Equals(id) && s.DateBooking.ToString().Contains(date)
                          select new
                          {
                              Id = s.Id,
                              ỈmageUrl = s.Customer.ImageUrl,
                              TimeStart = s.Customer.Fullname,
                              Price = s.Price,
                              Feedback = s.Feedback,
                              Duration = s.Duration,
                              DateBooking = DateTime.Now.AddHours(7),
                              PaymentId = s.PaymentId,
                              ConsultantId = s.ConsultantId,
                              CustomerId = s.CustomerId,

                              Status = s.Status
                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }


        [HttpGet("Gethistorybookingcustomer")]
        [Authorize]
        public IActionResult GetAllListHistoryCustomer(int id, string date)
        {
            var result = (from s in _context.Bookings
                          select new
                          {
                              Id = s.Id,
                              Price = s.Price,
                              Feedback = s.Feedback,
                              Duration = s.Duration,
                              DateBooking = DateTime.Now.AddHours(7),
                              PaymentId = s.PaymentId,
                              ConsultantId = s.ConsultantId,
                              CustomerId = s.CustomerId,

                              Status = s.Status
                          }).ToList();

            if (id >= 0)
            {
                result = (from s in _context.Bookings
                          where s.CustomerId.Equals(id) && s.DateBooking.ToString().Contains(date)
                          select new
                          {
                              Id = s.Id,
                              Price = s.Price,
                              Feedback = s.Feedback,
                              Duration = s.Duration,
                              DateBooking = DateTime.Now.AddHours(7),
                              PaymentId = s.PaymentId,
                              ConsultantId = s.ConsultantId,
                              CustomerId = s.CustomerId,

                              Status = s.Status
                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }


        [HttpGet("Getappointmentbookingcustomer")]
        [Authorize]
        public IActionResult GetAllListAppoitmentCustomer(int id, string date)
        {
            var result = (from s in _context.Bookings
                          select new
                          {
                              Id = s.Id,

                              Price = s.Price,
                              Feedback = s.Feedback,
                              Duration = s.Duration,
                              DateBooking = s.DateBooking,
                              PaymentId = s.PaymentId,
                              ConsultantId = s.ConsultantId,
                              CustomerId = s.CustomerId,

                              Status = s.Status
                          }).ToList();

            if (id >= 0)
            {
                result = (from s in _context.Bookings
                          where s.CustomerId.Equals(id) && s.DateBooking.ToString().Contains(date) && s.Status == "booked"
                          select new
                          {
                              Id = s.Id,
                              Price = s.Price,
                              Feedback = s.Feedback,
                              Duration = s.Duration,
                              DateBooking = s.DateBooking,
                              PaymentId = s.PaymentId,
                              ConsultantId = s.ConsultantId,
                              CustomerId = s.CustomerId,

                              Status = s.Status
                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }






        [HttpGet("Getappointmentbooking")]
        [Authorize]
        public IActionResult GetAllListAppoitment(int id, string date)
        {
            var result = (from s in _context.Bookings
                          select new
                          {
                              Id = s.Id,
                              ỈmageUrl = s.Customer.ImageUrl,
                              TimeStart = s.Customer.Fullname,
                              Price = s.Price,
                              Feedback = s.Feedback,
                              Duration = s.Duration,
                              DateBooking = DateTime.Now.AddHours(7),
                              PaymentId = s.PaymentId,
                              ConsultantId = s.ConsultantId,
                              CustomerId = s.CustomerId,

                              Status = s.Status
                          }).ToList();

            if (id >= 0)
            {
                result = (from s in _context.Bookings
                          where s.ConsultantId.Equals(id) && s.DateBooking.ToString().Contains(date) && s.Status == "booked"
                          select new
                          {
                              Id = s.Id,
                              ỈmageUrl = s.Customer.ImageUrl,
                              TimeStart = s.Customer.Fullname,
                              Price = s.Price,
                              Feedback = s.Feedback,
                              Duration = s.Duration,
                              DateBooking = DateTime.Now,
                              PaymentId = s.PaymentId,
                              ConsultantId = s.ConsultantId,
                              CustomerId = s.CustomerId,

                              Status = s.Status
                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings(string chanelname)
        {
            /*  string token = "007eJxTYJBqnMK+4b3vm+1/n3bN/eY73TquxV1dcPqvZV+mfv9/znSXAoORiWVqipFBqrFlormJuUGaZap5aoqBsWmSUVqiiYGR0emXhsne1sbJcUzWrIwMEAjiMzGEGDIwAADsxiEw";

              rtc_engine_ = AgoraRtcEngine.CreateAgoraRtcEngine();
            //  rtc_engine_ = (IAgoraRtcEngine)AgoraRtcEngine.GetEngine("249ed20e39a7470f9e7ed035b2fa4022");
              RtcEngineContext rtc_engine_context = new RtcEngineContext("249ed20e39a7470f9e7ed035b2fa4022");
              rtc_engine_.Initialize(rtc_engine_context);
              rtc_engine_.CreateChannel("T1");*/
            var token = _agoraProvider.GenerateToken(chanelname, 4.ToString(), 30);
            int id = 105;
            string tenslot = "12:30";
            DateTime ngay = DateTime.Now.AddHours(7);
            string channel = id + tenslot + ngay;
            channel = channel.Replace(" ", "");

                
            
            return Ok(new { StatusCode = 200, Message = "Load successful", data = token, chanelname= channel});


        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            //  var auth = agora.rtc.
            // var abc = agora.rtc.AgoraRtcEngine.CreateAgoraRtcEngine();
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

          //  return slotBooking;
            return Ok(new { StatusCode = 200, Message = "Load successful", data = booking });
        }

        // PUT: api/Bookings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBooking(int id, Booking booking)
        {
          

            return NoContent();
        }



        [HttpPut("feedbackbycustomer")]
        public async Task<IActionResult> FeedbackByCustomer(int id, string feedback, int rate)
        {
            var booking = await _context.Bookings.FindAsync(id);



            if (booking == null)
            {
                return StatusCode(404, new { StatusCode = 404, Message = "NotFound the id of booking" });
            }
            if(booking.Feedback != null || booking.Rate >0) return StatusCode(400, new { StatusCode = 400, Message = "Booking has already feedback or rating" });
            var consultant = _context.Consultants.Where(a => a.Id == booking.ConsultantId).FirstOrDefault();

            if (booking.Status == "booked" ) return StatusCode(400, new { StatusCode = 400, Message = "Booking hasn't exist!" });

            if (booking.Status == "success")
            {
                booking.Feedback = feedback;
                booking.Rate = rate;
            }
            else booking.Status = booking.Status;


            double tbc = rate;
            int count = 0;
            var ratingbooking = (from s in _context.Bookings
                                 where s.ConsultantId == booking.ConsultantId && s.Status == "success"
                                 select new
                                 {
                                     Rate = s.Rate

                                 }).ToList();
            if (ratingbooking.Count > 0) { 
            foreach (var item in ratingbooking)
                {
                    if (item.Rate != null)
                    {

                        tbc = tbc + (int)item.Rate;
                        count = count + 1;
                    }
                }

                count = count + 1;
                double resultrating = tbc / count;
                consultant.Rating = Math.Round(resultrating, 1);
                _context.Consultants.Update(consultant);


                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
            }
           

            return Ok(new { StatusCode = 200, Content = "The booking was feedback successfully!!" });
        }
        // POST: api/Bookings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]
       
        public async Task<ActionResult<Booking>> PostBooking(int slotid, int customerid, int consultantid)
        {
            try
            {
                //   var wallet = _context.Customers.AsQueryable();
                //   wallet = _context.Customers.Where(cu => cu.Id.Equals(booking.CustomerId));

                //var wallet = await _context.Wallets.FindAsync(booking.CustomerId);

                var consutan = _context.Consultants.Where(us => us.Id == consultantid).FirstOrDefault();
                var user = _context.Users.Where(us => us.Email.ToUpper() == consutan.Email.ToUpper()).FirstOrDefault();

                var wallet = _context.Wallets.Where(a => a.CustomerId == customerid).FirstOrDefault();
                var slotbooking = await _context.SlotBookings.FindAsync(slotid);

                if(slotbooking.Status != "active" ) return StatusCode(409, new { StatusCode = 409, Message = "Slot đã được đặt hoặc không tồn tại. Vui Lòng chọn lại Slot khác!!" });
                if (slotbooking.Status == "livestream") return StatusCode(409, new { StatusCode = 409, Message = "Không thể đặt slot livestream được đâu!!" });
                if (wallet.Crab - slotbooking.Price >= 0 && slotbooking.Status == "active") {
                    DateTime dateslot = (DateTime)slotbooking.DateSlot;
                    string newString = dateslot.ToString("dd/MM/yyyy");
                    //slotbooking.Status = "booked";
                    // _context.SlotBookings.Update(slotbooking);
                    //  await _context.SaveChangesAsync();

                 
                    var payment = new Payment();
                    {


                        payment.Among = slotbooking.Price;
                        payment.Status = "active";
                        payment.CustomerId = customerid;

                    }
                    _context.Payments.Add(payment);                    
                    await _context.SaveChangesAsync();

                    var paymentnew = _context.Payments.Max(it => it.Id);

                  




                   

                    //var paymentnew =  _context.Payments.Where(a => a.Status == "active").Last();

              if(slotbooking.BookingId > 0)
                    {
                        return StatusCode(409, new { StatusCode = 409, Message = "Slot đã được đặt hoặc không tồn tại. Vui Lòng chọn lại Slot khác!!" });
                    }

             


                    var bok = new Booking();
                {
                    bok.Price = slotbooking.Price;
                    bok.Duration = "30";
                    bok.DateBooking = DateTime.Now.AddHours(7);
                    bok.PaymentId = paymentnew;
                    bok.ConsultantId = consultantid;
                    bok.CustomerId = customerid;
                    bok.Status = "booked";

                }
                    _context.Bookings.Add(bok);       
                await _context.SaveChangesAsync();


                    var bookingnew = _context.Bookings.Max(it => it.Id);

                 

                    slotbooking.Status = "booked";
                    slotbooking.BookingId = bookingnew;
                    _context.SlotBookings.Update(slotbooking);
                    await _context.SaveChangesAsync();

                    if (slotbooking.BookingId != bookingnew)
                    {
                        return StatusCode(409, new { StatusCode = 409, Message = "Slot đã được đặt hoặc không tồn tại. Vui Lòng chọn lại Slot khác!!" });
                    }



                    var trans = new Transaction();
                    {
                        trans.DateCreate = DateTime.Now.AddHours(7);
                        trans.PaymentId = paymentnew;
                        trans.Description = "-" + slotbooking.Price + " GEM Thanh toán cho cuộc gặp: " + slotbooking.TimeStart + " Ngày: " + newString;
                        trans.WalletId = wallet.Id;

                    }
                    _context.Transactions.Add(trans);
                    await _context.SaveChangesAsync();




                    string chanelname = "SLOT_" + slotid;
                    chanelname = chanelname.Replace(" ", "");
                    var token =  _agoraProvider.GenerateToken(chanelname, 0.ToString(), 0);

                    var roomvideocall = new RoomVideoCall();
                    {
                        roomvideocall.ChanelName = chanelname;
                        roomvideocall.SlotId = slotid;
                        roomvideocall.Token = token;
                    }
                    _context.RoomVideoCalls.Add(roomvideocall);
                    await _context.SaveChangesAsync();


                    var notifi = new Models.Notification();
                    {
                        notifi.ConsultantId = consultantid;
                        notifi.DateCreate = DateTime.Now.AddHours(7);
                        notifi.IsAdmin = false;
                        notifi.Type = "booked";
                        notifi.Status = "notseen";
                        notifi.Description = slotbooking.TimeStart + " Ngày: " + newString + " đã được đặt!";

                    }
                    _context.Notifications.Add(notifi);
                    await _context.SaveChangesAsync();



                    wallet.Crab = (int?)(wallet.Crab - slotbooking.Price);
                    wallet.HistoryTrans = DateTime.Now.AddHours(7);
                    wallet.CustomerId = customerid;
                    _context.Wallets.Update(wallet);
                    await _context.SaveChangesAsync();



                    // await _sendnoti.Sendmessagefcm("Slot đã được đặt thành công", slotbooking.TimeStart + " Ngày: " + dateslot.ToShortDateString() + " đã được đặt!", user.FcmToken);
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
                            Title = "Slot đã được đặt thành công",
                            Body = slotbooking.TimeStart + " Ngày: " + newString + " đã được đặt!"
                        }
                    };
                    await FirebaseMessaging.DefaultInstance.SendAsync(message);




                }
                else return StatusCode(409, new { StatusCode = 409, Message = "Tài khoản không đủ số dư để đặt!!" });

           

                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 201, Message = "Booking succesfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

      








        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var us = await _context.Bookings.FindAsync(id);
            if (us == null)
            {
                return NotFound();
            }
            if (us.Status != "inactive")
                us.Status = "inactive";
            else us.Status = us.Status;
            _context.Bookings.Update(us);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Content = "The Booking was deleted successfully!!" });
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }


     
    }
}
