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
using TenBackend.Models.Entities;
using TenBackend.Models.Assitants;
using TenBackend.Models.PDL;
using TenBackend.Models.Tools.PushHelpers;

namespace TenBackend.Controllers
{
    public class ImageUnlockerController : ApiController
    {
        private TenBackendDbContext db = new TenBackendDbContext();

        // GET api/ImageUnlocker
        public IQueryable<ImageUnlocker> GetImageUnlockers()
        {
            return db.ImageUnlockers;
        }

        // GET api/ImageUnlocker/5
        [ResponseType(typeof(ImageUnlocker))]
        public IHttpActionResult GetImageUnlocker(int id)
        {
            ImageUnlocker imageunlocker = db.ImageUnlockers.Find(id);
            if (imageunlocker == null)
            {
                return NotFound();
            }

            return Ok(imageunlocker);
        }

        // GET api/ImageUnlocker/5
        ///<summary>获取用户owner对用户unlocker已经解锁的图片的记录</summary>
        /// <param name="owner"></param>
        [ResponseType(typeof(IQueryable<ImageUnlocker>))]
        public IQueryable<ImageUnlocker> GetImageUnlocker(int owner,int unlocker)
        {
            return db.ImageUnlockers.Where(e=>e.Owner == owner &&　e.Unlocker == unlocker);
        }


        // PUT api/ImageUnlocker/5
        public IHttpActionResult PutImageUnlocker(int id, ImageUnlocker imageunlocker)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != imageunlocker.ID)
            {
                return BadRequest();
            }

            db.Entry(imageunlocker).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageUnlockerExists(id))
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

        // POST api/ImageUnlocker
        /// <summary>
        /// 解锁图片
        /// </summary>
        /// <param name="imageunlocker">Tip:1.TenImageID为图片的ID,</param>
        [ResponseType(typeof(ImageUnlocker))]
        public IHttpActionResult PostImageUnlocker(ImageUnlocker imageunlocker)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TenUser unlocker = db.TenUsers.Find(imageunlocker.Unlocker);
            decimal offset = unlocker.PCoin - imageunlocker.Pcoin;
            if (offset < 0)
            {
                return BadRequest("insufficient Balance");
            }

            unlocker.PCoin = unlocker.PCoin - imageunlocker.Pcoin;

            TenUser owner = db.TenUsers.Find(imageunlocker.Owner);
            owner.PCoin = owner.PCoin + imageunlocker.Pcoin;

            db.Entry(unlocker).State = EntityState.Modified;
            db.Entry(owner).State = EntityState.Modified;

            db.ImageUnlockers.Add(imageunlocker);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = imageunlocker.ID }, imageunlocker);
        }

        // DELETE api/ImageUnlocker/5
        [ResponseType(typeof(ImageUnlocker))]
        public IHttpActionResult DeleteImageUnlocker(int id)
        {
            ImageUnlocker imageunlocker = db.ImageUnlockers.Find(id);
            if (imageunlocker == null)
            {
                return NotFound();
            }

            db.ImageUnlockers.Remove(imageunlocker);
            db.SaveChanges();

            return Ok(imageunlocker);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ImageUnlockerExists(int id)
        {
            return db.ImageUnlockers.Count(e => e.ID == id) > 0;
        }
    }
}