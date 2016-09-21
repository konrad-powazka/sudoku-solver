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
        private Bitmap _sourceSudokuBitmap;
        private Bitmap _solvedSudokuBitmap;

        public Form1()
        {
            InitializeComponent();
            openFileDialog1.InitialDirectory = Path.GetFullPath(@"..\..\..\TestImages");
        }

        private void loadSourceImageButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var fileStream = openFileDialog1.OpenFile();
            var previousSourceSudokuBitmap = _sourceSudokuBitmap;
            _sourceSudokuBitmap = new Bitmap(fileStream);
            sourceImagePictureBox.Image = _sourceSudokuBitmap;
            solveButton.Enabled = true;
            previousSourceSudokuBitmap?.Dispose();
        }

        private void solveButton_Click(object sender, EventArgs e)
        {
            solvedImagePictureBox.Image = null;
            _solvedSudokuBitmap?.Dispose();
            var solver = new SudokuPhotoSolver();
            var solvedSudokuBitmap = solver.SolveSudokuPhoto(_sourceSudokuBitmap);

            if (solvedSudokuBitmap != null)
            {
                _solvedSudokuBitmap = solvedSudokuBitmap;
                solvedImagePictureBox.Image = _solvedSudokuBitmap;
            }
            else
            {
                MessageBox.Show("Solving failed");
            }
        }
    }
}
