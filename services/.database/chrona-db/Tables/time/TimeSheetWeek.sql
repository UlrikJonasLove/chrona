CREATE TABLE [time].[TimesheetWeek]
(
	Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TimesheetWeek PRIMARY KEY,

	UserId BIGINT NOT NULL,

	IsoYear INT NOT NULL,
	IsoWeek INT NOT NULL,

	Status INT NOT NULL,

	SubmittedAtUtc DATETIME2 NULL,
	SubmittedByUserId BIGINT NULL,

	ReviewedAtUtc DATETIME2 NULL,
	ReviewedByUserId BIGINT NULL,

	RejectionReason NVARCHAR(500) NULL,

	CreatedAtUtc DATETIME2 NOT NULL CONSTRAINT DF_TimesheetWeek_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
	UpdatedAtUtc DATETIME2 NOT NULL CONSTRAINT DF_TimesheetWeek_UpdatedAtUtc DEFAULT (SYSUTCDATETIME()),

	RowVersion ROWVERSION NOT NULL,

	CONSTRAINT UQ_TimesheetWeek_User_Year_Week UNIQUE (UserId, IsoYear, IsoWeek)
);
GO

CREATE INDEX IX_TimesheetWeek_UserId_Status
ON [time].[TimesheetWeek] (UserId, Status);
GO