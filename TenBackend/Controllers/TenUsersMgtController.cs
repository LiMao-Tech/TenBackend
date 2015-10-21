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

using TenBackend.Models.DbContexts;


namespace TenBackend.Controllers
{
    public class TenUsersMgtController : Controller
    {
        private TenUserDbContext db = new TenUserDbContext();

        // GET: TenUsers
        public async Task<ActionResult> Index()
        {
            return View(await db.TenUsers.ToListAsync());
        }

        [HttpGet]
        public string QueryLocationByCoordinates(double? latiMin, double? latiMax, double? longiMin, double? longiMax)
        {
            IQueryable<TenUser> tus = db.TenUsers.Where(x => x.latitude >= latiMin && x.latitude <= latiMax && x.longitude >= longiMin && x.longitude <= longiMax);

            List<Dictionary<string, string>> retList = new List<Dictionary<string, string>>();
            foreach (TenUser tu in tus)
            {
                Dictionary<string, string> values = new Dictionary<string, string>();

                values["user_index"] = tu.user_index.ToString();
                values["user_id"] = tu.user_id;
                values["user_name"] = tu.user_name;
                values["gender"] = tu.gender.ToString();
                values["birth_date"] = tu.birth_date.ToString();
                values["joined_date"] = tu.joined_date.ToString();
                values["last_login_datetime"] = tu.last_login_datetime.ToString();
                values["p_coin"] = tu.p_coin.ToString();
                values["outer_score"] = tu.outer_score.ToString();
                values["inner_score"] = tu.inner_score.ToString();
                values["energy"] = tu.energy.ToString();
                values["quote"] = tu.quote;
                values["latitude"] = tu.latitude.ToString();
                values["longitude"] = tu.longitude.ToString();

                retList.Add(values);
            }
            return JsonConvert.SerializeObject(retList);
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
            Dictionary<string, string> ret = new Dictionary<string,string>();
            ret["status"] = "Sucess";
            string retJson = JsonConvert.SerializeObject(ret);
            return retJson;
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
