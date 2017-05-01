USE [anant]
GO

/****** Object:  Table [dbo].[as_Cache_BaseItemCollection]    Script Date: 9/6/2015 5:52:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[as_Cache_BaseItemCollection](
	[FileID] [int] IDENTITY(1,1) NOT NULL,
	[ItemPortalID] [varchar](max) NULL,
	[ItemKey] [varchar](255) NULL,
	[ItemType] [varchar](255) NULL,
	[ItemTitle] [varchar](max) NULL,
	[ItemPath] [varchar](max) NULL,
	[ItemContent] [varchar](max) NULL,
	[ItemSummary] [varchar](max) NULL,
	[ItemFilePath] [varchar](max) NULL,
	[ItemFileName] [varchar](max) NULL,
	[ItemCreatedDate] [datetime] NULL,
	[ItemModifiedDate] [datetime] NULL,
	[ItemViewRoles] [varchar](max) NULL,
	[ItemMeta] [varchar](max) NULL,
	[ItemFileSize] [varchar](max) NULL,
	[ItemSource] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[FileID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


