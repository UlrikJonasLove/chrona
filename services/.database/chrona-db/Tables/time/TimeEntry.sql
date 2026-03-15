CREATE TABLE [time].[TimeEntry]
(
	Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TimeEntry PRIMARY KEY,

	TimesheetWeekId BIGINT NOT NULL,
	UserId BIGINT NOT NULL,
	ProjectId BIGINT NOT NULL,

	WorkDate DATE NOT NULL,
	Minutes INT NOT NULL,

	StartedAtUtc DATETIME2 NULL,
	StoppedAtUtc DATETIME2 NULL,

	Comment NVARCHAR(500) NULL,

	CreatedAtUtc DATETIME2 NOT NULL CONSTRAINT DF_TimeEntry_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
	UpdatedAtUtc DATETIME2 NOT NULL CONSTRAINT DF_TimeEntry_UpdatedAtUtc DEFAULT (SYSUTCDATETIME()),

	RowVersion ROWVERSION NOT NULL,

	CONSTRAINT FK_TimeEntry_TimesheetWeek
		FOREIGN KEY (TimesheetWeekId) REFERENCES [time].[TimesheetWeek] (Id),

	CONSTRAINT CK_TimeEntry_Minutes_Range CHECK (Minutes >= 0 AND Minutes <= 1440),

	CONSTRAINT CK_TimeEntry_Timer_Validity CHECK
	(
		(StartedAtUtc IS NULL AND StoppedAtUtc IS NULL)
		OR
		(StartedAtUtc IS NOT NULL AND StoppedAtUtc IS NULL)
		OR
		(StartedAtUtc IS NOT NULL AND StoppedAtUtc IS NOT NULL AND StoppedAtUtc >= StartedAtUtc)
	)
);
GO

CREATE INDEX IX_TimeEntry_User_WorkDate
ON [time].[TimeEntry] (UserId, WorkDate);
GO

CREATE INDEX IX_TimeEntry_TimesheetWeekId
ON [time].[TimeEntry] (TimesheetWeekId);
GO

CREATE INDEX IX_TimeEntry_User_StartedAtUtc
ON [time].[TimeEntry] (UserId, StartedAtUtc);
GO