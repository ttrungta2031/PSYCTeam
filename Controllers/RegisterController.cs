using PsychologicalCounseling.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PsychologicalCounseling.Common;
using PsychologicalCounseling.Services;

using System.Net.Mail;
using System.Net;



namespace PsychologicalCounseling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class registerController : ControllerBase
    {
        private Random randomGenerator = new Random();
        private readonly PsychologicalCouselingContext _context;
        private readonly IMailService _mailService;
        private static string apiKey = "AIzaSyD5NIvb5a4ZsEwnSYAII5803RgSSHdVn14";

        private static string Bucket = "psycteamv1.appspot.com";
        private static string AuthEmail = "tuanninh655@gmail.com";
        private static string AuthPassword = "bivsan1";
        public registerController(PsychologicalCouselingContext context, IMailService mailService)
        {
            _context = context;
            _mailService = mailService;    
        }

        // api/Register/resend  //-Ta
        [HttpPost("resend")]  //email  
        public async Task<IActionResult> reSendEmail(string email)
        {
            try
            {
                email = addTailEmail(email);
                if (!Validate.isEmail(email))
                {
                    return StatusCode(409, new { StatusCode = 409, message = "Exception Email format" });
                }
                var auth = new Firebase.Auth.FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(apiKey));
                var authen = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
                string tokenfb = authen.FirebaseToken;
               
                var a = GetAccount_byEmail(email);
                if (a != null)
                {
                    await sendCodeEmail(a);
                    return StatusCode(200, new { StatusCode = 200, message = "Email re-send/Mã code đã được gửi đến email của bạn!" });
                }
                
                await auth.SendEmailVerificationAsync(tokenfb);
                await sendCodeEmail(a);
                return StatusCode(200, new { StatusCode = 200, message = "Email re-send/Mã code đã được gửi đến email của bạn!" });
            }
            catch (Exception ex)
            {
                return StatusCode(409, new { StatusCode = 409, message = "Account resend failed (" + ex.Message + ")" });
            }
        }


        [HttpPost("resendbyusername")]  //email  
        public async Task<IActionResult> reSendEmailByUserName(string username)
        {
            try
            {
                /* email = addTailEmail(email);
                 if (!Validate.isEmail(email))
                 {
                     return StatusCode(409, new { StatusCode = 409, message = "Exception Email format" });
                 }*/
                var a = _context.Users.Where(a => a.UserName.ToUpper() == username.ToUpper()).FirstOrDefault();
                if(a == null) { return StatusCode(409, new { StatusCode = 409, message = "Account resend failed! The Account is not exist" }); }
                var auth = new Firebase.Auth.FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(apiKey));
                var authen = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
                string tokenfb = authen.FirebaseToken;
              
                //   var a = GetAccount_byEmail(email);
                if (a != null)
                {
                    await sendCodeEmail(a);
                    return StatusCode(200, new { StatusCode = 200, message = "Email re-send/Mã code đã được gửi đến email của bạn!", email = a.Email });
                }

                await auth.SendEmailVerificationAsync(tokenfb);
                await sendCodeEmail(a);
                return StatusCode(200, new { StatusCode = 200, message = "Email re-send/Mã code đã được gửi đến email của bạn!" });
            }
            catch (Exception ex)
            {
                return StatusCode(409, new { StatusCode = 409, message = "Account resend failed (" + ex.Message + ")" });
            }
        }


        [HttpPut("changepassbyusername")] //email, passhash, code   ok
        public async Task<IActionResult> changePassByUserName(Models.User user)
        {
            try
            {
                //   var username = _context.Users.Where(a => a.UserName == user.UserName).FirstOrDefault();



                /*    user.Email = addTailEmail(user.Email);
                    if (!Validate.isEmail(user.Email))
                    {
                        return StatusCode(409, new { StatusCode = 409, message = "Exception Email format" });//ok
                    }
    */
                // var a = GetAccount_byEmail(user.Email);
                //   var auth = new Firebase.Auth.FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(apiKey));
                //     var authen = await auth.SignInWithEmailAndPasswordAsync(a.Email, a.PassWord);

                //  string tokenfb = authen.User.LocalId;
                //   await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(tokenfb);
                var a = _context.Users.Where(a => a.UserName.ToUpper() == user.UserName.ToUpper()).FirstOrDefault();
     
                if (a != null)
                {
                    if (a.Code == user.Code)
                    {
                        //     a.Firebaseid = tokenfb;
                        a.PassWord = user.PassWord;
                        _context.Users.Update(a);

                        await _context.SaveChangesAsync();


                        //    var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
                        //     var authen = await auth.SignInWithEmailAndPasswordAsync(user.Email, user.PassWord);


                        //    UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(user.Email);
                        //  await FirebaseAuth.DefaultInstance.DeleteUserAsync(userRecord.Uid);

                        //  await auth.CreateUserWithEmailAndPasswordAsync(user.Email, user.PassWord);







                        return Ok(new { StatusCode = 200, Message = "Password change successfully" });//ok
                    }
                    else
                    {
                        return StatusCode(400, new { StatusCode = 400, message = "Password change failed (incorrect code)" });//ok
                    }
                }
                return StatusCode(400, new { StatusCode = 400, message = "Password changen failed (account does not exist)" });//ok
            }
            catch (Exception ex)
            {
                return StatusCode(409, new { StatusCode = 409, message = "Password changen failed (" + ex.Message + ")" });//ok
            }
        }




        [HttpPut("confirm")]
        public async Task<IActionResult> submitCode(Models.User user)
        {
            try
            {
                user.Email = addTailEmail(user.Email);
                if (!Validate.isEmail(user.Email))
                {
                    return StatusCode(409, new { StatusCode = 409, message = "Exception Email format" });//ok
                }
                var consu = GetAccount_byEmailConsultant(user.Email);
                var cus = GetAccount_byEmailCustomer(user.Email);
                var a = GetAccount_byEmail(user.Email);
                if (a != null)
                {
                    if (a.Status == "active")
                    {
                        return StatusCode(409, new { StatusCode = 409, message = "Account aldready active" });//ok
                    }
                    else
                    {
                        if (a.Code == user.Code)
                        {
                            a.Status = "active";
                            a.Email = user.Email;
                            _context.Users.Update(a);

                            if (consu != null)
                            {
                                consu.Status = "active";
                                consu.Email = user.Email;
                                _context.Consultants.Update(consu);
                                await _context.SaveChangesAsync();

                                return Ok(new { StatusCode = 200, Message = "Email verification successfully", idconsu = consu.Id});
                            }
                            if (cus != null)
                            {
                                cus.Status = "active";
                                cus.Email = user.Email;
                                _context.Customers.Update(cus);
                                await _context.SaveChangesAsync();

                                return Ok(new { StatusCode = 200, Message = "Email verification successfully", idcustomer = cus.Id });
                            }
                        
                            await _context.SaveChangesAsync();

                            return Ok(new { StatusCode = 200, Message = "Email verification successfully" });
                        }
                        else
                        {
                            return StatusCode(400, new { StatusCode = 400, message = "Email verification failed (incorrect code)" }); //ok
                        }
                    }
                }
                return StatusCode(400, new { StatusCode = 400, message = "Email verification failed (account does not exist)" });//ok
            }
            catch (Exception ex)
            {
                return StatusCode(409, new { StatusCode = 409, message = "Account registered failed (" + ex.Message + ")" });//ok
            }
        }

        [HttpPut("change-pass")] //email, passhash, code   ok
        public async Task<IActionResult> changePass(Models.User user)
        {
            try
            {
             //   var username = _context.Users.Where(a => a.UserName == user.UserName).FirstOrDefault();
              


                user.Email = addTailEmail(user.Email);
                if (!Validate.isEmail(user.Email))
                {
                    return StatusCode(409, new { StatusCode = 409, message = "Exception Email format" });//ok
                }

                var a = GetAccount_byEmail(user.Email);
                 //   var auth = new Firebase.Auth.FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(apiKey));
                //     var authen = await auth.SignInWithEmailAndPasswordAsync(a.Email, a.PassWord);

              //  string tokenfb = authen.User.LocalId;
             //   await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(tokenfb);

                if (a != null)
                {
                    if (a.Code == user.Code)
                    {
                   //     a.Firebaseid = tokenfb;
                        a.PassWord = user.PassWord;
                        _context.Users.Update(a);       
                        
                        await _context.SaveChangesAsync();


                   //    var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
                        //     var authen = await auth.SignInWithEmailAndPasswordAsync(user.Email, user.PassWord);


                        //    UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(user.Email);
                        //  await FirebaseAuth.DefaultInstance.DeleteUserAsync(userRecord.Uid);

                        //  await auth.CreateUserWithEmailAndPasswordAsync(user.Email, user.PassWord);

                    
                        

                       


                        return Ok(new { StatusCode = 200, Message = "Password change successfully" });//ok
                    }
                    else
                    {
                        return StatusCode(400, new { StatusCode = 400, message = "Password change failed (incorrect code)" });//ok
                    }
                }
                return StatusCode(400, new { StatusCode = 400, message = "Password changen failed (account does not exist)" });//ok
            }
            catch (Exception ex)
            {
                return StatusCode(409, new { StatusCode = 409, message = "Password changen failed (" + ex.Message + ")" });//ok
            }
        }





        //send Email(useraccount) //-Ta
        private async Task<bool> sendCodeEmail(Models.User a)
        {
            try
            {
                a.Email = addTailEmail(a.Email);
                if (!Validate.isEmail(a.Email))
                {
                    return false;
                }
                a.Code = randomGenerator.Next(1000, 9999).ToString(); //moi lan resend la reset code
                _context.Users.Update(a);
                await _context.SaveChangesAsync();

                var mailRequest = new MailRequest();
                mailRequest.ToEmail = a.Email;
                var username = mailRequest.ToEmail.Split('@')[0];
                mailRequest.Subject = "Xin chào  " + username + ", Chào mừng đến với Psychological Counseling App";
                mailRequest.Description = "Mã xác thực của bạn là: " +"    " + a.Code;
                mailRequest.Value = a.Code;
                await _mailService.SendEmailAsync(mailRequest.ToEmail,mailRequest.Subject,mailRequest.Description,mailRequest.Value);
                Console.WriteLine(mailRequest.ToEmail);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
      




        // Get account by(email) //-Ta
        private Models.User GetAccount_byEmail(string email)
        {
            var account = _context.Users.Where(a => a.Email.ToUpper() == email.ToUpper()).FirstOrDefault();

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

        private Models.Customer GetAccount_byEmailCustomer(string email)
        {
            var account = _context.Customers.Where(a => a.Email.ToUpper() == email.ToUpper()).FirstOrDefault();

            if (account == null)
            {
                return null;
            }

            return account;
        }

       /* private Models.Customer GetAccount_byUsernameCustomer(string username)
        {
            var account = _context.Customers.Where(a => a.Us.ToUpper() == email.ToUpper()).FirstOrDefault();

            if (account == null)
            {
                return null;
            }

            return account;
        }*/


        //-Ta
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
