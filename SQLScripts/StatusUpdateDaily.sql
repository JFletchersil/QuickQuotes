USE [msdb]
GO

/****** Object:  Job [Check Status Update Daily]    Script Date: 29/03/2018 17:00:26 ******/
EXEC msdb.dbo.sp_delete_job @job_id=N'b1b16b96-cf04-4b6c-94de-be213a1fdec6', @delete_unused_schedule=1
GO

/****** Object:  Job [Check Status Update Daily]    Script Date: 29/03/2018 17:00:26 ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]    Script Date: 29/03/2018 17:00:26 ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'Check Status Update Daily', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'Automates the status progression of events so that old events can be removed', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'BATTLESTARCON\Gmandam', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Execute SQL Statements]    Script Date: 29/03/2018 17:00:27 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Execute SQL Statements', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'UPDATE QUOTES
SET QuoteStatus = (SELECT QuoteStatuses.StatusID FROM QuoteStatuses WHERE STATE = ''QQExpiring'')
WHERE Quotes.QuoteDate <= DATEADD(mm, -2, GETDATE()) AND QuoteStatus NOT IN 
(SELECT QuoteStatuses.StatusID FROM QuoteStatuses WHERE STATE = ''QQExpired'' OR STATE = ''QQExpiring'')

UPDATE QUOTES
SET QuoteStatus = (SELECT QuoteStatuses.StatusID FROM QuoteStatuses WHERE STATE = ''QQExpired'')
WHERE Quotes.QuoteDate <= DATEADD(mm, -3, GETDATE()) AND QuoteStatus NOT IN 
(SELECT QuoteStatuses.StatusID FROM QuoteStatuses WHERE STATE = ''QQExpired'' OR STATE = ''QQExpiring'')

DELETE FROM QUOTES
WHERE Quotes.QuoteDate <= DATEADD(mm, -4, GETDATE())', 
		@database_name=N'QuoteDB', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'syspolicy_purge_history_schedule', 
		@enabled=1, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=1, 
		@freq_subday_interval=0, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=0, 
		@active_start_date=20080101, 
		@active_end_date=99991231, 
		@active_start_time=20000, 
		@active_end_time=235959, 
		@schedule_uid=N'19eb1879-4634-48dd-841b-d53df5337da6'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:
GO


