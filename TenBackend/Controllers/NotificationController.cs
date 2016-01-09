using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TenBackend.Models.Entities;
using TenBackend.Models.Assitants;
using TenBackend.Models.PDL;
using TenBackend.Models.Tools.PushHelpers;

namespace TenBackend.Controllers
{
    public class NotificationController : Controller
    {
        private TenBackendDbContext db = new TenBackendDbContext();

        // GET: /Notification/
        public ActionResult Index()
        {

            return View(db.TenMsgs.Where(msg => msg.MsgType == 0 && msg.Sender == 0).ToList());
        }

        // GET: /Notification/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenMsg tenmsg = db.TenMsgs.Find(id);
            if (tenmsg == null)
            {
                return HttpNotFound();
            }
            return View(tenmsg);
        }

        // GET: /Notification/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Notification/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="MsgIndex,PhoneType,MsgTime,MsgContent")] TenMsg tenmsg)
        {
            if (ModelState.IsValid)
            {
                tenmsg.Sender = 0;
                tenmsg.Receiver = 0;
                tenmsg.MsgType = 0;
                tenmsg.IsLocked = false;
                tenmsg.MsgTime = DateTime.Now;

                db.TenMsgs.Add(tenmsg);
                db.SaveChanges();

                    foreach (TenUser u in db.TenUsers.Where(e => e.PhoneType == 0))
                    {
                        TenLogin tenlogin = db.TenLogins.Where(e => e.UserIndex == u.UserIndex).FirstOrDefault();
                        TenPushBroker.GetInstance().SendNotification2Apple(tenlogin.DeviceToken, tenmsg.MsgContent);
                    }
                   
                return RedirectToAction("Index");
            }

            return View(tenmsg);
        }

        // GET: /Notification/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenMsg tenmsg = db.TenMsgs.Find(id);
            if (tenmsg == null)
            {
                return HttpNotFound();
            }
            return View(tenmsg);
        }

        // POST: /Notification/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="MsgIndex,Sender,Receiver,PhoneType,IsLocked,MsgType,MsgTime,MsgContent")] TenMsg tenmsg)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tenmsg).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tenmsg);
        }

        // GET: /Notification/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenMsg tenmsg = db.TenMsgs.Find(id);
            if (tenmsg == null)
            {
                return HttpNotFound();
            }
            return View(tenmsg);
        }

        // POST: /Notification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TenMsg tenmsg = db.TenMsgs.Find(id);
            db.TenMsgs.Remove(tenmsg);
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
