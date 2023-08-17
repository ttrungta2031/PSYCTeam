using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using PsychologicalCounseling.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PsychologicalCounseling.Services
{
    public class JobTestService : IJobTestService
    {
        private readonly PsychologicalCouselingContext _context;
        public JobTestService(PsychologicalCouselingContext context)
        {
            //  _scopeFactory = scopeFactory.ServiceProvider.CreateScope().GetRequiredService<PsychologicalCouselingContext>(); ;

            _context = context;

        }
        public async Task AutoInactiveSlotActive()
        {
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

            var sort2 = result.Where(s => Formatday(s.DateSlot, s.TimeEnd) == false).ToList();
            if(result.Count() > 0) {
            for (int i = 0; i < sort2.Count(); i++)
            {
                var slotchange = await _context.SlotBookings.FindAsync(sort2[i].Id);
                if (slotchange != null)
                {
                    slotchange.Status = "inactive";
                    _context.SlotBookings.Update(slotchange);
                    _context.SaveChanges();
                    Console.WriteLine("Đã đổi status slot: " + slotchange.SlotId + " thành công");
                }
               
            }
            }
        }

        public async Task AutoOverDueSlotBooked()
        {
            var result = (from s in _context.SlotBookings
                          where s.Status == "booked"
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


            var sort2 = result.Where(s => Formatdayv2(s.DateSlot, s.TimeStart) == false).ToList();
            if (result.Count() > 0)
            {
                for (int i = 0; i < sort2.Count(); i++)
                {
                    var slotchange = await _context.SlotBookings.FindAsync(sort2[i].Id);
                    if (slotchange != null)
                    {
                        slotchange.Status = "overdue";
                        _context.SlotBookings.Update(slotchange);
                        _context.SaveChanges();
                        Console.WriteLine("Đã đổi status slot: " + slotchange.SlotId + " thành công");
                    }

                }
            }
        }


        public async Task AutoSendNotiCall()
        {
            var result = (from s in _context.SlotBookings
                          where s.Status == "booked"
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
            var sort2 = result.Where(s => Formatdayversendnoti(s.DateSlot, s.TimeStart) == true).ToList();

            if (sort2.Count() > 0)
            {
                for (int i = 0; i < sort2.Count(); i++)
                {
                    var slotbooking = await _context.SlotBookings.FindAsync(sort2[i].Id);
                    if (slotbooking != null)
                    {
                        var hassentnoti = "Cuộc hẹn: " + slotbooking.TimeStart;

                        var flag = _context.Notifications.Where(a => a.Description.Contains(hassentnoti)).FirstOrDefault();
                        if (flag == null)
                        {
                            var notifi = new Models.Notification();
                            {
                                notifi.ConsultantId = slotbooking.ConsultantId;
                                notifi.DateCreate = DateTime.Now.AddHours(7);
                                notifi.IsAdmin = false;
                                notifi.Type = "slotcoming";
                                notifi.Status = "notseen";
                                notifi.Description = "Cuộc hẹn: " + slotbooking.TimeStart + " sắp đến giờ!";

                            }

                            _context.Notifications.Add(notifi);
                            await _context.SaveChangesAsync();


                            var consutan = _context.Consultants.Where(us => us.Id == slotbooking.ConsultantId).FirstOrDefault();
                            var user = _context.Users.Where(us => us.Email.ToUpper() == consutan.Email.ToUpper()).FirstOrDefault();

                            if (FirebaseApp.DefaultInstance == null)
                            {
                                FirebaseApp.Create(new AppOptions()
                                {
                                    Credential = GoogleCredential.FromFile("private_key.json")
                                });
                            }
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
                                    Title = "Sắp đến giờ cuộc hẹn CallVideo",
                                    Body = "Cuộc hẹn: " + slotbooking.TimeStart + " sắp đến giờ! Mau chuẩn bị thôi!!"
                                }
                            };
                            await FirebaseMessaging.DefaultInstance.SendAsync(message);

                            Console.WriteLine(slotbooking.SlotId + " Sắp đến giờ");
                        }

                       

                    }

                }
            }



        }
        public void ReccuringJob()
        {
            Console.WriteLine("Hello from a Scheduled job!");
        }
        public void DelayedJob()
        {
            Console.WriteLine("Hello from a Delayed job!");
        }
        public void ContinuationJob()
        {
            Console.WriteLine("Hello from a Continuation job!");
        }



        private bool Formatday(DateTime? day, string time)
        {
            DateTime timenow = DateTime.UtcNow.AddHours(7);

            var time1 = day?.ToShortDateString();
            string timestart = time;
            var result = time1 + " " + timestart;
            //10/6/2022 09:00:00
            DateTime resultnew = DateTime.ParseExact(result, "M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            //  timestart   compare timenow
            //vd:    30/10/2022 15:00:00    compare     27/10/2022 4:30:00     true  nhận về slot active và appointment
         //   if(resultnew - resultnew)
            if (resultnew.CompareTo(timenow) == 1) return true;
            return false;

        }


        private bool Formatdayv2(DateTime? day, string time)
        {
            DateTime timenow = DateTime.UtcNow.AddHours(7);

            var time1 = day?.ToShortDateString();
            string timestart = time;
            var result = time1 + " " + timestart;
            //10/6/2022 09:00:00
            DateTime resultnew = DateTime.ParseExact(result, "M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            resultnew = resultnew.AddMinutes(40);
            //  timestart   compare timenow
            //vd:    30/10/2022 15:00:00    compare     27/10/2022 4:30:00     true  nhận về slot active và appointment
            if (resultnew.CompareTo(timenow) == 1) return true;
            return false;

        }

        private bool Formatdayversendnoti(DateTime? day, string time)
        {
            DateTime timenow = DateTime.UtcNow.AddHours(7);

            var time1 = day?.ToShortDateString();
            string timestart = time;
            var result = time1 + " " + timestart;
            //10/6/2022 09:00:00                            now: 10/6/2022 20:30
            DateTime resultnew = DateTime.ParseExact(result, "M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
           // resultnew = resultnew.AddMinutes(-15);
            //  timestart   compare timenow
            //vd:    30/10/2022 15:00:00    compare     27/10/2022 4:30:00     true  nhận về slot active và appointment
            if ((resultnew - timenow).TotalMinutes <= 15)
            {
                return true;
            }
            // if (resultnew.CompareTo(timenow) < 0) return true;
            return false;

        }
    }
}
