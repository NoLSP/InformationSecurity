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

namespace AESPrepare
{
    public partial class Form1 : Form
    {
        PictureBox startImageBox;
        PictureBox finalImageBox;
        Label exceptionLabel;
        Timer timer;

        byte[] startFile;
        byte[] extendedFile;


        public Form1()
        {
            InitializeComponent();
            Form1_Load();
        }

        private void Form1_Load()
        {
            var loadImageButton = new Button();
            loadImageButton.Size = new Size(100, 30);
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

            var prepareButton = new Button();
            prepareButton.Size = new Size(100, 30);
            prepareButton.Location = new Point(580, 560);
            prepareButton.Text = "Prepare";
            prepareButton.Click += Prepare_Click;
            this.Controls.Add(prepareButton);

            var encriptButton = new Button();
            encriptButton.Size = new Size(100, 30);
            encriptButton.Location = new Point(580, 600);
            encriptButton.Text = "Encrypt";
            encriptButton.Click += Encrypt_Click;
            this.Controls.Add(encriptButton);

            var decryptButton = new Button();
            decryptButton.Size = new Size(100, 30);
            decryptButton.Location = new Point(580, 640);
            decryptButton.Text = "Decrypt";
            decryptButton.Click += Decrypt_Click;
            this.Controls.Add(decryptButton);

            var translateButton = new Button();
            translateButton.Size = new Size(100, 30);
            translateButton.Location = new Point(580, 680);
            translateButton.Text = "Translate";
            translateButton.Click += Translate_Click;
            this.Controls.Add(translateButton);

            exceptionLabel = new Label();
            exceptionLabel.Size = new Size(200, 40);
            exceptionLabel.Location = new Point(690, 590);
            exceptionLabel.Text = "Ошибка. Изображения имеют разные размеры.";
            exceptionLabel.ForeColor = Color.Red;
            exceptionLabel.Visible = false;
            this.Controls.Add(exceptionLabel);

            timer = new Timer();
            timer.Interval = 2000;
            timer.Tick += (object s, EventArgs ev) => { exceptionLabel.Visible = false; (s as Timer).Stop(); };
        }

        private void Prepare_Click(object sender, EventArgs e)
        {
            var n = 16;

            var dictionary = GetDictionary(n);

            var image = PrepareImage(startFile, n);

            dictionary.AddRange(image);
            extendedFile = dictionary.ToArray();

            exceptionLabel.Text = "Изображение расширено";
            exceptionLabel.Visible = true;
            timer.Start();
        }

        private void Decrypt_Click(object sender, EventArgs e)
        {
            

            exceptionLabel.Text = "Изображение расшифровано";
            exceptionLabel.Visible = true;
            timer.Start();
        }

        private void Translate_Click(object sender, EventArgs e)
        {


            exceptionLabel.Text = "Изображение расшифровано";
            exceptionLabel.Visible = true;
            timer.Start();
        }

        private List<byte> GetDictionary(int n)
        {
            var dictionary = new List<byte>();

            for(var i = 0; i <= 255; i++)
            {
                dictionary.Add(Convert.ToByte(i));
                for(var j = 1; j <= n-1; j++)
                {
                    dictionary.Add(0);
                }
            }

            return dictionary;
        }

        private List<byte> PrepareImage(byte[] image, int n)
        {
            var preparedImage = new List<byte>();

            foreach (var i in image)
            {
                preparedImage.Add(Convert.ToByte(i));
                for (var j = 1; j <= n - 1; j++)
                {
                    preparedImage.Add(0);
                }
            }

            return preparedImage;
        }

        private void Encrypt_Click(object sender, EventArgs e)
        {
            byte[] encriptedImage = null;

            if (extendedFile != null)
                encriptedImage = CryptoHelper.EncryptStringToBytes_Aes(Encoding.UTF8.GetString(extendedFile));

            if (encriptedImage != null)
            {
                exceptionLabel.Text = "Изображение зашифровано";
                exceptionLabel.Visible = true;
                timer.Start();
            }
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
                startImageBox.Image = bitmap;
                startFile = ImageToByteArray(bitmap);
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

        public byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
        
        public Image ByteArrayToImage(byte[] array)
        {
            using (var ms = new MemoryStream(array))
            {
                return Image.FromStream(ms);
            }
        }
    }
}
