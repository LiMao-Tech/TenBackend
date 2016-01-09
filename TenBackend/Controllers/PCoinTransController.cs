using PushSharp;
using PushSharp.Apple;
using PushSharp.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
    public class PCoinTransController : ApiController
    {

      
        private TenBackendDbContext db = new TenBackendDbContext();


        /// <summary>
        /// Gets all PcoinTrans data from the server.
        /// </summary>
        // GET api/PCoinTrans
        public IQueryable<PCoinTrans> GetPCoinTrans()
        {
            return db.PCoinTrans;
        }

        // GET api/PCoinTrans/5
        //[ResponseType(typeof(PCoinTrans))]
        //public IHttpActionResult GetPCoinTrans(int id)
        //{
        //    PCoinTrans pcointrans = db.PCoinTrans.Find(id);
        //    if (pcointrans == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(pcointrans);
        //}

        //GET api/PCoinTrans?userindex=5
        /// <summary>
        /// Get all trans records of the userindex user
        /// </summary>
        [ResponseType(typeof(IQueryable<PCoinTrans>))]
        public IQueryable<PCoinTrans> GetPCoinTrans(int userindex)
        {
          
            return db.PCoinTrans.Where(p => p.Sender == userindex || p.Receiver == userindex);
        }

        // PUT api/PCoinTrans/5
        public IHttpActionResult PutPCoinTrans(int id, PCoinTrans pcointrans)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pcointrans.ID)
            {
                return BadRequest();
            }

            db.Entry(pcointrans).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PCoinTransExists(id))
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

        // POST api/PCoinTrans
        /// <summary>
        /// Add a row of trans data
        /// </summary>
        [ResponseType(typeof(PCoinTrans))]
        public IHttpActionResult PostPCoinTrans(PCoinTrans pcointrans)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            

            //更新用户Pcoin信息
            TenUser uSender = db.TenUsers.Find(pcointrans.Sender);
            TenUser uReciver = db.TenUsers.Find(pcointrans.Receiver);

            decimal offset = uSender.PCoin-pcointrans.Amount;
            //余额不足
            if (offset < 0)
            {
                return BadRequest("insufficient Balance");
            }

            try
            {
                uSender.PCoin = offset;
                uReciver.PCoin = uReciver.PCoin + pcointrans.Amount;
                db.Entry(uSender).State = EntityState.Modified;
                db.Entry(uReciver).State = EntityState.Modified;
                db.PCoinTrans.Add(pcointrans);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return BadRequest("failed, details:" + e.ToString());
            }
            

            //发送通知
            if (pcointrans.PhoneType == Commons.PHONE_TYPE_IPHONE)
            {
                TenLogin targetLogin = db.TenLogins.Where(tl => tl.UserIndex == pcointrans.Receiver).FirstOrDefault();

                TenMsg tenmsg = new TenMsg();
                tenmsg.MsgType = Commons.MSG_TYPE_PCOIN;
                tenmsg.Sender = 0;
                tenmsg.Receiver = pcointrans.Receiver;
                String content = new StringBuilder().Append(uSender.UserName).Append("赠送了您一笔P币").ToString();
                tenmsg.MsgContent = content;
                tenmsg.MsgTime = DateTime.Now;
                db.TenMsgs.Add(tenmsg);
                db.SaveChanges();

                TenPushBroker.GetInstance().SendNotification2Apple(targetLogin.DeviceToken, content);
            }
            else if (pcointrans.PhoneType == Commons.PHONE_TYPE_ANDROID)
            {

            }

            return CreatedAtRoute("DefaultApi", new { id = pcointrans.ID }, pcointrans);
        }

        // DELETE api/PCoinTrans/5
        [ResponseType(typeof(PCoinTrans))]
        public IHttpActionResult DeletePCoinTrans(int id)
        {
            PCoinTrans pcointrans = db.PCoinTrans.Find(id);
            if (pcointrans == null)
            {
                return NotFound();
            }

            db.PCoinTrans.Remove(pcointrans);
            db.SaveChanges();

            return Ok(pcointrans);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PCoinTransExists(int id)
        {
            return db.PCoinTrans.Count(e => e.ID == id) > 0;
        }


        static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            //Currently this event will only ever happen for Android GCM
            Console.WriteLine("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
        }

        static void NotificationSent(object sender, INotification notification)
        {
            Console.WriteLine("Sent: " + sender + " -> " + notification);
        }

        static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            Console.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
        }

        static void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
        }

        static void ServiceException(object sender, Exception exception)
        {
            Console.WriteLine("Service Exception: " + sender + " -> " + exception);
        }

        static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            Console.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
        }

        static void ChannelDestroyed(object sender)
        {
            Console.WriteLine("Channel Destroyed for: " + sender);
        }

        static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            Console.WriteLine("Channel Created for: " + sender);
        }

        static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}