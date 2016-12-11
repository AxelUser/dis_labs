using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YesNoWebApp.Models
{
    public class MessageModel
    {
        public Guid Id { get; set; }
        public string MessageText { get; set; }
        public DateTime ClientDate { get; set; }
    }
}