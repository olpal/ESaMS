//The Event Scheduling and Management System (ESaMS) is designed
//to manage, schedule, display and track a repository of events.
//Copyright (C) 2014 AJ

//This program is free software: you can redistribute it and/or modify it
//under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
//See the GNU General Public License for more details. 

//You should have received a copy of the GNU General Public License
//along with this program. If not, see http://www.gnu.org/licenses/

using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Threading;
using System.Globalization;
using System.Runtime.Remoting.Messaging;

public partial class Views_EventList : System.Web.UI.Page
{
//**Region - Page Variables**
    //Create a new database object
    Database Database = new Database();   
    //Variable representing number of rows updated
    private int rowsUpdated;


//**Region - Page Related Methods**
    //Occurs when the page is loading
    protected void Page_init(object sender, EventArgs e)
    {
        //Load master file variables
        loadMasterInfo();
    }

    //Occurs when the page loads
    protected void Page_Load(object sender, EventArgs e)
    {  
        //If the load is not a postback
        if (!Page.IsPostBack)
        {          
            //Populate the shift title
            populateTitle();
            //Populate the Main table
            populateGrid();
            //Populate list boxes on event add panel
            load_ListBoxes();
        }
    }

    //This method handles errors when they occur
    public void Page_Error(object sender, EventArgs e)
    {
        Exception objErr = Server.GetLastError().GetBaseException();
        string err = "<b>Error Caught in Page_Error event</b><hr><br>" +
                "<br><b>Error in: </b>" + Request.Url.ToString() +
                "<br><b>Error Message: </b>" + objErr.Message.ToString() +
                "<br><b>Stack Trace:</b><br>";
        if (objErr.StackTrace != null)
        {
            //Add the stack trace
            err += objErr.StackTrace.ToString();
        }
        Response.Write(err.ToString());
        Server.ClearError();
    }


//**Region - Data Processing Methods**

    //This method selects and binds comment data to the comment gridview
    private void bindData(int count)
    {
        //Get the eid from the hidden field
        int EID = int.Parse(CEIDH.Value);
        //Get the datatable
        DataTable dt = Database.select_Comment(count, EID, new DateTime(1900, 1, 1), new DateTime(1900, 1, 1), false);
        //Create the data table
        ComGrid.DataSource = dt;
        //Set the event title
        ComEventTitle.Text = ComEventTitleH.Value;
        //Bind the data
        ComGrid.DataBind();
    }

    //This method is used to enable the right timer depending on the browser

    //This method is used to clear the components on the One Time event adding panel
    private void clearPanel()
    {
        //Reset text boxes
        EventTitle.Text = "";
        URLTextBox.Text = "";
        DateBox.Text = "";
        //Reset Dropdown lists
        CategoryList.SelectedIndex = 0;
        MinList.SelectedIndex = 0;
        HourList.SelectedIndex = 0;
        //Reset Checkboxes
        FinishTime.Checked = false;
        //Reset Labels
        ResponseName.Text = "";
        ResponseDate.Text = "";
    }

    //This method is used to check if a comment has been created in the prior time frame
    private int CommentCheck(int elid)
    {
        //Get comments in last 60 minutes
        DataTable dt = Database.select_Comment(0, elid, System.DateTime.Now.AddMinutes(-60), System.DateTime.Now.AddMinutes(1), true);
        //If there is a row in the datatable
        if (dt.Rows.Count > 0) { return dt.Rows[0].Field<int>("CID"); }
        else{ return -1; }
    }

    //This method creates the controls for the masterpage
    private ImageButton[] createControls()
    {
        //Get the browser
        string browse = Request.Browser.Browser;
        //Get the version
        int ver = Request.Browser.MajorVersion;
        //BUtton array to return
        ImageButton[] returnArray = new ImageButton[4];
        //create buttons to add
        ImageButton saveIB = new ImageButton();
        ImageButton newIB = new ImageButton();
        ImageButton eventIB = new ImageButton();
        ImageButton forIB = new ImageButton();
        //set the button Css class
        saveIB.CssClass = "LowerControlButtons";
        newIB.CssClass = "LowerControlButtons";
        eventIB.CssClass = "LowerControlButtons";
        forIB.CssClass = "LowerControlButtons";
        //Set the buttons ID
        saveIB.ID = "EnterButton";
        newIB.ID = "NewImageB";
        eventIB.ID = "EventImageB";
        forIB.ID = "ForImageB";
        //Set the buttons tooltip
        saveIB.ToolTip = "Save the current Shift's Event List";
        newIB.ToolTip = "Create a new Shift Event List";
        eventIB.ToolTip = "Add a one time event to the current Shift Event List";
        forIB.ToolTip = "View Events that will forward to the next Shift Event List";
        //Set the buttons imageurl
        saveIB.ImageUrl = "~/Images/Save.gif";
        newIB.ImageUrl = "~/Images/CreateNew.gif";
        eventIB.ImageUrl = "~/Images/Plus.gif";
        forIB.ImageUrl = "~/Images/Forevents.gif";
        //set image buttons event handler    
        newIB.Click += new ImageClickEventHandler(create_EventList);
        eventIB.OnClientClick = "InfoSetup(this)";
        forIB.OnClientClick = "InfoSetup(this)";
        //Add post back url to save button
        saveIB.PostBackUrl = "~/Views/EventList.aspx";
        //assign synchronous postback event handles to save button and timer
        saveIB.Click += new ImageClickEventHandler(SaveB_Click); 
        //Add buttons to the array
        returnArray[0] = forIB;
        returnArray[1] = eventIB;
        returnArray[2] = saveIB;
        returnArray[3] = newIB;
        //return the array
        return returnArray;
    }

    //This method creates the modal popup extenders for the master page
    private AjaxControlToolkit.ModalPopupExtender[] createExtenders()
    {
        //BUtton array to return
        AjaxControlToolkit.ModalPopupExtender[] returnArray = new AjaxControlToolkit.ModalPopupExtender[2];
        //create modalpopupextenders
        AjaxControlToolkit.ModalPopupExtender EveTPop = new AjaxControlToolkit.ModalPopupExtender();
        AjaxControlToolkit.ModalPopupExtender ForPop = new AjaxControlToolkit.ModalPopupExtender();
        //set the button Css class
        EveTPop.BackgroundCssClass = "ModelBack";
        ForPop.BackgroundCssClass = "ModelBack";
        //Set the extender ID
        EveTPop.ID = "EveTPop";
        ForPop.ID = "ForPop";
        //Set the extenders target control id
        EveTPop.TargetControlID = "EventImageB";
        ForPop.TargetControlID = "ForImageB";
        //Set the extenders popup control id
        EveTPop.PopupControlID = "masterMain_EventPan";
        ForPop.PopupControlID = "masterMain_ForPanel";
        //Add extender to the array
        returnArray[0] = EveTPop;
        returnArray[1] = ForPop;
        //return the array
        return returnArray;
    }

    //This method creates the status objects for the masterpage
    private WebControl[] createStatus()
    {
        
        //BUtton array to return
        WebControl[] returnArray = new WebControl[12];
        //Create status images        
        Image AllStat = new Image();
        Image ForStat = new Image();
        Image UntStat = new Image();
        Image ComStat = new Image();
        Image IncStat = new Image();
        Image ErrStat = new Image();
        //set status image ids
        AllStat.ID = "AllStat";
        ForStat.ID = "ForStat";
        UntStat.ID = "UntStat";
        ComStat.ID = "ComStat";
        IncStat.ID = "IncStat";
        ErrStat.ID = "ErrStat";
        //set status image urls
        AllStat.ImageUrl = "~/Images/Allevents.gif";
        ForStat.ImageUrl = "~/Images/Forevents.gif";
        UntStat.ImageUrl = "~/Images/Untouch.gif";
        ComStat.ImageUrl = "~/Images/Success.gif";
        IncStat.ImageUrl = "~/Images/Partial.gif";
        ErrStat.ImageUrl = "~/Images/Error.gif";
        //set status image tooltips
        AllStat.ToolTip = "Count of all events";
        ForStat.ToolTip = "Count of events that were forwarded to this list";
        UntStat.ToolTip = "Count of unaltered events";
        ComStat.ToolTip = "Count of completed events";
        IncStat.ToolTip = "Count of incomplete events";
        ErrStat.ToolTip = "Count of error events";
        //Set the css class
        AllStat.CssClass = "LowerControlStatus";
        ForStat.CssClass = "LowerControlStatus";
        UntStat.CssClass = "LowerControlStatus";
        ComStat.CssClass = "LowerControlStatus";
        IncStat.CssClass = "LowerControlStatus";
        ErrStat.CssClass = "LowerControlStatus";
        //Create status images        
        Label AllT = new Label();
        Label ForT = new Label();
        Label UntT = new Label();
        Label ComT = new Label();
        Label IncT = new Label();
        Label ErrT = new Label();
        //set status image ids
        AllT.ID = "AllT";
        ForT.ID = "ForT";
        UntT.ID = "UntT";
        ComT.ID = "ComT";
        IncT.ID = "IncT";
        ErrT.ID = "ErrT";
        //Set the css class
        AllT.CssClass = "StatusTextCount";
        ForT.CssClass = "StatusTextCount";
        UntT.CssClass = "StatusTextCount";
        ComT.CssClass = "StatusTextCount";
        IncT.CssClass = "StatusTextCount";
        ErrT.CssClass = "StatusTextCount";
        //addcomponents in the correct order
        returnArray[0] = AllStat;
        returnArray[1] = AllT;
        returnArray[2] = ForStat;
        returnArray[3] = ForT;
        returnArray[4] = UntStat;
        returnArray[5] = UntT;
        returnArray[6] = ComStat;
        returnArray[7] = ComT;
        returnArray[8] = IncStat;
        returnArray[9] = IncT;
        returnArray[10] = ErrStat;
        returnArray[11] = ErrT;
        //return the array
        return returnArray;
    }

    //This method deletes a comment from the comment table
    private int deleteComment(int CID)
    {
        //Call the update method
        int rowDel = Database.delete_EventComment(CID);
        //If the row was deleted
        if (rowDel > 0) { updateStatus(System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + " - Comment " + CID + " has been deleted"); }
        else { updateStatus(System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + " - Comment " + CID + " FAILED to be deleted."); }
        //return the rows deleted
        return rowDel;
    }

    //Used to control edit mode on the comment grid
    private void editMode(Boolean edit, int index)
    {
        //Get the datarow in question
        GridViewRow dataRow = ComGrid.Rows[index];
        //Get relevant link buttons
        LinkButton ELB = (LinkButton)dataRow.FindControl("EditLB");
        LinkButton ULB = (LinkButton)dataRow.FindControl("UpdateLB");
        LinkButton CLB = (LinkButton)dataRow.FindControl("CancelLB");
        LinkButton DLB = (LinkButton)dataRow.FindControl("DeleteLB");
        TextBox ComBox = (TextBox)dataRow.FindControl("CommentBox");
        //If edit is true
        if (edit == true)
        {
            //Set the relevant link buttons
            ELB.Visible = false;
            ULB.Visible = true;
            CLB.Visible = true;
            DLB.Visible = false;
            InsertLB.Visible = false;
            ComBox.ReadOnly = false;
            ComBox.Width = 350;
            ComBox.BackColor = System.Drawing.ColorTranslator.FromHtml("#FEE2E2");
            ComBox.BorderStyle = BorderStyle.Solid;
            ComBox.BorderColor = System.Drawing.ColorTranslator.FromHtml("#7C0604");
            ComBox.BorderWidth = Unit.Parse("1");
        }
        else
        {
            //Set the relevant link buttons
            ELB.Visible = true;
            ULB.Visible = false;
            CLB.Visible = false;
            DLB.Visible = true;
            InsertLB.Visible = true;
            ComBox.ReadOnly = true;
        }
    }

    //Populate the drop down lists associated with the schedule with days and times
    private void load_ListBoxes()
    {
        //Numbers for hours
        String[] hours = { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", 
                         "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23"};
        //Numbers for minutes
        String[] minutes = { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", 
                         "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" , "24", "25", "26", 
                         "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", 
                         "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54",
                         "55", "56", "57", "58", "59" };
        //Set the hour source
        HourList.DataSource = hours;
        //set the minute source
        MinList.DataSource = minutes;
        //Bind the data sources
        HourList.DataBind();
        MinList.DataBind();
        //Get the categories from the Database
        List<Category> Cats = Database.select_EventCategory();
        //Add the names of the categories to the list box
        foreach (Category c in Cats)
        {
            //Add the value to the array
            CategoryList.Items.Add(c.Name);
        }
    }

    //This method sets masterpage variables
    private void loadMasterInfo()
    {
        //Set the oage Title
        Page.Title = "Current Shift View";
        //Set the header text
        ((Label)Master.FindControl("mHeaderLabel")).Text = "Current Shift View";
        //Get the lower right control panel from the master page
        Panel lowerRight = (Panel)Master.FindControl("masterLowerControlPR");
        //Get the lower left control panel from the master page
        Panel lowerLeft = (Panel)Master.FindControl("masterLowerControlPL");
        //get the buttons to add
        ImageButton[] toAdd = createControls();
        //foreach buttton
        foreach (ImageButton adding in toAdd)
        {
            //Add the button to the panel
            lowerRight.Controls.Add(adding);
        }
        //get the buttons to add
        WebControl[] toAddStat = createStatus();
        //foreach buttton
        foreach (WebControl adding in toAddStat)
        {
            //Add the button to the panel
            lowerLeft.Controls.Add(adding);
        }
        //get the buttons to add
        AjaxControlToolkit.ModalPopupExtender[] toAddExt = createExtenders();
        //foreach buttton
        foreach (AjaxControlToolkit.ModalPopupExtender adding in toAddExt)
        {
            //Add the button to the panel
            lowerRight.Controls.Add(adding);
        } 
    }

    //Populates the main table with data from the SQL Database
    private void populateGrid()
    {
        //Set the data source
        ManageGrid.DataSource = Database.select_EventList();
        //Update the event counts
        updateEventCount();
        //Bind the datatable to the grid
        ManageGrid.DataBind();
        //Call the UpdatePanels update method
        GridViewPanel.Update();
    }

    //Populates the main table with data from the SQL Database
    private void populateGridForward()
    {
        //New datatable object
        DataTable dt = Database.select_EventListForward();
        //Update the number of events
        ForEventsNum.Text = dt.Rows.Count.ToString();
        //Set the data source
        ForGrid.DataSource = dt;
        //Bind the datatable to the grid
        ForGrid.DataBind();
        //Call the UpdatePanels update method
        ForwardData.Update();
    }

    //This function loads the name and date of the current shift
    private void populateTitle()
    {
        //Get the Data from the eventlist status table
        string[] shiftData = ((string)Database.select_EventListActiveStatus()).Split(char.Parse(","));
        //Get the shiftname
        String shift = shiftData[0];
        //Get the Shift Date
        DateTime shiftDate = DateTime.Parse(shiftData[1].ToString());
        //Set the shift label name
        PageName.Text = shift;
        //Set the shift label date
        PageDate.Text = shiftDate.ToString(" - dddd MMM dd yyyy HH:mm");
        //Set the current shift
        CurrentShift.Value = shiftData[1].ToString();
    }

    //This method takes stakes start date/time and end date/time as strings, validates if they are valid
    //and returns an array containing a start datetime, end datetime, status int, and ,message string
    private object[] processTimes(String startTime, String startDate, String endTime, String endDate)
    {
        //Regex expressions for testing
        Regex timeTest = new Regex("\\d\\d\\:\\d\\d");
        Regex dateTest1 = new Regex("\\d\\d\\/\\d\\d\\/\\d\\d\\d\\d");
        Regex dateTest2 = new Regex("\\d\\/\\d\\d\\/\\d\\d\\d\\d");
        Regex dateTest3 = new Regex("\\d\\d\\/\\d\\/\\d\\d\\d\\d");
        Regex dateTest4 = new Regex("\\d\\/\\d\\/\\d\\d\\d\\d");
        //Datetime objects to return
        DateTime startDateTime = new DateTime(1900, 1, 1);
        DateTime endDateTime = new DateTime(1900, 1, 1);
        //New object array to return
        object[] returnArray = new object[4];
        //Status message about the time
        string statusMessage = "";
        //Status code
        int statusCode = 0;

        //Array holding month groupings
        int[] thirty = { 9, 4, 6, 11 }; int[] thirtyO = { 1, 3, 5, 7, 8, 10, 12 }; int[] two = { 2 };
        //if there is an endtime and enddate
        if ((startTime.Length > 0) && (startDate.Length > 0))
        {
            //If the time and date is in the correct format
            if (timeTest.IsMatch(startTime) && (dateTest1.IsMatch(startDate)
                || dateTest2.IsMatch(startDate) || dateTest3.IsMatch(startDate) || dateTest4.IsMatch(startDate)))
            {
                //Split array of time variables
                string[] timeArray = startTime.Split(":".ToCharArray());
                string[] dateArray = startDate.Split("/".ToCharArray());
                //Int variables representing day and month
                int month = int.Parse(dateArray[0]); int day = int.Parse(dateArray[1]);
                //If the time variables are valid
                if ((int.Parse(timeArray[0]) < 24) && (int.Parse(timeArray[1]) < 60))
                {
                    //If the month and day are valid
                    if ((thirty.Contains(month) && day <= 30) || (thirtyO.Contains(month) && day <= 31) || (two.Contains(month) && day < 30))
                    {
                        //Construct a new date/time object using the start time and date fields
                        startDateTime = new DateTime(int.Parse(dateArray[2]), month, day, int.Parse(timeArray[0]), int.Parse(timeArray[1]), 0);
                        //Set the Status code
                        statusCode = 1;
                    }
                    else
                    {
                        //Assign a message to the status message variable
                        statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nInvalid date format in StartDateBox " + startDate;
                        //Set the Status code
                        statusCode = 3;
                    }

                }
                else
                {
                    //Assign a message to the status message variable
                    statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nInvalid time format in StartTimeBox " + startTime;
                    //Set the Status code
                    statusCode = 3;
                }
            }
            else
            {
                //Assign a message to the status message variable
                statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nInvalid date format in StartTimeBox " + startTime;
                //Set the Status code
                statusCode = 3;
            }
        }
        //if the endtime and enddate are not empty strings
        if ((endTime.Length > 0) && (endDate.Length > 0))
        {
            //If the time and date is in the correct format
            if (timeTest.IsMatch(endTime) && (dateTest1.IsMatch(endTime)
                || dateTest2.IsMatch(endDate) || dateTest3.IsMatch(endDate) || dateTest4.IsMatch(startDate)))
            {
                //Split array of time variables
                string[] timeArray = endTime.Split(":".ToCharArray());
                string[] dateArray = endDate.Split("/".ToCharArray());
                //Int variables representing day and month
                int month = int.Parse(dateArray[0]); int day = int.Parse(dateArray[1]);
                //If the time variables are valid
                if ((int.Parse(timeArray[0]) < 24) && (int.Parse(timeArray[1]) < 60))
                {
                //If the month and day are valid
                if ((thirty.Contains(month) && day <= 30) || (thirtyO.Contains(month) && day <= 31) || (two.Contains(month) && day < 30))
                {
                    //Construct a new date/time object using the end time and date fields
                    endDateTime = new DateTime(int.Parse(dateArray[2]), month, day, int.Parse(timeArray[0]), int.Parse(timeArray[1]), 0);
                    //Set the Status code
                    statusCode = 1;
                }
                else
                {
                    //Assign a message to the status message variable
                    statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nMessage: Invalid date format in EndDateBox " + endDate;
                    //Set the Status code
                    statusCode = 3;
                }
            }
            else
            {
                //Assign a message to the status message variable
                statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nMessage: Invalid time format in EndTimeBox " + endTime;
                //Set the Status code
                statusCode = 3;
            }
        }
        else
            {
            //Assign a message to the status message variable
            statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nMessage: Invalid date format in EndTimeBox " + endTime + " or EndDateBox " + endDate;
            //Set the Status code
            statusCode = 3;
            }
        }    
        //Assign the variables to the return array
        returnArray[0] = startDateTime;
        returnArray[1] = endDateTime;
        returnArray[2] = statusCode;
        returnArray[3] = statusMessage;
        //Return the object array
        return returnArray;
    }

    //This method processes a start time and returns a datetime starttime, datetime endtime
    //a string status, and an integer status code in an array
    private object[] processTimes(String startTime, String startDate)
    {
        //New object array to return
        object[] returnArray = processTimes(startTime, startDate, "", "");
        //Return the object array
        return returnArray;
    }

    //This method gets the different event counts for the active event list from the database
    private void updateEventCount()
    {
        //Get the Total row count
        string[] count = ((string)Database.select_EventListCount()).Split(char.Parse(","));
        //Get the status labels from the master page
        Label AllT = (Label)(Master.FindControl("AllT"));
        Label ForT = (Label)(Master.FindControl("ForT"));
        Label UntT = (Label)(Master.FindControl("UntT"));
        Label ComT = (Label)(Master.FindControl("ComT"));
        Label IncT = (Label)(Master.FindControl("IncT"));
        Label ErrT = (Label)(Master.FindControl("ErrT"));
        //Set the appropriate components text
        AllT.Text = (count[0].ToString());
        UntT.Text = (count[1].ToString());
        ComT.Text = (count[2].ToString());
        IncT.Text = (count[3].ToString());
        ErrT.Text = (count[4].ToString());
        ForT.Text = (count[5].ToString());
    }

    //Function used to select edited rows and pass updates to the datatbase asynchronously
    private void updateRow(GridViewRow row)
    {
        //get the relevent fields of data
        HiddenField elid = (HiddenField)row.FindControl("ELIDH");
        HiddenField eid = (HiddenField)row.FindControl("EIDH");
        HiddenField schedTime = (HiddenField)row.FindControl("SchedTimeH");
        bool ForwardH = bool.Parse(((HiddenField)row.FindControl("Forward")).Value);
        bool StartChanged = bool.Parse(((HiddenField)row.FindControl("StartChanged")).Value);
        CheckBox aStatus = (CheckBox)row.FindControl("ActiveCheck");
        TextBox startTime = ((TextBox)row.FindControl("StartTimeBox"));
        String startDate = ((TextBox)row.FindControl("StartDateBox")).Text;
        TextBox endTime = ((TextBox)row.FindControl("EndTimeBox"));
        String endDate = ((TextBox)row.FindControl("EndDateBox")).Text;
        //Final datetime variables for start and end times
        //1900,1,1 represents an invalid date. If this date is passed to the SQL stored procedure
        //It will be ignored
        DateTime startDateTime = new DateTime(1900, 1, 1);
        DateTime endDateTime = new DateTime(1900, 1, 1);
        //Control datetime for times to be compared against
        DateTime compareTime = new DateTime(1900, 1, 1);
        //Uername
        String UserName = Page.User.Identity.Name.ToString();
        //Boolean representing complete status
        Boolean complete = false;
        //String represent Status message to update
        String statusMessage = "";
        //Integer representing Status code to update 0=no information 1=succesfull completion 2=successful incomplete 3=error
        int statusCode = 0;

        //if there is a event list id variable
        if (elid.Value.Length > 0)
        {
            //Add 1 to updatedRows
            rowsUpdated++;
            //variables from time process
            object[] vars;
            //scheduled date time
            DateTime schedDateTime = DateTime.Parse(schedTime.Value);
            //Current shift
            DateTime shift = DateTime.Parse(CurrentShift.Value);  
            //if the endtime box is enabled indicating an end time present process start and end date/times
            if (endTime.Enabled == true) { vars = processTimes(startTime.Text, startDate, endTime.Text, endDate); }
            //else Process the startdate times and get the return array
            else { vars = processTimes(startTime.Text, startDate); }
            //assign variables from processTimes
            startDateTime = (DateTime)vars[0];
            endDateTime = (DateTime)vars[1];
            statusCode = int.Parse(vars[2].ToString());
            statusMessage = vars[3].ToString();
            //Boolean indicating comment in last 60 mintues
            int comToday = CommentCheck(int.Parse(eid.Value));
            
            //StartTime and forwarded event processing
            //If the startDateTime is not equal to the control time and 
            if ((startDateTime != compareTime))
            {             
                //If the start date time is less than the shift
                if(startDateTime < shift)
                {
                    //If the event is forwarded and start time has been changed
                    if (ForwardH == true && StartChanged == true)
                    {
                        //If there was no comment
                        if (comToday == -1)
                        {
                            //Assign a message to the status message variable
                            statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nMessage: " + startDateTime.ToString("MM/dd/yyyy HH:mm") + " occurs before the shift date of " + shift.ToString("MM/dd/yyyy HH:mm") +
                                ". In order to enter a start date/time that occurs before the current shift, a comment must be entered within the previous 60 minutes.";
                            //Set the Status code to 3 indicating an error
                            statusCode = 3;
                            //Set the start date time to a value indicating ignore
                            startDateTime = new DateTime(1900, 1, 1);
                            endDateTime = new DateTime(1900, 1, 1);
                        }
                        //If there is a comment set
                        else
                        {
                            //Set the comment to uneditable as it is being used 
                            Database.update_Comment_Editable(comToday, false);
                        }
                    }
                    //Else if the event is not forwarded
                    else if (ForwardH == false)
                    {
                        //Assign a message to the status message variable
                        statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nMessage: Invalid Row Update - The start date/time of the event " + startDateTime.ToString("MM/dd/yyyy HH:mm") +
                                                        " occurs before the scheduled date/time of the current shift " + shift.ToString("MM/dd/yyyy HH:mm");
                        //Set the Status code to 3 indicating an error
                        statusCode = 3;
                        //Set the start date time to a value indicating ignore
                        startDateTime = new DateTime(1900, 1, 1);
                        endDateTime = new DateTime(1900, 1, 1);
                    }
                }
            }
            //Endtime processing and updating
            //statuscode == 1 indicating valid start/end times
            if (statusCode == 1)
            {
                //If the endbox is enabled
                if (endTime.Enabled == true)
                {
                    //If the start and end times are valid by not equaling the control time
                    if (endDateTime != compareTime && startDateTime != compareTime)
                    {
                        //If endDatetime is less than startdate time
                        if (endDateTime < startDateTime)
                        {
                            //Assign a message to the status message variable
                            statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nMessage: EndDate " + endDateTime.ToString() + " occurs before StartDate " + startDateTime.ToString();
                            //Set the Status code to 3 indicating an error
                            statusCode = 3;
                            //Reset the enddate variable
                            endDateTime = new DateTime(1900, 1, 1);
                        }
                        //Else if the enddate time is less than the scheduled date time
                        else if (endDateTime < shift)
                        {
                            //If there was no comment
                            if (comToday == -1)
                            {
                                //Assign a message to the status message variable
                                statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nMessage: " + endDateTime.ToString("MM/dd/yyyy HH:mm") + " occurs before the shift date of " + shift.ToString("MM/dd/yyyy HH:mm") +
                                                         ". In order to enter a end time less than that of the current shift, a comment must be entered within the previous 60 minutes.";
                                //Set the Status code to 3 indicating an error
                                statusCode = 3;
                                //Set the end date time to a value indicating ignore
                                endDateTime = new DateTime(1900, 1, 1);
                            }
                            //If there is a comment set
                            else
                            {
                                //Set the comment to uneditable as it is being used 
                                Database.update_Comment_Editable(comToday, false);
                                //Assign a message to the status message variable
                                statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nMessage: Update of row succeeded";
                                //Set the Status code to 1 indicating complete
                                statusCode = 1;
                                //Set complete to true
                                complete = true;
                            }
                        }
                        //set successful update paremeters
                        else
                        {
                            //Assign a message to the status message variable
                            statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nMessage: Update of row succeeded";
                            //Set the Status code to 1 indicating complete
                            statusCode = 1;
                            //Set complete to true
                            complete = true;
                        }
                    }
                    //Else if startime exists and enddatetime is still equal to the control time
                    else if ((startDateTime != compareTime) && (endDateTime == compareTime))
                    {
                        //Assign a message to the status message variable
                        statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nMessage: Update of row succeeded";
                        //Set the Status code to 2 indicating parital completion
                        statusCode = 2;
                    }
                    else if ((startDateTime == compareTime) && (endDateTime != compareTime))
                    {
                        //Assign a message to the status message variable
                        statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nMessage: Invalid Row Update - Endtime entered without a start time";
                        //Set the Status code to 3 indicating error
                        statusCode = 3;
                    }                   
                }
                //Else the record only contains a start time box
                else
                {
                    //If  status code is 1 indicating valid start and end time or is equal to 0 indicating a blank save
                    if (statusCode == 0 || statusCode == 1)
                    {
                        //Assign a message to the status message variable
                        statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nMessage: Update of row succeeded";
                        //Set complete to true
                        complete = true;
                    }
                }
            }            
            //Call the Database update function with appropriate variables
            Database.update_EventList((int.Parse(elid.Value)), complete, endDateTime, (aStatus.Checked), startDateTime, statusCode, statusMessage, UserName);
        }              
    }

    //This method updates the status bar with the provided message
    private void updateStatus(String message)
    {
        //GEt the status label from the master panel
        Label StatusBox = (Label)Master.FindControl("mHeaderLabelCenter");
        //Set the status text
        StatusBox.Text = message;  
        //Update the update panel
        ((UpdatePanel)Master.FindControl("masterUpperUpdate")).Update();
    }

    //This method is used to validate the components on the One Time event adding panel
    private bool validateEventPanel()
    {
        //Boolean to control event insert
        bool valid = true;
        //If te event name field is blank
        if (EventTitle.Text.Length <= 0)
        {
            //Set insert to false
            valid = false;
            //Set the status message
            ResponseName.Text = " - An Event name must be included";
        }
        else { ResponseName.Text = ""; }
        //Regext to test date
        Regex dateTest = new Regex("\\d\\d\\/\\d\\d\\/\\d\\d\\d\\d");
        //If a valid date is not present
        if (!dateTest.IsMatch(DateBox.Text))
        {
            //Set insert to false
            valid = false;
            //Set the status message
            ResponseDate.Text = "A date in the correct format mm/dd/yyyy must be entered";
        }
        else{ 
            //Datatable of event lsit status
            DataTable dt = Database.select_EventListStatus();
            //select the row if the in play event list
            DataRow inPlayRow = (dt.Select("[InPlay] = 'true'"))[0];
            //Get the date of the inplay row
            DateTime inPlayDate = (DateTime)inPlayRow.Field<DateTime>("LastCreated");
            //Get date components
            int year = int.Parse((DateBox.Text.Split("/".ToCharArray())[2]).ToString());
            int month = int.Parse((DateBox.Text.Split("/".ToCharArray())[0]).ToString());
            int day = int.Parse((DateBox.Text.Split("/".ToCharArray())[1]).ToString());
            int hour = int.Parse(HourList.SelectedValue);
            int min = int.Parse(MinList.SelectedValue);
            //Create new date time from date/time boxes
            DateTime eventTime = new DateTime(year, month, day, hour, min, 00);
            //if the eventTime is not between the start and end of the current shift
            if((eventTime < inPlayDate) || (eventTime > inPlayDate.AddHours(12)))
            {
                //Set insert to false
                valid = false;
                //Set the status message
                ResponseDate.Text = "A date/time during the current shift must be entered";
            }
            else{ ResponseDate.Text = ""; }
        }
        //Return the insert value
        return valid;
    }

    //Updates the status message to include the number of rows update
    private void updateRowMessage(string message)
    {
        //Update the status message
        updateStatus(System.DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " - " + message);
        //Set rows updated to 0
        rowsUpdated = 0;
    }

//**Region - Event Related Methods
    //Occurs when a row is bound to the comment data grid
    protected void ComGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //Cast the datarow
        GridViewRow dataRow = (GridViewRow)e.Row;
        //If the row is not a header
        if (dataRow.RowType == DataControlRowType.DataRow)
        {
            //Get the data row view
            DataRowView rowView = (DataRowView)e.Row.DataItem;
            //Get the object array behind it
            Object[] Values = rowView.Row.ItemArray;
            //Get relevant link buttons
            LinkButton ELB = (LinkButton)dataRow.FindControl("EditLB");
            //Get relevant link buttons
            LinkButton DLB = (LinkButton)dataRow.FindControl("DeleteLB");
            //Get the doc hidden field
            HiddenField Edit = (HiddenField)dataRow.FindControl("Editable");
            //date time of comment
            DateTime comment = System.DateTime.ParseExact((Values[3].ToString() + " " + Values[4].ToString()), "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
            //Person who wrote the comment
            String commentor = Values[6].ToString();
            //The editable value from the DB
            string editString = Values[7].ToString();
            //Comment editable statu
            bool editable = true;
            //if the string has a value
            if (editString.Length > 0) { editable = bool.Parse(editString); }     
            //Set hidden field with editable status
            Edit.Value = editable.ToString();
            //If the comment has not been created in the last 12 hours by the current user or is not editable disable the edit button
            if ((comment < System.DateTime.Now.AddHours(-12)) || !(commentor.Equals(Page.User.Identity.Name.ToString())) || editable == false) { ELB.Visible = false; DLB.Visible = false; }
        }
    }

    //Occurs when edit mode is canceled on a row in the comment window
    protected void ComView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        //Get the datarow in question
        GridViewRow dataRow = ComGrid.Rows[e.RowIndex];
        //Set the edit index to -1 signifying no edit more
        ComGrid.EditIndex = -1;
        //Set the edit mode variabes
        editMode(false, e.RowIndex);
        //If no comment was added - Delete the row
        if (((TextBox)dataRow.FindControl("CommentBox")).Text.Length <= 0) { deleteComment(int.Parse(dataRow.Cells[0].Text));}
        //Bind data
        bindData(int.Parse(PriorCountH.Value));
        //Update the panel
        ComData.Update();
    }

    //This occurs when a comment is deleted from the comment table
    protected void ComGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        //Get the datarow in question
        GridViewRow dataRow = ComGrid.Rows[e.RowIndex];
        //Delete the row
        deleteComment(int.Parse(dataRow.Cells[0].Text));
        //bind the data
        bindData(0);
        //Update the panel
        ComData.Update();
    }

    //Occurs when a row enters editing mode in the comment window
    protected void ComView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        //Set the new edit index
        ComGrid.EditIndex = e.NewEditIndex;
        //bind the data
        bindData(int.Parse(PriorCountH.Value));
        //Set the edit mode variabes
        editMode(true, e.NewEditIndex);
        //Update the panel
        ComData.Update();
    }

    //Occurs when a row is updating in the comment window
    protected void ComView_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        //Get the datarow in question
        GridViewRow dataRow = ComGrid.Rows[e.RowIndex];
        //get the value of the selected radio 
        int count = int.Parse(PriorCountH.Value);
        //Get the doc hidden field
        HiddenField Edit = (HiddenField)dataRow.FindControl("Editable");
        //Call the update method
        Database.update_Comment(int.Parse(dataRow.Cells[0].Text), ((TextBox)dataRow.FindControl("CommentBox")).Text.ToString().Trim(), dataRow.Cells[5].Text.ToString().Trim(),
                                                            System.DateTime.ParseExact((dataRow.Cells[1].Text + " " + dataRow.Cells[2].Text), "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture), System.DateTime.Now, bool.Parse(Edit.Value));
        //set edit mode
        editMode(false, e.RowIndex);
        //Set the edit index to -1 signifying no edit more
        ComGrid.EditIndex = -1;
        //Bind Data
        bindData(int.Parse(PriorCountH.Value));
        //Refresh the panel
        ComData.Update();
    }

    //Used to control the look of a row in the grid table
    private void controlRowDisplay(Object sender, GridViewRowEventArgs e)
    {
        //Cast the datarow
        GridViewRow dataRow = (GridViewRow)e.Row;
        //If the row is not a header
        if (dataRow.RowType == DataControlRowType.DataRow)
        {
            //Get the data row view
            DataRowView rowView = (DataRowView)e.Row.DataItem;
            //Get the doc hidden field
            HyperLink Doc = (HyperLink)dataRow.FindControl("DocB");
            //Get the url hidden field
            HyperLink URLAdd = (HyperLink)dataRow.FindControl("UrlB");
            //Get hidden elid field
            HiddenField ELID = (HiddenField)dataRow.FindControl("ELIDH");
            //Get the hidden scheduled time field
            HiddenField SchedTime = (HiddenField)dataRow.FindControl("SchedTimeH");
            //Get the hidden schedule code time field
            HiddenField ForwardH = (HiddenField)dataRow.FindControl("Forward");
            //Status image
            Image StatusIm = (Image)dataRow.FindControl("StatusIm");
            //Doc image
            Image DocIm = (Image)dataRow.FindControl("DocLinkImg");
            //Url image
            Image UrlIm = (Image)dataRow.FindControl("SiteLinkImg");
            //Event id hidden field
            HiddenField EID = (HiddenField)dataRow.FindControl("EIDH");
            //Performance button
            ImageButton PerfB = (ImageButton)dataRow.FindControl("PerformanceListB");

            //Get the end date fields
            TextBox EndTimeB = (TextBox)dataRow.FindControl("EndTimeBox");
            TextBox EndDateB = (TextBox)dataRow.FindControl("EndDateBox");
            AjaxControlToolkit.CalendarExtender EndDateCal = (AjaxControlToolkit.CalendarExtender)dataRow.FindControl("EndDateCal");
            //Get the data controls
            TextBox StartTimeB = (TextBox)dataRow.FindControl("StartTimeBox");
            TextBox StartDateB = (TextBox)dataRow.FindControl("StartDateBox");
            AjaxControlToolkit.CalendarExtender StartDateCal = (AjaxControlToolkit.CalendarExtender)dataRow.FindControl("StartDateCal");

            //Get the object array behind it
            Object[] Values = rowView.Row.ItemArray;
            //Get the values from the row
            String BeDateS = Values[3].ToString();
            String UrlS = Values[6].ToString();
            Boolean FinishS = Boolean.Parse(Values[7].ToString());
            String StartTimeS = Values[8].ToString();
            String StartDateS = Values[9].ToString();
            String DocS = Values[13].ToString();
            String ScriptS = Values[14].ToString();
            Boolean Forward = Boolean.Parse(Values[15].ToString());
            int StatusCode = int.Parse(Values[16].ToString());
            Boolean Active = Boolean.Parse(Values[12].ToString());
            String CatCol = Values[19].ToString();

            //Set hidden fields fields
            ELID.Value = Values[0].ToString();
            EID.Value = Values[18].ToString();
            SchedTime.Value = Values[1].ToString();
            PerfB.ToolTip = "Scheduled For: " + Values[3].ToString() + " " + Values[2].ToString();
            //If there is a status message
            if (Values[17].ToString().Length > 0)
            {
                //Set the tooltip to include the username and message
                StatusIm.ToolTip = "User: " + Values[20].ToString() + Values[17].ToString();
            }

            //iF active is true
            if (Active)
            {
                //If a color exists for the category
                if (CatCol.Length > 0)
                {
                    //Set the color
                    dataRow.Cells[3].ForeColor = System.Drawing.ColorTranslator.FromHtml("#" + CatCol);
                }
                //Highlight forwarded status
                if (Forward == true)
                {
                    //Set the boundfield cell to red
                    dataRow.Cells[1].CssClass = "Forward";
                    //set the forward hidden field of the row to true
                    ForwardH.Value = "true";
                }
                //If there is a start time present
                if (StartTimeS.Length > 0)
                {
                    //Set the values
                    StartTimeB.Text = StartTimeS.ToString().Substring(0, 5);
                    StartDateB.Text = StartDateS;
                    StartDateCal.SelectedDate = DateTime.Parse(StartDateS);
                }
                else
                {
                    //Set the box value
                    StartDateB.Text = BeDateS;
                    StartDateCal.SelectedDate = DateTime.Parse(BeDateS);
                }

                //If finish time is required
                if (FinishS == true)
                {
                    //Get date variables
                    String EndTimes = Values[10].ToString();
                    String EndDateS = Values[11].ToString();
                    //If there is a value for time
                    if (EndTimes.Length > 0)
                    {
                        //Set the values
                        EndTimeB.Text = EndTimes.ToString().Substring(0, 5);
                        EndDateB.Text = EndDateS;
                    }
                }
                else
                {
                    //Disable the boxes
                    EndDateB.Enabled = false;
                    EndTimeB.Enabled = false;
                    EndDateB.CssClass = "TableTimeDateBoxNo";
                    EndTimeB.CssClass = "TableTimeDateBoxNo";
                }
            }
            else
            {
                //Disable start and end boxes
                StartDateB.Visible = false;
                StartTimeB.Visible = false;
                EndDateB.Visible = false;
                EndTimeB.Visible = false;
                StartDateB.CssClass = "DisabledTextBox";
                StartTimeB.CssClass = "DisabledTextBox";
                EndDateB.CssClass = "DisabledTextBox";
                EndTimeB.CssClass = "DisabledTextBox";
                //Set the row style to disabled
                dataRow.CssClass = "Disabled";
                dataRow.Cells[1].CssClass = "DisabledTextCent";
                dataRow.Cells[4].CssClass = "DisabledTextJust";
                dataRow.Cells[3].CssClass = "DisabledTextCent";
                dataRow.Cells[5].CssClass = "DisabledTextJust";
                dataRow.Cells[6].CssClass = "DisabledTextJust";
                //Get the disable button
                ImageButton StatB = (ImageButton)dataRow.FindControl("StatusB");
                //Set the tooltip of the button
                StatB.ImageUrl = "~/Images/Enable.gif";
                StatB.ToolTip = "Enable";
            }
            //If a document is provided
            if (DocS.Length > 0)
            {
                //set the doc hidden field
                Doc.NavigateUrl = DocS;
                //Set the image
                DocIm.ImageUrl = "~/Images/Doc1.gif";
                //Set the tooltip
                Doc.ToolTip = "Documentation Link";
            }
            else
            {
                //Disable the url button
                Doc.Enabled = false;
                //Clear the tooltip
                Doc.ToolTip = "";
            }
            //If a url is provided
            if (UrlS.Length > 0)
            {
                if (UrlS.StartsWith("mailto:"))
                {
                    //Set the image to the mail icon
                    UrlIm.ImageUrl = "~/Images/Email.gif";
                    //Set the tooltip
                    URLAdd.ToolTip = "Email Link";
                }
                else
                {
                    //Set the image
                    UrlIm.ImageUrl = "~/Images/Link.gif";
                    //Set the tooltip
                    URLAdd.ToolTip = "Site Link";
                }
                //set the url navigate link
                URLAdd.NavigateUrl = UrlS;
                
            }
            else
            {
                //Disable the url button
                URLAdd.Enabled = false;
                //Clear the tooltip
                URLAdd.ToolTip = "";
            }
            //Update the status image with the appropriate image
            switch (StatusCode)
            {
                case 1:
                    //Set the image button to the appropriate status
                    StatusIm.ImageUrl = "~/Images/Success.gif";
                    break;
                case 2:
                    //Set the image button to the appropriate status
                    StatusIm.ImageUrl = "~/Images/Partial.gif";
                    break;
                case 3:
                    //Set the image button to the appropriate status
                    StatusIm.ImageUrl = "~/Images/Error.gif";
                    break;
                default:
                    //Set the image button to the appropriate status
                    StatusIm.ImageUrl = "~/Images/Untouch.gif";
                    break;
            }
        }
    }

    //This method generates an event list. The method uses the current time
    //and a value from the Database to determine whether or not to create a new 
    //event list, and which one day/night to create.
    protected void create_EventList(Object obj, EventArgs ags)
    {
        //Current Date
        DateTime Date = System.DateTime.Now.Date;
        //Current datetime
        TimeSpan Current = DateTime.Now.TimeOfDay;
        //Day shift time
        TimeSpan dayShift = new TimeSpan(08,00,00);
        //Night Shift time
        TimeSpan nightShift = new TimeSpan(20, 00, 00);

        //New datatable object
        DataTable dt = Database.select_EventListStatus();
        //Get the dayshift row
        DataRow dayrow = (dt.Select("[Shiftname] = 'DayShift'"))[0];
        //Get the nightshift row
        DataRow nightrow = (dt.Select("[Shiftname] = 'NightShift'"))[0];
        
        //If it is after 8am and before 8pm, and todays date is greater than the date of the last generated shift
        if ((Current > dayShift && Current < nightShift) && (Date > dayrow.Field<DateTime>("LastCreated").Date))
        {
            //Roll over the current event list
            Database.rollover_EventList();
            //Go to the event forward page
            Response.Redirect("~/Views/Forward.aspx");
        }
        //If the current time is after nightshift or before day shift and the todays date is greater than or equal to the last generated shift list and the type of shift is not already in play OR Today's datetime is a full day past the last generated shift event list
        else if (((Current > nightShift || Current < dayShift)) &&
            (((Date >= nightrow.Field<DateTime>("LastCreated").Date) && (nightrow.Field<bool>("InPlay") == false)) || (System.DateTime.Now > nightrow.Field<DateTime>("LastCreated").AddDays(1))))
        {
            //Roll over the current event list
            Database.rollover_EventList();
            //Go to the event forward page
            Response.Redirect("~/Views/Forward.aspx");
        }
        else
        {
            //GEt the status label from the master panel
            Label StatusBox = (Label)Master.FindControl("mHeaderLabelCenter"); 
            //Update the text in the info box
            updateRowMessage("Current Event List already in play.");
            //update the gird
            populateGrid();
        }
    }

    //This method is used to populate performance data when the index of the radio list changes
    protected void DateListPer_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Update message
        updateRowMessage(rowsUpdated + " rows saved to the database.");
        //Cast thesender object to a image button
        RadioButtonList btn = (RadioButtonList)sender;
        //get the value of the selected radio 
        int occur = int.Parse(btn.SelectedValue);
        //Bind data to the grid
        PerGrid.DataSource = Database.select_EventPerformance(occur, int.Parse(PEIDH.Value), new DateTime(1900, 1, 1), new DateTime(1900, 1, 1));
        PerGrid.DataBind();      
        //Set the event title
        PerEventTitle.Text = PerEventTitleH.Value;
        //Set the selected index to -1
        DateListPer.SelectedIndex = -1;
        //Refresh the update panel
        PerData.Update();
    }

    //This method is used to populate comment data when the index of the radio list changes
    protected void DateListCom_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Update message
        updateRowMessage(rowsUpdated + " rows saved to the database.");
        //Cast thesender object to a image button
        RadioButtonList btn = (RadioButtonList)sender;
        //get the value of the selected radio 
        int occur = int.Parse(btn.SelectedValue);
        //Bind the data
        bindData(occur);
        //Make the insert button visible
        InsertLB.Visible = true;
        //Set the previous count hidden field
        PriorCountH.Value = DateListCom.SelectedValue.ToString();
        //Set the selected index to -1
        DateListCom.SelectedIndex = -1;
        //Refresh the update panel
        ComData.Update();
    }

    //This occurs when a row is bound to the forward data grid
    protected void ForGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //Cast the datarow
        GridViewRow dataRow = (GridViewRow)e.Row;
        //If the row is not a header
        if (dataRow.RowType == DataControlRowType.DataRow)
        {
            //Get the data row view
            DataRowView rowView = (DataRowView)e.Row.DataItem;
            //Get the object array behind it
            Object[] Values = rowView.Row.ItemArray;
            //Category color
            String CatCol = Values[19].ToString();
            //Forward status
            Boolean Forward = Boolean.Parse(Values[15].ToString());
            //If a color exists for the category
            if (CatCol.Length > 0)
            {
                //Set the color
                dataRow.Cells[1].ForeColor = System.Drawing.ColorTranslator.FromHtml("#" + CatCol);
            }
            //Highlight forwarded status
            if (Forward == true)
            {
                //Set the boundfield cell to red
                dataRow.Cells[0].CssClass = "Forward";
            }
        }
    }

    //Loads the forwarded events into the FOrward grid view
    protected void GetData_Click(object sender, EventArgs e)
    {
        //Update message
        updateRowMessage(rowsUpdated + " rows saved to the database.");
        //populate the forward grid
        populateGridForward();
    }

    //This occurs when toggle active status image button is clicked
    protected void iButtonActive_Click(Object obj, EventArgs ags)
    {
        //Cast thesender object to a image button
        ImageButton btn = (ImageButton)obj;
        //Get the sending row
        GridViewRow row = (GridViewRow)btn.NamingContainer;
        //Get the hidden value for elid
        HiddenField ELIDH = (HiddenField)row.FindControl("ELIDH");
        //Get the hidden value for eid
        HiddenField EIDH = (HiddenField)row.FindControl("EIDH");
        //Get the ELID from the row
        int elid = Convert.ToInt32(ELIDH.Value);
        int eid = Convert.ToInt32(EIDH.Value);
        //Get the Active status
        CheckBox checkStat = (CheckBox)row.FindControl("ActiveCheck");
        //Boolean indicating if a comment in last 60 mintues was entered
        int comToday = CommentCheck(eid);  
        //If this is an enable click  
        if (checkStat.Checked == false)
        {
            //Call the Database method
            Database.update_ActiveStatusEventList(elid, true);
            //Assign a message to the status message variable
            String statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nMessage: Changing of Active status succeeded.";
            //boolean indicating if the event is complete
            bool complete = Database.select_EventListComplete(elid);
            //if boolean is true indicating event is complete
            if (complete)
            {
                //Update the status info with successful message and complete status
                Database.update_StatusInfo(elid, statusMessage, 1);
            }
            else
            {
                //Update the status info with successful message and incomplete status
                Database.update_StatusInfo(elid, statusMessage, 2);
            }
        }
        //if this is a disable event where a comment has been set
        else if (comToday != -1)
        {
            //Set the comment in question to uneditable
            Database.update_Comment_Editable(comToday, false);
            //Call the Database method
            Database.update_ActiveStatusEventList(elid, false);
            //Assign a message to the status message variable
            String statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nMessage: Changing of Active status succeeded.";
            //Update the status info with successful message
            Database.update_StatusInfo(elid, statusMessage, 1);
        }
        else
        {
            //Assign a message to the status message variable
            String statusMessage = "\nTime: " + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\nMessage: Changing of Active status failed due to no provided comment in the last 60 minutes. A comment entered in the last 60 minutes is required when changing the active status of an event.";
            //Update the status info with an error
            Database.update_StatusInfo(elid, statusMessage, 3);
        }
        //populate the grid
        populateGrid();
    }

    //Occurs when the insert button is clickeds in the comment window
    protected void InsertLB_Click(object sender, EventArgs e)
    {
        //Update message
        updateRowMessage(rowsUpdated + " rows saved to the database.");
        //Get the eid from the hidden field
        int EID = int.Parse(CEIDH.Value);
        //Call the add comment method
        Database.add_Comment(EID, "", Page.User.Identity.Name.ToString(), System.DateTime.Parse(ComSchedTime.Value), DateTime.Now, true);
        //Set the edit index the new row
        ComGrid.EditIndex = (0);
        //Bind Data
        bindData(int.Parse(PriorCountH.Value));
        //set edit mode
        editMode(true, 0);
        //Update the panel
        ComData.Update();    
    }

    //This occurs everytime a row is bound to the data grid
    protected void rowDataGridBind(Object sender, GridViewRowEventArgs e)
    {
        //Call the function that sets up the row
        controlRowDisplay(sender, e);
    }

    //This method is used to update an edited row
    protected void rowEdited(object sender, EventArgs e)
    {
        //The textbox sending the event
        TextBox box = (TextBox)sender;
        //Gridview row of textbox
        GridViewRow row = (GridViewRow)box.Parent.Parent;
        //Set the row saved value to true indicating 
        HiddenField rowSaved = (((HiddenField)row.FindControl("Saved")));
        //if the row has not been previously saved
        if (bool.Parse(rowSaved.Value.ToString()) == false)
        {
            //if the text box is the start or end date text box check the queue first to see if the row index already exists
            if (box.ID.StartsWith("Start"))
            {
                //set the hidden StartChanged fields value
                ((HiddenField)row.FindControl("StartChanged")).Value = "true";
            }
            //Set saved to true
            rowSaved.Value = "true";
            //Update the row
            updateRow(row);
        }
    }

    //Occurs when the submit button is clicked on the OneTime Event Adding Panel
    protected void SubmitEvent_Click(object sender, EventArgs e)
    {
        //Update message
        updateRowMessage(rowsUpdated + " rows saved to the database.");
        //If valid is true
        if (validateEventPanel())
        {
            //Get date components
            int year = int.Parse((DateBox.Text.Split("/".ToCharArray())[2]).ToString());
            int month = int.Parse((DateBox.Text.Split("/".ToCharArray())[0]).ToString());
            int day = int.Parse((DateBox.Text.Split("/".ToCharArray())[1]).ToString());
            int hour = int.Parse(HourList.SelectedValue);
            int min = int.Parse(MinList.SelectedValue);
            //Create new date time from date/time boxes
            DateTime eventTime = new DateTime(year, month, day, hour, min, 00);
            //Add the event to the database
            Database.add_EventLive(CategoryList.SelectedValue, EventTitle.Text, FinishTime.Checked, URLTextBox.Text, eventTime, Page.User.Identity.Name);
            //Update the componets
            EventStatL.Text = System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + " - One Time Event Added";
            //Update the status message
            updateStatus(System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + " - One Time Event Added");
            //clear the panel
            clearPanel();
        }
        //Update the panel
        EventPanUpdate.Update();
    }

    //This method is called when the save eventlist button is clicked
    protected void SaveB_Click(object sender, EventArgs e)
    {
        //refresh the grid
        populateGrid();
       //Update message
        updateRowMessage("Synching of " + rowsUpdated + " rows to the database is now complete.");
    }

    //This occurs when the timer interval expires
    protected void updateControl(Object obj, EventArgs ags)
    {
        //refresh the grid
        populateGrid();
        //Update message
        updateRowMessage("Synching of " + rowsUpdated + " rows to the database is now complete.");
    }


}