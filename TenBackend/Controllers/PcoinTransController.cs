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
using TenBackend.Models;

using TenBackend.Models.DbContexts;


namespace TenBackend.Controllers
{
    public class PcoinTransController : ApiController
    {
        private PcoinTransDbContext db = new PcoinTransDbContext();

        // GET: api/PcoinTrans
        public IQueryable<PcoinTrans> GetPcoinTrans()
        {
            return db.PcoinTrans;
        }

        // GET: api/PcoinTrans/5
        [ResponseType(typeof(PcoinTrans))]
        public async Task<IHttpActionResult> GetPcoinTrans(int id)
        {
            PcoinTrans pcoinTrans = await db.PcoinTrans.FindAsync(id);
            if (pcoinTrans == null)
            {
                return NotFound();
            }

            return Ok(pcoinTrans);
        }

        // POST: api/PcoinTrans
        [ResponseType(typeof(PcoinTrans))]
        public async Task<IHttpActionResult> PostPcoinTrans(PcoinTrans pcoinTrans)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PcoinTrans.Add(pcoinTrans);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = pcoinTrans.pcoin_index }, pcoinTrans);
        }
    }
}