--Insert EventList_Status data
INSERT INTO [EventList_Status] VALUES ('DayShift','true','2014-08-17 08:00:00')
INSERT INTO [EventList_Status] VALUES ('NightShift','false','2014-08-16 20:00:00')

--Insert Eventlist codes
INSERT INTO [EventList_Codes] VALUES (0,'Event has not been modified since its insertion into the Event List table.')
INSERT INTO [EventList_Codes] VALUES (1,'Event has successfully been modified.')
INSERT INTO [EventList_Codes] VALUES (2,'Event has been modified but is not yet complete')
INSERT INTO [EventList_Codes] VALUES (3,'Previous event modification failed')

--Insert Event Schedule Codes
INSERT INTO [Event_ScheduleCodes] VALUES (1,'Sunday')
INSERT INTO [Event_ScheduleCodes] VALUES (2,'Monday')
INSERT INTO [Event_ScheduleCodes] VALUES (3,'Tuesday')
INSERT INTO [Event_ScheduleCodes] VALUES (4,'Wedensday')
INSERT INTO [Event_ScheduleCodes] VALUES (5,'Thursday')
INSERT INTO [Event_ScheduleCodes] VALUES (6,'Friday')
INSERT INTO [Event_ScheduleCodes] VALUES (7,'Saturday')

--Insert Event Schedule data
INSERT INTO [Event_Schedule] VALUES ('Daily')
INSERT INTO [Event_Schedule] VALUES ('Weekly')
INSERT INTO [Event_Schedule] VALUES ('Bi-Weekly')
INSERT INTO [Event_Schedule] VALUES ('Monthly')
INSERT INTO [Event_Schedule] VALUES ('Quarterly')
INSERT INTO [Event_Schedule] VALUES ('One-Time')
INSERT INTO [Event_Schedule] VALUES ('Frequent Reoccurance')