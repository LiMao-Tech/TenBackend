using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TenBackend.Models;
using System.IO;
using TenBackend.Models.DbContexts;
using System.Data.Entity;

namespace TenBackend.Controllers
{
    public class TenImageController : Controller
    {
       private TenImageDbContext db = new TenImageDbContext();

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
                profile.FileName = id + "_" + Guid.NewGuid().ToString() + System.IO.Path.GetFileName(upload.FileName);
                profile.ContentType = upload.ContentType;
                profile.UploadTime = DateTime.Now;
                upload.SaveAs(Path.Combine(profile.BasePath, profile.FileName));

                TenUserDbContext udb = new TenUserDbContext();
                TenUser tenuser = udb.TenUsers.Find(id);
                tenuser.ProfileUrl = Path.Combine(profile.BasePath, profile.FileName);
                udb.Entry(tenuser).State = EntityState.Modified;
                udb.SaveChanges();

                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();

                return Json(tenuser);

            }

            return Json("noUpload");
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
                        FileName = id + "_" + Guid.NewGuid().ToString()+System.IO.Path.GetFileName(upload.FileName),
                        ContentType = upload.ContentType,
                        BasePath = Path.Combine(Server.MapPath("~/images")),
                        ImageType = ImageType.Photo,
                        UploadTime = DateTime.Now,
                        UserIndex = id
                    };

                    upload.SaveAs(Path.Combine(photo.BasePath, photo.FileName));
                    db.TenImages.Add(photo);
                    db.SaveChanges();
                }

                return Json("success");

            }

            return Json("noUploads");
        }

        /// <summary>
        /// 删除照片
        /// </summary>
        /// <param name="id">图片ID</param>
        public JsonResult DeletePhoto(int id)
        {
            try
            {
                TenImage image = db.TenImages.Find(id);
                string path = Path.Combine(image.BasePath, image.FileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
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
        public ActionResult Index(int id)
        {
            var imageToRetrieve = db.TenImages.Find(id);
            FileStream fileStream = new FileStream(Path.Combine(imageToRetrieve.BasePath, imageToRetrieve.FileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return File(fileStream, imageToRetrieve.ContentType);
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
                    BasePath = Path.Combine(Server.MapPath("~/images")),
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

       
    
	}
}