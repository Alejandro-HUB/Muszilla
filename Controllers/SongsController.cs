using Muszilla.Helpers;
using Muszilla.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Linq;

namespace Muszilla.Controllers
{
    [Route("api/songs")]
    public class SongsController : Controller //This is the controller than handles the file upload 
    {
        public string url = "";
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        bool failedSongTableInsert = false;
        // make sure that appsettings.json is filled with the necessary details of the azure storage
        private readonly AzureStorageConfig storageConfig = null;
        //DB Helper
        DBHelper dBHelper = new DBHelper();

        public SongsController(IOptions<AzureStorageConfig> config)
        {
            storageConfig = config.Value;
        }

        // POST /api/songs/upload
        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(ICollection<IFormFile> files)
        {
            bool isUploaded = false;
            bool isAudio = false;
            string Song_Name = "";
            string email = "";
            string id = "";
            string currentPlaylist = "";


            try
            {
                if (files.Count == 0)
                    return BadRequest("No files received from the upload");

                if (storageConfig.AccountKey == string.Empty || storageConfig.AccountName == string.Empty)
                    return BadRequest("sorry, can't retrieve your azure storage details from appsettings.js, make sure that you add azure storage details there");

                if (storageConfig.Container == string.Empty)
                    return BadRequest("Please provide a name for your image container in the azure blob storage");

                foreach (var formFile in files)
                {
                    if (ViewBag.Email != "")
                    {
                        if (StorageHelper.IsImage(formFile)) //Checks if the file is an image 
                        {
                            ViewBag.Message = "You can only upload a song!";
                            return new UnsupportedMediaTypeResult();
                        }
                        else if (StorageHelper.IsAudio(formFile)) //Checks if the file is an audio file
                        {
                            //Set flag to true 
                            isAudio = true;

                            //Check if the song's name has invalid characters
                            bool InvalidFileName = CleanString.IsValidFilename(formFile.FileName);

                            //Check if the song's name contains unicode characters that have an AnsiCode > 127
                            bool unicodeCharacters = CleanString.ContainsUnicodeCharacter(formFile.FileName);

                            if (InvalidFileName || !unicodeCharacters)
                            {
                                Song_Name = CleanString.UseStringBuilderWithHashSet(formFile.FileName);
                                Song_Name = CleanString.EscapeForeignCharacters(Song_Name);
                              
                                //If clean up got rid of file name because it only included invalid characters, replace empty string
                                if (Song_Name == string.Empty || Song_Name == "" || string.IsNullOrEmpty(Song_Name))
                                {
                                    ViewBag.Message = "Invalid Image Name!";
                                    return new UnsupportedMediaTypeResult();
                                }
                                else if (Song_Name == ".mp3")
                                {
                                    Song_Name = "EmptyFileName.mp3";
                                }
                                else if (Song_Name == ".wma")
                                {
                                    Song_Name = "EmptyFileName.wma";
                                }
                                else if (Song_Name == ".wav")
                                {
                                    Song_Name = "EmptyFileName.wav";
                                }
                                else if (Song_Name == ".wmv")
                                {
                                    Song_Name = "EmptyFileName.wmv";
                                }

                            }
                            else
                            {
                                Song_Name = formFile.FileName;
                            }
                            
                            url = "https://devstorageale.blob.core.windows.net/muszilla/" + Song_Name;
                            if (formFile.Length > 0 && !dBHelper.ContainsSongTable(Song_Name))
                            {
                                using (Stream stream = formFile.OpenReadStream())
                                {
                                    isUploaded = await StorageHelper.UploadFileToStorage(stream, Song_Name, storageConfig);
                                    isAudio = true;
                                }
                            }
                        }
                        else
                        {
                            return new UnsupportedMediaTypeResult();
                        }
                    }
                }

                //                                                                          ** Start logic for Uploading Songs **
                if (isAudio) //Inserting a song into the database
                {
                    string connection = Muszilla.Properties.Resources.ConnectionString;
                    if (HttpContext.Session.GetString("Email") != null)
                    {
                        using (SqlConnection con = new SqlConnection(connection))
                        {
                            email = HttpContext.Session.GetString("Email");
                            id = HttpContext.Session.GetString("User_ID");
                            //currentPlaylist = HttpContext.Session.GetString("CurrentPlaylistID");

                            //Check if the song already exists in the MAIN Song table
                            if (!dBHelper.ContainsSongTable(Song_Name))
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
                                //Insert to Song Users Table
                                string query = "insert into Songs_Users(Song_Name, Song_Audio, Song_Owner, Song_Playlist_ID) values('" + Song_Name + "', '" + url + "', '" + id + "', '" + currentPlaylist + "')";
                                using (SqlCommand com = new SqlCommand(query, con))
                                {
                                    con.Open();
                                    com.ExecuteNonQuery();
                                    HttpContext.Session.SetString("Song_Name", Song_Name);
                                    HttpContext.Session.SetString("Song_Audio", url);
                                    con.Close();
                                }

                                //Insert to MAIN Song Table
                                string query2 = "insert into Songs(SongName, SongAudio) values('" + Song_Name + "', '" + url + "')";
                                using (SqlCommand com = new SqlCommand(query2, con))
                                {
                                    con.Open();
                                    com.ExecuteNonQuery();
                                    con.Close();
                                    failedSongTableInsert = false;
                                }
                            }
                            else
                            {
                                failedSongTableInsert = true;
                            }

                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error";
                    }
                    if (!failedSongTableInsert)
                    {
                        return new AcceptedResult();
                    }
                    else
                    {
                        ViewBag.Message = "Song already exists, please check our database.";
                        return BadRequest("Song already exists, please check our database.");
                    }
                }
                //                                                                          ** End logic for Uploading Songs **

                else
                    return BadRequest("Looks like the song could not be uploaded to the storage");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}