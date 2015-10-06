using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALPR.BLL.PreprocessingTools.Binarization
{
    public class SimpleBinarization
    {
        public Bitmap BitmapToBlackWhite2(Bitmap src)
        {
          
            double treshold = 0.3;

          

            Bitmap dst = new Bitmap(src.Width, src.Height);

            for (int i = 0; i < src.Width; i++)
            {
                for (int j = 0; j < src.Height; j++)
                {
                    // 1.
                    dst.SetPixel(i, j, src.GetPixel(i, j).GetBrightness() < treshold ? Color.Black : Color.White);

                }
            }

            return dst;
        }


    }
}
