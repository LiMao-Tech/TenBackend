using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TenBackend.Models.Assitants;

namespace TenBackend.Models.Tools
{
    public class TenEmailHelper
    {
        private volatile static TenEmailHelper _instance = null;
        private static readonly object lockHelper = new object();
        
        static string GMAIL_SMTP_ADDR = "smtp.gmail.com";
        static int GMAIL_SMTP_PORT = 587;
        static string EMAIL_ACCOUT= "mingshuai001@gmail.com";
        static string EMAIL_PASSWORD = "shyboy123";
        static string EMAIL_TITLE = "Ten账户激活";
        static string EMAIL_BODY = "欢迎您使用Ten,请点击此链接以绑定您的账户：";
        static string VALIDATE_API = "api/BindInfo?validate=";
        

        private SmtpClient client = new SmtpClient(GMAIL_SMTP_ADDR, GMAIL_SMTP_PORT);
        private TenEmailHelper() {
            client.Credentials = new NetworkCredential(EMAIL_ACCOUT, EMAIL_PASSWORD);
            client.EnableSsl = true;
        }

        public static TenEmailHelper GetInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                    {
                        _instance =  new TenEmailHelper();

                    }
                }
            }

            return _instance;
        }

        public  void SendValidateEmail(string recEmail,string validateStr)
        {   
            string content = new StringBuilder(EMAIL_BODY)
                .Append(Commons.TEN_HOME)
                .Append(VALIDATE_API)
                .Append(validateStr)
                .ToString();

            client.Send(EMAIL_ACCOUT,recEmail,EMAIL_TITLE,content);
           
        }

    }
}