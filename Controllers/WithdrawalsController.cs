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
    public class WithdrawalsController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;

        public WithdrawalsController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        // GET: api/Withdrawals
        [HttpGet("Getallwithdraw")]

        public IActionResult GetAllWithdraw( string search , string date, int walletid, int pagesize = 10, int pagenumber = 1)
        {

            DateTime day = DateTime.Now.AddHours(7);
            var result = (from s in _context.Withdrawals

                          select new
                          {
                              Id = s.Id,
                              ConsultantName = s.Wallet.Consultant.FullName,
                              AccountName = s.FullName,
                              BankName = s.BankName,
                              BankNumber = s.BankNumber,
                              Description = s.Description,
                              DateCreate = s.DateCreate,
                              RequestAmount = s.RequestAmount,
                              ActualWithdrawal = s.ActualWithdrawal,
                              WalletId = s.WalletId,
                              Status = s.Status,



                          }).ToList();
            if (string.IsNullOrEmpty(date) && walletid > 0)
            {
                result = (from s in _context.Withdrawals
                          where s.WalletId == walletid
                          select new
                          {
                              Id = s.Id,
                              ConsultantName = s.Wallet.Consultant.FullName,
                              AccountName = s.FullName,
                              BankName = s.BankName,
                              BankNumber = s.BankNumber,
                              Description = s.Description,
                              DateCreate = s.DateCreate,
                              RequestAmount = s.RequestAmount,
                              ActualWithdrawal = s.ActualWithdrawal,
                              WalletId =s.WalletId,
                              Status = s.Status,

                          }).ToList();



                //  return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
            }           
            else if (!string.IsNullOrEmpty(date) && walletid > 0)
            {
                var datenew = DateTime.Parse(date).ToShortDateString();
                var sort2 = result.Where(s => s.WalletId == walletid && s.DateCreate.GetValueOrDefault().ToShortDateString().Contains(datenew));
                var sort5 = sort2.OrderByDescending(x => x.Id).ToList();
                var paging2 = sort5.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
                double totalpage3 = (double)sort5.Count() / pagesize;
                totalpage3 = Math.Ceiling(totalpage3);
                return Ok(new { StatusCode = 200, Message = "Load successful", data = paging2 , totalpage = totalpage3});

            }
            else if (!string.IsNullOrEmpty(date))
            {
                var datenew = DateTime.Parse(date).ToShortDateString();
                var sort3 = result.Where(s => s.DateCreate.GetValueOrDefault().ToShortDateString().Contains(datenew));
                var sort6 = sort3.OrderByDescending(x => x.Id).ToList();
                var paging3 = sort6.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
                double totalpage2 = (double)sort6.Count() / pagesize;
                totalpage2 = Math.Ceiling(totalpage2);
                return Ok(new { StatusCode = 200, Message = "Load successful", data = paging3 , totalpage = totalpage2});


            }
            else if (!string.IsNullOrEmpty(search))
            {
               // var datenew = DateTime.Parse(date).ToShortDateString();
                var sort3 = result.Where(s => s.ConsultantName.Contains(search) || s.AccountName.Contains(search) || s.BankNumber.Contains(search));
                var sort6 = sort3.OrderByDescending(x => x.Id).ToList();
                var paging3 = sort6.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
                double totalpage2 = (double)sort6.Count() / pagesize;
                totalpage2 = Math.Ceiling(totalpage2);
                return Ok(new { StatusCode = 200, Message = "Load successful", data = paging3, totalpage = totalpage2 });


            }
            else if (!string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(search))
            {
                var datenew = DateTime.Parse(date).ToShortDateString();
                var sort3 = result.Where(s => s.DateCreate.GetValueOrDefault().ToShortDateString().Contains(datenew) && s.ConsultantName.Contains(search) || s.DateCreate.GetValueOrDefault().ToShortDateString().Contains(datenew) && s.AccountName.Contains(search) || s.DateCreate.GetValueOrDefault().ToShortDateString().Contains(datenew) && s.BankNumber.Contains(search) );
                var sort6 = sort3.OrderByDescending(x => x.Id).ToList();
                var paging3 = sort6.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
                double totalpage2 = (double)sort6.Count() / pagesize;
                totalpage2 = Math.Ceiling(totalpage2);
                return Ok(new { StatusCode = 200, Message = "Load successful", data = paging3, totalpage = totalpage2 });


            }
            //!string.IsNullOrEmpty(search)
            /*foreach (var item in result.ToList())
            {
                if (Formatday(item.TimeStart))
                {
                    result.Remove(item);
                }

            }*/

            var sort = result.OrderByDescending(x => x.Id).ToList();
            var paging = sort.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
            double totalpage1 = (double)sort.Count() / pagesize;
            totalpage1 = Math.Ceiling(totalpage1);
            return Ok(new { StatusCode = 200, Message = "Load successful", data = paging , totalpage = totalpage1});
            // return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // GET: api/Withdrawals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Withdrawal>> GetWithdrawal(int id)
        {
            var withdrawal = await _context.Withdrawals.FindAsync(id);

            if (withdrawal == null)
            {
                return NotFound();
            }

            return withdrawal;
        }

        // PUT: api/Withdrawals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWithdrawal(int id, Withdrawal withdrawal)
        {
            if (id != withdrawal.Id)
            {
                return BadRequest();
            }

            _context.Entry(withdrawal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WithdrawalExists(id))
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
        public class DetailWithdrawal
        {
            public string Name { get; set; }
            public string Month { get; set; }
            public double? Total { get; set; }
        }

        [HttpGet("ReportWithdrawalByYear")]
        public async Task<ActionResult<Withdrawal>> DetailWithdraw(int year = 2022)
        {
            List<DetailWithdrawal> detail = new List<DetailWithdrawal>();
            for (int month = 1; month <= 12; month++)
            {

                var firstDayOfMonth = new DateTime(year, month, 1);
                var lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                double total = 0;
                var resultwithdrawal = (from s in _context.Withdrawals
                                        where s.DateCreate >= firstDayOfMonth && s.DateCreate <= lastDayOfMonth && s.Status == "success"
                                        select new
                                     {
                                         Id = s.Id,
                                         DateCreate = s.DateCreate,
                                         ActualWithdrawal = s.ActualWithdrawal
                                     }).ToList();
                foreach (var item in resultwithdrawal)
                {
                    total = (double)(total + item.ActualWithdrawal);
                }

                DetailWithdrawal withdraw = new DetailWithdrawal();
                {
                    withdraw.Month = "Tháng " + month.ToString();
                    withdraw.Name = "Rút";
                    withdraw.Total = total;
                }
                detail.Add(withdraw);
            }
            var resultnew = detail.ToList();

            return Ok(new { StatusCode = 200, Content = "Load successful", Data = resultnew });

        }


        [HttpGet("historywithdrawal")]
        public async Task<ActionResult<Deposit>> HistoryWithDrawalByConsultantId(int consultantid, int pagesize = 10, int pagenumber = 1)
        {
            var wallet = _context.Wallets.Where(a => a.ConsultantId == consultantid).FirstOrDefault();

            if (wallet == null)
            {
                return NotFound();
            }
            var result = (from s in _context.Withdrawals
                          where s.WalletId == wallet.Id
                          select new
                          {
                              Id = s.Id,         
                              AccountName = s.FullName,
                              BankName = s.BankName,
                              BankNumber = s.BankNumber,
                              Description = s.Description,
                              DateCreate = s.DateCreate,
                              RequestAmount = s.RequestAmount,
                              ActualWithdrawal = s.ActualWithdrawal,
                              Status = s.Status,
                          }).ToList();
            var sort = result.OrderByDescending(x => x.Id).ToList();
            var paging = sort.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
            double totalpage1 = (double)sort.Count() / pagesize;
            totalpage1 = Math.Ceiling(totalpage1);
            return Ok(new { StatusCode = 200, Content = "Load successful", Data = paging , totalpage = totalpage1});
        }
        public class WithdrawalInfo
        {
            public string FullName { get; set; }
            public string BankName { get; set; }
            public string BankNumber { get; set; }
            public int? Amount { get; set; }
            public int? ConsultantId { get; set; }
            public string Password { get; set; }
        }
        // POST: api/Withdrawals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("withdraw")]
        public async Task<ActionResult<Withdrawal>> PostWithdrawal([FromBody] WithdrawalInfo withdrawinfo)
        {
            int gasfee = 1;
            if(withdrawinfo.Amount < 0 || string.IsNullOrEmpty(withdrawinfo.BankName) || string.IsNullOrEmpty(withdrawinfo.BankNumber) || string.IsNullOrEmpty(withdrawinfo.FullName))
            {
                return StatusCode(400, new { StatusCode = 400, Message = "Thông tin bị thiếu hoặc số tiền rút từ 100 trở lên!" });
            }    
              var wallet = _context.Wallets.Where(a => a.ConsultantId == withdrawinfo.ConsultantId).FirstOrDefault();
              var consultant = _context.Consultants.Where(a => a.Id == wallet.ConsultantId).FirstOrDefault();
              if (wallet == null) { return StatusCode(404, new { StatusCode = 404, Message = "Không tìm thấy Ví!!" }); }
              if (consultant == null) { return StatusCode(404, new { StatusCode = 404, Message = "Không tìm thấy Ví!!" }); }
            if (wallet.Crab < withdrawinfo.Amount)
            {
                return StatusCode(409, new { StatusCode = 409, Message = "Tài khoản không đủ số dư!" });

            }
         //  DateTime daynow = DateTime.UtcNow.AddHours(7);
          //  var timenow = daynow.ToShortDateString();

            if (wallet.Crab >= withdrawinfo.Amount && wallet.PassWord == withdrawinfo.Password.ToLower()) {

                var withdraw = new Withdrawal();
                {
                    withdraw.DateCreate = DateTime.UtcNow.AddHours(7);
                    withdraw.FullName = consultant.FullName;
                    withdraw.BankName = withdrawinfo.BankName;
                    withdraw.BankNumber = withdrawinfo.BankNumber;
                    withdraw.RequestAmount = withdrawinfo.Amount;
                    withdraw.ActualWithdrawal = withdrawinfo.Amount;
                    withdraw.WalletId = wallet.Id;
                    withdraw.Status = "waiting";

                }
                _context.Withdrawals.Add(withdraw);
                await _context.SaveChangesAsync();

                var notifi = new Notification();
                {
                    //  notifi.ConsultantId = booking.CustomerId;
                    notifi.DateCreate = DateTime.UtcNow.AddHours(7);
                    notifi.IsAdmin = true;
                    notifi.Type = "withdraw";
                    notifi.Status = "notseen";
                    notifi.Description = consultant.FullName + " vừa tiến hành rút " + withdrawinfo.Amount + " GEM" + " Chờ xử lí!";

                }
                _context.Notifications.Add(notifi);
                await _context.SaveChangesAsync();

                wallet.Crab = (int?)(wallet.Crab - withdrawinfo.Amount);
                wallet.HistoryTrans = DateTime.Now.AddHours(7);
                _context.Wallets.Update(wallet);
                await _context.SaveChangesAsync();



                return Ok(new { StatusCode = 200, Message = "Withdraw Successful, Waiting for approve" });
            }


            return StatusCode(409, new { StatusCode = 409, Message = "Tài khoản không đủ số dư hoặc đang bị hạn chế!" });

        }


        [HttpPut("acceptwithdraw")]
        public async Task<IActionResult> AcceptWithdraw(int id)
        {
            var us = await _context.Withdrawals.FindAsync(id);
            var wallet = _context.Wallets.Where(a => a.Id == us.WalletId).FirstOrDefault();        
            if (us == null)
            {
                return NotFound();
            }
            if (us.Status == "waiting")
            {
               us.Status = "success";

            }
            else us.Status = us.Status;
            _context.Withdrawals.Update(us);
            await _context.SaveChangesAsync();


            var notifi = new Models.Notification();
            {
                notifi.ConsultantId = wallet.ConsultantId;
                notifi.DateCreate = DateTime.Now.AddHours(7);
                notifi.Type = "withdraw";
                notifi.IsAdmin = false;
                notifi.Status = "notseen";
                notifi.Description = "Đơn rút tiền " + us.RequestAmount + " GEM được chấp nhận!";

            }
            _context.Notifications.Add(notifi);      
            await _context.SaveChangesAsync();



            return Ok(new { StatusCode = 200, Content = "The Withdrawal was accepted successfully!!" });
        }

        [HttpPut("rejectwithdraw")]
        public async Task<IActionResult> RejectWithdraw(int id, string description)
        {
            var us = await _context.Withdrawals.FindAsync(id);
            var wallet = _context.Wallets.Where(a => a.Id == us.WalletId).FirstOrDefault();    
            if (us == null)
            {
                return NotFound();
            }
            if (us.Status == "waiting")
            {

                wallet.Crab = wallet.Crab + us.RequestAmount;
                wallet.HistoryTrans = DateTime.Now.AddHours(7);

                us.Status = "fail";
                us.Description = description;
                
            }
            else us.Status = us.Status;

            //    _context.Wallets.Update(wallet);
            _context.Wallets.Update(wallet);
            _context.Withdrawals.Update(us);
            await _context.SaveChangesAsync();


            var notifi = new Models.Notification();
            {
                notifi.ConsultantId = wallet.ConsultantId;
                notifi.DateCreate = DateTime.Now.AddHours(7);
                notifi.Type = "withdraw";
                notifi.IsAdmin = false;
                notifi.Status = "notseen";
                notifi.Description = "Đơn rút tiền " + us.RequestAmount + " GEM đã bị từ chối!";

            }
            _context.Notifications.Add(notifi);
            await _context.SaveChangesAsync();



            return Ok(new { StatusCode = 200, Content = "The Withdrawal was rejected successfully!!" });
        }


        // DELETE: api/Withdrawals/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteWithdrawal(int id)
        {
 

            return NoContent();
        }

        private bool WithdrawalExists(int id)
        {
            return _context.Withdrawals.Any(e => e.Id == id);
        }
    }
}
