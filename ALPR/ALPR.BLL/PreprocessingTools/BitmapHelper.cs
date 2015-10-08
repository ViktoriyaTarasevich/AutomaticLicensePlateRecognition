using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ALPR.BLL.PreprocessingTools
{
    public static class BitmapHelper
    {
        public static Bitmap CopyToSquareCanvas(this Bitmap sourceBitmap, int canvasWidthLenght)
        {
            var ratio = 1.0f;
            var maxSide = sourceBitmap.Width > sourceBitmap.Height
                ? sourceBitmap.Width
                : sourceBitmap.Height;

            ratio = maxSide/(float) canvasWidthLenght;

            var bitmapResult = (sourceBitmap.Width > sourceBitmap.Height
                ? new Bitmap(canvasWidthLenght, (int) (sourceBitmap.Height/ratio))
                : new Bitmap((int) (sourceBitmap.Width/ratio), canvasWidthLenght));

            using (var graphicsResult = Graphics.FromImage(bitmapResult))
            {
                graphicsResult.CompositingQuality = CompositingQuality.HighQuality;
                graphicsResult.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsResult.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphicsResult.DrawImage(sourceBitmap,
                    new Rectangle(0, 0,
                        bitmapResult.Width, bitmapResult.Height),
                    new Rectangle(0, 0,
                        sourceBitmap.Width, sourceBitmap.Height),
                    GraphicsUnit.Pixel);
                graphicsResult.Flush();
            }

            return bitmapResult;
        }

        public static byte[] BitmapToByteArray(this Bitmap bitmap)
        {
            var bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var numbytes = bmpdata.Stride*bitmap.Height;
            var bytedata = new byte[numbytes];
            var ptr = bmpdata.Scan0;

            Marshal.Copy(ptr, bytedata, 0, numbytes);

            bitmap.UnlockBits(bmpdata);

            return bytedata;
        }

        public static Bitmap ByteArrayToBitmap(this byte[] arr, int width, int height)
        {
            var resultBitmap = new Bitmap(width,
                height);

            var resultData =
                resultBitmap.LockBits(new Rectangle(0, 0,
                    resultBitmap.Width, resultBitmap.Height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);

            Marshal.Copy(arr, 0, resultData.Scan0,
                arr.Length);

            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }
    }
}