using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PsychologicalCounseling.Services
{

  
    public class SendNotiService : ISendNotiService
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;
        //  private readonly PsychologicalCouselingContext _context;
        //   private readonly IUserService _userService;
        private static string apiKey = "AIzaSyD5NIvb5a4ZsEwnSYAII5803RgSSHdVn14";

        private static string Bucket = "psycteamv1.appspot.com";
        private static string AuthEmail = "tuanninh655@gmail.com";
        private static string AuthPassword = "bivsan1";


        public async Task Sendmessagefcm(string title, string body, string fcmToken)
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
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                }
            };

           await FirebaseMessaging.DefaultInstance.SendAsync(message);
          //  if (res != "") Console.WriteLine(" Thành công");
          //  else Console.WriteLine("Không Thành công");

        }


    }



}
