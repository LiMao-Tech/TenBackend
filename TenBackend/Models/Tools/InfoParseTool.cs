using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using TenBackend.Models.Assitants;
using TenBackend.Models.Entities;

namespace TenBackend.Models.Tools
{
    public class InfoParseTool
    {
        public static string parseSex(byte gender)
        {
            if (gender == 0) return "男";
            else return "女";
        }

        public static string parseMarriage(byte marriage)
        {
            if (marriage == 0) return "未婚";
            else return "已婚";
        }

        public static string parseRealUnixTime(long unixTime)
        {
            TimeZoneInfo local = TimeZoneInfo.Local;

            return string.Format("{0:yyyy-MM-dd HH:mm:ss}", TimeUtils.UnixTimestampToDateTime(new DateTime(), unixTime + (long)local.BaseUtcOffset.TotalSeconds));

        }
        public static string parseRealBirthday(long unixTime)
        {
            TimeZoneInfo local = TimeZoneInfo.Local;
            return TimeUtils.UnixTimestampToDateTime(new DateTime(), unixTime + (long)local.BaseUtcOffset.TotalSeconds).ToShortDateString();
        }

        public static int parseLevel(TenUser u)
        {
            return (u.InnerScore + u.OuterScore + u.Energy) / 3;
        }

        static string AK = "qHq7tPP7u2T1TMUmrjTBBQNU";
        static string BAIDU_GEOCODER_API = "http://api.map.baidu.com/geocoder/v2/?";
        static string COORD_TYPE_WGS84LL = "wgs84ll";//GPS经纬度
        static string OUT_PUT = "json";
        static string CAN_NOT_GET = "未知";
        public static string parseLocation(double? lati, double? longi)
        {
            string url = new StringBuilder(BAIDU_GEOCODER_API)
                  .Append("coordtypy=").Append(COORD_TYPE_WGS84LL)
                  .Append("&ak=").Append(AK)
                  .Append("&location=").Append(lati).Append(",").Append(longi)
                  .Append("&output=").Append(OUT_PUT).ToString();
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            //如果主体信息不为空，则接收主体信息内容
            if (response.ContentLength <= 0)
                return CAN_NOT_GET;
            //接收响应主体信息
            using (Stream stream = response.GetResponseStream())
            {
                int totalLength = (int)response.ContentLength;
                int numBytesRead = 0;
                byte[] bytes = new byte[totalLength + 1024];
                //通过一个循环读取流中的数据，读取完毕，跳出循环
                while (numBytesRead < totalLength)
                {
                    int num = stream.Read(bytes, numBytesRead, 1024);  //每次希望读取1024字节
                    if (num == 0)   //说明流中数据读取完毕
                        break;
                    numBytesRead += num;
                }
                JObject jo = JObject.Parse(Encoding.UTF8.GetString(bytes));
                //Console.WriteLine(jo.ToString());
                string status = jo.GetValue("status").ToString();
                if (status == "0")
                {
                    JObject result = JObject.Parse(jo.GetValue("result").ToString());
                    if (result.GetValue("cityCode").ToString() == "0")
                    {
                        return CAN_NOT_GET;
                    }
                    JObject addressComponent = JObject.Parse(result.GetValue("addressComponent").ToString());
                    string country = addressComponent.GetValue("country").ToString();
                    string province = addressComponent.GetValue("province").ToString();
                    string city = addressComponent.GetValue("city").ToString();
                    // Console.WriteLine(city);
                    if (province == "")
                    {
                        return new StringBuilder(country).Append(" ").Append(city).ToString();
                    }


                    return new StringBuilder(province).Append(" ").Append(city).ToString();
                }
                return CAN_NOT_GET;
            }
        }


        public static string parsePhoneType(byte phoneType)
        {
            if (phoneType == Commons.PHONE_TYPE_IPHONE)
            {
                return "iphone";
            }
            else
            {
                return "android";
            }
        }


        public static long parseTimeOption(int timeOption)
        {
            DateTime now = DateTime.Now;           
            if (timeOption == Commons.TIME_OPTION_NOW)
            {
                //time       
                return  TimeUtils.DateTimeToUnixTimestamp(new DateTime(now.Year,now.Month,now.Day));
            }

            if (timeOption == Commons.TIME_OPTION_LAST)
            {

                int month = deMonthFormat(now.Month-1);

                return TimeUtils.DateTimeToUnixTimestamp(new DateTime(now.Year, month, now.Day));
            }

            if (timeOption == Commons.TIME_OPTION_LAST_2)
            {
                int month = deMonthFormat(now.Month-2);
                return TimeUtils.DateTimeToUnixTimestamp(new DateTime(now.Year, month, now.Day));
            }


            if (timeOption == Commons.TIME_OPTION_ALL)
            {
                return 0;
            }

            return 0;
        }

     

        static int deMonthFormat(int month)
        {
            if (month == 0) return 12;
            if (month == -1) return 12;
            return month;
        }

        static int deDayFormat(DateTime now)
        {
            if (now.Day == 1)
            {
                if(now.Month == 1 ||
                    now.Month == 3 ||
                    now.Month == 5 ||
                    now.Month == 7 ||
                    now.Month == 8 ||
                    now.Month == 10 ||
                    now.Month == 12)
                {
                    return 31;
                }


                if (now.Month == 2)
                {
                    if (now.Year % 4 == 0) return 29;
                    else return 28;
                }

                return 30;
                
            }

            return now.Day;
           
        }
    }
}