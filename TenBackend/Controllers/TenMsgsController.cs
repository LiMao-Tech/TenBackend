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
using TenBackend.Models.Tools;

namespace TenBackend.Controllers
{
    public class TenMsgsController : ApiController
    {
       
        private TenBackendDbContext db = new TenBackendDbContext();

        public IHttpActionResult GetTenMsgs(string clientId)
        {
            GeTuiPush.GetInstance().PushMessageToSingle("Ten", "Wlecome to join", "http://www.limao-tech.com/Ten/TenImage?id=2&tumbnail=true", clientId);
            return Ok();
        }

        // GET api/TenMsgs
        /// <summary>
        /// 获取所有聊天记录
        /// </summary>
        public IQueryable<TenMsg> GetTenMsgs()
        {
            return db.TenMsgs;
        }

        //Get latest message
        // GET api/TenMsgs
        /// <summary>
        ///获取指定receiver的MsgIndex>currIndex的聊天消息
        /// </summary>
        [ResponseType(typeof(List<TenMsg>))]
        public IHttpActionResult GetTenMsgs(int receiver, int currIndex)
        {
            try
            {
                List<TenMsg> list = db.TenMsgs.Where(m => m.Receiver == receiver && m.MsgIndex > currIndex).ToList();
                list.Sort((m1, m2) => m1.MsgIndex - m2.MsgIndex);
                return Ok(list);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        // GET api/TenMsg/5
        /// <summary>
        ///  获取指定用户的最后一条聊天消息的Index
        /// </summary>
        /// <param name="id">Value of the UserIndex</param>
        public IHttpActionResult GetTenMsgs(int userIndex)
        {
            int lastIndex = -1;
            try
            {
             lastIndex  =  db.TenMsgs.Where(m => m.Receiver == userIndex).Max(m => m.MsgIndex);
            }
            catch (Exception e)
            {
                return Ok(lastIndex);
            }
           
            return Ok(lastIndex);
        }

        // GET api/TenMsgs/5
        /// <summary>
        /// get specific users' limited numbers(decide by amount) of messages before the msgIndex
        /// </summary>

        [ResponseType(typeof(List<TenMsg>))]
        public IHttpActionResult GetTenMsgs(int sender, int receiver, int msgIndx, int amount)
        {
            try
            {
                List<TenMsg> msgs = db.TenMsgs.Where(m =>
                m.Sender == sender &&
                m.Receiver == receiver &&
                m.MsgType == Commons.MSG_TYPE_USER &&
                m.MsgIndex < msgIndx)
                .OrderBy(m => m.MsgIndex).ToList();

                List<TenMsg> list = new List<TenMsg>();
                int baseIndex = msgs.Count - amount;
                for (int i = baseIndex; i < msgs.Count; i++)
                {
                    list.Add(msgs.ElementAt(i));
                }
                list.Sort((m1, m2) => m1.MsgIndex - m2.MsgIndex);
                return Ok(list);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }



        // GET api/TenMsg/5
        /// <summary>
        /// 获取指定用户,msgindex > currentIndex的系统通知(receiver=0,msgType=0)或特定用户的图片，P币通知
        /// </summary>
        [ResponseType(typeof(IQueryable<TenMsg>))]
        public IQueryable<TenMsg> GetTenMsgs(int receiver, int msgType,int currentIndex)
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
        /// 发送聊天消息
        /// </summary>
        [ResponseType(typeof(TenMsg))]
        public IHttpActionResult PostTenMsg(TenMsg tenmsg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            tenmsg.MsgTime = TimeUtils.DateTimeToUnixTimestamp(DateTime.UtcNow);

            db.TenMsgs.Add(tenmsg);
            db.SaveChanges();

            if (tenmsg.PhoneType == Commons.PHONE_TYPE_IPHONE) // iPhone
            {
                TenLogin targetLogin = db.TenLogins.Where(tl => tl.UserIndex == tenmsg.Receiver).FirstOrDefault();
                if (tenmsg.Sender == 0)
                {
                    TenPushBroker.GetInstance().
                       SendNotification2Apple(
                       targetLogin.DeviceToken,
                       new StringBuilder().Append(tenmsg.MsgContent).ToString());
                }
                else
                {
                    TenUser u = db.TenUsers.Find(tenmsg.Sender);
                    TenPushBroker.GetInstance().
                        SendNotification2Apple(
                        targetLogin.DeviceToken,
                        new StringBuilder(u.UserName).Append(":").Append(tenmsg.MsgContent).ToString());
                }
              
            }
            else if (tenmsg.PhoneType == Commons.PHONE_TYPE_ANDROID) // Android
            {
                TenLogin targetLogin = db.TenLogins.Where(tl => tl.UserIndex == tenmsg.Receiver).FirstOrDefault();
                if (tenmsg.Sender == 0)
                {
                    GeTuiPush.GetInstance().PushMessageToSingle("Ten", tenmsg.MsgContent, "", targetLogin.DeviceToken);
                }
                else
                {
                    TenUser u = db.TenUsers.Find(tenmsg.Sender);
                    GeTuiPush.GetInstance().PushMessageToSingle(u.UserName, tenmsg.MsgContent, u.ProfileUrl, targetLogin.DeviceToken);
                }
               
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