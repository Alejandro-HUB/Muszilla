/****** Object:  Table [dbo].[Consumer]    Script Date: 4/22/2021 6:44:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Consumer](
	[User_ID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Pass_word] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[Picture] [varchar](max) NULL,
	[CurrentPlaylistID] [int] NULL,
	[isGoogleUser] [int] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[User_ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Playlist]    Script Date: 4/22/2021 6:44:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Playlist](
	[Playlist_ID] [int] IDENTITY(1,1) NOT NULL,
	[Playlist_Name] [varchar](50) NOT NULL,
	[User_ID_FK] [int] NULL,
 CONSTRAINT [PK_Playlist] PRIMARY KEY CLUSTERED 
(
	[Playlist_ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Songs_Users]    Script Date: 4/22/2021 6:44:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Songs_Users](
	[Song_ID] [int] IDENTITY(1,1) NOT NULL,
	[Song_Name] [varchar](max) NULL,
	[Song_Audio] [varchar](max) NULL,
	[Song_Owner] [int] NULL,
	[Song_Playlist_ID] [int] NULL,
 CONSTRAINT [PK_Songs] PRIMARY KEY CLUSTERED 
(
	[Song_ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Songs]    Script Date: 4/5/2022 11:04:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Songs](
	[SongID] [int] IDENTITY(1,1) NOT NULL,
	[SongName] [varchar](max) NULL,
	[SongAudio] [varchar](max) NULL,
	[TimesPlayed] [int] NULL,
 CONSTRAINT [PKSongs] PRIMARY KEY CLUSTERED 
(
	[SongID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sysdiagrams]    Script Date: 4/22/2021 6:44:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sysdiagrams](
	[name] [sysname] NOT NULL,
	[principal_id] [int] NOT NULL,
	[diagram_id] [int] IDENTITY(1,1) NOT NULL,
	[version] [int] NULL,
	[definition] [varbinary](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[diagram_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_principal_name] UNIQUE NONCLUSTERED 
(
	[principal_id] ASC,
	[name] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Consumer] ADD  CONSTRAINT [DF_Users_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Playlist]  WITH CHECK ADD  CONSTRAINT [User_FK] FOREIGN KEY([User_ID_FK])
REFERENCES [dbo].[Consumer] ([User_ID])
GO
ALTER TABLE [dbo].[Playlist] CHECK CONSTRAINT [User_FK]
GO
ALTER TABLE [dbo].[Songs_Users]  WITH CHECK ADD  CONSTRAINT [FK_Songs_Owner] FOREIGN KEY([Song_Owner])
REFERENCES [dbo].[Consumer] ([User_ID])
GO
ALTER TABLE [dbo].[Songs_Users] CHECK CONSTRAINT [FK_Songs_Owner]
GO
ALTER TABLE [dbo].[SongsPlaylistR]  WITH CHECK ADD  CONSTRAINT [FK__SongsPlay__playl__72C60C4A] FOREIGN KEY([playlistID])
REFERENCES [dbo].[Playlist] ([Playlist_ID])
GO
ALTER TABLE [dbo].[SongsPlaylistR] CHECK CONSTRAINT [FK__SongsPlay__playl__72C60C4A]
GO
ALTER TABLE [dbo].[SongsPlaylistR]  WITH CHECK ADD  CONSTRAINT [FK__SongsPlay__songs__71D1E811] FOREIGN KEY([songsID])
REFERENCES [dbo].[Songs_Users] ([Song_ID])
GO
ALTER TABLE [dbo].[SongsPlaylistR] CHECK CONSTRAINT [FK__SongsPlay__songs__71D1E811]
GO
EXEC sys.sp_addextendedproperty @name=N'microsoft_database_tools_support', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysdiagrams'
GO
