using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;
using TenBackend.Models;
using TenBackend.Models.DbContexts;

namespace TenBackend.Controllers
{
    public class TenLoginController : ApiController
    {

        private TenUserDbContext db = new TenUserDbContext();

        // POST: api/TenLogin
        public IHttpActionResult PostTenLogin(string user_id, string user_pw, string device_uuid, string device_token, string timesamp, string hash_result)
        {
            TenLoginObj tlio = new TenLoginObj(user_id, user_pw, device_uuid, device_token, timesamp, hash_result);

            TenUser tu = db.TenUsers.Where(x => x.user_id == tlio.user_id).FirstOrDefault();

            if (tu == null)
            {
                return NotFound();
            }
            else if (tlio.user_pw != tu.user_pw)
            {
                return BadRequest("Wrong Password");
            }
            else if (tlio.device_uuid.CompareTo(tu.device_uuid) != 0)
            {
                return BadRequest("You changed device");
            }
            else
            {
                string combo = tlio.user_id + tlio.user_pw + tlio.device_uuid.ToString() + tlio.device_token.ToString() + tlio.timestamp + TenLoginObj.COMPANY_CODE;

                SHA256Cng sha256 = new SHA256Cng();
                byte[] hasheValue = sha256.ComputeHash(GetBytes(combo));

                if (tlio.hash_result.CompareTo(hasheValue.ToString()) == 0)
                {
                    return Ok(hasheValue);
                }
                else
                {
                    return BadRequest(hasheValue.ToString());
                }
            }
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
