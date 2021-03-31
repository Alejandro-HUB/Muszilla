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
    public class ConsumerController : Controller
    {
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        List<ConsumerModel> consumers = new List<ConsumerModel>();

        private readonly ILogger<HomeController> _logger;

        public ConsumerController(ILogger<HomeController> logger)
        {
            _logger = logger;
            con.ConnectionString = Muszilla.Properties.Resources.ConnectionString; 
        }

        public IActionResult Index()
        {
            FetchData();
            return View(consumers);
        }

        private void FetchData()
        {
            if (consumers.Count > 0)
            {
                consumers.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "Select TOP(100) User_ID,USERNAME,FirstName,LastName,Email,Pass_word,CreatedDate,Old_Pass_word from Consumer";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    consumers.Add(new ConsumerModel() { User_ID = dr["User_ID"].ToString()
                    ,USERNAME = dr["USERNAME"].ToString()
                    ,FirstName = dr["FirstName"].ToString()
                    ,LastName = dr["LastName"].ToString()
                    ,Email = dr["Email"].ToString()
                    ,Pass_word = dr["Pass_word"].ToString()
                    ,CreatedDate = dr["CreatedDate"].ToString()
                    ,Old_Pass_word = dr["Old_Pass_word"].ToString()
                    });
                }
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
