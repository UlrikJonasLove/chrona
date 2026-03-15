CREATE TABLE [auth].[UserRefreshSession]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL,

	[UserId] BIGINT NOT NULL,
	[OrganisationId] BIGINT NOT NULL,

	[RefreshTokenHash] VARBINARY(64) NOT NULL,

	[CreatedAtUtc] DATETIME2(3) NOT NULL CONSTRAINT [DF_auth_UserRefreshSession_CreatedAtUtc] DEFAULT (SYSUTCDATETIME()),
	[ExpiresAtUtc] DATETIME2(3) NOT NULL,

	[RevokedAtUtc] DATETIME2(3) NULL,
	[ReplacedByUserRefreshSessionId] BIGINT NULL,

	[UserAgent] NVARCHAR(300) NULL,
	[IpAddress] NVARCHAR(45) NULL,

	CONSTRAINT [PK_auth_UserRefreshSession] PRIMARY KEY CLUSTERED ([Id] ASC),

	CONSTRAINT [FK_auth_UserRefreshSession_User] FOREIGN KEY ([UserId])
		REFERENCES [auth].[User] ([Id]),

	CONSTRAINT [FK_auth_UserRefreshSession_Organisation] FOREIGN KEY ([OrganisationId])
		REFERENCES [auth].[Organisation] ([Id]),

	CONSTRAINT [FK_auth_UserRefreshSession_ReplacedBy] FOREIGN KEY ([ReplacedByUserRefreshSessionId])
		REFERENCES [auth].[UserRefreshSession] ([Id])
);
GO

CREATE INDEX [IX_auth_UserRefreshSession_UserId]
	ON [auth].[UserRefreshSession] ([UserId]);
GO

CREATE INDEX [IX_auth_UserRefreshSession_OrganisationId]
	ON [auth].[UserRefreshSession] ([OrganisationId]);
GO

CREATE INDEX [IX_auth_UserRefreshSession_RefreshTokenHash]
	ON [auth].[UserRefreshSession] ([RefreshTokenHash]);
GO