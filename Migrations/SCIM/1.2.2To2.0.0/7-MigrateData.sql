BEGIN
BEGIN TRANSACTION;
	DECLARE @ValueString NVARCHAR(MAX);
	DECLARE @ValueInteger INT;
	DECLARE @ValueBoolean BIT;
	DECLARE @ValueDateTime DATETIME2(7);
	DECLARE @ValueDecimal DECIMAL(18,2);
	DECLARE @ValueByte VARBINARY(MAX);
	DECLARE @ValueReference NVARCHAR(MAX);
	DECLARE @SCIMRepresentationAttributeId NVARCHAR(450);
	DECLARE @parentAttributeId NVARCHAR(450);
	DECLARE @schemaAttributeId NVARCHAR(450);
	DECLARE @representationId NVARCHAR(450);
	DECLARE @attributeId NVARCHAR(450);
	DECLARE @fullPath NVARCHAR(MAX);
	DECLARE value_cursor CURSOR FOR SELECT ValueString, ValueInteger, ValueBoolean, ValueDateTime, ValueDecimal, ValueByte, ValueReference, SCIMRepresentationAttributeId from [dbo].[SCIMRepresentationAttributeValueLst];	
	OPEN value_cursor 
	FETCH NEXT FROM value_cursor INTO @ValueString, @ValueInteger, @ValueBoolean, @ValueDateTime, @ValueDecimal, @ValueByte, @ValueReference, @SCIMRepresentationAttributeId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SELECT @parentAttributeId = ParentAttributeId, 
			@schemaAttributeId = SchemaAttributeId,
			@representationId = RepresentationId, 
			@attributeId = AttributeId
			from [dbo].[SCIMRepresentationAttributeLst] where [Id] = @SCIMRepresentationAttributeId;
		DELETE FROM [dbo].[FullPathType];
		select @fullPath = [dbo].get_full_path(0, @schemaAttributeId);
		IF EXISTS(SELECT * FROM [dbo].ProcessedAttributeId where [Id] = @SCIMRepresentationAttributeId and [Source] = 'RepresentationAttribute')
		BEGIN
			UPDATE [dbo].SCIMRepresentationAttributeLst SET
				ValueBoolean = @ValueBoolean, 
				ValueDateTime = @ValueDateTime, 
				ValueDecimal = @ValueDecimal,
				ValueInteger = @ValueInteger, 
				ValueReference = @ValueReference,
				ValueString = @ValueString,
				AttributeId = NEWID(),
				@fullPath = @fullPath
				where Id = @SCIMRepresentationAttributeId
		END

		IF NOT EXISTS(SELECT * FROM [dbo].ProcessedAttributeId where [Id] = @SCIMRepresentationAttributeId and [Source] = 'RepresentationAttribute')
		BEGIN
			INSERT INTO [dbo].SCIMRepresentationAttributeLst values(
				NEWID(), 
				@parentAttributeId, 
				@schemaAttributeId,
				@representationId,
				@attributeId,
				@fullPath,
				null,
				@ValueBoolean,
				@ValueDateTime,
				@ValueDecimal,
				@ValueInteger,
				@ValueReference,
				@ValueString)
		END

		INSERT INTO [dbo].ProcessedAttributeId values (@SCIMRepresentationAttributeId, 'RepresentationAttribute');
		FETCH NEXT FROM value_cursor INTO @ValueString, @ValueInteger, @ValueBoolean, @ValueDateTime, @ValueDecimal, @ValueByte, @ValueReference, @SCIMRepresentationAttributeId
	END	
	CLOSE value_cursor
	DEALLOCATE value_cursor 

	DECLARE @Id NVARCHAR(450);
	DECLARE @canonicalValues NVARCHAR(MAX);
	DECLARE schema_cursor CURSOR FOR SELECT [Id], [CanonicalValues] from [dbo].[SCIMSchemaAttribute];
	OPEN schema_cursor 
	FETCH NEXT FROM schema_cursor INTO @Id, @canonicalValues
	WHILE @@FETCH_STATUS = 0	
	BEGIN
		DELETE FROM [dbo].[FullPathType];
		select @fullPath = [dbo].get_full_path(0, @Id);
		UPDATE [dbo].[SCIMSchemaAttribute] set FullPath = @fullPath, 
		DefaultValueString = null, 
		ReferenceTypes = null, 
		DefaultValueInt = null,
		CanonicalValues = REPLACE(REPLACE(REPLACE(@canonicalValues, '[', ''), ']', ''), '"', '') where [Id] = @Id
		FETCH NEXT FROM schema_cursor INTO @Id, @canonicalValues
	END	
	CLOSE schema_cursor
	DEALLOCATE schema_cursor 
	COMMIT TRANSACTION;
END