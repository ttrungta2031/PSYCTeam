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
    public class HousesController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public HousesController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/Houses
        [HttpGet("Getallhouses")]

        public IActionResult GetAllList(string search)
        {
            var result = (from s in _context.Houses
                          select new
                          {
                              Id = s.Id,

                              ImageUrl=s.ImageUrl,
            Name =s.Name,
            Element =s.Element,
            Tag =s.Tag,
            Description =s.Description,
            Maincontent =s.Maincontent


                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Houses
                          where s.Name.Contains(search)
                          select new
                          {
                              Id = s.Id,

                              ImageUrl = s.ImageUrl,
                              Name = s.Name,
                              Element = s.Element,
                              Tag = s.Tag,
                              Description = s.Description,
                              Maincontent = s.Maincontent

                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }
        // GET: api/Houses/5
        [HttpGet("getbyid")]
    
        public async Task<ActionResult> GetHouse(int id)
        {
            var all = _context.Houses.AsQueryable();

            all = _context.Houses.Where(us => us.Id.Equals(id));
            var result = all.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // PUT: api/Houses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> PutHouse(Models.House house)
        {

            try
            {
                var ho = await _context.Houses.FindAsync(house.Id);
                if (ho == null)
                {
                    return NotFound();
                }
                ho.ImageUrl = house.ImageUrl == null ? ho.ImageUrl : house.ImageUrl;
                ho.Name = house.Name == null ? ho.Name : house.Name;
                ho.Element = house.Element == null ? ho.Element : house.Element;
                ho.Tag = house.Tag == null ? ho.Tag : house.Tag;
                ho.Description = house.Description == null ? ho.Description : house.Description;
                ho.Maincontent = house.Maincontent == null ? ho.Maincontent : house.Maincontent;


                _context.Houses.Update(ho);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // POST: api/Houses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]
        [Authorize]

        public async Task<ActionResult<House>> PostHouse(House house)
        {
            try
            {
                var ho = new House();
                {
                    ho.ImageUrl = house.ImageUrl;
                    ho.Name = house.Name;
                    ho.Element = house.Element;
                    ho.Tag = house.Tag;
                    ho.Description = house.Description;
                    ho.Maincontent = house.Maincontent;

                }
                _context.Houses.Add(ho);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }


        // DELETE: api/Houses/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteHouse(int id)
        {
            var house = await _context.Houses.FindAsync(id);
            if (house == null)
            {
                return NotFound();
            }

            _context.Houses.Remove(house);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HouseExists(int id)
        {
            return _context.Houses.Any(e => e.Id == id);
        }
    }
}
