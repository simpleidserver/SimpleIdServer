BEGIN
	DECLARE @membersId as NVARCHAR(450) = (SELECT [Id] FROM [dbo].[SCIMSchemaAttributeModel] where Name = 'members' and SchemaId = 'urn:ietf:params:scim:schemas:core:2.0:Group');
	DECLARE @groupsId as NVARCHAR(450) = (SELECT [Id] FROM [dbo].[SCIMSchemaAttributeModel] where Name = 'groups' and SchemaId = 'urn:ietf:params:scim:schemas:core:2.0:User');
	DECLARE @membersValueId as NVARCHAR(450) = (SELECT [Id] FROM [dbo].[SCIMSchemaAttributeModel] where [Name] = 'value' and ParentId = @membersId);
	DECLARE @groupsValueId as NVARCHAR(450) = (SELECT [Id] FROM [dbo].[SCIMSchemaAttributeModel] where [Name] = 'value' and ParentId = @groupsId);
	
	-- Update mapping
	UPDATE [dbo].[SCIMAttributeMappingLst] set TargetAttributeId = (@membersId) where SourceResourceType = 'Users' and TargetResourceType = 'Groups'
	UPDATE [dbo].[SCIMAttributeMappingLst] set SourceAttributeId = (@groupsId) where SourceResourceType = 'Users' and TargetResourceType = 'Groups'
	UPDATE [dbo].[SCIMAttributeMappingLst] set TargetAttributeId = (@groupsId) where SourceResourceType = 'Groups' and TargetResourceType = 'Users'
	UPDATE [dbo].[SCIMAttributeMappingLst] set SourceAttributeId = (@membersId) where SourceResourceType = 'Groups' and TargetResourceType = 'Users'

	DECLARE @counter INT;
	DECLARE @Id NVARCHAR(450);
	DECLARE @attrId NVARCHAR(450);
	DECLARE @parentId NVARCHAR(450);
	DECLARE @newId NVARCHAR(450);
	DECLARE @sourceRepId NVARCHAR(450);
	DECLARE @sourceRepDisplayName NVARCHAR(MAX);
	DECLARE @valueAttrId NVARCHAR(450);
	DECLARE @targetValueAttrId NVARCHAR(450);
	DECLARE @targetDisplayAttrId NVARCHAR(450);
	DECLARE @sourceResourceType NVARCHAR(MAX);
	DECLARE @targetResourceType NVARCHAR(MAX);
	DECLARE @targetAttributeId NVARCHAR(MAX);
	DECLARE @sourceAttributeId NVARCHAR(MAX);
	DECLARE attr_mapping_cursor CURSOR FOR SELECT [Id], [SourceResourceType], [TargetResourceType], [TargetAttributeId], [SourceAttributeId] from [dbo].[SCIMAttributeMappingLst];
	
	OPEN attr_mapping_cursor 
	FETCH NEXT FROM attr_mapping_cursor INTO @Id, @sourceResourceType, @targetResourceType, @targetAttributeId, @sourceAttributeId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @counter = 0;
		SELECT @valueAttrId = [Id] from [dbo].[SCIMSchemaAttributeModel] where [ParentId] = @sourceAttributeId and [Name] = 'value'
		SELECT @targetValueAttrId = [Id] from [dbo].[SCIMSchemaAttributeModel] where [ParentId] = @targetAttributeId and [Name] = 'value'
		SELECT @targetDisplayAttrId = [Id] from [dbo].[SCIMSchemaAttributeModel] where [ParentId] = @targetAttributeId and [Name] = 'display'
		DECLARE attrid_cursor CURSOR FOR SELECT [dbo].[SCIMRepresentationAttributeValueLst].ValueString, [dbo].[SCIMRepresentationLst].[Id], [dbo].[SCIMRepresentationLst].DisplayName FROM [dbo].[SCIMRepresentationAttributeLst] 
			INNER JOIN [dbo].[SCIMRepresentationLst] ON [dbo].[SCIMRepresentationLst].[Id] = [dbo].[SCIMRepresentationAttributeLst].[RepresentationId]
			INNER JOIN [dbo].[SCIMRepresentationAttributeValueLst] ON [dbo].[SCIMRepresentationAttributeLst].[Id] = [dbo].[SCIMRepresentationAttributeValueLst].SCIMRepresentationAttributeId
			WHERE SchemaAttributeId = @valueAttrId
			AND [dbo].[SCIMRepresentationLst].[ResourceType] = @sourceResourceType
		PRINT 'Start to update ' + @targetResourceType
		OPEN attrid_cursor
		FETCH NEXT FROM attrid_cursor INTO @attrId, @sourceRepId, @sourceRepDisplayName
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF NOT EXISTS(SELECT * FROM [dbo].[SCIMRepresentationAttributeLst] where RepresentationId = @attrId AND SchemaAttributeId = @targetAttributeId)
			BEGIN
				SET @parentId = CAST(NEWID() as NVARCHAR(450));
				INSERT INTO [dbo].[SCIMRepresentationAttributeLst] values(@parentId, NULL, @targetAttributeId, @attrId);
				SET @newId = CAST(NEWID() as NVARCHAR(450));
				INSERT INTO [dbo].[SCIMRepresentationAttributeLst] values(@newId, @parentId, @targetValueAttrId, @attrId);
				INSERT INTO [dbo].[SCIMRepresentationAttributeValueLst] ([Id], ValueString, SCIMRepresentationAttributeId) values(NEWID(), @sourceRepId, @newId);
				if @targetDisplayAttrId IS NOT NULL
				BEGIN
					SET @newId = CAST(NEWID() as NVARCHAR(450));
					INSERT INTO [dbo].[SCIMRepresentationAttributeLst] values(@newId, @parentId, @targetDisplayAttrId, @attrId);
					INSERT INTO [dbo].[SCIMRepresentationAttributeValueLst] ([Id], ValueString, SCIMRepresentationAttributeId) values(NEWID(), @sourceRepDisplayName, @newId);
				END
				
				PRINT 'value=' + @newId +', display=' + @sourceRepDisplayName;
				set @counter = @counter + 1
			END
			FETCH NEXT FROM attrid_cursor INTO @attrId, @sourceRepId, @sourceRepDisplayName
		END
		CLOSE attrid_cursor
		DEALLOCATE attrid_cursor 
		PRINT 'Number of updated ' + @targetResourceType + '=' + CAST(@counter as NVARCHAR);
		FETCH NEXT FROM attr_mapping_cursor INTO @Id, @sourceResourceType, @targetResourceType, @targetAttributeId, @sourceAttributeId
	END
	CLOSE attr_mapping_cursor
	DEALLOCATE attr_mapping_cursor 
END