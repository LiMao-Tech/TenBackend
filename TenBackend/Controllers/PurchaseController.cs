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
using TenBackend.Models.Assitants;
using TenBackend.Models.Entities;
using TenBackend.Models.PDL;

namespace TenBackend.Controllers
{
    public class PurchaseController : ApiController
    {
        private TenBackendDbContext db = new TenBackendDbContext();

        // GET api/Purchase
        public IQueryable<Purchase> GetPurchases()
        {
            return db.Purchases;
        }

        // GET api/Purchase/5
        /// <summary>
        /// 获取用户的购买记录
        /// </summary>
        /// <param name="userIndex">用户账号</param>
        /// <param name="purchaseType">购买类别，0为购买P币，1为购买等级</param>
        [ResponseType(typeof(IQueryable<Purchase>))]
        public IQueryable<Purchase> GetPurchase(int userIndex,byte purchaseType)
        {
           TenLogin tenlogin = db.TenLogins.Where(u => u.UserIndex == userIndex).FirstOrDefault();
            
           return  db.Purchases.Where(p => p.UserId == tenlogin.UserID &&  p.PurchaseType == purchaseType);
        }

        // GET api/Purchase/5
        /// <summary>
        /// 获取该用户purchaseIndex之后的购买记录
        /// </summary>
        /// <param name="userIndex">用户账号</param>
        /// <param name="purchaseIndex">purchaseIndex</param>
        [ResponseType(typeof(IQueryable<Purchase>))]
        public IQueryable<Purchase> GetPurchase(int userIndex, int purchaseIndex)
        {
            TenLogin tenlogin = db.TenLogins.Where(u => u.UserIndex == userIndex).FirstOrDefault();

            return db.Purchases.Where(p => p.UserId == tenlogin.UserID && p.ID > purchaseIndex);
        }

        // PUT api/Purchase/5
        public IHttpActionResult PutPurchase(int id, Purchase purchase)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != purchase.ID)
            {
                return BadRequest();
            }

            db.Entry(purchase).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseExists(id))
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

        // POST api/Purchase
        [ResponseType(typeof(Purchase))]
        public IHttpActionResult PostPurchase(Purchase purchase)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Purchases.Add(purchase);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = purchase.ID }, purchase);
        }

        // DELETE api/Purchase/5
        [ResponseType(typeof(Purchase))]
        public IHttpActionResult DeletePurchase(int id)
        {
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return NotFound();
            }

            db.Purchases.Remove(purchase);
            db.SaveChanges();

            return Ok(purchase);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PurchaseExists(int id)
        {
            return db.Purchases.Count(e => e.ID == id) > 0;
        }
    }
}