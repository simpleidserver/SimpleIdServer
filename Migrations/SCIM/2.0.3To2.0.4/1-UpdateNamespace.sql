UPDATE [attr]
set [attr].[Namespace] = [sAttr].[SchemaId]
FROM [dbo].[SCIMRepresentationAttributeLst] [attr]
INNER JOIN [dbo].[SCIMSchemaAttribute] [sAttr] on [attr].[SchemaAttributeId] = [sAttr].[Id]