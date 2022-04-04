using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Muszilla.Models
{
    public class SongsModel
    {
        [DisplayName("ID")]
        public string Song_ID { get; set; }
        [DisplayName("Name")]
        public string Song_Name { get; set;}
        [DisplayName("Audio")]
        public string Song_Audio { get; set; }
        [DisplayName("Owner")]
        public string Song_Owner { get; set; }
        public List<SongsModel> songsList { get; set; }
        public List<SongsModel> searchedSongsList { get; set; }

    }
}
