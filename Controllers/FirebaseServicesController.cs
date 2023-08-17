using Firebase.Auth;
using Firebase.Storage;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PsychologicalCounseling.Models;
using PsychologicalCounseling.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PsychologicalCounseling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirebaseServicesController : ControllerBase
    {
        private Random randomGenerator = new Random();
        private readonly IMailService _mailService;
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;
        private readonly PsychologicalCouselingContext _context;
        //private readonly ISendNotiService _sendnoti;
        //   private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        /*  private static string apiKey = "AIzaSyD4wde62JjvquWn7izXRPgpaja6Aibm3-k";

          private static string Bucket = "psychologicalcounseling-28efa.appspot.com";
          private static string AuthEmail = "teamwithfirebase@gmail.com";
          private static string AuthPassword = "Test@123";*/
         private static string apiKey = "AIzaSyD5NIvb5a4ZsEwnSYAII5803RgSSHdVn14";

          private static string Bucket = "psycteamv1.appspot.com";
          private static string AuthEmail = "tuanninh655@gmail.com";
          private static string AuthPassword = "bivsan1";
        public FirebaseServicesController(IConfiguration config, PsychologicalCouselingContext context, IHostingEnvironment env, IHttpContextAccessor httpContextAccessor, IMailService mailService)
        {
            _config = config;
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _mailService = mailService;

        }

        [HttpPost("logincustomer")]
        public async Task<IActionResult> loginCustomer(Models.User us)
        {
            var user = GetAccount_byUsername(us.UserName);
            if (user == null) return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid username/password!!" });
            var customer = GetAccount_byEmailCustomer(user.Email);
            var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));

            var authen = user.Firebaseid;
            if (user != null && customer != null && authen != null)
            {
                var hasuser = _context.Users.SingleOrDefault(p => (p.UserName.ToUpper() == us.UserName.ToUpper()) && us.PassWord == p.PassWord && p.Status == "active");
                if (hasuser == null)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email or username/password!!" });
                }

                if (hasuser != null)
                {
                    if (hasuser.Status == "inactive")
                    {
                        return StatusCode(409, new { StatusCode = 409, Message = "The Account is not verified" });
                    }
                    if (customer.Status == "inactive") return StatusCode(409, new { StatusCode = 409, Message = "The Account is banned or not verified" });
                    var token = GenerateJwtToken(hasuser);
                  //  string tokenfb = authen.FirebaseToken;
                    string uidfb = authen;
                    int iddb = hasuser.Id;
                    int customerid = customer.Id;
                    if (uidfb != "")
                    {

                        user.FcmToken = us.FcmToken;
                        _context.Users.Update(user);
                        await _context.SaveChangesAsync();

                        return Ok(new { StatusCode = 200, Message = "Login with role customer successful",uidfb, customerid, iddb, JWTTOKEN = token, Role = "Customer" });
                    }

                }
            }
            return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email or username/password!!" });
        }


        [HttpPost("loginadmin")]
        public async Task<IActionResult> loginAdmin(Models.User us)
        {
         //   var userid = _userService.GetUserId();
            var user = GetAccount_byUsername(us.UserName);
            //  var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            //  var authen = await auth.SignInWithEmailAndPasswordAsync(user.Email, user.PassWord);
            //Tạo MD5 
            MD5 mh = MD5.Create();
            //Chuyển kiểu chuổi thành kiểu byte
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes("SHPT");
            //mã hóa chuỗi đã chuyển
            byte[] hash = mh.ComputeHash(inputBytes);
            //tạo đối tượng StringBuilder (làm việc với kiểu dữ liệu lớn)
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            var testmd5 = sb.ToString();

            return Ok(new { StatusCode = 200, Message = "TestMD%", hashcode = testmd5 });




            if (user != null )
            {
                var hasuser = _context.Users.SingleOrDefault(p => (p.UserName.ToUpper() == us.UserName.ToUpper() && us.PassWord == p.PassWord && p.Status == "active" && p.IsAdmin=="true"));
                if (hasuser == null)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email or username/password!!" });
                }
                else
                {
                    var token = GenerateJwtToken(hasuser);
                //    string tokenfb = authen.FirebaseToken;
                //    string uidfb = authen.User.LocalId;
                    int iddb = hasuser.Id;
                    user.FcmToken = us.FcmToken;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    HttpContext.Session.SetString("Admin", "HangFire2810");
                    return Ok(new { StatusCode = 200, Message = "Login with role Admin successful", JWTTOKEN = token, Role = "Admin" });
                    

                }
            }
            return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email or username/password!!" });
        }



        [HttpPost("loginconsultant")]
        public async Task<IActionResult> loginConsultant(Models.User us)
        { 
            var user = GetAccount_byUsername(us.UserName);
            if (user == null) return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid username/password!!" });
            var consultant = GetAccount_byEmailConsultant(user.Email);
            var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            var authen = user.Firebaseid;

            if (user != null && consultant != null && authen !=null)
            {
                var hasuser = _context.Users.SingleOrDefault(p => (p.UserName.ToUpper() == us.UserName.ToUpper()) && us.PassWord == p.PassWord && p.Status == "active");
                if (hasuser == null)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email or username/password!!" });
                }

                if (hasuser != null)
                {
                    if (hasuser.Status == "inactive")
                    {
                        return StatusCode(409, new { StatusCode = 409, Message = "The Account is not verified" });
                    }
                    if (consultant.Status == "inactive") return StatusCode(409, new { StatusCode = 409, Message = "The Account is not verified" });
                    if (consultant.Status == "banned") return StatusCode(409, new { StatusCode = 409, Message = "The Account has banned " });
                    var token = GenerateJwtToken(hasuser);
                //    string tokenfb = authen.FirebaseToken;
                    string uidfb = authen;
                    int id = consultant.Id;

                    if (uidfb != "")
                    {

                        user.FcmToken = us.FcmToken;
                        _context.Users.Update(user);
                        await _context.SaveChangesAsync();


                        var hasconsu = _context.DeviceTokens.SingleOrDefault(p => p.ConsultantId == id);
                        if (hasconsu == null)
                        {
                            var consu = new DeviceToken();
                            {
                                consu.ConsultantId = id;                        
                            }
                            _context.DeviceTokens.Add(consu);
                            await _context.SaveChangesAsync();
                        }




                        var checkwarning = _context.Notifications.Where(s => s.ConsultantId == id && s.Type == "Warning").Any();
                        if(checkwarning == false)
                        {
                            var slotoverdue = _context.SlotBookings.Where(s => s.ConsultantId == id && s.Status == "overdue").Select(a => a.SlotId);
                            int countoverdue = slotoverdue.Count();
                            if (countoverdue == 2)
                            {
                                var notifi = new Models.Notification();
                                {
                                    notifi.ConsultantId = consultant.Id;
                                    notifi.DateCreate = DateTime.Now.AddHours(7);
                                    notifi.IsAdmin = false;
                                    notifi.Type = "Warning";
                                    notifi.Status = "notseen";
                                    notifi.Description = "Cảnh báo: có 2 slot bỏ lỡ!";

                                }
                                _context.Notifications.Add(notifi);
                                await _context.SaveChangesAsync();

                                var a = _context.Users.Where(a => a.Email.ToUpper() == consultant.Email.ToUpper()).FirstOrDefault();
                                await sendCodeEmail(a);
                            }
                          

                        }
                       



                        return Ok(new { StatusCode = 200, Message = "Login with role Consultant successful", uidfb, id, JWTTOKEN = token, Role = "Consultant" });
                    }

                }
            }
            return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email or username/password!!" });
        }


        [HttpPost("loginapp")]
        public async Task<IActionResult> loginApp(Models.User us)
        {


          



            var user = GetAccount_byUsername(us.UserName);
            if(user == null) return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid username/password!!" });
            var consultant = GetAccount_byEmailConsultant(user.Email);
            var customer = GetAccount_byEmailCustomer(user.Email);
            var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            
          //  var authen2 = await auth.SignInWithCustomTokenAsync(user.Firebaseid);
         
            var authen = user.Firebaseid;



            //var authen2 = await auth.SignInWithCustomTokenAsync(user.Firebaseid);
            //   if (user == null && consultant == null) return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid username/password!!" });
            //   if (user == null && customer == null) return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid username/password!!" });



         /*   var logged = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            if (logged != null)
            {
              return StatusCode(409, new { StatusCode = 409, Message = "Tài khoản đã được đăng nhập!!" });               
            }*/





                /*   var loggeduser = Convert.ToInt32(HttpContext.User.FindFirstValue(""));

                   if(loggeduser >0) { 
                   if(user.Id == loggeduser)
                   {
                       Console.WriteLine("tai khoan dang dang nhap o mot noi khac!");
                   }

                   }*/



                if (user != null && customer != null && authen != null )
            {
                var hasuser = _context.Users.SingleOrDefault(p => (p.UserName.ToUpper() == us.UserName.ToUpper()) && us.PassWord == p.PassWord && p.Status == "active");
                if (hasuser == null)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid  username/password!!" });
                   // return BadRequest(" The Account not exist or Invalid email or username/password!!");
                }
                

                if (hasuser != null)
                {
                    if (hasuser.Status == "inactive")
                    {
                        return StatusCode(409, new { StatusCode = 409, Message = "The Account is not verified" });
                    }
                    if (customer.Status == "inactive") return StatusCode(409, new { StatusCode = 409, Message = "The Account is not verified" });
                    if (customer.Status == "banned") return StatusCode(409, new { StatusCode = 409, Message = "The Account is banned" });
                    var token = GenerateJwtToken(hasuser);
                    //string tokenfb = authen.FirebaseToken;
                    string uidfb = authen;
                    int idcustomer = customer.Id;
                    int iddb = hasuser.Id;
                  
                    if (uidfb != "")
                    {

                        user.FcmToken = us.FcmToken;
                        _context.Users.Update(user);
                        await _context.SaveChangesAsync();
                        return Ok(new { StatusCode = 200, Message = "Login with role customer successful", uidfb, iddb, idcustomer, JWTTOKEN = token, Role = "Customer" });
                    }

                }
            }


            if (user != null && consultant != null && authen != null )
            {
                var hasuser = _context.Users.SingleOrDefault(p => (p.UserName.ToUpper() == us.UserName.ToUpper()) && us.PassWord == p.PassWord && p.Status == "active");
                if (hasuser == null)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid username/password!!" });
                }


                if (hasuser != null)
                {
                    if (hasuser.Status == "inactive")
                    {
                        return StatusCode(409, new { StatusCode = 409, Message = "The Account is not verified" });
                    }
                    if (consultant.Status == "inactive") return StatusCode(409, new { StatusCode = 409, Message = "The Account is not verified" });
                    if (consultant.Status == "banned") return StatusCode(409, new { StatusCode = 409, Message = "The Account is banned " });
                    var token = GenerateJwtToken(hasuser);
                    //string tokenfb = authen.FirebaseToken;
                    string uidfb = authen;
                    int iddb = hasuser.Id;
                    int idconsultant = consultant.Id;
                    if (uidfb != "")
                    {
                        user.FcmToken = us.FcmToken;
                        _context.Users.Update(user);
                        await _context.SaveChangesAsync();

                        var hasconsu = _context.DeviceTokens.SingleOrDefault(p => p.ConsultantId == idconsultant);
                        if (hasconsu == null)
                        {
                            var consu = new DeviceToken();
                            {
                                consu.ConsultantId = idconsultant;
                            }
                            _context.DeviceTokens.Add(consu);
                            await _context.SaveChangesAsync();
                        }
                        


                        return Ok(new { StatusCode = 200, Message = "Login with role consultant successful", uidfb, iddb, idconsultant, JWTTOKEN = token, Role = "Consultant" });
                    }

                }
            }
           
            return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid username/password!!" });
        }

        [HttpPost("sendmessage")]
        public async Task<IActionResult> SendNotifi(string title, string body)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("private_key.json")
                });
            }

            var fcmToken = "cXnbFBfDSYO-bvozERnnz-:APA91bFzRJ0fShDdm-1PA1cq6uIUfqZvlhiiwS9pO4rXV63ZFOWX11V_47-eVNV5K9_44pOP4ZDWSC9fx9fOUqACUtserjqIj85NBjz26wUe-U-bW4tMNYH5C8UX2MoK8v0TGiB6y5WQ";
           // cXnbFBfDSYO - bvozERnnz -:APA91bFzRJ0fShDdm - 1PA1cq6uIUfqZvlhiiwS9pO4rXV63ZFOWX11V_47 - eVNV5K9_44pOP4ZDWSC9fx9fOUqACUtserjqIj85NBjz26wUe - U - bW4tMNYH5C8UX2MoK8v0TGiB6y5WQ    //  fcm cua web: var fcmToken = "fKkLgEfjJC5RiR0xaHFGDn:APA91bGL9WuFwTRMGkXdd4wYcv3iQ4W-wIkkLrpC3vpnDwhrYF9rdLcmqJWIsn3CUcud2rGW4bCf0YjY0PKNlZVWZ89HSh7ibfCgLqnClgZnaZERwow4j15uquxqwKxlS1iu3agYrZ1v";
            //  eEE7zdTlTbuXcwuX32NQu2: APA91bFVAUk78aqB0 - Xgzdy7qi5 - wwLgRLXxIbQcuijxALJTfpegEkj - WdwT_OskwSQgh7 - kUTclXZ2YUg_Plf4S1VM3rLxlYdVZtE2 - LqYtWgAX9Tp8XnOmoLA1l4FDLzREk2sFEMmD
            // fcmtoken moi cai thiet bi


            var message = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    {"Mydatav1","PSYCteamv1" },
                },
                Token = fcmToken,
                Notification = new FirebaseAdmin.Messaging.Notification()
                {
                    Title = title,
                    Body = body
                }
            };

            string res = FirebaseMessaging.DefaultInstance.SendAsync(message).Result;
            if (res != "")
            {
                return Ok(new { StatusCode = 200, Message = "Successfully sent message", data = res, title, body });
            }
            return BadRequest("Error with FCMtoken");
        }




        private Models.User GetAccount_byEmail(string email)
        {
            var account = _context.Users.Where(a => a.Email.ToUpper() == email.ToUpper() || a.UserName.ToUpper() == email.ToUpper()).FirstOrDefault();

            if (account == null)
            {
                return null;
            }

            return account;
        }


        private async Task<bool> Sendmessagefcm(string title, string body, string fcmToken)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("private_key.json")
            });

         //   var fcmToken = "fKkLgEfjJC5RiR0xaHFGDn:APA91bGL9WuFwTRMGkXdd4wYcv3iQ4W-wIkkLrpC3vpnDwhrYF9rdLcmqJWIsn3CUcud2rGW4bCf0YjY0PKNlZVWZ89HSh7ibfCgLqnClgZnaZERwow4j15uquxqwKxlS1iu3agYrZ1v";
            //  fcm cua web: var fcmToken = "fKkLgEfjJC5RiR0xaHFGDn:APA91bGL9WuFwTRMGkXdd4wYcv3iQ4W-wIkkLrpC3vpnDwhrYF9rdLcmqJWIsn3CUcud2rGW4bCf0YjY0PKNlZVWZ89HSh7ibfCgLqnClgZnaZERwow4j15uquxqwKxlS1iu3agYrZ1v";
            //  eEE7zdTlTbuXcwuX32NQu2: APA91bFVAUk78aqB0 - Xgzdy7qi5 - wwLgRLXxIbQcuijxALJTfpegEkj - WdwT_OskwSQgh7 - kUTclXZ2YUg_Plf4S1VM3rLxlYdVZtE2 - LqYtWgAX9Tp8XnOmoLA1l4FDLzREk2sFEMmD
            // fcmtoken moi cai thiet bi
           
            var message = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    {"Mydatav1","PSYCteamv1" },
                },
                Token = fcmToken,
                Notification = new FirebaseAdmin.Messaging.Notification()
                {
                    Title = title,
                    Body = body
                }
            };

            string res = FirebaseMessaging.DefaultInstance.SendAsync(message).Result;
            if (res != "")
            {
                return true;
            }
            return false;
        }



        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> Upload(IFormFile file)
        {

            var fileupload = file;
            FileStream fs = null;
            if (fileupload.Length > 0)
            {
                {
                    string foldername = "firebaseFiles";
                    string path = Path.Combine($"Images", $"Images/{foldername}");


                    if (Directory.Exists(path))
                    {

                        using (fs = new FileStream(Path.Combine(path, fileupload.FileName), FileMode.Create))
                        {

                            await fileupload.CopyToAsync(fs);
                        }

                        fs = new FileStream(Path.Combine(path, fileupload.FileName), FileMode.Open);


                    }


                    else
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));

                var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);


                var cancel = new CancellationTokenSource();

                var upload = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
                    ).Child("images").Child(fileupload.FileName).PutAsync(fs, cancel.Token);

                // await upload;
                try
                {
                    string link = await upload;

                    return Ok(new { StatusCode = 200, Message = "Upload FIle success" , data = link});
                }
                catch (Exception ex)
                {
                    throw;
                }

            }


            return BadRequest("Failed Upload");
        }










        private Models.Customer GetAccount_byEmailCustomer(string email)
        {
            var account = _context.Customers.Where(a => a.Email.ToUpper() == email.ToUpper()).FirstOrDefault();

            if (account == null)
            {
                return null;
            }

            return account;
        }
        private Models.User GetAccount_byUsername(string username)
        {
            var account = _context.Users.Where(a => a.UserName.ToUpper() == username.ToUpper()).FirstOrDefault();

            if (account == null)
            {
                return null;
            }

            return account;
        }





        private Models.Consultant GetAccount_byEmailConsultant(string email)
        {
            var account = _context.Consultants.Where(a => a.Email.ToUpper() == email.ToUpper()).FirstOrDefault();

            if (account == null)
            {
                return null;
            }

            return account;
        }

        private async Task<bool> sendCodeEmail(Models.User a)
        {
            try
            {
                var consultant = _context.Consultants.Where(a => a.Email.ToUpper() == a.Email.ToUpper()).FirstOrDefault();
                var mailRequest = new MailRequest();
                mailRequest.ToEmail = a.Email;

                var username = mailRequest.ToEmail.Split('@')[0];
                mailRequest.Subject = "Xin chào " + consultant.FullName + ", Cảnh báo về việc bỏ lỡ cuộc hẹn";
                mailRequest.Description = "Cảnh báo: Bạn vừa bỏ lỡ 2/2 cuộc hẹn. Nếu như bỏ lỡ quá 2 cuộc hẹn, bạn có thể bị vi phạm và khoá tài khoản.";
                mailRequest.Value = "Cảnh báo";
                await _mailService.SendEmailAsync(mailRequest.ToEmail, mailRequest.Subject, mailRequest.Description, mailRequest.Value);
                Console.WriteLine(mailRequest.ToEmail);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string GenerateJwtToken(Models.User user)
        {
            string role = "admin";
            var useracc = GetAccount_byUsername(user.UserName);         
            var consultant = GetAccount_byEmailConsultant(useracc.Email);
            var customer = GetAccount_byEmailCustomer(useracc.Email);
            if(consultant != null)
            {
                role = "consultant";
            }
            if (customer != null)
            {
                role = "customer";
            }


            var securitykey = Encoding.UTF8.GetBytes(_config["Jwt:Secret"]);
            var claims = new Claim[] {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role),
     
            };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(securitykey), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }



    }
}
