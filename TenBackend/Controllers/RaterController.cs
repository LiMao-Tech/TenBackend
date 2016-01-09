using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using TenBackend.Models.Assitants;
using TenBackend.Models.Entities;
using TenBackend.Models.PDL;
using TenBackend.Models.Tools.PushHelpers;

namespace TenBackend.Controllers
{
    public class RaterController : ApiController
    {
        static String rateStr = "对你进行了评价，赶快看看吧。。。";

        private TenBackendDbContext db = new TenBackendDbContext();

        // GET api/Rater
        public IQueryable<Rater> GetRaters()
        {
            return db.Raters;
        }

        // GET api/Rater/5
        [ResponseType(typeof(Rater))]
        public IHttpActionResult GetRater(int id)
        {
            Rater rater = db.Raters.Find(id);
            if (rater == null)
            {
                return NotFound();
            }

            return Ok(rater);
        }

        // PUT api/Rater/5
        public IHttpActionResult PutRater(int id, Rater rater)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rater.ID)
            {
                return BadRequest();
            }

            db.Entry(rater).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RaterExists(id))
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

        // POST api/Rater
        [ResponseType(typeof(Rater))]
        public IHttpActionResult PostRater(Rater rater)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //更新被评分者分数

            TenUser tenuser = db.TenUsers.Find(rater.UserIndex);
            tenuser.OuterScore = (tenuser.OuterScore + rater.OuterScore) / 2;
            tenuser.InnerScore = (tenuser.InnerScore + rater.InnerScore) / 2;
            tenuser.Energy = (tenuser.Energy + rater.Energy) / 2;
            db.Entry(tenuser).State = EntityState.Modified;
            
            //添加记录
            db.Raters.Add(rater);
            db.SaveChanges();

            //发送通知
            TenLogin tenlogin = db.TenLogins.Find(rater.UserIndex);

            if (tenuser.PhoneType == Commons.PHONE_TYPE_IPHONE)
            {
                String content = new StringBuilder(tenuser.UserName).Append(rateStr).ToString();
                TenPushBroker.GetInstance().SendNotification2Apple(tenlogin.DeviceToken, content);
            }
            else if (tenuser.PhoneType == Commons.PHONE_TYPE_ANDROID)
            {

            }


            return CreatedAtRoute("DefaultApi", new { id = rater.ID }, rater);
        }

        // DELETE api/Rater/5
        [ResponseType(typeof(Rater))]
        public IHttpActionResult DeleteRater(int id)
        {
            Rater rater = db.Raters.Find(id);
            if (rater == null)
            {
                return NotFound();
            }

            db.Raters.Remove(rater);
            db.SaveChanges();

            return Ok(rater);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RaterExists(int id)
        {
            return db.Raters.Count(e => e.ID == id) > 0;
        }
    }
}