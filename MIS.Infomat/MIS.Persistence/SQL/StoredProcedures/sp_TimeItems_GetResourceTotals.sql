-- =============================================
-- Author:		<Vladimir Deryagin>
-- Create date: <2020-10-19>
-- =============================================
USE MIS
GO

CREATE PROCEDURE [dbo].[sp_TimeItems_GetResourceTotals]
	@beginDate DATETIME,
	@endDate DATETIME,
	@specialtyID INT = 0
AS
BEGIN
	SELECT
		 t.[rf_DocPRVDID] AS [ResourceID]
		,t.[Date] AS [Date]
		,COUNT(t.[DoctorTimeTableID]) AS [TimesCount]
		,COUNT(v.[DoctorVisitTableID]) AS [VisitsCount]
	FROM
		[dbo].[hlt_DoctorTimeTable] AS t LEFT OUTER JOIN
		[dbo].[hlt_DoctorVisitTable] AS v ON t.[DoctorTimeTableID] = v.[rf_DoctorTimeTableID] INNER JOIN
		[dbo].[hlt_DocPRVD] AS r ON t.[rf_DocPRVDID] = r.[DocPRVDID] AND r.[rf_PRVSID] > 0
	WHERE
		t.[Date] BETWEEN @beginDate AND @endDate
		AND t.[Begin_Time] > '19000101'
		AND t.[FlagAccess] BETWEEN 4 AND 7
		AND (@specialtyID = 0 OR r.[rf_PRVSID] = @specialtyID)
		AND r.[InTime] = 1
	GROUP BY
		 t.[rf_DocPRVDID]
		,t.[Date]
END
GO