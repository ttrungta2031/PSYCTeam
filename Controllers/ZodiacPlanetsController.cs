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

    public class ZodiacPlanetsController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public ZodiacPlanetsController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/ZodiacPlanets
        [HttpGet("Getallzodiacplanet")]
        public IActionResult GetAllList(string search)
        {
            var result = (from s in _context.ZodiacPlanets
                          select new
                          {
                              Id = s.Id,
                              Description = s.Description,
                              ZodiacId = s.ZodiacId,
                              PlanetId = s.PlanetId

                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.ZodiacPlanets
                          where s.Description.Contains(search)
                          select new
                          {
                              Id = s.Id,
                              Description = s.Description,
                              ZodiacId = s.ZodiacId,
                              PlanetId = s.PlanetId

                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // GET: api/ZodiacPlanets/5
        [HttpGet("getbyid")]
        public async Task<ActionResult> GetZodiacPlanet(int id)
        {
            var all = _context.ZodiacPlanets.AsQueryable();

            all = _context.ZodiacPlanets.Where(us => us.Id.Equals(id));
            var result = all.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }


        [HttpGet("getbyidzopla")]
        public async Task<ActionResult> GetZodiacplanetv2(int zodiacid, int planetid)
        {
            var all = _context.ZodiacPlanets.AsQueryable();

            all = _context.ZodiacPlanets.Where(us => us.ZodiacId.Equals(zodiacid) && us.PlanetId.Equals(planetid));
            var result = all.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }
        // PUT: api/ZodiacPlanets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutZodiacPlanet(Models.ZodiacPlanet zodiacplanet)
        {

            try
            {
                var zopla = await _context.ZodiacPlanets.FindAsync(zodiacplanet.Id);
                if (zopla == null)
                {
                    return NotFound();
                }
                zopla.Description = zodiacplanet.Description == null ? zopla.Description : zodiacplanet.Description;
                zopla.ZodiacId = zodiacplanet.ZodiacId == null ? zopla.ZodiacId : zodiacplanet.ZodiacId;
                zopla.PlanetId = zodiacplanet.PlanetId == null ? zopla.PlanetId : zodiacplanet.PlanetId;


                _context.ZodiacPlanets.Update(zopla);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // POST: api/ZodiacPlanets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ZodiacHouse>> PostZodiacPlanet(ZodiacPlanet zodiacplanet)
        {
            try
            {
                var zopla = new ZodiacPlanet();
                {
                    zopla.Description = zodiacplanet.Description;
                    zopla.ZodiacId = zodiacplanet.ZodiacId;
                    zopla.PlanetId = zodiacplanet.PlanetId;


                }
                _context.ZodiacPlanets.Add(zopla);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // DELETE: api/ZodiacPlanets/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteZodiacPlanet(int id)
        {
            var zopla = await _context.ZodiacPlanets.FindAsync(id);
            if (zopla == null)
            {
                return NotFound();
            }

            _context.ZodiacPlanets.Remove(zopla);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool ZodiacPlanetExists(int id)
        {
            return _context.ZodiacPlanets.Any(e => e.Id == id);
        }
    }
}
