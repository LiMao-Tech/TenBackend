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

namespace TenBackend.Controllers
{
    public class MsgsController : ApiController
    {
        private MsgDbContext db = new MsgDbContext();

        // GET: api/Msgs
        public IQueryable<Msg> GetTenUsers()
        {
            return db.TenUsers;
        }

        // GET: api/Msgs/5
        [ResponseType(typeof(Msg))]
        public async Task<IHttpActionResult> GetMsg(int id)
        {
            Msg msg = await db.TenUsers.FindAsync(id);
            if (msg == null)
            {
                return NotFound();
            }

            return Ok(msg);
        }

        // PUT: api/Msgs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMsg(int id, Msg msg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != msg.msg_index)
            {
                return BadRequest();
            }

            db.Entry(msg).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MsgExists(id))
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

        // POST: api/Msgs
        [ResponseType(typeof(Msg))]
        public async Task<IHttpActionResult> PostMsg(Msg msg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TenUsers.Add(msg);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = msg.msg_index }, msg);
        }

        // DELETE: api/Msgs/5
        [ResponseType(typeof(Msg))]
        public async Task<IHttpActionResult> DeleteMsg(int id)
        {
            Msg msg = await db.TenUsers.FindAsync(id);
            if (msg == null)
            {
                return NotFound();
            }

            db.TenUsers.Remove(msg);
            await db.SaveChangesAsync();

            return Ok(msg);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MsgExists(int id)
        {
            return db.TenUsers.Count(e => e.msg_index == id) > 0;
        }
    }
}