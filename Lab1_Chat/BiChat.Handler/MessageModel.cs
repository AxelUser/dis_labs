using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiChat.Handler
{
    /// <summary>
    /// Класс сообщения.
    /// </summary>
    public class MessageModel
    {
        public const int COUNT_ID = 16; //Потому что Guid
        public const int COUNT_NAME = 256;
        public const int COUNT_TEXT = 1024;

        /// <summary>
        /// Id сообщения.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Имя отправителя.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string Text { get; set; }

        public MessageModel() { }

        /// <summary>
        /// Создание сообщения из массива байт.
        /// </summary>
        /// <param name="bytes"></param>
        public MessageModel(byte[] bytes)
        {
            Id = new Guid(bytes.Take(COUNT_ID).ToArray());
            Name = Encoding.Unicode.GetString(bytes, COUNT_ID, COUNT_NAME).TrimEnd('\0');
            Text = Encoding.Unicode.GetString(bytes, COUNT_ID + COUNT_NAME, COUNT_TEXT).TrimEnd('\0');
        }

        /// <summary>
        /// Получение количества байт для записи сообщения.
        /// </summary>
        /// <returns>Количество байт.</returns>
        public static int GetMaxLengthInBytes()
        {
            return COUNT_ID + COUNT_NAME + COUNT_TEXT;
        }

        /// <summary>
        /// Конвертер сообщения в массив байт.
        /// </summary>
        /// <returns>Сообщение в бинарном виде.</returns>
        public byte[] ToByteArray()
        {
            byte[] newArray = new byte[COUNT_ID + COUNT_NAME + COUNT_TEXT];
            Id.ToByteArray().CopyTo(newArray, 0);
            Encoding.Unicode.GetBytes(Name.Trim()).CopyTo(newArray, COUNT_ID);
            Encoding.Unicode.GetBytes(Text.Trim()).CopyTo(newArray, COUNT_ID + COUNT_NAME);
            return newArray;
        }

        public override bool Equals(object obj)
        {
            MessageModel m = obj as MessageModel;
            if(m == null)
            {
                return false;
            }
            return m.Id == Id && m.Name == Name && m.Text == Text;
        }

        public override string ToString()
        {
            return Name + ": " + Text;
        }
    }
}
