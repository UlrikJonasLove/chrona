CREATE TABLE [auth].[UserRole]
(
	[UserId] BIGINT NOT NULL,
	[RoleId] BIGINT NOT NULL,
	[CreatedAtUtc] DATETIME2(3) NOT NULL CONSTRAINT [DF_auth_UserRole_CreatedAtUtc] DEFAULT (SYSUTCDATETIME()),

	CONSTRAINT [PK_auth_UserRole] PRIMARY KEY CLUSTERED ([UserId], [RoleId]),
	CONSTRAINT [FK_auth_UserRole_User] FOREIGN KEY ([UserId]) REFERENCES [auth].[User] ([Id]),
	CONSTRAINT [FK_auth_UserRole_Role] FOREIGN KEY ([RoleId]) REFERENCES [auth].[Role] ([Id])
);