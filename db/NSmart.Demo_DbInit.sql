USE [master]
GO
CREATE DATABASE [NSmart.Demo01] 
GO
USE [NSmart.Demo01] 
GO
CREATE TABLE [dbo].[Departments](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[DirectorID] [int] NULL,
 CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[DepartmentsToMaxID](
	[MaxID] [int] NOT NULL,
 CONSTRAINT [PK_DepartmentsToMaxID] PRIMARY KEY CLUSTERED 
(
	[MaxID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[Employes](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Gender] [int] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[Dimission] [bit] NOT NULL,
	[DepartmentID] [int] NOT NULL,
	[Age] [int] NOT NULL,
 CONSTRAINT [PK_Employes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  ForeignKey [FK_Employe_Department]    Script Date: 06/27/2012 18:11:42 ******/
ALTER TABLE [dbo].[Employes]  WITH CHECK ADD  CONSTRAINT [FK_Employes_Departments] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Departments] ([ID])
GO
ALTER TABLE [dbo].[Employes] CHECK CONSTRAINT [FK_Employes_Departments]
GO
CREATE TABLE [dbo].[EmployesToMaxID](
	[MaxID] [int] NOT NULL,
 CONSTRAINT [PK_EmployesToMaxID] PRIMARY KEY CLUSTERED 
(
	[MaxID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [Departments] ([ID],[Name],[DirectorID]) VALUES (1,N'总经理办',1)
INSERT [Departments] ([ID],[Name],[DirectorID]) VALUES (2,N'财务部',1)
INSERT [Departments] ([ID],[Name],[DirectorID]) VALUES (3,N'人事部',1)
INSERT [Departments] ([ID],[Name],[DirectorID]) VALUES (4,N'市场部',1)
INSERT [Departments] ([ID],[Name],[DirectorID]) VALUES (5,N'技术部',1)
GO
INSERT [DepartmentsToMaxID] ([MaxID]) VALUES (5)
GO
INSERT [Employes] ([ID],[Name],[Gender],[CreateTime],[Dimission],[DepartmentID],[Age]) VALUES (1,N'张三',0,N'2010-5-1 15:22:00',0,1,28)
INSERT [Employes] ([ID],[Name],[Gender],[CreateTime],[Dimission],[DepartmentID],[Age]) VALUES (2,N'李四',1,N'2010-5-1 15:25:30',0,1,28)
INSERT [Employes] ([ID],[Name],[Gender],[CreateTime],[Dimission],[DepartmentID],[Age]) VALUES (3,N'王五',1,N'2010-5-1 15:25:30',0,2,31)
INSERT [Employes] ([ID],[Name],[Gender],[CreateTime],[Dimission],[DepartmentID],[Age]) VALUES (4,N'赵六',2,N'2012-6-1 16:21:03',0,5,26)
GO
INSERT [EmployesToMaxID] ([MaxID]) VALUES (8)
GO

CREATE DATABASE [NSmart.Demo02] 
GO
USE [NSmart.Demo02] 
GO
CREATE TABLE [dbo].[Departments](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[DirectorID] [int] NULL,
 CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[Employes](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Gender] [int] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[Dimission] [bit] NOT NULL,
	[DepartmentID] [int] NOT NULL,
	[Age] [int] NOT NULL,
 CONSTRAINT [PK_Employes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  ForeignKey [FK_Employe_Department]    Script Date: 06/27/2012 18:11:42 ******/
ALTER TABLE [dbo].[Employes]  WITH CHECK ADD  CONSTRAINT [FK_Employes_Departments] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Departments] ([ID])
GO
ALTER TABLE [dbo].[Employes] CHECK CONSTRAINT [FK_Employes_Departments]
GO
INSERT [Departments] ([ID],[Name],[DirectorID]) VALUES (1,N'总经理办',1)
INSERT [Departments] ([ID],[Name],[DirectorID]) VALUES (2,N'财务部',1)
INSERT [Departments] ([ID],[Name],[DirectorID]) VALUES (3,N'人事部',1)
INSERT [Departments] ([ID],[Name],[DirectorID]) VALUES (4,N'市场部',1)
INSERT [Departments] ([ID],[Name],[DirectorID]) VALUES (5,N'技术部',1)
GO
INSERT [Employes] ([ID],[Name],[Gender],[CreateTime],[Dimission],[DepartmentID],[Age]) VALUES (5,N'孙七',0,N'2010-5-1 15:22:00',0,1,28)
INSERT [Employes] ([ID],[Name],[Gender],[CreateTime],[Dimission],[DepartmentID],[Age]) VALUES (6,N'周八',1,N'2010-5-1 15:25:30',0,1,28)
INSERT [Employes] ([ID],[Name],[Gender],[CreateTime],[Dimission],[DepartmentID],[Age]) VALUES (7,N'吴九',1,N'2010-5-1 15:25:30',0,2,31)
INSERT [Employes] ([ID],[Name],[Gender],[CreateTime],[Dimission],[DepartmentID],[Age]) VALUES (8,N'郑十',2,N'2012-6-1 16:21:03',0,5,26)
GO

