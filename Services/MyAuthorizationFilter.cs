using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;


namespace PsychologicalCounseling.Services
{
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();        
            // Allow all authenticated users to see the Dashboard (potentially dangerous). 
           
            if (httpContext.Session?.GetString("Admin")=="HangFire2810" )
            {
                return true;
            }
            return false;
        }
    }
}
