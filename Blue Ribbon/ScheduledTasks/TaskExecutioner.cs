using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentScheduler;

namespace Blue_Ribbon.ScheduledTasks
{
    public class TaskExecutioner : Registry
    {
        public TaskExecutioner()
        {

            Schedule<DailyLimitsChecker>().ToRunNow();

            //Check all open reviews at 6:30 AM Pacific time. Updates Review count.
            Schedule<ReviewsChecker>().ToRunEvery(1).Days().At(06, 30);

            //Reset campaigns with daily limits at Midnight:30.
            Schedule<DailyLimitsChecker>().ToRunEvery(1).Days().At(00, 30);

            //Check all open reviews at 8:30 PM Pacific time. Updates Review count.
            Schedule<ReviewsChecker>().ToRunEvery(1).Days().At(20, 30);
        }
    }
}