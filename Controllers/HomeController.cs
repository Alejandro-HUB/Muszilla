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
            com.CommandText = "select * from Consumer where Email= '"+acc.Email+"' and Pass_word= '"+acc.Pass_word+"'";
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                con.Close();
                return View("User_Homepage");
            }
            else 
            {
                con.Close();
                ViewBag.Message = "Email or Password Incorrect";
                return View("Index");
            }
        }

        [HttpPost]
        public IActionResult Create(Register add)
        {
            string connection = Muszilla.Properties.Resources.ConnectionString;
            using (SqlConnection con = new SqlConnection(connection))
            {
                string query = "insert into Consumer(FirstName, LastName, Email, Pass_word) values('" + add.FirstName + "', '" + add.LastName + "', '" + add.Email + "', '" + add.Pass_word + "')";
                using (SqlCommand com = new SqlCommand(query, con))
                {
                    con.Open();
                    com.ExecuteNonQuery();
                    ViewBag.Message = "New User inserted succesfully!";
                }
                con.Close();
                return View("Index");
            }
        }

        public IActionResult Logout()
        {
            ViewBag.Message = "Logg out succesfull";
            return View("Index");
        }

    }
}
