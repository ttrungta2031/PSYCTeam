using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PsychologicalCounseling.Models;
using System.IO;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Packaging;
using PsychologicalCounseling.Services;

namespace PsychologicalCounseling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
 
    public class ZodiacsController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;
        private readonly IDrawnatalchart _drawnatal;
        public ZodiacsController(PsychologicalCouselingContext context, IDrawnatalchart drawnatal)
        {
            _context = context;
            _drawnatal = drawnatal;
        }

        // GET: api/Zodiacs
        [HttpGet("Getchart")]
        public async Task<IActionResult> GetChartAsync(DateTime date, double longtitude, double latitude)
        {
            try
            {
                /* var file = this._astrology.GetChartStream(date, longtitude, latitude);*/

                var fileName = _drawnatal.GetChartLinkFirebase(date, longtitude, latitude);


             /*   var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                string link = await _drawnatal.UploadChart(file, fileName.Split('\\').Last());
                file.Close();
                System.IO.File.Delete(fileName);
*/
                return Ok(fileName);
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.ToLower().Contains("not found"))
                    return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }




        [HttpGet("Getallzodiacs")]
        [Authorize(Roles ="admin")]
        public IActionResult GetAllList(string search)
        {
          
            var result = (from s in _context.Zodiacs
                          select new
                          {
                              Id = s.Id,
                              ImageUrl = s.ImageUrl,
                              Name = s.Name,
                              DateStart = s.DateStart,
                              DateEnd = s.DateEnd,
                              DescriptionShort = s.DescriptionShort,
                              DescriptionDetail = s.DescriptionDetail

                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Zodiacs
                          where s.Name.Contains(search)
                          select new
                          {
                              Id = s.Id,
                              ImageUrl = s.ImageUrl,
                              Name = s.Name,
                              DateStart = s.DateStart,
                              DateEnd = s.DateEnd,
                              DescriptionShort = s.DescriptionShort,
                              DescriptionDetail = s.DescriptionDetail

                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }
        // GET: api/Zodiacs/5
        [HttpGet("getbyid")]
        public async Task<ActionResult> GetZodiac(int id)
        {
          //  var all = _context.Zodiacs.AsQueryable();
            var zodi = await _context.Zodiacs.FindAsync(id);
            // all = _context.Zodiacs.Where(us => us.Id.Equals(id));
            DateTime ds = (DateTime)zodi.DateStart == null ? DateTime.Now : (DateTime)zodi.DateStart;
            DateTime de = (DateTime)zodi.DateEnd == null ? DateTime.Now : (DateTime)zodi.DateEnd;
            

            var result = (from s in _context.Zodiacs
                          where s.Id == id
                          select new
                          {
                              Id = s.Id,
                              ImageUrl = s.ImageUrl,
                              Name = s.Name,
                              DayStart = ds.Day,
                              MonthStart = ds.Month,
                              DayEnd = de.Day,
                              MonthEnd = de.Month,
                              DescriptionShort = s.DescriptionShort,
                              DescriptionDetail = s.DescriptionDetail

                          }).ToList();

            //  var result = all.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // PUT: api/Zodiacs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]
        public async Task<IActionResult> PutZodiac(Models.Zodiac zodiac)
        {

            try
            {
                var zodi = await _context.Zodiacs.FindAsync(zodiac.Id);
                if (zodi == null)
                {
                    return NotFound();
                }
                zodi.ImageUrl = zodiac.ImageUrl == null ? zodi.ImageUrl : zodiac.ImageUrl;
                zodi.Name = zodiac.Name == null ? zodi.Name : zodiac.Name;
                zodi.DateStart = zodiac.DateStart == null ? zodi.DateStart : zodiac.DateStart;
                zodi.DateEnd = zodiac.DateEnd == null ? zodi.DateEnd : zodiac.DateEnd;
                zodi.DescriptionShort = zodiac.DescriptionShort == null ? zodi.DescriptionShort : zodiac.DescriptionShort;
                zodi.DescriptionDetail = zodiac.DescriptionDetail == null ? zodi.DescriptionDetail : zodiac.DescriptionDetail;


                _context.Zodiacs.Update(zodi);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }


        [HttpPut("updatedocs")]
        public async Task<IActionResult> PutZodiacByDoc(int id, IFormFile file)
        {

            try
            {
                var zodi = await _context.Zodiacs.FindAsync(id);
                if (zodi == null)
                {
                    return NotFound();
                }               
                if (file != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        var word = WordprocessingDocument.Open(stream, true);
                        var paragraph = word.MainDocumentPart.Document.Body;

                        zodi.DescriptionDetail = paragraph.InnerText;
                        _context.Zodiacs.Update(zodi);
                        await _context.SaveChangesAsync();
                    }


                }


                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }












        // POST: api/Zodiacs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]

        public async Task<ActionResult<Zodiac>> PostZodiac(Zodiac zodiac)
        {
            try
            {
                var zodi = new Zodiac();
                {
                    zodi.ImageUrl = zodiac.ImageUrl;
                    zodi.Name = zodiac.Name;
                    zodi.DateStart = zodiac.DateStart;
                    zodi.DateEnd = zodiac.DateEnd;
                    zodi.DescriptionShort = zodiac.DescriptionShort;
                    zodi.DescriptionDetail = zodiac.DescriptionDetail;
                }
                _context.Zodiacs.Add(zodi);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // DELETE: api/Zodiacs/5
        [HttpDelete]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteZodiac(int id)
        {
            var zodiac = await _context.Zodiacs.FindAsync(id);
            if (zodiac == null)
            {
                return NotFound();
            }

            _context.Zodiacs.Remove(zodiac);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ZodiacExists(int id)
        {
            return _context.Zodiacs.Any(e => e.Id == id);
        }




        [HttpPost("CreateExcel")]
        [Authorize(Roles = "admin")]
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
                            try { 
                           var zodiac = (new Zodiac
                            {
                               ImageUrl = worksheet.Cells[row, 2].Value == null ? string.Empty : worksheet.Cells[row, 2].Value.ToString().Trim(),
                               Name = worksheet.Cells[row, 3].Value == null ? string.Empty : worksheet.Cells[row, 3].Value.ToString().Trim(),
                               DateStart = (DateTime)worksheet.Cells[row, 4].Value,
                               DateEnd = (DateTime)worksheet.Cells[row, 5].Value,
                               DescriptionShort = worksheet.Cells[row, 6].Value == null ? string.Empty : worksheet.Cells[row, 6].Value.ToString().Trim(),
                               DescriptionDetail = worksheet.Cells[row, 7].Value == null ? string.Empty : worksheet.Cells[row, 7].Value.ToString().Trim(),


                               
                           });


                            _context.Zodiacs.Add(zodiac);
                                await _context.SaveChangesAsync();
                            }
                            catch(Exception e)
                            {
                                return StatusCode(500, new { StatusCode = 500, Message = "Insert data failed at row: "+row + e});
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



















    }
}
