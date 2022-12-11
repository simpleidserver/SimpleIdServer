CREATE PROCEDURE [dbo].[ResolveGroups]
	@representationId NVARCHAR(450),
	@parentReferences NVARCHAR(450) OUTPUT
AS

	DECLARE @parentRepresentationId NVARCHAR(450)
	DECLARE @nbRecords INTEGER
	SET @nbRecords = (
		SELECT COUNT(*)
		FROM [dbo].[SCIMRepresentationAttributeLst] [attr]
		INNER JOIN [dbo].[SCIMRepresentationLst] [repr] ON [repr].[Id] = [attr].[ValueString]
		WHERE [attr].FullPath = 'members.value' AND [attr].RepresentationId = @representationId
		AND [repr].[ResourceType] = 'Group')
	IF @nbRecords = 0 
		SELECT @parentReferences = @parentReferences
	ELSE
		BEGIN
		DECLARE representationC CURSOR LOCAL FOR  (
			SELECT [attr].[ValueString]
			FROM [dbo].[SCIMRepresentationAttributeLst] [attr]
			INNER JOIN [dbo].[SCIMRepresentationLst] [repr] ON [repr].[Id] = [attr].[ValueString]
			WHERE [attr].FullPath = 'members.value' AND [attr].RepresentationId = @representationId
			AND [repr].[ResourceType] = 'Group')
		OPEN representationC
		FETCH NEXT FROM representationC into @parentRepresentationId

		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF @parentReferences IS NULL 
			BEGIN
				SELECT @parentReferences = @parentRepresentationId
			END
			ELSE
			BEGIN
				IF CHARINDEX(@parentReferences, @parentRepresentationId) = 0
					SELECT @parentReferences = CONCAT(@parentReferences,'_', @parentRepresentationId)
			END

			EXEC [dbo].[ResolveGroups] @parentRepresentationId, @parentReferences OUTPUT
			FETCH NEXT FROM representationC into @parentRepresentationId
		END
		CLOSE representationC
		DEALLOCATE representationC
	END
GO