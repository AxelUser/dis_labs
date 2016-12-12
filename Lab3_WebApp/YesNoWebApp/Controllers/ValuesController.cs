using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using YesNoWebApp.Models;

namespace YesNoWebApp.Controllers
{
    public class ValuesController : ApiController
    {
        // POST api/values
        public async Task<AnswerModel> Post([FromBody] MessageModel message)
        {
            if (!string.IsNullOrEmpty(message?.MessageText?.Trim()))
            {
                if (!message.MessageText.TrimEnd().Contains("?"))
                {
                    return new AnswerModel()
                    {
                        Error = "LOL, questions must contains question marks, bro!"
                    };
                }
                using (HttpClient client = new HttpClient())
                {
                    string answerRaw = await client.GetStringAsync("https://yesno.wtf/api");
                    var answerModel = JsonConvert.DeserializeObject<AnswerModel>(answerRaw);
                    return answerModel;
                }
            }
            else
            {
                return new AnswerModel()
                {
                    Error = "Question is not valid, dude!"
                };
            }
        }
    }
}
