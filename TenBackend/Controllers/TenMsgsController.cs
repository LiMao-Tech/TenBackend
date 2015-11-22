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
using TenBackend.Models.DbContexts;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushSharp;
using System.IO;
using PushSharp.Apple;
using PushSharp.Core;
using System.Diagnostics;
using System.Text;

namespace TenBackend.Controllers
{
    public class TenMsgsController : ApiController
    {
        static string PUSH_CERTI_LOC = "./Resources/TenDevMsgPush.p12";
        static string PUSH_CERTI_PWD = "limao1234";


        private PushBroker m_pushBroker = new PushBroker();
        private Byte[] m_appleCerti = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PUSH_CERTI_LOC));

        private TenMsgDbContext m_db = new TenMsgDbContext();
        private TenLoginDbContext m_loginDb = new TenLoginDbContext();


        public TenMsgsController()
        {
            //Wire up the events for all the services that the broker registers
            m_pushBroker.OnNotificationSent += NotificationSent;
            m_pushBroker.OnChannelException += ChannelException;
            m_pushBroker.OnServiceException += ServiceException;
            m_pushBroker.OnNotificationFailed += NotificationFailed;
            m_pushBroker.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
            m_pushBroker.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
            m_pushBroker.OnChannelCreated += ChannelCreated;
            m_pushBroker.OnChannelDestroyed += ChannelDestroyed;

            m_pushBroker.RegisterAppleService(new ApplePushChannelSettings(m_appleCerti, PUSH_CERTI_PWD));
            
            // push.StopAllServices();
        }


        // GET: api/TenMsgs
        public IQueryable<TenMsg> GetTenMsgs()
        {

            return m_db.TenMsgs;
        }

        // GET: api/TenMsgs/5
        [ResponseType(typeof(TenMsg))]
        public IHttpActionResult GetTenMsg(int id)
        {
            TenMsg tenMsg = m_db.TenMsgs.Find(id);
            if (tenMsg == null)
            {
                return NotFound();
            }

            return Ok(tenMsg);
        }

        // POST: api/TenMsgs
        [ResponseType(typeof(TenMsg))]
        public IHttpActionResult PostTenMsg(TenMsg tenMsg)
        {

            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            m_db.TenMsgs.Add(tenMsg);
            m_db.SaveChanges();

            if (tenMsg.PhoneType == 0) // iPhone
            {
                TenLogin targetLogin = m_loginDb.TenLogins.Where(tl => tl.UserIndex == tenMsg.Receiver).FirstOrDefault();

                Debug.WriteLine("Target Login: " + targetLogin.LastLogin);
                Debug.WriteLine("Device Token: " + ByteArrayToString(targetLogin.DeviceToken));

                m_pushBroker.QueueNotification(new AppleNotification()
                                           .ForDeviceToken(ByteArrayToString(targetLogin.DeviceToken))
                                           .WithAlert(tenMsg.MsgContent)
                                           .WithBadge(7)
                                           .WithSound("sound.caf"));
                /*
                m_pushBroker.QueueNotification(new AppleNotification()
                                           .ForDeviceToken("d0d0a5a868b2b70f5f6900a6cbe034facf38050b4402d14b61a68ae6c27b0b92")
                                           .WithAlert("Hi from TDS!")
                                           .WithBadge(7)
                                           .WithSound("sound.caf"));*/

            }
            else if (tenMsg.PhoneType == 1) // Android
            {

            }

            return CreatedAtRoute("DefaultApi", new { id = tenMsg.MsgIndex }, tenMsg);
        }

        // PUT: api/TenMsgs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTenMsg(int id, TenMsg tenMsg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tenMsg.MsgIndex)
            {
                return BadRequest();
            }

            
            
            try
            {
                m_db.SaveChanges();
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

        

        // DELETE: api/TenMsgs/5
        [ResponseType(typeof(TenMsg))]
        public IHttpActionResult DeleteTenMsg(int id)
        {
            TenMsg tenMsg = m_db.TenMsgs.Find(id);
            if (tenMsg == null)
            {
                return NotFound();
            }

            m_db.TenMsgs.Remove(tenMsg);
            m_db.SaveChanges();

            return Ok(tenMsg);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TenMsgExists(int id)
        {
            return m_db.TenMsgs.Count(e => e.MsgIndex == id) > 0;
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