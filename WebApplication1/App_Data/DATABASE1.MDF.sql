﻿/*
Deployment script for C:\USERS\TEMP\DOCUMENTS\VISUAL STUDIO 2017\PROJECTS\WEBAPPLICATION1\WEBAPPLICATION1\APP_DATA\DATABASE1.MDF

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "C:\USERS\TEMP\DOCUMENTS\VISUAL STUDIO 2017\PROJECTS\WEBAPPLICATION1\WEBAPPLICATION1\APP_DATA\DATABASE1.MDF"
:setvar DefaultFilePrefix "C_\USERS\TEMP\DOCUMENTS\VISUAL STUDIO 2017\PROJECTS\WEBAPPLICATION1\WEBAPPLICATION1\APP_DATA\DATABASE1.MDF_"
:setvar DefaultDataPath "C:\Users\TEMP\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\"
:setvar DefaultLogPath "C:\Users\TEMP\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ANSI_NULLS ON,
                ANSI_PADDING ON,
                ANSI_WARNINGS ON,
                ARITHABORT ON,
                CONCAT_NULL_YIELDS_NULL ON,
                QUOTED_IDENTIFIER ON,
                ANSI_NULL_DEFAULT ON,
                CURSOR_DEFAULT LOCAL,
                RECOVERY FULL,
                AUTO_SHRINK OFF 
            WITH ROLLBACK IMMEDIATE;
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_CLOSE OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET DISABLE_BROKER 
            WITH ROLLBACK IMMEDIATE;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Creating [dbo].[Auction]...';


GO
CREATE TABLE [dbo].[Auction] (
    [GUID]          VARCHAR (256) NOT NULL,
    [Name]          VARCHAR (50)  NULL,
    [ImageLink]     VARCHAR (50)  NULL,
    [Duration]      INT           NULL,
    [StartingPrice] INT           NULL,
    [CurrentPrice]  INT           NULL,
    [DateCreated]   DATE          NULL,
    [DateOpened]    DATE          NULL,
    [DateClosed]    DATE          NULL,
    [Status]        INT           NULL,
    PRIMARY KEY CLUSTERED ([GUID] ASC)
);


GO
PRINT N'Creating [dbo].[Order]...';


GO
CREATE TABLE [dbo].[Order] (
    [Id]    INT          NOT NULL,
    [name]  NCHAR (100)  NULL,
    [price] DECIMAL (18) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[User]...';


GO
CREATE TABLE [dbo].[User] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [Name]      VARCHAR (30)  NULL,
    [Surname]   VARCHAR (30)  NULL,
    [Email]     VARCHAR (30)  NULL,
    [Password]  VARCHAR (256) NULL,
    [NumTokens] DECIMAL (18)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_User_Column] UNIQUE NONCLUSTERED ([Email] ASC)
);


GO
PRINT N'Update complete.';


GO