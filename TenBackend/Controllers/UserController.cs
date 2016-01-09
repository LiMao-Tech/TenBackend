using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TenBackend.Models.Entities;
using TenBackend.Models.Assitants;
using TenBackend.Models.PDL;
using TenBackend.Models.Tools.PushHelpers;

namespace TenBackend.Controllers
{
    public class UserController : Controller
    {
        private TenBackendDbContext db = new TenBackendDbContext();

        // GET: /User/
        public ActionResult Index()
        {
            return View(db.TenLogins.ToList());
        }

        public ActionResult ShowPhotos(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IQueryable<TenImage> tenimages = db.TenImages.Where(img => img.UserIndex == id && img.ImageType == ImageType.Photo);

            return View(tenimages);
        }

        // GET: /User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenLogin tenlogin = db.TenLogins.Find(id);


            if (tenlogin == null)
            {
                return HttpNotFound();
            }

            TenUser tenUser = db.TenUsers.Find(tenlogin.UserIndex);
            if (tenUser == null)
            {
                return HttpNotFound("No tenUser details, it is a invalid account!");
            }

            IQueryable<TenImage> tenImages = db.TenImages.Where(i => i.UserIndex == tenlogin.UserIndex);

            if (tenImages == null)
            {
                return HttpNotFound("No profile image, it is invalid account!");
            }

            UserDetails details = new UserDetails();
            details.TenLogin = tenlogin;
            details.TenUser = tenUser;
            details.ProfileID = tenImages.Where(img => img.ImageType == ImageType.Profile).FirstOrDefault().ID;
            details.PhotoCount = tenImages.Where(img => img.ImageType == ImageType.Photo).ToList().Count;
            return View(details);
        }

        // GET: /User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="LoginIndex,UserIndex,UserID,UserPWD,LastLogin,DeviceUUID,DeviceToken,HashValue")] TenLogin tenlogin)
        {
            if (ModelState.IsValid)
            {
                db.TenLogins.Add(tenlogin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tenlogin);
        }

        // GET: /User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenLogin tenlogin = db.TenLogins.Find(id);
            if (tenlogin == null)
            {
                return HttpNotFound();
            }
            return View(tenlogin);
        }

        // POST: /User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="LoginIndex,UserIndex,UserID,UserPWD,LastLogin,DeviceUUID,DeviceToken,HashValue")] TenLogin tenlogin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tenlogin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tenlogin);
        }

        // GET: /User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenLogin tenlogin = db.TenLogins.Find(id);
            if (tenlogin == null)
            {
                return HttpNotFound();
            }
            return View(tenlogin);
        }

        // POST: /User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TenLogin tenlogin = db.TenLogins.Find(id);
            db.TenLogins.Remove(tenlogin);

            TenUser tenuser = db.TenUsers.Find(tenlogin.UserIndex);
            if (tenuser != null)
            {
                db.TenUsers.Remove(tenuser);
            }

            IQueryable<TenImage> tenimages = db.TenImages.Where(img => img.UserIndex == tenlogin.UserIndex);
            if (tenimages != null)
            {
                foreach (TenImage image in tenimages)
                {
                    string path = Path.Combine(image.BasePath, image.FileName);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    db.TenImages.Remove(image);
                }
            }

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
