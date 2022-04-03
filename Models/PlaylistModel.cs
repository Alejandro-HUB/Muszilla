using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Muszilla.Models
{
    public class PlaylistModel
    {
        public string Playlist_ID { get; set; }
        [DisplayName("Name")]
        public string Playlist_Name { get; set; }
        public string User_ID_FK { get; set; }
        public List<PlaylistModel> Playlists { get; set; }
        public string currentDIV { get; set; }
        public string currentID { get; set; }
        public bool Clicked_Playlist { get; set; }
    }
}
