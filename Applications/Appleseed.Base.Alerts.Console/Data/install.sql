USE [master]
GO
/****** Object:  Database [Appleseed_Base_App_Alerts]    Script Date: 6/28/2017 4:41:44 PM ******/
CREATE DATABASE [Appleseed_Base_App_Alerts] ON  PRIMARY 
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Appleseed_Base_App_Alerts].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET ARITHABORT OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET  MULTI_USER 
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET DB_CHAINING OFF 
GO
USE [Appleseed_Base_App_Alerts]
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [Appleseed_Base_App_Alerts]
GO
/****** Object:  Table [dbo].[Alert_Email_Log]    Script Date: 6/28/2017 4:41:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Alert_Email_Log](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[status] [nchar](15) NOT NULL,
	[log_time] [datetime] NOT NULL,
	[alert_user_setting_id] [int] NOT NULL,
 CONSTRAINT [PK_Alert_Email_Log] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Alert_Types]    Script Date: 6/28/2017 4:41:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Alert_Types](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](150) NULL,
 CONSTRAINT [PK_Alert_Types] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Alert_User_Settings]    Script Date: 6/28/2017 4:41:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Alert_User_Settings](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [uniqueidentifier] NOT NULL,
	[source] [nvarchar](500) NOT NULL,
	[alert_type_id] [int] NOT NULL,
	[alert_schedule] [nchar](20) NOT NULL,
	[last_send] [datetime] NULL,
	[last_status] [nchar](15) NULL,
 CONSTRAINT [PK_Alert_User_Settings] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Alert_Email_Log]  WITH CHECK ADD  CONSTRAINT [FK_Alert_Email_Log_Alert_User_Settings] FOREIGN KEY([alert_user_setting_id])
REFERENCES [dbo].[Alert_User_Settings] ([id])
GO
ALTER TABLE [dbo].[Alert_Email_Log] CHECK CONSTRAINT [FK_Alert_Email_Log_Alert_User_Settings]
GO
ALTER TABLE [dbo].[Alert_User_Settings]  WITH CHECK ADD  CONSTRAINT [FK_Alert_User_Settings_Alert_Types] FOREIGN KEY([alert_type_id])
REFERENCES [dbo].[Alert_Types] ([id])
GO
ALTER TABLE [dbo].[Alert_User_Settings] CHECK CONSTRAINT [FK_Alert_User_Settings_Alert_Types]
GO
USE [master]
GO
ALTER DATABASE [Appleseed_Base_App_Alerts] SET  READ_WRITE 
GO
