/****** Object:  Table [dbo].[as_Cache_BaseItemCollection]    Script Date: 04/22/2015 00:00:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

if db_id('anant') is null
BEGIN
CREATE DATABASE anant	
END
GO

USE anant
GO

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'as_Cache_BaseItemCollection'))
BEGIN
CREATE TABLE [dbo].as_Cache_BaseItemCollection(
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
PRIMARY KEY CLUSTERED 
(
	[FileID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

SET ANSI_PADDING OFF
GO

BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.as_Cache_BaseItemCollection SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
