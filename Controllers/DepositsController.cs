using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PsychologicalCounseling.Models;
using PsychologicalCounseling.Services;
using PsychologicalCounseling.Services.VnPay;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;
using ZXing.Rendering;

namespace PsychologicalCounseling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositsController : ControllerBase
    {
        private readonly PsychologicalCouselingContext _context;
        private Random randomGenerator = new Random();

        private static string apiKey = "AIzaSyD5NIvb5a4ZsEwnSYAII5803RgSSHdVn14";

        private static string Bucket = "psycteamv1.appspot.com";
        private static string AuthEmail = "tuanninh655@gmail.com";
        private static string AuthPassword = "bivsan1";
        private readonly IDrawnatalchart _drawnatal;
        public DepositsController(PsychologicalCouselingContext context, IDrawnatalchart drawnatal)
        {
            _context = context;
            _drawnatal = drawnatal;

        }

        // GET: api/Deposits


        [HttpGet("GenerateQRCode")]
        public async Task<ActionResult> GetDeposits(string sdt, string name, string amount)
        {          
            var qrcode_text = $"2|99|{sdt}|{name}|ttrungta2031@gmail.com|0|0|{amount}|abcdf";
            var qrcode_textng = $"36|02570769301|Tran Trung Ta|TPbank|200000";
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            EncodingOptions enconding = new EncodingOptions() { Width = 300, Height = 300, Margin = 0, PureBarcode = true };
            enconding.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            barcodeWriter.Renderer = new BitmapRenderer();
            barcodeWriter.Options = enconding;
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            var bitmap = barcodeWriter.Write(qrcode_textng);
            // Bitmap logo = "https://firebasestorage.googleapis.com/v0/b/psychologicalcounseling-28efa.appspot.com/o/images%2FMoMo_Logo.png?alt=media&token=65c3a8b0-9324-475b-875a-4f1bd6bd290a";
            // Graphics g = Graphics.FromImage(bitmap);
            //  g.DrawImage( new Point((bitmap.Width), (bitmap.Height)));
            // bitmap.Save(path,ImageFormat.Jpeg);
            var bytes = ImagetoByteArrayaa(bitmap);
            //var abc = ImagetoByteArrayaa(bitmap).;
            // string b = enconding.GS1Format.ToString();

            //  var testchoi = ImagetoByteArrayaa(byteeee);

            /*  var file = File(byteeee, "image/jpeg");
              string imgString = Convert.ToBase64String(bytes);
              byte[] bytetest = Encoding.UTF8.GetBytes(byteeee);
              Image image = Base64ToImage(imgString);*/
            /*   HttpResponseMessage response = new HttpResponseMessage();            
               response.Content = new ByteArrayContent(bytes);
               response.Content.LoadIntoBufferAsync(bytes.Length).Wait();
               response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");*/
            // return response;

            //   return new FileStreamResult(file, "image/bmp");
            //return File(bytetest, "image/jpeg");

            Image img = (Image)bitmap;
            var stream = new MemoryStream();
            img.Save(stream, ImageFormat.Bmp);
            var link = _drawnatal.GetImageLinkFirebase(img);







           return Ok(new { StatusCode = 200, link = link });
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
                foreach(var item in resultdeposit)
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






        [HttpGet("Getalldeposit")]
      
        public IActionResult GetAllDeposit(string code, string date, int walletid, int pagesize = 5, int pagenumber = 1)
        {

            DateTime day = DateTime.Now.AddHours(7);
            var result = (from s in _context.Deposits
                          
                          select new
                          {
                              Id=s.Id,
                              CustomerName = s.Wallet.Customer.Fullname,
                              Code=s.Code,
                              Amount =s.Amount,
                              Hashcode =s.Hashcode,
                              DateCreate = s.DateCreate,
                              ReceiveAccountid =s.ReceiveAccountid,
                              Status =s.Status,
                              WalletId =s.WalletId


                          }).ToList();
            if (string.IsNullOrEmpty(date) && walletid > 0)
            {
                result = (from s in _context.Deposits
                          where s.WalletId == walletid 
                          select new
                          {
                              Id = s.Id,
                              CustomerName = s.Wallet.Customer.Fullname,
                              Code = s.Code,
                              Amount = s.Amount,
                              Hashcode = s.Hashcode,
                              DateCreate = s.DateCreate,
                              ReceiveAccountid = s.ReceiveAccountid,
                              Status = s.Status,
                              WalletId = s.WalletId

                          }).ToList();


              
              //  return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
            }
            else if (!string.IsNullOrEmpty(code))
            {
                result = (from s in _context.Deposits
                          where s.Code == code
                          select new
                          {
                              Id = s.Id,
                              CustomerName = s.Wallet.Customer.Fullname,
                              Code = s.Code,
                              Amount = s.Amount,                          
                              Hashcode = s.Hashcode,
                              DateCreate = s.DateCreate,
                              ReceiveAccountid = s.ReceiveAccountid,
                              Status = s.Status,
                              WalletId = s.WalletId

                          }).ToList();


            }
            else if (!string.IsNullOrEmpty(date) && walletid > 0)
            {
                var datenew = DateTime.Parse(date).ToShortDateString();
                var sort2 = result.Where(s => s.WalletId == walletid && s.DateCreate.GetValueOrDefault().ToShortDateString().Contains(datenew));
                var paging2 = sort2.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
                double totalpage3 = (double)sort2.Count() / pagesize;
                totalpage3 = Math.Ceiling(totalpage3);
                return Ok(new { StatusCode = 200, Message = "Load successful", data = paging2, totalpage = totalpage3 });

            }
            else if (!string.IsNullOrEmpty(date))
            {
                var datenew = DateTime.Parse(date).ToShortDateString();
                var sort3 = result.Where(s => s.DateCreate.GetValueOrDefault().ToShortDateString().Contains(datenew));
                var paging3 = sort3.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
                double totalpage2 = (double)sort3.Count() / pagesize;
                totalpage2 = Math.Ceiling(totalpage2);
                return Ok(new { StatusCode = 200, Message = "Load successful", data = paging3 , totalpage = totalpage2});


            }

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


        // GET: api/Deposits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Deposit>> GetDeposit(int id)
        {
            var deposit = await _context.Deposits.FindAsync(id);

            if (deposit == null)
            {
                return NotFound();
            }

            return deposit;
        }

        //   var wallet = _context.Wallets.Where(a => a.CustomerId == customerid).FirstOrDefault();


        [HttpGet("historydeposit")]
        public async Task<ActionResult<Deposit>> HistoryDepositByCustomerId(string search ,string date,int customerid, int pagesize = 10, int pagenumber = 1)
        {
            var wallet = _context.Wallets.Where(a => a.CustomerId == customerid).FirstOrDefault();

            if (wallet == null)
            {
                return NotFound();
            }
            var result = (from s in _context.Deposits
                          where s.WalletId == wallet.Id
                          select new
                          {
                              Id = s.Id,
                              CustomerName = s.Wallet.Customer.Fullname,
                              Code = s.Code,
                              Amount = s.Amount,
                              Hashcode = s.Hashcode,
                              DateCreate = s.DateCreate,
                              Status = s.Status,
                          }).ToList();

             if (!string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(search))
            {
                var datenew = DateTime.Parse(date).ToShortDateString();
                var sort2 = result.Where(s => s.Amount.GetValueOrDefault().ToString().Contains(search) && s.DateCreate.GetValueOrDefault().ToShortDateString().Contains(datenew));
                var paging2 = sort2.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
                double totalpage3 = (double)sort2.Count() / pagesize;
                totalpage3 = Math.Ceiling(totalpage3);
                return Ok(new { StatusCode = 200, Message = "Load successful", data = paging2, totalpage = totalpage3 });

            }
            if (!string.IsNullOrEmpty(date) && string.IsNullOrEmpty(search))
            {
                var datenew = DateTime.Parse(date).ToShortDateString();
                var sort3 = result.Where(s => s.DateCreate.GetValueOrDefault().ToShortDateString().Contains(datenew));
                var paging3 = sort3.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
                double totalpage2 = (double)sort3.Count() / pagesize;
                totalpage2 = Math.Ceiling(totalpage2);
                return Ok(new { StatusCode = 200, Message = "Load successful", data = paging3, totalpage = totalpage2 });
            }
            if (!string.IsNullOrEmpty(search))
            {
                var sort3 = result.Where(s => s.Amount.GetValueOrDefault().ToString().Contains(search));
                var paging3 = sort3.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
                double totalpage2 = (double)sort3.Count() / pagesize;
                totalpage2 = Math.Ceiling(totalpage2);
                return Ok(new { StatusCode = 200, Message = "Load successful", data = paging3, totalpage = totalpage2 });

            }


                var sort = result.OrderByDescending(x => x.Id).ToList();
            var paging = sort.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
            double totalpage1 = (double)sort.Count() / pagesize;
            totalpage1 = Math.Ceiling(totalpage1);
            return Ok(new { StatusCode = 200, Content = "Load successful", Data = paging, totalpage = totalpage1 });
        }

        // PUT: api/Deposits/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeposit(int id, Deposit deposit)
        {
            if (id != deposit.Id)
            {
                return BadRequest();
            }

            _context.Entry(deposit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepositExists(id))
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

        // POST: api/Deposits
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
       
        [HttpPut("acceptdeposit")]
        public async Task<IActionResult> AcceptDeposit(int id)
        {
            var us = await _context.Deposits.FindAsync(id);
            var wallet = _context.Wallets.Where(a => a.Id == us.WalletId).FirstOrDefault();
          //  var walletadmin = _context.Wallets.Where(a => a.IsAdmin == "admin").FirstOrDefault();
            if (us == null)
            {
                return NotFound();
            }
            if (us.Status == "waiting")
            {
                wallet.Crab = wallet.Crab + us.Amount;
                wallet.HistoryTrans = DateTime.Now.AddHours(7);

                us.Status = "success";






             

            }
            else us.Status = us.Status;

            _context.Wallets.Update(wallet);
            _context.Deposits.Update(us);
            await _context.SaveChangesAsync();


            var notifi = new Models.Notification();
            {
                notifi.CustomerId = wallet.CustomerId;
                notifi.DateCreate = DateTime.Now.AddHours(7);
                notifi.Type = "deposit";
                notifi.IsAdmin = false;
                notifi.Status = "notseen";
                notifi.Description = "Đơn nạp tiền " + us.Amount+ " GEM đã được xử lí thành công!";

            }
            _context.Notifications.Add(notifi);


          //  walletadmin.Crab = (int?)(walletadmin.Crab - us.Amount );
         //   walletadmin.HistoryTrans = DateTime.Now.AddHours(7);
          //  _context.Wallets.Update(walletadmin);

            await _context.SaveChangesAsync();



            return Ok(new { StatusCode = 200, Content = "The Deposit was accepted successfully!!" });
        }

        [HttpPut("rejectdeposit")]
        public async Task<IActionResult> RejectDeposit(int id)
        {
            var us = await _context.Deposits.FindAsync(id);
            var wallet = _context.Wallets.Where(a => a.Id == us.WalletId).FirstOrDefault();
            var walletadmin = _context.Wallets.Where(a => a.IsAdmin == "admin").FirstOrDefault();
            if (us == null)
            {
                return NotFound();
            }
            if (us.Status == "waiting")
            {                     
                us.Status = "fail";
            }
            else us.Status = us.Status;

        //    _context.Wallets.Update(wallet);
            _context.Deposits.Update(us);
            await _context.SaveChangesAsync();


            var notifi = new Models.Notification();
            {
                notifi.CustomerId = wallet.CustomerId;
                notifi.DateCreate = DateTime.Now.AddHours(7);
                notifi.Type = "deposit";
                notifi.IsAdmin = false;
                notifi.Status = "notseen";
                notifi.Description = "Đơn nạp tiền " + us.Amount + " GEM đã bị từ chối!";

            }
            _context.Notifications.Add(notifi);
            await _context.SaveChangesAsync();



            return Ok(new { StatusCode = 200, Content = "The Deposit was rejected successfully!!" });
        }






        [HttpPost("create")]
        public async Task<ActionResult<Deposit>> PostDeposit(int customerid, int amount)
        {

            var reaccrandom = (from row in _context.ReceiveAccounts
                               where row.Status == "active"
                               select new
                               {
                                   Id = row.Id,
                                   Name = row.Name,
                                   QRCode = row.QrCode,
                                   PhoneNumber = row.PhoneNumber,
                                   BankNumber = row.BankNumber,
                                   BankName = row.BankName,
                                   DateCreate = row.DateCreate,
                                   Status = row.Status
                               }).ToList();
            if (reaccrandom != null)
            {
                int count = reaccrandom.Count();
                int index = new Random().Next(count);
                var reaccount = reaccrandom.Skip(index).FirstOrDefault();
                if (reaccount == null) { return StatusCode(409, new { StatusCode = 409, Message = "Tài khoản nhận hiện đang bảo trì!" }); }
                // var qrcode = GetQrcode(reaccount.PhoneNumber, reaccount.Name, dep.Amount.ToString());
                var sotien = amount * 1000;
                var code = randomGenerator.Next(100000, 999999).ToString();
                var qrcode_text = $"2|99|{reaccount.PhoneNumber}|{reaccount.Name}|ttrungta2031@gmail.com|0|0|{sotien.ToString()}|{code}";
                BarcodeWriter barcodeWriter = new BarcodeWriter();
                EncodingOptions enconding = new EncodingOptions() { Width = 300, Height = 300, Margin = 0, PureBarcode = false };
                enconding.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
                barcodeWriter.Renderer = new BitmapRenderer();
                barcodeWriter.Options = enconding;
                barcodeWriter.Format = BarcodeFormat.QR_CODE;
                var bitmap = barcodeWriter.Write(qrcode_text);
                // Bitmap logo = "https://firebasestorage.googleapis.com/v0/b/psychologicalcounseling-28efa.appspot.com/o/images%2FMoMo_Logo.png?alt=media&token=65c3a8b0-9324-475b-875a-4f1bd6bd290a";
                // Graphics g = Graphics.FromImage(bitmap);

                //  g.DrawImage( new Point((bitmap.Width), (bitmap.Height)));
                // bitmap.Save(path,ImageFormat.Jpeg);

                var bytes = ImagetoByteArrayaa(bitmap);

                var wallet = _context.Wallets.Where(a => a.CustomerId == customerid).FirstOrDefault();
                var customer = _context.Customers.Where(a => a.Id == wallet.CustomerId).FirstOrDefault();
                if (wallet == null) { return StatusCode(409, new { StatusCode = 409, Message = "Tài khoản nhận hiện đang bảo trì!" }); }
                if (customer == null) { return StatusCode(409, new { StatusCode = 409, Message = "Tài khoản nhận hiện đang bảo trì!" }); }
                // string customername = wallet.Customer.Fullname;


                var hashcodenew = Sha256encrypt(code + "256");


                var notifi = new Notification();
                {
                    //  notifi.ConsultantId = booking.CustomerId;
                    notifi.DateCreate = DateTime.Now.AddHours(7);
                    notifi.IsAdmin = true;
                    notifi.Type = "deposit";
                    notifi.Status = "notseen";
                    notifi.Description = customer.Fullname + " vừa tiến hành nạp " + amount + " GEM" + " Chờ xử lí! Mã giao dịch : " + code;

                }
                _context.Notifications.Add(notifi);
                await _context.SaveChangesAsync();

                var deposit = new Deposit();
                {

                    deposit.Amount = amount;
                    deposit.Code = code;
                    deposit.DateCreate = DateTime.Now.AddHours(7);
                    deposit.Hashcode = hashcodenew;
                    deposit.WalletId = wallet.Id;
                    deposit.ReceiveAccountid = reaccount.Id;
                    deposit.Status = "waiting";
                }
                _context.Deposits.Add(deposit);
                // await _context.SaveChangesAsync();
                await _context.SaveChangesAsync();
                //      var depositidnew = _context.Deposits.Max(it => it.Id);
                //    var fileimage = File(bytes, "image");
                var file = File(bytes, "image/jpeg");

                Image img = (Image)bitmap;
                var stream = new MemoryStream();
                img.Save(stream, ImageFormat.Bmp);
                var link = _drawnatal.GetImageLinkFirebase(img);
                var linkbank = "https://img.vietqr.io/image/" + reaccount.BankName + "-" + reaccount.BankNumber + "-8z0UZDN.jpg?amount=" + sotien + "%5C&addInfo=" + code + "%5C&accountName=" + reaccount.Name;

                //            await _context.SaveChangesAsync();

                // return File(bytes, "image/bmp");
                //https://img.vietqr.io/image/tpbank-02570769301-compact2.jpg?amount=200000\&addInfo=nap%20tien%20cho%20Anh%20Ta\&accountName=Tran%Trung%20Ta
                //https://img.vietqr.io/image/vpbank-020304082000-compact2.jpg?amount=790000%5C&addInfo=777444%5C&accountName=Quy%20Vac%20Xin%20Covid

                //https://psycteam.azurewebsites.net/api/VnPay/ConfirmVnpay
                string url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
                string returnUrl = "https://psycteamv2.azurewebsites.net/api/VnPay/Getlinkvnpay";
                string tmnCode = "4LXA1E1I";
                string hashSecret = "RGKBNUTCAMSROXEOWMCSLLGAAEBSJCIJ";

                PayLib pay = new PayLib();
                pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
                pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
                pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
                pay.AddRequestData("vnp_Amount", (sotien * 100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
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

   
                await _context.SaveChangesAsync();





                return Ok(new { StatusCode = 200, Message = "Deposit Successful, Waiting for accept", vnpaylink= paymentUrl, qrcodemomo = link, qrcodebank = linkbank, code = code, name = reaccount.Name, phonenumber = reaccount.PhoneNumber, banknumber = reaccount.BankNumber, amount = amount });
            }
            return StatusCode(409, new { StatusCode = 409, Message = "Tài khoản nhận hiện đang bảo trì" });
        }



        // DELETE: api/Deposits/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteDeposit(int id)
        {
            var deposit = await _context.Deposits.FindAsync(id);
            if (deposit == null)
            {
                return NotFound();
            }

            _context.Deposits.Remove(deposit);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static string Sha256encrypt(string phrase)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA256Managed sha256hasher = new SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(phrase));
            return Convert.ToBase64String(hashedDataBytes);
        }

        private string ImageToBase64(Bitmap bitmap)
        {
            // string path = "D:\SampleImage.jpg";
            using (MemoryStream m = new MemoryStream())
            {
                bitmap.Save(m, bitmap.RawFormat);
                byte[] imageBytes = m.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                //var imagenew = _drawnatal.GetImageLinkFirebase(base64String);
                return base64String;

            }
        }
        private Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            return image;
        }
        private bool DepositExists(int id)
        {
            return _context.Deposits.Any(e => e.Id == id);
        }
        private  byte[] ImagetoByteArrayaa(System.Drawing.Image imagein)
        {
            MemoryStream ms = new MemoryStream();
            imagein.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }


        private Models.ReceiveAccount GetReceiveAccount_byName(string name)
        {
            var account = _context.ReceiveAccounts.Where(a => a.PhoneNumber.ToUpper() == name.ToUpper()).FirstOrDefault();

            if (account == null)
            {
                return null;
            }

            return account;
        }




        private  FileContentResult  GetQrcode(string sdt, string name, string amount)
        {
            var qrcode_text = $"2|99|{sdt}|{name}|ttrungta2031@gmail.com|0|0|{amount}";
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            EncodingOptions enconding = new EncodingOptions() { Width = 300, Height = 300, Margin = 0, PureBarcode = false };
            enconding.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            barcodeWriter.Renderer = new BitmapRenderer();
            barcodeWriter.Options = enconding;
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            var bitmap = barcodeWriter.Write(qrcode_text);
            // Bitmap logo = "https://firebasestorage.googleapis.com/v0/b/psychologicalcounseling-28efa.appspot.com/o/images%2FMoMo_Logo.png?alt=media&token=65c3a8b0-9324-475b-875a-4f1bd6bd290a";
            // Graphics g = Graphics.FromImage(bitmap);

            //  g.DrawImage( new Point((bitmap.Width), (bitmap.Height)));
            // bitmap.Save(path,ImageFormat.Jpeg);

            var bytes = ImagetoByteArrayaa(bitmap);

            return File(bytes, "image/bmp");


           // return Ok(new { StatusCode = 200, Content = "qrcode generate successful", data = bytes });
        }
    }
}
