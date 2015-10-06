using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ALPR.BLL.PreprocessingTools;
using ALPR.BLL.PreprocessingTools.Binarization;
using ALPR.BLL.PreprocessingTools.Filters;

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

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamReader streamReader = new StreamReader(ofd.FileName);
                var originalBitmap = (Bitmap)Bitmap.FromStream(streamReader.BaseStream);
                streamReader.Close();
                var previewBitmap = originalBitmap.CopyToSquareCanvas(WINDOW_WIDTH);


                pictureBox1.Image = previewBitmap;
                ResetPictureBoxes(new List<PictureBox>() {pictureBox2,pictureBox3, pictureBox4 });
            } 
        }

        private void GrayScaleFilter_Click(object sender, EventArgs e)
        {
            var bitmap = pictureBox1.Image;
            var grayscale = new GrayscaleConverter();
            
            pictureBox2.Image = grayscale.MakeGrayscale3((Bitmap)bitmap);
            ResetPictureBoxes(new List<PictureBox>() { pictureBox3,pictureBox4 });
        }

        private void MedianFilter_Click(object sender, EventArgs e)
        {
            var bitmap = pictureBox2.Image;
            var medianFilter = new MedianFilter();

            pictureBox3.Image = medianFilter.Filter((Bitmap)bitmap, trackBar1.Value);
            ResetPictureBoxes(new List<PictureBox>() { pictureBox4});
            
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
        }

        private void Binarization_Click(object sender, EventArgs e)
        {
           Bitmap temp = (Bitmap)pictureBox3.Image.Clone();
            var sb = new SimpleBinarization();
            pictureBox4.Image = sb.BitmapToBlackWhite2(temp);

        }

        private void ResetPictureBoxes(IEnumerable<PictureBox> pictureBoxes)
        {
            foreach (var pb in pictureBoxes)
            {
                if (pb.Image != null)
                {
                    pb.Image.Dispose();
                    pb.Image = null;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Bitmap temp = (Bitmap)pictureBox3.Image.Clone();

            var ot = new OtsuThresholder();

            var resultBytes = temp.BitmapToByteArray();
            var newBmp = new byte[resultBytes.Length];
            ot.DoThreshold(resultBytes, ref newBmp);




            pictureBox4.Image = newBmp.ByteArrayToBitmap(temp.Width, temp.Height);
        }

        
    }
}
