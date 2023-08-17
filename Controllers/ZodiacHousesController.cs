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

    public class ZodiacHousesController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public ZodiacHousesController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/ZodiacHouses
        [HttpGet("Getallzodiachouse")]
        public IActionResult GetAllList(string search)
        {
            var result = (from s in _context.ZodiacHouses
                          select new
                          {
                              Id = s.Id,
                              Description= s.Description,
                              ZodiacId = s.ZodiacId,
                              HouseId =s.HouseId

                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.ZodiacHouses
                          where s.Description.Contains(search)
                          select new
                          {
                              Id = s.Id,
                              Description = s.Description,
                              ZodiacId = s.ZodiacId,
                              HouseId = s.HouseId

                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // GET: api/ZodiacHouses/5
        [HttpGet("getbyid")]
        public async Task<ActionResult> GetZodiacHouse(int id)
        {
            var all = _context.ZodiacHouses.AsQueryable();

            all = _context.ZodiacHouses.Where(us => us.Id.Equals(id));
            var result = all.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }


        [HttpGet("getbyidzoho")]
        public async Task<ActionResult> GetZodiacHousev2(int zodiacid, int houseid)
        {
            var all = _context.ZodiacHouses.AsQueryable();

            all = _context.ZodiacHouses.Where(us => us.ZodiacId.Equals(zodiacid) && us.HouseId.Equals(houseid));
            var result = all.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // PUT: api/ZodiacHouses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutZodiacHouse(Models.ZodiacHouse zodiachouse)
        {

            try
            {
                var zohou = await _context.ZodiacHouses.FindAsync(zodiachouse.Id);
                if (zohou == null)
                {
                    return NotFound();
                }
                zohou.Description = zodiachouse.Description == null ? zohou.Description : zodiachouse.Description;
                zohou.ZodiacId = zodiachouse.ZodiacId == null ? zohou.ZodiacId : zodiachouse.ZodiacId;
                zohou.HouseId = zodiachouse.HouseId == null ? zohou.HouseId : zodiachouse.HouseId;


                _context.ZodiacHouses.Update(zohou);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // POST: api/ZodiacHouses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ZodiacHouse>> PostZodiacHouse(ZodiacHouse zodiachouse)
        {
            try
            {
                var zohou = new ZodiacHouse();
                {
                    zohou.Description = zodiachouse.Description;
                    zohou.ZodiacId = zodiachouse.ZodiacId;
                    zohou.HouseId = zodiachouse.HouseId;


                }
                _context.ZodiacHouses.Add(zohou);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // DELETE: api/ZodiacHouses/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteZodiacHouse(int id)
        {
            var zohou = await _context.ZodiacHouses.FindAsync(id);
            if (zohou == null)
            {
                return NotFound();
            }

            _context.ZodiacHouses.Remove(zohou);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ZodiacHouseExists(int id)
        {
            return _context.ZodiacHouses.Any(e => e.Id == id);
        }
    }
}
