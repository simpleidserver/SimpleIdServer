BEGIN
	DECLARE @listOfParentIds table (id NVARCHAR(450))
	DECLARE @parentId NVARCHAR(450)
	DECLARE @userGroupId NVARCHAR(450)
	DECLARE @userRepresentationId NVARCHAR(450)
	DECLARE @groupId NVARCHAR(450)
	DECLARE @groupDisplay NVARCHAR(450)
	DECLARE @parentReferences NVARCHAR(MAX)
	DECLARE @targetAttributeId NVARCHAR(450)
	DECLARE @nbReferences INT
	SET @targetAttributeId = (SELECT TOP(1) Id FROM [dbo].[SCIMSchemaAttribute] WHERE FullPath = 'groups')
	DECLARE @groupsValueAttrId NVARCHAR(450)
	SET @groupsValueAttrId = (SELECT TOP(1) Id FROM [dbo].[SCIMSchemaAttribute] WHERE FullPath = 'groups.value')
	DECLARE @groupsTypeAttrId NVARCHAR(450)
	SET @groupsTypeAttrId = (SELECT TOP(1) Id from [dbo].[SCIMSchemaAttribute] WHERE FullPath = 'groups.type')
	DECLARE @groupsDisplayAttrId NVARCHAR(450)
	SET @groupsDisplayAttrId = (SELECT TOP(1) Id from [dbo].[SCIMSchemaAttribute] WHERE FullPath = 'groups.display')
	DECLARE memberC CURSOR FOR (
		SELECT [attr].[ValueString], [attr].[RepresentationId]
		FROM [dbo].[SCIMRepresentationAttributeLst] [attr]
		INNER JOIN [dbo].[SCIMRepresentationLst] [repr] ON [repr].[Id] = [attr].[ValueString]
		WHERE [attr].FullPath = 'members.value' AND [repr].ResourceType = 'User'
	);

	OPEN memberC
	FETCH NEXT FROM memberC into @userRepresentationId, @groupId

	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXEC [dbo].[ResolveGroups] @groupId, @parentReferences OUTPUT
		INSERT @listOfParentIds SELECT value from string_split(@parentReferences, '_')

		DECLARE listOfParentC CURSOR FOR (
			SELECT * FROM @listOfParentIds
		);

		OPEN listOfParentC
		FETCH NEXT FROM listOfParentC into @parentId
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @nbReferences = (SELECT COUNT(*) FROM [dbo].[SCIMRepresentationIndirectReference] WHERE SCIMRepresentationId = @userRepresentationId AND TargetReferenceId = @parentId)
			IF @nbReferences = 0
			BEGIN
				SET @userGroupId = NEWID()
				SET @groupDisplay = (SELECT [DisplayName] FROM [dbo].[SCIMRepresentationLst] WHERE [Id] = @parentId)
				-- INSERT indirect attributes
				INSERT INTO [dbo].[SCIMRepresentationAttributeLst] ([Id], [AttributeId], [ParentAttributeId], [SchemaAttributeId], [RepresentationId], [FullPath], [ValueString], [Namespace])
				VALUES (@userGroupId, NEWID(), NULL, @targetAttributeId, @userRepresentationId, 'groups', NULL, 'urn:ietf:params:scim:schemas:core:2.0:User')
				INSERT INTO [dbo].[SCIMRepresentationAttributeLst] ([Id], [AttributeId], [ParentAttributeId], [SchemaAttributeId], [RepresentationId], [FullPath], [ValueString], [Namespace])
				VALUES (NEWID(), NEWID(), @userGroupId, @groupsValueAttrId, @userRepresentationId, 'groups.value', @parentId, 'urn:ietf:params:scim:schemas:core:2.0:User')
				INSERT INTO [dbo].[SCIMRepresentationAttributeLst] ([Id], [AttributeId], [ParentAttributeId], [SchemaAttributeId], [RepresentationId], [FullPath], [ValueString], [Namespace])
				VALUES (NEWID(), NEWID(), @userGroupId, @groupsTypeAttrId, @userRepresentationId, 'groups.type', 'indirect', 'urn:ietf:params:scim:schemas:core:2.0:User')
				INSERT INTO [dbo].[SCIMRepresentationAttributeLst] ([Id], [AttributeId], [ParentAttributeId], [SchemaAttributeId], [RepresentationId], [FullPath], [ValueString], [Namespace])
				VALUES (NEWID(), NEWID(), @userGroupId, @groupsDisplayAttrId, @userRepresentationId, 'groups.display', @groupDisplay, 'urn:ietf:params:scim:schemas:core:2.0:User')

				-- INSERT references
				INSERT INTO [dbo].[SCIMRepresentationIndirectReference] ([NbReferences], [TargetReferenceId], [TargetAttributeId], [SCIMRepresentationId]) VALUES (1, @parentId, @targetAttributeId, @userRepresentationId)
			END
			ELSE
				UPDATE [dbo].[SCIMRepresentationIndirectReference] SET [NbReferences] = [NbReferences] + 1 WHERE SCIMRepresentationId = @userRepresentationId AND TargetReferenceId = @parentId
			FETCH NEXT FROM listOfParentC into @parentId
		END

		CLOSE listOfParentC
		DEALLOCATE listOfParentC

		FETCH NEXT FROM memberC into @userRepresentationId, @groupId
	END
	CLOSE memberC
	DEALLOCATE memberC
END