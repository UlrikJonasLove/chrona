CREATE TABLE [auth].[User]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL,

	[Firstname] NVARCHAR(150) NOT NULL,
	[Lastname] NVARCHAR(150) NOT NULL,

	[OrganisationId] BIGINT NOT NULL,

	[Email] NVARCHAR(100) NOT NULL,
	[Password] NVARCHAR(300) NOT NULL,

	[IsActive] BIT NOT NULL CONSTRAINT [DF_auth_User_IsActive] DEFAULT (1),
	[CreatedAtUtc] DATETIME2(3) NOT NULL CONSTRAINT [DF_auth_User_CreatedAtUtc] DEFAULT (SYSUTCDATETIME()),
	[LastLoginAtUtc] DATETIME2(3) NULL,

	CONSTRAINT [PK_auth_User] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_auth_User_Organisation] FOREIGN KEY ([OrganisationId])
		REFERENCES [auth].[Organisation] ([Id])
);
GO

CREATE UNIQUE INDEX [UX_auth_User_OrganisationId_Email]
	ON [auth].[User] ([OrganisationId], [Email]);
GO

CREATE INDEX [IX_auth_User_OrganisationId]
	ON [auth].[User] ([OrganisationId]);
GO