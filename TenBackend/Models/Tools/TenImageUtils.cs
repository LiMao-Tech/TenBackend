using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace TenBackend.Models.Tools
{
    public class TenImageUtils
    {
        public static void resise2Thumbnail(String src, String des)
        {
            double thumbWidth = 400;

            System.Drawing.Image image = System.Drawing.Image.FromFile(src);
            double srcWidth = image.Width;
            double srcHeight = image.Height;
            double thumbHeight = (srcHeight / srcWidth) * thumbWidth;

            Bitmap bmp = new Bitmap((int)thumbWidth, (int)thumbHeight);
            //从Bitmap创建一个Graphics对象，用来绘制高质量缩略图
            System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //把原始图像绘制成上面所设置宽高的缩小图
            System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, (int)thumbWidth, (int)thumbHeight);
            gr.DrawImage(image, rectDestination, 0, 0, (int)srcWidth, (int)srcHeight, GraphicsUnit.Pixel);
            //保存图像
            bmp.Save(des);

            bmp.Dispose();
            image.Dispose();
        }
    }
}