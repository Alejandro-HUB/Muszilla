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
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        public IActionResult Index()
        {
            return View();
        }
        public void ConnectionString()
        {
            con.ConnectionString = Muszilla.Properties.Resources.ConnectionString;
        }

        public IActionResult Verify(ConsumerModel acc)
        {
            ConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from Consumer where Email= '"+acc.Email+"'";
            //command.CommandText = "select * from Consumer where Pass_word= '" + acc.Pass_word+ "'";
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                con.Close();
                return View("Create");
            }
            else 
            {
                con.Close();
                return View("Error");
            }
        }
    
    }
}
