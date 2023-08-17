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

    public class WalletsController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public WalletsController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/Wallets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wallet>>> GetWallets()
        {
            return await _context.Wallets.ToListAsync();
        }

        // GET: api/Wallets/5
     

        [HttpGet("getbycustomerid")]
        public async Task<ActionResult> GetWalletByCustomerid(int id)
        {
            var result = (from s in _context.Wallets
                          where s.CustomerId == id 
                          select new
                          {
                              Name = s.Customer.Fullname,
                              Gem = s.Crab
                          }).ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        [HttpGet("getbyconsultantid")]
        public async Task<ActionResult> GetWalletByConsultantid(int id)
        {
            var result = (from s in _context.Wallets
                          where s.ConsultantId == id
                          select new
                          {
                              Name = s.Consultant.FullName,
                              Gem = s.Crab
                          }).ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }
        // PUT: api/Wallets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        public class WalletInfo
        {

            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
            public int? ConsultantId { get; set; }
        }

        public class WalletInfo1
        {        
            public string NewPassword { get; set; }
            public int? ConsultantId { get; set; }
        }

        [HttpPut("createpasswordwallet")]
        public async Task<IActionResult> CreatePassWordWallet([FromBody] WalletInfo1 walletinfo)
        {
            var wallet = _context.Wallets.Where(a => a.ConsultantId == walletinfo.ConsultantId).FirstOrDefault();
            if (wallet == null || walletinfo.ConsultantId < 0)
            {
                return NotFound();
            }
            if (String.IsNullOrEmpty(wallet.PassWord))
            {
                if(walletinfo.NewPassword.Any(ch => Char.IsLetter(ch) || !Char.IsLetterOrDigit(ch)) || walletinfo.NewPassword.Length >6) return StatusCode(400, new { StatusCode = 400, Message = "Mật khẩu phải có 6 ký tự là chữ số!" });
                wallet.PassWord = walletinfo.NewPassword.ToLower();
                _context.Wallets.Update(wallet);
                await _context.SaveChangesAsync();
            }
            else return StatusCode(400, new { StatusCode = 400, Message = "Thông tin Ví đã có Password không thể tạo mới!" });

            return Ok(new { StatusCode = 200, Content = "The Password of Wallet was create successfully!!" });
        }



        [HttpPut("changepasswallet")]
        public async Task<IActionResult> ChangePassWordWallet([FromBody] WalletInfo walletinfo)
        {
            var wallet = _context.Wallets.Where(a => a.ConsultantId == walletinfo.ConsultantId).FirstOrDefault();
            if (wallet == null || walletinfo.ConsultantId < 0)
            {
                return NotFound();
            }
            if(wallet.PassWord == walletinfo.OldPassword)
            {
                if (walletinfo.NewPassword.Any(ch => Char.IsLetter(ch) || !Char.IsLetterOrDigit(ch)) || walletinfo.NewPassword.Length > 6) return StatusCode(400, new { StatusCode = 400, Message = "Mật khẩu phải có 6 ký tự là chữ số!" });

                wallet.PassWord = walletinfo.NewPassword.ToLower();
                _context.Wallets.Update(wallet);
                await _context.SaveChangesAsync();
            }
            else return StatusCode(400, new { StatusCode = 400, Message = "Sai thông tin định dạng!" });

            return Ok(new { StatusCode = 200, Content = "The Password of Waleet was changed successfully!!" });
        }


        [HttpPut("forgotpasswallet")]
        public async Task<IActionResult> ForgotPassWordWallet([FromBody] WalletInfo walletinfo)
        {

            if(walletinfo.ConsultantId < 1) return NotFound();
            var wallet = _context.Wallets.Where(a => a.ConsultantId == walletinfo.ConsultantId).FirstOrDefault();
            var consu = _context.Consultants.Where(a => a.Id == walletinfo.ConsultantId).FirstOrDefault();
            var user = _context.Users.Where(a => a.Email == consu.Email).FirstOrDefault();
            if (wallet == null || walletinfo.ConsultantId < 1)
            {
                return NotFound();
            }
            if (user.PassWord == walletinfo.OldPassword)
            {
                if (walletinfo.NewPassword.Any(ch => Char.IsLetter(ch) || !Char.IsLetterOrDigit(ch)) || walletinfo.NewPassword.Length > 6) return StatusCode(400, new { StatusCode = 400, Message = "Mật khẩu phải có 6 ký tự là chữ số!" });
                wallet.PassWord = walletinfo.NewPassword.ToLower();
                _context.Wallets.Update(wallet);
                await _context.SaveChangesAsync();
            }
            else return StatusCode(400, new { StatusCode = 400, Message = "Sai thông tin định dạng!" });

            return Ok(new { StatusCode = 200, Content = "The Password of Waleet was changed successfully!!" });
        }

        // POST: api/Wallets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Wallet>> PostWallet(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (WalletExists(wallet.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetWallet", new { id = wallet.Id }, wallet);
        }

        // DELETE: api/Wallets/5
       /* [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null)
            {
                return NotFound();
            }

            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();

            return NoContent();
        }
*/
        private bool WalletExists(int id)
        {
            return _context.Wallets.Any(e => e.Id == id);
        }
    }
}
