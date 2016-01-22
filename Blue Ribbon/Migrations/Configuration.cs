namespace Blue_Ribbon.Migrations
{
    using AmazonAPI;
    using DAL;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal sealed class Configuration : DbMigrationsConfiguration<Blue_Ribbon.DAL.BRContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        private ApplicationDbContext appcontext = new ApplicationDbContext();
        private BRContext db = new BRContext();


        protected override void Seed(Blue_Ribbon.DAL.BRContext context)
        {
            //var vendors = new List<Vendor>
            //{
            //    new Vendor {VendorId=100,Name="Acme Company",JoinDate=DateTime.Parse("2015-03-03") },

            //};

            ////NOte: dynamic population of vendor Name is not easy through Amazon API, so name must be manually Set when adding Vendor.

            //vendors.ForEach(s => context.Vendors.Add(s));
            //context.SaveChanges();

            //var campaigns = new List<Campaign>
            //{
            //new Campaign{CampaignID = 2, ASIN="B00AEI58HI",VendorID=1, SalePrice="2.55"},
            //new Campaign{CampaignID = 3, ASIN="B00DZITYGU",VendorID=1, SalePrice="7.14"},
            //new Campaign{CampaignID = 4, ASIN="B00E9Y6QLA",VendorID=1, SalePrice="6.71"}
            //};
            //Dictionary<string, List<dynamic>> data = new Dictionary<string, List<dynamic>> { };
            //List<string> ids = new List<string> { };
            //foreach (var prod in campaigns)
            //{
            //    ids.Add(prod.ASIN);
            //}
            //LookupByASIN generateData = new LookupByASIN(ids.ToArray());
            //ItemLookupResponse response = generateData.GetData();

            //foreach (var item in response.Items[0].Item)
            //{
            //    List<dynamic> attributes = new List<dynamic>();
            //    attributes.Add(item.ItemAttributes.Title);
            //    attributes.Add(item.ItemAttributes.Binding);
            //    attributes.Add(item.LargeImage.URL);
            //    string description = "Sorry, no description available.";

            //    if (item.EditorialReviews != null)
            //    {
            //        description = item.EditorialReviews[0].Content;
            //        description = Regex.Replace(description, @"<[^>]+>|&nbsp;", "").Trim();
            //    }
            //    attributes.Add(description);
            //    string rawprice = item.Offers.Offer[0].OfferListing[0].Price.Amount;
            //    rawprice = (double.Parse(rawprice) / 100).ToString();
            //    attributes.Add(rawprice);
            //    data.Add(item.ASIN, attributes);

            //}
            //foreach (var prod in campaigns)
            //{
            //    List<dynamic> itemattributes = data[prod.ASIN];
            //    prod.Name = itemattributes[0];
            //    prod.Category = itemattributes[1];
            //    prod.ImageUrl = itemattributes[2];
            //    prod.Description = itemattributes[3];
            //    prod.RetailPrice = itemattributes[4];
            //    prod.SetNumericalPrices();
            //}

            //campaigns.ForEach(s => context.Campaigns.Add(s));
            //context.SaveChanges();


            //if (!appcontext.Roles.Any(r => r.Name == "customer"))
            //{
            //    var store = new RoleStore<IdentityRole>(appcontext);
            //    var manager = new RoleManager<IdentityRole>(store);
            //    var role = new IdentityRole { Name = "customer" };

            //    manager.Create(role);
            //}

            //if (!appcontext.Roles.Any(r => r.Name == "campaignManager"))
            //{
            //    var store = new RoleStore<IdentityRole>(appcontext);
            //    var manager = new RoleManager<IdentityRole>(store);
            //    var role = new IdentityRole { Name = "campaignManager" };

            //    manager.Create(role);
            //}

        }
    }
}
