using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PsychologicalCounseling.Models;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace PsychologicalCounseling.Services
{
    public class AutoService : IHostedService, IDisposable
    {
    
        private Timer? _timer = null;
        private readonly PsychologicalCouselingContext _context;
        public AutoService(IServiceScopeFactory scopeFactory)
        {
          //  _scopeFactory = scopeFactory.ServiceProvider.CreateScope().GetRequiredService<PsychologicalCouselingContext>(); ;
        
            _context = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<PsychologicalCouselingContext>();

        }


        public  Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Task đang chạy!");
            //_timer =  new Timer(AutoChangeStatusSlot, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            //  _timer = new Timer(Test_Create_Noti, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }



        public async void AutoChangeStatusSlot(object? state)
        {
           // var abc = _context.SurveyTypes.Asy();
            var result =  (from s in _context.SlotBookings
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
            for(int i = 0; i < sort2.Count(); i++)
            {
                var slotchange = await _context.SlotBookings.FindAsync(sort2[i].Id);
                if (slotchange != null)
                {
                    Console.WriteLine("Đã đổi status slot: " + slotchange.SlotId + " thành công");
                }
               
                
            }                  
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
            if (resultnew.CompareTo(timenow) == 1) return true;
            return false;



          


        }
        public void  Test_Create_Noti(object? state)
        {

            var notifi = new Models.Notification();
            {

                notifi.DateCreate = DateTime.Now.AddHours(7);
                notifi.IsAdmin = false;
                notifi.Type = "test";
                notifi.Status = "notseen";
                notifi.Description = "test";

            }
            _context.Notifications.Add(notifi);
            _context.SaveChanges();


            Console.WriteLine("Đã tạo noti thành công");



        }
        //Service StopAsync when project shutdown
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Task kết thúc chạy!");
            _timer?.Change(Timeout.Infinite, 0);
           
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

       
    }
}
