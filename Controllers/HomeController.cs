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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Options;
using Muszilla.Helpers;
using System.IO;

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
            string fn = "";
            string ln = "";
            string url = "";
            ConnectionString();
            con.Open();
            com.Connection = con;
            //com.CommandText = "select * from Consumer where Email= '" + acc.Email + "' and Pass_word= '" + acc.Pass_word + "'";
            com.CommandText = "select FirstName, LastName, Picture from Consumer where Email = '" + acc.Email + "'and Pass_word= '" + acc.Pass_word + "'";
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                fn = dr["FirstName"].ToString();
                ln = dr["LastName"].ToString();
                url = dr["Picture"].ToString();
                con.Close();
                HttpContext.Session.SetString("Email", acc.Email);
                HttpContext.Session.SetString("Pass_word", acc.Pass_word);
                HttpContext.Session.SetString("FirstName", fn);
                HttpContext.Session.SetString("LastName", ln);
                HttpContext.Session.SetString("Picture", url);
                return RedirectToAction("Homepage");
            }
            else
            {
                con.Close();
                ViewBag.Message = "Email or Password Incorrect";
                return View("Index");
            }
        }

        public IActionResult Homepage()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.Pass_word = HttpContext.Session.GetString("Pass_word");
                ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
                ViewBag.LastName = HttpContext.Session.GetString("LastName");
                ViewBag.Picture = HttpContext.Session.GetString("Picture");
                return View("User_Homepage");
            }
            return View();
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
            ViewBag.Message = "Log out successful!";
            return View("Index");
        }
    }
}
       