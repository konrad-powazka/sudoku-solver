namespace SudokuSolver.DesktopUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.solveButton = new System.Windows.Forms.Button();
            this.loadSourceImageButton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.sourceImagePictureBox = new System.Windows.Forms.PictureBox();
            this.solvedImagePictureBox = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sourceImagePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.solvedImagePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.InitialDirectory = "D:\\k\\";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.solveButton);
            this.panel1.Controls.Add(this.loadSourceImageButton);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(687, 63);
            this.panel1.TabIndex = 1;
            // 
            // solveButton
            // 
            this.solveButton.Enabled = false;
            this.solveButton.Location = new System.Drawing.Point(156, 3);
            this.solveButton.Name = "solveButton";
            this.solveButton.Size = new System.Drawing.Size(140, 57);
            this.solveButton.TabIndex = 1;
            this.solveButton.Text = "Solve";
            this.solveButton.UseVisualStyleBackColor = true;
            this.solveButton.Click += new System.EventHandler(this.solveButton_Click);
            // 
            // loadSourceImageButton
            // 
            this.loadSourceImageButton.Location = new System.Drawing.Point(3, 3);
            this.loadSourceImageButton.Name = "loadSourceImageButton";
            this.loadSourceImageButton.Size = new System.Drawing.Size(147, 57);
            this.loadSourceImageButton.TabIndex = 0;
            this.loadSourceImageButton.Text = "Load Sudoku photo";
            this.loadSourceImageButton.UseVisualStyleBackColor = true;
            this.loadSourceImageButton.Click += new System.EventHandler(this.loadSourceImageButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(15, 78);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.sourceImagePictureBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.solvedImagePictureBox);
            this.splitContainer1.Size = new System.Drawing.Size(684, 334);
            this.splitContainer1.SplitterDistance = 339;
            this.splitContainer1.TabIndex = 4;
            // 
            // sourceImagePictureBox
            // 
            this.sourceImagePictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceImagePictureBox.Location = new System.Drawing.Point(0, 0);
            this.sourceImagePictureBox.Name = "sourceImagePictureBox";
            this.sourceImagePictureBox.Size = new System.Drawing.Size(339, 334);
            this.sourceImagePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.sourceImagePictureBox.TabIndex = 0;
            this.sourceImagePictureBox.TabStop = false;
            // 
            // solvedImagePictureBox
            // 
            this.solvedImagePictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.solvedImagePictureBox.Location = new System.Drawing.Point(0, 0);
            this.solvedImagePictureBox.Name = "solvedImagePictureBox";
            this.solvedImagePictureBox.Size = new System.Drawing.Size(341, 334);
            this.solvedImagePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.solvedImagePictureBox.TabIndex = 0;
            this.solvedImagePictureBox.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 415);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Sudoku Solver";
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sourceImagePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.solvedImagePictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button loadSourceImageButton;
        private System.Windows.Forms.Button solveButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox sourceImagePictureBox;
        private System.Windows.Forms.PictureBox solvedImagePictureBox;
    }
}

