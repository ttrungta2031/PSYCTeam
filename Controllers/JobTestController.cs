using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PsychologicalCounseling.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PsychologicalCounseling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobTestController : ControllerBase
    {
        private readonly IJobTestService _jobTestService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;
        public JobTestController(IJobTestService jobTestService, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _jobTestService = jobTestService;
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }
        [HttpGet("/autoinactiveslotactive")]
        [Authorize(Roles = "admin")]
        public ActionResult CreateFireAndForgetJob()
        {
            _recurringJobManager.AddOrUpdate("autoinactiveslotactive", () => _jobTestService.AutoInactiveSlotActive(), Cron.Minutely);
            return Ok();
        }


        [HttpGet("/autooverdueslotbooked")]
        [Authorize(Roles = "admin")]
        public ActionResult AutoOverDueSlotBooked()
        {
            _recurringJobManager.AddOrUpdate("autooverdueslotbooked", () => _jobTestService.AutoOverDueSlotBooked(), Cron.Minutely);
            return Ok();
        }


        [HttpGet("/autosendnoticall")]
        [Authorize(Roles = "admin")]
        public ActionResult AutoSendNotiCall()
        {
            _recurringJobManager.AddOrUpdate("autosendnoticall", () => _jobTestService.AutoSendNotiCall(), Cron.Minutely);
            return Ok();
        }
    }
}
