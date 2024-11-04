USE [CHECKMATE]
GO

/****** Object:  Trigger [dbo].[TRIGGERAFTERINSERT]    Script Date: 18/06/2014 11:37:12 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[TRGAFTERINSERT]
   ON  [dbo].[USER_MAIN]
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @userGuid uniqueidentifier;
	declare @userRoleGuid uniqueidentifier;
	declare @projectGuid uniqueidentifier;
	declare @discipline nvarchar(50);
	declare @creatorGuid uniqueidentifier;
	declare @createdDate datetime;

	select @userGuid = i.GUID from inserted i;
	select @userRoleGuid = i.ROLE from inserted i;
	select @projectGuid = i.DPROJECT from inserted i;
	select @creatorGuid = i.CREATEDBY from inserted i;
	select @createdDate = i.CREATED from inserted i;
	select @discipline = i.DDISCIPLINE from inserted i;

	if @projectGuid != '00000000-0000-0000-0000-000000000000' AND @userRoleGuid != '00000000-0000-0000-0000-000000000000'
	BEGIN
    -- Insert statements for trigger here
		insert into USER_PROJECT
		(USERGUID, PROJECTGUID, CREATED, CREATEDBY)
		values(@userGuid, @projectGuid, @createdDate, @creatorGuid);

		insert into USER_DISC
		(USERGUID, DISCIPLINE, CREATED, CREATEDBY)
		values(@userGuid, @discipline, @createdDate, @creatorGuid);
	END
END

GO

