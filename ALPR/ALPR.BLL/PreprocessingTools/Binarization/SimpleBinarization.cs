using System.Drawing;

namespace ALPR.BLL.PreprocessingTools.Binarization
{
    public class SimpleBinarization
    {
        public Bitmap BitmapToBlackWhite2(Bitmap src)
        {
            var treshold = 0.3;


            var dst = new Bitmap(src.Width, src.Height);

            for (var i = 0; i < src.Width; i++)
            {
                for (var j = 0; j < src.Height; j++)
                {
                    // 1.
                    dst.SetPixel(i, j, src.GetPixel(i, j).GetBrightness() < treshold ? Color.Black : Color.White);
                }
            }

            return dst;
        }
    }
}