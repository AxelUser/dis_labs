using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiChat.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiChat.Handler.Tests
{
    [TestClass()]
    public class MessageModelTests
    {
        [TestMethod()]
        public void ToByteArrayTest()
        {
            MessageModel messageValidate = new MessageModel()
            {
                Id = Guid.NewGuid(),
                Name = "TestUser",
                Text = "Hello!"
            };

            byte[] b = messageValidate.ToByteArray();

            MessageModel messageTest = new MessageModel(b);

            Assert.AreEqual<MessageModel>(messageValidate, messageTest);
        }

        [TestMethod()]
        public void GetMaxLengthInBytesTest()
        {
            MessageModel messageValidate = new MessageModel()
            {
                Id = Guid.NewGuid(),
                Name = "TestUser",
                Text = "Hello!"
            };

            byte[] b = messageValidate.ToByteArray();

            Assert.AreEqual(b.Length, MessageModel.GetMaxLengthInBytes());
        }
    }
}