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
    public class SongsPlaylistController : Controller
    {
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        List<SongsPlaylistModel> sp = new List<SongsPlaylistModel>();

        private readonly ILogger<SongsPlaylistController> _logger;

        public SongsPlaylistController(ILogger<SongsPlaylistController> logger)
        {
            _logger = logger;
            con.ConnectionString = Muszilla.Properties.Resources.ConnectionString; 
        }

        public IActionResult Index()
        {
            FetchData();
            return View(sp);
        }

        private void FetchData()
        {
            if (sp.Count > 0)
            {
                sp.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "Select TOP(100) playlistID,songsID from SongsPlaylistR";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    sp.Add(new SongsPlaylistModel() { playlistID = dr["playlistID"].ToString()
                    ,songsID = dr["songsID"].ToString()
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
