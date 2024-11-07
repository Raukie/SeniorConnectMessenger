GO
CREATE DATABASE SeniorConnectPG8
GO
USE [SeniorConnectPG8]
GO
CREATE TABLE [Users] (
  [ID] int IDENTITY(1, 1),
  [Username] nvarchar(50) NOT NULL,
  [Password] nvarchar(255) NOT NULL,
  [CreatedAt] datetime2 NOT NULL DEFAULT 'SYSDATETIME()',
  [UpdatedAt] datetime2,
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [UserProfile] (
  [ID] int IDENTITY(1, 1),
  [Initials] nvarchar(10) NOT NULL,
  [FirstName] nvarchar(25) NOT NULL,
  [LastName] nvarchar(50) NOT NULL,
  [Gender] nvarchar(25) NOT NULL,
  [BirthDate] date NOT NULL,
  [Street] nvarchar(255),
  [Housenumber] nvarchar(20),
  [Zipcode] nvarchar(20),
  [City] nvarchar(150),
  [Country] nvarchar(3) NOT NULL,
  [SearchRadius] int NOT NULL DEFAULT (20),
  [UserID] int NOT NULL,
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [Replies] (
  [ID] int IDENTITY(1, 1),
  [Content] int NOT NULL,
  [ReplyToID] int,
  [DisucssionID] int NOT NULL,
  [UserID] int NOT NULL,
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [Types] (
  [ID] int IDENTITY(1, 1),
  [Name] nvarchar(50) NOT NULL,
  [IsMemberState] bit DEFAULT (1),
  [IsGroupState] bit DEFAULT (0),
  [IsVacancyState] bit DEFAULT (0),
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [Groups] (
  [ID] int IDENTITY(1, 1),
  [Name] nvarchar(50),
  [OwnerID] int NOT NULL,
  [Type] nvarchar(25),
  [TypeID] int NOT NULL,
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [Vacancies] (
  [ID] int IDENTITY(1, 1),
  [Hours] int NOT NULL,
  [PerDay] bit NOT NULL,
  [RequiresMotivation] bit NOT NULL DEFAULT (1),
  [Name] nvarchar(50) NOT NULL,
  [Start] date NOT NULL,
  [End] date DEFAULT (null),
  [Limit] int NOT NULL DEFAULT (1),
  [City] nvarchar(75) NOT NULL,
  [SearchRadius] int NOT NULL DEFAULT (25),
  [GroupID] int NOT NULL,
  [TypeID] int NOT NULL,
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [Members] (
  [ID] int IDENTITY(1, 1),
  [GroupID] int NOT NULL,
  [VacancyID] int,
  [UserID] int NOT NULL,
  [TypeID] int NOT NULL,
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [MemberActivities] (
  [ID] int IDENTITY(1, 1),
  [Name] nvarchar(50) NOT NULL,
  [Data] nvarchar(max) NOT NULL,
  [ActivityID] int NOT NULL,
  [MemberID] int NOT NULL,
  [TypeID] int NOT NULL,
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [Applications] (
  [ID] int IDENTITY(1, 1),
  [Motivation] text NOT NULL,
  [Reason] int,
  [VacancyID] int NOT NULL,
  [UserID] int NOT NULL,
  [ProcessedByID] int,
  [TypeID] int NOT NULL,
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [Forums] (
  [ID] int IDENTITY(1, 1),
  [Name] nvarchar(100) NOT NULL,
  [Description] int NOT NULL,
  [GroupID] int,
  [CreatedBy] int NOT NULL,
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [Discussions] (
  [ID] int IDENTITY(1, 1),
  [Topic] nvarchar(100),
  [Content] int NOT NULL,
  [ForumID] int NOT NULL,
  [CreatedBy] int NOT NULL,
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [Activities] (
  [ID] int IDENTITY(1, 1),
  [Title] nvarchar(150) NOT NULL,
  [LongDescription] nvarchar(max) NOT NULL,
  [ShortDescription] nvarchar(255) NOT NULL,
  [Date] datetime DEFAULT (null),
  [Location] nvarchar(255) NOT NULL,
  [GroupID] int NOT NULL,
  [CreatedBy] int NOT NULL,
  [TypeID] int NOT NULL,
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [Roles] (
  [ID] int IDENTITY(1, 1),
  [Name] nvarchar(25) NOT NULL,
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [UserRoles] (
  [UserID] int NOT NULL,
  [RoleID] int NOT NULL
)
GO

CREATE TABLE [Chats] (
  [ID] int IDENTITY(1, 1),
  [Name] nvarchar(64) NOT NULL,
  [IsGroupChat] bit NOT NULL,
  [Hash] nvarchar(256),
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [UserChats] (
  [ID] int IDENTITY(1, 1),
  [UserID] int NOT NULL,
  [LastReadMessageID] int NOT NULL,
  [ChatID] int,
  [IsAdmin] bit NOT NULL,
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [Messages] (
  [ID] int IDENTITY(1, 1),
  [ChatID] int NOT NULL,
  [Content] nvarchar(255) NOT NULL,
  [UserID] int,
  [SendAt] datetime NOT NULL,
  PRIMARY KEY ([ID])
)
GO

CREATE TABLE [GroupChats] (
  [ID] int IDENTITY(1, 1),
  [MemberID] int NOT NULL,
  [LastReadMessageID] int NOT NULL,
  [ChatID] int NOT NULL,
  [IsAdmin] bit NOT NULL,
  PRIMARY KEY ([ID])
)
GO

CREATE UNIQUE INDEX [Users_index_0] ON [Users] ("Username")
GO

CREATE UNIQUE INDEX [UK_UserProfile_UserID] ON [UserProfile] ("UserID")
GO

ALTER TABLE [UserProfile] ADD CONSTRAINT [FK_UserProfile_Users_ID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([ID])
GO

ALTER TABLE [Replies] ADD CONSTRAINT [FK_Replies_Replies_ID] FOREIGN KEY ([ReplyToID]) REFERENCES [Replies] ([ID])
GO

ALTER TABLE [Replies] ADD CONSTRAINT [FK_Replies_Users_ID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([ID])
GO

ALTER TABLE [Groups] ADD CONSTRAINT [FK_Groups_Types_ID] FOREIGN KEY ([TypeID]) REFERENCES [Types] ([ID])
GO

ALTER TABLE [Groups] ADD CONSTRAINT [FK_Groups_Users_ID] FOREIGN KEY ([OwnerID]) REFERENCES [Users] ([ID])
GO

ALTER TABLE [Vacancies] ADD CONSTRAINT [FK_Vacancies_Groups_ID] FOREIGN KEY ([GroupID]) REFERENCES [Groups] ([ID])
GO

ALTER TABLE [Vacancies] ADD CONSTRAINT [FK_Vacancies_Types_ID] FOREIGN KEY ([TypeID]) REFERENCES [Types] ([ID])
GO

ALTER TABLE [Members] ADD CONSTRAINT [FK_Members_Types_ID] FOREIGN KEY ([TypeID]) REFERENCES [Types] ([ID])
GO

ALTER TABLE [Members] ADD CONSTRAINT [FK_Members_UserProfile_ID] FOREIGN KEY ([UserID]) REFERENCES [UserProfile] ([ID])
GO

ALTER TABLE [Members] ADD CONSTRAINT [FK_Members_Vacancies_ID] FOREIGN KEY ([VacancyID]) REFERENCES [Vacancies] ([ID])
GO

ALTER TABLE [MemberActivities] ADD CONSTRAINT [FK_MemberActivities_Members_ID] FOREIGN KEY ([MemberID]) REFERENCES [Members] ([ID])
GO

ALTER TABLE [MemberActivities] ADD CONSTRAINT [FK_MemberActivities_Types_ID] FOREIGN KEY ([TypeID]) REFERENCES [Types] ([ID])
GO

ALTER TABLE [Applications] ADD CONSTRAINT [FK_Applications_Members_ID] FOREIGN KEY ([ProcessedByID]) REFERENCES [Members] ([ID])
GO

ALTER TABLE [Applications] ADD CONSTRAINT [FK_Applications_Types_ID] FOREIGN KEY ([TypeID]) REFERENCES [Types] ([ID])
GO

ALTER TABLE [Applications] ADD CONSTRAINT [FK_Applications_Users_ID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([ID])
GO

ALTER TABLE [Applications] ADD CONSTRAINT [FK_Applications_Vacancies_ID] FOREIGN KEY ([VacancyID]) REFERENCES [Vacancies] ([ID])
GO

ALTER TABLE [Forums] ADD CONSTRAINT [FK_Forums_Groups_ID] FOREIGN KEY ([GroupID]) REFERENCES [Groups] ([ID])
GO

ALTER TABLE [Forums] ADD CONSTRAINT [FK_Forums_Users_ID] FOREIGN KEY ([CreatedBy]) REFERENCES [Users] ([ID])
GO

ALTER TABLE [Discussions] ADD CONSTRAINT [FK_Discussions_Forums_ID] FOREIGN KEY ([ForumID]) REFERENCES [Forums] ([ID])
GO

ALTER TABLE [Discussions] ADD CONSTRAINT [FK_Discussions_Users_ID] FOREIGN KEY ([CreatedBy]) REFERENCES [Users] ([ID])
GO

ALTER TABLE [Activities] ADD CONSTRAINT [FK_Activities_Groups_ID] FOREIGN KEY ([GroupID]) REFERENCES [Groups] ([ID])
GO

ALTER TABLE [Activities] ADD CONSTRAINT [FK_Activities_Types_ID] FOREIGN KEY ([TypeID]) REFERENCES [Types] ([ID])
GO

ALTER TABLE [Activities] ADD CONSTRAINT [FK_Activities_Users_ID] FOREIGN KEY ([CreatedBy]) REFERENCES [Users] ([ID])
GO

ALTER TABLE [UserRoles] ADD CONSTRAINT [FK_UserRoles_Roles_ID] FOREIGN KEY ([RoleID]) REFERENCES [Roles] ([ID])
GO

ALTER TABLE [UserRoles] ADD CONSTRAINT [FK_UserRoles_Users_ID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([ID])
GO

ALTER TABLE [UserChats] ADD CONSTRAINT [FK_UserChats_Chats_ID] FOREIGN KEY ([ChatID]) REFERENCES [Chats] ([ID])
GO

ALTER TABLE [UserChats] ADD CONSTRAINT [FK_UserChats_Users_ID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([ID])
GO

ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_Chats_ID] FOREIGN KEY ([ChatID]) REFERENCES [Chats] ([ID])
GO

ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_UserProfile_ID] FOREIGN KEY ([UserID]) REFERENCES [UserProfile] ([ID])
GO

ALTER TABLE [GroupChats] ADD CONSTRAINT [FK_GroupChats_Chats_ID] FOREIGN KEY ([ChatID]) REFERENCES [Chats] ([ID])
GO

ALTER TABLE [GroupChats] ADD CONSTRAINT [FK_GroupChats_Members_ID] FOREIGN KEY ([MemberID]) REFERENCES [Members] ([ID])
GO



USE SeniorConnectPG8
GO
--
-- Inserting data into table [PG8_SeniorConnect].[dbo].[Types]
--
SET IDENTITY_INSERT dbo.Types ON
GO
INSERT dbo.Types(ID, Name, IsMemberState, IsGroupState, IsVacancyState) VALUES (1, N'Volunteer', 0, 1, 0)
INSERT dbo.Types(ID, Name, IsMemberState, IsGroupState, IsVacancyState) VALUES (2, N'Tutor', NULL, 1, 1)
INSERT dbo.Types(ID, Name, IsMemberState, IsGroupState, IsVacancyState) VALUES (3, N'Unprocessed', 0, 1, 1)
INSERT dbo.Types(ID, Name, IsMemberState, IsGroupState, IsVacancyState) VALUES (4, N'Approved', 0, 0, 0)
INSERT dbo.Types(ID, Name, IsMemberState, IsGroupState, IsVacancyState) VALUES (5, N'Denied', 1, 0, 1)
INSERT dbo.Types(ID, Name, IsMemberState, IsGroupState, IsVacancyState) VALUES (6, N'Frozen', 1, 1, 0)
INSERT dbo.Types(ID, Name, IsMemberState, IsGroupState, IsVacancyState) VALUES (7, N'Expelled', 1, 1, NULL)
INSERT dbo.Types(ID, Name, IsMemberState, IsGroupState, IsVacancyState) VALUES (8, N'OnHold', 1, 1, 0)
INSERT dbo.Types(ID, Name, IsMemberState, IsGroupState, IsVacancyState) VALUES (9, N'Open', 0, 1, 0)
INSERT dbo.Types(ID, Name, IsMemberState, IsGroupState, IsVacancyState) VALUES (10, N'Full', 1, 0, 0)
INSERT dbo.Types(ID, Name, IsMemberState, IsGroupState, IsVacancyState) VALUES (11, N'Expired', 1, NULL, 0)
GO
SET IDENTITY_INSERT dbo.Types OFF
GO

--
-- Set NOEXEC to off
--
GO