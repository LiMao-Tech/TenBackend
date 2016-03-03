using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Assitants
{
    public class Commons
    {
        public static byte PHONE_TYPE_IPHONE = 0;
        public static byte PHONE_TYPE_ANDROID = 1;


        public static byte MSG_TYPE_SYSTEM = 0;
        public static byte MSG_TYPE_USER = 0;
        public static byte MSG_TYPE_IMAGE = 1;
        public static byte MSG_TYPE_PCOIN = 2;

        public const int PURCHASE_TYPE_APPLE = 0 ;
        public const int PURCHASE_TYPE_UNLOCK_LEVEL = 1;
        public const int PURCHASE_TYPE_UNLOCK_IMAGE = 2;
        public const int PURCHASE_TYPE_PCOIN_TRANS = 3;

        public static string TEN_HOME = "http://www.limao-tech.com/Ten/";

        public static int TIME_OPTION_NOW = 0;
        public static int TIME_OPTION_LAST = 1;
        public static int TIME_OPTION_LAST_2 = 2;
        public static int TIME_OPTION_ALL = 3;


        public static decimal PCOIN_PRICE = 10;//1美元 10 P币
        public static decimal LEVEL_PRICE = 10;//1级10P，2级20P
    }
}