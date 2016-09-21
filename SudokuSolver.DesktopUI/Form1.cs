using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
                var solver = new SudokuPhotoSolver();
                var solution = solver.SolveSudokuPhoto(image);
                var message = solution?.ToString() ?? "Solving failed";

                MessageBox.Show(message);
            }
        }
    }
}
