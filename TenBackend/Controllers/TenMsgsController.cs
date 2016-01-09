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
using TenBackend.Models.Entities;
using TenBackend.Models.Assitants;
using TenBackend.Models.PDL;
using TenBackend.Models.Tools.PushHelpers;

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

        // GET api/TenMsg/5
        /// <summary>
        /// return specific user's latest MessageIndex in server's database,-1 repreases exception
        /// </summary>
        /// <param name="id">Value of the UserIndex</param>
        [ResponseType(typeof(TenMsg))]
        public IHttpActionResult GetTenMsg(int userIndex)
        {
            int lastIndex = -1;
            try
            {
             lastIndex  =  db.TenMsgs.Where(m => m.Receiver == userIndex).Max(m => m.MsgIndex);
            }
            catch (Exception e)
            {
                throw e;
            }
           
            return Ok(lastIndex);
        }

        // GET api/TenMsgs/5
        /// <summary>
        /// get specific users' limited numbers(decide by amount) of messages before the msgIndex
        /// </summary>
        public List<TenMsg> GetTenMsgs(int sender,int receiver,int msgIndx,int amount)
        {
            List<TenMsg> msgs = db.TenMsgs.Where(m =>
                m.Sender == sender &&
                m.Receiver == receiver &&
                m.MsgType == Commons.MSG_TYPE_USER &&
                m.MsgIndex < msgIndx)
                .OrderBy(m => m.MsgIndex).ToList();
            List<TenMsg> list = new List<TenMsg>();
            int baseIndex = msgs.Count-amount ;
            for (int i = baseIndex; i < msgs.Count; i++)
            {
                list.Add(msgs.ElementAt(i));
            }
            list.Sort((m1, m2) => m1.MsgIndex - m2.MsgIndex);
            return list;
        }



        // GET api/TenMsg/5
        /// <summary>
        /// Get the System Notification or PcoinNotification or ImageNotifications of specail user
        /// Tips:
        ///     receiver==0 && msgType == 0 indicates the SystemNotification
        ///     msgType == 0 indicates the Nomal Message
        ///     msgType == 1 indicates the Image Message
        ///     msgType == 2 indicates the PcoinNotification
        /// </summary>
        /// <param name="receiver"></param>
        public IQueryable<TenMsg> GetTenMsg(int receiver, int msgType,int currentIndex)
        {
            return db.TenMsgs.Where(msg => msg.Receiver == receiver && msg.MsgType == msgType && msg.MsgIndex > currentIndex);
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