using System.Threading.Tasks;
using PsychologicalCounseling.Models;

namespace PsychologicalCounseling.Services
{
    public interface ISendNotiService
    {
        Task Sendmessagefcm(string title, string body, string fcmToken);
    }
}
