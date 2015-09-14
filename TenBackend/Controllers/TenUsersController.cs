using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TenBackend.Models;
using System.IO;
using Newtonsoft.Json;

namespace TenBackend.Controllers
{
    public class TenUsersController : Controller
    {
        private TenUserDbContext db = new TenUserDbContext();

        // GET: TenUsers
        public async Task<ActionResult> Index()
        {
            return View(await db.TenUsers.ToListAsync());
        }

        // GET: TenUsers.QueryLocationByID
        [HttpGet]
        public string QueryLocationByID(int? id)
        {
            TenUser tu = db.TenUsers.Find(id);
            Dictionary<string, string> values = new Dictionary<string,string>();
            values["latitude"] = tu.latitude.ToString();
            values["longitude"] = tu.longitude.ToString();
            return JsonConvert.SerializeObject(values);
        }

        [HttpPost]
        public string UpdateLocationByID()
        {
            string input = new StreamReader(Request.InputStream).ReadToEnd();
            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(input);
            TenUser tu = db.TenUsers.Find(Convert.ToInt32(values["id"]));
            
            tu.latitude = Convert.ToDouble(values["latitude"]);
            tu.longitude = Convert.ToDouble(values["longitude"]);
            db.SaveChanges();
            return "SUCCESS";
        }

        [HttpPost]
        public string DummyPost()
        {
            string input = new StreamReader(Request.InputStream).ReadToEnd();
            
            return input;
        }

        // GET: TenUsers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenUser tenUser = await db.TenUsers.FindAsync(id);
            if (tenUser == null)
            {
                return HttpNotFound();
            }
            return View(tenUser);
        }

        // GET: TenUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TenUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "user_index,user_id,user_name,gender,birth_date,joined_date,last_login_datetime,p_coin,outer_score,inner_score,energy,quote,latitude,longitude")] TenUser tenUser)
        {
            if (ModelState.IsValid)
            {
                db.TenUsers.Add(tenUser);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tenUser);
        }

        // GET: TenUsers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenUser tenUser = await db.TenUsers.FindAsync(id);
            if (tenUser == null)
            {
                return HttpNotFound();
            }
            return View(tenUser);
        }

        // POST: TenUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "user_index,user_id,user_name,gender,birth_date,joined_date,last_login_datetime,p_coin,outer_score,inner_score,energy,quote,latitude,longitude")] TenUser tenUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tenUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tenUser);
        }

        // GET: TenUsers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenUser tenUser = await db.TenUsers.FindAsync(id);
            if (tenUser == null)
            {
                return HttpNotFound();
            }
            return View(tenUser);
        }

        // POST: TenUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TenUser tenUser = await db.TenUsers.FindAsync(id);
            db.TenUsers.Remove(tenUser);
            await db.SaveChangesAsync();
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
