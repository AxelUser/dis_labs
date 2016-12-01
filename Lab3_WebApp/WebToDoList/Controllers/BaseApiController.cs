using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebToDoList.Models;

namespace WebToDoList.Controllers
{
    public class BaseApiController: ApiController
    {
        protected DALService dalService;
        public BaseApiController()
        {
            dalService = new DALService();
        }
    }
}