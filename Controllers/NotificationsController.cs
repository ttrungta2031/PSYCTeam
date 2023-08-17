using System;
using System.Collections.Generic;
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
    public class NotificationsController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public NotificationsController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/Notifications
        [HttpGet("getnotificationbycustomer")]
        public IActionResult GetNotiByCustomer(int id)
        {
            //  string format = "dd/MM/yyyy";
            DateTime timenow = DateTime.UtcNow.AddHours(7);
           
            var result = (from s in _context.Notifications
                          where s.CustomerId == id && s.Status == "notseen" || s.CustomerId == id && s.Status == "seen"
                          select new
                          {
                              Id = s.Id,
                              DateCreate=s.DateCreate,
                              Description=s.Description,
                              Type = s.Type,
                              Status = s.Status,
                          }).ToList();



            var sort = result.OrderByDescending(x => x.Id).ToList();
            return Ok(new { StatusCode = 200, Message = "Load successful", data = sort });
        }




        [HttpGet("getnotificationbyconsultant")]
        public IActionResult GetNotiByConsultant(int id)
        {
            //  string format = "dd/MM/yyyy";
          //  var hasconsu = _context.DeviceTokens.SingleOrDefault(p => p.ConsultantId == id);
         //   DateTime timenow = DateTime.UtcNow.AddHours(7);
            var result = (from s in _context.Notifications
                          where s.ConsultantId == id && s.Status == "notseen"  || s.ConsultantId == id && s.Status == "seen" 
                          select new
                          {
                              Id = s.Id,
                              DateCreate = s.DateCreate,
                              Description = s.Description,
                              Type = s.Type,
                              Status = s.Status,
                          }).ToList();

           
            var sort = result.OrderByDescending(x => x.Id).ToList();
            return Ok(new { StatusCode = 200, Message = "Load successful", data = sort });
        }


        [HttpGet("countnotibyconsultant")]
        public IActionResult CountNotiByConsultant(int id)
        {
            //  string format = "dd/MM/yyyy";
            var hasconsu = _context.DeviceTokens.SingleOrDefault(p => p.ConsultantId == id);          
            var chuongnoti = (from s in _context.Notifications
                              where s.ConsultantId == id && s.Status == "notseen" && s.DateCreate > hasconsu.Datechange || s.ConsultantId == id && s.Status == "seen" && s.DateCreate > hasconsu.Datechange
                              select new
                              {
                                  Id = s.Id,
                                  DateCreate = s.DateCreate,
                                  Description = s.Description,
                                  Type = s.Type,
                                  Status = s.Status,
                              }).ToList();
            var count = chuongnoti.Count();

            return Ok(new { StatusCode = 200, Message = "Load successful", amount = count });
        }






        [HttpGet("getnotificationbyadmin")]
        public IActionResult GetNotiByAdmin()
        {
            //  string format = "dd/MM/yyyy";
            var result = (from s in _context.Notifications
                          where s.IsAdmin == true && s.Status =="notseen" || s.IsAdmin == true && s.Status == "seen"
                          select new
                          {
                              Id = s.Id,
                              DateCreate = s.DateCreate,
                              Description = s.Description,
                              Type = s.Type,
                              Status = s.Status,
                          }).ToList();


            var sort = result.OrderByDescending(x => x.Id).ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = sort });
        }

        // GET: api/Notifications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);

            if (notification == null)
            {
                return NotFound();
            }

            return notification;
        }

        // PUT: api/Notifications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotification(int id, Notification notification)
        {
            if (id != notification.Id)
            {
                return BadRequest();
            }

            _context.Entry(notification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationExists(id))
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

        // POST: api/Notifications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("refreshnotibyconsultant")]
        public async Task<IActionResult> RefreshNotiByConsu(int id)
        {
            //  string format = "dd/MM/yyyy";
            var hasconsu = _context.DeviceTokens.SingleOrDefault(p => p.ConsultantId == id);
            if (hasconsu == null)
            {
                var consu = new DeviceToken();
                {
                    consu.ConsultantId = id;
                    consu.Datechange = DateTime.UtcNow.AddHours(7);
                }
                _context.DeviceTokens.Add(consu);
                await _context.SaveChangesAsync();
            }
            else
            {
                hasconsu.Datechange = DateTime.UtcNow.AddHours(7);
                _context.DeviceTokens.Update(hasconsu);
                await _context.SaveChangesAsync();
            }

            return Ok(new { StatusCode = 200, Message = "Refresh Notification successful" });
        }
        [HttpPost]
        public async Task<ActionResult<Notification>> PostNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (NotificationExists(notification.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetNotification", new { id = notification.Id }, notification);
        }

        // DELETE: api/Notifications/5
        [HttpPut("seennoti")]
        public async Task<IActionResult> SeenNotifi(int id)
        {
            var us = await _context.Notifications.FindAsync(id);
            if (us == null || id <=0)
            {
                return NotFound();
            }
            if (us.Status == "notseen")
                us.Status = "seen";
            else us.Status = us.Status;
            _context.Notifications.Update(us);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Content = "The Notification was seen successfully!!" });
        }

        [HttpPut("seenallnotibyadmin")]
        public async Task<IActionResult> SeenAllNotiByAdmin()
        {
            var result = (from s in _context.Notifications
                          where s.IsAdmin == true && s.Status == "notseen"
                          select new
                          {
                              Id = s.Id,
                              DateCreate = s.DateCreate,
                              Description = s.Description,
                              Type = s.Type,
                              Status = s.Status,
                          }).ToList();
            var sort = result.OrderByDescending(x => x.Id).ToList();
            if (result.Count() > 0)
            {
                for (int i = 0; i < sort.Count(); i++)
                {
                    var notifi = await _context.Notifications.FindAsync(sort[i].Id);
                    if (notifi != null)
                    {
                        notifi.Status = "seen";
                        _context.Notifications.Update(notifi);
                        _context.SaveChanges();
                        Console.WriteLine("Đã đổi status slot: " + notifi.Id + " thành công");
                    }

                }
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = sort });
        }


        [HttpPut("hiddennoti")]
        public async Task<IActionResult> HiddenNoti(int id)
        {
            var us = await _context.Notifications.FindAsync(id);
            if (us == null || id <=0)
            {
                return NotFound();
            }
            if (us.Status == "notseen" || us.Status =="seen")
                us.Status = "hidden";
            else us.Status = us.Status;
            _context.Notifications.Update(us);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Content = "The Notification was hidden successfully!!" });
        }



        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
          /*  var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
*/
            return NoContent();
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}
