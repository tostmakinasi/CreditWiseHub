using CreditWiseHub.BackgroundJob.Managers.RecurringJobs;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.BackgroundJob.Schedules
{
    public static class RecurringJobs
    {

        [Obsolete]
        public static void AutomaticPatmentOperation()
        {
            RecurringJob.RemoveIfExists(nameof(AutomaticPaymentsScheduleJobsManager));
            RecurringJob.AddOrUpdate<AutomaticPaymentsScheduleJobsManager>(nameof(AutomaticPaymentsScheduleJobsManager),
                job => job.Process(), Cron.Daily(23), TimeZoneInfo.Local);
        }
    }
}
