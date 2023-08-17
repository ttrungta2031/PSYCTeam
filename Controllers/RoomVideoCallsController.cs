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
    public class RoomVideoCallsController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public RoomVideoCallsController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/RoomVideoCalls
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomVideoCall>>> GetRoomVideoCalls()
        {
           /* string somestring = "Trần Trung Tá vừa tiến hành nạp 200 CUA Chờ xử lí! Mã giao dịch : 339115";
            string newstring = somestring.Substring(somestring.Length - 21, 21);*/

          //  Console.WriteLine(newstring);
            return await _context.RoomVideoCalls.ToListAsync();
        }

        // GET: api/RoomVideoCalls/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomVideoCall>> GetRoomVideoCall(int id)
        {
            var roomVideoCall = await _context.RoomVideoCalls.FindAsync(id);

            if (roomVideoCall == null)
            {
                return NotFound();
            }

            return roomVideoCall;
        }

        // PUT: api/RoomVideoCalls/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomVideoCall(int id, RoomVideoCall roomVideoCall)
        {
            if (id != roomVideoCall.Id)
            {
                return BadRequest();
            }

            _context.Entry(roomVideoCall).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomVideoCallExists(id))
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

        // POST: api/RoomVideoCalls
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoomVideoCall>> PostRoomVideoCall(RoomVideoCall roomVideoCall)
        {
            _context.RoomVideoCalls.Add(roomVideoCall);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoomVideoCall", new { id = roomVideoCall.Id }, roomVideoCall);
        }

        // DELETE: api/RoomVideoCalls/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteRoomVideoCall(int id)
        {
            var roomVideoCall = await _context.RoomVideoCalls.FindAsync(id);
            if (roomVideoCall == null)
            {
                return NotFound();
            }

            _context.RoomVideoCalls.Remove(roomVideoCall);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomVideoCallExists(int id)
        {
            return _context.RoomVideoCalls.Any(e => e.Id == id);
        }
    }
}
