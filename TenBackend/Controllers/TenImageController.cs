using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Configuration;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net.Http.Headers;

namespace TenBackend.Controllers
{
    public class TenImageController : ApiController
    {
        // GET
        [ResponseType(typeof(StatusCodeResult))]
        public HttpResponseMessage GetTenImage(string name)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
            using (var ms = new MemoryStream())
            {
                blockBlob.DownloadToStream(ms);

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(ms.GetBuffer())
                };
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("inline")
                {
                    FileName = name
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                return result;
            }
        }

        // POST
        [ResponseType(typeof(StatusCodeResult))]
        public StatusCodeResult PostTenImage(string name) {

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);

            using (var fileStream = HttpContext.Current.Request.InputStream)
            {
                blockBlob.UploadFromStream(fileStream);
            }

            return StatusCode(HttpStatusCode.OK);
        }

        private CloudStorageAccount storageAccount { get; set; }
        private CloudBlobClient blobClient { get; set; }
        private CloudBlobContainer container { get; set; }

        public TenImageController()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            blobClient = storageAccount.CreateCloudBlobClient();
            container = blobClient.GetContainerReference("profile-images");
        }
    }
}
