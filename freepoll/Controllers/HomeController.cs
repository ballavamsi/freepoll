using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using freepoll.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace freepoll.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("swagger");
        }
    }
}