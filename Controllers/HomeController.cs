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
using com.sun.tools.@internal.ws.processor.model;
using System.Text.Json;
using System.Data;
using System.ComponentModel.DataAnnotations;

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

        //DB Helper
        DBHelper dBHelper = new DBHelper();

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
            string isGoogleUser = "";
            FetchPlaylistData();
            FetchSongData();
            FeaturedSongs();
            TopPickedSongs();
            GetListofPlaylistIDs();
            ConnectionString();
            acc.Pass_word = HashHelper.GetHashString(acc.Pass_word);

            con.Open();
            com.Connection = con;
            com.CommandText = "select isGoogleUser from dbo.Consumer where Email = '" + acc.Email + "'";
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                isGoogleUser = dr["isGoogleUser"].ToString();
            }
            con.Close();

            if (isGoogleUser.Contains("1"))
            {
                ViewBag.Message = "Please login using Google";
                return View("Index");
            }
            else
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "select User_ID, Email, Pass_word, FirstName, LastName, Picture, isGoogleUser from Consumer where Email = '" + acc.Email + "' and Pass_word = '" + acc.Pass_word + "'";
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
                    HttpContext.Session.SetString("isGoogleUser", isGoogleUser);

                    return RedirectToAction("Homepage", Tuple.Create(consumer, storage, songsModel));

                }
                else
                {
                    con.Close();
                    ViewBag.Message = "Email or Password Incorrect";
                    return View("Index");
                }
            } 
        }

        public void FeaturedSongs()
        {
            List<SongsModel> songListFromDB = new List<SongsModel>();
            ConnectionString();

            try
            {

                //Fetch songs
                if (songListFromDB.Count > 0)
                {
                    songListFromDB.Clear();
                }
                con.Open();
                com.Connection = con;
                com.CommandText = "select TOP(6) * from Songs order by SongID desc";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    songListFromDB.Add(new SongsModel()
                    {
                        Song_ID = dr["SongID"].ToString(),
                        Song_Name = dr["SongName"].ToString(),
                        Song_Audio = dr["SongAudio"].ToString()
                    });
                }
                con.Close();
                songsModel.Featured = songListFromDB;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void TopPickedSongs()
        {
            List<SongsModel> songListFromDB = new List<SongsModel>();
            ConnectionString();

            try
            {

                //Fetch songs
                if (songListFromDB.Count > 0)
                {
                    songListFromDB.Clear();
                }
                con.Open();
                com.Connection = con;
                com.CommandText = "select TOP(6) * from Songs order by TimesPlayed desc";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    songListFromDB.Add(new SongsModel()
                    {
                        Song_ID = dr["SongID"].ToString(),
                        Song_Name = dr["SongName"].ToString(),
                        Song_Audio = dr["SongAudio"].ToString()
                    });
                }
                con.Close();
                songsModel.TopPicked = songListFromDB;
            }
            catch (Exception ex)
            {
                throw ex;
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
                ViewBag.isGoogleUser = HttpContext.Session.GetString("isGoogleUser");
                GetListofPlaylistIDs();
                FetchPlaylistData();
                FetchSongData();
                FeaturedSongs();
                TopPickedSongs();
                return View("User_Homepage", Tuple.Create(consumer, storage, songsModel, playlistModel));
            }
            return View(Tuple.Create(consumer, storage, songsModel));
        }

        [HttpPost]
        public void UpdateSongPlayed(string songID)
        {
            int CountValue = 0;
            string fromDB = string.Empty;

            if (!string.IsNullOrEmpty(songID))
            {
                ConnectionString();

                con.Open();
                com.Connection = con;
                com.CommandText = "select TimesPlayed from Songs where SongID = '" + songID + "'";
                dr = com.ExecuteReader();
                if (dr.Read())
                {
                    fromDB = dr["TimesPlayed"].ToString();
                }
                con.Close();

                CountValue = Int32.Parse(fromDB);
                CountValue += 1;

                con.Open();
                com.Connection = con;
                com.CommandText = "update Songs set TimesPlayed = '" + CountValue + "'  where SongID ='" + songID + "'";
                com.ExecuteNonQuery();
                con.Close();
            }
        }


        [HttpPost]
        public string GetID(string dataID)
        {
            string jsonString = "";
            FetchPlaylistData();
            GetListofPlaylistIDs();
            String id;
            id = HttpContext.Session.GetString("User_ID");
            if (!string.IsNullOrEmpty(dataID))
            {
                ConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "update Consumer set CurrentPlaylistID = '" + dataID + "'  where User_ID ='" + id + "'";
                com.ExecuteNonQuery();
                con.Close();
                HttpContext.Session.SetString("CurrentPlaylistID", dataID);
                FetchSongData();
                FeaturedSongs();
                TopPickedSongs();
                jsonString = JsonSerializer.Serialize(songsModel.songsList);
                return jsonString;
            }
            FetchSongData();
            FeaturedSongs();
            TopPickedSongs();
            jsonString = JsonSerializer.Serialize(songsModel.songsList);
            return jsonString;
        }

        [HttpPost]
        public string addSongToPlaylist(string addSongAudio, string songName, string PlaylistID, string playListName)
        {
            string id = HttpContext.Session.GetString("User_ID");
            ConnectionString();

            if (!dBHelper.ContainsSongUsersTable(songName, PlaylistID, id))
            {
                //Repopulate data so it is not null
                if (HttpContext.Session.GetString("Email") != null)
                {
                    ViewBag.Email = HttpContext.Session.GetString("Email");
                    ViewBag.Pass_word = HttpContext.Session.GetString("Pass_word");
                    ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
                    ViewBag.LastName = HttpContext.Session.GetString("LastName");
                    ViewBag.Picture = HttpContext.Session.GetString("Picture");
                    ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                    ViewBag.isGoogleUser = HttpContext.Session.GetString("isGoogleUser");
                    FetchPlaylistData();
                    FetchSongData();
                    FeaturedSongs();
                    TopPickedSongs();
                }
                try
                {
                    con.Open();
                    com.Connection = con;

                    if (!string.IsNullOrEmpty(songName))
                    {
                        com.CommandText = "insert into Songs_Users(Song_Name, Song_Audio, Song_Owner, Song_Playlist_ID) values('" + songName + "', '" + addSongAudio + "', '" + id + "', '" + PlaylistID + "')";

                        com.ExecuteNonQuery();
                        con.Close();
                    }
                    else
                    {
                        con.Close();
                    }
                    ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                    return "SUCCESS: " + "ADDED: " + songName + " TO " + playListName;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return "FAIL: " + songName + " ALREADY EXISTS IN " + playListName;
            }
        }

        [HttpPost]
        public string deletePlaylist(string deletePlaylist, string PlaylistName)
        {
            ConnectionString();

            //Repopulate data so it is not null
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.Pass_word = HttpContext.Session.GetString("Pass_word");
                ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
                ViewBag.LastName = HttpContext.Session.GetString("LastName");
                ViewBag.Picture = HttpContext.Session.GetString("Picture");
                ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                ViewBag.isGoogleUser = HttpContext.Session.GetString("isGoogleUser");
                FetchPlaylistData();
                FetchSongData();
                FeaturedSongs();
                TopPickedSongs();
            }
            try
            {
                con.Open();
                com.Connection = con;

                if (!string.IsNullOrEmpty(deletePlaylist))
                {
                    com.CommandText = "delete from dbo.Songs_Users where Song_Playlist_ID = '" + deletePlaylist + "'";

                    com.ExecuteNonQuery();
                    con.Close();

                    con.Open();
                    com.CommandText = "delete from dbo.Playlist where Playlist_ID = '" + deletePlaylist + "'";
                    com.ExecuteNonQuery();
                    con.Close();
                }
                else
                {
                    con.Close();
                }
                ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                ViewBag.Message = "Deleted playlist: " + PlaylistName;
                return deletePlaylist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public string deleteSong(string deleteSong, string songName)
        {
            ConnectionString();

            //Repopulate data so it is not null
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.Pass_word = HttpContext.Session.GetString("Pass_word");
                ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
                ViewBag.LastName = HttpContext.Session.GetString("LastName");
                ViewBag.Picture = HttpContext.Session.GetString("Picture");
                ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                ViewBag.isGoogleUser = HttpContext.Session.GetString("isGoogleUser");
                FetchPlaylistData();
                FetchSongData();
                FeaturedSongs();
                TopPickedSongs();
            }
            try
            {
                con.Open();
                com.Connection = con;

                if (!string.IsNullOrEmpty(deleteSong))
                {
                    com.CommandText = "delete from dbo.Songs_Users where Song_ID = '" + deleteSong + "'";

                    com.ExecuteNonQuery();
                    con.Close();
                }
                else
                {
                    con.Close();
                }
                ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                ViewBag.Message = "Deleted song: " + songName;
                return deleteSong;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetListofPlaylistIDs()
        {
            ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
            List<ListofIDsModel> PlaylistsFromDB = new List<ListofIDsModel>();
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
                com.CommandText = "select c.CurrentPlaylistID, p.Playlist_ID  from consumer c, Playlist p where User_ID ='" + id + "'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    PlaylistsFromDB.Add(new ListofIDsModel()
                    {
                        IdsToBeHidden = dr["Playlist_ID"].ToString(),
                        Current_Playlist_ID = dr["CurrentPlaylistID"].ToString(),
                    });
                }
                con.Close();
                playlistModel.ListofIDs = PlaylistsFromDB;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult Create(Register add) // Adding a new user inside the database
        {
            string connection = Muszilla.Properties.Resources.ConnectionString;
            bool validEmail = false;
            if (new EmailAddressAttribute().IsValid(add.Email))
            {
                validEmail = true;
            }

            if (validEmail)
            {
                //Hash password
                add.Pass_word = HashHelper.GetHashString(add.Pass_word);

                using (SqlConnection con = new SqlConnection(connection))
                {
                    string query = "insert into Consumer(FirstName, LastName, Email, Pass_word, isGoogleUser) values('" + add.FirstName + "', '" + add.LastName + "', '" + add.Email + "', '" + add.Pass_word + "', '0')";
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
            ViewBag.Message = "Please enter a valid email. Example: email@domain.com";
            return View("Index");
        }

        [HttpPost]
        public IActionResult GoogleLogin(string FirstName, string LastName, string ImageURL, string email) // Adding a new user inside the database
        {
            string connection = Muszilla.Properties.Resources.ConnectionString;
            string ID = string.Empty;
            string picture = string.Empty;

            if (!dBHelper.ContainsGoogleUser(email))
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    string query = "insert into Consumer(FirstName, LastName, Email, Picture, isGoogleUser) values('" + FirstName + "', '" + LastName + "', '" + email + "', '" + ImageURL + "', '1')";
                    using (SqlCommand com = new SqlCommand(query, con))
                    {
                        con.Open();
                        com.ExecuteNonQuery();
                        ViewBag.Message = "New User inserted succesfully!";
                    }
                    con.Close();

                    GetListofPlaylistIDs();


                    con.Open();
                    com.Connection = con;
                    com.CommandText = "select User_ID, Picture from dbo.Consumer where Email = '" + email + "'";
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        ID = dr["User_ID"].ToString();
                        picture = dr["Picture"].ToString();
                    }
                    con.Close();

                    HttpContext.Session.SetString("User_ID", ID);
                    HttpContext.Session.SetString("Email", email);
                    HttpContext.Session.SetString("FirstName", FirstName);
                    HttpContext.Session.SetString("LastName", LastName);
                    HttpContext.Session.SetString("Picture", ImageURL);
                    HttpContext.Session.SetString("isGoogleUser", "1");


                    return RedirectToAction("Homepage", Tuple.Create(consumer, storage, songsModel));
                }
            }
            else
            {
                GetListofPlaylistIDs();
                using (SqlConnection con = new SqlConnection(connection))
                {
                    con.Open();
                    com.Connection = con;

                    if (FirstName != null)
                    {
                        com.CommandText = "update Consumer set FirstName = '" + FirstName + "'  where User_ID ='" + ID + "'";
                        com.ExecuteNonQuery();
                    }
                    if (LastName != null)
                    {
                        com.CommandText = "update Consumer set LastName = '" + LastName + "'  where User_ID ='" + ID + "'";
                        com.ExecuteNonQuery();
                    }
                    if (ImageURL != null)
                    {
                        com.CommandText = "update Consumer set Picture = '" + ImageURL + "'  where User_ID ='" + ID + "'";
                        com.ExecuteNonQuery();
                    }
                    if (email != null)
                    {
                        com.CommandText = "update Consumer set Email = '" + email + "'  where User_ID ='" + ID + "'";
                        com.ExecuteNonQuery();
                    }

                    con.Close();

                    con.Open();
                    com.Connection = con;
                    com.CommandText = "select User_ID, Picture from dbo.Consumer where Email = '" + email + "'";
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        ID = dr["User_ID"].ToString();
                        picture = dr["Picture"].ToString();
                    }
                    con.Close();
                }

                if (string.IsNullOrEmpty(ImageURL))
                {
                    HttpContext.Session.SetString("Picture", picture);
                }
                else if (picture != ImageURL && !string.IsNullOrEmpty(ImageURL))
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "UPDATE Consumer SET Picture = '" + ImageURL + "' where Email = '" + email + "'";
                    dr = com.ExecuteReader();
                    com.ExecuteNonQuery();
                    con.Close();
                    HttpContext.Session.SetString("Picture", ImageURL);
                }
                else
                {
                    HttpContext.Session.SetString("Picture", ImageURL);
                }

                HttpContext.Session.SetString("User_ID", ID);
                HttpContext.Session.SetString("Email", email);
                HttpContext.Session.SetString("FirstName", FirstName);
                HttpContext.Session.SetString("LastName", LastName);
                HttpContext.Session.SetString("isGoogleUser", "1");


                return RedirectToAction("Homepage", Tuple.Create(consumer, storage, songsModel));
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
            HttpContext.Session.SetString("CurrentSearchQuery", empty);
            HttpContext.Session.SetString("isGoogleUser", empty);
            ViewBag.Message = "Log out successful!";
            return View("Index");
        }

        public IActionResult Update(ConsumerModel edit) //Updates fields inside the database
        {
            string id = "";
            FetchPlaylistData();
            FetchSongData();
            FeaturedSongs();
            TopPickedSongs();
            GetListofPlaylistIDs();
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
                //Hash password
                edit.Pass_word = HashHelper.GetHashString(edit.Pass_word);

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
            return RedirectToAction("Homepage", Tuple.Create(consumer, storage, songsModel, playlistModel));
        }


        private void FetchPlaylistData()
        {
            ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
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


        public IActionResult sortSongsByName()
        {
            List<SongsModel> songListFromDB = new List<SongsModel>();
            var currentQuery = HttpContext.Session.GetString("CurrentSearchQuery");

            //Repopulate data so it is not null
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.Pass_word = HttpContext.Session.GetString("Pass_word");
                ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
                ViewBag.LastName = HttpContext.Session.GetString("LastName");
                ViewBag.Picture = HttpContext.Session.GetString("Picture");
                ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                ViewBag.isGoogleUser = HttpContext.Session.GetString("isGoogleUser");
                FetchPlaylistData();
                FetchSongData();
                FeaturedSongs();
                TopPickedSongs();
            }

            if (currentQuery != null)
            {
                ConnectionString();

                if (songListFromDB.Count > 0)
                {
                    songListFromDB.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = currentQuery + " order by SongName";

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        songListFromDB.Add(new SongsModel()
                        {
                            Song_ID = dr["SongID"].ToString(),
                            Song_Name = dr["SongName"].ToString(),
                            Song_Audio = dr["SongAudio"].ToString()
                        });
                    }
                    con.Close();

                    songsModel.searchedSongsList = songListFromDB;
                    ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                    return PartialView("User_Homepage", Tuple.Create(consumer, storage, songsModel, playlistModel));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return PartialView("User_Homepage", Tuple.Create(consumer, storage, songsModel, playlistModel));
        }

        public IActionResult sortSongsByDate()
        {
            List<SongsModel> songListFromDB = new List<SongsModel>();
            var currentQuery = HttpContext.Session.GetString("CurrentSearchQuery");

            //Repopulate data so it is not null
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.Pass_word = HttpContext.Session.GetString("Pass_word");
                ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
                ViewBag.LastName = HttpContext.Session.GetString("LastName");
                ViewBag.Picture = HttpContext.Session.GetString("Picture");
                ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                ViewBag.isGoogleUser = HttpContext.Session.GetString("isGoogleUser");
                FetchPlaylistData();
                FetchSongData();
                FeaturedSongs();
                TopPickedSongs();
            }

            if (currentQuery != null)
            {
                ConnectionString();

                if (songListFromDB.Count > 0)
                {
                    songListFromDB.Clear();
                }
                try
                {
                    con.Open();
                    com.Connection = con;
                    com.CommandText = currentQuery;

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        songListFromDB.Add(new SongsModel()
                        {
                            Song_ID = dr["SongID"].ToString(),
                            Song_Name = dr["SongName"].ToString(),
                            Song_Audio = dr["SongAudio"].ToString()
                        });
                    }
                    con.Close();

                    songsModel.searchedSongsList = songListFromDB;
                    ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                    return PartialView("User_Homepage", Tuple.Create(consumer, storage, songsModel, playlistModel));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return PartialView("User_Homepage", Tuple.Create(consumer, storage, songsModel, playlistModel));
        }

        public IActionResult searchForAllSongs()
        {
            List<SongsModel> songListFromDB = new List<SongsModel>();
            ConnectionString();

            //Repopulate data so it is not null
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.Pass_word = HttpContext.Session.GetString("Pass_word");
                ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
                ViewBag.LastName = HttpContext.Session.GetString("LastName");
                ViewBag.Picture = HttpContext.Session.GetString("Picture");
                ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                ViewBag.isGoogleUser = HttpContext.Session.GetString("isGoogleUser");
                FetchPlaylistData();
                FetchSongData();
                FeaturedSongs();
                TopPickedSongs();
            }


            if (songListFromDB.Count > 0)
            {
                songListFromDB.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "select TOP(100) * from dbo.Songs order by SongName";
                HttpContext.Session.SetString("CurrentSearchQuery", "select TOP(100) * from dbo.Songs");

                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    songListFromDB.Add(new SongsModel()
                    {
                        Song_ID = dr["SongID"].ToString(),
                        Song_Name = dr["SongName"].ToString(),
                        Song_Audio = dr["SongAudio"].ToString()
                    });
                }
                con.Close();

                songsModel.searchedSongsList = songListFromDB;
                ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                return PartialView("User_Homepage", Tuple.Create(consumer, storage, songsModel, playlistModel));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult searchForSongs(SongsModel songModelSearch)
        {
            List<SongsModel> songListFromDB = new List<SongsModel>();
            ConnectionString();

            //Repopulate data so it is not null
            if (HttpContext.Session.GetString("Email") != null)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.Pass_word = HttpContext.Session.GetString("Pass_word");
                ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
                ViewBag.LastName = HttpContext.Session.GetString("LastName");
                ViewBag.Picture = HttpContext.Session.GetString("Picture");
                ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                ViewBag.isGoogleUser = HttpContext.Session.GetString("isGoogleUser");
                FetchPlaylistData();
                FetchSongData();
                FeaturedSongs();
                TopPickedSongs();
            }


            if (songListFromDB.Count > 0)
            {
                songListFromDB.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;

                if (!string.IsNullOrEmpty(songModelSearch.Song_Name))
                {
                    com.CommandText = "select TOP(100) * from dbo.Songs where SongName like '%" + songModelSearch.Song_Name + "%' order by SongName";
                    HttpContext.Session.SetString("CurrentSearchQuery", "select TOP(100) * from dbo.Songs where SongName like '%" + songModelSearch.Song_Name + "%'");

                    dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        songListFromDB.Add(new SongsModel()
                        {
                            Song_ID = dr["SongID"].ToString(),
                            Song_Name = dr["SongName"].ToString(),
                            Song_Audio = dr["SongAudio"].ToString()
                        });
                    }
                    con.Close();
                }
                else
                {
                    con.Close();
                }

                songsModel.searchedSongsList = songListFromDB;
                ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
                return PartialView("User_Homepage", Tuple.Create(consumer, storage, songsModel, playlistModel));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FetchSongData()
        {
            ViewBag.CurrentPlaylistID = HttpContext.Session.GetString("CurrentPlaylistID");
            FetchPlaylistData();
            GetListofPlaylistIDs();

            List<SongsModel> songListFromDB = new List<SongsModel>();
            ConnectionString();
            String id;
            String currentPlaylist = "";
            id = HttpContext.Session.GetString("User_ID");

            try
            {
                //Get current playlistID from DB - Uncomment if Needed
                con.Open();
                com.Connection = con;
                com.CommandText = "select CurrentPlaylistID from Consumer where User_ID ='" + id + "'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    currentPlaylist = dr["CurrentPlaylistID"].ToString();
                }
                con.Close();

                //Fetch songs
                if (songListFromDB.Count > 0)
                {
                    songListFromDB.Clear();
                }
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from Songs_Users where Song_Owner ='" + id + "' and Song_Playlist_ID ='" + currentPlaylist + "'";
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
