using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YesNoWebApp.Models
{
    public class AnswerModel
    {
        public bool IsPositive { get; set; }
        public string Answer { get; set; }
        public string Image { get; set; }
        public string Error { get; set; }
    }
}