using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Net.Http.Headers;
using System.Configuration;
using System.Web.Http.Results;
using System.IO;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

using TenBackend.Models;
using TenBackend.Models.DbContexts;
using System.Web;


namespace TenBackend.Controllers
{
    public class TenImageUrlsController : ApiController
    {
        private TenImageUrlDbContext db = new TenImageUrlDbContext();

        private CloudStorageAccount storageAccount { get; set; }
        private CloudBlobClient blobClient { get; set; }
        private CloudBlobContainer container { get; set; }

        public TenImageUrlsController()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            blobClient = storageAccount.CreateCloudBlobClient();
            container = blobClient.GetContainerReference("profile-images");
        }

        // GET: api/TenImageUrls
        public IQueryable<TenImageUrl> GetImageUrls()
        {
            return db.ImageUrls;
        }

        // GET: api/TenImageUrls?user_index
        [ResponseType(typeof(TenImageUrl))]
        public IHttpActionResult GetTenImageUrl(int user_index)
        {
            IQueryable<TenImageUrl> tenImageUrls =  db.ImageUrls.Where(iu => iu.user_index==user_index);

            if (tenImageUrls == null)
            {
                return NotFound();
            }

            return Ok(tenImageUrls);
        }

        // GET
        [ResponseType(typeof(StatusCodeResult))]
        public HttpResponseMessage GetTenImageUrl(string tenImageUrl)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(tenImageUrl);
            using (var ms = new MemoryStream())
            {
                blockBlob.DownloadToStream(ms);

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(ms.GetBuffer())
                };
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("inline")
                {
                    FileName = tenImageUrl
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                return result;
            }
        }

        // POST: api/TenImageUrls
        [ResponseType(typeof(TenImageUrl))]
        public async Task<IHttpActionResult> PostTenImageUrl(TenImageUrl tenImageUrl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(tenImageUrl);

            using (var fileStream = HttpContext.Current.Request.InputStream)
            {
                blockBlob.UploadFromStream(fileStream);
            }

            return StatusCode(HttpStatusCode.OK);

            db.ImageUrls.Add(tenImageUrl);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = tenImageUrl.image_url_index }, tenImageUrl);
        }

        // DELETE: api/TenImageUrls/5
        [ResponseType(typeof(TenImageUrl))]
        public async Task<IHttpActionResult> DeleteTenImageUrl(int id)
        {
            TenImageUrl tenImageUrl = await db.ImageUrls.FindAsync(id);
            if (tenImageUrl == null)
            {
                return NotFound();
            }

            db.ImageUrls.Remove(tenImageUrl);
            await db.SaveChangesAsync();

            return Ok(tenImageUrl);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TenImageUrlExists(int id)
        {
            return db.ImageUrls.Count(e => e.image_url_index == id) > 0;
        }
    }

        // PUT: api/TenImageUrls/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTenImageUrl(int id, TenImageUrl tenImageUrl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tenImageUrl.image_url_index)
            {
                return BadRequest();
            }

            db.Entry(tenImageUrl).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TenImageUrlExists(id))
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
}