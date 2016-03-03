﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CSharpTest
{
    class Program
    {
         static string IMAGE_PATH = "D:\\TenImages";
        static string ORIGINAL = "Original";
        static string THUMBNAIL = "Thumbnail";




        static void checkeDir(){
            String originalPath = Path.Combine(IMAGE_PATH, ORIGINAL);
            String thumbnailPath = Path.Combine(IMAGE_PATH,THUMBNAIL);
            Console.WriteLine(originalPath);
            Console.WriteLine(thumbnailPath);
            if (!Directory.Exists(originalPath))
            {
                Directory.CreateDirectory(originalPath);
            }
            if (!Directory.Exists(thumbnailPath))
            {
                Directory.CreateDirectory(thumbnailPath);
            }
        }
        static void Main(string[] args)
        {
            //checkeDir();
            //long now = TimeUtils.DateTimeToUnixTimestamp(System.DateTime.UtcNow);

            //Console.WriteLine(now);
            //Console.WriteLine(TimeUtils.UnixTimestampToDateTime(new DateTime(),now));

            //Console.WriteLine(parseLocation(59.3694045162381, 18.0656478367914));
            //Console.ReadLine();
            //Console.WriteLine(string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now));
            //Console.WriteLine(new DateTime(2016, 0, 3).Ticks);
            Console.WriteLine(parseTimeOption(0));
            Console.WriteLine(parseTimeOption(1));
            Console.WriteLine(parseTimeOption(2));
        }

        public static long parseTimeOption(int timeOption)
        {
            DateTime now = DateTime.Now;
            if (timeOption == 0)
            {
                //time       
                return TimeUtils.DateTimeToUnixTimestamp(new DateTime(now.Year, now.Month, now.Day));
            }

            if (timeOption == 1)
            {

                int month = deMonthFormat(now.Month-1);

                return TimeUtils.DateTimeToUnixTimestamp(new DateTime(now.Year, month, now.Day));
            }

            if (timeOption == 2)
            {
                int month = deMonthFormat(now.Month-2);
                return TimeUtils.DateTimeToUnixTimestamp(new DateTime(now.Year, month, now.Day));
            }


            if (timeOption == 3)
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


        static string AK = "qHq7tPP7u2T1TMUmrjTBBQNU";
        static string BAIDU_GEOCODER_API = "http://api.map.baidu.com/geocoder/v2/?";
        static string COORD_TYPE_WGS84LL = "wgs84ll";//GPS经纬度
        static string OUT_PUT = "json";
        static string CAN_NOT_GET = "未知";
        public static string parseLocation(double lati, double longi)
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
               Console.WriteLine(jo.ToString());
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
    }
}