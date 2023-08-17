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

    public class PlanetHousesController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public PlanetHousesController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/PlanetHouses
        [HttpGet("Getallplanethouse")]
        public IActionResult GetAllList(string search)
        {
            var result = (from s in _context.PlanetHouses
                          select new
                          {
                              Id = s.Id,
                              Description = s.Description,
                              HouseId = s.HouseId,
                              PlanetId = s.PlanetId

                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.PlanetHouses
                          where s.Description.Contains(search)
                          select new
                          {
                              Id = s.Id,
                              Description = s.Description,
                              HouseId = s.HouseId,
                              PlanetId = s.PlanetId

                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }
        // GET: api/PlanetHouses/5
        [HttpGet("getbyid")]
        public async Task<ActionResult> GetPlanetHouse(int id)
        {
            var all = _context.PlanetHouses.AsQueryable();

            all = _context.PlanetHouses.Where(us => us.Id.Equals(id));
            var result = all.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }



        [HttpGet("getbyidplahou")]
        public async Task<ActionResult> GetHouseplanetv2(int houseid, int planetid)
        {
            var all = _context.PlanetHouses.AsQueryable();

            all = _context.PlanetHouses.Where(us => us.HouseId.Equals(houseid) && us.PlanetId.Equals(planetid));
            var result = all.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }
        // PUT: api/PlanetHouses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutPlanetHouse(Models.PlanetHouse planethouse)
        {

            try
            {
                var plahou = await _context.PlanetHouses.FindAsync(planethouse.Id);
                if (plahou == null)
                {
                    return NotFound();
                }
                plahou.Description = planethouse.Description == null ? plahou.Description : planethouse.Description;
                plahou.HouseId = planethouse.HouseId == null ? plahou.HouseId : planethouse.HouseId;
                plahou.PlanetId = planethouse.PlanetId == null ? plahou.PlanetId : planethouse.PlanetId;


                _context.PlanetHouses.Update(plahou);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // POST: api/PlanetHouses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]
        [Authorize(Roles = "admin")]

        public async Task<ActionResult<PlanetHouse>> PostPlanetHouse(PlanetHouse planethouse)
        {
            try
            {
                var plahou = new PlanetHouse();
                {
                    plahou.Description = planethouse.Description;
                    plahou.HouseId = planethouse.HouseId;
                    plahou.PlanetId = planethouse.PlanetId;


                }
                _context.PlanetHouses.Add(plahou);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // DELETE: api/PlanetHouses/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeletePlanetHouse(int id)
        {
            var plahou = await _context.PlanetHouses.FindAsync(id);
            if (plahou == null)
            {
                return NotFound();
            }

            _context.PlanetHouses.Remove(plahou);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool PlanetHouseExists(int id)
        {
            return _context.PlanetHouses.Any(e => e.Id == id);
        }
    }
}
