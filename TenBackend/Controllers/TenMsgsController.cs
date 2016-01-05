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

namespace TenBackend.Controllers
{
    public class TenMsgsController : ApiController
    {
        static string PUSH_CERTI_LOC = "./Resources/TenPushNotiDev.p12";
        static string PUSH_CERTI_PWD = "LiMao1234";

        private PushBroker m_pushBroker = new PushBroker();
        private Byte[] m_appleCerti = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PUSH_CERTI_LOC));

        private TenBackendDbContext db = new TenBackendDbContext();

        public TenMsgsController()
        {
            // Wire up the events for all the services that the broker registers
            m_pushBroker.OnNotificationSent += NotificationSent;
            m_pushBroker.OnChannelException += ChannelException;
            m_pushBroker.OnServiceException += ServiceException;
            m_pushBroker.OnNotificationFailed += NotificationFailed;
            m_pushBroker.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
            m_pushBroker.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
            m_pushBroker.OnChannelCreated += ChannelCreated;
            m_pushBroker.OnChannelDestroyed += ChannelDestroyed;

            m_pushBroker.RegisterAppleService(new ApplePushChannelSettings(m_appleCerti, "LiMao1234"));
        }

        // GET api/TenMsgs
        public IQueryable<TenMsg> GetTenMsgs()
        {
            return db.TenMsgs;
        }

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

        [ResponseType(typeof(List<TenMsg>))]
        public List<TenMsg> GetTenMsg(int id, int receiver, int sender)
        {
            List<TenMsg> list = db.TenMsgs.Where(m => m.MsgIndex > id && m.Receiver == receiver && m.Sender == sender).ToList();
            List<TenMsg> list1 = db.TenMsgs.Where(m => m.MsgIndex > id && m.Receiver == sender && m.Sender == receiver).ToList();
            list.AddRange(list1);
            list.Sort((m1, m2) => m1.MsgIndex - m2.MsgIndex);
            return list;
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

            if (tenmsg.PhoneType == 0) // iPhone
            {
                TenLogin targetLogin = db.TenLogins.Where(tl => tl.UserIndex == tenmsg.Receiver).FirstOrDefault();
                TenUser u = db.TenUsers.Find(tenmsg.Sender);

                m_pushBroker.QueueNotification(new AppleNotification()
                                            .ForDeviceToken(targetLogin.DeviceToken)
                                            .WithAlert( u.UserName+": "+tenmsg.MsgContent)
                                            .WithBadge(7)
                                            .WithSound("sound.caf"));
            }
            else if (tenmsg.PhoneType == 1) // Android
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