using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PsychologicalCounseling.Models;

namespace PsychologicalCounseling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyHoroscopesController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public DailyHoroscopesController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/DailyHoroscopes
        [HttpGet("Getalldailyhoroscopes")]
        public IActionResult GetAllList(int id, string date)
        {
            var result = (from s in _context.DailyHoroscopes
                          select new
                          {
                              Id = s.Id,                                        
                              ImageUrl = s.ImageUrl,
                              Date=s.Date,
                              Context =s.Context,
                              Job =s.Job,
                              Affection =s.Affection,
                              LuckyNumber =s.LuckyNumber,
                              GoodTime =s.GoodTime,
                              Color =s.Color,
                              ShouldThing =s.ShouldThing,
                              ShouldNotThing =s.ShouldNotThing,
                              ZodiacId =s.ZodiacId


                          }).ToList();

            if (!string.IsNullOrEmpty(date))
            {
                result = (from s in _context.DailyHoroscopes
                          where s.Zodiac.Id.Equals(id) && s.Date.ToString().Contains(date)
                          select new
                          {
                              Id = s.Id,
                              ImageUrl = s.ImageUrl,
                              Date = s.Date,
                              Context = s.Context,
                              Job = s.Job,
                              Affection = s.Affection,
                              LuckyNumber = s.LuckyNumber,
                              GoodTime = s.GoodTime,
                              Color = s.Color,
                              ShouldThing = s.ShouldThing,
                              ShouldNotThing = s.ShouldNotThing,
                              ZodiacId = s.ZodiacId
                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // GET: api/DailyHoroscopes/5
        [HttpGet("getbyid")]

        public async Task<ActionResult> GetDailyHoroscopes(int id)
        {
            var all = _context.DailyHoroscopes.AsQueryable();

            all = _context.DailyHoroscopes.Where(us => us.Id.Equals(id));
            var result = all.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // PUT: api/DailyHoroscopes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]
        public async Task<IActionResult> PutDailyhoroscope(DailyHoroscope dailyhoroscope)
        {

            try
            {
             
                var daily = await _context.DailyHoroscopes.FindAsync(dailyhoroscope.Id);
                if (daily == null)
                {
                    return NotFound();
                }
                daily.ImageUrl = dailyhoroscope.ImageUrl == null ? daily.ImageUrl : dailyhoroscope.ImageUrl; 
                daily.Date = DateTime.Now.AddHours(7);
                daily.Context = dailyhoroscope.Context == null ? daily.Context : dailyhoroscope.Context; 
                daily.Job = dailyhoroscope.Job == null ? daily.Job : dailyhoroscope.Job; 
                daily.Affection = dailyhoroscope.Affection == null ? daily.Affection : dailyhoroscope.Affection; 
                daily.LuckyNumber = dailyhoroscope.LuckyNumber == null ? daily.LuckyNumber : dailyhoroscope.LuckyNumber; 
                daily.GoodTime = dailyhoroscope.GoodTime == null ? daily.GoodTime : dailyhoroscope.GoodTime; 
                daily.Color = dailyhoroscope.Color == null ? daily.Color : dailyhoroscope.Color; 
                daily.ShouldThing = dailyhoroscope.ShouldThing == null ? daily.ShouldThing : dailyhoroscope.ShouldThing;
                daily.ShouldNotThing = dailyhoroscope.ShouldNotThing == null ? daily.ShouldNotThing : dailyhoroscope.ShouldNotThing; 
                daily.ZodiacId = dailyhoroscope.ZodiacId == null ? daily.ZodiacId : dailyhoroscope.ZodiacId; 


                _context.DailyHoroscopes.Update(daily);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // POST: api/DailyHoroscopes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]


        public async Task<ActionResult<DailyHoroscope>> PostDailyHoroscope(DailyHoroscope dailyhoroscope)
        {
            try
            {
                var daily = new DailyHoroscope();
                {
                    daily.ImageUrl = dailyhoroscope.ImageUrl;
                    daily.Date = DateTime.Now.AddHours(7);
                    daily.Context = dailyhoroscope.Context;
                    daily.Job = dailyhoroscope.Job;
                    daily.Affection = dailyhoroscope.Affection;
                    daily.LuckyNumber = dailyhoroscope.LuckyNumber;
                    daily.GoodTime = dailyhoroscope.GoodTime;
                    daily.Color = dailyhoroscope.Color;
                    daily.ShouldThing = dailyhoroscope.ShouldThing;
                    daily.ShouldNotThing = dailyhoroscope.ShouldNotThing;
                    daily.ZodiacId = dailyhoroscope.ZodiacId;
                }
                _context.DailyHoroscopes.Add(daily);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }


        [HttpPost("CreateExcel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowcount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowcount; row++)
                        {
                            try
                            {
                                var daily = (new DailyHoroscope
                                {

                                    ImageUrl = worksheet.Cells[row, 2].Value == null ? string.Empty : worksheet.Cells[row, 2].Value.ToString().Trim(),
                                    Date = (DateTime)worksheet.Cells[row, 3].Value,
                                    Context = worksheet.Cells[row, 4].Value == null ? string.Empty : worksheet.Cells[row, 4].Value.ToString().Trim(),
                                    Job = worksheet.Cells[row, 5].Value == null ? string.Empty : worksheet.Cells[row, 5].Value.ToString().Trim(),
                                    Affection = worksheet.Cells[row, 6].Value == null ? string.Empty : worksheet.Cells[row, 6].Value.ToString().Trim(),
                                    LuckyNumber = (int?)(double)worksheet.Cells[row, 7].Value,
                                    GoodTime = worksheet.Cells[row, 8].Value == null ? string.Empty : worksheet.Cells[row, 8].Value.ToString().Trim(),
                                    Color = worksheet.Cells[row, 9].Value == null ? string.Empty : worksheet.Cells[row, 9].Value.ToString().Trim(),
                                    ShouldThing = worksheet.Cells[row, 10].Value == null ? string.Empty : worksheet.Cells[row, 10].Value.ToString().Trim(),
                                    ShouldNotThing = worksheet.Cells[row, 11].Value == null ? string.Empty : worksheet.Cells[row, 11].Value.ToString().Trim(),
                                    ZodiacId = (int?)(double)worksheet.Cells[row, 12].Value,


                                });


                                _context.DailyHoroscopes.Add(daily);
                                await _context.SaveChangesAsync();
                            }
                            catch (Exception e)
                            {
                                return StatusCode(500, new { StatusCode = 500, Message = "Insert data failed at row: " + row + e });
                            }

                        }


                    }
                }

                return StatusCode(201, new { StatusCode = 200, Message = "Insert data successful" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Insert data failed, internal errors. Example: input with file not .xlsx" + e });
            }


        }

        // DELETE: api/DailyHoroscopes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteDailyHoroscope(int id)
        {
            var dailyHoroscope = await _context.DailyHoroscopes.FindAsync(id);
            if (dailyHoroscope == null)
            {
                return NotFound();
            }

            _context.DailyHoroscopes.Remove(dailyHoroscope);
            await _context.SaveChangesAsync();

            return NoContent();
        }


       


        private bool DailyHoroscopeExists(int id)
        {
            return _context.DailyHoroscopes.Any(e => e.Id == id);
        }
    }
}
