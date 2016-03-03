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
using TenBackend.Models.Assitants;
using TenBackend.Models.PDL;
using TenBackend.Models.Tools.PushHelpers;
using TenBackend.Models.Tools;
using Newtonsoft.Json.Linq;
using System.Text;

namespace TenBackend.Controllers
{
    public class TenUsersController : ApiController
    {
        private TenBackendDbContext db = new TenBackendDbContext();

        // GET api/TenUsers
        /// <summary>
        /// Get all the TenUser datas
        /// </summary>
        public List<TenUser> GetTenUsers()
        {
            

            return getTrueUsers();
        }

        // GET api/TenUsers/5
        /// <summary>
        /// Get the special TenUser data
        /// </summary>
        /// <param name="id">Value of UserIndex</param>
        [ResponseType(typeof(TenUser))]
        public IHttpActionResult GetTenUser(int id)
        {
            TenUser tenuser = db.TenUsers.Find(id);
            if (tenuser == null)
            {
                return NotFound();
            }

            return Ok(tenuser);
        }

        // GET: api/TenUsers?nLati=...&sLati=...&wLongi=...&eLongi=...
        /// <summary>
        /// Get the special TenUsers of the location
        /// </summary>
        [ResponseType(typeof(IEnumerable<TenUser>))]
        public IEnumerable<TenUser> GetTenUser(double nLati, double sLati, double wLongi, double eLongi)
        {

            return getTrueUsers().Where(tu => tu.Lati < nLati && tu.Lati > sLati && tu.Longi > wLongi && tu.Longi < eLongi);
        }



        // PUT api/TenUsers/5
        /// <summary>
        /// Update TenUser
        /// </summary>
        public IHttpActionResult PutTenUser(int id, TenUser tenuser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tenuser.UserIndex)
            {
                return BadRequest();
            }

            db.Entry(tenuser).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TenUserExists(id))
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

        /// <summary>
        /// Update location
        /// </summary>
        /// <param name="id">TenUserIndex</param>
        /// <param name="Lati">New Lati</param>
        /// <param name="Longi">New Lati</param>
        public IHttpActionResult PutTenUser(int id, double Lati, double Longi)
        {
            TenUser tenuser = db.TenUsers.Find(id);
            if (tenuser == null)
            {
                return NotFound();
            }

            tenuser.Lati = Lati;
            tenuser.Longi = Longi;
            db.Entry(tenuser).State = EntityState.Modified;
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// 解锁等级
        /// </summary>
        /// <param name="id">TenUserIndex</param>
        /// <param name="pcoin">解锁花的P币</param>
        /// <param name="level">解锁的等级</param>
        public IHttpActionResult PutTenUser(int id, decimal pcoin, int level)
        {
            Purchase p = new Purchase();
            p.PurchaseDate = TimeUtils.DateTimeToUnixTimestamp(DateTime.UtcNow);
            p.ModifiedDate = p.PurchaseDate;
            p.UserId = db.TenLogins.Where(u => u.UserIndex == id).FirstOrDefault().UserID;
            //jobject jo = new jobject();
            //jo.add("pcoin", pcoin);
            //jo.add("level", level);
            //string content = jo.tostring().replace("\r", string.empty).replace("\n", string.empty).replace("\\", string.empty);
            p.Content = ""+pcoin;
            p.PurchaseType = Commons.PURCHASE_TYPE_UNLOCK_LEVEL;
            p.Status = "处理中";
            db.Purchases.Add(p);
            db.SaveChanges();

            try
            {
                TenUser tenuser = db.TenUsers.Find(id);
                decimal offset = tenuser.PCoin - pcoin;
                if (offset >= 0)
                {
                    tenuser.PCoin = offset;
                    tenuser.AVG = level;
                    tenuser.Expire = TimeUtils.DateTimeToUnixTimestamp(DateTime.UtcNow.AddHours(24));
                    db.Entry(tenuser).State = EntityState.Modified;
                    db.SaveChanges();
                    p.Status = "完成";
                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();

                }
                else
                {
                    p.Status = "余额不足";
                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();

                    return BadRequest("余额不足");
                }
            }
            catch (Exception e)
            {
                p.Status = "异常";
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();

                return BadRequest("异常");
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        /// <summary>
        /// 从苹果购买P币
        /// </summary>
        /// <param name="id">TenUserIndex</param>
        /// <param name="pcoin">购买的P币</param>
        public IHttpActionResult PutTenUser(int id, decimal pcoin, string note="无")
        {
            Purchase p = new Purchase();
            p.PurchaseDate = TimeUtils.DateTimeToUnixTimestamp(DateTime.UtcNow);
            p.ModifiedDate = p.PurchaseDate;
            p.UserId = db.TenLogins.Where(u => u.UserIndex == id).FirstOrDefault().UserID;
            p.Content = ""+pcoin;
            p.Status = "处理中";
            p.PurchaseType = Commons.PURCHASE_TYPE_APPLE;
            db.Purchases.Add(p);
            db.SaveChanges();

            try
            {
                TenUser tenuser = db.TenUsers.Find(id);
                tenuser.PCoin = tenuser.PCoin + pcoin;
                db.Entry(tenuser).State = EntityState.Modified;
                db.SaveChanges();
                p.Status = "完成";
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                p.Status = "异常";
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();

                return BadRequest("异常");
            }

          

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/TenUsers
        /// <summary>
        /// Add a row of TenUser data
        /// </summary>
       [ResponseType(typeof(TenUser))]
      public IHttpActionResult PostTenUser(TenUser tenuser)
        {
           
            if (!ModelState.IsValid)
             {
                return BadRequest(ModelState);
             }

            tenuser.JoinedDate = TimeUtils.DateTimeToUnixTimestamp(DateTime.UtcNow) ;
            tenuser.AVG = 0;
            tenuser.Expire = 0;
            
            db.TenUsers.Add(tenuser);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = tenuser.UserIndex }, tenuser);
         }
        
        // DELETE api/TenUsers/5
       /// <summary>
       /// Delete the special TenUser
       /// </summary>
       /// <param name="id">Value of UserIndex</param>
        [ResponseType(typeof(TenUser))]
        public IHttpActionResult DeleteTenUser(int id)
        {
            TenUser tenuser = db.TenUsers.Find(id);
            if (tenuser == null)
            {
                return NotFound();
            }

            db.TenUsers.Remove(tenuser);
            db.SaveChanges();

            return Ok(tenuser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TenUserExists(int id)
        {
            return db.TenUsers.Count(e => e.UserIndex == id) > 0;
        }

        private List<TenUser> getTrueUsers()
        {
            IQueryable<TenLogin> tenlogins = from tenlogin in db.TenLogins where (tenlogin.UserIndex != 0) select tenlogin;

            List<TenUser> users = new List<TenUser>();
            foreach(TenLogin u in tenlogins){
                users.Add(db.TenUsers.Find(u.UserIndex));
            }
            return users;
        }
    }
}