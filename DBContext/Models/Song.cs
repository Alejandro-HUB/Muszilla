using System;
using System.Collections.Generic;

namespace DBContext.Models;

public partial class Song
{
    public int SongId { get; set; }

    public string? SongName { get; set; }

    public string? SongAudio { get; set; }

    public int? TimesPlayed { get; set; }
}
