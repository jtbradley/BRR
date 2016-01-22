using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blue_Ribbon.DAL;
using Blue_Ribbon.Models;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Blue_Ribbon.ViewModels;

namespace Blue_Ribbon.Controllers
{
    [Authorize(Roles = "campaignManager")]
    public class CustomerController : Controller
    {
        private BRContext db = new BRContext();
        private ApplicationDbContext appdb = new ApplicationDbContext();


        // GET: Customer
        public ActionResult Index(bool showAll = false,string currentSearch = null)
        {
            //because there are so many customers, this index page takes too long to load.
            //By default I'm going to only show 50 customers. But a checkbox will allow to show all records
            //but will require longer load time.  

            CustomerIndexViewModel customers = new CustomerIndexViewModel(showAll, currentSearch);

            return View(customers);
        }

        // GET: Customer/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,FirstName,LastName,JoinDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer)
        {
            //public ActionResult Edit(Customer customer, bool idchanged, string newid)
            //DOES NOT ACTUALLY RESULT IN CHANGES CURRENTLY.
            //Sooooooooooooooo, this needs to go to account controller and update ApplicationUser primary key.
            //    But I'm reading that this needs to be done with SQL stored procedures. The code below results 
            //in concurrency error because you can't update record that doesn't exist.
            //also, we would need to update Reviews and Item Requests and UserRole on an ID change. Ugh.

            //if (idchanged){
            //    try
            //    {
            //        ParsedFeed profilecheck = new ParsedFeed(newid);
            //    }
            //    catch
            //    {
            //        ViewBag.Error = "There is a problem with that Amazon Profile ID. Please check and try again.";
            //        return View(customer);
            //    }
            //    bool check = new AccountController().EditAmazonID(customer.CustomerID, newid);
            //    customer.CustomerID = newid;
            //}

            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                ApplicationUser user = (from a in appdb.Users
                                        where a.CustomerID == customer.CustomerID
                                        select a).First();

                if (customer.Email != user.Email)
                {
                    bool check = new AccountController().EditEmail(customer.Email, customer.CustomerID);
                }
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Customer/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }


        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
