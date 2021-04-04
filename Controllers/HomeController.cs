using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Muszilla.Models;
using System.Data.SqlClient;
using Sitecore.FakeDb;

namespace Muszilla.Controllers
{
    public class HomeController : Controller
    {
        Login login = new Login();
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index([Bind]ConsumerModel acc)
        {
            int res = login.Verify(acc);
            if (res == 1)
            {
                TempData["msg"] = "You are welcome to Admin Section";
            }
            else
            {
                TempData["msg"] = "Admin id or Password is wrong.!";
            }
            return View();
        }
    
    }
}
