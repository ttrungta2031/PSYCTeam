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

    public class PlanetsController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public PlanetsController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/Planets
        [HttpGet("Getallplanets")]
        public IActionResult GetAllList(string search)
        {
            var result = (from s in _context.Planets
                          select new
                          {
                              Id = s.Id,

                              ImageUrl=s.ImageUrl,
            Name = s.Name,
            Tag =s.Tag,
            Element =s.Element,
            Description =s.Description,
            Maincontent =s.Maincontent


                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Planets
                          where s.Name.Contains(search)
                          select new
                          {
                              Id = s.Id,

                              ImageUrl = s.ImageUrl,
                              Name = s.Name,
                              Tag = s.Tag,
                              Element = s.Element,
                              Description = s.Description,
                              Maincontent = s.Maincontent


                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // GET: api/Planets/5
        [HttpGet("getbyid")]
        public async Task<ActionResult> GetPlanet(int id)
        {
            var all = _context.Planets.AsQueryable();

            all = _context.Planets.Where(us => us.Id.Equals(id));
            var result = all.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }


        // PUT: api/Planets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutPlanet(Models.Planet planet)
        {

            try
            {
                var pla = await _context.Planets.FindAsync(planet.Id);
                if (pla == null)
                {
                    return NotFound();
                }
                pla.ImageUrl = planet.ImageUrl == null ? pla.ImageUrl : planet.ImageUrl;
                pla.Name = planet.Name == null ? pla.Name : planet.Name;
                pla.Tag = planet.Tag == null ? pla.Tag : planet.Tag;
                pla.Element = planet.Element == null ? pla.Element : planet.Element;
                pla.Description = planet.Description == null ? pla.Description : planet.Description;
                pla.Maincontent = planet.Maincontent == null ? pla.Maincontent : planet.Maincontent;


                _context.Planets.Update(pla);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // POST: api/Planets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Planet>> PostPlanet(Planet planet)
        {
            try
            {
                var pla = new Planet();
                {
                    pla.ImageUrl = planet.ImageUrl;
                    pla.Name = planet.Name;
                    pla.Tag = planet.Tag;
                    pla.Element = planet.Element;
                    pla.Description = planet.Description;
                    pla.Maincontent = planet.Maincontent;

                }
                _context.Planets.Add(pla);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // DELETE: api/Planets/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeletePlanet(int id)
        {
            var pla = await _context.Planets.FindAsync(id);
            if (pla == null)
            {
                return NotFound();
            }

            _context.Planets.Remove(pla);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool PlanetExists(int id)
        {
            return _context.Planets.Any(e => e.Id == id);
        }
    }
}
