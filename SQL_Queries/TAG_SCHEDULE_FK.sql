USE [CHECKMATE]
GO

ALTER TABLE [dbo].[TAG]  WITH CHECK ADD  CONSTRAINT [FK_TAG_SCHEDULE] FOREIGN KEY([SCHEDULEGUID])
REFERENCES [dbo].[SCHEDULE] ([GUID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[TAG] CHECK CONSTRAINT [FK_TAG_SCHEDULE]
GO


