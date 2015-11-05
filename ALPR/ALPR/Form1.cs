using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using ALPR.BLL.PreprocessingTools;
using ALPR.BLL.PreprocessingTools.Binarization;
using ALPR.BLL.PreprocessingTools.EdgeDetection;
using ALPR.BLL.PreprocessingTools.Filters;
using Point = System.Drawing.Point;

namespace ALPR
{
    public partial class Form1 : Form
    {
        private int WINDOW_WIDTH = 300;

        public Form1()
        {
            InitializeComponent();
        }

        private void LoadPicture_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select an image file.";
            ofd.Filter = "Jpeg Images(*.jpg)|*.jpg|Png Images(*.png)|*.png";
            ofd.Filter += "|Bitmap Images(*.bmp)|*.bmp";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                StreamReader streamReader = new StreamReader(ofd.FileName);
                var originalBitmap = (Bitmap) Bitmap.FromStream(streamReader.BaseStream);
                streamReader.Close();
                var previewBitmap = originalBitmap.CopyToSquareCanvas(WINDOW_WIDTH);


                pictureBox1.Image = previewBitmap;
                groupBox1.Visible = true;
                groupBox2.Visible = true;
            }
        }

        private void GrayScaleFilter_Click(object sender, EventArgs e)
        {


            var bitmap = GetPictureBox().Image;
            var grayscale = new GrayscaleConverter();

            pictureBox5.Image = grayscale.MakeGrayscale3((Bitmap) bitmap);
           
        }

        private void MedianFilter_Click(object sender, EventArgs e)
        {
            var bitmap = GetPictureBox().Image;
            var medianFilter = new MedianFilter();

            pictureBox5.Image = medianFilter.Filter((Bitmap) bitmap, trackBar1.Value);
           
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
        }

        private void Binarization_Click(object sender, EventArgs e)
        {
            Bitmap temp = (Bitmap)GetPictureBox().Image.Clone();
            var sb = new SimpleBinarization();
            pictureBox5.Image = sb.BitmapToBlackWhite2(temp);
        }

        

        private void OtsuBinarization_Click(object sender, EventArgs e)
        {
            Bitmap temp = (Bitmap)GetPictureBox().Image.Clone();

            var ot = new OtsuThresholder();

            var resultBytes = temp.BitmapToByteArray();
            var newBmp = new byte[resultBytes.Length];
            ot.DoThreshold(resultBytes, ref newBmp);


            pictureBox5.Image = newBmp.ByteArrayToBitmap(temp.Width, temp.Height);
        }

        private PictureBox GetPictureBox()
        {
            if (pictureBox5.Image == null)
                return pictureBox1;
            return pictureBox5;
        }

        private void ResetPicture_Click(object sender, EventArgs e)
        {
            pictureBox5.Image = pictureBox1.Image;
        }

        private void GaussFilter_Click(object sender, EventArgs e)
        {
            var bitmap = GetPictureBox().Image;
            var gaussFilter = new GaussFilter();

            pictureBox5.Image = gaussFilter.FilterProcessImage( trackBar2.Value,(Bitmap)bitmap);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label3.Text = trackBar2.Value.ToString();
        }

        private void EdgeDetection_Click(object sender, EventArgs e)
        {
            var bitmap = GetPictureBox().Image;
            pictureBox5.Image = EdgeDetectionDifference.EdgeDetectDifference((Bitmap) bitmap, (byte) trackBar3.Value);
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void ProcessImage(Bitmap bitmap)
        {
            // lock image
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // step 1 - turn background to black
            ColorFiltering colorFilter = new ColorFiltering();

            colorFilter.Red = new IntRange(0, 64);
            colorFilter.Green = new IntRange(0, 64);
            colorFilter.Blue = new IntRange(0, 64);
            colorFilter.FillOutsideRange = false;

            colorFilter.ApplyInPlace(bitmapData);

            // step 2 - locating objects
            BlobCounter blobCounter = new BlobCounter();

            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 5;
            blobCounter.MinWidth = 5;

            blobCounter.ProcessImage(bitmapData);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            bitmap.UnlockBits(bitmapData);

   
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            Graphics g = Graphics.FromImage(bitmap);
           

            Pen brownPen = new Pen(Color.Red, 3); 


            for (int i = 0, n = blobs.Length; i < n; i++)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);


                List<IntPoint> corners;

             
                if (shapeChecker.IsConvexPolygon(edgePoints, out corners))
                {
                    
                    PolygonSubType subType = shapeChecker.CheckPolygonSubType(corners);

                    
                    if (subType == PolygonSubType.Unknown)
                    {
                        if (corners.Count == 4)
                        {
                            g.DrawPolygon(brownPen, ToPointsArray(corners));
                        }
                        
                    }
                    else
                    {
                        if (corners.Count == 4)
                        {
                            g.DrawPolygon(brownPen, ToPointsArray(corners));
                        }
                    }

                    
                }
            }

            brownPen.Dispose();
            g.Dispose();

           
            Clipboard.SetDataObject(bitmap);
           
            pictureBox5.Image = bitmap;

           
        }

        

       
        private Point[] ToPointsArray(List<IntPoint> points)
        {
            Point[] array = new Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new Point(points[i].X, points[i].Y);
            }

            return array;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var bitmap = GetPictureBox().Image.Clone();
            ProcessImage((Bitmap)bitmap);
        }
    }
}