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
    public class MsgsController : ApiController
    {
        private MsgDbContext db = new MsgDbContext();

        // GET: api/Msgs
        public IQueryable<Msg> GetMsgs()
        {
            return db.Msgs;
        }

        // GET: api/Msgs/5
        [ResponseType(typeof(Msg))]
        public async Task<IHttpActionResult> GetMsg(int id)
        {
            Msg msg = await db.Msgs.FindAsync(id);
            if (msg == null)
            {
                return NotFound();
            }

            return Ok(msg);
        }

        // POST: api/Msgs
        [ResponseType(typeof(Msg))]
        public async Task<IHttpActionResult> PostMsg(Msg msg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Msgs.Add(msg);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = msg.msg_index }, msg);
        }

        // DELETE: api/Msgs/5
        [ResponseType(typeof(Msg))]
        public async Task<IHttpActionResult> DeleteMsg(int id)
        {
            Msg msg = await db.Msgs.FindAsync(id);
            if (msg == null)
            {
                return NotFound();
            }

            db.Msgs.Remove(msg);
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
            return db.Msgs.Count(e => e.msg_index == id) > 0;
        }
    }
}