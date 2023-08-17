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
    public class ArticlesController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public ArticlesController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/Articles
        [HttpGet("GetallArticles")]
        public IActionResult GetAllList(string search)
        {
            string format = "dd/MM/yyyy";
            var result = (from s in _context.Articles
                          select new
                          {
                              Id = s.Id,
                              UrlBanner = s.UrlBanner,
                              Title =s.Title,
            Description =s.Description,
            CreateDay =s.CreateDay,
            ContentNews =s.ContentNews,
            Status =s.Status,


                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Articles
                          where s.Title.Contains(search)
                          select new
                          {
                              Id = s.Id,
                              UrlBanner = s.UrlBanner,
                              Title = s.Title,
                              Description = s.Description,
                              CreateDay = s.CreateDay,
                              ContentNews = s.ContentNews,
                              Status = s.Status,

                          }).ToList();
            }
       
            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }


        [HttpGet("getarticlescustomer")]
        public IActionResult GetArticlesCustomer(string search)
        {
            string format = "dd/MM/yyyy";
            var result = (from s in _context.Articles
                          where s.Status == "active"
                          select new
                          {
                              Id = s.Id,
                              UrlBanner = s.UrlBanner,
                              Title = s.Title,
                              Description = s.Description,
                              CreateDay = s.CreateDay,
                              ContentNews = s.ContentNews,
                              Status = s.Status,


                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Articles
                          where s.Title.Contains(search) && s.Status == "active"
                          select new
                          {
                              Id = s.Id,
                              UrlBanner = s.UrlBanner,
                              Title = s.Title,
                              Description = s.Description,
                              CreateDay = s.CreateDay,
                              ContentNews = s.ContentNews,
                              Status = s.Status,

                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // GET: api/Articles/5
        [HttpGet("getbyid")]
        public async Task<ActionResult> GetArticle(int id)
        {
            var all = _context.Articles.AsQueryable();

            all = _context.Articles.Where(us => us.Id.Equals(id));
            var result = all.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // PUT: api/Articles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]
        public async Task<IActionResult> PutArticle(Models.Article article)
        {

            try
            {
                var art = await _context.Articles.FindAsync(article.Id);
                if (art == null)
                {
                    return NotFound();
                }

                art.Title = article.Title == null ? art.Title : article.Title;
                art.UrlBanner = article.UrlBanner == null ? art.UrlBanner : article.UrlBanner;
                art.Description = article.Description == null ? art.Description : article.Description;
                art.CreateDay = article.CreateDay == null ? DateTime.Now.AddHours(7) : article.CreateDay;
                art.ContentNews = article.ContentNews == null ? art.ContentNews : article.ContentNews;
                art.Status = article.Status == null ? art.Status : article.Status;


                _context.Articles.Update(art);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }


        // POST: api/Articles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]

        public async Task<ActionResult<Zodiac>> PostArticle(Article article)
        {
            try
            {
                var art = new Article();
                {
                    art.Title = article.Title;
                    art.UrlBanner = article.UrlBanner;
                    art.Description = article.Description;
                    art.CreateDay = DateTime.Now.AddHours(7);
                    art.ContentNews = article.ContentNews;
                    art.Status = "inactive";
                }
                _context.Articles.Add(art);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteẢticle(int id)
        {
            var us = await _context.Articles.FindAsync(id);
            if (us == null)
            {
                return NotFound();
            }
            if (us.Status == "active")
                us.Status = "inactive";
            else
                us.Status = "active";

            _context.Articles.Update(us);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Content = "The Article was inactive successfully!!" });
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}
