/****** Object:  Table [dbo].[Report_List]    Script Date: 08/17/2014 11:20:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Report_List](
	[RLID] [int] IDENTITY(1,1) NOT NULL,
	[EID] [int] NULL,
	[StartTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[RLID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Events](
	[EID] [int] IDENTITY(1,1) NOT NULL,
	[ECID] [int] NOT NULL,
	[ETID] [int] NOT NULL,
	[SID] [int] NOT NULL,
	[Name] [varchar](512) NOT NULL,
	[FinishTimeRequired] [bit] NOT NULL,
	[URL] [varchar](512) NULL,
	[Active] [bit] NOT NULL,
	[Documentation] [varchar](512) NULL,
	[Script] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[EID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventListBackup](
	[EID] [int] NULL,
	[Scheduled] [datetime] NULL,
	[Category] [varchar](255) NULL,
	[Name] [varchar](255) NULL,
	[FinishTime] [bit] NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[Documentation] [varchar](512) NULL,
	[SiteLink] [varchar](512) NULL
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventList_Status](
	[SHID] [int] IDENTITY(1,1) NOT NULL,
	[ShiftName] [varchar](32) NOT NULL,
	[InPlay] [bit] NOT NULL,
	[LastCreated] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[SHID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventList_Codes](
	[StatusCode] [int] NOT NULL,
	[CodeDef] [varchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[StatusCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[EventList_Audit](
	[EAID] [int] IDENTITY(1,1) NOT NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[UserName] [varchar](255) NULL,
	[EID] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[ScheduledTime] [datetime] NOT NULL,
	[Forward] [bit] NULL,
 CONSTRAINT [PK__EventList_Audit__2E70E1FD] PRIMARY KEY CLUSTERED 
(
	[EAID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Event_Timing](
	[ETID] [int] IDENTITY(1,1) NOT NULL,
	[SID] [int] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[ScheduleCode] [varchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[ETID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Event_ScheduleCodes](
	[ESCID] [int] IDENTITY(1,1) NOT NULL,
	[ScheduleCode] [int] NOT NULL,
	[DayofWeek] [varchar](24) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ESCID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Event_Schedule](
	[SID] [int] IDENTITY(1,1) NOT NULL,
	[Schedule] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Event_Performance](
	[EPID] [int] IDENTITY(1,1) NOT NULL,
	[EID] [int] NOT NULL,
	[ShiftDate] [datetime] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[EPID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Event_ListBackup](
	[ELBID] [int] IDENTITY(1,1) NOT NULL,
	[ScheduledTime] [datetime] NULL,
	[EID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ELBID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Event_List](
	[ELID] [int] IDENTITY(1,1) NOT NULL,
	[Complete] [bit] NOT NULL,
	[EndTime] [datetime] NULL,
	[EID] [int] NULL,
	[Active] [bit] NOT NULL,
	[ScheduledTime] [datetime] NULL,
	[StartTime] [datetime] NULL,
	[Forward] [bit] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[StatusInfo] [varchar](255) NULL,
	[UserName] [varchar](128) NULL,
PRIMARY KEY CLUSTERED 
(
	[ELID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Event_Comments](
	[CID] [int] IDENTITY(1,1) NOT NULL,
	[EID] [int] NOT NULL,
	[Comment] [varchar](512) NOT NULL,
	[UserName] [varchar](48) NOT NULL,
	[ScheduledTime] [datetime] NOT NULL,
	[CommentTime] [datetime] NOT NULL,
	[Editable] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[CID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Event_Category](
	[ECID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [varchar](255) NOT NULL,
	[CategoryColor] [varchar](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[ECID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Event_Audit](
	[DateCreated] [datetime] NULL,
	[UserName] [varchar](25) NULL,
	[EAUID] [int] IDENTITY(1,1) NOT NULL,
	[EID] [int] NOT NULL,
	[Info] [varchar](510) NULL,
	[Name] [varchar](512) NULL,
	[Category] [varchar](64) NULL,
 CONSTRAINT [PK__Event_Audit__4EDDB18F] PRIMARY KEY CLUSTERED 
(
	[EAUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Event_List]  WITH CHECK ADD FOREIGN KEY([EID])
REFERENCES [dbo].[Events] ([EID])
GO

ALTER TABLE [dbo].[Event_Performance]  WITH CHECK ADD FOREIGN KEY([EID])
REFERENCES [dbo].[Events] ([EID])
GO

ALTER TABLE [dbo].[Event_Timing]  WITH CHECK ADD FOREIGN KEY([SID])
REFERENCES [dbo].[Event_Schedule] ([SID])
GO

ALTER TABLE [dbo].[Events]  WITH CHECK ADD FOREIGN KEY([ECID])
REFERENCES [dbo].[Event_Category] ([ECID])
GO

ALTER TABLE [dbo].[Events]  WITH CHECK ADD FOREIGN KEY([ETID])
REFERENCES [dbo].[Event_Timing] ([ETID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Events]  WITH CHECK ADD FOREIGN KEY([SID])
REFERENCES [dbo].[Event_Schedule] ([SID])
GO

ALTER TABLE [dbo].[Report_List]  WITH CHECK ADD FOREIGN KEY([EID])
REFERENCES [dbo].[Events] ([EID])
GO
