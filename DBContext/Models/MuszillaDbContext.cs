using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DBContext.Models;

public partial class MuszillaDbContext : DbContext
{
    public MuszillaDbContext()
    {
    }

    public MuszillaDbContext(DbContextOptions<MuszillaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<Song> Songs { get; set; }

    public virtual DbSet<SongsUser> SongsUsers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(DBContext.Properties.Resources.ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.ToTable("Playlist");

            entity.Property(e => e.PlaylistId).HasColumnName("Playlist_ID");
            entity.Property(e => e.PlaylistName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Playlist_Name");
            entity.Property(e => e.UserIdFk).HasColumnName("User_ID_FK");

            entity.HasOne(d => d.UserIdFkNavigation).WithMany(p => p.Playlists)
                .HasForeignKey(d => d.UserIdFk)
                .HasConstraintName("User_FK");
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(e => e.SongId).HasName("PKSongs");

            entity.Property(e => e.SongId).HasColumnName("SongID");
            entity.Property(e => e.SongAudio).IsUnicode(false);
            entity.Property(e => e.SongName).IsUnicode(false);
        });

        modelBuilder.Entity<SongsUser>(entity =>
        {
            entity.HasKey(e => e.SongId).HasName("PK_Songs");

            entity.Property(e => e.SongId).HasColumnName("Song_ID");
            entity.Property(e => e.SongAudio)
                .IsUnicode(false)
                .HasColumnName("Song_Audio");
            entity.Property(e => e.SongName)
                .IsUnicode(false)
                .HasColumnName("Song_Name");
            entity.Property(e => e.SongOwner).HasColumnName("Song_Owner");
            entity.Property(e => e.SongPlaylistId).HasColumnName("Song_Playlist_ID");

            entity.HasOne(d => d.SongOwnerNavigation).WithMany(p => p.SongsUsers)
                .HasForeignKey(d => d.SongOwner)
                .HasConstraintName("FK_Songs_Owner");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_Users");

            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("User_ID");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrentPlaylistId).HasColumnName("CurrentPlaylistID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsGoogleUser).HasColumnName("isGoogleUser");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.Picture).IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
