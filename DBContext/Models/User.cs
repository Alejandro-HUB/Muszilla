using System;
using System.Collections.Generic;

namespace DBContext.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Picture { get; set; }

    public int? CurrentPlaylistId { get; set; }

    public int? IsGoogleUser { get; set; }

    public string? AccessToken { get; set; }

    public virtual ICollection<Playlist> Playlists { get; } = new List<Playlist>();

    public virtual ICollection<SongsUser> SongsUsers { get; } = new List<SongsUser>();
}
