create function [dbo].get_full_path
(
	@level INT,
	@schemaAttributeId NVARCHAR(450)
) returns NVARCHAR(MAX)
as
	begin
		DECLARE @name NVARCHAR(MAX);
		DECLARE @parentId NVARCHAR(MAX);
		DECLARE @result NVARCHAR(MAX);
		DECLARE @sql VARCHAR(4000);
		DECLARE @cmd VARCHAR(4000);
		select @name = [Name], @parentId = [ParentId] from [dbo].[SCIMSchemaAttribute] where [Id] = @schemaAttributeId;
		if (@name IS NOT NULL)
		BEGIN
			set @level = @level + 1
			SET @sql = 'INSERT INTO  dbo.FullPathType values('''+ @name +''', '+CAST(@level as VARCHAR(50))+')'
			SELECT @cmd = 'sqlcmd -S ' + @@servername + ' -d ' + db_name() + ' -Q "' + @sql + '"'
			EXEC master..xp_cmdshell @cmd, 'no_output'
		END
		if (@parentId IS NOT NULL)
		BEGIN
			return [dbo].get_full_path(@level, @parentId)
		END

		SELECT @result = STRING_AGG([T].[Name],'.') FROM (SELECT [Name] FROM dbo.FullPathType ORDER BY [Level] desc OFFSET 0 ROWS) as T;
		return @result
	end