IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'reminderapp_dev')
BEGIN
    CREATE DATABASE reminderapp_dev;
END
GO

USE reminderapp_dev;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[User] (
        [Id]               UNIQUEIDENTIFIER NOT NULL,
        [Username]         VARCHAR(100)     NOT NULL,
        [Email]            VARCHAR(150)     NOT NULL,
        [Password]         VARCHAR(255)     NOT NULL,
        [CreatedAt]        DATETIME         NOT NULL CONSTRAINT DF_User_CreatedAt DEFAULT GETDATE(),
        
        CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_User_Email] UNIQUE ([Email]),
        CONSTRAINT [UQ_User_Username] UNIQUE ([Username])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Reminder]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Reminder] (
        [Id]               UNIQUEIDENTIFIER NOT NULL,
        [UserId]           UNIQUEIDENTIFIER NOT NULL,
        [Title]            VARCHAR(150)     NOT NULL,
        [Description]      VARCHAR(1000)    NULL,     
        [TargetDateTime]   DATETIME         NOT NULL, 
        [Status]           INT              NOT NULL, 

        CONSTRAINT [PK_Reminder] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Reminder_User] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[User] ([Id]) 
            ON DELETE CASCADE,
        CONSTRAINT [CK_Reminder_Status] CHECK ([Status] BETWEEN 0 AND 3)
    );
    CREATE NONCLUSTERED INDEX [IX_Reminder_UserId] ON [dbo].[Reminder] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_Reminder_Status] ON [dbo].[Reminder] ([Status]);
END
GO
