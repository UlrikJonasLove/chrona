CREATE TABLE [auth].[Organisation]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL,
	[Name] NVARCHAR(200) NOT NULL,
	[Slug] NVARCHAR(100) NOT NULL,
	[IsActive] BIT NOT NULL CONSTRAINT [DF_auth_Organisation_IsActive] DEFAULT (1),
	[CreatedAtUtc] DATETIME2(3) NOT NULL CONSTRAINT [DF_auth_Organisation_CreatedAtUtc] DEFAULT (SYSUTCDATETIME()),

	CONSTRAINT [PK_auth_Organisation] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UQ_auth_Organisation_Slug] UNIQUE ([Slug])
);
GO

CREATE UNIQUE INDEX [UX_auth_Organisation_Slug]
	ON [auth].[Organisation] ([Slug]);
GO