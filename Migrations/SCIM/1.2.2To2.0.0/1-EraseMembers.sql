BEGIN
	DECLARE @membersId as NVARCHAR(450) = (SELECT [Id] FROM [SCIM].[dbo].[SCIMSchemaAttributeModel] where Name = 'members' and SchemaId = 'urn:ietf:params:scim:schemas:core:2.0:Group');
	DECLARE @groupsId as NVARCHAR(450) = (SELECT [Id] FROM [SCIM].[dbo].[SCIMSchemaAttributeModel] where Name = 'groups' and SchemaId = 'urn:ietf:params:scim:schemas:core:2.0:User');
	
	-- Remove all the attribute mapping.
	DELETE FROM [SCIM].[dbo].[SCIMAttributeMappingLst] where SourceResourceType = 'Users' and TargetResourceType = 'Groups'
	DELETE FROM [SCIM].[dbo].[SCIMAttributeMappingLst] where SourceResourceType = 'Groups' and TargetResourceType = 'Users'

	-- Update mapping
	INSERT INTO [SCIM].[dbo].[SCIMAttributeMappingLst] VALUES(NEWID(), 'Users', 'groups', 'Groups', @membersId, @groupsId);
	INSERT INTO [SCIM].[dbo].[SCIMAttributeMappingLst] VALUES(NEWID(), 'Groups', 'users', 'Users', @groupsId, @membersId);
	
	DECLARE @Id NVARCHAR(450);
	DECLARE @sourceResourceType NVARCHAR(MAX);
	DECLARE @targetResourceType NVARCHAR(MAX);
	DECLARE @targetAttributeId NVARCHAR(MAX);
	DECLARE @sourceAttributeId NVARCHAR(MAX);
	DECLARE @valueAttrId NVARCHAR(450);
	DECLARE attr_mapping_cursor CURSOR FOR SELECT [Id], [SourceResourceType], [TargetResourceType], [TargetAttributeId], [SourceAttributeId] from [SCIM].[dbo].[SCIMAttributeMappingLst] where SourceResourceType = 'Groups' and TargetResourceType = 'Users';
	
	OPEN attr_mapping_cursor 
	FETCH NEXT FROM attr_mapping_cursor INTO @Id, @sourceResourceType, @targetResourceType, @targetAttributeId, @sourceAttributeId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DELETE FROM [dbo].[SCIMRepresentationAttributeValueLst] where SCIMRepresentationAttributeId IN (SELECT [Id] FROM [dbo].[SCIMRepresentationAttributeLst] where ParentId IN (SELECT [Id] from [dbo].[SCIMRepresentationAttributeLst] where SchemaAttributeId = @targetAttributeId))
		DELETE FROM [dbo].[SCIMRepresentationAttributeLst] where ParentId IN (SELECT [Id] from [dbo].[SCIMRepresentationAttributeLst] where SchemaAttributeId = @targetAttributeId) 
		DELETE FROM [dbo].[SCIMRepresentationAttributeLst] where SchemaAttributeId = @targetAttributeId
		FETCH NEXT FROM attr_mapping_cursor INTO @Id, @sourceResourceType, @targetResourceType, @targetAttributeId, @sourceAttributeId
	END
	CLOSE attr_mapping_cursor
	DEALLOCATE attr_mapping_cursor 
END