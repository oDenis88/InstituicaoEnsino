CREATE DATABASE [InstituicaoEnsinoDB];
USE [InstituicaoEnsinoDB];

CREATE TABLE [dbo].[Professor] (
	[ID_Professor] int NOT NULL IDENTITY(1,1) PRIMARY KEY, 
	[Nome] nvarchar(254) NOT NULL, 
	[DataUltimaImportacao] datetime
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Aluno] (
	[ID_Aluno] int NOT NULL IDENTITY(1,1) PRIMARY KEY, 
	[ID_Professor] int NOT NULL, 
	[Nome] nvarchar(254) NOT NULL, 
	[Mensalidade] money NOT NULL, 
	[DataVencimento] datetime NOT NULL, 
	FOREIGN KEY ([ID_Professor])
		REFERENCES [dbo].[Professor] ([ID_Professor])
		ON UPDATE CASCADE ON DELETE CASCADE
) ON [PRIMARY]
GO