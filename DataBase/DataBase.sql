USE [master]
GO
/****** Object:  Database [TelegramPedidosDB]    Script Date: 02/06/2025 13:12:51 ******/
CREATE DATABASE [TelegramPedidosDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TelegramPedidosDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\TelegramPedidosDB.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'TelegramPedidosDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\TelegramPedidosDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [TelegramPedidosDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TelegramPedidosDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TelegramPedidosDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TelegramPedidosDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TelegramPedidosDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [TelegramPedidosDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TelegramPedidosDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [TelegramPedidosDB] SET  MULTI_USER 
GO
ALTER DATABASE [TelegramPedidosDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TelegramPedidosDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TelegramPedidosDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TelegramPedidosDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [TelegramPedidosDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [TelegramPedidosDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [TelegramPedidosDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [TelegramPedidosDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [TelegramPedidosDB]
GO
/****** Object:  Table [dbo].[Administradores]    Script Date: 02/06/2025 13:12:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Administradores](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Usuario] [nvarchar](50) NOT NULL,
	[Contrasena] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categorias]    Script Date: 02/06/2025 13:12:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categorias](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
	[Estado] [bit] NOT NULL,
	[FechaCreacion] [datetime2](7) NOT NULL,
	[FechaActualizacion] [datetime2](7) NULL,
 CONSTRAINT [PK_Categorias] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Clientes]    Script Date: 02/06/2025 13:12:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clientes](
	[Id] [bigint] NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[Telefono] [nvarchar](20) NULL,
	[Username] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CuentasPago]    Script Date: 02/06/2025 13:12:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CuentasPago](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TipoCuenta] [nvarchar](100) NULL,
	[Banco] [nvarchar](100) NULL,
	[Numero] [nvarchar](50) NULL,
	[Titular] [nvarchar](150) NULL,
	[Instrucciones] [nvarchar](max) NULL,
	[Estado] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetallePedido]    Script Date: 02/06/2025 13:12:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetallePedido](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PedidoId] [varchar](20) NOT NULL,
	[Producto] [nvarchar](100) NULL,
	[Cantidad] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Mensajes]    Script Date: 02/06/2025 13:12:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Mensajes](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ClientId] [bigint] NOT NULL,
	[Content] [nvarchar](max) NULL,
	[PhotoBase64] [nvarchar](max) NULL,
	[IsFromAdmin] [bit] NOT NULL,
	[IsRead] [bit] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[FromFirstName] [nvarchar](255) NULL,
	[FromLastName] [nvarchar](255) NULL,
	[FromUsername] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Pedidos]    Script Date: 02/06/2025 13:12:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pedidos](
	[Id] [varchar](20) NOT NULL,
	[ClienteId] [bigint] NOT NULL,
	[TelefonoEntrega] [nvarchar](20) NULL,
	[DireccionEntrega] [nvarchar](255) NULL,
	[Productos] [nvarchar](max) NULL,
	[Observaciones] [nvarchar](255) NULL,
	[MetodoPago] [nvarchar](50) NULL,
	[Estado] [nvarchar](50) NULL,
	[FechaHora] [datetime] NULL,
	[EstadoVerificacionPago] [nvarchar](50) NULL,
	[FechaVerificacionPago] [datetime] NULL,
	[UsuarioVerificacion] [nvarchar](100) NULL,
	[ObservacionesVerificacion] [nvarchar](255) NULL,
	[PagoPendiente] [bit] NOT NULL,
	[PagoConfirmado] [bit] NOT NULL,
	[FechaConfirmacionPago] [datetime] NULL,
	[MetodoConfirmacionPago] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Productos]    Script Date: 02/06/2025 13:12:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Productos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[Descripcion] [nvarchar](255) NULL,
	[Precio] [decimal](10, 2) NOT NULL,
	[CategoriaId] [int] NOT NULL,
	[Estado] [bit] NOT NULL,
	[FechaCreacion] [datetime2](7) NOT NULL,
	[FechaActualizacion] [datetime2](7) NULL,
	[EnPromocion] [bit] NOT NULL,
 CONSTRAINT [PK_Productos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reservas]    Script Date: 02/06/2025 13:12:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reservas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Cliente] [nvarchar](100) NOT NULL,
	[Telefono] [nvarchar](20) NULL,
	[CantidadPersonas] [int] NULL,
	[Fecha] [date] NULL,
	[Hora] [nvarchar](10) NULL,
	[Estado] [nvarchar](20) NULL,
	[Observaciones] [nvarchar](255) NULL,
	[ClienteId] [bigint] NULL,
	[MontoSeguro] [decimal](10, 2) NULL,
	[SeguroPagado] [bit] NULL,
	[FechaPagoSeguro] [datetime] NULL,
	[MetodoPagoSeguro] [nvarchar](50) NULL,
	[ClienteAsistio] [bit] NULL,
	[FechaAsistencia] [datetime] NULL,
	[SeguroReembolsado] [bit] NULL,
	[FechaCreacion] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Administradores] ON 

INSERT [dbo].[Administradores] ([Id], [Usuario], [Contrasena]) VALUES (1, N'admin', N'1234')
SET IDENTITY_INSERT [dbo].[Administradores] OFF
GO
SET IDENTITY_INSERT [dbo].[Categorias] ON 

INSERT [dbo].[Categorias] ([Id], [Nombre], [Estado], [FechaCreacion], [FechaActualizacion]) VALUES (1, N'Comidas Rapidas', 1, CAST(N'2025-05-26T08:21:25.6933333' AS DateTime2), CAST(N'2025-05-26T08:23:43.2000000' AS DateTime2))
INSERT [dbo].[Categorias] ([Id], [Nombre], [Estado], [FechaCreacion], [FechaActualizacion]) VALUES (2, N'Pizzas', 1, CAST(N'2025-05-26T08:21:25.6933333' AS DateTime2), CAST(N'2025-05-29T12:54:17.7666667' AS DateTime2))
INSERT [dbo].[Categorias] ([Id], [Nombre], [Estado], [FechaCreacion], [FechaActualizacion]) VALUES (3, N'Bebidas', 1, CAST(N'2025-05-26T08:21:25.6933333' AS DateTime2), NULL)
INSERT [dbo].[Categorias] ([Id], [Nombre], [Estado], [FechaCreacion], [FechaActualizacion]) VALUES (4, N'Postres', 1, CAST(N'2025-05-26T08:21:25.6933333' AS DateTime2), NULL)
INSERT [dbo].[Categorias] ([Id], [Nombre], [Estado], [FechaCreacion], [FechaActualizacion]) VALUES (5, N'Comida Colombiana', 1, CAST(N'2025-05-26T08:21:25.6933333' AS DateTime2), NULL)
INSERT [dbo].[Categorias] ([Id], [Nombre], [Estado], [FechaCreacion], [FechaActualizacion]) VALUES (6, N'Desayunos', 1, CAST(N'2025-05-26T08:21:25.6933333' AS DateTime2), NULL)
SET IDENTITY_INSERT [dbo].[Categorias] OFF
GO
INSERT [dbo].[Clientes] ([Id], [Nombre], [Telefono], [Username]) VALUES (1247841370, N'Yorfran', NULL, NULL)
INSERT [dbo].[Clientes] ([Id], [Nombre], [Telefono], [Username]) VALUES (1509132877, N'Miguel', NULL, NULL)
INSERT [dbo].[Clientes] ([Id], [Nombre], [Telefono], [Username]) VALUES (1644454715, N'Kogaraashi1', NULL, NULL)
INSERT [dbo].[Clientes] ([Id], [Nombre], [Telefono], [Username]) VALUES (7176516515, N'Moises', NULL, NULL)
INSERT [dbo].[Clientes] ([Id], [Nombre], [Telefono], [Username]) VALUES (7408850245, N'Cristian', NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[CuentasPago] ON 

INSERT [dbo].[CuentasPago] ([Id], [TipoCuenta], [Banco], [Numero], [Titular], [Instrucciones], [Estado]) VALUES (1, N'Deposito de bajo monto', N'DaviPlata ', N'3117312426', N'Miguel Garcia', N'No transfiya', N'Activa')
INSERT [dbo].[CuentasPago] ([Id], [TipoCuenta], [Banco], [Numero], [Titular], [Instrucciones], [Estado]) VALUES (2, N'Deposito de bajo monto', N'Nequi', N'3117312426', N'Miguel Garcia', N'No transfiya', N'Activa')
INSERT [dbo].[CuentasPago] ([Id], [TipoCuenta], [Banco], [Numero], [Titular], [Instrucciones], [Estado]) VALUES (3, N'Cuenta Bancaria', N'Bancolombia', N'91234556543', N'patiño', N'Buscar cuenta...', N'Inactiva')
SET IDENTITY_INSERT [dbo].[CuentasPago] OFF
GO
SET IDENTITY_INSERT [dbo].[DetallePedido] ON 

INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (23, N'280525001', N'Sancocho de Gallina', 5)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (24, N'280525002', N'Cerveza Aguila', 2)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (25, N'290525001', N'Sancocho de Gallina', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (26, N'290525002', N'Arroz con Leche', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (27, N'290525003', N'Arroz con Leche', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (28, N'290525004', N'Arroz con Leche', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (29, N'290525005', N'Arroz con Leche', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (30, N'290525006', N'Bandeja Paisa', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (31, N'290525007', N'Bandeja Paisa', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (32, N'290525008', N'Bandeja Paisa', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (33, N'290525009', N'Bandeja Paisa', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (34, N'290525010', N'Bandeja Paisa', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (35, N'290525011', N'Arepa con Huevo', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (36, N'290525012', N'Hamburguesa Doble', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (37, N'290525013', N'Helado de Curuba', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (38, N'290525014', N'Desayuno Colombiano', 2)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (39, N'290525015', N'Pizza Suprema', 2)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (40, N'290525016', N'Desayuno Colombiano', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (41, N'290525017', N'Perro Caliente Especial', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (42, N'290525018', N'Pizza Suprema', 2)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (43, N'290525019', N'Hamburguesa Doble', 2)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (44, N'290525019', N'Coca Cola 350ml', 2)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (45, N'290525020', N'Pizza Vegetariana', 2)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (46, N'300525001', N'Pizza Vegetariana', 3)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (47, N'300525001', N'Hamburguesa Doble', 2)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (48, N'300525001', N'Coca Cola 350ml', 3)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (49, N'300525001', N'Empanadas de Pollo', 5)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (50, N'300525001', N'Salchipapa Grande', 2)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (51, N'300525001', N'Pizza Suprema', 5)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (52, N'300525002', N'Pizza Vegetariana', 3)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (53, N'300525002', N'Hamburguesa Doble', 2)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (54, N'300525002', N'Coca Cola 350ml', 3)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (55, N'300525002', N'Empanadas de Pollo', 5)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (56, N'300525002', N'Salchipapa Grande', 2)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (57, N'300525002', N'Pizza Suprema', 5)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (58, N'300525003', N'Desayuno Colombiano', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (59, N'300525004', N'Desayuno Colombiano', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (60, N'300525005', N'Pizza Suprema', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (61, N'300525006', N'Ajiaco Bogotano', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (62, N'300525007', N'Pizza Vegetariana', 2)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (63, N'300525008', N'Jugo de Maracuya', 2)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (64, N'300525009', N'Chocolate con Queso', 1)
INSERT [dbo].[DetallePedido] ([Id], [PedidoId], [Producto], [Cantidad]) VALUES (65, N'300525010', N'Desayuno Colombiano', 2)
SET IDENTITY_INSERT [dbo].[DetallePedido] OFF
GO
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'280525001', 1644454715, N'', N'Tu casa', N'5xSancocho de Gallina', N'', N'Efectivo', N'terminado', CAST(N'2025-05-28T11:33:26.103' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'280525002', 7408850245, N'', N'Call 32 a B', N'2xCerveza Aguila', N'', N'Efectivo', N'terminado', CAST(N'2025-05-28T11:35:23.803' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525001', 1509132877, N'', N'Calle 21', N'1xSancocho de Gallina', N'Con bastante presa', N'Efectivo', N'terminado', CAST(N'2025-05-29T00:02:06.333' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525002', 1509132877, N'', N'/pedir', N'1xArroz con Leche', N'', N'Transferencia', N'Cancelado', CAST(N'2025-05-29T08:05:13.443' AS DateTime), NULL, NULL, NULL, NULL, 1, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525003', 1509132877, N'', N'/pedir', N'1xArroz con Leche', N'', N'Transferencia', N'Cancelado', CAST(N'2025-05-29T08:05:13.597' AS DateTime), NULL, NULL, NULL, NULL, 1, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525004', 1509132877, N'', N'/pedir', N'1xArroz con Leche', N'', N'Transferencia', N'Rechazado', CAST(N'2025-05-29T08:05:13.727' AS DateTime), NULL, NULL, NULL, NULL, 0, 1, CAST(N'2025-05-29T08:06:13.223' AS DateTime), N'Manual desde Panel')
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525005', 1509132877, N'', N'/pedir', N'1xArroz con Leche', N'', N'Transferencia', N'Cancelado', CAST(N'2025-05-29T08:05:13.833' AS DateTime), NULL, NULL, NULL, NULL, 1, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525006', 1509132877, N'', N'/pedir', N'1xBandeja Paisa', N'', N'Efectivo', N'terminado', CAST(N'2025-05-29T08:06:58.913' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525007', 1509132877, N'', N'/pedir', N'1xBandeja Paisa', N'', N'Efectivo', N'camino', CAST(N'2025-05-29T08:06:58.980' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525008', 1509132877, N'', N'/pedir', N'1xBandeja Paisa', N'', N'Efectivo', N'preparacion', CAST(N'2025-05-29T08:06:59.040' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525009', 1509132877, N'', N'/pedir', N'1xBandeja Paisa', N'', N'Efectivo', N'terminado', CAST(N'2025-05-29T08:06:59.077' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525010', 1509132877, N'', N'/pedir', N'1xBandeja Paisa', N'', N'Efectivo', N'terminado', CAST(N'2025-05-29T08:06:59.107' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525011', 1509132877, N'', N'Calle 23', N'1xArepa con Huevo', N'', N'Transferencia', N'terminado', CAST(N'2025-05-29T08:23:05.437' AS DateTime), NULL, NULL, NULL, NULL, 0, 1, CAST(N'2025-05-29T08:23:41.873' AS DateTime), N'Manual desde Panel')
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525012', 1509132877, N'', N'Calle 23', N'1xHamburguesa Doble', N'', N'Efectivo', N'terminado', CAST(N'2025-05-29T14:20:09.397' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525013', 1509132877, N'', N'Calle 21', N'1xHelado de Curuba', N'', N'Efectivo', N'Rechazado', CAST(N'2025-05-29T20:41:51.350' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525014', 1509132877, N'', N'Calle 32', N'2xDesayuno Colombiano', N'', N'Efectivo', N'Rechazado', CAST(N'2025-05-29T20:42:12.543' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525015', 1509132877, N'', N'Manzana 22', N'2xPizza Suprema', N'', N'Efectivo', N'Aceptado', CAST(N'2025-05-29T22:06:32.487' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525016', 1509132877, N'', N'Calle 43', N'1xDesayuno Colombiano', N'', N'Efectivo', N'Rechazado', CAST(N'2025-05-29T22:06:57.910' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525017', 1509132877, N'', N'Jejewje', N'1xPerro Caliente Especial', N'Mañana', N'Efectivo', N'Rechazado', CAST(N'2025-05-29T22:08:15.517' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525018', 1509132877, N'', N'Y', N'2xPizza Suprema', N'', N'Efectivo', N'Rechazado', CAST(N'2025-05-29T22:09:22.203' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525019', 1247841370, N'', N'Tv 34', N'2xHamburguesa Doble, 2xCoca Cola 350ml', N'', N'Transferencia', N'Cancelado', CAST(N'2025-05-29T22:16:31.867' AS DateTime), NULL, NULL, NULL, NULL, 1, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'290525020', 1509132877, N'', N'Hhggh', N'2xPizza Vegetariana', N'', N'Transferencia', N'Aceptado', CAST(N'2025-05-29T22:21:26.443' AS DateTime), NULL, NULL, NULL, NULL, 0, 1, CAST(N'2025-05-30T00:14:55.237' AS DateTime), N'Manual desde Panel')
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'300525001', 1509132877, N'', N'/pedir', N'3xPizza Vegetariana, 2xHamburguesa Doble, 3xCoca Cola 350ml, 5xEmpanadas de Pollo, 2xSalchipapa Grande, 5xPizza Suprema', N'', N'Efectivo', N'terminado', CAST(N'2025-05-30T00:16:01.257' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'300525002', 1509132877, N'', N'/pedir', N'3xPizza Vegetariana, 2xHamburguesa Doble, 3xCoca Cola 350ml, 5xEmpanadas de Pollo, 2xSalchipapa Grande, 5xPizza Suprema', N'', N'Efectivo', N'Rechazado', CAST(N'2025-05-30T00:16:01.627' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'300525003', 1509132877, N'', N'Manzana 22', N'1xDesayuno Colombiano', N'', N'Efectivo', N'Rechazado', CAST(N'2025-05-30T00:44:26.973' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'300525004', 1509132877, N'', N'Manzana 22', N'1xDesayuno Colombiano', N'', N'Efectivo', N'Rechazado', CAST(N'2025-05-30T00:44:27.040' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'300525005', 1509132877, N'', N'Calle 32', N'1xPizza Suprema', N'', N'Efectivo', N'Pendiente', CAST(N'2025-05-30T00:58:25.917' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'300525006', 1247841370, N'', N'Tv 1', N'1xAjiaco Bogotano', N'', N'Transferencia', N'PendientePago', CAST(N'2025-05-30T00:58:33.250' AS DateTime), NULL, NULL, NULL, NULL, 1, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'300525007', 1509132877, N'', N'Calle', N'2xPizza Vegetariana', N'', N'Efectivo', N'Aceptado', CAST(N'2025-05-30T00:59:18.533' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'300525008', 1247841370, N'', N'Tv 2', N'2xJugo de Maracuya', N'', N'Efectivo', N'Pendiente', CAST(N'2025-05-30T00:59:40.677' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'300525009', 1509132877, N'', N'/pedir', N'1xChocolate con Queso', N'', N'Efectivo', N'Pendiente', CAST(N'2025-05-30T05:40:32.043' AS DateTime), NULL, NULL, NULL, NULL, 0, 0, NULL, NULL)
INSERT [dbo].[Pedidos] ([Id], [ClienteId], [TelefonoEntrega], [DireccionEntrega], [Productos], [Observaciones], [MetodoPago], [Estado], [FechaHora], [EstadoVerificacionPago], [FechaVerificacionPago], [UsuarioVerificacion], [ObservacionesVerificacion], [PagoPendiente], [PagoConfirmado], [FechaConfirmacionPago], [MetodoConfirmacionPago]) VALUES (N'300525010', 1509132877, N'', N'Calle 27', N'2xDesayuno Colombiano', N'', N'Transferencia', N'PendientePago', CAST(N'2025-05-30T05:54:57.727' AS DateTime), NULL, NULL, NULL, NULL, 1, 0, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Productos] ON 

INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (1, N'Hamburguesa Sencilla', N'Hamburguesa con carne, lechuga, tomate y salsas', CAST(18000.00 AS Decimal(10, 2)), 1, 1, CAST(N'2025-05-26T08:21:25.7466667' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (2, N'Hamburguesa Doble', N'Hamburguesa con doble carne, queso, lechuga y tomate', CAST(25000.00 AS Decimal(10, 2)), 1, 1, CAST(N'2025-05-26T08:21:25.7466667' AS DateTime2), CAST(N'2025-05-26T08:23:53.2766667' AS DateTime2), 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (3, N'Perro Caliente Especial', N'Salchicha con papas, queso, salsas y vegetales', CAST(15000.00 AS Decimal(10, 2)), 1, 1, CAST(N'2025-05-26T08:21:25.7466667' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (4, N'Salchipapa Grande', N'Papas francesas con salchicha, queso y salsas', CAST(20000.00 AS Decimal(10, 2)), 1, 1, CAST(N'2025-05-26T08:21:25.7466667' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (6, N'Arepa Rellena', N'Arepa con pollo, carne o queso', CAST(12000.00 AS Decimal(10, 2)), 1, 1, CAST(N'2025-05-26T08:21:25.7466667' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (7, N'Pizza Hawaiana Personal', N'Pizza con jamÃ³n, piÃ±a y queso mozzarella', CAST(28000.00 AS Decimal(10, 2)), 2, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (8, N'Pizza Pepperoni Mediana', N'Pizza con pepperoni y queso mozzarella', CAST(35000.00 AS Decimal(10, 2)), 2, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (9, N'Pizza Mexicana Grande', N'Pizza con carne, frijoles, jalapeños y queso', CAST(45000.00 AS Decimal(10, 2)), 2, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), CAST(N'2025-05-28T09:31:11.2166667' AS DateTime2), 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (10, N'Pizza Vegetariana', N'Pizza con champiñones, pimenton, cebolla y aceitunas', CAST(32000.00 AS Decimal(10, 2)), 2, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), CAST(N'2025-05-26T11:16:24.6033333' AS DateTime2), 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (11, N'Pizza Cuatro Quesos', N'Pizza con mozzarella, parmesano, gorgonzola y queso de cabra', CAST(38000.00 AS Decimal(10, 2)), 2, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (12, N'Pizza Suprema', N'Pizza con jamÃ³n, pepperoni, champiÃ±ones y pimentÃ³n', CAST(42000.00 AS Decimal(10, 2)), 2, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (13, N'Coca Cola 350ml', N'Refresco de cola en lata', CAST(3500.00 AS Decimal(10, 2)), 3, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), CAST(N'2025-05-28T11:33:11.0566667' AS DateTime2), 1)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (14, N'Jugo Natural de Lulo', N'Jugo fresco de lulo colombiano', CAST(6000.00 AS Decimal(10, 2)), 3, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), CAST(N'2025-05-29T12:52:44.6700000' AS DateTime2), 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (15, N'Aguapanela con Limon', N'Bebida tradicional colombiana', CAST(5000.00 AS Decimal(10, 2)), 3, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), CAST(N'2025-05-29T12:52:09.1733333' AS DateTime2), 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (16, N'Cerveza Aguila', N'Cerveza nacional colombiana 330ml', CAST(4000.00 AS Decimal(10, 2)), 3, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), CAST(N'2025-05-29T12:53:57.9900000' AS DateTime2), 1)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (17, N'Jugo de Maracuya', N'Jugo natural de maracuya', CAST(7500.00 AS Decimal(10, 2)), 3, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), CAST(N'2025-05-26T11:15:08.1833333' AS DateTime2), 1)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (18, N'Agua Cristal 600ml', N'Agua mineral natural', CAST(2500.00 AS Decimal(10, 2)), 3, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), CAST(N'2025-05-29T12:52:50.8066667' AS DateTime2), 1)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (19, N'Tres Leches', N'Torta tradicional de tres leches', CAST(12000.00 AS Decimal(10, 2)), 4, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (20, N'Flan de Coco', N'Flan casero con coco y caramelo', CAST(8500.00 AS Decimal(10, 2)), 4, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (21, N'Brownie con Helado', N'Brownie de chocolate con helado de vainilla', CAST(15000.00 AS Decimal(10, 2)), 4, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (22, N'Arroz con Leche', N'Postre tradicional colombiano', CAST(6000.00 AS Decimal(10, 2)), 4, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (23, N'Cheesecake de MaracuyÃ¡', N'Tarta de queso con maracuyÃ¡', CAST(14000.00 AS Decimal(10, 2)), 4, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (24, N'Helado de Curuba', N'Helado artesanal de curuba', CAST(9000.00 AS Decimal(10, 2)), 4, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (25, N'Bandeja Paisa', N'Plato tipico con frijoles, arroz, carne, huevo y mas', CAST(35000.00 AS Decimal(10, 2)), 5, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), CAST(N'2025-05-29T12:54:10.3633333' AS DateTime2), 1)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (26, N'Sancocho de Gallina', N'Sopa tradicional con gallina y verduras', CAST(28000.00 AS Decimal(10, 2)), 5, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (27, N'Ajiaco Bogotano', N'Sopa de pollo con papa criolla y guascas', CAST(25000.00 AS Decimal(10, 2)), 5, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (28, N'Lechona Tolimense', N'Cerdo relleno con arroz y arveja', CAST(30000.00 AS Decimal(10, 2)), 5, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (29, N'Tamales Vallunos', N'Tamales envueltos en hoja de platano', CAST(18000.00 AS Decimal(10, 2)), 5, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), CAST(N'2025-05-26T11:15:35.8566667' AS DateTime2), 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (30, N'Mondongo Paisa', N'Sopa de mondongo con verduras', CAST(25000.00 AS Decimal(10, 2)), 5, 0, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), CAST(N'2025-05-29T12:54:25.7933333' AS DateTime2), 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (31, N'Desayuno Colombiano', N'Huevos, arepa, queso, chocolate y pan', CAST(16000.00 AS Decimal(10, 2)), 6, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (32, N'Calentado Paisa', N'Frijoles recalentados con arroz, huevo y carne', CAST(18000.00 AS Decimal(10, 2)), 6, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), CAST(N'2025-05-26T11:15:53.0500000' AS DateTime2), 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (33, N'Arepa con Huevo', N'Arepa frita rellena de huevo', CAST(8000.00 AS Decimal(10, 2)), 6, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (34, N'Chocolate con Queso', N'Chocolate caliente con queso campesino', CAST(7000.00 AS Decimal(10, 2)), 6, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), NULL, 0)
INSERT [dbo].[Productos] ([Id], [Nombre], [Descripcion], [Precio], [CategoriaId], [Estado], [FechaCreacion], [FechaActualizacion], [EnPromocion]) VALUES (35, N'Empanadas de Pollo', N'Dos empanadas fritas de pollo con aji', CAST(6000.00 AS Decimal(10, 2)), 6, 1, CAST(N'2025-05-26T08:21:25.7500000' AS DateTime2), CAST(N'2025-05-26T11:16:05.6600000' AS DateTime2), 0)
SET IDENTITY_INSERT [dbo].[Productos] OFF
GO
SET IDENTITY_INSERT [dbo].[Reservas] ON 

INSERT [dbo].[Reservas] ([Id], [Cliente], [Telefono], [CantidadPersonas], [Fecha], [Hora], [Estado], [Observaciones], [ClienteId], [MontoSeguro], [SeguroPagado], [FechaPagoSeguro], [MetodoPagoSeguro], [ClienteAsistio], [FechaAsistencia], [SeguroReembolsado], [FechaCreacion]) VALUES (1, N'Miguel', N'Desconocido', 43, CAST(N'2020-12-21' AS Date), N'7:00', N'Confirmada', N'', 1509132877, CAST(10000.00 AS Decimal(10, 2)), 1, CAST(N'2025-05-29T20:42:39.673' AS DateTime), N'Manual', 0, NULL, 0, CAST(N'2025-05-25T22:55:24.923' AS DateTime))
INSERT [dbo].[Reservas] ([Id], [Cliente], [Telefono], [CantidadPersonas], [Fecha], [Hora], [Estado], [Observaciones], [ClienteId], [MontoSeguro], [SeguroPagado], [FechaPagoSeguro], [MetodoPagoSeguro], [ClienteAsistio], [FechaAsistencia], [SeguroReembolsado], [FechaCreacion]) VALUES (2, N'Miguel', N'Desconocido', 2, CAST(N'2025-06-02' AS Date), N'7:00', N'Completada', N'', 1509132877, CAST(10000.00 AS Decimal(10, 2)), 1, CAST(N'2025-05-29T08:18:41.157' AS DateTime), N'Manual', 1, CAST(N'2025-05-29T08:18:46.057' AS DateTime), 1, CAST(N'2025-05-26T08:25:59.173' AS DateTime))
INSERT [dbo].[Reservas] ([Id], [Cliente], [Telefono], [CantidadPersonas], [Fecha], [Hora], [Estado], [Observaciones], [ClienteId], [MontoSeguro], [SeguroPagado], [FechaPagoSeguro], [MetodoPagoSeguro], [ClienteAsistio], [FechaAsistencia], [SeguroReembolsado], [FechaCreacion]) VALUES (3, N'Cristian', N'Desconocido', 5, CAST(N'2025-05-29' AS Date), N'17:30', N'Completada', N'No.', 7408850245, CAST(10000.00 AS Decimal(10, 2)), 1, CAST(N'2025-05-29T12:55:05.277' AS DateTime), N'Manual', 1, CAST(N'2025-05-29T12:55:13.873' AS DateTime), 0, CAST(N'2025-05-28T11:36:54.533' AS DateTime))
INSERT [dbo].[Reservas] ([Id], [Cliente], [Telefono], [CantidadPersonas], [Fecha], [Hora], [Estado], [Observaciones], [ClienteId], [MontoSeguro], [SeguroPagado], [FechaPagoSeguro], [MetodoPagoSeguro], [ClienteAsistio], [FechaAsistencia], [SeguroReembolsado], [FechaCreacion]) VALUES (4, N'Cristian', N'Desconocido', 5, CAST(N'2025-05-29' AS Date), N'17:30', N'NoShow', N'No.', 7408850245, CAST(10000.00 AS Decimal(10, 2)), 1, CAST(N'2025-05-28T11:37:34.463' AS DateTime), N'Manual', 0, CAST(N'2025-05-28T11:37:45.327' AS DateTime), 0, CAST(N'2025-05-28T11:36:55.263' AS DateTime))
INSERT [dbo].[Reservas] ([Id], [Cliente], [Telefono], [CantidadPersonas], [Fecha], [Hora], [Estado], [Observaciones], [ClienteId], [MontoSeguro], [SeguroPagado], [FechaPagoSeguro], [MetodoPagoSeguro], [ClienteAsistio], [FechaAsistencia], [SeguroReembolsado], [FechaCreacion]) VALUES (5, N'Miguel', N'Desconocido', 3, CAST(N'2025-06-29' AS Date), N'7:00', N'Confirmada', N'', 1509132877, CAST(10000.00 AS Decimal(10, 2)), 1, CAST(N'2025-05-29T22:29:01.477' AS DateTime), N'Manual', 0, NULL, 0, CAST(N'2025-05-29T13:18:03.557' AS DateTime))
INSERT [dbo].[Reservas] ([Id], [Cliente], [Telefono], [CantidadPersonas], [Fecha], [Hora], [Estado], [Observaciones], [ClienteId], [MontoSeguro], [SeguroPagado], [FechaPagoSeguro], [MetodoPagoSeguro], [ClienteAsistio], [FechaAsistencia], [SeguroReembolsado], [FechaCreacion]) VALUES (6, N'Miguel', N'Desconocido', 6, CAST(N'2022-06-21' AS Date), N'7:00', N'Confirmada', N'', 1509132877, CAST(10000.00 AS Decimal(10, 2)), 1, CAST(N'2025-05-29T22:28:43.077' AS DateTime), N'Manual', 0, NULL, 0, CAST(N'2025-05-29T21:34:36.157' AS DateTime))
INSERT [dbo].[Reservas] ([Id], [Cliente], [Telefono], [CantidadPersonas], [Fecha], [Hora], [Estado], [Observaciones], [ClienteId], [MontoSeguro], [SeguroPagado], [FechaPagoSeguro], [MetodoPagoSeguro], [ClienteAsistio], [FechaAsistencia], [SeguroReembolsado], [FechaCreacion]) VALUES (7, N'Miguel', N'Desconocido', 3, CAST(N'2025-05-29' AS Date), N'fecha_hoy', N'Confirmada', N'', 1509132877, CAST(10000.00 AS Decimal(10, 2)), 1, CAST(N'2025-05-29T22:29:37.070' AS DateTime), N'Manual', 0, NULL, 0, CAST(N'2025-05-29T22:29:22.620' AS DateTime))
INSERT [dbo].[Reservas] ([Id], [Cliente], [Telefono], [CantidadPersonas], [Fecha], [Hora], [Estado], [Observaciones], [ClienteId], [MontoSeguro], [SeguroPagado], [FechaPagoSeguro], [MetodoPagoSeguro], [ClienteAsistio], [FechaAsistencia], [SeguroReembolsado], [FechaCreacion]) VALUES (8, N'Miguel', N'Desconocido', 3, CAST(N'2025-05-29' AS Date), N'fecha_hoy', N'Confirmada', N'', 1509132877, CAST(10000.00 AS Decimal(10, 2)), 1, CAST(N'2025-05-29T22:29:35.237' AS DateTime), N'Manual', 0, NULL, 0, CAST(N'2025-05-29T22:29:22.870' AS DateTime))
INSERT [dbo].[Reservas] ([Id], [Cliente], [Telefono], [CantidadPersonas], [Fecha], [Hora], [Estado], [Observaciones], [ClienteId], [MontoSeguro], [SeguroPagado], [FechaPagoSeguro], [MetodoPagoSeguro], [ClienteAsistio], [FechaAsistencia], [SeguroReembolsado], [FechaCreacion]) VALUES (9, N'Miguel', N'Desconocido', 2, CAST(N'2025-05-29' AS Date), N'7:00', N'Solicitud', N'', 1509132877, CAST(10000.00 AS Decimal(10, 2)), 0, NULL, NULL, 0, NULL, 0, CAST(N'2025-05-29T22:59:10.837' AS DateTime))
INSERT [dbo].[Reservas] ([Id], [Cliente], [Telefono], [CantidadPersonas], [Fecha], [Hora], [Estado], [Observaciones], [ClienteId], [MontoSeguro], [SeguroPagado], [FechaPagoSeguro], [MetodoPagoSeguro], [ClienteAsistio], [FechaAsistencia], [SeguroReembolsado], [FechaCreacion]) VALUES (10, N'Miguel', N'Desconocido', 1, CAST(N'2025-05-30' AS Date), N'7:00', N'Solicitud', N'', 1509132877, CAST(10000.00 AS Decimal(10, 2)), 0, NULL, NULL, 0, NULL, 0, CAST(N'2025-05-29T22:59:35.767' AS DateTime))
INSERT [dbo].[Reservas] ([Id], [Cliente], [Telefono], [CantidadPersonas], [Fecha], [Hora], [Estado], [Observaciones], [ClienteId], [MontoSeguro], [SeguroPagado], [FechaPagoSeguro], [MetodoPagoSeguro], [ClienteAsistio], [FechaAsistencia], [SeguroReembolsado], [FechaCreacion]) VALUES (11, N'Miguel', N'Desconocido', 3, CAST(N'2025-05-29' AS Date), N'8:00', N'Solicitud', N'', 1509132877, CAST(10000.00 AS Decimal(10, 2)), 0, NULL, NULL, 0, NULL, 0, CAST(N'2025-05-29T23:02:10.047' AS DateTime))
INSERT [dbo].[Reservas] ([Id], [Cliente], [Telefono], [CantidadPersonas], [Fecha], [Hora], [Estado], [Observaciones], [ClienteId], [MontoSeguro], [SeguroPagado], [FechaPagoSeguro], [MetodoPagoSeguro], [ClienteAsistio], [FechaAsistencia], [SeguroReembolsado], [FechaCreacion]) VALUES (14, N'Yorfran', N'Desconocido', 6, CAST(N'2024-03-12' AS Date), N'18:00', N'Solicitud', N'', 1247841370, CAST(10000.00 AS Decimal(10, 2)), 0, NULL, NULL, 0, NULL, 0, CAST(N'2025-05-30T00:59:15.570' AS DateTime))
INSERT [dbo].[Reservas] ([Id], [Cliente], [Telefono], [CantidadPersonas], [Fecha], [Hora], [Estado], [Observaciones], [ClienteId], [MontoSeguro], [SeguroPagado], [FechaPagoSeguro], [MetodoPagoSeguro], [ClienteAsistio], [FechaAsistencia], [SeguroReembolsado], [FechaCreacion]) VALUES (16, N'Miguel', N'Desconocido', 5, CAST(N'2025-05-31' AS Date), N'12:00', N'Solicitud', N'', 1509132877, CAST(10000.00 AS Decimal(10, 2)), 0, NULL, NULL, 0, NULL, 0, CAST(N'2025-05-30T01:06:00.690' AS DateTime))
INSERT [dbo].[Reservas] ([Id], [Cliente], [Telefono], [CantidadPersonas], [Fecha], [Hora], [Estado], [Observaciones], [ClienteId], [MontoSeguro], [SeguroPagado], [FechaPagoSeguro], [MetodoPagoSeguro], [ClienteAsistio], [FechaAsistencia], [SeguroReembolsado], [FechaCreacion]) VALUES (17, N'Miguel', N'Desconocido', 1, CAST(N'2025-05-30' AS Date), N'7:00', N'Aceptada', N'', 1509132877, CAST(10000.00 AS Decimal(10, 2)), 0, NULL, NULL, 0, NULL, 0, CAST(N'2025-05-30T06:57:41.593' AS DateTime))
SET IDENTITY_INSERT [dbo].[Reservas] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Administ__E3237CF75665C861]    Script Date: 02/06/2025 13:12:52 ******/
ALTER TABLE [dbo].[Administradores] ADD UNIQUE NONCLUSTERED 
(
	[Usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_Categorias_Nombre]    Script Date: 02/06/2025 13:12:52 ******/
ALTER TABLE [dbo].[Categorias] ADD  CONSTRAINT [UK_Categorias_Nombre] UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Mensajes_ClientId]    Script Date: 02/06/2025 13:12:52 ******/
CREATE NONCLUSTERED INDEX [IX_Mensajes_ClientId] ON [dbo].[Mensajes]
(
	[ClientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Mensajes_IsRead]    Script Date: 02/06/2025 13:12:52 ******/
CREATE NONCLUSTERED INDEX [IX_Mensajes_IsRead] ON [dbo].[Mensajes]
(
	[IsRead] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Mensajes_Timestamp]    Script Date: 02/06/2025 13:12:52 ******/
CREATE NONCLUSTERED INDEX [IX_Mensajes_Timestamp] ON [dbo].[Mensajes]
(
	[Timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Pedidos_EstadoVerificacionPago]    Script Date: 02/06/2025 13:12:52 ******/
CREATE NONCLUSTERED INDEX [IX_Pedidos_EstadoVerificacionPago] ON [dbo].[Pedidos]
(
	[EstadoVerificacionPago] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_Productos_Nombre]    Script Date: 02/06/2025 13:12:52 ******/
ALTER TABLE [dbo].[Productos] ADD  CONSTRAINT [UK_Productos_Nombre] UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Categorias] ADD  DEFAULT ((1)) FOR [Estado]
GO
ALTER TABLE [dbo].[Categorias] ADD  DEFAULT (getdate()) FOR [FechaCreacion]
GO
ALTER TABLE [dbo].[Mensajes] ADD  DEFAULT ((0)) FOR [IsFromAdmin]
GO
ALTER TABLE [dbo].[Mensajes] ADD  DEFAULT ((0)) FOR [IsRead]
GO
ALTER TABLE [dbo].[Mensajes] ADD  DEFAULT (getdate()) FOR [Timestamp]
GO
ALTER TABLE [dbo].[Pedidos] ADD  DEFAULT (getdate()) FOR [FechaHora]
GO
ALTER TABLE [dbo].[Pedidos] ADD  DEFAULT ((0)) FOR [PagoPendiente]
GO
ALTER TABLE [dbo].[Pedidos] ADD  DEFAULT ((0)) FOR [PagoConfirmado]
GO
ALTER TABLE [dbo].[Productos] ADD  DEFAULT ((1)) FOR [Estado]
GO
ALTER TABLE [dbo].[Productos] ADD  DEFAULT (getdate()) FOR [FechaCreacion]
GO
ALTER TABLE [dbo].[Productos] ADD  DEFAULT ((0)) FOR [EnPromocion]
GO
ALTER TABLE [dbo].[Reservas] ADD  DEFAULT ((0)) FOR [MontoSeguro]
GO
ALTER TABLE [dbo].[Reservas] ADD  DEFAULT ((0)) FOR [SeguroPagado]
GO
ALTER TABLE [dbo].[Reservas] ADD  DEFAULT ((0)) FOR [ClienteAsistio]
GO
ALTER TABLE [dbo].[Reservas] ADD  DEFAULT ((0)) FOR [SeguroReembolsado]
GO
ALTER TABLE [dbo].[Reservas] ADD  DEFAULT (getdate()) FOR [FechaCreacion]
GO
ALTER TABLE [dbo].[DetallePedido]  WITH CHECK ADD FOREIGN KEY([PedidoId])
REFERENCES [dbo].[Pedidos] ([Id])
GO
ALTER TABLE [dbo].[Pedidos]  WITH CHECK ADD FOREIGN KEY([ClienteId])
REFERENCES [dbo].[Clientes] ([Id])
GO
ALTER TABLE [dbo].[Productos]  WITH CHECK ADD  CONSTRAINT [FK_Productos_Categorias] FOREIGN KEY([CategoriaId])
REFERENCES [dbo].[Categorias] ([Id])
GO
ALTER TABLE [dbo].[Productos] CHECK CONSTRAINT [FK_Productos_Categorias]
GO
ALTER TABLE [dbo].[Reservas]  WITH CHECK ADD  CONSTRAINT [FK_Reservas_Clientes] FOREIGN KEY([ClienteId])
REFERENCES [dbo].[Clientes] ([Id])
GO
ALTER TABLE [dbo].[Reservas] CHECK CONSTRAINT [FK_Reservas_Clientes]
GO
ALTER TABLE [dbo].[Productos]  WITH CHECK ADD  CONSTRAINT [CK_Productos_Precio] CHECK  (([Precio]>(0)))
GO
ALTER TABLE [dbo].[Productos] CHECK CONSTRAINT [CK_Productos_Precio]
GO
USE [master]
GO
ALTER DATABASE [TelegramPedidosDB] SET  READ_WRITE 
GO
