using System;
using System.Collections.Generic;

namespace DBContext.Models;

public partial class SongsUser
{
    public int SongId { get; set; }

    public string? SongName { get; set; }

    public string? SongAudio { get; set; }

    public int? SongOwner { get; set; }

    public int? SongPlaylistId { get; set; }

    public virtual User? SongOwnerNavigation { get; set; }
}
