using InformationSequriry_01.Core;
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

namespace InformationSequriry_01
{
    public partial class Form1 : Form
    {
        PictureBox startImageBox;
        PictureBox finalImageBox;
        Label exceptionLabel;

        byte[] startFile;
        byte[] extendedFile;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var loadImageButton = new Button();
            loadImageButton.Size = new Size(100,20);
            loadImageButton.Location = new Point(256, 600);
            loadImageButton.Text = "Загрузить изображение";
            loadImageButton.Click += LoadImageButton_Click;
            this.Controls.Add(loadImageButton);

            startImageBox = new PictureBox();
            startImageBox.Size = new Size(512, 512);
            startImageBox.Location = new Point(50, 50);
            this.Controls.Add(startImageBox);

            finalImageBox = new PictureBox();
            finalImageBox.Size = new Size(512, 512);
            finalImageBox.Location = new Point(680, 50);
            this.Controls.Add(finalImageBox);

            // var countPSNRButton = new Button();
            // countPSNRButton.Size = new Size(100, 20);
            // countPSNRButton.Location = new Point(580, 610);
            // countPSNRButton.Text = "PSNR";
            // countPSNRButton.Click += CountPSNR_Click;
            // this.Controls.Add(countPSNRButton);

            exceptionLabel = new Label();
            exceptionLabel.Size = new Size(200, 40);
            exceptionLabel.Location = new Point(690, 590);
            exceptionLabel.Text = "Ошибка. Изображения имеют разные размеры.";
            exceptionLabel.ForeColor = Color.Red;
            exceptionLabel.Visible = false;
            this.Controls.Add(exceptionLabel);

            var timer = new Timer();
            timer.Interval = 5000;
            timer.Tick += (object s, EventArgs ev) => { exceptionLabel.Visible = false; };
            timer.Start();
        }

        
        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Файлы изображений|*.bmp;*.png;*.jpg";
            if (openDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                var bitmap = LoadBitmap(openDialog.FileName);
                pictureBox.Image = bitmap;
            }
            catch (OutOfMemoryException ex)
            {
                MessageBox.Show("Ошибка чтения картинки");
                return;
            }
        }

        public static Bitmap LoadBitmap(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                return new Bitmap(fs);
        }
    }
}
