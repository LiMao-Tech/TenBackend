using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TenBackend.Models.PDL;

namespace TenBackend.Controllers
{
    public class AdminController : Controller
    {

        private TenBackendDbContext db = new TenBackendDbContext();

        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(string username, string userpsd)
        {

            return Json(db.Users);
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