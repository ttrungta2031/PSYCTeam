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
    public class OptionQuestionsController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public OptionQuestionsController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/OptionQuestions
        [HttpGet("Getoptionquestion")]
        //      [Authorize(Roles ="admin")]
        public IActionResult GetAllList(int surveyid)
        {
            var result = (from s in _context.OptionQuestions

                          select new
                          {
                              Id = s.Id,
                              OptionText = s.OptionText,
                              Type = s.Type,
                              Question = s.Question.Description,
                              QuestionId = s.QuestionId


                          }).ToList();

            if (surveyid > 0)
            {
                result = (from s in _context.OptionQuestions
                          where s.Question.SurveyId == surveyid
                          select new
                          {
                              Id = s.Id,
                              OptionText = s.OptionText,
                              Type = s.Type,
                              Question = s.Question.Description,
                              QuestionId = s.QuestionId
                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // GET: api/OptionQuestions/5


        [HttpGet("getoptionbysurveyid")]
        //      [Authorize(Roles ="admin")]
        public IActionResult GetOptionBySurveyId(int id, int pagesize =5, int pagenumber = 1)
        {
           if(id<=0 ) return StatusCode(404, new { StatusCode = 404, Message = "Not Found!" });
            var result = (from s in _context.OptionQuestions
                          where s.Question.SurveyId == id
                          select new
                          {
                              Id = s.Id,
                              Question = s.Question.Description,
                               OptionText = s.OptionText,
                                Type = s.Type,
                              Questionid = s.QuestionId
                          }).ToList();

       
            var count = result.Count();
            var totalpage = count / pagesize;
            var sort = result.OrderBy(x => x.Id).ToList();
            var paging = sort.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = paging, total = count, totalpage = totalpage});
        }

        [HttpGet("getoptionbyquestionid")]
        //      [Authorize(Roles ="admin")]
        public IActionResult GetOptionByQuestionId(int questionid)
        {
            var result = (from s in _context.OptionQuestions

                          select new
                          {
                              Id = s.Id,
                              OptionText = s.OptionText,
                              Type = s.Type,
                              Question = s.Question.Description,
                              QuestionId = s.QuestionId
                          }).ToList();

            if (  questionid > 0)
            {
                result = (from s in _context.OptionQuestions
                          where s.QuestionId == questionid
                          select new
                          {
                              Id = s.Id,
                              OptionText = s.OptionText,
                              Type = s.Type,
                              Question = s.Question.Description,
                              QuestionId = s.QuestionId
                          }).ToList();
            }
            var count = result.Count();       
            var sort = result.OrderBy(x => x.Id).ToList();
          

            return Ok(new { StatusCode = 200, Message = "Load successful", data = sort, total = count });
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<OptionQuestion>> GetOptionQuestion(int id)
        {
            var optionQuestion = await _context.OptionQuestions.FindAsync(id);

            if (optionQuestion == null)
            {
                return NotFound();
            }

            return optionQuestion;
        }

        // PUT: api/OptionQuestions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]

        public async Task<IActionResult> PutOptionQuestion(OptionQuestion opt)
        {

            try
            {
                var option = await _context.OptionQuestions.FindAsync(opt.Id);
                //   var consu = await _context.Consultants.FindAsync(con.Id);
                if (option == null)
                {
                    return NotFound();
                }
                option.OptionText = opt.OptionText == null ? option.OptionText : opt.OptionText;
                option.Type = opt.Type == null ? option.Type : opt.Type;
                option.QuestionId = opt.QuestionId == null ? option.QuestionId : opt.QuestionId;



                _context.OptionQuestions.Update(option);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // POST: api/OptionQuestions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]

        public async Task<ActionResult<OptionQuestion>> PostOptionQuestion(OptionQuestion question)
        {
            try
            {
                var ques = new OptionQuestion();
                {
                    ques.OptionText = question.OptionText;
                    ques.Type = question.Type;
                    ques.QuestionId = question.QuestionId;
                }
                _context.OptionQuestions.Add(ques);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // DELETE: api/OptionQuestions/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteOptionQuestion(int id)
        {
            var optionQuestion = await _context.OptionQuestions.FindAsync(id);
            if (optionQuestion == null)
            {
                return NotFound();
            }

            _context.OptionQuestions.Remove(optionQuestion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OptionQuestionExists(int id)
        {
            return _context.OptionQuestions.Any(e => e.Id == id);
        }
    }
}
