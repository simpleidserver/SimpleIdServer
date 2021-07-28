BEGIN
	DECLARE @Id NVARCHAR(450);
	DECLARE @sourceResourceType NVARCHAR(MAX);
	DECLARE @targetResourceType NVARCHAR(MAX);
	DECLARE @targetAttributeId NVARCHAR(MAX);
	DECLARE @sourceAttributeId NVARCHAR(MAX);
	DECLARE @valueAttrId NVARCHAR(450);
	DECLARE attr_mapping_cursor CURSOR FOR SELECT [Id], [SourceResourceType], [TargetResourceType], [TargetAttributeId], [SourceAttributeId] from [SCIM].[dbo].[SCIMAttributeMappingLst] where [SourceResourceType] = 'Groups';
	
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