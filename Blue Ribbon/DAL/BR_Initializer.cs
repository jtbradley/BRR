using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Blue_Ribbon.Models;
using Blue_Ribbon.AmazonAPI;
using Blue_Ribbon.Controllers;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace Blue_Ribbon.DAL
{
    public class BR_Initializer : System.Data.Entity. DropCreateDatabaseIfModelChanges<BRContext>
    {

        private ApplicationDbContext appcontext = new ApplicationDbContext();

        protected override void Seed(BRContext context)
        {


            //var customers = new List<Customer>
            //{
            //new Customer{CustomerID="A38V65TBGRO6GP",FirstName = "Tech",LastName="Academy",JoinDate=DateTime.Parse("2015-03-22")},
            //new Customer{CustomerID="A3HHUC6WR3X1C5",FirstName = "Nancy",LastName="Dunston",JoinDate=DateTime.Parse("2015-02-04")},
            //new Customer{CustomerID="A1XAG9YCYKPFMK",FirstName = "Carlos",LastName="Simpson",JoinDate=DateTime.Parse("2014-11-09")},
            //new Customer{CustomerID="A1YPSAU696NYA4",FirstName = "Olivia",LastName="Gray",JoinDate=DateTime.Parse("2014-12-29")},
            //new Customer{CustomerID="A3TB4WX2TOREEV",FirstName = "Rotty",LastName="Folger",JoinDate=DateTime.Parse("2015-01-18")},
            //new Customer{CustomerID="A4ZHC3QDRSX7F",FirstName = "Montez",LastName="Williams",JoinDate=DateTime.Parse("2015-10-21")},
            //new Customer{CustomerID="A1SLX7WOPZZUVF",FirstName = "Sandra",LastName="Cruz",JoinDate=DateTime.Parse("2015-06-01")},
            //new Customer{CustomerID="ABSDQ8NXYWQ1Z",FirstName = "Bertha",LastName="Donaldson",JoinDate=DateTime.Parse("2014-03-03")},
            //};

            //customers.ForEach(s => context.Customers.Add(s));
            //context.SaveChanges();

            //var vendors = new List<Vendor>
            //{
            //    new Vendor {Name="Acme Company",JoinDate=DateTime.Parse("2014-03-03") },
            //    new Vendor {Name="Junk & Junk",JoinDate=DateTime.Parse("2014-03-03") },
            //    new Vendor {Name="Awesome Stuff, Inc" ,JoinDate=DateTime.Parse("2014-03-03")},
            //    new Vendor {Name="Oye, Papi!" ,JoinDate=DateTime.Parse("2014-03-03")},
            //    new Vendor {Name="Ha Ha Industries" ,JoinDate=DateTime.Parse("2014-03-03")},
            //    new Vendor {Name="Blammo, Inc." ,JoinDate=DateTime.Parse("2014-03-03")},
            //    new Vendor {Name="Blasio De Oro" ,JoinDate=DateTime.Parse("2014-03-03")},
            //    new Vendor {Name="Red Ribbon" ,JoinDate=DateTime.Parse("2014-03-03")},
            //    new Vendor {Name="Future, Inc" ,JoinDate=DateTime.Parse("2014-03-03")},
            //    new Vendor {Name="Whammo, Inc." ,JoinDate=DateTime.Parse("2014-03-03")},

            //};

            ////NOte: dynamic population of vendor Name is not easy through Amazon API, so name must be manually Set when adding Vendor.

            //vendors.ForEach(s => context.Vendors.Add(s));
            //context.SaveChanges();

            //var campaigns = new List<Campaign>
            //{
            //new Campaign{CampaignID = 1, ASIN="B002LMN7B4",VendorID="3029864011", SalePrice="2.75"},
            //new Campaign{CampaignID = 2, ASIN="B00AEI58HI",VendorID="2601925011", SalePrice="2.55"},
            //new Campaign{CampaignID = 3, ASIN="B00DZITYGU",VendorID="2590609011", SalePrice="7.14"},
            //new Campaign{CampaignID = 4, ASIN="B00E9Y6QLA",VendorID="6126709011", SalePrice="6.71"},
            //new Campaign{CampaignID = 5, ASIN="B0156MGYAQ",VendorID="7141123011", SalePrice="12.45"},
            //new Campaign{CampaignID = 6, ASIN="B00NH11X64",VendorID="2528919011", SalePrice="5.90"},
            //new Campaign{CampaignID = 7, ASIN="B004G9L1H2",VendorID="2586938011", SalePrice="6.43"},
            //new Campaign{CampaignID = 8, ASIN="B00MG5RU4G",VendorID="2592600011", SalePrice="18.23"},
            //new Campaign{CampaignID = 9, ASIN="B00UR0EFEK",VendorID="3110398011", SalePrice="9.07"},
            //new Campaign{CampaignID = 10, ASIN="B0009V1WSE",VendorID="2597530011", SalePrice="2.02"}

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
            //    data.Add( item.ASIN, attributes);

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

            //var reviews = new List<Review>
            //{
            //new Review{CampaignID=1,CustomerID="A38V65TBGRO6GP",SelectedDate=DateTime.Parse("2015-09-25"), ProductRating=null, ReviewDate=null},
            //new Review{CampaignID=2,CustomerID="A3HHUC6WR3X1C5",SelectedDate=DateTime.Parse("2015-09-23"), ProductRating=null,ReviewDate=null},
            //new Review{CampaignID=3,CustomerID="A1XAG9YCYKPFMK",SelectedDate=DateTime.Parse("2015-09-13"), ProductRating=null,ReviewDate=null},
            //new Review{CampaignID=4,CustomerID="A1YPSAU696NYA4",SelectedDate=DateTime.Parse("2015-10-03"), ProductRating=null,ReviewDate=null},
            //new Review{CampaignID=5,CustomerID="A3TB4WX2TOREEV",SelectedDate=DateTime.Parse("2015-10-10"), ProductRating=null,ReviewDate=null},
            //new Review{CampaignID=6,CustomerID="A4ZHC3QDRSX7F",SelectedDate=DateTime.Parse("2015-09-28"), Reviewed = true, ProductRating=5,ReviewDate=DateTime.Parse("2015-10-05")},
            //new Review{CampaignID=7,CustomerID="A1SLX7WOPZZUVF",SelectedDate=DateTime.Parse("2015-08-13"), ProductRating=null,ReviewDate=null},
            //new Review{CampaignID=7,CustomerID="ABSDQ8NXYWQ1Z",SelectedDate=DateTime.Parse("2015-07-31"), ProductRating=null,ReviewDate=null},
            //new Review{CampaignID=3,CustomerID="ABSDQ8NXYWQ1Z",SelectedDate=DateTime.Parse("2015-08-28"), ProductRating=null,ReviewDate=null},
            //new Review{CampaignID=8,CustomerID="A38V65TBGRO6GP",SelectedDate=DateTime.Parse("2015-09-28"), ProductRating=null,ReviewDate=null},
            //new Review{CampaignID=9,CustomerID="A38V65TBGRO6GP",SelectedDate=DateTime.Parse("2015-07-28"), ProductRating=null,ReviewDate=null},
            //new Review{CampaignID=3,CustomerID="A38V65TBGRO6GP",SelectedDate=DateTime.Parse("2015-08-28"), ProductRating=null,ReviewDate=null},
            //new Review{CampaignID=8,CustomerID="A4ZHC3QDRSX7F",SelectedDate=DateTime.Parse("2015-09-03"), Reviewed = true, ProductRating=5,ReviewDate=DateTime.Parse("2015-09-25")},
            //};
            //reviews.ForEach(s => context.Reviews.Add(s));
            //context.SaveChanges();

            //var requests = new List<ItemRequest>
            //{
            //    new ItemRequest {CampaignID=1,CustomerID="A3HHUC6WR3X1C5",RequestDate=DateTime.Parse("2015-09-25"),ReviewType = ReviewType.Text },
            //    new ItemRequest {CampaignID=2,CustomerID="ABSDQ8NXYWQ1Z",RequestDate=DateTime.Parse("2015-09-25"),ReviewType = ReviewType.Text },
            //    new ItemRequest {CampaignID=8,CustomerID="A1YPSAU696NYA4",RequestDate=DateTime.Parse("2015-09-25"),ReviewType = ReviewType.Text },
            //    new ItemRequest {CampaignID=4,CustomerID="A1XAG9YCYKPFMK",RequestDate=DateTime.Parse("2015-09-25"),ReviewType = ReviewType.Text },
            //    new ItemRequest {CampaignID=5,CustomerID="ABSDQ8NXYWQ1Z",RequestDate=DateTime.Parse("2015-09-25"),ReviewType = ReviewType.Text },
            //    new ItemRequest {CampaignID=6,CustomerID="A3HHUC6WR3X1C5",RequestDate=DateTime.Parse("2015-09-25"),ReviewType = ReviewType.Text },
            //    new ItemRequest {CampaignID=1,CustomerID="ABSDQ8NXYWQ1Z",RequestDate=DateTime.Parse("2015-09-25"),ReviewType = ReviewType.Text },
            //    new ItemRequest {CampaignID=3,CustomerID="A1SLX7WOPZZUVF",RequestDate=DateTime.Parse("2015-09-25"),ReviewType = ReviewType.Text },
            //    new ItemRequest {CampaignID=9,CustomerID="A1XAG9YCYKPFMK",RequestDate=DateTime.Parse("2015-09-25"),ReviewType = ReviewType.Text },
            //    new ItemRequest {CampaignID=3,CustomerID="A1YPSAU696NYA4",RequestDate=DateTime.Parse("2015-09-25"),ReviewType = ReviewType.Text },
            //};
            //requests.ForEach(s => context.ItemRequests.Add(s));
            //context.SaveChanges();

            if (!appcontext.Roles.Any(r => r.Name == "customer"))
            {
                var store = new RoleStore<IdentityRole>(appcontext);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "customer" };

                manager.Create(role);
            }

            if (!appcontext.Roles.Any(r => r.Name == "campaignManager"))
            {
                var store = new RoleStore<IdentityRole>(appcontext);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "campaignManager" };

                manager.Create(role);
            }


        }

    }
}