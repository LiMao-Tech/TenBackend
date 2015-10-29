using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TenBackend.Models;
using TenBackend.Models.DbContexts;

namespace TenBackend.Controllers
{
    public class TenLoginsController : ApiController
    {
        private TenLoginDbContext tldb = new TenLoginDbContext();

        // GET: api/TenLogins
        public IQueryable<TenLogin> GetLogins()
        {
            return tldb.TenLogins;
        }

        // GET: api/TenLogins/5
        [ResponseType(typeof(TenLogin))]
        public async Task<IHttpActionResult> GetTenLogin(int id)
        {
            TenLogin tenLogin = await tldb.TenLogins.FindAsync(id);
            if (tenLogin == null)
            {
                return NotFound();
            }

            return Ok(tenLogin);
        }

        // GET: api/TenLogins/5
        [ResponseType(typeof(TenLogin))]
        public IHttpActionResult GetTenLogin(string userID, string userPWD, string lastLogin, string DeviceUUID, string DeviceToken, string HashValue)
        {
            TenLogin tenLogin = tldb.TenLogins.Where(x => x.UserID.CompareTo(userID) == 0).FirstOrDefault();
            if (tenLogin == null)
            {
                return NotFound();
            }

            if (tenLogin.UserPWD != userPWD)
            {
                return BadRequest("Wrong PWD");
            }
            return Ok(tenLogin);
        }

        // PUT: api/TenLogins/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTenLogin(int id, TenLogin tenLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tenLogin.LoginIndex)
            {
                return BadRequest();
            }

            tldb.Entry(tenLogin).State = EntityState.Modified;

            try
            {
                await tldb.SaveChangesAsync();
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

        // POST: api/TenLogins
        [ResponseType(typeof(TenLogin))]
        public async Task<IHttpActionResult> PostTenLogin(TenLogin tenLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            tldb.TenLogins.Add(tenLogin);
            await tldb.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = tenLogin.LoginIndex }, tenLogin);
        }

        // DELETE: api/TenLogins/5
        [ResponseType(typeof(TenLogin))]
        public async Task<IHttpActionResult> DeleteTenLogin(int id)
        {
            TenLogin tenLogin = await tldb.TenLogins.FindAsync(id);
            if (tenLogin == null)
            {
                return NotFound();
            }

            tldb.TenLogins.Remove(tenLogin);
            await tldb.SaveChangesAsync();

            return Ok(tenLogin);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                tldb.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TenLoginExists(int id)
        {
            return tldb.TenLogins.Count(e => e.LoginIndex == id) > 0;
        }
    }
}