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
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public async Task<AnswerModel> Post([FromBody] MessageModel message)
        {
            using (HttpClient client = new HttpClient())
            {
                string answerRaw = await client.GetStringAsync("https://yesno.wtf/api");
                var answerModel = JsonConvert.DeserializeObject<AnswerModel>(answerRaw);
                return answerModel;
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
