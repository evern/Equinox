USE [CHECKMATE]
GO

/****** Object:  Table [dbo].[CERTIFICATE_STATUS]    Script Date: 9/5/2022 1:57:14 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CERTIFICATE_STATUS](
	[GUID] [uniqueidentifier] NOT NULL,
	[CERTIFICATE_MAIN_GUID] [uniqueidentifier] NOT NULL,
	[SEQUENCE_NUMBER] [numeric](10, 0) NOT NULL,
	[STATUS_NUMBER] [numeric](5, 0) NOT NULL,
	[CREATED] [datetime] NOT NULL,
	[CREATEDBY] [uniqueidentifier] NOT NULL,
	[DELETED] [datetime] NULL,
	[DELETEDBY] [uniqueidentifier] NULL,
 CONSTRAINT [PK_CERTIFICATE_STATUS] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


