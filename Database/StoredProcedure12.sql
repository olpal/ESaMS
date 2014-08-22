/****** Object:  StoredProcedure [dbo].[_sp_Update_EventsListStatus]    Script Date: 08/17/2014 11:32:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <December 28th 2012>
-- Description:	<This stored procedure is used to insert an event into the the EventSchedule table>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventSchedule]
	-- Add the parameters for the stored procedure here
	@Schedule varchar(255)

AS
BEGIN
	SET NOCOUNT ON;
	--Start of transaction
	BEGIN TRANSACTION 
	BEGIN TRY	
		--Insert data into 
		INSERT INTO dbo.Event_Schedule VALUES (@Schedule)
	COMMIT 
	END TRY
	BEGIN CATCH
		ROLLBACK 
	END CATCH
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <December 28th 2012>
-- Description:	<This stored procedure is used to insert an event into the the timing table>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventAudit]
	-- Add the parameters for the stored procedure here	
	@DateCreated datetime,
	@UserName varchar(255),
	@EID int,
	@Info varchar(510),
	@Name varchar(512),
	@Category varchar(512)
AS
BEGIN
	SET NOCOUNT ON;
	--Insert data into 
	INSERT INTO dbo.Event_Audit VALUES (@DateCreated, @UserName, @EID, @Info, @Name, @Category)
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <December 28th 2012>
-- Description:	<This stored procedure is used to insert an event into the the eventlist audit table>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventListAudit]
	-- Add the parameters for the stored procedure here
	@StartTime datetime,
	@EndTime datetime,	
	@UserName varchar(255),
	@EID int,
	@Active bit,
	@ScheduledTime datetime,
	@Forward bit
	
AS
BEGIN
	SET NOCOUNT ON;
	--Insert data into 
	INSERT INTO dbo.EventList_Audit VALUES (@StartTime, @EndTime, @UserName, @EID, @Active, @ScheduledTime, @Forward)
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <December 28th 2012>
-- Description:	<This stored procedure is used to insert an event into the the events table>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_Events]
	-- Add the parameters for the stored procedure here
	@EventCat varchar(55),
	@Sched varchar(55),
	@Name varchar(255),
	@FinishTimeRequired bit,
	@URL varchar(255),
	@Active bit,
	@StartTime datetime,
	@ScheduleCode varchar(10),
	@Doc varchar(255),
	@Script varchar(255),
	@UserName varchar (255),
	@info varchar (510)
AS
BEGIN
	SET NOCOUNT ON;
	--Start of transaction
	BEGIN TRANSACTION 
	BEGIN TRY	
		--Integer to represent ECID
		DECLARE @ECID int
		--Integer to represent SID
		DECLARE @SID int
		--Inteter to represent current row
		DECLARE @ETID int

		--Set the category ID
		SET @ECID = (SELECT ECID FROM dbo.Event_Category WHERE CategoryName=@EventCat)
		--Set the schedule ID
		SET @SID = (SELECT SID FROM dbo.Event_Schedule WHERE Schedule=@Sched)

		--Insert data into 
		INSERT INTO dbo.Event_Timing VALUES (@SID, @StartTime, @ScheduleCode)
		
		--Set the variable to the last row added
		SET @ETID = (SELECT TOP 1 ETID FROM dbo.Event_Timing ORDER BY ETID DESC)

		--Insert into the event into the events table
		INSERT INTO dbo.Events VALUES (@ECID, @ETID, @SID, @Name, @FinishTimeRequired, @URL, @Active, @Doc, @Script)
		--Inteter to represent current row and current date
		DECLARE @EID int, @CurrentDate datetime
		--Set the variable to the last row added
		SET @EID = (SELECT TOP 1 EID FROM dbo.Events ORDER BY EID DESC)
		--Get the current date
		SET @CurrentDate = (SELECT GETDATE())
		--Insert a record into the event audit table
		EXEC [dbo].[_sp_Insert_EventAudit] @CurrentDate,@UserName,@EID,@info,@Name,@EventCat
		COMMIT
		RETURN @EID 
	END TRY
	BEGIN CATCH
		ROLLBACK 
		SELECT ERROR_MESSAGE()
	END CATCH
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <December 28th 2012>
-- Description:	<This stored procedure is used to insert an event into the the timing table>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventPerformance]
	-- Add the parameters for the stored procedure here
	@EID int,	
	@StartTime datetime,
	@EndTime datetime,
	@SchedTime datetime

AS
BEGIN
	SET NOCOUNT ON;
	--Insert data into 
	INSERT INTO dbo.Event_Performance VALUES (@EID, @SchedTime, @StartTime, @EndTime)
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <December 28th 2012>
-- Description:	<This stored procedure is used to insert an event into the the eventlist table>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventList]
	-- Add the parameters for the stored procedure here
	@Complete bit,
	@EndTime datetime,
	@EID int,
	@Enabled bit,
	@BeginTime datetime,
	@StartTime datetime,
	@Forward bit,
	@StatusCode int,
	@StatusInfo varchar,
	@UserName varchar
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO dbo.Event_List VALUES (@Complete,@EndTime,@EID,@Enabled,@BeginTime,@StartTime,@Forward,@StatusCode,@StatusInfo,@UserName)
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <January 01 2013>
-- Description:	<This stored procedure is used to insert weekly events into the event list table>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventList_Weekly]
	-- Add the parameters for the stored procedure here
	@StartTimeIn datetime,
	@EndTimeIn datetime,
	@Report int

AS
BEGIN
	--Temporary Table to hold active events
	DECLARE TempTable CURSOR FOR 
	SELECT et.StartTime, et.ScheduleCode,e.EID, e.Active 
	FROM dbo.Events e JOIN dbo.Event_Timing et 
	ON e.ETID=et.ETID 
	WHERE e.Active='True' AND e.SID=2

	--Declare Variables to hold returned information
	DECLARE @StartTime DateTime, @StartTimeLoop DateTime, @ScheduleCode varchar(25), @EID int, @Active bit;

	--Open the temp table
	OPEN TempTable

	--Fetch the first row
	FETCH NEXT FROM TempTable
	INTO @StartTime, @ScheduleCode, @EID,@Active

	--While there are rows
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--ASsign start time in to starttimeloop this ensures that the original starttimein is not modified and can be used for all loops
		SET @StartTimeLoop = @StartTimeIn
		--While starttimeloop is less than endtimein
		WHILE @StartTimeLoop <= @EndTimeIn		
		BEGIN
			--If the Char Index contains the appropriate day and the event occurs before end time
			IF (CHARINDEX (CONVERT (varchar, DATEPART (dw, @StartTimeLoop)), @ScheduleCode) != 0)
			BEGIN
				--Construct the start time using todays date and the time from the event
				SET @StartTime = CONVERT(datetime, CAST(DATEPART(yy,@StartTimeLoop)AS varchar) + '-' +  CAST(DATEPART(mm,@StartTimeLoop)AS varchar)
				 + '-' +  CAST(DATEPART(dd,@StartTimeLoop)AS varchar) +
				 ' ' + CAST(DATEPART(hh,@StartTime) AS varchar) + ':' + CAST(DATEPART(mi,@StartTime) AS varchar) +  ':00', 120)
			
			END			
			--If the Char Index contains the appropriate day and the event occurs before end time
			IF (@StartTime >= @StartTimeLoop AND (@StartTime < @EndTimeIn))
			BEGIN
				--If this is a report request
				IF @Report = 0
					BEGIN
					--Insert into the Report List		
					INSERT INTO dbo.Report_List VALUES (@EID,@StartTime)
				END
				IF @Report = 1
				BEGIN	
					--Insert into the Event List		
					INSERT INTO dbo.Event_List VALUES ('False',null,@EID,@Active,@StartTime,null,'False',0,null,null)
				END
				IF @Report = 2
					BEGIN	
					--Insert into the Event List		
					INSERT INTO dbo.Event_ListBackup VALUES (@StartTime,@EID)
				END
			END
			--Add 1 day to the start time loop to check the next day
			SET @StartTimeLoop = DATEADD (dd, 1, @StartTimeLoop)
			--Set the hour and minute to 00:00 indicating the start of the day
			SET @StartTimeLoop =CONVERT(datetime, CAST(DATEPART(yy,@StartTimeLoop)AS varchar) + '-' +  CAST(DATEPART(mm,@StartTimeLoop)AS varchar)
				 + '-' +  CAST(DATEPART(dd,@StartTimeLoop)AS varchar) + ' 00:00:00', 120)
		END
		--Fetch next row
		FETCH NEXT FROM TempTable
		INTO @StartTime, @ScheduleCode, @EID,@Active
	END

	--Close the temp table
	CLOSE TempTable

	--Deallocate the temp table
	DEALLOCATE TempTable

END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <September 23 2013>
-- Description:	<This stored procedure is used to insert  frequent reoccuring events into the event list table>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventList_Reoccur]
	-- Add the parameters for the stored procedure here
	@StartTimeIn datetime,
	@EndTimeIn datetime,
	@Report int

AS
BEGIN

	--Temporary Table to hold active events
	DECLARE TempTable CURSOR FOR 
	SELECT et.StartTime, et.ScheduleCode,e.EID,e.Active 
	FROM dbo.Events e JOIN dbo.Event_Timing et 
	ON e.ETID=et.ETID 
	WHERE e.Active='True' AND e.SID=7
	
	--Declare Variables to hold returned information
	DECLARE @StartTime DateTime, @StartTimeLoop DateTime, @EndTimeLoop DateTime, @ScheduleCode varchar(25), @EID int, @Active bit;
	
	--Set Startimeloop to startimein
	SET @StartTimeLoop = @StartTimeIn
	--SET Endtime loop to the last possible second of the endday
	SET @EndTimeLoop = CONVERT(varchar, CAST((DATEPART(yy, @EndTimeIn)) AS varchar) + '-' + 
												 CAST((DATEPART(mm, @EndTimeIn)) AS varchar) + '-' + 
												 CAST((DATEPART(dd, @EndTimeIn)) AS varchar) + ' 23:59:59',120)
	--While starttimeloop is less than endtimein
	WHILE @StartTimeLoop <= @EndTimeLoop
	BEGIN
		--Open the temp table
		OPEN TempTable
		--Fetch the first row
		FETCH NEXT FROM TempTable
		INTO @StartTime, @ScheduleCode, @EID, @Active
		--While there are rows
		WHILE @@FETCH_STATUS = 0
		BEGIN
			--Declare variables
			DECLARE @DayWeek int, @RecDW int, @Inst int, @RecInst int, @CurrMon int, @CurrDate datetime, @SchDate datetime, @Last int;
			--Get current day of the week
			SET @DayWeek = DatePart(dw,@StartTimeLoop)
			--Get records day of week represented by the second number in schedulecode
			SET @RecDW =  CAST(SUBSTRING(@ScheduleCode,2,1) AS INT)
			--If the day of the week is today	
			IF @DayWeek = @RecDW
			BEGIN 	
				--SET inst to 0
				SET @Inst = 0
				--Get the current date
				SET @CurrDate = @StartTimeLoop
				--Current month
				SET @CurrMon = DatePart(mm,@CurrDate)
				--If it is the first occurance
				IF (DatePart(mm,(DATEADD(dd,-7,@CurrDate)))) = (DatePart(mm,(DATEADD(mm,-1,@CurrDate))))
				BEGIN
					--Set the occurance code to 1
					SET @Inst = 1
					SET @Last = 1
				END
				--If it is the second occurance
				ELSE IF (DatePart(mm,(DATEADD(dd,-14,@CurrDate)))) = (DatePart(mm,(DATEADD(mm,-1,@CurrDate))))
				BEGIN
					--Set the occurance code to 2
					SET @Inst = 2
					SET @Last = 2
				END
				--If it is the second occurance
				ELSE IF (DatePart(mm,(DATEADD(dd,-21,@CurrDate)))) = (DatePart(mm,(DATEADD(mm,-1,@CurrDate))))
				BEGIN
					--Set the occurance code to 3
					SET @Inst = 3
					SET @Last = 3
				END
				--If it is the second occurance
				ELSE IF (DatePart(mm,(DATEADD(dd,-28,@CurrDate)))) = (DatePart(mm,(DATEADD(mm,-1,@CurrDate))))
				BEGIN
					--Set the occurance code to 4
					SET @Inst = 4
					SET @Last = 4
				END
				--If it is the last occurance
				IF (DatePart(mm,(DATEADD(dd,7,@CurrDate)))) = (DatePart(mm,(DATEADD(mm,1,@CurrDate))))
				BEGIN
					--IF insta is 0
					IF (@Inst = 0)
					BEGIN
						--Sett insta to last
						SET @Inst = 5
					END
					--Set the occurance code to 5
					SET @Last = 5
				END
				--Get the frequency of occurance represented as the first number in schedulecode
				SET @RecInst =  CAST(SUBSTRING(@ScheduleCode,1,1) AS INT)
				--Construct datetime from todays date and the scheduled time
				SET @SchDate = CONVERT(varchar, CAST((DATEPART(yy, @CurrDate)) AS varchar) + '-' + 
												 CAST((DATEPART(mm, @CurrDate)) AS varchar) + '-' + 
												 CAST((DATEPART(dd, @CurrDate)) AS varchar) + ' ' + 
												 CAST((DATEPART(hh, @StartTime)) AS varchar) + ':' + 
												 CAST((DATEPART(mi, @StartTime)) AS varchar) + ':00',120)

				--If the event occurs before end time
				IF (@RecInst BETWEEN @Inst AND @Last) AND (@SchDate BETWEEN @StartTimeIn AND @EndTimeIn)
				BEGIN
					--If this is a report request
					IF @Report = 0
						BEGIN
						--Insert into the Report List		
						INSERT INTO dbo.Report_List VALUES (@EID,@SchDate)
					END
					IF @Report = 1
						BEGIN	
						--Insert into the Event List		
						INSERT INTO dbo.Event_List VALUES ('False',null,@EID,@Active,@SchDate,null,'False',0,null,null)
					END
					IF @Report = 2
					BEGIN	
						--Insert into the Event List		
						INSERT INTO dbo.Event_ListBackup VALUES (@SchDate,@EID)
					END
				END
			END	
			--Fetch next row
			FETCH NEXT FROM TempTable
			INTO @StartTime, @ScheduleCode, @EID, @Active
		END
		--Increase the day by one
		SET @StartTimeLoop = DateADD(dd,1,@StartTimeLoop)
		--Close the temp table
		CLOSE TempTable
	END
	--Deallocate the temp table
	DEALLOCATE TempTable
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <January 01 2013>
-- Description:	<This stored procedure is used to insert quarterly events into the event list table>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventList_Quarterly]
	-- Add the parameters for the stored procedure here
	@StartTimeIn datetime,
	@EndTimeIn datetime,
	@Report int

AS
BEGIN
	--Temporary Table to hold active events
	DECLARE TempTable CURSOR FOR 
	SELECT et.StartTime, et.ScheduleCode,e.EID, e.Active 
	FROM dbo.Events e JOIN dbo.Event_Timing et 
	ON e.ETID=et.ETID 
	WHERE e.Active='True' AND e.SID=5

	--Declare Variables to hold returned information
	DECLARE @StartTime DateTime, @StartTimeLoop DateTime, @ScheduleCode varchar(25), @EID int, @Active bit;

	--Open the temp table
	OPEN TempTable

	--Fetch the first row
	FETCH NEXT FROM TempTable
	INTO @StartTime, @ScheduleCode, @EID,@Active

	--While there are rows
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--ASsign start time in to starttimeloop this ensures that the original starttimein is not modified and can be used for all loops
		SET @StartTimeLoop = @StartTimeIn
		--While starttimeloop is less than endtimein
		WHILE @StartTimeLoop <= @EndTimeIn
		BEGIN
			--Construct the start time using todays year and the month, day, and time from the events time
			SET @StartTime = CONVERT(datetime, CAST(DATEPART(yy,@StartTimeLoop)AS varchar) + '-' +  CAST(DATEPART(mm,@StartTime)AS varchar)
				 + '-' +  CAST(DATEPART(dd,@StartTime)AS varchar) +
				 ' ' + CAST(DATEPART(hh,@StartTime) AS varchar) + ':' + CAST(DATEPART(mi,@StartTime) AS varchar) +  ':00', 120)
			--If the event occurs before end time
			IF (@StartTime BETWEEN @StartTimeIn AND @EndTimeIn)
			BEGIN
				--If this is a report request
				IF @Report = 0
					BEGIN
					--Insert into the Report List		
					INSERT INTO dbo.Report_List VALUES (@EID,@StartTime)
				END
				IF @Report = 1
				BEGIN	
					--Insert into the Event List		
					INSERT INTO dbo.Event_List VALUES ('False',null,@EID,@Active,@StartTime,null,'False',0,null,null)
				END
				IF @Report = 2
				BEGIN	
					--Insert into the Event List		
					INSERT INTO dbo.Event_ListBackup VALUES (@StartTime,@EID)
				END
			END
			--Add a day to the start time loop
			SET @StartTimeLoop = DATEADD (mm, 3, @StartTimeLoop)
			--Set the hour and minute to 00:00 indicating the start of the day
			SET @StartTimeLoop = CONVERT(datetime, CAST(DATEPART(yy,@StartTimeLoop)AS varchar) + '-' +  CAST(DATEPART(mm,@StartTimeLoop)AS varchar)
				 + '-' +  CAST(DATEPART(dd,@StartTimeLoop)AS varchar) + ' 00:00:00', 120)
		END
		--Fetch next row
		FETCH NEXT FROM TempTable
		INTO @StartTime, @ScheduleCode, @EID, @Active
	END

	--Close the temp table
	CLOSE TempTable

	--Deallocate the temp table
	DEALLOCATE TempTable

END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <August 25th 2013>
-- Description:	<This stored procedure is used call all relevant stored procedures related to inserting an event into the current event list and event table>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventList_Onetime_Live]
	-- Add the parameters for the stored procedure here
	@EventCat varchar(55),
	@Name varchar(255),
	@FinishTimeRequired bit,
	@URL varchar(255),
	@UserName varchar (255),
	@StartTime datetime

AS
BEGIN  
	BEGIN TRANSACTION
	BEGIN TRY
		DECLARE @EID int
		--Add the event to the event table
		EXEC @EID = [dbo].[_sp_Insert_Events] @EventCat, 'One-Time' ,@Name, @FinishTimeRequired, @URL, 'True', @StartTime,'' ,'','',@UserName,'Event Created' 
		--Update the EventListStatus table dayshift
		EXEC [dbo].[_sp_Insert_EventList] 'False', null, @EID, 'True', @StartTime, null, 'False', 0, null, null
		COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK 
		SELECT ERROR_MESSAGE()
	END CATCH
END 

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <January 01 2013>
-- Description:	<This stored procedure is used to insert one time events into the event list table>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventList_Onetime]
	-- Add the parameters for the stored procedure here
	@StartTimeIn datetime,
	@EndTimeIn datetime,
	@Report int

AS
BEGIN
	--Temporary Table to hold active events
	DECLARE TempTable CURSOR FOR 
	SELECT et.StartTime, et.ScheduleCode,e.EID ,e.Active
	FROM dbo.Events e JOIN dbo.Event_Timing et 
	ON e.ETID=et.ETID 
	WHERE e.Active='True' AND e.SID=6

	--Declare Variables to hold returned information
	DECLARE @StartTime DateTime, @ScheduleCode varchar(25), @EID int, @Active int;

	--Open the temp table
	OPEN TempTable

	--Fetch the first row
	FETCH NEXT FROM TempTable
	INTO @StartTime, @ScheduleCode, @EID, @Active

	--While there are rows
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--If the Char Index contains the appropriate day and the event occurs before end time
		IF (@StartTime >= @StartTimeIn) and (@StartTime < @EndTimeIn)
			BEGIN
				--If this is a report request
				--If this is a report request
				IF @Report = 0
					BEGIN
					--Insert into the Report List		
					INSERT INTO dbo.Report_List VALUES (@EID,@StartTime)
				END
				IF @Report = 1
				BEGIN	
					--Insert into the Event List		
					INSERT INTO dbo.Event_List VALUES ('False',null,@EID,@Active,@StartTime,null,'False',0,null,null)
				END
				IF @Report = 2
				BEGIN	
					--Insert into the Event List		
					INSERT INTO dbo.Event_ListBackup VALUES (@StartTime,@EID)
				END
			END
		--Fetch next row
		FETCH NEXT FROM TempTable
		INTO @StartTime, @ScheduleCode, @EID,@Active
	END

	--Close the temp table
	CLOSE TempTable

	--Deallocate the temp table
	DEALLOCATE TempTable

END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <January 01 2013>
-- Description:	<This stored procedure is used to insert Monthly events into the event list table>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventList_Monthly]
	-- Add the parameters for the stored procedure here
	@StartTimeIn datetime,
	@EndTimeIn datetime,
	@Report int

AS
BEGIN
	--Declare Variables to hold returned information
	DECLARE @StartTime DateTime, @ScheduleCode varchar(25), @EID int, @End datetime, 
	@CompTime dateTime,@CompTimeS dateTime,@CompTimeE dateTime,@TempEndTime datetime,@StartTimeLoop DateTime,@Active bit;
	DECLARE @DayTable TABLE (DayNum int)
	
	--Set end to endtime in	
	SET @End = @EndTimeIn
	--Set TempEnd time to 1 day + the endtime
	SET @TempEndTime = DATEADD(dd,1,@EndTimeIn)

	--Temporary Table to hold active events
	DECLARE TempTable CURSOR FOR 
	SELECT et.StartTime, et.ScheduleCode,e.EID,e.Active 
	FROM dbo.Events e JOIN dbo.Event_Timing et 
	ON e.ETID=et.ETID 
	WHERE e.Active='True' AND e.SID=4
	
	--while end is greater than or equal to startin add all days inbetween to the daytable
	WHILE @End >= @StartTimeIn
	BEGIN
		--Insert the day into the day table
		INSERT INTO @DayTable VALUES (DATEPART(dd,@End))
		--Subtract a day
		SET @END = DATEADD(dd,-1,@END)
	END

	--If it is the last day of the month
	IF DATEPART(mm,@StartTimeIn) != DATEPART(mm,(@TempEndTime))
	BEGIN
		--Insert 0 into the day table
		INSERT INTO @DayTable VALUES (0)
	END

	--Open the temp table
	OPEN TempTable

	--Fetch the first row
	FETCH NEXT FROM TempTable
	INTO @StartTime, @ScheduleCode, @EID, @Active
	
	--While there are rows
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--ASsign start time in to starttimeloop this ensures that the original starttimein is not modified and can be used for all loops
		SET @StartTimeLoop = @StartTimeIn
		--While starttimeloop is less than endtimein
		WHILE @StartTimeLoop <= @EndTimeIn
			BEGIN
			--Variable to hold checked schedule code
			DECLARE @ScheduleCodeMod varchar(25)
			--If schedule code is 0 indicating last day of month set the schedule code to the last day of the month
			IF @ScheduleCode = 0
			BEGIN
				DECLARE @TempTime datetime
				--Set the temptime to current year and month from @StartTimeLoop
				SET @TempTime = CONVERT(varchar,DATEPART(yyyy,@StartTimeLoop)) + '-' + CONVERT(varchar,DATEPART(mm,@StartTimeLoop)) + '-01 00:00:00'
				--Add one month to temptime
				SET @TempTime = DATEADD(mm,1,@TempTime)
				--SUbtract 1 day giving the last day of the start month
				SET @TempTime = DATEADD(dd,-1,@TempTime)
				--Set schedule code to the last day of the month
				SET @ScheduleCodeMod = DATEPART(dd,@TempTime)
			END
			ELSE
			BEGIN
				--Set the schedule codemod to schedule code
				SET @ScheduleCodeMod = @ScheduleCode
			END
			--IF the schedule code equals the start date day and is not eqaul to the end date day
			--Starttime will be the current date but end time will be 11:59 of the start day
			IF @ScheduleCodeMod = DATEPART(dd,@StartTimeLoop) AND @ScheduleCodeMod != DATEPART(dd,@EndTimeIn)
			BEGIN
				--Set start datetime to passed in
				SET @CompTimeS = @StartTimeLoop
				--Set the end time to the end of the current day
				SET @CompTimeE = CONVERT(varchar,DATEPART(yyyy,@StartTimeLoop)) + '-' + CONVERT(varchar,DATEPART(mm,@StartTimeLoop)) + '-' 
					+ CONVERT(varchar,DATEPART(dd,@StartTimeLoop)) + ' 23:59:59'
				--Set the time of the event to the start date and the event time
				SET @CompTime = CONVERT(varchar,DATEPART(yyyy,@StartTimeLoop)) + '-' + CONVERT(varchar,DATEPART(mm,@StartTimeLoop)) + '-' 
					+ CONVERT(varchar,@ScheduleCodeMod) + ' ' + CONVERT(varchar,DATEPART(hh,@StartTime)) + ':' + CONVERT(varchar,DATEPART(mi,@StartTime)) + ':00'	 
			END
			--IF the schedule code equals the end date day and is not eqaul to the start date day
			--end time will be the passed in end time however the start time will be 12am of the end day
			ELSE IF @ScheduleCodeMod = DATEPART(dd,@EndTimeIn) AND @ScheduleCodeMod != DATEPART(dd,@StartTimeLoop)
			BEGIN
				--Set the start time to the start of the current day
				SET @CompTimeS =  CONVERT(varchar,DATEPART(yyyy,@EndTimeIn)) + '-' + CONVERT(varchar,DATEPART(mm,@EndTimeIn)) + '-' 
								+ CONVERT(varchar,DATEPART(dd,@EndTimeIn)) + ' 00:00:00'
				--Set end datetime to passed in
				SET @CompTimeE = @EndTimeIn
				--Set the time of the event to the end date and the event time
				SET @CompTime = CONVERT(varchar,DATEPART(yyyy,@EndTimeIn)) + '-' + CONVERT(varchar,DATEPART(mm,@EndTimeIn)) + '-' 
							+ CONVERT(varchar,@ScheduleCodeMod) + ' ' + CONVERT(varchar,DATEPART(hh,@StartTime)) + ':' + CONVERT(varchar,DATEPART(mi,@StartTime)) + ':00'	 
			END
			ELSE
			BEGIN
				--Set start datetime to passed in
				SET @CompTimeS = @StartTimeLoop
				--Set end datetime to passed in
				SET @CompTimeE = @EndTimeIn
				--Set the time of the event to the start date and the event time
				SET @CompTime = CONVERT(varchar,DATEPART(yyyy,@StartTimeLoop)) + '-' + CONVERT(varchar,DATEPART(mm,@StartTimeLoop)) + '-' 
						+ CONVERT(varchar,@ScheduleCodeMod) + ' ' + CONVERT(varchar,DATEPART(hh,@StartTime)) + ':' + CONVERT(varchar,DATEPART(mi,@StartTime)) + ':00'	 
			END
			--If the start time of the event is between begining and end
			IF @CompTime BETWEEN @CompTimeS and @CompTimeE
			BEGIN	
				--IF the event schedule code is in the DayTable
				IF @ScheduleCodeMod IN (SELECT DayNum FROM @DayTable)
				BEGIN
					--If this is a report request
					IF @Report = 0
					BEGIN
						--Insert into the Report List		
						INSERT INTO dbo.Report_List VALUES (@EID,@CompTime)
					END	
					IF @Report = 1
					BEGIN	
						--Insert into the Event List		
						INSERT INTO dbo.Event_List VALUES ('False',null,@EID,@Active,@CompTime,null,'False',0,null,null)
					END
					IF @Report = 2
					BEGIN	
						--Insert into the Event List		
						INSERT INTO dbo.Event_ListBackup VALUES (@CompTime,@EID)
					END
				END
			END
		--Add a day to the start time loop
		SET @StartTimeLoop = DATEADD (mm, 1, @StartTimeLoop)
		END
		--Fetch next row
		FETCH NEXT FROM TempTable
		INTO @StartTime, @ScheduleCode, @EID, @Active
	END

	--Close the temp table
	CLOSE TempTable

	--Deallocate the temp table
	DEALLOCATE TempTable
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <December 28th 2012>
-- Description:	<This stored procedure is used to insert an event into the the eventlist table>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventList_Daily]
	-- Add the parameters for the stored procedure here
	@StartTimeIn datetime,
	@EndTimeIn datetime,
	@Report int


AS
BEGIN
	--Temporary Table to hold active events
	DECLARE TempTable CURSOR FOR 
	SELECT et.StartTime, et.ScheduleCode,e.EID,e.Active
	FROM dbo.Events e JOIN dbo.Event_Timing et 
	ON e.ETID=et.ETID 
	WHERE e.Active='True' AND e.SID=1

	--Declare Variables to hold returned information
	DECLARE @StartTime DateTime, @StartTimeLoop DateTime, @ScheduleCode varchar(25), @EID int,@Active int;

	--Open the temp table
	OPEN TempTable

	--Fetch the first row
	FETCH NEXT FROM TempTable
	INTO @StartTime, @ScheduleCode, @EID, @Active
	
	--While there are rows
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--ASsign start time in to starttimeloop this ensures that the original starttimein is not modified and can be used for all loops
		SET @StartTimeLoop = @StartTimeIn
		--While starttimeloop is less than endtimein
		WHILE @StartTimeLoop <= @EndTimeIn
		BEGIN
			--If the Char Index contains the appropriate day and the event occurs before end time
			IF (CHARINDEX (CONVERT (varchar, DATEPART (dw, @StartTimeLoop)), @ScheduleCode) != 0)
			BEGIN
				--Construct the start time using todays date and the time from the event
				SET @StartTime = CONVERT(datetime, CAST(DATEPART(yy,@StartTimeLoop)AS varchar) + '-' +  CAST(DATEPART(mm,@StartTimeLoop)AS varchar)
				 + '-' +  CAST(DATEPART(dd,@StartTimeLoop)AS varchar) +
				 ' ' + CAST(DATEPART(hh,@StartTime) AS varchar) + ':' + CAST(DATEPART(mi,@StartTime) AS varchar) +  ':00', 120)
			
			END
			--If the Char Index contains the appropriate day and the event occurs before end time
			IF (@StartTime >= @StartTimeLoop AND (@StartTime < @EndTimeIn))
			BEGIN
				--If this is a report request
				IF @Report = 0
					BEGIN
					--Insert into the Report List		
					INSERT INTO dbo.Report_List VALUES (@EID,@StartTime)
				END
				IF @Report = 1
				BEGIN	
					--Insert into the Event List		
					INSERT INTO dbo.Event_List VALUES ('False',null,@EID,@Active,@StartTime,null,'False',0,null,null)
				END
				IF @Report = 2
					BEGIN	
					--Insert into the Event List		
					INSERT INTO dbo.Event_ListBackup VALUES (@StartTime,@EID)
				END
			END
			--Add a day to the start time loop
			SET @StartTimeLoop = DATEADD (dd, 1, @StartTimeLoop)
			--Set the hour and minute to 00:00 indicating the start of the day
			SET @StartTimeLoop =CONVERT(datetime, CAST(DATEPART(yy,@StartTimeLoop)AS varchar) + '-' +  CAST(DATEPART(mm,@StartTimeLoop)AS varchar)
				 + '-' +  CAST(DATEPART(dd,@StartTimeLoop)AS varchar) + ' 00:00:00', 120)
		END
		--Fetch next row
		FETCH NEXT FROM TempTable
		INTO @StartTime, @ScheduleCode, @EID,@Active
	END

	--Close the temp table
	CLOSE TempTable

	--Deallocate the temp table
	DEALLOCATE TempTable

END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <January 01 2013>
-- Description:	<This stored procedure is used to insert quarterly events into the event list table>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventList_BiWeekly]
	-- Add the parameters for the stored procedure here
	@StartTimeIn datetime,
	@EndTimeIn datetime,
	@Report int

AS
BEGIN
	--Temporary Table to hold active events
	DECLARE TempTable CURSOR FOR 
	SELECT et.StartTime, et.ScheduleCode,e.EID, e.Active 
	FROM dbo.Events e JOIN dbo.Event_Timing et 
	ON e.ETID=et.ETID 
	WHERE e.Active='True' AND e.SID=3

	--Declare Variables to hold returned information
	DECLARE @StartTime DateTime, @StartTimeLoop DateTime, @ScheduleCode varchar(25), @EID int, @Active bit;

	--Open the temp table
	OPEN TempTable

	--Fetch the first row
	FETCH NEXT FROM TempTable
	INTO @StartTime, @ScheduleCode, @EID,@Active

	--While there are rows
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--ASsign start time in to starttimeloop this ensures that the original starttimein is not modified and can be used for all loops
		SET @StartTimeLoop = @StartTimeIn
		--While starttimeloop is less than endtimein
		WHILE @StartTimeLoop <= @EndTimeIn
		BEGIN
			--If today is the same day as the starttime day
			IF DATEPART(dw,@StartTime) = DATEPART(dw,@StartTimeLoop)
			BEGIN
				--Declare variables for end of starttime year, end of current year and week of year
				Declare @WeekDiff int
				--Get the difference in weeks  between starttimeloop and the events time
				SET @WeekDiff = DATEDIFF(wk,@StartTime,@StartTimeLoop)		
				--If the difference between the startimeloop date and the starttime date is even indicating the event should occur this week
				IF (@WeekDiff % 2 = 0)
				BEGIN
					--Declare a time object to use for comparison
					DECLARE @SchedTime datetime
					--Construct the start time using todays year and the month, day, and time from the events time
					SET @SchedTime = CONVERT(datetime, CAST(DATEPART(yy,@StartTimeLoop)AS varchar) + '-' +  CAST(DATEPART(mm,@StartTimeLoop)AS varchar)
						 + '-' +  CAST(DATEPART(dd,@StartTimeLoop)AS varchar) +
						 ' ' + CAST(DATEPART(hh,@StartTime) AS varchar) + ':' + CAST(DATEPART(mi,@StartTime) AS varchar) +  ':00', 120)
					--If the event occurs before end time
					IF (@SchedTime BETWEEN @StartTimeIn AND @EndTimeIn)
					BEGIN
						--If this is a report request
						IF @Report = 0
							BEGIN
							--Insert into the Report List		
							INSERT INTO dbo.Report_List VALUES (@EID,@SchedTime)
						END
						IF @Report = 1
						BEGIN	
							--Insert into the Event List		
							INSERT INTO dbo.Event_List VALUES ('False',null,@EID,@Active,@SchedTime,null,'False',0,null,null)
						END
						IF @Report = 2
						BEGIN	
							--Insert into the Event List		
							INSERT INTO dbo.Event_ListBackup VALUES (@SchedTime,@EID)
						END
					END
				END
				--Add 7 days to the start time loop
				SET @StartTimeLoop = DATEADD (dd, 7, @StartTimeLoop)
			END
			ELSE
			BEGIN
				--Add a day to the start time loop
				SET @StartTimeLoop = DATEADD (dd, 1, @StartTimeLoop)
			END
			--Set the hour and minute to 00:00 indicating the start of the day
			SET @StartTimeLoop = CONVERT(datetime, CAST(DATEPART(yy,@StartTimeLoop)AS varchar) + '-' +  CAST(DATEPART(mm,@StartTimeLoop)AS varchar)
					+ '-' +  CAST(DATEPART(dd,@StartTimeLoop)AS varchar) + ' 00:00:00', 120)		
		END
	--Fetch next row
	FETCH NEXT FROM TempTable
	INTO @StartTime, @ScheduleCode, @EID, @Active
	END

	--Close the temp table
	CLOSE TempTable

	--Deallocate the temp table
	DEALLOCATE TempTable

END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <January 01 2013>
-- Description:	<This stored procedure is used to create an event list according to dates passed in>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventList_Create]
	-- Add the parameters for the stored procedure here
	@StartTimeIn datetime,
	@EndTimeIn datetime,
	@Report int

AS
BEGIN  
	BEGIN TRANSACTION
	BEGIN TRY
		--Get Relevant Daily Events
		EXEC [dbo].[_sp_Insert_EventList_Daily] @StartTimeIn,@EndTimeIn,@Report
		--Get Relevant Weekly Events
		EXEC [dbo].[_sp_Insert_EventList_Weekly] @StartTimeIn,@EndTimeIn,@Report
		--Get Relevant BiWeekly Events
		EXEC [dbo].[_sp_Insert_EventList_BiWeekly] @StartTimeIn,@EndTimeIn,@Report
		--Get Relevant Monthly Events
		EXEC [dbo].[_sp_Insert_EventList_Monthly] @StartTimeIn,@EndTimeIn,@Report
		--Get Relevant Quarterly Events
		EXEC [dbo].[_sp_Insert_EventList_Quarterly] @StartTimeIn,@EndTimeIn,@Report
		--Get Relevant One Time Events
		EXEC [dbo].[_sp_Insert_EventList_OneTime] @StartTimeIn,@EndTimeIn,@Report
		--Get Relevant Reoccuring Time Events
		EXEC [dbo].[_sp_Insert_EventList_Reoccur] @StartTimeIn,@EndTimeIn,@Report
		COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT ERROR_MESSAGE()
	END CATCH
END 

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <October 28th 2012>
-- Description:	<This stored procedure is used to insert a a comment into the the comment table>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventComment]
	-- Add the parameters for the stored procedure here
	@EID int,
	@Comment varchar(512),
	@UserName varchar(48),	
	@SchedTime datetime,
	@ComTime datetime,
	@Editable bit

AS
BEGIN
	SET NOCOUNT ON;
	--Insert data into 
	INSERT INTO dbo.Event_Comments VALUES (@EID, @Comment, @UserName, @SchedTime, @ComTime, @Editable)
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <December 28th 2012>
-- Description:	<This stored procedure is used to insert an event into the the event category table>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_EventCategory]
	-- Add the parameters for the stored procedure here
	@CategoryName varchar(255),
	@CategoryColor varchar(7)

AS
BEGIN
	SET NOCOUNT ON;
	--Start of transaction
	BEGIN TRANSACTION 
	BEGIN TRY	
		--Insert data into 
		INSERT INTO dbo.Event_Category VALUES (@CategoryName,@CategoryColor)
	COMMIT 
	END TRY
	BEGIN CATCH
		ROLLBACK 
	END CATCH
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <January 01 2013>
-- Description:	<This stored procedure is used to create a report list according to dates passed in>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Insert_ReportList_Create]
	-- Add the parameters for the stored procedure here
	@StartTimeIn datetime,
	@EndTimeIn datetime

AS
BEGIN  
	BEGIN TRANSACTION
	BEGIN TRY
		--Clear the report list table
		DELETE FROM dbo.Report_List WHERE RLID > 0
		--Populate the report list table
		EXEC [dbo].[_sp_Insert_EventList_Create] @StartTimeIn,@EndTimeIn,0
		COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT ERROR_MESSAGE()
	END CATCH
END 

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <August 24th 2013>
-- Description:	<This stored procedure is used to update the current active eventlist >
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Update_EventsListStatus]
	-- Add the parameters for the stored procedure here
	@Shift varchar(32),
	@InPlay bit,
	@LastCreated datetime
AS
BEGIN
	SET NOCOUNT ON;
	--Start of transaction
	BEGIN TRANSACTION 
	BEGIN TRY	
		IF @InPlay = 1
		BEGIN
			--Update row in the events table
			UPDATE dbo.EventList_Status SET InPlay=@InPlay, LastCreated=@LastCreated WHERE ShiftName=@Shift
		END
		ELSE
		BEGIN
			--Update row in the events table
			UPDATE dbo.EventList_Status SET InPlay=@InPlay WHERE ShiftName=@Shift
		END
		COMMIT 
	END TRY
	BEGIN CATCH
		ROLLBACK 
	END CATCH
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <September 30 2013>
-- Description:	<This stored procedure is used to update an events status code and message in the active eventlist>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Update_EventsList_StatusInfo]
	-- Add the parameters for the stored procedure here
	@ELID int,
	@StatusInfo varchar(255),
	@StatusCode int
AS
BEGIN
	SET NOCOUNT ON;
	--Start of transaction
	BEGIN TRANSACTION 
	BEGIN TRY	
		--Update row in the events table
		UPDATE dbo.Event_List SET StatusInfo=@StatusInfo, StatusCode=@StatusCode WHERE ELID=@ELID
		COMMIT 
	END TRY
	BEGIN CATCH
		ROLLBACK 
	END CATCH
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <August 28 2013>
-- Description:	<This stored procedure is used to update an events forwarded status in the active eventlist>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Update_EventsList_Forward]
	-- Add the parameters for the stored procedure here
	@ELID int,
	@Forward bit
AS
BEGIN
	SET NOCOUNT ON;
	--Start of transaction
	BEGIN TRANSACTION 
	BEGIN TRY	
		--Update row in the events table
		UPDATE dbo.Event_List SET Forward=@Forward WHERE ELID=@ELID
		COMMIT 
	END TRY
	BEGIN CATCH
		ROLLBACK 
	END CATCH
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <February 11 2013>
-- Description:	<This stored procedure is used to update an events active status in the active eventlist>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Update_EventsList_Active]
	-- Add the parameters for the stored procedure here
	@ELID int,
	@Active bit
AS
BEGIN
	SET NOCOUNT ON;
	--Start of transaction
	BEGIN TRANSACTION 
	BEGIN TRY	
		--Update row in the events table
		UPDATE dbo.Event_List SET Active=@Active WHERE ELID=@ELID
		COMMIT 
	END TRY
	BEGIN CATCH
		ROLLBACK 
	END CATCH
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <December 28th 2012>
-- Description:	<This stored procedure is used to update an events active status in the eventlist>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Update_Events_Active]
	-- Add the parameters for the stored procedure here
	@EID int,
	@Active bit
AS
BEGIN
	SET NOCOUNT ON;
	--Start of transaction
	BEGIN TRANSACTION 
	BEGIN TRY	
		--Update row in the events table
		UPDATE dbo.Events SET Active=@Active WHERE EID=@EID
		COMMIT 
	END TRY
	BEGIN CATCH
		ROLLBACK 
	END CATCH
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <December 28th 2012>
-- Description:	<This stored procedure is used to update an event in the eventlist and timing tables>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Update_Events]
	-- Add the parameters for the stored procedure here
	@EID int,
	@EventCat varchar(55),
	@Sched varchar(55),
	@Name varchar(255),
	@FinishTimeRequired bit,
	@URL varchar(255),
	@Doc varchar(255),
	@Script varchar(255),
	@Active bit,
	@StartTime datetime,
	@ScheduleCode varchar(10),
	@UserName varchar (255),
	@info varchar (510)
AS
BEGIN
	SET NOCOUNT ON;
	--Start of transaction
	BEGIN TRANSACTION 
	BEGIN TRY	
		--Inteter to represent current row
		DECLARE @ETID int
		--Integer to represent ECID
		DECLARE @ECID int
		--Integer to represent ECID
		DECLARE @SID int
		--Set the variable to the last row added
		SET @ETID = (SELECT ETID FROM dbo.Events WHERE EID = @EID)
		--Set the category ID
		SET @ECID = (SELECT ECID FROM dbo.Event_Category WHERE CategoryName=@EventCat)
		--Set the schedule ID
		SET @SID = (SELECT SID FROM dbo.Event_Schedule WHERE Schedule=@Sched)
		--update row in the event timing table
		UPDATE dbo.Event_Timing SET SID=@SID, StartTime=@StartTime, ScheduleCode=@ScheduleCode WHERE ETID=@ETID 
		--Update row in the events table
		UPDATE dbo.Events SET ECID=@ECID, ETID=@ETID, SID=@SID, Name=@Name, FinishTimeRequired=@FinishTimeRequired, URL=@URL, Active=@Active, Documentation=@Doc, Script=@Script  WHERE EID=@EID
		--Datetime for now
		DECLARE @CurrentDate datetime
		--Get the current date
		SET @CurrentDate = (SELECT GETDATE())
		--Insert a record into the event audit table
		EXEC [dbo].[_sp_Insert_EventAudit] @CurrentDate,@UserName,@EID,@info,@Name,@EventCat
		COMMIT 
	END TRY
	BEGIN CATCH
		ROLLBACK 
	END CATCH
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <December 28th 2012>
-- Description:	<This stored procedure is used to update an event in the eventlist table>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Update_EventList]
	-- Add the parameters for the stored procedure here
	@ELID int,
	@Complete bit,
	@EndTime datetime,
	@Active bit,
	@StartTime datetime,
	@StatusCode int,
	@StatusInfo varchar(255),
	@UserName varchar(128)
	
AS
BEGIN
	SET NOCOUNT ON;
	--If begining date is invalide set both  start and endtime to null
	IF (DATEPART(yyyy,@StartTime) = 1900)
	BEGIN 
		--Set startime to null
		SET @StartTime = null
		--Set endtime to null
		SET @EndTime = null
	END
	--If the begining date is valid and the enddate is invalid with year 1900
	IF not(DATEPART(yyyy,@StartTime) = 1900) and (DATEPART(yyyy,@EndTime) = 1900)
	BEGIN
		--Set endtime to null
		SET @EndTime = null
	END
	--Update row in the event list table
	UPDATE dbo.Event_List SET Complete=@Complete, EndTime=@EndTime, Active=@Active, 
			StartTime=@StartTime, StatusCode=@StatusCode, StatusInfo=@StatusInfo, UserName=@UserName
	WHERE ELID=@ELID
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <October 28th 2012>
-- Description:	<This stored procedure is used to update information in the comment table>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Update_EventComment]
	-- Add the parameters for the stored procedure here
	@CID int,
	@Comment varchar(512),
	@UserName varchar(48),	
	@SchedTime datetime,
	@ComTime datetime,
	@Editable bit

AS
BEGIN
	SET NOCOUNT ON;
	--Insert data into 
	UPDATE dbo.Event_Comments SET Comment=@Comment, UserName=@UserName, ScheduledTime=@SchedTime, CommentTime=@ComTime, Editable=@Editable WHERE CID=@CID
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <October 28th 2012>
-- Description:	<This stored procedure is used to update information in the category table>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Update_EventCategory]
	-- Add the parameters for the stored procedure here
	@ECID int,	
	@CatName varchar(255),
	@CatColor varchar(7)

AS
BEGIN
	SET NOCOUNT ON;
	--Insert data into 
	UPDATE dbo.Event_Category SET CategoryName=@CatName, CategoryColor=@CatColor WHERE  ECID=@ECID
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <April 30 2014>
-- Description:	<This stored procedure is used to update an events editabe status>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Update_Comment_Editable]
	-- Add the parameters for the stored procedure here
	@CID int,
	@Editable bit
AS
BEGIN
	SET NOCOUNT ON;
	--Start of transaction
	BEGIN TRANSACTION 
	BEGIN TRY	
		--Update row in the events table
		UPDATE dbo.Event_Comments SET Editable=@Editable WHERE CID=@CID
		COMMIT 
	END TRY
	BEGIN CATCH
		ROLLBACK 
	END CATCH
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <February 9th 2013>
-- Description:	<This stored procedure is used to select all events from the ReportList table with all relevant data>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_ReportList]
	@Category varchar(25)
AS
BEGIN
	SET NOCOUNT ON;
	--SQL variable
	DECLARE @SQL varchar(1024)
	--Select all relevant rows and columns from the events,event_timing,event_category and event_schedule tables
	SET @SQL =( CASE WHEN (@Category='*') THEN 
		'SELECT rl.EID, CONVERT (varchar(5), rl.StartTime, 114) AS StartTime, CONVERT (varchar(25), rl.StartTime, 101) AS StartDate,
		ev.CategoryName,ev.name, ev.url, ev.Documentation, ev.CategoryColor
		FROM dbo.Report_list rl join 
		(SELECT e.name,e.url,e.documentation,e.finishtimerequired,e.eid,ec.categoryname, ec.categorycolor  
		FROM dbo.events e JOIN dbo.event_category ec ON e.ecid = ec.ecid) ev 
		ON rl.eid = ev.eid WHERE ev.CategoryName LIKE ''%'''
	ELSE
		--Select all relevant rows and columns from the events,event_timing,event_category and event_schedule tables
		'SELECT rl.EID, CONVERT (varchar(5), rl.StartTime, 114) AS StartTime, CONVERT (varchar(25), rl.StartTime, 101) AS StartDate,				
		ev.CategoryName,ev.name, ev.url, ev.Documentation, ev.CategoryColor
		FROM dbo.Report_list rl join 
		(SELECT e.name,e.url,e.documentation,e.finishtimerequired,e.eid,ec.categoryname, ec.categorycolor 
		FROM dbo.events e JOIN dbo.event_category ec ON e.ecid = ec.ecid) ev 
		ON rl.eid = ev.eid WHERE ev.CategoryName=''' + @Category + ''' '
	END)
	--Add the order by statement to the list
	SET @SQL = @SQL + 'ORDER BY StartDate, StartTime, ev.categoryname, ev.name ASC'		
	EXEC (@SQL)	

END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <September 10th 2013>
-- Description:	<This stored procedure is used to select event data from the performance table>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_Performance]
@EID int,
@Count int,
@StartTime datetime,
@EndTime datetime

AS
BEGIN
	SET NOCOUNT ON;
	--SQL variable
	DECLARE @SQL varchar(1024)
	--SET SQL 
	SET @SQL = 
		' CONVERT (varchar(25), ep.ShiftDate, 101) AS SchedDate, CONVERT (varchar(8), ep.ShiftDate, 114) AS SchedTime, 
		CONVERT (varchar(25), ep.StartTime, 101) AS StartDate, CONVERT (varchar(8), ep.StartTime, 114) AS StartTime,
		CONVERT (varchar(25), ep.EndTime, 101) AS EndDate, CONVERT (varchar(8), ep.EndTime, 114) AS EndTime, DATEDIFF(mi,ep.StartTime,ep.EndTime) AS ''Duration[Mins]''
		FROM dbo.Event_Performance ep 
		WHERE ep.eid=' + CAST(@EID AS varchar)
	--If all data is requested
	IF @Count != 0
	BEGIN
		--Select event information from the event_performance table
		SET @SQL = 'SELECT TOP ' + CAST(@Count AS varchar) + @SQL + ' ORDER BY ShiftDate DESC'
	END
	ELSE
	BEGIN
		--Select event information from the event_performance table
		SET @SQL = 'SELECT' + @SQL
		--If the begining and end date are both valid by not contain year 1900
		IF not(DATEPART(yyyy,@StartTime) = 1900) and not(DATEPART(yyyy,@EndTime) = 1900)
		BEGIN
			SET @SQL = @SQL + ' AND (ep.ShiftDate BETWEEN '''+ CAST(@StartTime AS varchar) + ''' AND ''' + CAST(@EndTime AS varchar) + ''') ORDER BY ShiftDate DESC'
		END
		ELSE
		BEGIN
			SET @SQL = @SQL + ' ORDER BY ShiftDate DESC'
		END
	END
	--Execute sql
	EXEC (@SQL)
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <January 19th 2013>
-- Description:	<This stored procedure is used to select all events from the Event_Schedule tables>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_EventSchedule]

AS
BEGIN
	SET NOCOUNT ON;
	--Select all rows from the Event_Schedule table
	SELECT * FROM dbo.Event_Schedule
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <January 18th 2013>
-- Description:	<This stored procedure is used to select all events from the relative Events table 
--				according to the passed in category>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_EventManagement]
	@Category varchar(25),
	@Schedule varchar (25)
AS
BEGIN
	SET NOCOUNT ON;
	--SQL variable
	DECLARE @SQL varchar(1024)
	--Select all relevant rows and columns from the events,event_timing,event_category and event_schedule tables
	SET @SQL =( CASE WHEN (@Category='*') THEN 
		'SELECT eetes.EID, ec.CategoryName, eetes.name, eetes.active, eetes.Schedule, CONVERT (varchar(5), eetes.StartTime, 114) AS StartTime, eetes.ScheduleCode, ec.CategoryColor, CONVERT (varchar(25), eetes.StartTime, 101) AS StartDate FROM dbo.Event_Category ec JOIN 
		(SELECT e.name, e.etid,e.ecid,e.finishtimerequired,e.url,e.active,e.EID, eet.Schedule, eet.StartTime, eet.ScheduleCode FROM dbo.Events e JOIN 
		(SELECT es.SID, es.Schedule, et.etid, et.StartTime, et.ScheduleCode FROM dbo.Event_Schedule es JOIN dbo.Event_Timing et ON es.SID=et.SID) 
		eet ON e.ETID=eet.ETID) eetes 
		ON ec.ECID=eetes.ECID WHERE ec.CategoryName LIKE ''%'''
	ELSE
	--Select all relevant rows and columns from the events,event_timing,event_category and event_schedule tables
	'SELECT eetes.EID, ec.CategoryName, eetes.name, eetes.active, eetes.Schedule, CONVERT (varchar(5), eetes.StartTime, 114) AS StartTime, eetes.ScheduleCode, ec.CategoryColor, CONVERT (varchar(25), eetes.StartTime, 101) AS StartDate FROM dbo.Event_Category ec JOIN 
	(SELECT e.name, e.etid,e.ecid,e.finishtimerequired,e.url,e.active,e.EID, eet.Schedule, eet.StartTime, eet.ScheduleCode FROM dbo.Events e JOIN 
	(SELECT es.SID, es.Schedule, et.etid, et.StartTime, et.ScheduleCode FROM dbo.Event_Schedule es JOIN dbo.Event_Timing et ON es.SID=et.SID) 
	eet ON e.ETID=eet.ETID) eetes 
	ON ec.ECID=eetes.ECID WHERE ec.CategoryName=''' + @Category + ''' '
	END)
	--If the schedule value is not equal to all
	IF @Schedule != '*'
	BEGIN
		--Add and statement including schedule type
		SET @SQL = @SQL + ' AND eetes.Schedule=''' + @Schedule + ''' '
	End
	--Add the order by statement to the list
	SET @SQL = @SQL + 'ORDER BY ec.CategoryName,eetes.Name,StartTime'		
	EXEC (@SQL)
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <August 24th 2013>
-- Description:	<This stored procedure is used to select the event list status table>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_EventListStatus]

AS
BEGIN
	SET NOCOUNT ON;

	--Get the shiftstatus table
	SELECT els.ShiftName, els.InPlay, els.LastCreated FROM dbo.EventList_Status els

END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <January 11th 2013>
-- Description:	<This stored procedure is used to select all forwarder events from the EventList table with all relevant data>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_EventListForward]

AS
BEGIN
	SET NOCOUNT ON;
	--Select all rows from the Event_Listt table with relevant information
	SELECT el.ELID, el.ScheduledTime ,CONVERT (varchar(5), el.ScheduledTime, 114) AS SchedTime, CONVERT (varchar(25), el.ScheduledTime, 101) AS SchedDate, 
		   ev.CategoryName,ev.name, ev.url,ev.finishtimerequired,
		   CONVERT (varchar(8), el.StartTime, 114) AS StartTime, CONVERT (varchar(25), el.StartTime, 101) AS StartDate,
		   CONVERT (varchar(8), el.EndTime, 114) AS EndTime, CONVERT (varchar(25), el.EndTime, 101) AS EndDate, el.Active, ev.Documentation, ev.Script, el.Forward, el.StatusCode, el.StatusInfo, ev.eid, ev.categorycolor
	FROM dbo.event_list el join 
	(SELECT e.name,e.url,e.finishtimerequired,e.eid,e.Documentation, e.Script,ec.categoryname, ec.categorycolor 
	FROM dbo.events e JOIN dbo.event_category ec ON e.ecid = ec.ecid) ev 
	ON el.eid = ev.eid WHERE Complete=0 AND Active=1 ORDER BY el.ScheduledTime ASC, ev.categoryname ASC, ev.name ASC
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <February 24th 2014>
-- Description:	<This stored procedure is used to select counts of current events in the event list table>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_EventListCounts]
@All int OUTPUT,
@Unt int OUTPUT,
@Com int OUTPUT,
@Inc int OUTPUT,
@Err int OUTPUT,
@For int OUTPUT

AS
BEGIN
	SET NOCOUNT ON;
	--Get the counts for the events
	SELECT @All = COUNT(StatusCode) FROM dbo.event_list 
	SELECT @Unt = COUNT(StatusCode) FROM dbo.event_list WHERE StatusCode=0
	SELECT @Com = COUNT(StatusCode) FROM dbo.event_list WHERE StatusCode=1
	SELECT @Inc = COUNT(StatusCode) FROM dbo.event_list WHERE StatusCode=2
	SELECT @Err = COUNT(StatusCode) FROM dbo.event_list WHERE StatusCode=3
	SELECT @For = COUNT(Forward) FROM dbo.event_list WHERE Forward='true'
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <February 9th 2013>
-- Description:	<This stored procedure is used to select all events from the ReportList table with all relevant data>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_EventListBackup]
AS
BEGIN
	SET NOCOUNT ON;
	--SQL variable
	SELECT * FROM EventListBackup ORDER BY Scheduled, Category, Name;

END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <August 24th 2013>
-- Description:	<This stored procedure is used to select the active event list status row>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_EventListActiveStatus]
@Shift varchar(64) OUTPUT,
@LastCreated varchar(64) OUTPUT

AS
BEGIN
	SET NOCOUNT ON;
	--Get the name of the shift in play
	SELECT @Shift = els.ShiftName FROM dbo.EventList_Status els WHERE InPlay=1
	--Get the date of the shift in play
	SELECT @LastCreated = CONVERT (varchar(32), els.LastCreated, 121) FROM dbo.EventList_Status els WHERE InPlay=1
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <January 11th 2013>
-- Description:	<This stored procedure is used to select all events from the EventList table with all relevant data>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_EventList]

AS
BEGIN
	SET NOCOUNT ON;
	--Select all rows from the Event_Listt table with relevant information
	SELECT el.ELID, el.ScheduledTime ,CONVERT (varchar(5), el.ScheduledTime, 114) AS SchedTime, CONVERT (varchar(25), el.ScheduledTime, 101) AS SchedDate, 
		   ev.CategoryName,ev.name, ev.url,ev.finishtimerequired,
		   CONVERT (varchar(8), el.StartTime, 114) AS StartTime, CONVERT (varchar(25), el.StartTime, 101) AS StartDate,
		   CONVERT (varchar(8), el.EndTime, 114) AS EndTime, CONVERT (varchar(25), el.EndTime, 101) AS EndDate, el.Active, ev.Documentation, ev.Script, el.Forward, el.StatusCode, el.StatusInfo, ev.eid, ev.categorycolor, el.UserName
	FROM dbo.event_list el join 
	(SELECT e.name,e.url,e.finishtimerequired,e.eid,e.Documentation, e.Script,ec.categoryname, ec.categorycolor 
	FROM dbo.events e JOIN dbo.event_category ec ON e.ecid = ec.ecid) ev 
	ON el.eid = ev.eid ORDER BY el.ScheduledTime ASC, ev.categoryname ASC, ev.name ASC
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <January 5th 2014>
-- Description:	<This stored procedure is used to select the completed status of an event from the event list table>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_EventComplete]
@ELID int

AS
BEGIN
	SET NOCOUNT ON;
	--Boolean to return
	Declare @ReturnBool bit
	--Select the event specifeds completed status
	SET @ReturnBool = (SELECT el.Complete FROM dbo.Event_List el WHERE el.elid = @ELID)
	--Return the bit
	RETURN @ReturnBool
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <December 28th 2012>
-- Description:	<This stored procedure is used to select all events from the Event_Category tables>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_EventCategory]

AS
BEGIN
	SET NOCOUNT ON;
	--Select all rows from the Event_Category table
	SELECT * FROM dbo.Event_Category 
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <October 18th 2013>
-- Description:	<This stored procedure is used to select event audit from the audit table>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_EventAudit]
@EID int,
@Count int,
@StartTime datetime,
@EndTime datetime
AS
BEGIN
	SET NOCOUNT ON;
	--SQL variable
	DECLARE @SQL varchar(1024)
	--If all data is requested
	IF @Count != 0
	BEGIN
		--Select an events audit information from the audit table
		SET @SQL = 
			'SELECT TOP ' + CAST(@Count AS varchar) + ' ea.EAUID AS AuditID, ea.EID AS EventID, CONVERT (varchar(16), ea.DateCreated, 120) AS DateTime, ea.Info, ea.UserName
			FROM dbo.Event_Audit ea 
			WHERE ea.EID=' + CAST(@EID AS varchar) +
			'ORDER BY ea.DateCreated DESC'
	END
	ELSE
	BEGIN
		--Select an events audit information from the audit table
		SET @SQL = 
			'SELECT ea.EAUID AS AuditID, ea.EID AS EventID, CONVERT (varchar(16), ea.DateCreated, 120) AS DateTime, ea.Info, ea.UserName
			FROM dbo.Event_Audit ea 
			WHERE ea.EID=' + CAST(@EID AS varchar) 
		--If the begining and end date are both valid by not contain year 1900
		IF not(DATEPART(yyyy,@StartTime) = 1900) and not(DATEPART(yyyy,@EndTime) = 1900)
		BEGIN
			SET @SQL = @SQL + ' AND (ea.DateCreated BETWEEN '''+ CAST(@StartTime AS varchar) + ''' AND ''' + CAST(@EndTime AS varchar) + ''') ORDER BY ea.DateCreated DESC'
		END
		ELSE
		BEGIN
			SET @SQL = @SQL + ' ORDER BY ea.DateCreated DESC'
		END
	END
	--Execute sql
	EXEC (@SQL)
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <January 25th 2013>
-- Description:	<This stored procedure is used to select a single event from the EventList table with all relevant data>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_Event]
@EID int

AS
BEGIN
	SET NOCOUNT ON;
	--Select an event from the event table with its timing and schedule information
	SELECT ef.eid, ef.name, ef.Documentation, ef.Schedule, ef.StartTime, ef.ScheduleCode, ef.FinishTimeRequired, ef.Active, ec.CategoryName, ef.URL 
	FROM dbo.Event_Category ec 
	JOIN (SELECT e.eid, e.ecid, e.name, e.Documentation, e.URL, etes.Schedule, etes.StartTime, etes.ScheduleCode, e.FinishTimeRequired, e.Active FROM dbo.Events e JOIN 
	(SELECT et.ETID, et.SID, et.StartTime, et.ScheduleCode, es.Schedule 
	FROM dbo.Event_Timing et JOIN dbo.Event_Schedule es ON et.SID=es.SID) etes
	ON e.ETID = etes.ETID) ef ON ec.ecid=ef.ecid 
	WHERE ef.eid=@EID
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <October 18th 2013>
-- Description:	<This stored procedure is used to select event data from the comment table>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_Comment]
@EID int,
@Count int,
@StartTime datetime,
@EndTime datetime,
@CommentTime bit

AS
BEGIN
	SET NOCOUNT ON;
	--SQL variable
	DECLARE @SQL varchar(1024)
	--Set SQL variable with standard sql
	SET @SQL = 
		' ec.CID, CONVERT (varchar(10), ec.ScheduledTime, 101) AS EventDate, CONVERT (varchar(5), ec.ScheduledTime, 114) AS EventTime,
						CONVERT (varchar(10), ec.CommentTime, 101) AS CommDate, CONVERT (varchar(5), ec.CommentTime, 114) AS CommTime,
						ec.Comment, ec.UserName, ec.Editable
		FROM dbo.Event_Comments ec
		WHERE ec.eid=' + CAST(@EID AS varchar)
	--If all data is requested
	IF @Count != 0
	BEGIN
		--Select event information from the event-comments table
		SET @SQL = 	'SELECT TOP ' + CAST(@Count AS varchar) + @SQL + ' ORDER BY ec.ScheduledTime DESC, ec.CommentTime DESC'
	END
	ELSE
	BEGIN
		--Select event information from the event_comments table
		SET @SQL = 'SELECT' + @SQL 
		--If the begining and end date are both valid by not containing year 1900
		IF not(DATEPART(yyyy,@StartTime) = 1900) and not(DATEPART(yyyy,@EndTime) = 1900)
		BEGIN
			--If CommentTime is True
			IF @CommentTime = 1
			BEGIN
				SET @SQL = @SQL + ' AND (ec.CommentTime BETWEEN '''+ CAST(@StartTime AS varchar) + ''' AND ''' + CAST(@EndTime AS varchar) + ''')'
			END
			ELSE
			BEGIN
				SET @SQL = @SQL + ' AND (ec.ScheduledTime BETWEEN '''+ CAST(@StartTime AS varchar) + ''' AND ''' + CAST(@EndTime AS varchar) + ''')'
			END
		END
		--Add Order by statement
		SET @SQL = @SQL + ' ORDER BY ec.ScheduledTime DESC, ec.CommentTime DESC'
	END
	--Execute sql
	EXEC (@SQL)
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <February 9th 2013>
-- Description:	<This stored procedure is used to select all events from the ReportList table with all relevant data>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_AuditList]
	@StartTimeIn datetime,
	@EndTimeIn datetime,
	@Category varchar(25)
AS
BEGIN
	SET NOCOUNT ON;
	--SQL variable
	DECLARE @SQL varchar(1024)
	--Select all relevant rows and columns from the events,event_timing,event_category and event_schedule tables
	SET @SQL =( CASE WHEN (@Category='*') THEN 
		'SELECT ela.EID, 
		CONVERT (varchar(10), ela.StartTime, 101) AS StartDate,
		CONVERT (varchar(5), ela.StartTime, 114) AS StartTime, 
		CONVERT (varchar(10), ela.EndTime, 101) AS EndDate, 
		CONVERT (varchar(5), ela.EndTime, 114) AS EndTime, 
		CONVERT (varchar(10), ela.ScheduledTime, 101) AS ScheduledDate,
		CONVERT (varchar(5), ela.ScheduledTime, 114) AS ScheduledTime,
		ev.CategoryName,ev.name, ev.url, ev.Documentation, ela.UserName, ela.Active, ev.CategoryColor, ela.Forward
		FROM dbo.EventList_Audit ela join 
		(SELECT e.name,e.url,e.Documentation,e.finishtimerequired,e.eid,ec.categoryname, ec.categorycolor 
		FROM dbo.events e JOIN dbo.event_category ec ON e.ecid = ec.ecid) ev 
		ON ela.eid = ev.eid WHERE ev.CategoryName LIKE ''%'''
	ELSE
		--Select all relevant rows and columns from the events,event_timing,event_category and event_schedule tables
		'SELECT ela.EID, 
		CONVERT (varchar(10), ela.StartTime, 101) AS StartDate, 
		CONVERT (varchar(5), ela.StartTime, 114) AS StartTime,
		CONVERT (varchar(10), ela.EndTime, 101) AS EndDate,
		CONVERT (varchar(5), ela.EndTime, 114) AS EndTime,  
		CONVERT (varchar(10), ela.ScheduledTime, 101) AS ScheduledDate,
		CONVERT (varchar(5), ela.ScheduledTime, 114) AS ScheduledTime,
		ev.CategoryName,ev.name, ev.url, ev.Documentation, ela.UserName, ela.Active, ev.CategoryColor, ela.Forward
		FROM dbo.EventList_Audit ela join 
		(SELECT e.name,e.url,e.Documentation,e.finishtimerequired,e.eid,ec.categoryname, ec.categorycolor 
		FROM dbo.events e JOIN dbo.event_category ec ON e.ecid = ec.ecid) ev 
		ON ela.eid = ev.eid WHERE ev.CategoryName=''' + @Category + ''' '
	END)
PRINT @SQL
	--Add the order by statement to the list
	SET @SQL = @SQL +  ' AND ela.ScheduledTime BETWEEN ''' + CONVERT(varchar(25), @StartTimeIn, 120) + ''' AND''' + CONVERT(varchar(25), @EndTimeIn, 120) + ''' ORDER BY ScheduledDate, ScheduledTime, ev.categoryname, ev.name ASC'
	EXEC (@SQL)
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <January 25th 2013>
-- Description:	<This stored procedure is used to select the active status from an event in the eventlist table>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Select_ActiveEventList]
@ELID int

AS
BEGIN
	--Boolean to return
	Declare @ReturnBool bit

	SET NOCOUNT ON;
	--Select an event from the event table with its timing and schedule information
	SET @ReturnBool = (SELECT Active FROM dbo.Event_List WHERE ELID=@ELID)
	--Return the bit
	RETURN @ReturnBool
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <August 25th 2013>
-- Description:	<This stored procedure is used call all relevant stored procedures related to an eventlist rollover>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_RollOver_EventList]
	-- Add the parameters for the stored procedure here
	@StartTimeIn datetime,
	@EndTimeIn datetime,
	@DayShift bit,
	@NightShift bit

AS
BEGIN  
	BEGIN TRANSACTION
	BEGIN TRY
		--Last shift created
		Declare @LastShift datetime
		--Select the time the last event list was created
		SET @LastShift = (SELECT TOP 1 LastCreated FROM dbo.EventList_Status ORDER BY LastCreated DESC)
		--If the @LastShift is not equal to the start time of the shift being created
		IF @LastShift <> @StartTimeIn
		BEGIN
			--Update the EventListStatus table dayshift
			EXEC [dbo].[_sp_Update_EventsListStatus] 'DayShift', @DayShift, @StartTimeIn
			--Update the EventListStatus table nightshift
			EXEC [dbo].[_sp_Update_EventsListStatus] 'NightShift', @NightShift, @StartTimeIn
			--Create a new event list
			EXEC [dbo].[_sp_Insert_EventList_Create] @StartTimeIn, @EndTimeIn, 1
		END
		ELSE
		BEGIN
			PRINT 'No Creation'
		END
		COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK 
		SELECT ERROR_MESSAGE()
	END CATCH
END 

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <December 28th 2012>
-- Description:	<This stored procedure is used to delete an event from the event and timing tables>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Delete_Events]
	-- Add the parameters for the stored procedure here
	@EID int,
	@UserName varchar(255)
AS
BEGIN
	--Start of transaction
	BEGIN TRANSACTION 
	BEGIN TRY
		--Temporary Table to hold active events
		DECLARE TempTable CURSOR FOR 
		SELECT e.ETID, ec.CategoryName, e.Name
		FROM dbo.Events e
		JOIN dbo.Event_Category ec
		ON e.ECID=ec.ECID
		WHERE EID = @EID
		
		--ETID integer row value, event name, and event category
		DECLARE @ETID int, @Name varchar(255), @Category varchar(255), @Now datetime
		
		--Open the temp table
		OPEN TempTable
		--Fetch the first row
		FETCH NEXT FROM TempTable
		INTO @ETID, @Category, @Name
		
		--Set now to current date time
		SET @Now = GETDATE()
		--Delete the events performance data
		DELETE FROM dbo.Event_Performance WHERE EID = @EID
		--Delete the event if it exists in the current Report_List
		DELETE FROM dbo.Report_List WHERE EID = @EID
		--Delete the row from the events table
		DELETE FROM dbo.Events WHERE EID = @EID
		--Delete the event from the timing table 
		DELETE FROM dbo.Event_Timing WHERE ETID = @ETID
		--Insert a record into the Event Aduit table
		EXEC [dbo].[_sp_Insert_EventAudit] @Now, @UserName, @EID, 'Event has been deleted', @Name, @Category
		--Close the temp table
		CLOSE TempTable
		--Deallocate the temp table
		DEALLOCATE TempTable
		COMMIT 
	END TRY
	BEGIN CATCH
		ROLLBACK
	END CATCH
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <December 28th 2012>
-- Description:	<This stored procedure is used to delete an event from the eventlist tables>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Delete_EventList]
	-- Add the parameters for the stored procedure here
	@ELID int
AS
BEGIN
	SET NOCOUNT ON;
	--Start of transaction
	BEGIN TRANSACTION 
	BEGIN TRY
		--Delete the row from the eventlist table
		DELETE FROM dbo.Event_List WHERE ELID = @ELID
		COMMIT 
	END TRY
	BEGIN CATCH
		ROLLBACK 
	END CATCH
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <February 25th 2014>
-- Description:	<This stored procedure is used to delete an event comment from the Event_Comments tables>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Delete_EventComment]
	-- Add the parameters for the stored procedure here
	@CID int
AS
BEGIN
	--Start of transaction
	BEGIN TRANSACTION 
	BEGIN TRY
		--Delete the row from the Event_Category table
		DELETE FROM dbo.Event_Comments WHERE CID = @CID
		COMMIT 
	END TRY
	BEGIN CATCH
		ROLLBACK 
	END CATCH
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <December 28th 2012>
-- Description:	<This stored procedure is used to delete an event from the Event_Category tables>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Delete_EventCategory]
	-- Add the parameters for the stored procedure here
	@ECID int
AS
BEGIN
	SET NOCOUNT ON;
	--Start of transaction
	BEGIN TRANSACTION 
	BEGIN TRY
		--Delete the row from the Event_Category table
		DELETE FROM dbo.Event_Category WHERE ECID = @ECID
		COMMIT 
	END TRY
	BEGIN CATCH
		ROLLBACK 
	END CATCH
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <January 10th 2013>
-- Description:	<This stored procedure is used to insert completed events into the audit table and remove 
--					them from the eventlist table>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_RollOver_EventList_toAudit]

AS
BEGIN
	SET NOCOUNT ON;
	--Temporary Table to hold completed events
	DECLARE TempTable CURSOR FOR 
	SELECT ELID,Complete,EndTime,EID,Active,ScheduledTime,StartTime,UserName,Forward FROM dbo.Event_List
	
	--Declare Variables to hold returned information
	DECLARE @ELID int, @EndTime DateTime, @EID int, @Active bit, @Forward bit,
	@ScheduledTime datetime, @StartTime DateTime, @Complete bit, @UserName varchar(128);
	
	--Open the temp table
	OPEN TempTable
	
	--Fetch the first row
	FETCH NEXT FROM TempTable
	INTO @ELID, @Complete, @EndTime, @EID, @Active, @ScheduledTime, @StartTime, @UserName, @Forward
	
	BEGIN TRANSACTION
	BEGIN TRY
		--While there are rows
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF @Complete = 'true' OR @Active = 'false'
			BEGIN
				--Insert the event into the event audit table
				EXEC [dbo].[_sp_Insert_EventListAudit] @StartTime, @EndTime, @UserName, @EID, @Active, @ScheduledTime, @Forward
				--IF the event is active
				IF @Active = 'true'
				BEGIN
					--Insert time data into the performance table
					Exec [dbo].[_sp_Insert_EventPerformance] @EID, @StartTime, @EndTime, @ScheduledTime
				END
				--Delete the event from the event list
				EXEC [dbo].[_sp_Delete_EventList] @ELID
			END
			ELSE
			BEGIN
				--Update the event to Forward status
				EXEC [dbo].[_sp_Update_EventsList_Forward] @ELID, 'true'
			END
			--Fetch the first row
			FETCH NEXT FROM TempTable
			INTO @ELID, @Complete, @EndTime, @EID, @Active, @ScheduledTime, @StartTime, @UserName, @Forward
		END
		COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SELECT ERROR_MESSAGE()
	END CATCH
	
	--Close the temp table
	CLOSE TempTable
		--Deallocate the temp table
	DEALLOCATE TempTable
	
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <March 11 2014>
-- Description:	<This stored procedure is used to create an event list backup list>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Archive_EventListBackup_Create]
	-- Add the parameters for the stored procedure here
	@StartTimeIn datetime,
	@EndTimeIn datetime

AS
BEGIN  
SET NOCOUNT ON 
	BEGIN TRANSACTION
	BEGIN TRY
		--Clear the Event list backup table
		DELETE FROM dbo.Event_ListBackup WHERE ELBID > 0
		--Populate the event_listbackup table
		EXEC [dbo].[_sp_Insert_EventList_Create] @StartTimeIn,@EndTimeIn,2
		
		--Decalare a table to return
		DECLARE @ReturnTable TABLE(EID int, Scheduled datetime, Category varchar(255), Name varchar(255),FinishTime bit,StartTime datetime, EndTime datetime,
									Documentation varchar(512), SiteLink varchar(512))
		--Declare a temp table
		DECLARE TempTable CURSOR FOR
		SELECT eve.EID, eve.CategoryName, eve.Name, eve.Documentation,eve.URL,eve.FinishTimeRequired, elb.ScheduledTime
		FROM dbo.Event_ListBackup elb 
		JOIN (SELECT ec.CategoryName,e.Name,e.EID,e.Active,e.Documentation,e.URL,e.FinishTimeRequired FROM dbo.Events e JOIN dbo.Event_Category ec ON e.ECID=ec.ECID) eve 
		ON elb.EID=eve.EID

		--Declare Variables to hold returned information
		DECLARE @EID int, @Cat varchar(255), @Name varchar(255), @Doc varchar(512), @Url varchar(512), @Fin bit, @Sched datetime

		--Open the temp table
		OPEN TempTable

		--Fetch the first row
		FETCH NEXT FROM TempTable
		INTO @EID, @Cat, @Name, @Doc, @Url, @Fin, @Sched

		--While there are rows
		WHILE @@FETCH_STATUS = 0
		BEGIN
			--Insert the row into the return table
			INSERT INTO @ReturnTable VALUES (@EID, @Sched, @Cat, @Name, @Fin, null, null, @Doc, @Url)
			--Fetch next row
			FETCH NEXT FROM TempTable
			INTO @EID, @Cat, @Name, @Doc, @Url, @Fin, @Sched
		END

		--Close the temp table
		CLOSE TempTable

		--Deallocate the temp table
		DEALLOCATE TempTable
		
		--Select the return table
		DELETE FROM EventListBackup;
		INSERT INTO EventListBackup
		SELECT * 
--		INTO EventListBackup 
		FROM @ReturnTable ORDER BY Scheduled, Category, Name;
		COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT ERROR_MESSAGE()
	END CATCH
END 

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <March 8th 2014>
-- Description:	<This stored procedure is used to delete all data older than the supplied time parameters from the Event Audtit table>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Archive_EventListAudit]
	
AS
BEGIN
	--Declare variables
	DECLARE @Now datetime, @Week int, @Month int, @Year int
	--DECLARE TempTables
	DECLARE @TempEvent TABLE (EAID int not null)
	--SET Time modifiers Data before the date modified by these time modifers will be discarded
	SET @Week = 0; SET @Month = 0; SET @Year = -7;
	--Set todays date
	SET @Now = GETDATE()
	--Implement the time modifiers
	SET @Now = DATEADD(ww,@Week,@Now);SET @Now = DATEADD(mm,@Month,@Now);SET @Now = DATEADD(yy,@Year,@Now);

	--SELECT all audit data that is older than the specified time and doesnt have an event in the Events table associated to it
	INSERT INTO @TempEvent 
	SELECT EAID FROM dbo.EventList_Audit WHERE ScheduledTime < @Now 

	--Delete all rows that were selected
	DELETE FROM dbo.EventList_Audit WHERE EAID IN (SELECT EAID FROM @TempEvent)

	--select the number of rows deleted
	SELECT @@ROWCOUNT
END

GO

-- =============================================
-- Author:		<Alexander Olpin>
-- Create date: <March 8th 2014>
-- Description:	<This stored procedure is used to delete all data older than the supplied time parameters from the Event Audtit table>
-- Version:		<1.1>
-- =============================================
CREATE PROCEDURE [dbo].[_sp_Archive_EventAudit]
	
AS
BEGIN
	--Declare variables
	DECLARE @Now datetime, @Week int, @Month int, @Year int
	--DECLARE TempTables
	DECLARE @TempEvent TABLE (EAUID int not null)
	--SET Time modifiers Data before the date modified by these time modifers will be discarded
	SET @Week = 0; SET @Month = 0; SET @Year = -7;
	--Set todays date
	SET @Now = GETDATE()
	--Implement the time modifiers
	SET @Now = DATEADD(ww,@Week,@Now);SET @Now = DATEADD(mm,@Month,@Now);SET @Now = DATEADD(yy,@Year,@Now);

	--SELECT all audit data that is older than the specified time and doesnt have an event in the Events table associated to it
	INSERT INTO @TempEvent 
	SELECT EAUID FROM dbo.Event_Audit WHERE DateCreated < @Now AND EID NOT IN (SELECT EID FROM dbo.Events)

	--Delete all rows that were selected
	DELETE FROM dbo.Event_Audit WHERE EAUID IN (SELECT EAUID FROM @TempEvent)

	--select the number of rows deleted
	SELECT @@ROWCOUNT
END

GO


