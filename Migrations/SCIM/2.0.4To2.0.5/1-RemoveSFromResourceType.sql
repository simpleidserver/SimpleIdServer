-- Update representations
UPDATE SCIMRepresentationLst
set [ResourceType] = SUBSTRING([ResourceType], 0, LEN([ResourceType]))
where SUBSTRING([ResourceType], LEN([ResourceType]), 1) = 's'

-- Update schema
UPDATE [SCIMSchemaLst]
set [ResourceType] = SUBSTRING([ResourceType], 0, LEN([ResourceType]))
where SUBSTRING([ResourceType], LEN([ResourceType]), 1) = 's'

-- Update attribute mappin
UPDATE [SCIMAttributeMappingLst]
set [SourceResourceType] = SUBSTRING([SourceResourceType], 0, LEN([SourceResourceType]))
where SUBSTRING([SourceResourceType], LEN([SourceResourceType]), 1) = 's'

UPDATE [SCIMAttributeMappingLst]
set [TargetResourceType] = SUBSTRING([TargetResourceType], 0, LEN([TargetResourceType]))
where SUBSTRING([TargetResourceType], LEN([TargetResourceType]), 1) = 's'