USE [padhyaso_Clinic]
GO
/****** Object:  UserDefinedFunction [dbo].[IsAuthorize_User]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[IsAuthorize_User]
(
	@Action VARCHAR(10) = NULL
	,@MenuId [bigint] = NULL
	,@User_Id [bigint] = NULL
	,@RoleId [bigint] = NULL
)
RETURNS BIT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result BIT = 0;
	
	-- Super Admin
	IF EXISTS (SELECT * FROM [dbo].[Users] WHERE [Id] = @User_Id) AND @User_Id = 1 
		AND @RoleId = 1
		AND EXISTS (SELECT * FROM [dbo].[Roles] WHERE [Id] = @RoleId AND IsAdmin = 1 AND IsActive = 1 AND IsDeleted = 0) 
	BEGIN SET @Result = 1; END
	
	-- Admin Users
	ELSE IF EXISTS (SELECT * FROM [dbo].[Users] WHERE [Id] = @User_Id AND IsActive = 1 AND IsDeleted = 0) 
		AND @RoleId != 1 
		AND EXISTS (SELECT * FROM [dbo].[Roles] WHERE [Id] = @RoleId AND IsAdmin = 1 AND IsActive = 1 AND IsDeleted = 0) 
		AND EXISTS (SELECT * FROM [dbo].[UserRoleMapping] WHERE [Id] = @User_Id AND [RoleId] = @RoleId AND IsActive = 1 AND IsDeleted = 0) 
		BEGIN 
			
			IF EXISTS (SELECT * FROM [dbo].[Menu] WHERE [Id] = @MenuId AND IsSuperAdmin = 0 AND IsActive = 1 AND IsDeleted = 0) 
				AND EXISTS (SELECT * FROM [dbo].[UserMenuAccess] WHERE [UserId] = @User_Id AND [RoleId] = @RoleId 
																		AND [MenuId] = @MenuId AND IsActive = 1 AND IsDeleted = 0
																		AND (1 = IIF(@Action = 'INSERT' AND IsCreate = 1,1,0)) OR 1 = IIF((@Action = 'UPDATE' OR @Action = 'STATUS') AND IsUpdate = 1,1,0) 
																		OR 1 = IIF(ISNULL(@Action, '') = '' AND IsRead = 1,1,0) OR 1 = IIF(@Action = 'DELETE' AND IsDelete = 1,1,0))
			BEGIN SET @Result = 1; END
			ELSE BEGIN SET @Result = 0; END

		END

	-- Othe Users
	ELSE IF EXISTS (SELECT * FROM [dbo].[Users] WHERE [Id] = @User_Id AND IsActive = 1 AND IsDeleted = 0) 
		AND @RoleId != 1 
		AND EXISTS (SELECT * FROM [dbo].[Roles] WHERE [Id] = @RoleId AND IsAdmin = 0 AND IsActive = 1 AND IsDeleted = 0) 
		AND EXISTS (SELECT * FROM [dbo].[UserRoleMapping] WHERE [Id] = @User_Id AND [RoleId] = @RoleId AND IsActive = 1 AND IsDeleted = 0) 
		BEGIN 
			
			IF EXISTS (SELECT * FROM [dbo].[Menu] WHERE [Id] = @MenuId AND IsSuperAdmin = 0 AND IsAdmin = 0 AND IsActive = 1 AND IsDeleted = 0) 
				AND EXISTS (SELECT * FROM [dbo].[UserMenuAccess] WHERE [UserId] = @User_Id AND [RoleId] = @RoleId 
																		AND [MenuId] = @MenuId AND IsActive = 1 AND IsDeleted = 0
																		AND (1 = IIF(@Action = 'INSERT' AND IsCreate = 1,1,0)) OR 1 = IIF((@Action = 'UPDATE' OR @Action = 'STATUS') AND IsUpdate = 1,1,0) 
																		OR 1 = IIF(ISNULL(@Action, '') = '' AND IsRead = 1,1,0) OR 1 = IIF(@Action = 'DELETE' AND IsDelete = 1,1,0))
			BEGIN SET @Result = 1; END
			ELSE BEGIN SET @Result = 0; END

		END

	-- UnAuthrize Users
	ELSE BEGIN SET @Result = 0; END

	RETURN @Result;

END
GO
/****** Object:  UserDefinedFunction [dbo].[SplitString]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[SplitString]
(    
    @Input NVARCHAR(MAX),
    @Character CHAR(1),
    @OutType CHAR(1)
)
RETURNS @Output TABLE (
    ItemStr NVARCHAR(MAX),
    ItemNum BIGINT
)
AS
BEGIN
    DECLARE @StartIndex INT, @EndIndex INT
 
    SET @StartIndex = 1
    IF SUBSTRING(@Input, LEN(@Input) - 1, LEN(@Input)) <> @Character
    BEGIN
        SET @Input = @Input + @Character
    END
 
    WHILE CHARINDEX(@Character, @Input) > 0
    BEGIN
        SET @EndIndex = CHARINDEX(@Character, @Input)
         
		IF(@OutType = 'S')
		BEGIN
		
			INSERT INTO @Output(ItemStr)
			SELECT SUBSTRING(@Input, @StartIndex, @EndIndex - 1)
         
		END
		ELSE 
		BEGIN
		
			INSERT INTO @Output(ItemNum)
			SELECT CONVERT(BIGINT, SUBSTRING(@Input, @StartIndex, @EndIndex - 1))
         
		END

        SET @Input = SUBSTRING(@Input, @EndIndex + 1, LEN(@Input))
    END
 
    RETURN
END
GO
/****** Object:  Table [dbo].[Appointment]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Appointment](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DepartmentId] [bigint] NULL,
	[DoctorId] [bigint] NULL,
	[PatientId] [bigint] NULL,
	[Date_Time] [datetime2](7) NULL,
	[Status] [nvarchar](max) NULL,
	[Disease] [nvarchar](max) NULL,
	[Prescription] [nvarchar](max) NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Appointment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Attachments]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Attachments](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Extension] [nvarchar](max) NOT NULL,
	[Size] [bigint] NOT NULL,
	[Type] [nvarchar](max) NULL,
	[Path] [nvarchar](max) NOT NULL,
	[Remarks] [nvarchar](max) NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Attachments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Department]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Department](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[LastModifiedBy] [bigint] NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Doctor]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Doctor](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[Firstname] [nvarchar](max) NOT NULL,
	[Lastname] [nvarchar](max) NOT NULL,
	[Work_Experience] [int] NOT NULL,
	[Specialization] [nvarchar](max) NULL,
	[Qualification] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[BirthDate] [datetime2](7) NULL,
	[Gender] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[ContactNo] [nvarchar](max) NULL,
	[Alt_ContactNo] [nvarchar](max) NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Doctor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employee]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NULL,
	[MiddleName] [nvarchar](50) NOT NULL,
	[UserType] [nvarchar](50) NULL,
	[Address] [nvarchar](50) NULL,
	[CityId] [bigint] NULL,
	[StateId] [bigint] NULL,
	[CountryId] [bigint] NULL,
	[Gender] [char](10) NULL,
	[Position] [nvarchar](50) NULL,
	[ContactNo] [nvarchar](10) NULL,
	[BloodGroup] [nvarchar](50) NULL,
	[BirthDate] [datetime2](7) NULL,
	[HireDate] [datetime2](7) NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Lov_Master]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Lov_Master](
	[Lov_Column] [nvarchar](max) NOT NULL,
	[Lov_Code] [nvarchar](max) NOT NULL,
	[Lov_Desc] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Menu]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Menu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ParentId] [bigint] NOT NULL,
	[Area] [nvarchar](50) NULL,
	[Controller] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Icon] [nvarchar](50) NULL,
	[DisplayOrder] [int] NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[IsSuperAdmin] [bit] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Menu_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Patient]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Patient](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Firstname] [nvarchar](max) NOT NULL,
	[Lastname] [nvarchar](max) NOT NULL,
	[Address] [nvarchar](max) NULL,
	[BirthDate] [datetime2](7) NULL,
	[Age] [int] NOT NULL,
	[Gender] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[ContactNo] [nvarchar](max) NULL,
	[Alt_ContactNo] [nvarchar](max) NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Patient] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DoctorId] [bigint] NOT NULL,
	[PatientId] [bigint] NOT NULL,
	[AppointmentId] [bigint] NOT NULL,
	[DepartmentId] [bigint] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[PaymentType] [nvarchar](max) NULL,
	[Status] [nvarchar](max) NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Payment_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleMenuAccess]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleMenuAccess](
	[RoleId] [bigint] NOT NULL,
	[MenuId] [bigint] NOT NULL,
	[IsRead] [bit] NOT NULL,
	[IsCreate] [bit] NOT NULL,
	[IsUpdate] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_RoleMenuAccess] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC,
	[MenuId] ASC,
	[IsRead] ASC,
	[IsCreate] ASC,
	[IsUpdate] ASC,
	[IsDelete] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[DisplayOrder] [int] NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserMenuAccess]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserMenuAccess](
	[UserId] [bigint] NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[MenuId] [bigint] NOT NULL,
	[IsRead] [bit] NOT NULL,
	[IsCreate] [bit] NOT NULL,
	[IsUpdate] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_UserMenuAccess] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC,
	[MenuId] ASC,
	[IsRead] ASC,
	[IsCreate] ASC,
	[IsUpdate] ASC,
	[IsDelete] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRoleMapping]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoleMapping](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_UserRoleMapping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](30) NOT NULL,
	[Password] [varchar](150) NOT NULL,
	[No_Of_Wrong_Password_Attempts] [int] NULL,
	[Next_Change_Password_Date] [datetime2](7) NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Users_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Employee] ADD  CONSTRAINT [DF_Employee_CreatedBy]  DEFAULT ((0)) FOR [CreatedBy]
GO
ALTER TABLE [dbo].[Employee] ADD  CONSTRAINT [DF_Employee_LastModifiedBy]  DEFAULT ((0)) FOR [LastModifiedBy]
GO
ALTER TABLE [dbo].[UserRoleMapping] ADD  CONSTRAINT [DF_UserRoleMapping_CreatedBy]  DEFAULT ((0)) FOR [CreatedBy]
GO
ALTER TABLE [dbo].[UserRoleMapping] ADD  CONSTRAINT [DF_UserRoleMapping_LastModifiedBy]  DEFAULT ((0)) FOR [LastModifiedBy]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_CreatedBy]  DEFAULT ((0)) FOR [CreatedBy]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_LastModifiedBy]  DEFAULT ((0)) FOR [LastModifiedBy]
GO
/****** Object:  StoredProcedure [dbo].[SP_Department_Save]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_Department_Save]
	@Id [bigint] = NULL
	,@Name [nvarchar](MAX) = NULL
	,@Description [nvarchar](MAX) = NULL
	,@IsActive [bit] = NULL
	,@Operated_By [bigint] = NULL
	,@Operated_RoleId [bigint] = NULL
	,@Operated_MenuId [bigint] = NULL
	,@Action VARCHAR(10) = NULL
	,@response nvarchar(MAX) out
AS
BEGIN

	SET NOCOUNT ON;

	SET @response = 'E|' + 'Opps!... Something went wrong. Please contact system administrator|0';
		
    IF EXISTS (SELECT * FROM [dbo].[Department] WHERE UPPER(TRIM([Name])) = UPPER(TRIM(@Name)) AND Id != @Id)
		BEGIN SET @response =  'E|' + 'Department is already exists|0'; END
	ELSE
	BEGIN

		--INSERT
		IF @Action = 'INSERT'
		BEGIN
		
			INSERT INTO [dbo].[Department]([Name], [Description], CreatedBy, CreatedDate, IsActive, IsDeleted)
			VALUES (@Name, @Description, @Operated_By, GETDATE(), @IsActive, 0)
 
			SET @response =  'S|' + 'Record saved successfully|' + CONVERT(NVARCHAR(MAX), SCOPE_IDENTITY());
		
		END
 
		  --UPDATE
		IF @Action = 'UPDATE'
		  BEGIN
			
			UPDATE [dbo].[Department] 
				SET [Name] = @Name
					, [Description] = @Description
					, IsActive = @IsActive, LastModifiedBy = @Operated_By, LastModifiedDate = GETDATE()
				WHERE [Id]= @Id;
 
				SET @response =  'S|' + 'Record saved successfully|' + CONVERT(NVARCHAR(MAX), @Id);

		  END
 
	END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Department_Status]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_Department_Status]
	@Id [bigint] = NULL
	,@IsActive [bit] = NULL
	,@Operated_By [bigint] = NULL
	,@Operated_RoleId [bigint] = NULL
	,@Operated_MenuId [bigint] = NULL
	,@Action VARCHAR(10) = NULL
	,@response nvarchar(MAX) out
AS
BEGIN

	SET NOCOUNT ON;

	SET @response = 'E|' + 'Opps!... Something went wrong. Please contact system administrator|0';
		
		  --ACTIVE/INACTIVE
		IF @Action = 'STATUS'
		  BEGIN
			
				UPDATE [dbo].[Department] SET IsActive = @IsActive, LastModifiedBy = @Operated_By, LastModifiedDate = GETDATE()
				WHERE [Id]= @Id;
				
				SET @response =  'S|' + 'Status changed successfully|' + CONVERT(NVARCHAR(MAX), @Id);
			
		  END
 
		  --DELETE
		IF @Action = 'DELETE'
		  BEGIN
			
				UPDATE [dbo].[Department] SET IsActive = 0, IsDeleted = 1, LastModifiedBy = @Operated_By, LastModifiedDate = GETDATE()
				WHERE [Id]= @Id;
 
				SET @response =  'S|' + 'Record deleted successfully|' + CONVERT(NVARCHAR(MAX), @Id);
			
		  END

END
GO
/****** Object:  StoredProcedure [dbo].[SP_Employee_GET]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_Employee_GET]
	@Id [bigint] = NULL
	,@Operated_By [bigint] = NULL
	,@Operated_RoleId [bigint] = NULL
	,@Operated_MenuId [bigint] = NULL
AS
BEGIN

	SET NOCOUNT ON;

	IF @Id > 0
		SELECT X.Id, X.UserId, X.RoleId, X.FirstName, X.LastName, X.MiddleName, Z.Username UserName, X.UserType, X.Address, X.CityId, X.StateId, X.CountryId, X.Gender, X.Position, X.ContactNo, X.BloodGroup
		, X.BirthDate, IIF(X.BirthDate IS NOT NULL, CONVERT(VARCHAR, X.BirthDate, 103), NULL)BirthDate_Text, X.HireDate, X.IsActive 
		FROM [dbo].[Employee] X 
		LEFT JOIN [dbo].[Users] Z ON X.UserId = Z.Id
		LEFT JOIN [dbo].[UserRoleMapping] ZX ON (X.UserId = ZX.UserId AND X.RoleId = ZX.RoleId)
		WHERE X.Id = @Id;
	BEGIN
        SELECT X.Id, X.UserId, X.RoleId, X.FirstName, X.LastName, X.MiddleName, Z.Username UserName, X.UserType, X.Address, X.CityId, X.StateId, X.CountryId, X.Gender, X.Position, X.ContactNo, X.BloodGroup
		, X.BirthDate, IIF(X.BirthDate IS NOT NULL, CONVERT(VARCHAR, X.BirthDate, 103), NULL)BirthDate_Text, X.HireDate, X.IsActive 
		FROM [dbo].[Employee] X 
		LEFT JOIN [dbo].[Users] Z ON X.UserId = Z.Id
		LEFT JOIN [dbo].[UserRoleMapping] ZX ON (X.UserId = ZX.UserId AND X.RoleId = ZX.RoleId)
	END

END
GO
/****** Object:  StoredProcedure [dbo].[SP_Employee_Save]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_Employee_Save]
	@Id [bigint] = NULL
	,@UserId [bigint] = NULL
	,@RoleId [bigint] = NULL
	,@UserName [nvarchar](MAX) = NULL
	,@Password [nvarchar](MAX) = NULL
	,@FirstName [nvarchar](MAX) = NULL
	,@MiddleName [nvarchar](MAX) = NULL
	,@LastName [nvarchar](MAX) = NULL
	,@UserType [nvarchar](MAX) = NULL
	,@BirthDate [nvarchar](MAX) = NULL
	,@IsActive [bit] = NULL
	,@Operated_By [bigint] = NULL
	,@Operated_RoleId [bigint] = NULL
	,@Operated_MenuId [bigint] = NULL
	,@Action VARCHAR(10) = NULL
	,@response nvarchar(MAX) out
AS
BEGIN

	SET NOCOUNT ON;

	SET @response = 'E|' + 'Opps!... Something went wrong. Please contact system administrator|0';
		
	DECLARE @User_Id bigint = 0;
	DECLARE @Employee_Id bigint = 0;

    IF EXISTS (SELECT * FROM [dbo].[Users] WHERE UPPER(TRIM([UserName])) = UPPER(TRIM(@UserName)) AND Id != @UserId)
		BEGIN SET @response =  'E|' + 'Username is already exists|0'; END
	ELSE IF EXISTS (SELECT * FROM [dbo].[Employee] WHERE UPPER(TRIM([FirstName])) = UPPER(TRIM(@FirstName)) 
						AND UPPER(TRIM([MiddleName])) = UPPER(TRIM(@MiddleName)) AND UPPER(TRIM([LastName])) = UPPER(TRIM(@LastName)) 
						AND UPPER(TRIM([UserType])) = UPPER(TRIM(@UserType)) AND Id = @Id 
					)
		BEGIN SET @response =  'E|' + 'Employee is already exists|0'; END
	ELSE
	BEGIN

		--INSERT
		IF @Action = 'INSERT'
		BEGIN
		
			INSERT INTO [dbo].[Users]([UserName], [Password], CreatedBy, CreatedDate, IsActive, IsDeleted)
			VALUES (@UserName, @Password, @Operated_By, GETDATE(), @IsActive, 0)
 
			SET @User_Id = SCOPE_IDENTITY();
			
			INSERT INTO [dbo].[UserRoleMapping]([UserId], [RoleId], CreatedBy, CreatedDate, IsActive, IsDeleted)
			VALUES (@UserId, @RoleId, @Operated_By, GETDATE(), @IsActive, 0)
 

			INSERT INTO [dbo].[Employee](UserId, RoleId, FirstName, LastName, MiddleName, UserType, BirthDate, CreatedBy, CreatedDate, IsActive, IsDeleted)
			VALUES (@UserId, @RoleId, @FirstName, @LastName, @MiddleName, @UserType, IIF(@BirthDate IS NOT NULL, CONVERT(DATETIME, @BirthDate, 103), NULL), @Operated_By, GETDATE(), @IsActive, 0)
 
			SET @Employee_Id = SCOPE_IDENTITY();

			SET @response =  'S|' + 'Record saved successfully|' + CONVERT(NVARCHAR(MAX), @Employee_Id);
		
		END
 
		  --UPDATE
		IF @Action = 'UPDATE'
		  BEGIN
			IF EXISTS (SELECT * FROM [dbo].[Users] WHERE UPPER(TRIM([UserName])) = UPPER(TRIM(@UserName)) AND [Id] != @Id )
				BEGIN SET @response =  'E|' + 'Username is already exists|0'; END
			ELSE
			BEGIN
				UPDATE [dbo].[Users] 
				SET [UserName] = @UserName
					, [Password] = IIF(@Password IS NOT NULL AND @Password != '', @Password, [Password])
					, IsActive = @IsActive, LastModifiedBy = @Operated_By, LastModifiedDate = GETDATE()
				WHERE [Id]= @Id;
 
				SET @response =  'S|' + 'Record saved successfully|' + CONVERT(NVARCHAR(MAX), @Id);
			END
		  END
 
		--  --ACTIVE/INACTIVE
		--IF @Action = 'STATUS'
		--  BEGIN
		--	IF EXISTS (SELECT * FROM [dbo].[Users] WHERE [Id] = @Id)
		--	BEGIN
		--		UPDATE [dbo].[Users] SET IsActive = @IsActive, LastModifiedBy = @Operated_By, LastModifiedDate = GETDATE()
		--		WHERE [Id]= @Id;
 
		--		SET @response =  'S|' + 'Status changed successfully|' + CONVERT(NVARCHAR(MAX), @Id);
		--	END
		--  END
 
		--  --DELETE
		--IF @Action = 'DELETE'
		--  BEGIN
		--	IF EXISTS (SELECT * FROM [dbo].[Users] WHERE [Id] = @Id)
		--	BEGIN
		--		UPDATE [dbo].[Users] SET IsActive = 0, IsDeleted = 1, LastModifiedBy = @Operated_By, LastModifiedDate = GETDATE()
		--		WHERE [Id]= @Id;
 
		--		SET @response =  'S|' + 'Record deleted successfully|' + CONVERT(NVARCHAR(MAX), @Id);
		--	END
		--  END

	END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Employee_Status]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_Employee_Status]
	@Id [bigint] = NULL
	,@IsActive [bit] = NULL
	,@Operated_By [bigint] = NULL
	,@Operated_RoleId [bigint] = NULL
	,@Operated_MenuId [bigint] = NULL
	,@Action VARCHAR(10) = NULL
	,@response nvarchar(MAX) out
AS
BEGIN

	SET NOCOUNT ON;

	SET @response = 'E|' + 'Opps!... Something went wrong. Please contact system administrator|0';
		
	DECLARE @User_Id bigint = 0;
	DECLARE @Employee_Id bigint = 0;

    IF NOT EXISTS (SELECT * FROM [dbo].[Employee] WHERE Id = @Id )
		BEGIN SET @response =  'E|' + 'Employee is not available|0'; END
	ELSE
	BEGIN

		  --ACTIVE/INACTIVE
		IF @Action = 'STATUS'
		  BEGIN
			
				UPDATE [dbo].[Employee] SET IsActive = @IsActive, LastModifiedBy = @Operated_By, LastModifiedDate = GETDATE()
				WHERE [Id]= @Id;
				
				UPDATE [dbo].[Users] SET IsActive = @IsActive, LastModifiedBy = @Operated_By, LastModifiedDate = GETDATE()
				WHERE [Id] IN (SELECT UserId FROM [dbo].[Employee] WHERE [Id] = @Id);
 
				SET @response =  'S|' + 'Status changed successfully|' + CONVERT(NVARCHAR(MAX), @Id);
			
		  END
 
		  --DELETE
		IF @Action = 'DELETE'
		  BEGIN
			
				UPDATE [dbo].[Employee] SET IsActive = 0, IsDeleted = 1, LastModifiedBy = @Operated_By, LastModifiedDate = GETDATE()
				WHERE [Id]= @Id;
 
				UPDATE [dbo].[Users] SET IsActive = 0, IsDeleted = 1, LastModifiedBy = @Operated_By, LastModifiedDate = GETDATE()
				WHERE [Id] IN (SELECT UserId FROM [dbo].[Employee] WHERE [Id] = @Id);
 
				SET @response =  'S|' + 'Record deleted successfully|' + CONVERT(NVARCHAR(MAX), @Id);
			
		  END

		
	END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_Lov_GET]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_Lov_GET]
	@Lov_Column nvarchar(max) = NULL
AS
BEGIN

	SET NOCOUNT ON;

	IF @Lov_Column IS NOT NULL AND @Lov_Column != ''

	SELECT DISTINCT Lov_Column, Lov_Code, Lov_Desc
	FROM [dbo].[LOV_Master] X 
	WHERE X.Lov_Column = @Lov_Column;

	ELSE

	SELECT DISTINCT Lov_Column, Lov_Code, Lov_Desc
	FROM [dbo].[LOV_Master];


END
GO
/****** Object:  StoredProcedure [dbo].[SP_User_Save]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_User_Save]
	@Id [bigint] = NULL
	,@UserName [nvarchar](MAX) = NULL
	,@Password [nvarchar](MAX) = NULL
	,@IsActive [bit] = NULL
	,@Operated_By [bigint] = NULL
	,@Operated_RoleId [bigint] = NULL
	,@Operated_MenuId [bigint] = NULL
	,@Action VARCHAR(10) = NULL
	,@response nvarchar(MAX) out
AS
BEGIN

	SET NOCOUNT ON;

	SET @response = 'E|' + 'Opps!... Something went wrong. Please contact system administrator|0';

	
		--INSERT
		IF @Action = 'INSERT'
		  BEGIN
			IF EXISTS (SELECT * FROM [dbo].[Users] WHERE UPPER(TRIM([UserName])) = UPPER(TRIM(@UserName)))
				BEGIN SET @response =  'E|' + 'Username is already exists|0'; END
			ELSE
			BEGIN
				INSERT INTO [dbo].[Users]([UserName], [Password], CreatedBy, CreatedDate, IsActive, IsDeleted)
				VALUES (@UserName, @Password, @Operated_By, GETDATE(), @IsActive, 0)
 
				SET @Id = SCOPE_IDENTITY();
				SET @response =  'S|' + 'Record saved successfully|' + CONVERT(NVARCHAR(MAX), @Id);
			END
		  END
 
		  --UPDATE
		IF @Action = 'UPDATE'
		  BEGIN
			IF EXISTS (SELECT * FROM [dbo].[Users] WHERE UPPER(TRIM([UserName])) = UPPER(TRIM(@UserName)) AND [Id] != @Id )
				BEGIN SET @response =  'E|' + 'Username is already exists|0'; END
			ELSE
			BEGIN
				UPDATE [dbo].[Users] SET [UserName] = @UserName, [Password] = @Password, IsActive = @IsActive, LastModifiedBy = @Operated_By, LastModifiedDate = GETDATE()
				WHERE [Id]= @Id;
 
				SET @response =  'S|' + 'Record saved successfully|' + CONVERT(NVARCHAR(MAX), @Id);
			END
		  END
 
		--  --ACTIVE/INACTIVE
		--IF @Action = 'STATUS'
		--  BEGIN
		--	IF EXISTS (SELECT * FROM [dbo].[Users] WHERE [Id] = @Id)
		--	BEGIN
		--		UPDATE [dbo].[Users] SET IsActive = @IsActive, LastModifiedBy = @Operated_By, LastModifiedDate = GETDATE()
		--		WHERE [Id]= @Id;
 
		--		SET @response =  'S|' + 'Status changed successfully|' + CONVERT(NVARCHAR(MAX), @Id);
		--	END
		--  END
 
		--  --DELETE
		--IF @Action = 'DELETE'
		--  BEGIN
		--	IF EXISTS (SELECT * FROM [dbo].[Users] WHERE [Id] = @Id)
		--	BEGIN
		--		UPDATE [dbo].[Users] SET IsActive = 0, IsDeleted = 1, LastModifiedBy = @Operated_By, LastModifiedDate = GETDATE()
		--		WHERE [Id]= @Id;
 
		--		SET @response =  'S|' + 'Record deleted successfully|' + CONVERT(NVARCHAR(MAX), @Id);
		--	END
		--  END
			
END
GO
/****** Object:  StoredProcedure [dbo].[SP_User_Status]    Script Date: 04-09-2024 03:52:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_User_Status]
	@Id [bigint] = NULL
	,@IsActive [bit] = NULL
	,@Operated_By [bigint] = NULL
	,@Operated_RoleId [bigint] = NULL
	,@Operated_MenuId [bigint] = NULL
	,@Action VARCHAR(10) = NULL
	,@response nvarchar(MAX) out
AS
BEGIN

	SET NOCOUNT ON;

	SET @response = 'E|' + 'Opps!... Something went wrong. Please contact system administrator|0';

	
    IF @Id > 0
		BEGIN

		  --ACTIVE/INACTIVE
		IF @Action = 'STATUS'
		  BEGIN
			IF EXISTS (SELECT * FROM [dbo].[Users] WHERE [Id] = @Id)
			BEGIN
				UPDATE [dbo].[Users] SET IsActive = @IsActive, LastModifiedBy = @Operated_By, LastModifiedDate = GETDATE()
				WHERE [Id]= @Id;
 
				SET @response =  'S|' + 'Status changed successfully|' + CONVERT(NVARCHAR(MAX), @Id);
			END
		  END
 
		  --DELETE
		IF @Action = 'DELETE'
		  BEGIN
			IF EXISTS (SELECT * FROM [dbo].[Users] WHERE [Id] = @Id)
			BEGIN
				UPDATE [dbo].[Users] SET IsActive = 0, IsDeleted = 1, LastModifiedBy = @Operated_By, LastModifiedDate = GETDATE()
				WHERE [Id]= @Id;
 
				SET @response =  'S|' + 'Record deleted successfully|' + CONVERT(NVARCHAR(MAX), @Id);
			END
		  END

		END
	
END
GO
