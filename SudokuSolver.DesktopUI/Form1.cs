using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuSolver.DesktopUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            openFileDialog1.InitialDirectory = Path.GetFullPath(@"..\..\..\TestImages");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using (var fileStream = openFileDialog1.OpenFile())
            using (var image = new Bitmap(fileStream))
            {
                //ImageTransformation transformation = new ImageTransformation();

                //Bitmap grayscaleBitmap = ImageTransformation.ConvertToGreyscale(image);

                //grayscaleBitmap.Save(@"D:\k\trash\grayscaleBitmap.jpg");


                Bitmap transformedImage = ImageTransformation.TranformImage(image);
                transformedImage.Save(@"D:\k\trash\transformedImage.jpg");

                Bitmap thresholdedImage = ImageTransformation.PerformThresholding(transformedImage);
                thresholdedImage.Save(@"D:\k\trash\thresholdedImage.jpg");

                var solver = new SudokuPhotoSolver();
                var solution = solver.SolveSudokuPhoto(thresholdedImage, thresholdedImage);
                var message = solution?.ToString() ?? "Solving failed";

                MessageBox.Show(message);
            }
        }
    }
}
