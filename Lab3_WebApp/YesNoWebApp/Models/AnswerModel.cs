using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YesNoWebApp.Models
{
    public class AnswerModel
    {
        [JsonProperty("isPositive", Required = Required.AllowNull)]
        public bool IsPositive { get; set; }
        [JsonProperty("answer")]
        public string Caption { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
    }
}