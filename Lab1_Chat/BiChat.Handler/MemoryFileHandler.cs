using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace BiChat.Handler
{
    /// <summary>
    /// Обработчик для обращения к файлу в памяти.
    /// </summary>
    public class MemoryFileHandler: IDisposable
    {
        public const string DEF_PATH = "D:\\bi_chat.dat"; //хардкод путя к буферному файлу на диске
        public const string ALIAS = "CHAT_FILE"; //псевдоним файла в памяти
        public const int MSG_CAP = 100;
        private MemoryMappedFile file = null; //файл в памяти
        private static MemoryFileHandler singletonInstance = null; //сингтон данного обработчика

        public MemoryFileHandler(string path = null, bool openFromMemory = false)
        {
            string filePath = string.IsNullOrEmpty(path) ? DEF_PATH : path;
            file = GetFileInstance(filePath, openFromMemory);
        }

        /// <summary>
        /// Получить синглитон файла в памяти.
        /// </summary>
        /// <returns>Файл в памяти.</returns>
        private MemoryMappedFile GetFileInstance(string path, bool fromMemory)
        {
            if(file == null)
            {
                try
                {
                    if(fromMemory)
                    {
                        throw new Exception();
                    }
                    file = MemoryMappedFile.CreateFromFile(path, FileMode.OpenOrCreate, ALIAS, MessageModel.GetMaxLengthInBytes() * MSG_CAP);
                }
                catch(Exception e)
                {
                    file = MemoryMappedFile.CreateOrOpen(ALIAS, MessageModel.GetMaxLengthInBytes());
                }

            }
            return file;
        }

        public static MemoryFileHandler CreateInstance(string path = null, bool fromMemory = false)
        {
            singletonInstance = new MemoryFileHandler(path, fromMemory);
            return singletonInstance;
        }

        /// <summary>
        /// Получение синглтона обработчика для работы с файлом в памяти.
        /// </summary>
        /// <returns>Обработчик.</returns>
        public static MemoryFileHandler GetInstance()
        {
            if(singletonInstance == null)
            {
                singletonInstance = new MemoryFileHandler();
            }
            return singletonInstance;
        }

        /// <summary>
        /// Прочитать сообщение из файла.
        /// </summary>
        /// <returns>Сообщение.</returns>
        public MessageModel ReadMessage(int index = 0)
        {
            using (var acc = file.CreateViewAccessor())
            {
                int msgLength = MessageModel.GetMaxLengthInBytes();
                byte[] buffer = new byte[msgLength];
                acc.ReadArray(msgLength * index, buffer, 0, msgLength);
                return buffer.All(b => b == 0) ? null : new MessageModel(buffer);
            }
        }

        public List<MessageModel> ReadAllMessages()
        {
            List<MessageModel> messages = new List<MessageModel>();
            using(var acc = file.CreateViewAccessor())
            {
                MessageModel msg = new MessageModel();
                for (int i = 0; msg != null; i++)
                {
                    msg = ReadMessage(i);
                    if (msg != null)
                    {
                        messages.Add(msg);
                    }
                }
            }
            return messages;
        }

        /// <summary>
        /// Запись сообщения в файл.
        /// </summary>
        /// <param name="message">Сообщение для записи.</param>
        public void WriteMessage(MessageModel message)
        {
            using (var acc = file.CreateViewAccessor())
            {
                acc.WriteArray(0, message.ToByteArray(), 0, MessageModel.GetMaxLengthInBytes());
            }
        }

        public void AppendMessage(MessageModel message)
        {
            using (var acc = file.CreateViewAccessor())
            {
                int count = CountRecords();
                acc.WriteArray(count * MessageModel.GetMaxLengthInBytes(), 
                    message.ToByteArray(), 0, MessageModel.GetMaxLengthInBytes());
            }
        }

        public int CountRecords()
        {
            var temp = new MessageModel();
            return ReachEnd(out temp);            
        }

        public MessageModel ReadLastMessage()
        {
            MessageModel msg = new MessageModel();
            ReachEnd(out msg);
            return msg;
        }

        public static bool CheckFileExistInMemory()
        {
            try
            {
                using (var tempFile = MemoryMappedFile.OpenExisting(ALIAS))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        private int ReachEnd(out MessageModel lastMessge)
        {
            MessageModel msg = new MessageModel();
            lastMessge = null;
            int counter = 0;
            using (var acc = file.CreateViewAccessor())
            {                
                for (counter = 0; msg != null; counter++)
                {
                    msg = ReadMessage(counter);
                    if(msg != null)
                    {
                        lastMessge = msg;
                    }
                    else
                    {
                        return counter;
                    }
                }                
            }
            return 0;
        }

        public void Dispose()
        {
            if(file != null)
            {
                file.Dispose();
            }
        }
    }
}
