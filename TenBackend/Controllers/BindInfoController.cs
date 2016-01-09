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
        [ResponseType(typeof(BindInfo))]
        public IHttpActionResult PostBindInfo(BindInfo bindinfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //无此用户
            TenLogin tenlogin = db.TenLogins.Find(bindinfo.LoginIndex);
            if (tenlogin == null)
            {
                return NotFound();
            }

            //Email是否已经被绑定
            bool isBinded = db.BindInfoes.Where(i=>i.EmailAddress == bindinfo.EmailAddress).FirstOrDefault() != null;
            if(isBinded)
                return StatusCode(HttpStatusCode.Unauthorized);

            //此用户是否发起过绑定请求
            BindInfo info = db.BindInfoes.Where(i=>i.LoginIndex == bindinfo.LoginIndex).FirstOrDefault() ;    
            if(info == null){
                bindinfo.EmailTime = DateTime.Now;
                bindinfo.EmailState = false;
                bindinfo.ValidateStr = Guid.NewGuid().ToString();
                db.BindInfoes.Add(bindinfo);
                db.SaveChanges();
                TenEmailHelper.GetInstance().SendValidateEmail(bindinfo.EmailAddress, bindinfo.ValidateStr);

                return CreatedAtRoute("DefaultApi", new { id = bindinfo.ID }, bindinfo);
            }else{
                //未完
                return StatusCode(HttpStatusCode.NoContent);
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