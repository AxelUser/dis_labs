using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BiChat.Handler;

namespace BiChat.Client
{
    public partial class FormMain : Form
    {
        private WriteHandler writeHandler;
        private ReadHandler readHandler;
        private string username;


        public FormMain()
        {
            InitializeComponent();
        }

        private void ReadHandler_NewMessage(object sender, List<MessageModel> e)
        {
            listBox1.Items.Clear();
            listBox1.Items.AddRange(e.ToArray());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBoxMessageText.Text = textBoxMessageText.Text.Trim();
            string message = textBoxMessageText.Text;
            
            if (!String.IsNullOrEmpty(message))
            {                
                writeHandler.WriteMessage(textBoxMessageText.Text);                
            }
            else
            {
                MessageBox.Show("Введено пустое сообщение!");
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Текстовые файлы (.txt)|*.txt|Все файлы (*.*)|*.*";
            fd.FilterIndex = 0;
            fd.CheckFileExists = false;
            MemoryFileHandler fileHandler = null;
            if (MemoryFileHandler.CheckFileExistInMemory())
            {
                fileHandler = MemoryFileHandler.CreateInstance(fromMemory: true);
            }
            else
            {
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    fileHandler = MemoryFileHandler.CreateInstance(fd.FileName);
                }
                else
                {
                    Close();
                }
            }
            username = UsernameGenerator.CreateName();
            writeHandler = new WriteHandler(username, fileHandler);
            readHandler = new ReadHandler(fileHandler);
            readHandler.NewMessages += ReadHandler_NewMessage;
            readHandler.StartChecking();

            labelUsername.Text = "Имя: " + username;
            textBoxMessageText.MaxLength = Encoding.Unicode.GetMaxCharCount(MessageModel.COUNT_TEXT);
        }
    }
}
