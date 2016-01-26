using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TenBackend.Models.PDL;
using TenBackend.Models;
using System.IO;
using System.Data.Entity;
using System.Text;
using TenBackend.Models.Entities;
using TenBackend.Models.Assitants;
using TenBackend.Models.PDL;
using TenBackend.Models.Tools.PushHelpers;
using TenBackend.Models.Tools;


namespace TenBackend.Controllers
{
    public class TenImageController : Controller
    {
        static string IMAGE_PATH = "D:\\TenImages";
        static string ORIGINAL = "\\Original";
        static string THUMBNAIL = "\\Thumbnail";
        static string SEND_IMAGE_STR = "给你发送了张图片";
        private TenBackendDbContext db = new TenBackendDbContext();

        public TenImageController()
        {
            checkeDir();
        }

        private void checkeDir(){
            String originalPath = Path.Combine(IMAGE_PATH, ORIGINAL);
            String thumbnailPath = Path.Combine(IMAGE_PATH,THUMBNAIL);
            if (!Directory.Exists(originalPath))
            {
                Directory.CreateDirectory(originalPath);
            }
            if (!Directory.Exists(thumbnailPath))
            {
                Directory.CreateDirectory(thumbnailPath);
            }
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="upload">头像文件</param>
        public JsonResult UploadProfileImage(int id, HttpPostedFileBase upload)
        {
            if (upload != null && upload.ContentLength > 0)
            {

                TenImage profile = db.TenImages.FirstOrDefault(img => img.ImageType == ImageType.Profile && img.UserIndex == id);

                if (profile != null)
                {
                    profile.ContentType = upload.ContentType;
                    profile.UploadTime = DateTime.Now;

                    //覆盖
                    String src = Path.Combine(profile.BasePath, ORIGINAL, profile.FileName);
                    String des = Path.Combine(profile.BasePath, THUMBNAIL, profile.FileName);
                    upload.SaveAs(src);

                    TenImageUtils.resise2Thumbnail(src, des);
                  
                    db.Entry(profile).State = EntityState.Modified;
                    db.SaveChanges();

                    ChangeUserProfile(id);

                    return Json("success");

                }
                else
                {
                    var newProfile = new TenImage
                    {
                        FileName = id + "_" + Guid.NewGuid().ToString() + System.IO.Path.GetFileName(upload.FileName),
                        ContentType = upload.ContentType,
                        BasePath = Path.Combine(IMAGE_PATH),
                        IsLocked = false,
                        ImageType = ImageType.Profile,
                        UploadTime = DateTime.Now,
                        UserIndex = id
                    };
                    String src =  Path.Combine(newProfile.BasePath,ORIGINAL, newProfile.FileName);
                    String des = Path.Combine(newProfile.BasePath,THUMBNAIL,newProfile.FileName);
                    upload.SaveAs(src);
                    //转存缩略图
                    TenImageUtils.resise2Thumbnail(src, des);
                    db.TenImages.Add(newProfile);
                    db.SaveChanges();


                    ChangeUserProfile(id);

                    return Json("success");

                }

            }

            return Json("noUpload");
        }

        /// <summary>
        /// 获取用户头像
        /// </summary>
        /// <param name="userIndex">用户UserIndex</param>
        public ActionResult GetProfileByUser(int userIndex,bool thumbnail = true)
        {

            var imageToRetrieve = db.TenImages.Where(img => img.UserIndex == userIndex && img.ImageType == ImageType.Profile).FirstOrDefault();
            if (thumbnail)
            {
                FileStream fileStream = new FileStream(Path.Combine(imageToRetrieve.BasePath,THUMBNAIL, imageToRetrieve.FileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return File(fileStream, imageToRetrieve.ContentType);
            }
            else
            {
                FileStream fileStream = new FileStream(Path.Combine(imageToRetrieve.BasePath,ORIGINAL, imageToRetrieve.FileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return File(fileStream, imageToRetrieve.ContentType);
            }
           
        }


        /// <summary>
        /// 上传相片
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="uploads">照片文件</param>
        public JsonResult UploadPhotos(int id, HttpPostedFileBase[] uploads)
        {
            if (uploads.Length != 0)
            {

                foreach (HttpPostedFileBase upload in uploads)
                {
                    var photo = new TenImage
                    {
                        FileName = id + "_" + Guid.NewGuid().ToString() + System.IO.Path.GetFileName(upload.FileName),
                        ContentType = upload.ContentType,
                        BasePath = Path.Combine(IMAGE_PATH),
                        IsLocked = false,
                        ImageType = ImageType.Photo,
                        UploadTime = DateTime.Now,
                        UserIndex = id
                    };

                    String src = Path.Combine(photo.BasePath, ORIGINAL, photo.FileName);
                    String des = Path.Combine(photo.BasePath, THUMBNAIL, photo.FileName);
                    upload.SaveAs(src);
                    //转存缩略图
                    TenImageUtils.resise2Thumbnail(src, des);

                    db.TenImages.Add(photo);
                    db.SaveChanges();
                }

                return Json("success");

            }

            return Json("noUploads");
        }

        /// <summary>
        /// 发送图片Message
        /// </summary>
        /// <param name="sender">发送人</param>
        /// <param name="receiver">接收人</param>
        /// <param name="time">发送时间</param>
        /// <param name="phoneType">手机类型</param>
        /// <param name="upload">图片文件</param>
        public JsonResult SendImage(int sender, int receiver, DateTime time, byte phoneType,HttpPostedFileBase upload)
        {
             if (upload != null && upload.ContentLength > 0)
            {
                    
                    //保存图片
                    var image = new TenImage
                    {
                        FileName = sender + "_" + Guid.NewGuid().ToString() + System.IO.Path.GetFileName(upload.FileName),
                        ContentType = upload.ContentType,
                        BasePath = Path.Combine(IMAGE_PATH),
                        IsLocked = false,
                        ImageType = ImageType.Message,
                        UploadTime = time,
                        UserIndex = sender,
                        MsgIndex = -1
                    };

                    String src = Path.Combine(image.BasePath, ORIGINAL, image.FileName);
                    String des = Path.Combine(image.BasePath, THUMBNAIL, image.FileName);
                    upload.SaveAs(src);
                    //转存缩略图
                    TenImageUtils.resise2Thumbnail(src, des);
                 

                    db.TenImages.Add(image);
                    db.SaveChanges();

                    //添加MSG记录
                    TenUser tenuser = db.TenUsers.Find(sender);
                    TenImage tenimage = db.TenImages.Where(m =>
                            m.UserIndex == sender &&
                            m.MsgIndex == -1 &&
                            m.ImageType == ImageType.Message).FirstOrDefault();

                   
                    TenMsg tenmsg = new TenMsg();
                    tenmsg.Sender = sender;
                    tenmsg.Receiver = receiver;
                    tenmsg.MsgTime = time;
                    tenmsg.MsgType = Commons.MSG_TYPE_IMAGE;
                    tenmsg.PhoneType = phoneType;
                    tenmsg.MsgContent = new StringBuilder("http://www.limao-tech.com/Ten/TenImage?id=").Append(tenimage.ID).Append("&thumbnail=true").ToString();
                    tenmsg.IsLocked = false;
                    db.TenMsgs.Add(tenmsg);
                    db.SaveChanges();
                    
                 
                   //发送通知
                   TenLogin tenlogin = db.TenLogins.Where(u=>u.UserIndex == receiver).FirstOrDefault();
                   if(phoneType == Commons.PHONE_TYPE_IPHONE){

                       string content = new StringBuilder(tenuser.UserName).Append(SEND_IMAGE_STR).ToString();
                       TenPushBroker.GetInstance().SendNotification2Apple(tenlogin.DeviceToken, content);
                   }
                   else if (phoneType == Commons.PHONE_TYPE_ANDROID)
                   {

                   }

                   return Json("success");
                  
                }
             return Json("noUpload");
        }

        /// <summary>
        /// 删除照片
        /// </summary>
        /// <param name="id">图片ID</param>
        [HttpPost]
        public JsonResult DeletePhoto(int id)
        {
            try
            {
                TenImage image = db.TenImages.Find(id);
                if (image == null)
                {
                    return Json("404 no such image");
                }
                string src = Path.Combine(image.BasePath,ORIGINAL, image.FileName);
                string thumb = Path.Combine(image.BasePath, THUMBNAIL, image.FileName);
                if (System.IO.File.Exists(src))
                {
                    System.IO.File.Delete(src);
                }
                if (System.IO.File.Exists(thumb))
                {
                    System.IO.File.Delete(thumb);
                }
                db.TenImages.Remove(image);
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception e)
            {
                return Json(e);
            }

        }

        /// <summary>
        /// 获取用户图片信息
        /// </summary>
        /// <param name="id">用户ID</param>
        public JsonResult GetImagesByUser(int id)
        {
            var images = from img in db.TenImages
                         where img.UserIndex == id
                         select img;
            return Json(images, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="id">图片ID</param>
        public ActionResult Index(int id,bool thumbnail = true)
        {
            var imageToRetrieve = db.TenImages.Find(id);
           ;
            if (thumbnail)
            {
                  FileStream fileStream  = new FileStream(Path.Combine(imageToRetrieve.BasePath,THUMBNAIL,imageToRetrieve.FileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                  return File(fileStream, imageToRetrieve.ContentType);
            }
            else
            {
                FileStream fileStream = new FileStream(Path.Combine(imageToRetrieve.BasePath, ORIGINAL, imageToRetrieve.FileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return File(fileStream, imageToRetrieve.ContentType);
            }
        
          
        }

        /// <summary>
        /// 聊天图片
        /// </summary>
        /// <param name="id">MsgID</param>
        public JsonResult AddMsgImage(int id, HttpPostedFileBase upload)
        {
            if (upload != null && upload.ContentLength > 0)
            {

                var image = new TenImage
                {
                    FileName = "msg_" + Guid.NewGuid().ToString() + System.IO.Path.GetFileName(upload.FileName),
                    ContentType = upload.ContentType,
                    BasePath = Path.Combine(IMAGE_PATH),
                    IsLocked = false,
                    ImageType = ImageType.Message,
                    UploadTime = DateTime.Now,
                    MsgIndex = id
                };
                upload.SaveAs(Path.Combine(image.BasePath, image.FileName));
                db.TenImages.Add(image);
                db.SaveChanges();

                return Json(image);
            }
            return Json("noImage");
        }

        private void ChangeUserProfile(int id)
        {

            TenImage profile = db.TenImages.Where(m => m.UserIndex == id && m.ImageType == ImageType.Profile).FirstOrDefault();

            TenUser tenuser = db.TenUsers.Find(id);

            tenuser.ProfileUrl = new StringBuilder("http://www.limao-tech.com/Ten/TenImage?id=").Append(profile.ID).Append("&tumbnail=true").ToString();
            db.Entry(tenuser).State = EntityState.Modified;
            db.SaveChanges();
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