using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;
using System.IO;

    /// <summary>
    /// Provide all Database related functions
    /// </summary>
public class Database
{
    //SQL Server
    String connectionString = System.Configuration.ConfigurationManager.AppSettings["SQLConnection"].ToString();

    //This method adds an event category to the Database
    public String add_EventCategory(String CatName, String Color)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Insert_EventCategory";
            //add and set parameter
            com.Parameters.Add("@CategoryName", System.Data.SqlDbType.VarChar, 255).Value = CatName;
            com.Parameters.Add("@CategoryColor", System.Data.SqlDbType.VarChar, 7).Value = Color;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
            //Return number of rows effected
            return rows.ToString();
        }

    }

    //This method is used to add an event
    public string add_Event(String EventCategory, String Schedule, String Name,
                    Boolean FinishTime, String Doc, String URL, Boolean Active, DateTime StartTime, String SchedCode, String Comment, String Username)
    {
        //Variable to return representing event eid
        string returnEid = "";
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Insert_Events";
            //add and set parameter
            com.Parameters.Add("@EventCat", System.Data.SqlDbType.VarChar).Value = EventCategory;
            com.Parameters.Add("@Sched", System.Data.SqlDbType.VarChar).Value = Schedule;
            com.Parameters.Add("@Name", System.Data.SqlDbType.VarChar).Value = Name;
            com.Parameters.Add("@FinishTimeRequired", System.Data.SqlDbType.Bit).Value = FinishTime;
            com.Parameters.Add("@URL", System.Data.SqlDbType.VarChar).Value = URL;
            com.Parameters.Add("@Doc", System.Data.SqlDbType.VarChar).Value = Doc;
            com.Parameters.Add("@Script", System.Data.SqlDbType.VarChar).Value = "";
            com.Parameters.Add("@Active", System.Data.SqlDbType.Bit).Value = Active;
            com.Parameters.Add("@StartTime", System.Data.SqlDbType.DateTime).Value = StartTime;
            com.Parameters.Add("@ScheduleCode", System.Data.SqlDbType.VarChar).Value = SchedCode;
            com.Parameters.Add("@UserName", System.Data.SqlDbType.VarChar).Value = Username;
            com.Parameters.Add("@info", System.Data.SqlDbType.VarChar).Value = Comment;
            //Set up the return value
            SqlParameter retVal = com.Parameters.Add("@EID", System.Data.SqlDbType.Int);
            retVal.Direction = ParameterDirection.ReturnValue;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
            //Get the return value
            returnEid = (com.Parameters["@EID"].Value).ToString();
        }
        //return the variable
        return returnEid;
    }

    //This method is used to update an event
    public string add_EventLive(String EventCategory, String Name, Boolean FinishTime, String URL, DateTime StartTime, String Username)
    {
        //Variable to return representing event eid
        string returnEid = "";
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Insert_EventList_Onetime_Live";
            //add and set parameter
            com.Parameters.Add("@EventCat", System.Data.SqlDbType.VarChar).Value = EventCategory;
            com.Parameters.Add("@Name", System.Data.SqlDbType.VarChar).Value = Name;
            com.Parameters.Add("@FinishTimeRequired", System.Data.SqlDbType.Bit).Value = FinishTime;
            com.Parameters.Add("@URL", System.Data.SqlDbType.VarChar).Value = URL;
            com.Parameters.Add("@StartTime", System.Data.SqlDbType.DateTime).Value = StartTime;
            com.Parameters.Add("@UserName", System.Data.SqlDbType.VarChar).Value = Username;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
        //return the variable
        return returnEid;
    }


    //This method is used to update a script in the script table
    public void add_ScriptList(String Name, String Path,
                    Boolean Ustart, Boolean Uend)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Insert_ScriptList";
            //add and set parameter
            com.Parameters.Add("@Name", System.Data.SqlDbType.VarChar).Value = Name;
            com.Parameters.Add("@Path", System.Data.SqlDbType.VarChar).Value = Path;
            com.Parameters.Add("@UStart", System.Data.SqlDbType.Bit).Value = Ustart;
            com.Parameters.Add("@UEnd", System.Data.SqlDbType.Bit).Value = Uend;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method is used to update a script in the script table
    public int add_ScriptPer(int SCID, DateTime Start, String info, int Status)
    {
        //Variable to retunr
        int ScpRet = -1;
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Insert_ScriptPerformance";
            //add and set parameter
            com.Parameters.Add("@SCID", System.Data.SqlDbType.Int).Value = SCID;
            com.Parameters.Add("@StartTime", System.Data.SqlDbType.DateTime).Value = Start;
            com.Parameters.Add("@Info", System.Data.SqlDbType.VarChar).Value = info;
            com.Parameters.Add("@Status", System.Data.SqlDbType.Int).Value = Status;
            SqlParameter retVal = com.Parameters.Add("@SCPID", System.Data.SqlDbType.Int);
            retVal.Direction = ParameterDirection.ReturnValue;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
            //Get the return value
            ScpRet = (int)com.Parameters["@SCPID"].Value;
        }
        //Return the value
        return ScpRet;
    }

    //This method is used to update the active status of an event in the event table
    public void add_Comment(int EID, String Comment, String UserName, DateTime SchedTime, DateTime ComTime, Boolean editable)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Insert_EventComment";
            //add and set parameter
            com.Parameters.Add("@EID", System.Data.SqlDbType.Int).Value = EID;
            com.Parameters.Add("@Comment", System.Data.SqlDbType.Text).Value = Comment;
            com.Parameters.Add("@UserName", System.Data.SqlDbType.Text).Value = UserName;
            com.Parameters.Add("@SchedTime", System.Data.SqlDbType.DateTime).Value = SchedTime;
            com.Parameters.Add("@ComTime", System.Data.SqlDbType.DateTime).Value = ComTime;
            com.Parameters.Add("@Editable", System.Data.SqlDbType.Bit).Value = editable;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method selects events from the events table for managing
    public void create_Report(DateTime StartTime, DateTime EndTime)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Insert_ReportList_Create";
            //add and set parameter
            com.Parameters.Add("@StartTimeIn", System.Data.SqlDbType.DateTime).Value = StartTime;
            com.Parameters.Add("@EndTimeIn", System.Data.SqlDbType.DateTime).Value = EndTime;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method selects events from the events table for managing
    public void create_EventList(DateTime StartTime, DateTime EndTime, bool DayStat, bool NightStat)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_RollOver_EventList";
            //add and set parameter
            com.Parameters.Add("@StartTimeIn", System.Data.SqlDbType.DateTime).Value = StartTime;
            com.Parameters.Add("@EndTimeIn", System.Data.SqlDbType.DateTime).Value = EndTime;
            com.Parameters.Add("@DayShift", System.Data.SqlDbType.Bit).Value = DayStat;
            com.Parameters.Add("@NightShift", System.Data.SqlDbType.Bit).Value = NightStat;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method is used to delete an event from the event table
    public bool delete_Event(int EID, string @UserName)
    {
        //Return the row affected count
        int code;
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Delete_Events";
            //add and set parameter
            com.Parameters.Add("@EID", System.Data.SqlDbType.Int).Value = EID;
            com.Parameters.Add("@UserName", System.Data.SqlDbType.Text).Value = @UserName;
            //Add return parameter
            SqlParameter returnParam = com.Parameters.Add("returnParameter", System.Data.SqlDbType.Int);
            returnParam.Direction = ParameterDirection.ReturnValue;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
            //Get the return code
            code = int.Parse(com.Parameters["returnParameter"].Value.ToString());
        }
        //If the deletion was successful
        if (code < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //This method deletes an event category from the Database
    public void delete_EventCategory(int ECID)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Delete_EventCategory";
            //add and set parameter
            com.Parameters.Add("@ECID", System.Data.SqlDbType.Int).Value = ECID;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method deletes an event comment from the Database
    public int delete_EventComment(int CID)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Delete_EventComment";
            //add and set parameter
            com.Parameters.Add("@CID", System.Data.SqlDbType.Int).Value = CID;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
            //return number of rows edited
            return rows;
        }
    }

    //This method is used to delete a script from the script table
    public void delete_Script(int SCID)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Delete_ScriptList";
            //add and set parameter
            com.Parameters.Add("@SCID", System.Data.SqlDbType.Int).Value = SCID;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method rolls over the current event list
    public void rollover_EventList()
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_RollOver_EventList_toAudit";
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method selects events from the events table for managing
    public DataTable select_AuditReport(String Category, DateTime StartTime, DateTime EndTime)
    {
        //Datatable to return
        DataTable dt = new DataTable("Reporting");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //If the text is equal to all
            if (Category == "All") { Category = "*"; }
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_AuditList";
            //add and set parameter
            com.Parameters.Add("@Category", System.Data.SqlDbType.VarChar).Value = Category;
            com.Parameters.Add("@StartTimeIn", System.Data.SqlDbType.DateTime).Value = StartTime;
            com.Parameters.Add("@EndTimeIn", System.Data.SqlDbType.DateTime).Value = EndTime;
            //Assign the connection to the command
            com.Connection = conn;
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt);
        }
        //Return the list of categories
        return dt;
    }

    //This method gets the event categories from the Database
    public List<Category> select_EventCategory()
    {
        //List to return
        List<Category> cats = new List<Category>();
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_EventCategory";

            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            SqlDataReader Reader = com.ExecuteReader();
            //While the reader has results
            if (Reader.HasRows)
            {
                while (Reader.Read())
                {
                    //Add a new category to the list
                    cats.Add(new Category { ECID = (int)Reader.GetValue(0), Name = (String)Reader.GetValue(1) });
                }
            }
        }
        //Return the list of categories
        return cats;
    }

    //This method gets the event categories from the Database
    public DataTable select_EventCategoryTable()
    {
        //Datatable to return
        DataTable dt = new DataTable("Categories");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_EventCategory";
            //Assign the connection to the command
            com.Connection = conn;
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt); 
        }
        //return the datatable 
        return dt;
    }

    //This method selects events from the event list table
    public DataTable select_EventList()
    {
        //Datatable to return
        DataTable dt = new DataTable("EventList");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_EventList";
            //add and set parameter
            //Assign the connection to the command
            com.Connection = conn;
            //open the connection
            conn.Open();
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt);
        }
        //Return the list of categories
        return dt;
    }

    //This method is used to select counts from the current event list
    public string select_EventListCount()
    {
        //Variable to return representing event eid
        string counts = "";
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_EventListCounts";
            //Set up the return value
            SqlParameter retAll = com.Parameters.Add("@All", System.Data.SqlDbType.Int);
            SqlParameter retUnt = com.Parameters.Add("@Unt", System.Data.SqlDbType.Int);
            SqlParameter retCom = com.Parameters.Add("@Com", System.Data.SqlDbType.Int);
            SqlParameter retInc = com.Parameters.Add("@Inc", System.Data.SqlDbType.Int);
            SqlParameter retErr = com.Parameters.Add("@Err", System.Data.SqlDbType.Int);
            SqlParameter retFor = com.Parameters.Add("@For", System.Data.SqlDbType.Int);
            retAll.Direction = ParameterDirection.Output;
            retUnt.Direction = ParameterDirection.Output;
            retInc.Direction = ParameterDirection.Output;
            retCom.Direction = ParameterDirection.Output;
            retErr.Direction = ParameterDirection.Output;
            retFor.Direction = ParameterDirection.Output;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
            //Get the return value
            counts = ((com.Parameters["@All"].Value).ToString() + "," + (com.Parameters["@Unt"].Value).ToString() + "," + (com.Parameters["@Com"].Value).ToString() + "," +
                        (com.Parameters["@Inc"].Value).ToString() + "," + (com.Parameters["@Err"].Value).ToString() + "," + (com.Parameters["@For"].Value).ToString());
        }
        //return the variable
        return counts;
    }

    //This method gets the event schedules from the Database
    public List<Schedule> select_EventSchedule()
    {
        //List to return
        List<Schedule> sched = new List<Schedule>();
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_EventSchedule";
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            SqlDataReader Reader = com.ExecuteReader();
            //While the reader has results
            if (Reader.HasRows)
            {
                while (Reader.Read())
                {
                    //Add a new category to the list
                    sched.Add(new Schedule { SID = (int)Reader.GetValue(0), Name = (String)Reader.GetValue(1) });
                }
            }
        }
        //Return the list of categories
        return sched;
    }

    //This method select the active status of an event in the event list
    public string select_EventListActiveStatus()
    {
        //String to return
        String returnDate = "";
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_EventListActiveStatus";
            //Add Output variables
            SqlParameter retShift = com.Parameters.Add("@Shift", System.Data.SqlDbType.VarChar,64);
            SqlParameter retDate = com.Parameters.Add("@LastCreated", System.Data.SqlDbType.VarChar,64);
            retShift.Direction = ParameterDirection.Output;
            retDate.Direction = ParameterDirection.Output;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
            //Get the return value
            returnDate = ((com.Parameters["@Shift"].Value).ToString() + "," + (com.Parameters["@LastCreated"].Value).ToString());
        }
        //Return the list of categories
        return returnDate;
    }

    //This method selects non completed events 
    public DataTable select_EventListForward()
    {
        //Datatable to return
        DataTable dt = new DataTable("EventListForward");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_EventListForward";
            //add and set parameter
            //Assign the connection to the command
            com.Connection = conn;
            //open the connection
            conn.Open();
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt);
        }
        //Return the list of categories
        return dt;
    }

    //This method select the active status of an event in the event list
    public DataTable select_EventListStatus()
    {
        //Datatable to return
        DataTable dt = new DataTable("EventListStatus");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_EventListStatus";
            //add and set parameter
            //Assign the connection to the command
            com.Connection = conn;
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt);
        }
        //Return the list of categories
        return dt;
    }

    //This method select the active status of an event in the event list
    public DataTable select_EventListCreated()
    {
        //Datatable to return
        DataTable dt = new DataTable("EventListCreated");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_EventListCreated";
            //add and set parameter
            //Assign the connection to the command
            com.Connection = conn;
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt);
        }
        //Return the list of categories
        return dt;
    }

    //This method selects the complete status of an event in the event list
    public Boolean select_EventListComplete(int ELID)
    {
        //Boolean value to return
        Boolean returnBool = false;
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_EventComplete";
            //add and set parameter
            com.Parameters.Add("@ELID", System.Data.SqlDbType.Int).Value = ELID;
            //Assign the connection to the command
            com.Connection = conn;
            //Set up the return value
            SqlParameter retVal = new SqlParameter();
            retVal.Direction = ParameterDirection.ReturnValue;
            com.Parameters.Add(retVal);
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
            //Get the return value
            int ret = int.Parse(retVal.Value.ToString());
            //if it is a 1
            if (ret == 1) { returnBool = true; }
        }
        //Return the list of categories
        return returnBool;
    }

    //This method selects events from the events table for managing
    public DataTable select_EventAudit(int Count, int EID, DateTime StarTime, DateTime EndTime)
    {
        //Datatable to return
        DataTable dt = new DataTable("Audit");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_EventAudit";
            //add and set parameter
            com.Parameters.Add("@Count", System.Data.SqlDbType.Int).Value = Count;
            com.Parameters.Add("@EID", System.Data.SqlDbType.Int).Value = EID;
            com.Parameters.Add("@StartTime", System.Data.SqlDbType.DateTime).Value = StarTime;
            com.Parameters.Add("@EndTime", System.Data.SqlDbType.DateTime).Value = EndTime;
            //Assign the connection to the command
            com.Connection = conn;
            //open the connection
            conn.Open();
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt);
        }
        //Return the list of categories
        return dt;
    }

    //This method selects comment information from the comment table
    public DataTable select_Comment(int Count, int EID, DateTime StarTime, DateTime EndTime, Boolean Comment)
    {
        //Datatable to return
        DataTable dt = new DataTable("Comment");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_Comment";
            //add and set parameter
            com.Parameters.Add("@Count", System.Data.SqlDbType.Int).Value = Count;
            com.Parameters.Add("@EID", System.Data.SqlDbType.Int).Value = EID;
            com.Parameters.Add("@StartTime", System.Data.SqlDbType.DateTime).Value = StarTime;
            com.Parameters.Add("@EndTime", System.Data.SqlDbType.DateTime).Value = EndTime;
            com.Parameters.Add("@CommentTime", System.Data.SqlDbType.Bit).Value = Comment;
            //Assign the connection to the command
            com.Connection = conn;
            //open the connection
            conn.Open();
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt);
        }
        //Return the list of categories
        return dt;
    }

    //This method selects event performance data from the performance table for datagriding
    public DataTable select_EventPerformance(int Count, int EID, DateTime StarTime, DateTime EndTime)
    {
        //Datatable to return
        DataTable dt = new DataTable("Performance");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_Performance";
            //add and set parameter
            com.Parameters.Add("@Count", System.Data.SqlDbType.Int).Value = Count;
            com.Parameters.Add("@EID", System.Data.SqlDbType.Int).Value = EID;
            com.Parameters.Add("@StartTime", System.Data.SqlDbType.DateTime).Value = StarTime;
            com.Parameters.Add("@EndTime", System.Data.SqlDbType.DateTime).Value = EndTime;
            //Assign the connection to the command
            com.Connection = conn;
            //open the connection
            conn.Open();
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt);
        }
        //Return the list of categories
        return dt;
    }

    //This method selects event performance data from the performance table for datagraphing
    public DataTable select_EventPerformanceGraph(int Count, int EID)
    {
        //Datatable to return
        DataTable dt = new DataTable("PerformanceGraph");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_PerformanceGraph";
            //add and set parameter
            com.Parameters.Add("@Count", System.Data.SqlDbType.Int).Value = Count;
            com.Parameters.Add("@EID", System.Data.SqlDbType.Int).Value = EID;
            //Assign the connection to the command
            com.Connection = conn;
            //open the connection
            conn.Open();
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt);
        }
        //Return the list of categories
        return dt;
    }

    //This method selects events from the events table for managing
    public DataTable select_ScriptPerformance(DateTime StartTime, DateTime EndTime, int SCID)
    {
        //Datatable to return
        DataTable dt = new DataTable("ScriptPerformance");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_ScriptPerformance";
            //add and set parameter
            com.Parameters.Add("@StartDate", System.Data.SqlDbType.DateTime).Value = StartTime;
            com.Parameters.Add("@EndTime", System.Data.SqlDbType.DateTime).Value = EndTime;
            com.Parameters.Add("@SCID", System.Data.SqlDbType.Int).Value = SCID;
            //Assign the connection to the command
            com.Connection = conn;
            //open the connection
            conn.Open();
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt);
        }
        //Return the list of categories
        return dt;
    }

    //This method selects events from the events table for managing
    public DataTable select_ScriptPerformanceSingle(int SCID)
    {
        //Datatable to return
        DataTable dt = new DataTable("ScriptPerformance");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_ScriptPerformanceSingle";
            //add and set parameter
            com.Parameters.Add("@SCID", System.Data.SqlDbType.Int).Value = SCID;
            //Assign the connection to the command
            com.Connection = conn;
            //open the connection
            conn.Open();
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt);
        }
        //Return the list of categories
        return dt;
    }

    //This method select the active status of an event in the event list
    public Boolean select_ActiveStatusEventList(int ELID)
    {
        //Boolean value to return
        Boolean returnBool = false;
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_ActiveEventList";
            //add and set parameter
            com.Parameters.Add("@ELID", System.Data.SqlDbType.Int).Value = ELID;
            //Assign the connection to the command
            com.Connection = conn;
            //Set up the return value
            SqlParameter retVal = new SqlParameter();
            retVal.Direction = ParameterDirection.ReturnValue;
            com.Parameters.Add(retVal);
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
            //Get the return value
            int ret = int.Parse(retVal.Value.ToString());
            //if it is a 1
            if (ret == 1) { returnBool = true; }
        }    
        //Return the list of categories
        return returnBool;
    }

    //This method gets an event and its information for the provided eid
    public Event select_EventEdit(int EID)
    {
        //List to return
        Event eventM = new Event();
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_Event";
            //add and set parameter
            com.Parameters.Add("@EID", System.Data.SqlDbType.Int).Value = EID;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            SqlDataReader Reader = com.ExecuteReader();
            //While the reader has results
            if (Reader.HasRows)
            {
                //Get the row
                Reader.Read();
                //Add the appropriate values to the event object
                eventM.EID = Reader.GetInt32(0);
                eventM.Name = Reader.GetString(1);
                eventM.Documentation = Reader.GetValue(2).ToString();
                eventM.URL = Reader.GetValue(9).ToString();
                eventM.Schedule = Reader.GetString(3);
                eventM.StartTime = Reader.GetDateTime(4);
                eventM.ScheduleCode = Reader.GetValue(5).ToString();
                eventM.FinishTime = Reader.GetBoolean(6);
                eventM.Active = Reader.GetBoolean(7);
                eventM.Category = Reader.GetString(8);
            }
        }
        //Return the list of categories
        return eventM;
    }

    //This method selects events from the events table for managing
    public DataTable select_Management(String Category, String Schedule)
    {
        //Datatable to return
        DataTable dt = new DataTable("Event_Management");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //If the text is equal to all
            if (Category == "All") { Category = "*"; }
            //If the text is equal to all
            if (Schedule == "All") { Schedule = "*"; }
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_EventManagement";
            //add and set parameter
            com.Parameters.Add("@Category", System.Data.SqlDbType.VarChar).Value = Category;
            com.Parameters.Add("@Schedule", System.Data.SqlDbType.VarChar).Value = Schedule;
            //Assign the connection to the command
            com.Connection = conn;
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt);
        }
        //return the datatabel
        return dt;
    }

    //This method selects events from the events table for reporting
    public DataTable select_Reporting(String Category)
    {
        //Datatable to return
        DataTable dt = new DataTable("Reporting");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //If the text is equal to all
            if (Category == "All") { Category = "*"; }
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_ReportList";
            //add and set parameter
            com.Parameters.Add("@Category", System.Data.SqlDbType.VarChar).Value = Category;
            //Assign the connection to the command
            com.Connection = conn;
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt);
        }
        //Return the list of categories
        return dt;
    }

    //This method selects events from the events table for managing
    public DataTable select_ScriptList()
    {
        //Datatable to return
        DataTable dt = new DataTable("ScriptList");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Select_ScriptList";
            //add and set parameter
            //Assign the connection to the command
            com.Connection = conn;
            //open the connection
            conn.Open();
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt);
        }
        //Return the list of categories
        return dt;
    }

    //This method selects information from the passed in table
    public DataTable select_Table(String Table, String Cond)
    {
        //Datatable to return
        DataTable dt = new DataTable("EventList");
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            //New command object
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.Text;
            //If the Cond value exists
            if (Cond.Length > 0)
            {
                //Set the stored procedure to run
                com.CommandText = ("SELECT * FROM dbo." + Table + Cond);
            }
            else
            {
                //Set the stored procedure to run
                com.CommandText = ("SELECT * FROM dbo." + Table);
            }
            //Assign the connection to the command
            com.Connection = conn;
            //open the connection
            conn.Open();
            //Sql data adapter
            SqlDataAdapter adapt = new SqlDataAdapter();
            //ASsign the select command to the dataadapter
            adapt.SelectCommand = com;
            //Fill the data table
            adapt.Fill(dt);
        }
        //Return the list of categories
        return dt;
    }

    //This method is used to update the active status of an event in the event table
    public void update_ActiveStatus(int EID, Boolean Status)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            if (Status == true) { Status = false; }
            else { Status = true; }
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Update_Events_Active";
            //add and set parameter
            com.Parameters.Add("@EID", System.Data.SqlDbType.Int).Value = EID;
            com.Parameters.Add("@Active", System.Data.SqlDbType.Bit).Value = Status;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method is used to update the active status of an event in the event list
    public void update_ActiveStatusEventList(int ELID, Boolean Status)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Update_EventsList_Active";
            //add and set parameter
            com.Parameters.Add("@ELID", System.Data.SqlDbType.Int).Value = ELID;
            com.Parameters.Add("@Active", System.Data.SqlDbType.Bit).Value = Status;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method is used to update the variables in the comment table
    public void update_Category(int ECID, String CatName, String CatColor)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Update_EventCategory";
            //add and set parameter
            com.Parameters.Add("@ECID", System.Data.SqlDbType.Int).Value = ECID;
            com.Parameters.Add("@CatName", System.Data.SqlDbType.VarChar).Value = CatName;
            com.Parameters.Add("@CatColor", System.Data.SqlDbType.VarChar).Value = CatColor;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method is used to update the variables in the comment table
    public void update_Comment(int CID, String Comment, String UserName, DateTime SchedTime, DateTime ComTime, bool editable)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Update_EventComment";
            //add and set parameter
            com.Parameters.Add("@CID", System.Data.SqlDbType.Int).Value = CID;
            com.Parameters.Add("@Comment", System.Data.SqlDbType.VarChar).Value = Comment;
            com.Parameters.Add("@UserName", System.Data.SqlDbType.VarChar).Value = UserName;
            com.Parameters.Add("@SchedTime", System.Data.SqlDbType.DateTime).Value = SchedTime;
            com.Parameters.Add("@ComTime", System.Data.SqlDbType.DateTime).Value = ComTime;
            com.Parameters.Add("@Editable", System.Data.SqlDbType.Bit).Value = editable;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method is used to update the variables in the comment table
    public void update_Comment_Editable(int CID, bool editable)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Update_Comment_Editable";
            //add and set parameter
            com.Parameters.Add("@CID", System.Data.SqlDbType.Int).Value = CID;
            com.Parameters.Add("@Editable", System.Data.SqlDbType.Bit).Value = editable;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method is used to update an event in the event table
    public void update_Event(int EID, String EventCategory, String Schedule, String Name,
                    Boolean FinishTime, String Doc, String URL, Boolean Active, DateTime StartTime, String SchedCode, String Comment, String Username)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Update_Events";
            //add and set parameter
            com.Parameters.Add("@EID", System.Data.SqlDbType.Int).Value = EID;
            com.Parameters.Add("@EventCat", System.Data.SqlDbType.VarChar).Value = EventCategory;
            com.Parameters.Add("@Sched", System.Data.SqlDbType.VarChar).Value = Schedule;
            com.Parameters.Add("@Name", System.Data.SqlDbType.VarChar).Value = Name;
            com.Parameters.Add("@FinishTimeRequired", System.Data.SqlDbType.Bit).Value = FinishTime;
            com.Parameters.Add("@URL", System.Data.SqlDbType.VarChar).Value = URL;
            com.Parameters.Add("@Doc", System.Data.SqlDbType.VarChar).Value = Doc;
            com.Parameters.Add("@Script", System.Data.SqlDbType.VarChar).Value = "";
            com.Parameters.Add("@Active", System.Data.SqlDbType.Bit).Value = Active;
            com.Parameters.Add("@StartTime", System.Data.SqlDbType.DateTime).Value = StartTime;
            com.Parameters.Add("@ScheduleCode", System.Data.SqlDbType.VarChar).Value = SchedCode;
            com.Parameters.Add("@UserName", System.Data.SqlDbType.VarChar).Value = Username;
            com.Parameters.Add("@info", System.Data.SqlDbType.VarChar).Value = Comment;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method is used to update an event in the event table        //Create a new SQL connection using the connection 
    public void update_EventList(int ELID, Boolean Complete, DateTime EndTime, 
                    Boolean Enabled, DateTime StartTime, int StatusCode, String StatusInfo, String UserName)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Update_EventList";
            //add and set parameter
            com.Parameters.Add("@ELID", System.Data.SqlDbType.Int).Value = ELID;
            com.Parameters.Add("@Complete", System.Data.SqlDbType.Bit).Value = Complete;
            com.Parameters.Add("@EndTime", System.Data.SqlDbType.DateTime).Value = EndTime;
            com.Parameters.Add("@Active", System.Data.SqlDbType.Bit).Value = Enabled;
            com.Parameters.Add("@StartTime", System.Data.SqlDbType.DateTime).Value = StartTime;
            com.Parameters.Add("@StatusCode", System.Data.SqlDbType.Int).Value = StatusCode;
            com.Parameters.Add("@StatusInfo", System.Data.SqlDbType.VarChar).Value = StatusInfo;
            com.Parameters.Add("@UserName", System.Data.SqlDbType.VarChar).Value = UserName;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method is used to update a script in the script table
    public void update_ScriptList(int SCID, String Name, String Path,
                    Boolean Ustart, Boolean Uend)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Update_ScriptList";
            //add and set parameter
            com.Parameters.Add("@SCID", System.Data.SqlDbType.Int).Value = SCID;
            com.Parameters.Add("@Name", System.Data.SqlDbType.VarChar).Value = Name;
            com.Parameters.Add("@Path", System.Data.SqlDbType.VarChar).Value = Path;
            com.Parameters.Add("@UStart", System.Data.SqlDbType.Bit).Value = Ustart;
            com.Parameters.Add("@UEnd", System.Data.SqlDbType.Bit).Value = Uend;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method is used to update a script in the script table
    public void update_ScriptPer(int SCPID, DateTime end, String info, int Status)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Update_ScriptPerformance";
            //add and set parameter
            com.Parameters.Add("@SCPID", System.Data.SqlDbType.Int).Value = SCPID;
            com.Parameters.Add("@EndTime", System.Data.SqlDbType.DateTime).Value = end;
            com.Parameters.Add("@Info", System.Data.SqlDbType.VarChar).Value = info;
            com.Parameters.Add("@Status", System.Data.SqlDbType.Int).Value = Status;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }

    //This method is used to update the active status of an event in the event table
    public void update_StatusInfo(int ELID, String Status, int code)
    {
        //Create a new SQL connection using the connection string
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand com = new SqlCommand();
            //Set command type to stored procedure
            com.CommandType = System.Data.CommandType.StoredProcedure;
            //Set the stored procedure to run
            com.CommandText = "_sp_Update_EventsList_StatusInfo";
            //add and set parameter
            com.Parameters.Add("@ELID", System.Data.SqlDbType.Int).Value = ELID;
            com.Parameters.Add("@StatusInfo", System.Data.SqlDbType.VarChar).Value = Status;
            com.Parameters.Add("@StatusCode", System.Data.SqlDbType.Int).Value = code;
            //Assign the connection to the command
            com.Connection = conn;
            //Open Connection
            conn.Open();
            //Execute the command
            int rows = com.ExecuteNonQuery();
        }
    }
}

//This class is used to create a category object
public class Category
{
    public int ECID { get; set; }
    public string Name { get; set; }
}

//This class is used to create a schedule object
public class Schedule
{
    public int SID { get; set; }
    public string Name { get; set; }
}

//This class is used to create an event object 
public class Event
{
    public int EID { get; set; }
    public string Name { get; set; }
    public string Documentation { get; set; }
    public string URL { get; set; }
    public string Schedule { get; set; }
    public string ScheduleCode { get; set; }
    public bool Active { get; set; }
    public bool FinishTime { get; set; }
    public DateTime StartTime { get; set; }
    public string Category { get; set; }
}