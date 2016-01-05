using PushSharp;
using PushSharp.Apple;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TenBackend.Models;
using TenBackend.Models.PDL;

namespace TenBackend.Controllers
{
    public class NotificationController : Controller
    {
        static string PUSH_CERTI_LOC = "./Resources/TenPushNotiDev.p12";
        static string PUSH_CERTI_PWD = "LiMao1234";

        private PushBroker m_pushBroker = new PushBroker();
        private Byte[] m_appleCerti = System.IO.File.ReadAllBytes(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PUSH_CERTI_LOC));

        private TenBackendDbContext db = new TenBackendDbContext();

        // GET: /Notification/
        public ActionResult Index()
        {

            return View(db.TenMsgs.Where(msg => msg.MsgType == 0).ToList());
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

                m_pushBroker.RegisterAppleService(new ApplePushChannelSettings(m_appleCerti, PUSH_CERTI_PWD));
                if (tenmsg.PhoneType == 0) // iPhone
                {
                    foreach (TenUser u in db.TenUsers.Where(e => e.PhoneType == 0))
                    {
                        TenLogin tenlogin = db.TenLogins.Where(e => e.UserIndex == u.UserIndex).FirstOrDefault();
                        m_pushBroker.QueueNotification(new AppleNotification()
                                               .ForDeviceToken(tenlogin.DeviceToken)
                                               .WithAlert(tenmsg.MsgContent)
                                               .WithBadge(7)
                                               .WithSound("sound.caf"));
                    }

                    m_pushBroker.StopAllServices();
                   
                }
                else if (tenmsg.PhoneType == 1) // Android
                {

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
