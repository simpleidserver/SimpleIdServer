if not exists (select * from sysobjects where name='FullPathType' and xtype='U')
	CREATE TABLE dbo.FullPathType
	(  
	   [Name] NVARCHAR(MAX) ,  
	   [Level] INT 
	)

if not exists (select * from sysobjects where name='SCIMSchemaAttribute' and xtype='U')
	EXEC sp_rename 'dbo.SCIMSchemaAttributeModel', 'SCIMSchemaAttribute'

ALTER TABLE dbo.SCIMSchemaAttribute ADD FullPath VARCHAR (MAX) NULL;

if not exists (select * from sysobjects where name='SCIMSchemaExtension' and xtype='U')
	EXEC sp_rename 'dbo.SCIMSchemaExtensionModel', 'SCIMSchemaExtension'

EXEC sp_rename 'dbo.SCIMSchemaExtension.SCIMSchemaModelId', 'SCIMSchemaId', 'COLUMN';

UPDATE SCIMRepresentationLst set [Version] = 0
ALTER TABLE SCIMRepresentationLst ALTER COLUMN [Version] INT NOT NULL;

if not exists (select * from sysobjects where name='SCIMRepresentationSCIMSchema' and xtype='U')
	EXEC sp_rename 'dbo.SCIMRepresentationSchemaLst', 'SCIMRepresentationSCIMSchema'

EXEC sp_rename 'dbo.SCIMRepresentationSCIMSchema.SCIMRepresentationId', 'RepresentationsId', 'COLUMN';
EXEC sp_rename 'dbo.SCIMRepresentationSCIMSchema.SCIMSchemaId', 'SchemasId', 'COLUMN';


ALTER TABLE SCIMRepresentationAttributeLst ADD AttributeId NVARCHAR(MAX) NULL;
ALTER TABLE SCIMRepresentationAttributeLst ADD FullPath NVARCHAR(MAX) NULL;
EXEC sp_rename 'dbo.SCIMRepresentationAttributeLst.ParentId', 'ParentAttributeId', 'COLUMN';
ALTER TABLE SCIMRepresentationAttributeLst ADD ValueBinary NVARCHAR(MAX) NULL;
ALTER TABLE SCIMRepresentationAttributeLst ADD ValueBoolean BIT NULL;
ALTER TABLE SCIMRepresentationAttributeLst ADD ValueDateTime DATETIME2 NULL;
ALTER TABLE SCIMRepresentationAttributeLst ADD ValueDecimal DECIMAL(18,2) NULL;
ALTER TABLE SCIMRepresentationAttributeLst ADD ValueInteger INT NULL;
ALTER TABLE SCIMRepresentationAttributeLst ADD ValueReference NVARCHAR(MAX) NULL;
ALTER TABLE SCIMRepresentationAttributeLst ADD ValueString NVARCHAR(MAX) NULL;

CREATE TABLE dbo.ProcessedAttributeId (
	[Id] NVARCHAR(450),
	[Source] NVARCHAR(450)
)
USE [SCIM]
GO

CREATE TABLE [dbo].[ProvisioningConfigurations](
	[Id] [nvarchar](450) NOT NULL,
	[Type] [int] NOT NULL,
	[ResourceType] [nvarchar](max) NULL,
	[UpdateDateTime] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ProvisioningConfigurations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[ProvisioningConfigurationHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RepresentationId] [nvarchar](max) NULL,
	[RepresentationVersion] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[WorkflowInstanceId] [nvarchar](max) NULL,
	[WorkflowId] [nvarchar](max) NULL,
	[ExecutionDateTime] [datetime2](7) NOT NULL,
	[Exception] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[ProvisioningConfigurationId1] [nvarchar](450) NULL,
	[ProvisioningConfigurationId] [nvarchar](450) NULL,
 CONSTRAINT [PK_ProvisioningConfigurationHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProvisioningConfigurationHistory]  WITH CHECK ADD  CONSTRAINT [FK_ProvisioningConfigurationHistory_ProvisioningConfigurations_ProvisioningConfigurationId] FOREIGN KEY([ProvisioningConfigurationId])
REFERENCES [dbo].[ProvisioningConfigurations] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ProvisioningConfigurationHistory] CHECK CONSTRAINT [FK_ProvisioningConfigurationHistory_ProvisioningConfigurations_ProvisioningConfigurationId]
GO

ALTER TABLE [dbo].[ProvisioningConfigurationHistory]  WITH CHECK ADD  CONSTRAINT [FK_ProvisioningConfigurationHistory_ProvisioningConfigurations_ProvisioningConfigurationId1] FOREIGN KEY([ProvisioningConfigurationId1])
REFERENCES [dbo].[ProvisioningConfigurations] ([Id])
GO

ALTER TABLE [dbo].[ProvisioningConfigurationHistory] CHECK CONSTRAINT [FK_ProvisioningConfigurationHistory_ProvisioningConfigurations_ProvisioningConfigurationId1]
GO



CREATE TABLE [dbo].[ProvisioningConfigurationRecord](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Type] [int] NOT NULL,
	[IsArray] [bit] NOT NULL,
	[ValuesString] [nvarchar](max) NULL,
	[ProvisioningConfigurationId] [nvarchar](450) NULL,
	[ProvisioningConfigurationRecordId] [int] NULL,
 CONSTRAINT [PK_ProvisioningConfigurationRecord] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProvisioningConfigurationRecord]  WITH CHECK ADD  CONSTRAINT [FK_ProvisioningConfigurationRecord_ProvisioningConfigurationRecord_ProvisioningConfigurationRecordId] FOREIGN KEY([ProvisioningConfigurationRecordId])
REFERENCES [dbo].[ProvisioningConfigurationRecord] ([Id])
GO

ALTER TABLE [dbo].[ProvisioningConfigurationRecord] CHECK CONSTRAINT [FK_ProvisioningConfigurationRecord_ProvisioningConfigurationRecord_ProvisioningConfigurationRecordId]
GO

ALTER TABLE [dbo].[ProvisioningConfigurationRecord]  WITH CHECK ADD  CONSTRAINT [FK_ProvisioningConfigurationRecord_ProvisioningConfigurations_ProvisioningConfigurationId] FOREIGN KEY([ProvisioningConfigurationId])
REFERENCES [dbo].[ProvisioningConfigurations] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ProvisioningConfigurationRecord] CHECK CONSTRAINT [FK_ProvisioningConfigurationRecord_ProvisioningConfigurations_ProvisioningConfigurationId]
GO



