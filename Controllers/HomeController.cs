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
using javax.jws;

namespace Muszilla.Controllers                                                            //**This controller handles the CRUD functionalities** By Alejandro Lopez
{
    public class HomeController : Controller
    {
        //Connection for login
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        //Connection for upload music
        SqlConnection connect = new SqlConnection();
        SqlCommand command = new SqlCommand();
        SqlDataReader read;
        PlaylistModel playlistModel = new PlaylistModel();
        SongsModel songsModel = new SongsModel();
        ConsumerModel consumer = new ConsumerModel();
        AzureStorageConfig storage = new AzureStorageConfig();

        public IActionResult Index()
        {
            return View();
        }
        public void ConnectionString()
        {
            con.ConnectionString = Muszilla.Properties.Resources.ConnectionString;
            connect.ConnectionString = Muszilla.Properties.Resources.ConnectionString;
        }
        public IActionResult Verify(ConsumerModel acc, SongsModel song) //Checks if the user's email and password matches one inside the database
        {
            string fn = "";
            string ln = "";
            string url = "";
            string id = "";
            FetchPlaylistData();
            FetchSongData();
            ConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "select User_ID, Email, Pass_word, FirstName, LastName, Picture from Consumer where Email = '" + acc.Email + "' and Pass_word = '" + acc.Pass_word + "'";
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                fn = dr["FirstName"].ToString();
                ln = dr["LastName"].ToString();
                url = dr["Picture"].ToString();
                id = dr["User_ID"].ToString();
                con.Close();
                HttpContext.Session.SetString("User_ID", id);
                HttpContext.Session.SetString("Email", acc.Email);
                HttpContext.Session.SetString("Pass_word", acc.Pass_word);
                HttpContext.Session.SetString("FirstName", fn);
                HttpContext.Session.SetString("LastName", ln);
                HttpContext.Session.SetString("Picture", url);


                return RedirectToAction("Homepage", Tuple.Create(consumer, storage, songsModel));
            }
            else
            {
                con.Close();
                ViewBag.Message = "Email or Password Incorrect";
                return View("Index");
            }
        }

        public IActionResult Homepage() // Gets all the strings to show the user's information in their session
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.Pass_word = HttpContext.Session.GetString("Pass_word");
                ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
                ViewBag.LastName = HttpContext.Session.GetString("LastName");
                ViewBag.Picture = HttpContext.Session.GetString("Picture");
                ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                ViewBag.CurrentDIV = HttpContext.Session.GetString("CurrentDIV");
                FetchPlaylistData();
                FetchSongData();
                return View("User_Homepage", Tuple.Create(consumer, storage, songsModel, playlistModel));
            }
            return View(Tuple.Create(consumer, storage, songsModel));
        }

        [HttpPost]
        public IActionResult GetID(string dataID, string DIV)
        {
            FetchPlaylistData();
            FetchSongData();
            if (!string.IsNullOrEmpty(dataID) && !string.IsNullOrEmpty(DIV))
            {
                HttpContext.Session.SetString("CurrentPlaylistID", dataID);
                ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                HttpContext.Session.SetString("CurrentDIV", DIV);
                ViewBag.CurrentDIV = HttpContext.Session.GetString("CurrentDIV");
            }
            playlistModel.Clicked_Playlist = true;
            return View("User_Homepage", Tuple.Create(consumer, storage, songsModel, playlistModel));
        }

        [HttpPost]
        public IActionResult Create(Register add) // Adding a new user inside the database
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
        public IActionResult Logout() //Returns to the login page and clears the session
        {
            string empty = "";
            HttpContext.Session.SetString("User_ID", empty);
            HttpContext.Session.SetString("Email", empty);
            HttpContext.Session.SetString("Pass_word", empty);
            HttpContext.Session.SetString("FirstName", empty);
            HttpContext.Session.SetString("LastName", empty);
            HttpContext.Session.SetString("Picture", empty);
            HttpContext.Session.SetString("CurrentPlaylistID", empty);
            HttpContext.Session.SetString("CurrentDIV", empty);
            ViewBag.Message = "Log out successful!";
            return View("Index");
        }

        public IActionResult Update(ConsumerModel edit) //Updates fields inside the database
        {
            string id = "";
            FetchPlaylistData();
            FetchSongData();
            id = HttpContext.Session.GetString("User_ID");
            ConnectionString();
            con.Open();
            com.Connection = con;
            if (edit.FirstName != null)
            {
                com.CommandText = "update Consumer set FirstName = '" + edit.FirstName + "'  where User_ID ='" + id + "'";
                com.ExecuteNonQuery();
                HttpContext.Session.SetString("FirstName", edit.FirstName);
            }
            if (edit.LastName != null)
            {
                com.CommandText = "update Consumer set LastName = '" + edit.LastName + "'  where User_ID ='" + id + "'";
                com.ExecuteNonQuery();
                HttpContext.Session.SetString("LastName", edit.LastName);
            }
            if (edit.Email != null)
            {
                com.CommandText = "update Consumer set Email = '" + edit.Email + "'  where User_ID ='" + id + "'";
                com.ExecuteNonQuery();
                HttpContext.Session.SetString("Email", edit.Email);
            }
            if (edit.Pass_word != null)
            {
                com.CommandText = "update Consumer set Pass_word = '" + edit.Pass_word + "'  where User_ID ='" + id + "'";
                com.ExecuteNonQuery();
                HttpContext.Session.SetString("Pass_word", edit.Pass_word);
            }
            else
            {
                con.Close();
                return RedirectToAction("Homepage", Tuple.Create(consumer, storage, songsModel, playlistModel));
            }
            ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
            ViewBag.CurrentDIV = HttpContext.Session.GetString("CurrentDIV");
            con.Close();
            return RedirectToAction("Homepage", Tuple.Create(consumer, storage, songsModel, playlistModel));
        }

        public IActionResult CreatePlaylist(PlaylistModel createPlaylist)
        {
            if (createPlaylist.Playlist_Name != null)
            {
                String id;
                id = HttpContext.Session.GetString("User_ID");
                string connection = Muszilla.Properties.Resources.ConnectionString;

                using (SqlConnection con = new SqlConnection(connection))
                {
                    string query = "insert into Playlist(Playlist_Name, User_ID_FK)  values('" + createPlaylist.Playlist_Name + "', '" + id + "')";
                    using (SqlCommand com = new SqlCommand(query, con))
                    {
                        con.Open();
                        com.ExecuteNonQuery();
                        ViewBag.Message = "New Playlist inserted succesfully!";
                    }
                    con.Close();
                    return RedirectToAction("Homepage", Tuple.Create(consumer, storage, songsModel, playlistModel));
                }
            }
            ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
            ViewBag.CurrentDIV = HttpContext.Session.GetString("CurrentDIV");
            return RedirectToAction("Homepage", Tuple.Create(consumer, storage, songsModel, playlistModel));
        }


        private void FetchPlaylistData()
        {
            ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
            ViewBag.CurrentDIV = HttpContext.Session.GetString("CurrentDIV");
            List<PlaylistModel> PlaylistsFromDB = new List<PlaylistModel>();
            ConnectionString();
            String id;
            id = HttpContext.Session.GetString("User_ID");

            if (PlaylistsFromDB.Count > 0)
            {
                PlaylistsFromDB.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from Playlist where User_ID_FK ='" + id + "'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    PlaylistsFromDB.Add(new PlaylistModel()
                    {
                        Playlist_ID = dr["Playlist_ID"].ToString(),
                        Playlist_Name = dr["Playlist_Name"].ToString(),
                        User_ID_FK = dr["User_ID_FK"].ToString()
                    });
                }
                con.Close();
                playlistModel.Playlists = PlaylistsFromDB;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FetchSongData()
        {
            ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
            ViewBag.CurrentDIV = HttpContext.Session.GetString("CurrentDIV");
            List<SongsModel> songListFromDB = new List<SongsModel>();
            ConnectionString();
            String id;
            String currentPlaylist;
            id = HttpContext.Session.GetString("User_ID");
            currentPlaylist = HttpContext.Session.GetString("CurrentPlaylistID");


            if (songListFromDB.Count > 0)
            {
                songListFromDB.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from Songs where Song_Owner ='" + id + "' and Song_Playlist_ID ='" + currentPlaylist + "'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    songListFromDB.Add(new SongsModel()
                    {
                        Song_ID = dr["Song_ID"].ToString(),
                        Song_Name = dr["Song_Name"].ToString(),
                        Song_Audio = dr["Song_Audio"].ToString()
                    });
                }
                con.Close();
                songsModel.songsList = songListFromDB;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
