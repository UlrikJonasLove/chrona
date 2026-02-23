CREATE TABLE [time].[TimesheetWeekStatusEvent]
(
	Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TimesheetWeekStatusEvent PRIMARY KEY,

	TimesheetWeekId BIGINT NOT NULL,
	FromStatus INT NULL,
	ToStatus INT NOT NULL,

	ChangedAtUtc DATETIME2 NOT NULL CONSTRAINT DF_TimesheetWeekStatusEvent_ChangedAtUtc DEFAULT (SYSUTCDATETIME()),
	ChangedByUserId BIGINT NOT NULL,

	Note NVARCHAR(500) NULL,

	CONSTRAINT FK_TimesheetWeekStatusEvent_TimesheetWeek
		FOREIGN KEY (TimesheetWeekId) REFERENCES [time].[TimesheetWeek] (Id)
);
GO

CREATE INDEX IX_TimesheetWeekStatusEvent_TimesheetWeekId_ChangedAtUtc
ON [time].[TimesheetWeekStatusEvent] (TimesheetWeekId, ChangedAtUtc DESC);
GO