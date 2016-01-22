using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Blue_Ribbon.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using Blue_Ribbon.ScheduledTasks;

namespace Blue_Ribbon.DAL
{
    public class BRContext : DbContext
    {

        public BRContext() : base("BRContext")
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<ItemRequest> ItemRequests { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<PricingStructure> PricingStructure { get; set; }
        public DbSet<TaskLog> TaskLog { get; set; }
        public DbSet<NotificationLog> NotificationLog { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}