using PushSharp;
using PushSharp.Apple;
using PushSharp.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using TenBackend.Models;
using TenBackend.Models.PDL;
using TenBackend.Models.PushHelpers;

namespace TenBackend.Controllers
{
    public class TenMsgsController : ApiController
    {
       
        private TenBackendDbContext db = new TenBackendDbContext();

        // GET api/TenMsgs
        public IQueryable<TenMsg> GetTenMsgs()
        {
            return db.TenMsgs;
        }

        //Get latest message
        [ResponseType(typeof(List<TenMsg>))]
        public List<TenMsg> GetTenMsgs(int receiver, int currIndex)
        {
            List<TenMsg> list = db.TenMsgs.Where(m => m.Receiver == receiver && m.MsgIndex > currIndex).ToList();
            list.Sort((m1, m2) => m1.MsgIndex - m2.MsgIndex);
            return list;
        }

        // GET api/TenMsgs/5
        /// <summary>
        /// Get the special msg data
        /// </summary>
        /// <param name="id">Value of the MsgIndex</param>
        [ResponseType(typeof(TenMsg))]
        public IHttpActionResult GetTenMsg(int id)
        {

            TenMsg tenmsg = db.TenMsgs.Find(id);
            if (tenmsg == null)
            {
                return NotFound();
            }

            return Ok(tenmsg);
        }

        public IQueryable<TenMsg> GetTenMsgs(int msgType)
        {

            return db.TenMsgs.Where(msg => msg.MsgType == 0);
        }


        // PUT api/TenMsgs/5
        public IHttpActionResult PutTenMsg(int id, TenMsg tenmsg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tenmsg.MsgIndex)
            {
                return BadRequest();
            }

            db.Entry(tenmsg).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TenMsgExists(id))
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


        // POST api/TenMsgs
        /// <summary>
        /// Add a row of Msg data
        /// </summary>
        [ResponseType(typeof(TenMsg))]
        public IHttpActionResult PostTenMsg(TenMsg tenmsg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TenMsgs.Add(tenmsg);
            db.SaveChanges();

            if (tenmsg.PhoneType == Commons.PHONE_TYPE_IPHONE) // iPhone
            {
                TenLogin targetLogin = db.TenLogins.Where(tl => tl.UserIndex == tenmsg.Receiver).FirstOrDefault();
                TenUser u = db.TenUsers.Find(tenmsg.Sender);
                TenPushBroker.GetInstance().
                    SendNotification2Apple(
                    targetLogin.DeviceToken, 
                    new StringBuilder(u.UserName).Append(":").Append(tenmsg.MsgContent).ToString());
            }
            else if (tenmsg.PhoneType == Commons.PHONE_TYPE_ANDROID) // Android
            {

            }

            return CreatedAtRoute("DefaultApi", new { id = tenmsg.MsgIndex }, tenmsg);
        }

        // DELETE api/TenMsgs/5
        [ResponseType(typeof(TenMsg))]
        public IHttpActionResult DeleteTenMsg(int id)
        {
            TenMsg tenmsg = db.TenMsgs.Find(id);
            if (tenmsg == null)
            {
                return NotFound();
            }

            db.TenMsgs.Remove(tenmsg);
            db.SaveChanges();

            return Ok(tenmsg);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TenMsgExists(int id)
        {
            return db.TenMsgs.Count(e => e.MsgIndex == id) > 0;
        }

    }
}