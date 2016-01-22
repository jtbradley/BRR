using Blue_Ribbon.AmazonAPI;
using Blue_Ribbon.DAL;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Blue_Ribbon.ScheduledTasks
{
    public class ReviewsChecker : ITask, IRegisteredObject
    {
        private readonly object _lock = new object();

        private bool _shuttingDown;

        private BRContext db = new BRContext();

        public void CheckAll()
        {
            // Register this task with the hosting environment.
            // Allows for a more graceful stop of the task, in the case of IIS shutting down.
            HostingEnvironment.RegisterObject(this);

        }

        public void Execute()
        {
            lock (_lock)
            {
                if (_shuttingDown)
                    return;

                CheckForCompletedReviews checkall = new CheckForCompletedReviews();
                checkall.Check();
                TaskLog logItem = new TaskLog();
                logItem.SuccessDatestamp = DateTime.Now;
                logItem.TaskDescription = "AllReviewsChecked";
                db.TaskLog.Add(logItem);
                db.SaveChanges();
            }
        }

        public void Stop(bool immediate)
        {
            // Locking here will wait for the lock in Execute to be released until this code can continue.
            lock (_lock)
            {
                _shuttingDown = true;
            }

            HostingEnvironment.UnregisterObject(this);
        }
    }

    public class DailyLimitsChecker : ITask, IRegisteredObject
    {
        private readonly object _lock = new object();

        private bool _shuttingDown;

        private BRContext db = new BRContext();

        public void CheckAll()
        {
            // Register this task with the hosting environment.
            // Allows for a more graceful stop of the task, in the case of IIS shutting down.
            HostingEnvironment.RegisterObject(this);

        }

        public void Execute()
        {
            lock (_lock)
            {
                if (_shuttingDown)
                    return;

                //Instead of just blindly resetting all campaigns for the day, we're going to check the number of reviews.
                //Why? Incase the server times out, this task will run on start.
                var campaignsToReset = from c in db.Campaigns
                                       where c.OpenCampaign.Equals(true)
                                       where c.DailyLimitReached.Equals(true)
                                       select c;

                DateTime today = DateTime.Now.Date;
                foreach (var camp in campaignsToReset.ToList())
                {
                    int todaysCount = (from c in db.Reviews
                                       where c.CampaignID.Equals(camp.CampaignID)
                                       where c.SelectedDate > today
                                       select c).Count();

                    if (todaysCount < camp.DailyLimit && camp.DailyLimit > 0)
                    {
                        camp.DailyLimitReached = false;
                        db.Entry(camp).State = EntityState.Modified;
                    }
                }
                db.SaveChanges();

                TaskLog logItem = new TaskLog();
                logItem.SuccessDatestamp = DateTime.Now;
                logItem.TaskDescription = "DailyLimitsReset";
                db.TaskLog.Add(logItem);
                db.SaveChanges();

            }
        }

        public void Stop(bool immediate)
        {
            // Locking here will wait for the lock in Execute to be released until this code can continue.
            lock (_lock)
            {
                _shuttingDown = true;
            }

            HostingEnvironment.UnregisterObject(this);
        }
    }
}