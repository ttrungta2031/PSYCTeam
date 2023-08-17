using System.Threading.Tasks;
using PsychologicalCounseling.Models;

namespace PsychologicalCounseling.Services
{
    public interface IMailService 
    {
        Task SendCodeEmailAsync(MailRequest mailRequest);
        Task SendEmailAsync(string email, string subject, string htmlMessage, string value);
    }
}
