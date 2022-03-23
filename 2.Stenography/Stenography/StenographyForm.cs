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
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Stenography
{
    public partial class StenographyForm : Form
    {
        public StenographyForm()
        {
            InitializeComponent();
            StenographyForm_Load();
        }

        DocX startWordFile;
        string textToEncode;
        DocX encodedFile;
        Label exceptionLabel;
        Timer timer;

        private void StenographyForm_Load()
        {
            var loadWordFileButton = new Button();
            loadWordFileButton.Size = new Size(100, 30);
            loadWordFileButton.Location = new Point(100, 100);
            loadWordFileButton.Text = "Загрузить файл word";
            loadWordFileButton.Click += LoadWordFileButton_Click;
            this.Controls.Add(loadWordFileButton);

            var loadFileToEncodeButton = new Button();
            loadFileToEncodeButton.Size = new Size(100, 30);
            loadFileToEncodeButton.Location = new Point(100, 150);
            loadFileToEncodeButton.Text = "Загрузить текст";
            loadFileToEncodeButton.Click += LoadFileToEncode_Click;
            this.Controls.Add(loadFileToEncodeButton);

            var saveEncodedFileButton = new Button();
            saveEncodedFileButton.Size = new Size(100, 30);
            saveEncodedFileButton.Location = new Point(100, 200);
            saveEncodedFileButton.Text = "Сохранить";
            saveEncodedFileButton.Click += SaveEncodedFile_Click;
            this.Controls.Add(saveEncodedFileButton);

            var encodeButton = new Button();
            encodeButton.Size = new Size(100, 30);
            encodeButton.Location = new Point(100, 250);
            encodeButton.Text = "Зашифровать";
            encodeButton.Click += EncodeButton_Click;
            this.Controls.Add(encodeButton);

            exceptionLabel = new Label();
            exceptionLabel.Size = new Size(200, 40);
            exceptionLabel.Location = new Point(100, 300);
            exceptionLabel.Text = "";
            exceptionLabel.ForeColor = Color.Red;
            exceptionLabel.Visible = false;
            this.Controls.Add(exceptionLabel);

            timer = new Timer();
            timer.Interval = 2000;
            timer.Tick += (object s, EventArgs ev) => { exceptionLabel.Visible = false; (s as Timer).Stop(); };
        }

        private void LoadFileToEncode_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Файлы txt|*.txt";
            if (openDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                textToEncode = File.ReadAllText(openDialog.FileName, Encoding.UTF8);
                exceptionLabel.Text = "Файл загружен";
                exceptionLabel.Visible = true;
                timer.Start();
            }
            catch (Exception ex)
            {
                exceptionLabel.Text = "Ошибка чтения файла";
                exceptionLabel.Visible = true;
                timer.Start();
                return;
            }
        }

        private void EncodeButton_Click(object sender, EventArgs e)
        {
            var p = startWordFile.Paragraphs[0];
            var mt = p.MagicText;
            var template = p.MagicText[0].formatting;
            var textInWord = p.Text;
            var currentIndex = 0;
            var newText = new List<FormattedText>();
            var newParagraph = startWordFile.InsertParagraph();
            newParagraph.Font("Times New Roman");
            newParagraph.FontSize(14);

            foreach(var symbol in textToEncode)
            {
                while(Char.ToLower(textInWord[currentIndex]) != Char.ToLower(symbol))
                {
                    currentIndex++;
                    if(currentIndex >= textInWord.Length)
                    {
                        exceptionLabel.Text = "Ошибка шифрования. Символ не обнаружен";
                        exceptionLabel.Visible = true;
                        timer.Start();
                        return;
                    }
                }

                var textBefore = textInWord.Substring(0, currentIndex);
                newParagraph.Append(textBefore).Font("Times New Roman").FontSize(14);

                var formattedSymbol = textInWord[currentIndex].ToString();
                newParagraph.Append(formattedSymbol).Font("Times New Roman").FontSize(15);

                textInWord = textInWord.Substring(currentIndex + 1);
                currentIndex = 0;
            }

            if (textInWord.Length != 0)
            {
                newParagraph.Append(textInWord).Font("Times New Roman").FontSize(14);
            }

            encodedFile = startWordFile;
            encodedFile.RemoveParagraph(encodedFile.Paragraphs[0]);

            exceptionLabel.Text = "Текст зашифрован";
            exceptionLabel.Visible = true;
            timer.Start();
        }

        private void LoadWordFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Файлы word|*.doc;*.docx";
            if (openDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                startWordFile = DocX.Load(openDialog.FileName);
                exceptionLabel.Text = "Файл загружен";
                exceptionLabel.Visible = true;
                timer.Start();
            }
            catch (Exception ex)
            {
                exceptionLabel.Text = "Ошибка чтения файла";
                exceptionLabel.Visible = true;
                timer.Start();
                return;
            }
        }

        private void SaveEncodedFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Файлы word|*.doc;*.docx";
            if (saveDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                encodedFile.SaveAs(saveDialog.FileName);
                exceptionLabel.Text = "Файл сохранен";
                exceptionLabel.Visible = true;
                timer.Start();
            }
            catch (Exception ex)
            {
                exceptionLabel.Text = "Ошибка сохранения файла";
                exceptionLabel.Visible = true;
                timer.Start();
                return;
            }
        }

    }
}
