using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiChat.Handler
{
    public class WriteHandler
    {
        private MemoryFileHandler fileHandler;
        private string name;


        public WriteHandler(string userName, MemoryFileHandler fileHandler = null)
        {                        
            this.fileHandler = fileHandler ?? MemoryFileHandler.GetInstance();
            name = userName;
        }

        public void WriteMessage(string text)
        {
            MessageModel m = new MessageModel()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Text = text
            };
            fileHandler.AppendMessage(m);
        }
    }
}
