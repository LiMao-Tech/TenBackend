using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TenBackend.Models;
using Newtonsoft.Json;
using System.IO;

namespace TenBackend.Controllers
{
    public class MsgsController : Controller
    {
        private MsgDbContext db = new MsgDbContext();

        // GET: Msgs
        public async Task<ActionResult> Index()
        {
            return View(await db.Msgs.ToListAsync());
        }

        // GET:
        public string QueryMsgByID(int? id)
        {
            Msg msg = db.Msgs.Find(id);
            Dictionary<string, string> values = new Dictionary<string, string>();
            values["index"] = msg.msg_index.ToString();
            values["sender"] = msg.msg_sender.ToString();
            values["receiver"] = msg.msg_receiver.ToString();
            values["is_locked"] = msg.msg_is_locked.ToString();
            values["type"] = msg.msg_type.ToString();
            values["time"] = msg.msg_time.ToString();
            values["content"] = msg.msg_content;

            return JsonConvert.SerializeObject(values);
        }

        // GET:
        public string QueryMsgBySender(int? sender)
        {
            IQueryable<Msg> msgs = db.Msgs.Where(x => x.msg_sender == sender);

            List<Dictionary<string, string>> retList = new List<Dictionary<string, string>>();
            foreach (Msg msg in msgs)
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                values["index"] = msg.msg_index.ToString();
                values["sender"] = msg.msg_sender.ToString();
                values["receiver"] = msg.msg_receiver.ToString();
                values["is_locked"] = msg.msg_is_locked.ToString();
                values["type"] = msg.msg_type.ToString();
                values["time"] = msg.msg_time.ToString();
                values["content"] = msg.msg_content;
                retList.Add(values);
            }
            return JsonConvert.SerializeObject(retList);
        }

        // GET:
        public string QueryMsgByReceiver(int? receiver)
        {
            IQueryable<Msg> msgs = db.Msgs.Where(x => x.msg_receiver == receiver);

            List<Dictionary<string, string>> retList = new List<Dictionary<string, string>>();
            foreach (Msg msg in msgs)
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                values["index"] = msg.msg_index.ToString();
                values["sender"] = msg.msg_sender.ToString();
                values["receiver"] = msg.msg_receiver.ToString();
                values["is_locked"] = msg.msg_is_locked.ToString();
                values["type"] = msg.msg_type.ToString();
                values["time"] = msg.msg_time.ToString();
                values["content"] = msg.msg_content;
                retList.Add(values);
            }
            return JsonConvert.SerializeObject(retList);
        }

        public string QueryMsgBySenderAndReceiver(int? sender, int? receiver)
        {
            IQueryable<Msg> msgs = db.Msgs.Where(x => x.msg_sender == sender && x.msg_receiver == receiver);

            List<Dictionary<string, string>> retList = new List<Dictionary<string, string>>();
            foreach (Msg msg in msgs)
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                values["index"] = msg.msg_index.ToString();
                values["sender"] = msg.msg_sender.ToString();
                values["receiver"] = msg.msg_receiver.ToString();
                values["is_locked"] = msg.msg_is_locked.ToString();
                values["type"] = msg.msg_type.ToString();
                values["time"] = msg.msg_time.ToString();
                values["content"] = msg.msg_content;
                retList.Add(values);
            }
            return JsonConvert.SerializeObject(retList);
        }

        // POST:
        [HttpPost]
        public string PostMsg()
        {
            string input = new StreamReader(Request.InputStream).ReadToEnd();
            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(input);
        
            Msg msg = new Msg();
            msg.msg_sender = Int32.Parse(values["sender"]);
            msg.msg_receiver = Int32.Parse(values["receiver"]);
            msg.msg_is_locked = Convert.ToBoolean(values["is_locked"]);
            msg.msg_time = Convert.ToDateTime(values["time"]);
            msg.msg_type = Byte.Parse(values["type"]);
            msg.msg_content = values["content"];

            db.Msgs.Add(msg);
            db.SaveChanges();

            Dictionary<string, string> ret = new Dictionary<string,string>();
            ret["status"] = "Sucess";
            string retJson = JsonConvert.SerializeObject(ret);
            return retJson;
        }

        // GET: Msgs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Msg msg = await db.Msgs.FindAsync(id);
            if (msg == null)
            {
                return HttpNotFound();
            }
            return View(msg);
        }

        // GET: Msgs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Msgs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "msg_index,msg_sender,msg_receiver,msg_time,msg_content")] Msg msg)
        {
            if (ModelState.IsValid)
            {
                db.Msgs.Add(msg);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(msg);
        }

        // GET: Msgs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Msg msg = await db.Msgs.FindAsync(id);
            if (msg == null)
            {
                return HttpNotFound();
            }
            return View(msg);
        }

        // POST: Msgs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "msg_index,msg_sender,msg_receiver,msg_time,msg_content")] Msg msg)
        {
            if (ModelState.IsValid)
            {
                db.Entry(msg).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(msg);
        }

        // GET: Msgs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Msg msg = await db.Msgs.FindAsync(id);
            if (msg == null)
            {
                return HttpNotFound();
            }
            return View(msg);
        }

        // POST: Msgs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Msg msg = await db.Msgs.FindAsync(id);
            db.Msgs.Remove(msg);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
