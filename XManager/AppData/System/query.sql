USE [ExpenseManagerDb]
GO
/****** Object:  Table [dbo].[administration]    Script Date: 26/11/2022 9:23:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[administration](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[guid] [nvarchar](50) NOT NULL,
	[username] [nvarchar](50) NULL,
	[password] [nvarchar](max) NULL,
	[access_level] [nvarchar](50) NULL,
	[date_joined] [datetime] NULL,
	[last_logged] [datetime] NULL,
 CONSTRAINT [PK_administration] PRIMARY KEY CLUSTERED 
(
	[guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[appdata]    Script Date: 26/11/2022 9:23:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[appdata](
	[guid] [nvarchar](max) NULL,
	[primary_id] [int] IDENTITY(1,1) NOT NULL,
	[foreign_id] [int] NOT NULL,
	[file_name] [nvarchar](50) NULL,
	[file_type] [nvarchar](50) NULL,
	[file_bin] [varbinary](max) NULL,
	[last_update] [datetime] NULL,
 CONSTRAINT [PK_files] PRIMARY KEY CLUSTERED 
(
	[primary_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[distributions]    Script Date: 26/11/2022 9:23:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[distributions](
	[guid] [nvarchar](max) NULL,
	[commitment] [float] NULL,
	[saving] [float] NULL,
	[desire] [float] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[logs_error]    Script Date: 26/11/2022 9:23:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[logs_error](
	[datetime] [datetime] NULL,
	[hresult] [nvarchar](max) NULL,
	[error_message] [nvarchar](max) NULL,
	[error_full] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[master]    Script Date: 26/11/2022 9:23:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[master](
	[guid] [nvarchar](50) NOT NULL,
	[primary_id] [int] IDENTITY(1,1) NOT NULL,
	[date] [date] NOT NULL,
	[wages] [float] NULL,
	[dist_commitment] [float] NULL,
	[dist_saving] [float] NULL,
	[dist_desire] [float] NULL,
 CONSTRAINT [PK_master] PRIMARY KEY CLUSTERED 
(
	[primary_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[records]    Script Date: 26/11/2022 9:23:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[records](
	[guid] [nvarchar](max) NULL,
	[primary_id] [int] IDENTITY(1,1) NOT NULL,
	[foreign_id] [int] NOT NULL,
	[date] [date] NOT NULL,
	[description] [nvarchar](max) NULL,
	[type] [nvarchar](50) NULL,
	[quantity] [float] NULL,
	[price] [float] NULL,
 CONSTRAINT [PK_records] PRIMARY KEY CLUSTERED 
(
	[primary_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[system_log]    Script Date: 26/11/2022 9:23:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[system_log](
	[master_id] [int] NOT NULL,
	[records_id] [int] NOT NULL,
	[update_date] [datetime] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[administration] ADD  DEFAULT (getdate()) FOR [date_joined]
GO
ALTER TABLE [dbo].[administration] ADD  DEFAULT (getdate()) FOR [last_logged]
GO
ALTER TABLE [dbo].[appdata] ADD  DEFAULT (getdate()) FOR [last_update]
GO
ALTER TABLE [dbo].[logs_error] ADD  DEFAULT (getdate()) FOR [datetime]
GO
ALTER TABLE [dbo].[records] ADD  DEFAULT (getdate()) FOR [date]
GO
ALTER TABLE [dbo].[appdata]  WITH CHECK ADD  CONSTRAINT [FK_files_records] FOREIGN KEY([foreign_id])
REFERENCES [dbo].[records] ([primary_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[appdata] CHECK CONSTRAINT [FK_files_records]
GO
ALTER TABLE [dbo].[records]  WITH CHECK ADD  CONSTRAINT [FK_records_master] FOREIGN KEY([foreign_id])
REFERENCES [dbo].[master] ([primary_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[records] CHECK CONSTRAINT [FK_records_master]
GO
