CREATE TABLE [dbo].[MemberContacts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Photo] [varchar](100) NULL,
	[Meetup] [varchar](300) NULL,
	[Joined] [datetime] NULL,
	[Introduction] [varchar](2000) NULL,
	[Developer] [varchar](2000) NULL,
	[Mobile] [varchar](2000) NULL,
	[Topics] [varchar](2000) NULL,
	[Email] [varchar](300) NULL,
	[LinkedIn] [varchar](300) NULL,
	[Facebook] [varchar](300) NULL,
	[Tumblr] [varchar](300) NULL,
	[Twitter] [varchar](300) NULL
) ON [PRIMARY]
GO

ALTER TABLE dbo.MemberContacts ADD
	HasBeenIndexed bit NULL
GO

ALTER TABLE dbo.MemberContacts ADD CONSTRAINT
	DF_MemberContacts_HasBeenIndexed DEFAULT 0 FOR HasBeenIndexed
GO

UPDATE dbo.MemberContacts SET HasBeenIndexed = 0;
GO

