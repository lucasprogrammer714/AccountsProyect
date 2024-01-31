CREATE DATABASE [DB_Accounts]
GO
USE [DB_Accounts]
GO

/****** Object:  Table [dbo].[Clientes]    Script Date: 30/1/2024 17:00:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Clientes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](max) NULL,
	[Apellido] [varchar](max) NULL,
	[Usuario] [nvarchar](50) NULL,
	[Contraseña] [nvarchar](50) NULL,
	[DNI] [nchar](10) NULL,
	[Salt] [varbinary](max) NULL,
 CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



/****** Object:  Table [dbo].[Cuentas]    Script Date: 30/1/2024 17:00:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Cuentas](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NumeroCuenta] [nvarchar](50) NULL,
	[Saldo] [decimal](18, 0) NULL,
	[IdCliente] [int] NULL,
	[FechaCreacion] [datetime] NULL,
	[FechaActualizacion] [datetime] NULL,
 CONSTRAINT [PK_Cuentas] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Cuentas]  WITH CHECK ADD  CONSTRAINT [FK_Cuentas_Clientes] FOREIGN KEY([IdCliente])
REFERENCES [dbo].[Clientes] ([ID])
GO

ALTER TABLE [dbo].[Cuentas] CHECK CONSTRAINT [FK_Cuentas_Clientes]
GO


