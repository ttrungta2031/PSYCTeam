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
 
    public class SpecializationTypesController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public SpecializationTypesController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/SpecializationTypes
        [HttpGet("getallspecype")]
        public IActionResult GetAllList(string search)
        {
            //         string format = "dd/MM/yyyy";
            var result = (from s in _context.SpecializationTypes
                          select new
                          {
                              Id = s.Id,
                              Name = s.Name,
                              Specializations = s.Specializations
                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.SpecializationTypes
                          where s.Name.Contains(search)
                          select new
                          {
                              Id = s.Id,
                              Name = s.Name,
                              Specializations = s.Specializations

                          }).ToList();
            }
            /*            var count = 0;
                        foreach(var item in result)
                        {
                            if (item.Surveys != null) count = count + 1;
                        }

                        var newresult = result.Where(s => ).ToList();*/



            /*    var survey = (from s in _context.Surveys
                              where s.SurveyTypeId == surveytypeid
                              select new
                              {
                                  Id = s.Id,
                                  Name = s.Name,
                                  SurveyTypeName = s.SurveyType.Name,
                                  SurveyTypeId = s.SurveyTypeId

                              }).ToList();*/
            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // GET: api/SpecializationTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecializationType>> GetSpecializationType(int id)
        {
            var specializationType = await _context.SpecializationTypes.FindAsync(id);

            if (specializationType == null)
            {
                return NotFound();
            }

            return specializationType;
        }

        // PUT: api/SpecializationTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutSpecializationType(SpecializationType spec)
        {

            try
            {
                var surveytype = await _context.SpecializationTypes.FindAsync(spec.Id);
                //   var consu = await _context.Consultants.FindAsync(con.Id);
                if (surveytype == null)
                {
                    return NotFound();
                }
                surveytype.Name = spec.Name == null ? surveytype.Name : spec.Name;



                _context.SpecializationTypes.Update(surveytype);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // POST: api/SurveyTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<SpecializationType>> PostSpecializationType(SpecializationType spec)
        {
            try
            {
                var sur = new SpecializationType();
                {
                    sur.Name = spec.Name;
                }
                _context.SpecializationTypes.Add(sur);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }
        // DELETE: api/SpecializationTypes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteSpecializationType(int id)
        {
            var specializationType = await _context.SpecializationTypes.FindAsync(id);
            if (specializationType == null)
            {
                return NotFound();
            }

            _context.SpecializationTypes.Remove(specializationType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SpecializationTypeExists(int id)
        {
            return _context.SpecializationTypes.Any(e => e.Id == id);
        }
    }
}
