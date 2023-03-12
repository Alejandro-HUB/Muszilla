using System;
using System.Collections.Generic;

namespace DBContext.Models;

public partial class Playlist
{
    public int PlaylistId { get; set; }

    public string PlaylistName { get; set; } = null!;

    public int? UserIdFk { get; set; }

    public virtual User? UserIdFkNavigation { get; set; }
}
