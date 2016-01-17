using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TenBackend.Models.Entities;
using TenBackend.Models.PDL;
using TenBackend.Models.Tools;

namespace TenBackend.Controllers
{
    public class BindInfoController : ApiController
    {
        private TenBackendDbContext db = new TenBackendDbContext();

        static double VALIDATE_THRESHOLD_MINUTE = 3;

        // GET api/BindInfo
        public IQueryable<BindInfo> GetBindInfoes()
        {
            return db.BindInfoes;
        }

        // GET api/BindInfo/5
        [ResponseType(typeof(BindInfo))]
        public IHttpActionResult GetBindInfo(int id)
        {
            BindInfo bindinfo = db.BindInfoes.Find(id);
            if (bindinfo == null)
            {
                return NotFound();
            }

            return Ok(bindinfo);
        }


        /*
        /// <summary>
        /// 验证绑定
        /// </summary>
        public IHttpActionResult GetBindInfo(string email,string validate)
        {
            BindInfo bindinfo = db.BindInfoes.Where(e => e.EmailAddress == email).FirstOrDefault();
            if (bindinfo == null)
            {
                return NotFound();
            }

            //验证过期
            if (bindinfo.EmailTime.AddHours(24) > DateTime.Now)
            {
                return Ok("您的验证已过期，请重新申请绑定！！！");
            }

            //验证验证字符串
            if (bindinfo.ValidateStr != validate)
            {
                return Ok("绑定失败，请重新申请绑定！！！");
            }

            bindinfo.EmailState = true;
            db.Entry(bindinfo).State = EntityState.Modified;
            db.SaveChanges();

            return Ok("恭喜您，绑定成功！！！");
        }
        */

        // PUT api/BindInfo/5
        public IHttpActionResult PutBindInfo(int id, BindInfo bindinfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bindinfo.ID)
            {
                return BadRequest();
            }

            db.Entry(bindinfo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BindInfoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/BindInfo
        /// <summary>
        /// Email绑定请求（状态码：404用户不存在，401邮箱已经被绑定过）
        /// 
        /// </summary>
        [ResponseType(typeof(BindInfo))]
        public IHttpActionResult PostBindInfo(int loginIndex,String email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //无此用户
            TenLogin tenlogin = db.TenLogins.Find(loginIndex);
            if (tenlogin == null)
            {
                return NotFound();
            }

            //Email是否已经被绑定
            bool isBinded = db.BindInfoes.Where(i=>i.EmailAddress ==email && i.EmailState == true).FirstOrDefault() != null;
            if(isBinded)
                return StatusCode(HttpStatusCode.Unauthorized);

            //此用户是否发起过绑定请求
            BindInfo info = db.BindInfoes.Where(i=>i.LoginIndex == loginIndex).FirstOrDefault() ;    
            if(info == null){
                BindInfo bindinfo = new BindInfo();
                bindinfo.LoginIndex = loginIndex;
                bindinfo.EmailAddress = email;
                bindinfo.EmailTime = DateTime.Now;
                bindinfo.EmailState = false;
                bindinfo.ValidateStr = Guid.NewGuid().ToString();
                db.BindInfoes.Add(bindinfo);
                db.SaveChanges();

                TenEmailHelper.GetInstance().SendValidateEmail(email, bindinfo.ValidateStr);

                return CreatedAtRoute("DefaultApi", new { id = bindinfo.ID }, bindinfo);
            }else{
                //重新申请的绑定请求
                info.EmailAddress = email;
                info.EmailTime = DateTime.Now;
                info.ValidateStr = Guid.NewGuid().ToString();
                db.Entry(info).State = EntityState.Modified;
                db.SaveChanges();
                TenEmailHelper.GetInstance().SendValidateEmail(info.EmailAddress, info.ValidateStr);
                return Ok(info);
            }

           
        }

        // DELETE api/BindInfo/5
        [ResponseType(typeof(BindInfo))]
        public IHttpActionResult DeleteBindInfo(int id)
        {
            BindInfo bindinfo = db.BindInfoes.Find(id);
            if (bindinfo == null)
            {
                return NotFound();
            }

            db.BindInfoes.Remove(bindinfo);
            db.SaveChanges();

            return Ok(bindinfo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BindInfoExists(int id)
        {
            return db.BindInfoes.Count(e => e.ID == id) > 0;
        }
    }
}