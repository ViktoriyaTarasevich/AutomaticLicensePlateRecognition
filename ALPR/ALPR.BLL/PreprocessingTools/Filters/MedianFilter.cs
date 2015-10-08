using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ALPR.BLL.PreprocessingTools.Filters
{
    public class MedianFilter
    {
        public Bitmap Filter(Bitmap sourceBitmap,
            int matrixSize,
            int bias = 0,
            bool grayscale = false)
        {
            var sourceData =
                sourceBitmap.LockBits(new Rectangle(0, 0,
                    sourceBitmap.Width, sourceBitmap.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

            var pixelBuffer = new byte[sourceData.Stride*
                                       sourceData.Height];

            var resultBuffer = new byte[sourceData.Stride*
                                        sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0,
                pixelBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);

            if (grayscale)
            {
                float rgb = 0;

                for (var k = 0; k < pixelBuffer.Length; k += 4)
                {
                    rgb = pixelBuffer[k]*0.11f;
                    rgb += pixelBuffer[k + 1]*0.59f;
                    rgb += pixelBuffer[k + 2]*0.3f;


                    pixelBuffer[k] = (byte) rgb;
                    pixelBuffer[k + 1] = pixelBuffer[k];
                    pixelBuffer[k + 2] = pixelBuffer[k];
                    pixelBuffer[k + 3] = 255;
                }
            }

            var filterOffset = (matrixSize - 1)/2;
            var calcOffset = 0;

            var byteOffset = 0;

            var neighbourPixels = new List<int>();
            byte[] middlePixel;

            for (var offsetY = filterOffset;
                offsetY <
                sourceBitmap.Height - filterOffset;
                offsetY++)
            {
                for (var offsetX = filterOffset;
                    offsetX <
                    sourceBitmap.Width - filterOffset;
                    offsetX++)
                {
                    byteOffset = offsetY*
                                 sourceData.Stride +
                                 offsetX*4;

                    neighbourPixels.Clear();

                    for (var filterY = -filterOffset;
                        filterY <= filterOffset;
                        filterY++)
                    {
                        for (var filterX = -filterOffset;
                            filterX <= filterOffset;
                            filterX++)
                        {
                            calcOffset = byteOffset +
                                         (filterX*4) +
                                         (filterY*sourceData.Stride);

                            neighbourPixels.Add(BitConverter.ToInt32(
                                pixelBuffer, calcOffset));
                        }
                    }

                    neighbourPixels.Sort();

                    middlePixel = BitConverter.GetBytes(
                        neighbourPixels[filterOffset]);

                    resultBuffer[byteOffset] = middlePixel[0];
                    resultBuffer[byteOffset + 1] = middlePixel[1];
                    resultBuffer[byteOffset + 2] = middlePixel[2];
                    resultBuffer[byteOffset + 3] = middlePixel[3];
                }
            }

            var resultBitmap = new Bitmap(sourceBitmap.Width,
                sourceBitmap.Height);

            var resultData =
                resultBitmap.LockBits(new Rectangle(0, 0,
                    resultBitmap.Width, resultBitmap.Height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0,
                resultBuffer.Length);

            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }
    }
}