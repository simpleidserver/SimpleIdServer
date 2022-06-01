BEGIN
BEGIN TRANSACTION;
	DECLARE @parentAttributeId NVARCHAR(450);
	DECLARE value_cursor CURSOR FOR SELECT [user].[ParentAttributeId] as [ParentAttributeId] FROM [dbo].[SCIMRepresentationAttributeLst] [user] LEFT OUTER JOIN [dbo].[SCIMRepresentationAttributeLst] [group] ON [user].[ValueString] = [group].[RepresentationId] AND [group].[FullPath] = 'members.value' AND [group].[ValueString] = [user].[RepresentationId] where [user].FullPath = 'groups.value' and [user].[RepresentationId] IS NOT NULL and [group].[Id] IS NULL;	
	OPEN value_cursor 
	FETCH NEXT FROM value_cursor INTO @parentAttributeId	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DELETE FROM [dbo].[SCIMRepresentationAttributeLst] where [ParentAttributeId] = @parentAttributeId
		DELETE FROM [dbo].[SCIMRepresentationAttributeLst] where [Id] = @parentAttributeId
		FETCH NEXT FROM value_cursor INTO @parentAttributeId
	END
	CLOSE value_cursor
	DEALLOCATE value_cursor 
	COMMIT TRANSACTION;
END