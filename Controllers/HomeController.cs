using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Muszilla.Models;
using System.Data.SqlClient;

namespace Muszilla.Controllers
{
    public class HomeController : Controller
    {
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            con.ConnectionString = Muszilla.Properties.Resources.ConnectionString;
        }

        public IActionResult Index()
        {
            return View();
        }

        private void FetchData()
        {
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "Select TOP(100)[Song_ID],[Song_Name],[Song_Artist],[Song_Length],[Song_Genre] from Songs";
                dr = com.ExecuteReader();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
