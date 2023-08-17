using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PsychologicalCounseling.Common;
using PsychologicalCounseling.Models;
using PsychologicalCounseling.Services;

namespace PsychologicalCounseling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private Random randomGenerator = new Random();
        private readonly IMailService _mailService;
        private readonly PsychologicalCouselingContext _context;
        private static string apiKey = "AIzaSyD5NIvb5a4ZsEwnSYAII5803RgSSHdVn14";

        private static string Bucket = "psycteamv1.appspot.com";
        private static string AuthEmail = "tuanninh655@gmail.com";
        private static string AuthPassword = "bivsan1";
        public UsersController(PsychologicalCouselingContext context , IMailService mailService)
        {
            _context = context;
            _mailService = mailService;
        }

        // GET: api/Users
        [HttpGet("Getalluser")]
        [Authorize]
        public IActionResult GetAllList(string search)
        {
            var result = (from s in _context.Users
                          select new
                          {
                              Id = s.Id,
                              Username = s.UserName,
                              Password = s.PassWord,
                              Email = s.Email,
                              Code = s.Code,
                              Status = s.Status
                          }).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Users
                          where s.UserName.Contains(search)
                          select new
                          {
                              Id = s.Id,
                              Username = s.UserName,
                              Password = s.PassWord,
                              Email = s.Email,
                              Code = s.Code,
                              Status = s.Status
                          }).ToList();
            }

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }

        // GET: api/Users/5
        [HttpGet("getbyid")]
        [Authorize]
        public async Task<ActionResult> GetUser(int id)
        {
            var all = _context.Users.AsQueryable();

            all = _context.Users.Where(us => us.Id.Equals(id));
            var result = all.ToList();

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }



        [HttpGet("checkbyemail")]
        public async Task<ActionResult> CheckUserByEmail(string email)
        {
           
            var a = GetAccount_byEmail(email);
            if(a != null) { 
           if(a.Status == "inactive" || a.Status == "banned") {
                return StatusCode(200, new { StatusCode = 200, message = "Email đã tồn tại nhưng chưa kích hoạt hoặc bị khóa!", status = a.Status });
                }
            
            else if (a.Status == "active")  return StatusCode(200, new { StatusCode = 200, message = "Email đã tồn tại!", status = a.Status });
            }
            return Ok(new { StatusCode = 200, Message = "Email chưa đăng ký!", status = "null"});
        }




        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> PutUser(Models.User user)
        {

            try
            {
                var us = await _context.Users.FindAsync(user.Id);
                if (us == null)
                {
                    return NotFound();
                }
                us.UserName = user.UserName == null ? us.UserName : user.UserName;
                us.PassWord = user.PassWord == null ? us.PassWord : user.PassWord;
                us.Email = user.Email == null ? us.Email : user.Email;


                _context.Users.Update(us);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 201, Message = "Update Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
       /* [HttpPost("createconsultant")]

        public async Task<ActionResult<Models.User>> PostUserConsultant(Models.User user)
        {
            try
            {
                var isuser = GetAccount_byUsername(user.UserName);
                var a = GetAccount_byEmail(user.Email);
                if (a != null) { 
                    if(a.Status !="active") return StatusCode(409, new { StatusCode = 409, message = "Email has already been, But not verify or Banned!" });
                    return StatusCode(409, new { StatusCode = 409, message = "Email has already been, please try again with another email!" }); }
                if (isuser != null) return StatusCode(409, new { StatusCode = 409, message = "Username has already been, please try again with another!" });
                var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
                await auth.CreateUserWithEmailAndPasswordAsync(user.Email, user.PassWord);
                var authen = await auth.SignInWithEmailAndPasswordAsync(user.Email, user.PassWord);
                string tokenfb = authen.FirebaseToken;
                string uidfb = authen.User.LocalId;
                var us = new Models.User();
                {
                    us.UserName = user.UserName;
                    us.PassWord = user.PassWord;
                    us.Email = user.Email;
                    us.Firebaseid = uidfb;
                    us.Status = "inactive";

                }
                _context.Users.Add(us);

                var consu = new Consultant();
                {
                    consu.Email = user.Email;
                    consu.Status = "inactive";
                }
           
                _context.Consultants.Add(consu);
                await _context.SaveChangesAsync();

                //     var b = GetAccount_byEmail(us.Email);





                var consultannew = _context.Consultants.Max(it => it.Id);
                var wallet = new Wallet();
                {
                    wallet.Crab = 0;
                    wallet.ConsultantId = consultannew;
                }
                _context.Wallets.Add(wallet);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }*/

        [HttpPost("createconsultantv2")]

        public async Task<ActionResult<Models.User>> PostUserConsultantv2(Models.User user)
        {
            try
            {
                var isuser = GetAccount_byUsername(user.UserName);
                var a = GetAccount_byEmail(user.Email);

                if (a != null)
                {
                    if (a.Status != "active") return StatusCode(409, new { StatusCode = 409, message = "Email has already been, But not verify or Banned!" });
                    return StatusCode(409, new { StatusCode = 409, message = "Email has already been, please try again with another email!" });
                }
                if (isuser != null) return StatusCode(409, new { StatusCode = 409, message = "Username has already been, please try again with another!" });
                var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
                await auth.CreateUserWithEmailAndPasswordAsync(user.Email, user.PassWord);
                var authen = await auth.SignInWithEmailAndPasswordAsync(user.Email, user.PassWord);
                string tokenfb = authen.FirebaseToken;
                string uidfb = authen.User.LocalId;
                var us = new Models.User();
                {
                    us.UserName = user.UserName;
                    us.PassWord = user.PassWord;
                    us.Email = user.Email;
                    us.Firebaseid = uidfb;
                    us.Status = "inactive";

                }
                _context.Users.Add(us);


                var consu = new Consultant();
                {
                    consu.Email = user.Email;
                    consu.Status = "inactive";
                    consu.Experience = 1;
                    consu.Rating = 5;
                }

                _context.Consultants.Add(consu);
                await _context.SaveChangesAsync();


                var consultannew = _context.Consultants.Max(it => it.Id);
                var wallet = new Wallet();
                {
                    wallet.Crab = 0;
                    wallet.ConsultantId = consultannew;
                }
                _context.Wallets.Add(wallet);
                await _context.SaveChangesAsync();

                if (us != null)
                {
                    var b = GetAccount_byEmail(us.Email);
                    await sendCodeEmail(b);
                }
                //     var b = GetAccount_byEmail(us.Email);
              
         
                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }




        [HttpPost("createcustomer")]

        public async Task<ActionResult<Models.User>> PostUserCustomer(Models.User user)
        {
            try
            {
                var isuser = GetAccount_byUsername(user.UserName); 
                var a = GetAccount_byEmail(user.Email);
                if (a != null)
                {
                    if (a.Status != "active") return StatusCode(409, new { StatusCode = 409, message = "Email has already been, But not verify or Banned!" });
                    return StatusCode(409, new { StatusCode = 409, message = "Email has already been, please try again with another email!" });
                }
                if (isuser != null) return StatusCode(409, new { StatusCode = 409, message = "Username has already been, please try again with another!" });
                var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));




                await auth.CreateUserWithEmailAndPasswordAsync(user.Email, user.PassWord);
                var authen = await auth.SignInWithEmailAndPasswordAsync(user.Email, user.PassWord);

                string uidfb = authen.User.LocalId;


                var us = new Models.User();
                {
                    us.UserName = user.UserName;
                    us.PassWord = user.PassWord;
                    us.Email = user.Email;
                    us.Firebaseid = uidfb;
                    us.Status = "inactive";
              
                }
                _context.Users.Add(us);
             

                var customer = new Customer();
                {
                    customer.Email = user.Email;
                    customer.Status = "inactive";
                }
                _context.Customers.Add(customer);

                 await _context.SaveChangesAsync();


                var customernew = _context.Customers.Max(it => it.Id);



                var wallet = new Wallet();
                {
                    wallet.Crab = 0;
                    wallet.CustomerId = customernew;
                }
                _context.Wallets.Add(wallet);
                await _context.SaveChangesAsync();


                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        [HttpPost("createcustomerv2")]

        public async Task<ActionResult<Models.User>> PostUserCustomerV2(Models.User user)
        {
            try
            {
                var isuser = GetAccount_byUsername(user.UserName);
                var a = GetAccount_byEmail(user.Email);
                if (a != null)
                {
                    if (a.Status != "active") return StatusCode(409, new { StatusCode = 409, message = "Email has already been, But not verify or Banned!" });
                    return StatusCode(409, new { StatusCode = 409, message = "Email has already been, please try again with another email!" });
                }
                if (isuser != null) return StatusCode(409, new { StatusCode = 409, message = "Username has already been, please try again with another!" });
                var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));




                await auth.CreateUserWithEmailAndPasswordAsync(user.Email, user.PassWord);
                var authen = await auth.SignInWithEmailAndPasswordAsync(user.Email, user.PassWord);

                string uidfb = authen.User.LocalId;


                var us = new Models.User();
                {
                    us.UserName = user.UserName;
                    us.PassWord = user.PassWord;
                    us.Email = user.Email;
                    us.Firebaseid = uidfb;
                    us.Status = "inactive";

                }
                _context.Users.Add(us);


                var customer = new Customer();
                {
                    customer.Email = user.Email;
                    customer.Status = "inactive";
                }
                _context.Customers.Add(customer);

                await _context.SaveChangesAsync();


                var customernew = _context.Customers.Max(it => it.Id);



                var wallet = new Wallet();
                {
                    wallet.Crab = 0;
                    wallet.CustomerId = customernew;
                }
                _context.Wallets.Add(wallet);
                await _context.SaveChangesAsync();

                if (us != null)
                {
                    var b = GetAccount_byEmail(us.Email);
                    await sendCodeEmail(b);
                }
                return Ok(new { StatusCode = 201, Message = "Add Successfull", idcustomer = customernew });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var us = await _context.Users.FindAsync(id);
            if (us == null)
            {
                return NotFound();
            }
            if (us.Status == "active")
                us.Status = "inactive";
            else
                us.Status = "active";

            _context.Users.Update(us);
            await _context.SaveChangesAsync();


            var customer = _context.Customers.Where(a => a.Email.ToUpper() == us.Email.ToUpper()).FirstOrDefault();
            if (customer.Status == "active")
                customer.Status = "inactive";
            else
                customer.Status = "active";

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            var consultant = _context.Consultants.Where(a => a.Email.ToUpper() == us.Email.ToUpper()).FirstOrDefault();
            if (consultant.Status == "active")
                consultant.Status = "inactive";
            else
                consultant.Status = "active";

            _context.Consultants.Update(consultant);
            await _context.SaveChangesAsync();



            return Ok(new { StatusCode = 200, Content = "The User was deleted successfully!!" });
        }


        public class UserInfor1
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
    
            public string UserName { get; set; }
        }
        public class UserInfor2
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }

            public int Id { get; set; }
        }

        [HttpPut("changepassuserbyusername")]
        public async Task<IActionResult> ChangePassWordUserByUserName([FromBody] UserInfor1 userinfor)
        {
            var user = _context.Users.Where(a => a.UserName == userinfor.UserName).FirstOrDefault();
            if (user == null || string.IsNullOrEmpty(userinfor.UserName))
            {
                return NotFound();
            }
            if (user.PassWord == userinfor.OldPassword)
            {
                user.PassWord = userinfor.NewPassword;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            else return StatusCode(400, new { StatusCode = 400, Message = "Sai thông tin định dạng, mật khẩu củ không chính xác!" });

            return Ok(new { StatusCode = 200, Content = "The Password of Waleet was changed successfully!!" });
        }

        [HttpPut("changepassuserbyconsultantid")]
        public async Task<IActionResult> ChangePassWordByConsultantId([FromBody] UserInfor2 userinfor)
        {
            var consu = _context.Consultants.Where(a => a.Id == userinfor.Id).FirstOrDefault();

            var user = _context.Users.Where(a => a.Email == consu.Email).FirstOrDefault();
            if (user == null || userinfor.Id < 1)
            {
                return NotFound();
            }
            if (user.PassWord == userinfor.OldPassword)
            {
                user.PassWord = userinfor.NewPassword;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            else return StatusCode(400, new { StatusCode = 400, Message = "Sai thông tin định dạng, mật khẩu củ không chính xác!" });

            return Ok(new { StatusCode = 200, Content = "The Password of Waleet was changed successfully!!" });
        }

        [HttpPut("changepassuserbycustomerid")]
        public async Task<IActionResult> ChangePassWordByCustomerId([FromBody] UserInfor2 userinfor)
        {
            var customer = _context.Customers.Where(a => a.Id == userinfor.Id).FirstOrDefault();

            var user = _context.Users.Where(a => a.Email == customer.Email).FirstOrDefault();
            if (user == null || userinfor.Id < 1)
            {
                return NotFound();
            }
            if (user.PassWord == userinfor.OldPassword)
            {
                user.PassWord = userinfor.NewPassword;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            else return StatusCode(400, new { StatusCode = 400, Message = "Sai thông tin định dạng, mật khẩu củ không chính xác!" });

            return Ok(new { StatusCode = 200, Content = "The Password of Waleet was changed successfully!!" });
        }





        private Models.User GetAccount_byEmail(string email)
        {
            var account = _context.Users.Where(a => a.Email.ToUpper() == email.ToUpper()).FirstOrDefault();

            if (account == null)
            {
                return null;
            }

            return account;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }


        private async Task<bool> sendCodeEmail(Models.User a)
        {
            try
            {             
              
                a.Code = randomGenerator.Next(1000, 9999).ToString(); //moi lan resend la reset code
                _context.Users.Update(a);
                await _context.SaveChangesAsync();

                var mailRequest = new MailRequest();
                mailRequest.ToEmail = a.Email;
                var username = mailRequest.ToEmail.Split('@')[0];
                mailRequest.Subject = " Welcome to Psychological Counseling App";
                mailRequest.Description = "Your Verify code: " + "    " + a.Code;
                mailRequest.Value = a.Code;
                await _mailService.SendEmailAsync(mailRequest.ToEmail, mailRequest.Subject, mailRequest.Description, mailRequest.Value);
                Console.WriteLine(mailRequest.ToEmail);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
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
        private string addTailEmail(string email)
        {
            if (!email.Contains("@")) //auto add ".com"              
            {
                return email + "@gmail.com";
            }
            return email;
        }

    }
}
