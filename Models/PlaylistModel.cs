using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Alody.Models
{
    public class PlaylistModel
    {
        public string Playlist_ID { get; set; }
        [DisplayName("Name")]
        public string Playlist_Name { get; set; }
        public string User_ID_FK { get; set; }
        public List<PlaylistModel> Playlists { get; set; }
        public List<ListofIDsModel> ListofIDs { get; set; }
    }
}
