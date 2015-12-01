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
using TenBackend.Models;
using TenBackend.Models.PDL;

namespace TenBackend.Controllers
{
    public class TenLoginsController : ApiController
    {
        static string companyCode = "e40cb24cffee7767d8f3bd9faf882af614b9e4bd402dc53a70f4723cde991734";
        private TenBackendDbContext db = new TenBackendDbContext();

        // GET api/TenLogins
        public IQueryable<TenLogin> GetTenLogins()
        {
            return db.TenLogins;
        }

        // GET api/TenLogins/5
        [ResponseType(typeof(TenLogin))]
        public IHttpActionResult GetTenLogin(int id)
        {
            TenLogin tenlogin = db.TenLogins.Find(id);
            if (tenlogin == null)
            {
                return NotFound();
            }

            return Ok(tenlogin);
        }
         // GET: api/TenLogins/5
        [ResponseType(typeof(TenLogin))]
        public IHttpActionResult GetTenLogin(string userID, string userPWD, string lastLogin, string DeviceUUID, string DeviceToken, string HashValue)
        {
            TenLogin tenLogin = db.TenLogins.Where(x => x.UserID.CompareTo(userID) == 0).FirstOrDefault();

            if (tenLogin == null)
            {
                return NotFound();
            }

            if (tenLogin.UserPWD != userPWD)
            {
                return BadRequest("Wrong PWD");
            }

            //比较Hash sha-256
            StringBuilder sb = new StringBuilder();
            sb.Append(userID)
                .Append(userPWD)
                .Append(lastLogin)
                .Append(DeviceUUID)
                .Append(DeviceToken)
                .Append(companyCode);
            string mHash = HashEncrypt.SHA256Encrypt(sb.ToString());
            if (mHash != HashValue)
            {
                return BadRequest("Wrong HashValue!  " + "hashStr:" + sb.ToString() + "  hashResult:" + mHash+"  details:"+
                    "[" + userID + "," + userPWD + "," + lastLogin + "," + DeviceUUID + "," + DeviceToken + "," + companyCode + "]");
            }

            try
            {
                tenLogin.LastLogin = DateTime.Parse(lastLogin);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
            
            
            db.Entry(tenLogin).State = EntityState.Modified;
            db.SaveChanges();

            return Ok(tenLogin);
        }

       

        // PUT api/TenLogins/5
        public IHttpActionResult PutTenLogin(int id, TenLogin tenlogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tenlogin.LoginIndex)
            {
                return BadRequest();
            }

            db.Entry(tenlogin).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TenLoginExists(id))
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

        // POST api/TenLogins
        [ResponseType(typeof(TenLogin))]
        public IHttpActionResult PostTenLogin(TenLogin tenlogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           // tenlogin.LastLogin = DateTime.Now;
            db.TenLogins.Add(tenlogin);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = tenlogin.LoginIndex }, tenlogin);
        }

        // DELETE api/TenLogins/5
        [ResponseType(typeof(TenLogin))]
        public IHttpActionResult DeleteTenLogin(int id)
        {
            TenLogin tenlogin = db.TenLogins.Find(id);
            if (tenlogin == null)
            {
                return NotFound();
            }

            db.TenLogins.Remove(tenlogin);
            db.SaveChanges();

            return Ok(tenlogin);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TenLoginExists(int id)
        {
            return db.TenLogins.Count(e => e.LoginIndex == id) > 0;
        }
    }
}