USE [CHECKMATE]
GO

/****** Object:  Table [dbo].[CERTIFICATE_MAIN]    Script Date: 9/5/2022 1:57:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CERTIFICATE_MAIN](
	[GUID] [uniqueidentifier] NOT NULL,
	[PROJECTGUID] [uniqueidentifier] NOT NULL,
	[TEMPLATE_NAME] [nvarchar](50) NOT NULL,
	[NUMBER] [nvarchar](100) NOT NULL,
	[DESCRIPTION] [nvarchar](500) NULL,
	[CERTIFICATE] [varbinary](max) NOT NULL,
	[CREATED] [datetime] NOT NULL,
	[CREATEDBY] [uniqueidentifier] NOT NULL,
	[UPDATED] [datetime] NULL,
	[UPDATEDBY] [uniqueidentifier] NULL,
	[DELETED] [datetime] NULL,
	[DELETEDBY] [uniqueidentifier] NULL,
 CONSTRAINT [PK_CERTIFICATE_MAIN] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[CERTIFICATE_MAIN]  WITH CHECK ADD  CONSTRAINT [FK_CERTIFICATE_MAIN_PROJECT] FOREIGN KEY([PROJECTGUID])
REFERENCES [dbo].[PROJECT] ([GUID])
GO

ALTER TABLE [dbo].[CERTIFICATE_MAIN] CHECK CONSTRAINT [FK_CERTIFICATE_MAIN_PROJECT]
GO


