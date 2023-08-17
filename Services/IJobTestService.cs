using System.Threading.Tasks;

namespace PsychologicalCounseling.Services
{
    public interface IJobTestService
    {
        void ContinuationJob();
        void DelayedJob();
         Task AutoInactiveSlotActive();
        void ReccuringJob();
        Task AutoSendNotiCall();
        Task AutoOverDueSlotBooked();
    }
}