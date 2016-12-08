using MessageEncryptionService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebToDoList.Models;

namespace WebToDoList.Controllers
{
    public class TasksController : BaseApiController
    {
        public async Task<IEnumerable<TaskViewModel>> Get()
        {
            return await dalService.GetAllTasks();
        }

        public async void Post([FromBody]TaskViewModel model)
        {
            await dalService.AddRecord(model);
        }
    }
}
