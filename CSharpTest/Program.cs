using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            checkeDir();
            //long now = TimeUtils.DateTimeToUnixTimestamp(System.DateTime.UtcNow);

            //Console.WriteLine(now);
            //Console.WriteLine(TimeUtils.UnixTimestampToDateTime(new DateTime(),now));

        }
    }
}
