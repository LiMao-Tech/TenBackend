using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TenBackend.Models.Entities;
using TenBackend.Models.PDL;
using TenBackend.Models.Tools;

namespace TenBackend.Controllers
{
    public class ValidateController : Controller
    {
        private TenBackendDbContext db = new TenBackendDbContext();

        //
        // GET: /Validate/
        public ActionResult Index()
        {
            return View();
        }


        

        public ActionResult Email(string email, string validate)
        {

            BindInfo bindinfo = db.BindInfoes.Where(e => e.EmailAddress == email).FirstOrDefault();
            if (bindinfo == null)
            {
                ViewBag.message = "验证失败，您的链接存在问题！！！！";
                return View();
            }

            if (bindinfo.EmailState == true)
            {
                ViewBag.message = "此邮箱已绑定！！！";
                return View();
            }

            //验证过期
            if (TimeUtils.UnixTimestampToDateTime(new DateTime(), bindinfo.EmailTime).AddHours(24).Ticks < DateTime.UtcNow.Ticks)
            {
                ViewBag.message = "您的验证已过期，请重新申请绑定！！！";
                return View();
                
            }

            //验证验证字符串
            if (bindinfo.ValidateStr != validate)
            {
                ViewBag.message = "验证失败，您的链接存在问题！！！！";
                return View();
            }

            bindinfo.EmailState = true;
            db.Entry(bindinfo).State = EntityState.Modified;
            db.SaveChanges();

            ViewBag.message = "恭喜您，绑定Email成功！！！！";

            return View();
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