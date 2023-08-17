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
    public class SurveyTypesController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public SurveyTypesController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/SurveyTypes
        [HttpGet("getallsurveytype")]
        public IActionResult GetAllList(string search)
        {
            //         string format = "dd/MM/yyyy";
            var result = (from s in _context.SurveyTypes
                          select new
                          {
                              Id = s.Id,
                              Name = s.Name,
                              Surveys = s.Surveys
                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.SurveyTypes
                          where s.Name.Contains(search)
                          select new
                          {
                              Id = s.Id,
                              Name = s.Name,
                              Surveys = s.Surveys

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

        // GET: api/SurveyTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyType>> GetSurveyType(int id)
        {
            var surveyType = await _context.SurveyTypes.FindAsync(id);

            if (surveyType == null)
            {
                return NotFound();
            }

            return surveyType;
        }

        // PUT: api/SurveyTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]

        public async Task<IActionResult> PutSurveyType(SurveyType sur)
        {

            try
            {
                var surveytype = await _context.SurveyTypes.FindAsync(sur.Id);
                //   var consu = await _context.Consultants.FindAsync(con.Id);
                if (surveytype == null)
                {
                    return NotFound();
                }
                surveytype.Name = sur.Name == null ? surveytype.Name : sur.Name;



                _context.SurveyTypes.Update(surveytype);
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

        public async Task<ActionResult<SurveyType>> PostSurveyType(SurveyType survey)
        {
            try
            {
                var sur = new SurveyType();
                {
                    sur.Name = survey.Name;
                }
                _context.SurveyTypes.Add(sur);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // DELETE: api/SurveyTypes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteSurveyType(int id)
        {
            var surveyType = await _context.SurveyTypes.FindAsync(id);
            if (surveyType == null)
            {
                return NotFound();
            }

            _context.SurveyTypes.Remove(surveyType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SurveyTypeExists(int id)
        {
            return _context.SurveyTypes.Any(e => e.Id == id);
        }
    }
}
