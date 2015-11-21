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
    public class TenUsersController : ApiController
    {
        private TenUserDbContext db = new TenUserDbContext();

        // GET: api/TenUsers
        public IQueryable<TenUser> GetTenUsers()
        {
            return db.TenUsers;
        }

        // GET: api/TenUsers/5
        [ResponseType(typeof(TenUser))]
        public IHttpActionResult GetTenUser(int id)
        {
            TenUser tenUser = db.TenUsers.Find(id);
            if (tenUser == null)
            {
                return NotFound();
            }

            return Ok(tenUser);
        }

        // GET: api/TenUsers?nLati=...&sLati=...&wLongi=...&eLongi=...
        [ResponseType(typeof(IQueryable<TenUser>))]
        public IQueryable<TenUser> GetTenUser(double nLati, double sLati, double wLongi, double eLongi)
        {
            return db.TenUsers.Where(tu => tu.Lati < nLati && tu.Lati > sLati && tu.Longi > wLongi && tu.Longi < eLongi);            
        }

        // PUT: api/TenUsers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTenUser(int id, TenUser tenUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tenUser.UserIndex)
            {
                return BadRequest();
            }

            db.Entry(tenUser).State = EntityState.Modified;

            try
            {
                db.SaveChangesAsync();
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

        // POST: api/TenUsers
        [ResponseType(typeof(TenUser))]
        public IHttpActionResult PostTenUser(TenUser tenUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TenUsers.Add(tenUser);
            db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = tenUser.UserIndex }, tenUser);
        }

        // DELETE: api/TenUsers/5
        [ResponseType(typeof(TenUser))]
        public IHttpActionResult DeleteTenUser(int id)
        {
            TenUser tenUser = db.TenUsers.Find(id);
            if (tenUser == null)
            {
                return NotFound();
            }

            db.TenUsers.Remove(tenUser);
            db.SaveChangesAsync();

            return Ok(tenUser);
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
    }
}