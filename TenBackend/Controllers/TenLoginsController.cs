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


        /**
         * 错误代码表:
         * 404  用户不存在，用于找回密码
         * 401  用户存在
         * 401  参数不匹配
         * 401 密码错误
         * 401  App重装或者其他设备登录，需要修改DeviceUUID,并重新登录
         * 
         * */


        /// <summary>
        /// Gets all TenLogin data from the server.
        /// </summary>
        // GET api/TenLogins
        public IQueryable<TenLogin> GetTenLogins()
        {
            return db.TenLogins;
        }

        // GET api/TenLogins/5
        /// <summary>
        /// Get the special Tenlogin data
        /// </summary>
        /// <param name="id">The key in the Tenlogin table</param>
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

        // POST: api/TenLogins/5
        /// <summary>
        /// Get the password
        /// </summary>
        [ResponseType(typeof(string))]
        public IHttpActionResult PostTenLogin(string userID)
        {
            TenLogin tenlogin = db.TenLogins.Where(l=>l.UserID == userID).FirstOrDefault();
            if(tenlogin == null){
                //用户不存在
                return NotFound();
            }

            return Ok(tenlogin.UserPWD);
        }
         // GET: api/TenLogins/5
        /// <summary>
        /// Sign in to the server
        /// </summary>
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
                //密码错误
                return StatusCode(HttpStatusCode.Unauthorized);
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
                //return BadRequest("Wrong HashValue!  " + "hashStr:" + sb.ToString() + "  hashResult:" + mHash+"  details:"+
                //    "[" + userID + "," + userPWD + "," + lastLogin + "," + DeviceUUID + "," + DeviceToken + "," + companyCode + "]");
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            if (tenLogin.DeviceUUID != DeviceUUID)
            {
                //更换了设备或者应用重装，需要先修改DeviceUUID
                tenLogin.DeviceUUID = DeviceUUID;
            }

            tenLogin.DeviceToken = DeviceToken;

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

            TenUser tenuser = db.TenUsers.Find(tenLogin.UserIndex);

            if (tenuser == null) 
            {
                return NotFound();
            }

            return Ok(tenuser);
        }


        // GET: api/TenLogins/5
        /// <summary>
        /// Check if the same device or its a new app
        /// </summary>
        [ResponseType(typeof(TenLogin))]
        public IHttpActionResult GetTenLogin(string userID, string DeviceUUID)
        {
            TenLogin tenlogin = db.TenLogins.Where(l => l.UserID == userID && l.DeviceUUID == DeviceUUID).FirstOrDefault();
            if (tenlogin == null)
            {
                return NotFound();
            }
            return Ok(tenlogin);
        }
       

        // PUT api/TenLogins/5
        /// <summary>
        /// Update the special Tenlogin data
        /// </summary>
        /// <param name="id">The value of LoginIndex</param>
        public IHttpActionResult PutTenLogin(int id, TenLogin tenlogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tenlogin.LoginIndex)
            {
                StatusCode(HttpStatusCode.Unauthorized);//参数不匹配
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

            TenUser tenuser = db.TenUsers.Find(tenlogin.UserIndex);

            return Ok(tenuser);
        }

        public IHttpActionResult PostTenLogin(int userindex, string devicetoken)
        {

            TenLogin tenlogin = db.TenLogins.Where(u => u.UserIndex == userindex).FirstOrDefault();

            if (tenlogin == null) 
            {
                return NotFound();
            }

            tenlogin.DeviceToken = devicetoken;

            db.Entry(tenlogin).State = EntityState.Modified;

            try 
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

            return Ok();
        }

        // POST api/TenLogins
        /// <summary>
        /// Add a row of a TenLogin data
        /// </summary>
        [ResponseType(typeof(TenLogin))]
        public IHttpActionResult PostTenLogin(TenLogin tenlogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TenLogin t = db.TenLogins.Where(e => e.UserID == tenlogin.UserID).FirstOrDefault();
            if (t != null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);//用户存在
            }

            db.TenLogins.Add(tenlogin);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = tenlogin.LoginIndex }, tenlogin);
        }

        // DELETE api/TenLogins/5
        /// <summary>
        /// Delete the special TenLogin data
        /// </summary>
        /// <param name="id">The key in the Tenlogin table</param>
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