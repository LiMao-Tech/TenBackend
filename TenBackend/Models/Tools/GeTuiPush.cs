using com.igetui.api.openservice;
using com.igetui.api.openservice.igetui;
using com.igetui.api.openservice.igetui.template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Tools
{
    public class GeTuiPush
    {
        //参数设置 <-----参数需要重新设置----->
        //http的域名
        private static String HOST = "http://sdk.open.api.igexin.com/apiex.htm";

        //https的域名
        //private static String HOST = "https://api.getui.com/apiex.htm";


        private static String APPID = "H4NIc2ohOVAkOvrsWXWMj1";                     //您应用的AppId
        private static String APPKEY = "hpglcwA9Qe5oQsYMdfPeyA";                    //您应用的AppKey
        private static String MASTERSECRET = "8QszYosrOa7Gs21TEmJW5A";              //您应用的MasterSecret 

        private IGtPush m_push = new IGtPush(HOST, APPKEY, MASTERSECRET);

        private volatile static GeTuiPush _instance = null;
        private static readonly object lockHelper = new object();


        public static GeTuiPush GetInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                    {
                        _instance = new GeTuiPush();

                    }
                }
            }

            return _instance;
        }

        public void PushMessageToSingle(String title, String text, String logoUrl,String cilentId)
        {

            /*消息模版：
                1.TransmissionTemplate:透传模板
                2.LinkTemplate:通知链接模板
                3.NotificationTemplate：通知透传模板
                4.NotyPopLoadTemplate：通知弹框下载模板
             */

            // TransmissionTemplate template =  TransmissionTemplateDemo();
            NotificationTemplate template = NotificationTemplateDemo(title,text,logoUrl);
            //LinkTemplate template = LinkTemplateDemo();
            //NotyPopLoadTemplate template = NotyPopLoadTemplateDemo();
           // template.TransmissionContent = "测试";

            // 单推消息模型
            SingleMessage message = new SingleMessage();
            message.IsOffline = true;                         // 用户当前不在线时，是否离线存储,可选
            message.OfflineExpireTime = 1000 * 3600 * 12;            // 离线有效时间，单位为毫秒，可选
            message.Data = template;
            message.PushNetWorkType = 0;        //判断是否客户端是否wifi环境下推送，1为在WIFI环境下，0为非WIFI环境

            com.igetui.api.openservice.igetui.Target target = new com.igetui.api.openservice.igetui.Target();
            target.appId = APPID;
            target.clientId = cilentId;
            

            try
            {
                m_push.pushMessageToSingle(message, target);

            }
            catch (RequestException e)
            {
                throw e;
            }
        }

        //通知透传模板动作内容
        public static NotificationTemplate NotificationTemplateDemo(String title,String text,String logoUrl)
        {
            NotificationTemplate template = new NotificationTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            template.Title = title;         //通知栏标题
            template.Text = text;          //通知栏内容
            template.Logo = "";               //通知栏显示本地图片
            template.LogoURL = logoUrl;                    //通知栏显示网络图标

            template.TransmissionType = "1";          //应用启动类型，1：强制应用启动  2：等待应用启动
            template.TransmissionContent = "请填写透传内容";   //透传内容

            //设置客户端展示时间
            //String begin = "2015-03-06 14:36:10";
            //String end = "2015-03-06 14:46:20";
            //template.setDuration(begin, end);

            template.IsRing = true;                //接收到消息是否响铃，true：响铃 false：不响铃
            template.IsVibrate = true;               //接收到消息是否震动，true：震动 false：不震动
            template.IsClearable = true;             //接收到消息是否可清除，true：可清除 false：不可清除
            return template;
        }
    }
}