BEGIN
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
	DECLARE attr_mapping_cursor CURSOR FOR SELECT [Id], [SourceResourceType], [TargetResourceType], [TargetAttributeId], [SourceAttributeId] from [dbo].[SCIMAttributeMappingLst] where SourceResourceType = 'Group' and TargetResourceType = 'User';
	
	OPEN attr_mapping_cursor 
	FETCH NEXT FROM attr_mapping_cursor INTO @Id, @sourceResourceType, @targetResourceType, @targetAttributeId, @sourceAttributeId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @counter = 0;
		SELECT @valueAttrId = [Id] from [dbo].[SCIMSchemaAttribute] where [ParentId] = @sourceAttributeId and [Name] = 'value'
		SELECT @targetValueAttrId = [Id] from [dbo].[SCIMSchemaAttribute] where [ParentId] = @targetAttributeId and [Name] = 'value'
		SELECT @targetDisplayAttrId = [Id] from [dbo].[SCIMSchemaAttribute] where [ParentId] = @targetAttributeId and [Name] = 'display'
		DECLARE attrid_cursor CURSOR FOR SELECT [dbo].[SCIMRepresentationAttributeLst].ValueString, [dbo].[SCIMRepresentationLst].[Id], [dbo].[SCIMRepresentationLst].DisplayName FROM [dbo].[SCIMRepresentationAttributeLst] 
			INNER JOIN [dbo].[SCIMRepresentationLst] ON [dbo].[SCIMRepresentationLst].[Id] = [dbo].[SCIMRepresentationAttributeLst].[RepresentationId]
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
				INSERT INTO [dbo].[SCIMRepresentationAttributeLst] ([Id], [SchemaAttributeId], [RepresentationId], [ParentAttributeId], [ResourceType], [Namespace], [FullPath]) values(@parentId, @targetAttributeId, @attrId, NULL, 'User', 'urn:ietf:params:scim:schemas:core:2.0:User', 'groups');
				SET @newId = CAST(NEWID() as NVARCHAR(450));
				INSERT INTO [dbo].[SCIMRepresentationAttributeLst] ([Id], [SchemaAttributeId], [RepresentationId], [ParentAttributeId], [ResourceType], [Namespace], [FullPath], [ValueString]) values(@newId, @targetValueAttrId, @attrId, @parentId, 'User', 'urn:ietf:params:scim:schemas:core:2.0:User', 'groups.value', @sourceRepId);
				if @targetDisplayAttrId IS NOT NULL
				BEGIN
					SET @newId = CAST(NEWID() as NVARCHAR(450));
					INSERT INTO [dbo].[SCIMRepresentationAttributeLst] ([Id], [SchemaAttributeId], [RepresentationId], [ParentAttributeId], [ResourceType], [Namespace], [FullPath], [ValueString]) values(@newId, @targetDisplayAttrId, @attrId, @parentId, 'User', 'urn:ietf:params:scim:schemas:core:2.0:User', 'groups.display', @sourceRepDisplayName);
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