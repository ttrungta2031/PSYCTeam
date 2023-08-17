using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using PsychologicalCounseling.Models;
using PsychologicalCounseling.Services.VnPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PsychologicalCounseling.Controllers
{
    /*[Route("api/[controller]")]
    [ApiController]
    public class VnPayController : ControllerBase
    {
    }*/
    [Route("api/[controller]")]
    [ApiController]

    public class VnPayController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;
        private Random randomGenerator = new Random();
        public VnPayController(PsychologicalCouselingContext context)
        {
            _context = context;
        }

        [HttpPost("TestVnpay")]
        public async Task<ActionResult> TestVnpay()
        {
            var code = randomGenerator.Next(100000, 999999).ToString();
            string url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string returnUrl = "https://psycteamv2.azurewebsites.net/api/VnPay/Getlinkvnpay";
            string tmnCode = "4LXA1E1I";
            string hashSecret = "RGKBNUTCAMSROXEOWMCSLLGAAEBSJCIJ";
            PayLib pay = new PayLib();
            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", (10*100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", code); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn
          
            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");

            var urltest = location.AbsoluteUri;


            return StatusCode(200, new { StatusCode = 200, Message ="Oke",Link = paymentUrl, urltest });
        }
        public class CustomerInfo
        {

            public int Id { get; set; }

        }


        [HttpGet("Getlinkvnpay")]
        public async Task<ActionResult> Getlinkvnpay()
        {
            
            var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            var urltest = location.AbsoluteUri;
            // await upnewuri(urltest);
           




            string checkcode = "vnp_OrderInfo=";
            string checkresponse = "vnp_ResponseCode=00";

            var flag = 1;
            string flagcode = "";
            int customerid = 0;
            var resultcode = (from s in _context.Deposits
                              where s.Status == "waiting"
                              select new
                              {
                                  Id = s.Id,
                                  Code = s.Code,
                                  CustomerId =s.Wallet.CustomerId
                              }).ToList();

            var dev = new Models.DeviceToken();
            dev.FcmToken = urltest;
            _context.DeviceTokens.Add(dev);
            await _context.SaveChangesAsync();


            foreach (var item in resultcode)
            {
                checkcode = "vnp_OrderInfo=" + item.Code;
                var uriindb = _context.DeviceTokens.Where(a => a.FcmToken.Contains(checkresponse) && a.FcmToken.Contains(checkcode)).FirstOrDefault();
                if (uriindb == null)
                {
                    flag = 1;
                }
                else { 
                    flag = 2;
                    flagcode = item.Code;
                    customerid = (int)item.CustomerId;


                    var wallet = _context.Wallets.Where(a => a.CustomerId == customerid).FirstOrDefault();
                    var codedepo = _context.Deposits.Where(a => a.WalletId == wallet.Id && a.Code == flagcode).FirstOrDefault();
                    wallet.Crab = wallet.Crab + codedepo.Amount;
                    wallet.HistoryTrans = DateTime.Now.AddHours(7);

                    codedepo.Status = "success";

                    var notifi = new Models.Notification();
                    {
                        notifi.CustomerId = wallet.CustomerId;
                        notifi.DateCreate = DateTime.Now.AddHours(7);
                        notifi.Type = "deposit";
                        notifi.IsAdmin = false;
                        notifi.Status = "notseen";
                        notifi.Description = "Đơn nạp tiền " + codedepo.Amount + " GEM đã được xử lí thành công!";
                    }
                    _context.Notifications.Add(notifi);
                    _context.Wallets.Update(wallet);
                    _context.Deposits.Update(codedepo);
                    await _context.SaveChangesAsync();
                    return Redirect("https://psyc-customer.vercel.app");
                }

            }

            if (flag == 1)
            {
                return Redirect("https://psyc-customer.vercel.app");
                return StatusCode(400, new { StatusCode = 400, Message = "Bạn chưa thanh toán! Nếu gặp sự cố liên hệ cho hỗ trợ!" });
            }
            if(flag == 2)
            {

                var wallet = _context.Wallets.Where(a => a.CustomerId == customerid).FirstOrDefault();
                var codedepo = _context.Deposits.Where(a => a.WalletId == wallet.Id && a.Code == flagcode).FirstOrDefault();
                wallet.Crab = wallet.Crab + codedepo.Amount;
                wallet.HistoryTrans = DateTime.Now.AddHours(7);

                codedepo.Status = "success";

                var notifi = new Models.Notification();
                {
                    notifi.CustomerId = wallet.CustomerId;
                    notifi.DateCreate = DateTime.Now.AddHours(7);
                    notifi.Type = "deposit";
                    notifi.IsAdmin = false;
                    notifi.Status = "notseen";
                    notifi.Description = "Đơn nạp tiền " + codedepo.Amount + " GEM đã được xử lí thành công!";
                }
                _context.Notifications.Add(notifi);
                _context.Wallets.Update(wallet);
                _context.Deposits.Update(codedepo);
                await _context.SaveChangesAsync();
                return Redirect("https://psyc-customer.vercel.app");
                return StatusCode(200, new { StatusCode = 200, Message = "Confirm success!!!!!" });
            }




           /* if (urltest.Contains(checkresponse))
            {               
                return Redirect("https://psyc-customer.vercel.app/test");
            }*/
      
            return StatusCode(200, new { StatusCode = 200, Message = "Bạn chưa thanh toán! Nếu gặp sự cố liên hệ cho hỗ trợ!" });
        }


        [HttpPost("ConfirmVnpay")]
        public async Task<ActionResult> ConfirmVnpay(CustomerInfo customer)
        {

            return Redirect("https://psyc-customer.vercel.app");
            /*//var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            //  var urltest = location.AbsoluteUri;
            var linkcome = "https://psycteamv2.azurewebsites.net/api/VnPay/Getlinkvnpay";
            RequestHeaders header = Request.GetTypedHeaders();
            Uri uriReferer = header.Referer;
            var urltest = uriReferer.AbsoluteUri;
            var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            var urlnow = location.AbsoluteUri;
           


            var maxiddeposit = _context.Deposits.Where(a => a.Wallet.CustomerId == customer.Id).Max(it => it.Id);
            var codedepo = _context.Deposits.Where(a => a.Id == maxiddeposit).FirstOrDefault();
            //  string checkresponse = "vnp_ResponseCode=00";
            //  if (urltest.Contains(checkresponse) )
            var wallet = _context.Wallets.Where(a => a.Id == codedepo.WalletId).FirstOrDefault();
            //  var walletadmin = _context.Wallets.Where(a => a.IsAdmin == "admin").FirstOrDefault();

            string checkresponse = "vnp_ResponseCode=00";
            string checkcode = "vnp_OrderInfo=";
            var flag = 1;
            var resultcode = (from s in _context.Deposits
                      where s.Status == "waiting"
                       select new
                      {
                          Id = s.Id,
                          Code = s.Code
                      }).ToList();
            foreach(var item in resultcode)
            {
                 checkcode = "vnp_OrderInfo=" + item.Code;
                var uriindb = _context.DeviceTokens.Where(a => a.FcmToken.Contains(checkresponse) && a.FcmToken.Contains(checkcode)).FirstOrDefault();
                if (uriindb == null)
                {
                    flag = 1;
                }
                else flag = 2;

            }

            if(flag == 1) {
                return StatusCode(400, new { StatusCode = 400, Message = "Bạn chưa thanh toán! Nếu gặp sự cố liên hệ cho hỗ trợ!" });
            }

            if (codedepo.Status == "waiting")
            {
                wallet.Crab = wallet.Crab + codedepo.Amount;
                wallet.HistoryTrans = DateTime.Now.AddHours(7);

                codedepo.Status = "success";
                var notifi = new Models.Notification();
                {
                    notifi.CustomerId = wallet.CustomerId;
                    notifi.DateCreate = DateTime.Now.AddHours(7);
                    notifi.Type = "deposit";
                    notifi.IsAdmin = false;
                    notifi.Status = "notseen";
                    notifi.Description = "Đơn nạp tiền " + codedepo.Amount + " GEM đã được xử lí thành công!";

                }
                _context.Notifications.Add(notifi);
                _context.Wallets.Update(wallet);
                _context.Deposits.Update(codedepo);
                await _context.SaveChangesAsync();
                return StatusCode(200, new { StatusCode = 200, Message = "Confirm success!!!!!" });
            }
            else codedepo.Status = codedepo.Status;


            //  walletadmin.Crab = (int?)(walletadmin.Crab - us.Amount );
            //   walletadmin.HistoryTrans = DateTime.Now.AddHours(7);
            //  _context.Wallets.Update(walletadmin);

            await _context.SaveChangesAsync();

            return StatusCode(200, new { StatusCode = 200, Message = "Confirm success" });*/



            //       return StatusCode(200, new { StatusCode = 200, Message = "Chơi ăn gian à^^" });
        }



     


        private async Task<bool> upnewuri(string urinew)
        {
            try
            {
                var dev = new Models.DeviceToken();
                
                    dev.FcmToken = urinew;             
                _context.DeviceTokens.Add(dev);
                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
