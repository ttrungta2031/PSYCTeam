
using Microsoft.AspNetCore.Mvc;
using PsychologicalCounseling.Models;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;

using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PsychologicalCounseling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportDashboardController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;
      
        public ReportDashboardController(PsychologicalCouselingContext context)
        {
            _context = context;
        }




        [HttpGet("gettotaldashboard")]

        public IActionResult GetTopConsultantByBooking()
        {
            DateTime timenow = DateTime.UtcNow.AddHours(7);
            var daynow = timenow.ToShortDateString();
            var dayyesterday = timenow.AddDays(-1).ToShortDateString();
            var consultant = (from s in _context.Consultants
                              select new
                              {
                                  Id = s.Id,
                              }).ToList();
            var customer = (from s in _context.Customers
                            select new
                            {
                                Id = s.Id,
                            }).ToList();

            var booking = (from s in _context.SlotBookings
                           where s.Status == "success"
                           select new
                           {
                               Id = s.SlotId,
                               Price = s.Price
                           }).ToList();

            var deposit = (from s in _context.Deposits
                           where s.Status == "success"
                           select new
                           {
                               Id = s.Id,
                               DateCreate = s.DateCreate,
                               Amount = s.Amount
                           }).ToList();
            var withdrawal = (from s in _context.Withdrawals
                              where s.Status == "success"
                              select new
                              {
                                  Id = s.Id,
                                  DateCreate = s.DateCreate,
                                  Amount = s.ActualWithdrawal
                              }).ToList();


            double totaldeposit = 0;
            double totalwithdrawal = 0;
            double totaldepositbyday = 0;
            double totalwithdrawalbyday = 0;
            double totaldepositbyyesterday = 0;
            double totalwithdrawalbyyesterday = 0;
            foreach (var item in deposit)
            {
                totaldeposit = (double)(totaldeposit + item.Amount);
            }
            foreach (var item in withdrawal)
            {
                totalwithdrawal = (double)(totalwithdrawal + item.Amount);
            }
            string compairincome = "0%";
            double income = totaldeposit - totalwithdrawal;

            var withdrawbyday = withdrawal.Where(s => s.DateCreate.ToString().Contains(daynow)).ToList();
            var depositbyday = deposit.Where(s => s.DateCreate.ToString().Contains(daynow)).ToList();

            foreach (var item in depositbyday)
            {
                totaldepositbyday = (double)(totaldepositbyday + item.Amount);
            }
            foreach (var item in withdrawbyday)
            {
                totalwithdrawalbyday = (double)(totalwithdrawalbyday + item.Amount);
            }

            var withdrawbyyesterday = withdrawal.Where(s => s.DateCreate.ToString().Contains(dayyesterday)).ToList();
            var depositbyyesterday = deposit.Where(s => s.DateCreate.ToString().Contains(dayyesterday)).ToList();
            double totalpricefrombooking = 0;

            foreach (var item in depositbyyesterday)
            {
                totaldepositbyyesterday = (double)(totaldepositbyyesterday + item.Amount);
            }
            foreach (var item in withdrawbyyesterday)
            {
                totalwithdrawalbyyesterday = (double)(totalwithdrawalbyyesterday + item.Amount);
            }

            foreach( var item in booking)
            {
                totalpricefrombooking = (double)(totalpricefrombooking + item.Price);
            }


            double incomebyday = totaldepositbyday - totalwithdrawalbyday;
            double incomebyyesterday = totaldepositbyyesterday - totalwithdrawalbyyesterday;
            if (incomebyyesterday < 1) incomebyyesterday = 1;
           
             compairincome = (incomebyday / incomebyyesterday * 100) + "%";
  










            var countconsultant = consultant.Count();
            var countcustomer = customer.Count();
            var countbookingsuccess = booking.Count();
            List<TotalDashboard> totaldetail = new List<TotalDashboard>();

            TotalDashboard totaldash = new TotalDashboard();
            {
                totaldash.TotalConsultant = countconsultant;
                totaldash.TotalCustomer = countcustomer;
                totaldash.SucessBooking = countbookingsuccess;
                totaldash.TotalWithdrawal = totalwithdrawal;
                totaldash.TotalDeposit = totaldeposit;
                totaldash.Income = income;
                totaldash.IncomeDaily = incomebyday;
                totaldash.Compairincome = compairincome;
                totaldash.TotalPriceFromBooking = totalpricefrombooking;
            }

            totaldetail.Add(totaldash);
            var resultnew = totaldetail.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", Data = resultnew });
        }


        public class TotalDashboard
        {
            public int? TotalConsultant { get; set; }
            public int? TotalCustomer { get; set; }
            public int? SucessBooking { get; set; }

            public double? TotalWithdrawal { get; set; }
            public double? TotalDeposit { get; set; }
            public double? Income { get; set; }
            public double? IncomeDaily { get; set; }
            public string? Compairincome { get; set; }
            public double? TotalPriceFromBooking { get; set; }

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


        public class DetailDeposit
        {
            public string Name { get; set; }
            public string Month { get; set; }
            public double? Total { get; set; }
        }

        [HttpGet("ReportDepositByYear")]
        public async Task<ActionResult<Deposit>> DetailDeposits(int year = 2022)
        {
            List<DetailDeposit> detail = new List<DetailDeposit>();
            for (int month = 1; month <= 12; month++)
            {

                var firstDayOfMonth = new DateTime(year, month, 1);
                var lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                double total = 0;
                var resultdeposit = (from s in _context.Deposits
                                     where s.DateCreate >= firstDayOfMonth && s.DateCreate <= lastDayOfMonth && s.Status == "success"
                                     select new
                                     {
                                         Id = s.Id,
                                         CustomerName = s.Wallet.Customer.Fullname,
                                         DateCreate = s.DateCreate,
                                         Amount = s.Amount
                                     }).ToList();
                foreach (var item in resultdeposit)
                {
                    total = (double)(total + item.Amount);
                }

                DetailDeposit depo = new DetailDeposit();
                {
                    depo.Month = "Tháng " + month.ToString();
                    depo.Name = "Nạp";
                    depo.Total = total;
                }
                detail.Add(depo);
            }
            var resultnew = detail.ToList();

            return Ok(new { StatusCode = 200, Content = "Load successful", Data = resultnew });

        }
        [HttpGet("Gettopconsultantbyrate")]

        public IActionResult GetTopConsultantByRating()
        {
            var result = (from s in _context.Consultants
                          
                          select new
                          {
                              Id = s.Id,
                              FullName = s.FullName,
                              ImageUrl = s.ImageUrl,
                              Email = s.Email,
                              Address = s.Address,
                              Dob = s.Dob,
                              Gender = s.Gender,
                              Phone = s.Phone,
                              Status = s.Status,
                              Experience = s.Experience,
                              Booking = s.SlotBookings.Where(s => s.Status=="success").Count(),
                              Rating = s.Rating == null ? 0 : s.Rating
                          }).ToList();
            var sort = result.OrderByDescending(x => x.Booking).Take(10).ToList();
            return Ok(new { StatusCode = 200, Content = "Load successful", Data = sort });
        }




        public class HomeConsultant
        {

            public int Id { get; set; }
            public string FullName { get; set; }
            public string ImageUrl { get; set; }
            public int? Experience { get; set; }
            public double? Rating { get; set; }
            public string Specialization { get; set; }
            public int? Feedback { get; set; }
            public int? AmountWallet { get; set; }

        }




        [HttpGet("DashboardConsultant")]

        public IActionResult DashboardConsultant(int id)
        {
            var consu = _context.Consultants.Where(a => a.Id == id).FirstOrDefault();
            if (consu == null) return StatusCode(404, new { StatusCode = 404, Message = "Không tìm thấy thông tin Consultant" });

            var special = (from s in _context.Specializations
                           where s.ConsultantId == id
                           select new
                           {
                               Id = s.Id,
                               Specname = s.SpecializationType.Name,
                               SpecializationTypeId = s.SpecializationTypeId

                           }).ToList();


            var feedbackconsu = _context.Bookings.Where(a => a.ConsultantId == consu.Id && a.Feedback != null).Count();
            var wallet = _context.Wallets.Where(a => a.ConsultantId == consu.Id).FirstOrDefault();

            string fullName = consu.FullName;
            var arr = fullName.Split(' ');
            var firstname = arr[0];
            var lastname = arr[arr.Length -1];

            var name = firstname + " " + lastname;

            HomeConsultant info = new HomeConsultant();


            info.Id = consu.Id;
            info.FullName = name;
            info.ImageUrl = consu.ImageUrl;
            info.Experience = consu.Experience;
            info.Rating = consu.Rating;
            info.Feedback = feedbackconsu;
            info.AmountWallet = wallet.Crab;



            info.Specialization = "Chưa có";
            if (special.Count > 0)
            {
                info.Specialization = "";
                foreach (var item in special)
                {
                    info.Specialization = info.Specialization + item.Specname + ", ";

                }

                info.Specialization = info.Specialization.Remove(info.Specialization.Length - 2, 2);
            }

           


            List<HomeConsultant> listinfo = new List<HomeConsultant>()
            {
                new HomeConsultant{
                Id = info.Id,
                FullName = info.FullName,
                ImageUrl = info.ImageUrl,
                Experience = info.Experience,
                Rating = info.Rating,
                Specialization = info.Specialization,
                Feedback = info.Feedback,
                AmountWallet = info.AmountWallet
        }
            }.ToList();



            return Ok(new { StatusCode = 200, Message = "Load successful", data = listinfo });
        }
    }
}
