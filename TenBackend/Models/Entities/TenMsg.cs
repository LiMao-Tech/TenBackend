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

        /// <summary>
        /// 消息发送者的UserIndex,系统消息的消息这里是0
        /// </summary>
        public int Sender { get; set; }

        /// <summary>
        /// 消息接收者的的UserIndex
        /// </summary>
        public int Receiver { get; set; }

        /// <summary>
        /// 电话类型，0为Iphone,1为Android
        /// </summary>
        public byte PhoneType { get; set; }

        /// <summary>
        /// 锁定状态
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// 消息类型，0为系统消息，普通聊天消息,1为图片消息，2为P币消息
        /// </summary>
        public byte MsgType { get; set; }

        /// <summary>
        /// 消息时间
        /// </summary>
        public DateTime MsgTime { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
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