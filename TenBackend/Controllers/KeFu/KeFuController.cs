using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TenBackend.Models.Assitants;
using TenBackend.Models.Entities;
using TenBackend.Models.Entities.Kefu;
using TenBackend.Models.PDL;
using TenBackend.Models.Tools;
using TenBackend.Models.Tools.PushHelpers;

namespace TenBackend.Controllers.KeFu
{
    public class KeFuController : Controller
    {
        private TenBackendDbContext db = new TenBackendDbContext();

        //
        // GET: /KeFu/
        public ActionResult Index()
        {
            return View();
        }

        
        [HttpGet]
        public ActionResult UserDetails(string userId)
        {
            TenLogin tenLogin = db.TenLogins.Where(login => login.UserID == userId && login.UserIndex !=0 ).FirstOrDefault();
            if (tenLogin != null)
            {
                TenUser tenUser = db.TenUsers.Find(tenLogin.UserIndex);
                UserInfoDetails details = new UserInfoDetails();
                details.UserProfile = tenUser.ProfileUrl;
                details.UserId = tenLogin.UserID;
                details.UserName = tenUser.UserName;
                details.Sex = InfoParseTool.parseSex(tenUser.Gender);
                details.Marriage = InfoParseTool.parseMarriage(tenUser.Marriage);
                details.Birthday = InfoParseTool.parseRealBirthday(tenUser.Birthday);
                details.PCoin = tenUser.PCoin;
                details.Level = InfoParseTool.parseLevel(tenUser);
                details.Location = InfoParseTool.parseLocation(tenUser.Lati,tenUser.Longi);
                details.PhoneType = InfoParseTool.parsePhoneType(tenUser.PhoneType);
                details.JoinedTime = InfoParseTool.parseRealUnixTime(tenUser.JoinedDate);
                details.LastLogin = InfoParseTool.parseRealUnixTime(tenLogin.LastLogin);
                details.Hobby = tenUser.Hobby;
                details.Quote = tenUser.Quote;
                details.Status = "正常";
                details.UserIndex = tenUser.UserIndex;

                return Json(details,JsonRequestBehavior.AllowGet);
            }
            return Json("[]",JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UserList()
        {
            List<UserListInfo> infos = new List<UserListInfo>();

            var tenLogins = from login in db.TenLogins
                            where (login.UserIndex != 0)
                            select login;
            foreach (TenLogin login in tenLogins)
            {
                TenUser u = db.TenUsers.Find(login.UserIndex);
                UserListInfo info = new UserListInfo();
                info.UserId = login.UserID;
                info.UserName = u.UserName;
                info.Sex = InfoParseTool.parseSex(u.Gender);
                info.JoinedTime = InfoParseTool.parseRealUnixTime(u.JoinedDate);
                info.Level = InfoParseTool.parseLevel(u);
                info.Status = "正常";
                info.Achor = "<a onclick='showDetails(this.id)' id='"+info.UserId+"'>详细信息</a>";
                infos.Add(info);
            }

            UserList list = new UserList();
            list.data = infos;
            return Json(list,JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult NotificationList()
        {
            List<NotificationInfo> notifications = new List<NotificationInfo>();

            var tenmsgs = db.TenMsgs.Where(m => m.MsgType == Commons.MSG_TYPE_SYSTEM && m.Receiver == 0);

            foreach (TenMsg msg in tenmsgs)
            {
                NotificationInfo notification = new NotificationInfo();
                notification.ID = msg.MsgIndex;
                notification.Sender = "Ten团队";
                notification.Content = msg.MsgContent;
                notification.Time = InfoParseTool.parseRealUnixTime(msg.MsgTime);
                notification.Achor = "<a onclick='confirmDelete(this.id)' id ='" + notification.ID + "'>删除</a>";
                notifications.Add(notification);
            }
            NotificationList list = new NotificationList();
            list.data = notifications;
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Notification(string msgContent)
        {
                TenMsg tenmsg = new TenMsg();
                tenmsg.MsgContent = msgContent;
                tenmsg.Sender = 0;
                tenmsg.Receiver = 0;
                tenmsg.MsgType = Commons.MSG_TYPE_SYSTEM;
                tenmsg.IsLocked = false;
                tenmsg.MsgTime = TimeUtils.DateTimeToUnixTimestamp(DateTime.UtcNow);

                db.TenMsgs.Add(tenmsg);
                db.SaveChanges();

                foreach (TenLogin login in db.TenLogins)
                {
                    if (login.UserIndex != 0 && login.DeviceToken != null)
                    {
                        TenUser u = db.TenUsers.Find(login.UserIndex);
                        if (u.PhoneType == Commons.PHONE_TYPE_IPHONE)
                        {
                            TenPushBroker.GetInstance().SendNotification2Apple(login.DeviceToken, tenmsg.MsgContent);
                        }
                        else
                        {
                            GeTuiPush.GetInstance().PushMessageToSingle("Ten团队", tenmsg.MsgContent, "", login.DeviceToken);
                        }

                    }
                }
                    
                   
                return Json(tenmsg);
            
         }



        [HttpGet]
        public ActionResult Order(string userId, int timeOption, int typeOption)
        {
            List<Order> orders = new List<Order>();
            TenLogin tenlogin = db.TenLogins.Where(u => u.UserID == userId).FirstOrDefault();
            if (tenlogin != null)
            {

                long selectTime = InfoParseTool.parseTimeOption(timeOption);

                if (typeOption == Commons.PURCHASE_TYPE_PCOIN_TRANS)
                {
                    var ptrans = from trans in db.PCoinTrans
                                 where trans.Receiver == tenlogin.UserIndex || trans.Sender == tenlogin.UserIndex && trans.TransTime >= selectTime
                                 select trans;
                    foreach (PCoinTrans p in ptrans)
                    {
                        Order o = new Order();
                        o.UserId = userId;

                        if(p.Sender == tenlogin.UserIndex)
                        {
                            TenUser u = db.TenUsers.Find(p.Receiver);   
                            o.Details = new StringBuilder("转账给").Append(u.UserName).Append(p.Amount).Append("P币").ToString();
                        }
                        
                        if(p.Receiver == tenlogin.UserIndex){
                            TenUser u = db.TenUsers.Find(p.Sender);
                            o.Details = new StringBuilder("来自").Append(u.UserName).Append(p.Amount).Append("的P币转账").ToString();
                        }

                        o.OrderTime = InfoParseTool.parseRealUnixTime(p.TransTime);
                        o.ModifiedTime = InfoParseTool.parseRealUnixTime(p.TransTime);
                        o.Status = "完成";
                        o.OrderType = "转账";
                        orders.Add(o);
                    }
                }
                else if (typeOption == Commons.PURCHASE_TYPE_APPLE || typeOption == Commons.PURCHASE_TYPE_UNLOCK_LEVEL)
                {
                    var purchases = from p in db.Purchases
                                    where p.UserId == userId && p.PurchaseType == typeOption && p.PurchaseDate >= selectTime
                                    select p;
                    foreach (Purchase p in purchases)
                    {
                        Order o = new Order();
                        o.UserId = p.UserId;

                        if (p.PurchaseType == Commons.PURCHASE_TYPE_APPLE)
                        {
                            string dollar = "" + Int32.Parse(p.Content) / Commons.PCOIN_PRICE;
                            o.Details = new StringBuilder("从Apple购买").Append(p.Content).Append("P币,花费$").Append(dollar).ToString();
                            o.OrderType = "购买P币";
                        }
                        if (p.PurchaseType == Commons.PURCHASE_TYPE_UNLOCK_LEVEL)
                        {
                            string level = "" + Int32.Parse(p.Content) / Commons.LEVEL_PRICE;
                            o.Details = new StringBuilder("解锁等级").Append(level).Append("花费").Append(p.Content).Append("P币").ToString();
                            o.OrderType = "购买等级";
                        }
                        o.OrderTime = InfoParseTool.parseRealUnixTime(p.PurchaseDate);
                        o.ModifiedTime = o.OrderTime;
                        o.Status = p.Status;
                        orders.Add(o);
                    }
                }
                else if (typeOption == Commons.PURCHASE_TYPE_UNLOCK_IMAGE)
                {
                    var unlockers = from unlocker in db.ImageUnlockers
                                    where unlocker.Owner == tenlogin.UserIndex || unlocker.Unlocker == tenlogin.UserIndex && unlocker.UnlockTime >= selectTime
                                    select unlocker;
                    foreach (ImageUnlocker r in unlockers)
                    {
                        Order o = new Order();
                        o.UserId = userId;

                        if (r.Unlocker == tenlogin.UserIndex)
                        {
                            TenUser u = db.TenUsers.Find(r.Owner);
                            o.Details = new StringBuilder("解锁").Append(u.UserName).Append("照片花费").Append(r.Pcoin).Append("P币").ToString();
                        }
                        if (r.Owner == tenlogin.UserIndex)
                        {
                            TenUser u = db.TenUsers.Find(r.Unlocker);
                            o.Details = new StringBuilder().Append(u.UserName).Append("解锁您的照片，收到").Append(r.Pcoin).Append("P币").ToString();
                        }
                        o.OrderTime = InfoParseTool.parseRealUnixTime(r.UnlockTime);
                        o.ModifiedTime = o.OrderTime;
                        o.Status = "完成";
                        o.OrderType = "解锁照片";
                        orders.Add(o);
                    }
                }
            }

            return Json(orders,JsonRequestBehavior.AllowGet);
        }

        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
	}
}