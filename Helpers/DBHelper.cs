using Alody.Helpers;
using Alody.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Linq;


namespace Alody.Helpers
{
    public class DBHelper
    {
        public string url = "";
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        public bool ContainsGoogleUser(string IncomingUser)
        {
            List<UserModel> UsersFromDB = new List<UserModel>();
            string connection = Alody.Properties.Resources.ConnectionString;
            using (SqlConnection con = new SqlConnection(connection))
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from dbo.Consumer where isGoogleUser = '1'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    UsersFromDB.Add(new UserModel()
                    {
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        ImageURL = dr["Picture"].ToString(),
                        Email = dr["Email"].ToString()
                    });
                }
                con.Close();

                var ContainsUsers = UsersFromDB.Where(x => x.Email == IncomingUser);

                if (ContainsUsers == null || ContainsUsers.Count() == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }


        public bool ContainsSongTable(string IncomingSong)
        {
            List<SongsModel> songListFromDB = new List<SongsModel>();
            string connection = Alody.Properties.Resources.ConnectionString;
            using (SqlConnection con = new SqlConnection(connection))
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from dbo.Songs order by SongName";
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

                var ContainsSongs = songListFromDB.Where(x => x.Song_Name == IncomingSong);

                if (ContainsSongs == null || ContainsSongs.Count() == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }


        public bool ContainsSongUsersTable(string IncomingSong, string IncomingPlaylistID, string userID)
        {
            List<SongsModel> songListFromDB = new List<SongsModel>();
            string connection = Alody.Properties.Resources.ConnectionString;
            using (SqlConnection con = new SqlConnection(connection))
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from dbo.Songs_Users where Song_Playlist_ID = '"+ IncomingPlaylistID + "' and Song_Owner = '" + userID + "' order by Song_Name";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    songListFromDB.Add(new SongsModel()
                    {
                        Song_ID = dr["Song_ID"].ToString(),
                        Song_Name = dr["Song_Name"].ToString(),
                        Song_Audio = dr["Song_Audio"].ToString(),
                        Song_Owner = dr["Song_Owner"].ToString()
                    });
                }
                con.Close();

                var ContainsSongs = songListFromDB.Where(x => x.Song_Name == IncomingSong);

                if (ContainsSongs == null || ContainsSongs.Count() == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
