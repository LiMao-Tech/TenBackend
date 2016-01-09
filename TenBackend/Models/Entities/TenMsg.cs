using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Entities
{
    public class TenMsg
    {
        [Key]
        public int MsgIndex { get; set; }

        public int Sender { get; set; }

        public int Receiver { get; set; }

        public byte PhoneType { get; set; }

        public bool IsLocked { get; set; }

        public byte MsgType { get; set; }

        public DateTime MsgTime { get; set; }

        [Required]
        [StringLength(512)]
        public string MsgContent { get; set; }

        /*
        public TenMsg(int sender, int receiver, byte phoneType,
                        bool isLocked, DateTime msgTime,
                        string msgContent)
        {
            Sender = sender;
            Receiver = receiver;
            PhoneType = phoneType;
            IsLocked = isLocked;
            MsgTime = msgTime;
            MsgContent = msgContent;
        }
         * */
    }
}