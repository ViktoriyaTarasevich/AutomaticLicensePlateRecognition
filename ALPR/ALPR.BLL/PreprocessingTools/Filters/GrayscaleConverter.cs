using System.Drawing;
using System.Drawing.Imaging;

namespace ALPR.BLL.PreprocessingTools.Filters
{
    public class GrayscaleConverter
    {
        public Bitmap MakeGrayscale3(Bitmap original)
        {
            var newBitmap = new Bitmap(original.Width, original.Height);


            var g = Graphics.FromImage(newBitmap);


            var colorMatrix = new ColorMatrix(
                new[]
                {
                    new[] {.3f, .3f, .3f, 0, 0},
                    new[] {.59f, .59f, .59f, 0, 0},
                    new[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

            var attributes = new ImageAttributes();


            attributes.SetColorMatrix(colorMatrix);


            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            g.Dispose();
            return newBitmap;
        }
    }
}