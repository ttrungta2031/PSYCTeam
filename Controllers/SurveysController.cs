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
    public class SurveysController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public SurveysController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/Surveys
        [HttpGet("getallsurvey")]
        public IActionResult GetAllList(string search)
        {
   //         string format = "dd/MM/yyyy";
            var result = (from s in _context.Surveys
                          where s.Status == "active" && s.SurveyTypeId >0
                          select new
                          {
                              Id = s.Id,
                               Name = s.Name,
                               SurveyTypeName = s.SurveyType.Name,
                              SurveyTypeId = s.SurveyTypeId


                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Surveys
                          where s.Name.Contains(search) && s.Status == "active"
                          select new
                          {
                              Id = s.Id,
                              Name = s.Name,
                              SurveyTypeName = s.SurveyType.Name,
                              SurveyTypeId = s.SurveyTypeId

                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }


        [HttpGet("getsurveybysurveytypeid")]
        public IActionResult GetSurveyBySurveyTypeId(int surveytypeid)
        {
            //         string format = "dd/MM/yyyy";
            var result = (from s in _context.Surveys
                          where s.Status == "active"
                          select new
                          {
                              Id = s.Id,
                              Name = s.Name,
                              SurveyTypeName = s.SurveyType.Name,
                              SurveyTypeId = s.SurveyTypeId


                          }).ToList();

            if (surveytypeid > 0)
            {
                result = (from s in _context.Surveys
                          where s.SurveyTypeId == surveytypeid &&  s.Status == "active"
                          select new
                          {
                              Id = s.Id,
                              Name = s.Name,
                              SurveyTypeName = s.SurveyType.Name,
                              SurveyTypeId = s.SurveyTypeId

                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }



        [HttpGet("getallsurveybyadmin")]
        public IActionResult GetAllListByAdmin(string search)
        {
            //         string format = "dd/MM/yyyy";
            var result = (from s in _context.Surveys

                          select new
                          {
                              Id = s.Id,
                              Name = s.Name,
                              SurveyTypeName = s.SurveyType.Name,
                              SurveyTypeId = s.SurveyTypeId,
                              Status = s.Status

                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Surveys
                          where s.Name.Contains(search)
                          select new
                          {
                              Id = s.Id,
                              Name = s.Name,
                              SurveyTypeName = s.SurveyType.Name,
                              SurveyTypeId = s.SurveyTypeId,
                              Status = s.Status
                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }


        [HttpGet("getsurveybysurveytypeidbyadmin")]
        public IActionResult GetSurveyBySurveyTypeIdByAdmin(int surveytypeid)
        {
            //         string format = "dd/MM/yyyy";
            var result = (from s in _context.Surveys
                          select new
                          {
                              Id = s.Id,
                              Name = s.Name,
                              SurveyTypeName = s.SurveyType.Name,
                              SurveyTypeId = s.SurveyTypeId,
                              Status = s.Status

                          }).ToList();

            if (surveytypeid > 0)
            {
                result = (from s in _context.Surveys
                          where s.SurveyTypeId == surveytypeid 
                          select new
                          {
                              Id = s.Id,
                              Name = s.Name,
                              SurveyTypeName = s.SurveyType.Name,
                              SurveyTypeId = s.SurveyTypeId,
                              Status = s.Status
                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }




        // GET: api/Surveys/5


        // PUT: api/Surveys/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]

        public async Task<IActionResult> PutSurvey(Survey sur)
        {

            try
            {
                var survey = await _context.Surveys.FindAsync(sur.Id);
                //   var consu = await _context.Consultants.FindAsync(con.Id);
                if (survey == null)
                {
                    return NotFound();
                }
                survey.Name = sur.Name == null ? survey.Name : sur.Name;
                survey.SurveyTypeId = sur.SurveyTypeId == null ?  survey.SurveyTypeId : sur.SurveyTypeId;
        //        survey.Status = sur.Status == null ? survey.Status : sur.Status;


                _context.Surveys.Update(survey);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // POST: api/Surveys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]

        public async Task<ActionResult<Survey>> PostSurvey(Survey survey)
        {
            try
            {
                var sur = new Survey();
                {
                    sur.Name = survey.Name;
                    sur.SurveyTypeId = survey.SurveyTypeId;
                    sur.Status = "active";
                }
                _context.Surveys.Add(sur);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }
        // DELETE: api/Surveys/5
        [HttpPut("inactive")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> InactiveSurvey(int id)
        {
            var us = await _context.Surveys.FindAsync(id);
            if (us == null)
            {
                return NotFound();
            }
            if (us.Status == "active") { 
                us.Status = "inactive";
                _context.Surveys.Update(us);
                await _context.SaveChangesAsync();
                return StatusCode(200, new { StatusCode = 200, Message = "The Survey was inactive successfully!!" });
            }

            if (us.Status == "inactive") { 
                us.Status = "active";
                _context.Surveys.Update(us);
                await _context.SaveChangesAsync();
                return StatusCode(200, new { StatusCode = 200, Message = "The Survey was active successfully!!" });
            }
           // _context.Surveys.Update(us);
           // await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Content = "The Survey was inactive/active successfully!!" });
        }
        private bool SurveyExists(int id)
        {
            return _context.Surveys.Any(e => e.Id == id);
        }
    }
}
