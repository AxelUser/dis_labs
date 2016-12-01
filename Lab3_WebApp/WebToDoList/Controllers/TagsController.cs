using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebToDoList.Controllers
{
    public class TagsController : BaseApiController
    {
        public async Task<IEnumerable<string>> Get()
        {
            return await dalService.GetAllTags();
        }
    }
}
