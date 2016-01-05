using PushSharp;
using PushSharp.Apple;
using PushSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenBackend.Models.PushHelpers
{
    public class TenPushBroker
    {

        private static string PUSH_CERTI_LOC = "./Resources/TenPushNotiDev.p12";
        private string PUSH_CERTI_PWD = "LiMao1234";


        private Byte[] m_appleCerti = System.IO.File.ReadAllBytes(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PUSH_CERTI_LOC));

        private static int APPLE_NOTIFICATION_BAGE = 7;

        private PushBroker m_pushBroker = new PushBroker();

        private volatile static TenPushBroker _instance = null;
        private static readonly object lockHelper = new object();
        private TenPushBroker()
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
            m_pushBroker.RegisterAppleService(new ApplePushChannelSettings(m_appleCerti, PUSH_CERTI_PWD));
        }




        public static TenPushBroker GetInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                    {
                        _instance = new TenPushBroker();
              
                    }
                }
            }

            return _instance;
        }



        public void SendNotification2Apple(string deviceToken, string content)
        {

            //-------------------------
            // APPLE NOTIFICATIONS
            //-------------------------
            m_pushBroker.QueueNotification(new AppleNotification()
                                             .ForDeviceToken(deviceToken)
                                             .WithAlert(content)
                                             .WithBadge(APPLE_NOTIFICATION_BAGE)
                                             .WithSound("sound.caf"));
        }

        public void SendNotification2Android()
        {
            ////---------------------------
            //// ANDROID GCM NOTIFICATIONS
            ////---------------------------
            ////Configure and start Android GCM
            ////IMPORTANT: The API KEY comes from your Google APIs Console App, under the API Access section, 
            ////  by choosing 'Create new Server key...'
            ////  You must ensure the 'Google Cloud Messaging for Android' service is enabled in your APIs Console
            //push.RegisterGcmService(new GcmPushChannelSettings("YOUR Google API's Console API Access  API KEY for Server Apps HERE"));
            ////Fluent construction of an Android GCM Notification
            ////IMPORTANT: For Android you MUST use your own RegistrationId here that gets generated within your Android app itself!
            //push.QueueNotification(new GcmNotification().ForDeviceRegistrationId("DEVICE REGISTRATION ID HERE")
            //                      .WithJson("{\"alert\":\"Hello World!\",\"badge\":7,\"sound\":\"sound.caf\"}"));
        }

        public void StopNotificationService()
        {
            m_pushBroker.StopAllServices();
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

    }
}