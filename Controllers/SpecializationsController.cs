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

    public class SpecializationsController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public SpecializationsController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/Specializations
      
        // GET: api/Specializations/5


        [HttpGet("getallspecial")]
        public IActionResult GetAllList(string search)
        {
            //         string format = "dd/MM/yyyy";
            var result = (from s in _context.Specializations
                          where s.SpecializationTypeId > 0
                          select new
                          {
                              Id = s.Id,
                              ConsultantName = s.Consultant.FullName,
                              SpecializationTypeName = s.SpecializationType.Name,
                              SpecializationTypeId = s.SpecializationTypeId


                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Specializations
                          where s.Consultant.FullName.Contains(search) && s.SpecializationTypeId > 0
                          select new
                          {
                              Id = s.Id,
                              ConsultantName = s.Consultant.FullName,
                              SpecializationTypeName = s.SpecializationType.Name,
                              SpecializationTypeId = s.SpecializationTypeId

                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }
        [HttpGet("getbyconsultantid")]
     
        public async Task<ActionResult> GetSpecialization(int id)
        {
            var result = (from s in _context.Specializations
                          where s.ConsultantId == id
                          select new
                          {
                              Id = s.Id,
                              Specname = s.SpecializationType.Name,
                              SpecializationTypeId =s.SpecializationTypeId

                          }).ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // PUT: api/Specializations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecialization(int id, Specialization specialization)
        {
            if (id != specialization.Id)
            {
                return BadRequest();
            }

            _context.Entry(specialization).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecializationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Specializations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        public class SpecializationBody
        {
            public int ConsultantId { get; set; }
            public List<int> SpecId { get; set; }
        }

        [HttpPost("updatebylist")]
   
        public async Task<ActionResult<Specialization>> PostSpecializationByList([FromBody] SpecializationBody option)
        {
            try
            {
                var result = (from s in _context.Specializations
                              where s.ConsultantId == option.ConsultantId
                              select new
                              {
                                  Id = s.Id
                              }).ToList();
                if(result.Count > 0) { 
                foreach(var item in result)
                {
                    var specialization = await _context.Specializations.FindAsync(item.Id);
                    if (specialization == null)
                    {
                        return StatusCode(409, new { StatusCode = 409, Message = "Không tìm thấy loại chuyên môn hoặc chọn sai định dạng!" });
                    }

                    _context.Specializations.Remove(specialization);
                    await _context.SaveChangesAsync();

                }
                }




                // var all = _context.Specializations.Where(us => us.ConsultantId.Equals(id));
                for (int i = 0; i < option.SpecId.Count; i++)
                {
                    var existspec = _context.Specializations.Where(a => a.ConsultantId == option.ConsultantId).Where(b => b.SpecializationTypeId == option.SpecId[i]).FirstOrDefault();
                    var existspecname = _context.SpecializationTypes.Where(b => b.Id == option.SpecId[i]).FirstOrDefault();
                    if(existspecname == null) { return StatusCode(409, new { StatusCode = 409, Message = "Không tìm thấy loại chuyên môn hoặc chọn sai định dạng!" }); }
                    if (existspec != null) { return StatusCode(409, new { StatusCode = 409, Message = "Đã tồn tại Loại chuyên môn:  {"+existspecname.Name+"} Vui lòng kiểm tra lại! " }); }

                }

                    foreach (var item in option.SpecId)
                {
                    var specia = new Specialization();
                    {
                        specia.ConsultantId = option.ConsultantId;
                        specia.SpecializationTypeId = item;
                    }
                    _context.Specializations.Add(specia);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }


       

        [HttpPost("create")]

        public async Task<ActionResult<Specialization>> PostSpecialization(int consuid, int specid)
        {
            try
            {

                // var all = _context.Specializations.Where(us => us.ConsultantId.Equals(id));

               
                    var specia = new Specialization();
                    {
                        specia.ConsultantId = consuid;
                        specia.SpecializationTypeId = specid;
               
                    _context.Specializations.Add(specia);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }
       

        // DELETE: api/Specializations/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteSpecialization(int id)
        {
            var specialization = await _context.Specializations.FindAsync(id);
            if (specialization == null)
            {
                return NotFound();
            }

            _context.Specializations.Remove(specialization);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SpecializationExists(int id)
        {
            return _context.Specializations.Any(e => e.Id == id);
        }
    }
}
