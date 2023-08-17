using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PsychologicalCounseling.Services
{
    public class DashboardAuthorization : IDashboardAuthorizationFilter
    {
        public IEnumerable<HangfireUserCredentials> Users { get; }

        public DashboardAuthorization(IEnumerable<HangfireUserCredentials> users)
        {
            Users = users;
        }

        public bool Authorize(DashboardContext dashboardContext)
        {
            var context = dashboardContext.GetHttpContext();

            string header = context.Request.Headers["Authorization"];

            if (!string.IsNullOrWhiteSpace(header))
            {
                AuthenticationHeaderValue authValues = AuthenticationHeaderValue.Parse(header);

                if ("Basic".Equals(authValues.Scheme, StringComparison.InvariantCultureIgnoreCase))
                {
                    string parameter = Encoding.UTF8.GetString(Convert.FromBase64String(authValues.Parameter));
                    var parts = parameter.Split(':');

                    if (parts.Length > 1)
                    {
                        string username = parts[0];
                        string password = parts[1];

                        if ((!string.IsNullOrWhiteSpace(username)) && (!string.IsNullOrWhiteSpace(password)))
                        {
                            return Users.Any(user => user.ValidateUser(username, password)) || Challenge(context);
                        }
                    }
                }
            }

            return Challenge(context);
        }

        private bool Challenge(HttpContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");

            context.Response.WriteAsync("Authentication is required.");

            return false;
        }
        public class HangfireUserCredentials
        {
            public string Username { get; set; }
            public byte[] PasswordSha1Hash { get; set; }


            public string Password
            {
                set
                {
                    using (var cryptoProvider = SHA1.Create())
                    {
                        PasswordSha1Hash = cryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(value));
                    }
                }
            }

            public bool ValidateUser(string username, string password)
            {
                if (string.IsNullOrWhiteSpace(username))
                    throw new ArgumentNullException("login");

                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentNullException("password");

                if (username == Username)
                {
                    using (var cryptoProvider = SHA1.Create())
                    {
                        byte[] passwordHash = cryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(password));
                        return StructuralComparisons.StructuralEqualityComparer.Equals(passwordHash, PasswordSha1Hash);
                    }
                }
                else
                    return false;
            }
        }
    }
}
