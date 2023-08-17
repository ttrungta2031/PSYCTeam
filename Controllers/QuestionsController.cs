using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PsychologicalCounseling.Models;

namespace PsychologicalCounseling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public QuestionsController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/Questions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestions()
        {
            return await _context.Questions.ToListAsync();
        }

        // GET: api/Questions/5
        [HttpGet("getquestionbysurveyid")]
        //      [Authorize(Roles ="admin")]
        public IActionResult GetQuestionBySurveyId(int surveyid)
        {
            if(surveyid <= 0) return StatusCode(404,new { StatusCode = 404, Message = "NotFound" });
            var result = (from s in _context.Questions
                          where s.SurveyId == surveyid
                          select new
                          {
                              Id = s.Id,
                              Description = s.Description ,
                              Option = s.OptionQuestions
                          }).ToList();
     
            var count = result.Count();
            var sort = result.OrderBy(x => x.Id).ToList();


            return Ok(new { StatusCode = 200, Message = "Load successful", data = sort, total = count });
        }

        [HttpGet("getquestionbyid")]
        //      [Authorize(Roles ="admin")]
        public IActionResult GetQuestionById(int id)
        {
            var result = (from s in _context.Questions

                          select new
                          {
                              Id = s.Id,
                              Description = s.Description,
                              Option = s.OptionQuestions
                          }).ToList();

            if (id > 0)
            {
                result = (from s in _context.Questions
                          where s.Id == id
                          select new
                          {
                              Id = s.Id,
                              Description = s.Description,
                              Option = s.OptionQuestions
                          }).ToList();
            }
            var count = result.Count();
            var sort = result.OrderBy(x => x.Id).ToList();


            return Ok(new { StatusCode = 200, Message = "Load successful", data = sort, total = count });
        }

        // PUT: api/Questions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]

        public async Task<IActionResult> PutQuestion(Question que)
        {

            try
            {
                var question = await _context.Questions.FindAsync(que.Id);
                //   var consu = await _context.Consultants.FindAsync(con.Id);
                if (question == null)
                {
                    return NotFound();
                }
                question.Description = que.Description == null ? question.Description : que.Description;
                question.SurveyId = que.SurveyId == null ? question.SurveyId : que.SurveyId;


                _context.Questions.Update(question);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // POST: api/Questions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]

        public async Task<ActionResult<Question>> PostQuestion(Question question)
        {
            try
            {
                var ques = new Question();
                {
                    ques.Description = question.Description;
                    ques.SurveyId = question.SurveyId;
                }
                _context.Questions.Add(ques);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // DELETE: api/Questions/5
        [HttpDelete("removefromsurvey")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoveFromSurvey(int id)
        {
            var us = await _context.Questions.FindAsync(id);
            if (us == null)
            {
                return NotFound();
            }
            if (us.SurveyId > 0)
                us.SurveyId = null;




            _context.Questions.Update(us);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Content = "The Survey was inactive/active successfully!!" });
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
    }
}
