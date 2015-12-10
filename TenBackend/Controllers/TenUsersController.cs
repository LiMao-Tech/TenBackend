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
using TenBackend.Models;
using TenBackend.Models.PDL;

namespace TenBackend.Controllers
{
    public class TenUsersController : ApiController
    {
        private TenBackendDbContext db = new TenBackendDbContext();

        // GET api/TenUsers
        /// <summary>
        /// Get all the TenUser datas
        /// </summary>
        public IQueryable<TenUser> GetTenUsers()
        {
            return db.TenUsers;
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
        [ResponseType(typeof(IQueryable<TenUser>))]
        public IQueryable<TenUser> GetTenUser(double nLati, double sLati, double wLongi, double eLongi)
        {
            return db.TenUsers.Where(tu => tu.Lati < nLati && tu.Lati > sLati && tu.Longi > wLongi && tu.Longi < eLongi);
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
                 
              // tenuser.JoinedDate = DateTime.Now;
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
    }
}