USE [Exemplo.ContaCorrente]
GO
/****** Object:  Table [dbo].[Saldo]    Script Date: 21/05/2019 12:43:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Saldo](
	[Conta] [int] NOT NULL,
	[Valor] [numeric](22, 10) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Transacao]    Script Date: 21/05/2019 12:43:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transacao](
	[ID] [uniqueidentifier] NOT NULL,
	[Conta] [int] NOT NULL,
	[Data] [datetime] NOT NULL,
	[Descricao] [varchar](max) NOT NULL,
	[Valor] [numeric](22, 10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASCa
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
