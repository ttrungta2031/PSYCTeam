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

    public class PaymentsController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public PaymentsController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/Payments
        [HttpGet("test")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            /*var result = (from s in _context.SlotBookings
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

            var sort2 = result.Where(s => Formatday(s.DateSlot, s.TimeEnd) == false);*/
            return StatusCode(200, new { StatusCode = 200, message = "Email re-send"});
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

        // GET: api/Payments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null)
            {
                return NotFound();
            }

            return payment;
        }

        // PUT: api/Payments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPayment(int id, Payment payment)
        {
            if (id != payment.Id)
            {
                return BadRequest();
            }

            _context.Entry(payment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(id))
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

        // POST: api/Payments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Payment>> PostPayment(Payment payment)
        {
            _context.Payments.Add(payment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PaymentExists(payment.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPayment", new { id = payment.Id }, payment);
        }

        // DELETE: api/Payments/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
    }
}
