using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiChat.Handler
{
    /// <summary>
    /// Обработчик для чтения сообщения из файла.
    /// </summary>
    public class ReadHandler
    {
        private MemoryFileHandler fileHandler;
        public event EventHandler<List<MessageModel>> NewMessages;
        private bool checking;
        private BackgroundWorker worker;
        private Guid? currentId = null;


        public ReadHandler(MemoryFileHandler fileHandler = null)
        {
            this.fileHandler = fileHandler ?? MemoryFileHandler.GetInstance();
            checking = false;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var messages = e.UserState as List<MessageModel>;
            if(messages != null)
            {
                if(NewMessages != null)
                {
                    NewMessages(this, messages);
                }
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!worker.CancellationPending)
            {
                var messges = CheckForNewMessages();
                if (messges.Count > 0)
                {
                    worker.ReportProgress(0, messges);
                }
            }
        }

        public void StartChecking()
        {
            checking = true;
            worker.RunWorkerAsync();
        }

        public void StopChecking()
        {
            checking = false;
            worker.CancelAsync();
        }


        private List<MessageModel> CheckForNewMessages()
        {
            MessageModel m = fileHandler.ReadLastMessage();
            if(m != null && m.Id != currentId)
            {
                currentId = m.Id;
                return fileHandler.ReadAllMessages();
            }
            else
            {
                return new List<MessageModel>();
            }
        }
    }
}