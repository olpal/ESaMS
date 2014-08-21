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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text.RegularExpressions;

public partial class Views_Event : System.Web.UI.Page
{
//**Region - Page Variables**
    //Create a new database object
    Database Database = new Database();


//**Region - Page Related Methds**
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
    
    //Occurs pre page load
    protected void Page_init(object sender, EventArgs e)
    {
        //Load list boxes
        load_ListBoxes();
        //Populate the category box
        populateBoxes("", "");
        //Deal with parameters
        parameters();
        //Load master file variables
        loadMasterInfo();
       
    }

    //Occurs when page loads
    protected void Page_Load(object sender, EventArgs e)
    {
        //If the load is not a postback
        if (!Page.IsPostBack)
        {
            //Set the fields using the hidden id
            setFields(HiddenEID.Value);
        }
        //Register async postback
        ((ScriptManager)Master.FindControl("MasterScriptManager")).RegisterAsyncPostBackControl(EventTitle);
        ((ScriptManager)Master.FindControl("MasterScriptManager")).RegisterAsyncPostBackControl(DocURL);
        ((ScriptManager)Master.FindControl("MasterScriptManager")).RegisterAsyncPostBackControl(URLTextBox);
        ((ScriptManager)Master.FindControl("MasterScriptManager")).RegisterAsyncPostBackControl(CategoryList);
        ((ScriptManager)Master.FindControl("MasterScriptManager")).RegisterAsyncPostBackControl(ScheduleList);
        ((ScriptManager)Master.FindControl("MasterScriptManager")).RegisterAsyncPostBackControl(MonthDayList);
        ((ScriptManager)Master.FindControl("MasterScriptManager")).RegisterAsyncPostBackControl(OccurList);
        ((ScriptManager)Master.FindControl("MasterScriptManager")).RegisterAsyncPostBackControl(DailyList);
        ((ScriptManager)Master.FindControl("MasterScriptManager")).RegisterAsyncPostBackControl(DateBox);
        ((ScriptManager)Master.FindControl("MasterScriptManager")).RegisterAsyncPostBackControl(HourList);
        ((ScriptManager)Master.FindControl("MasterScriptManager")).RegisterAsyncPostBackControl(MinList);
        //Call the validation method
        validation();
    }


//**Region - Data Processing Methods**
    //This method ensures the correct day in the daily checkbox list is selected for weekly and bi-weekly schedules
    private void checkDailyList()
    {
        //Array to hold split date portions of the date box
        String[] dates = DateBox.Text.Split("/".ToCharArray());
        //Clear the daily list selection
        DailyList.ClearSelection();
        //if the array has 3 values indicating correct date format 
        if (dates.Length == 3)
        {
            //Create a new date time object from the datebox and hour and minute lists
            DateTime startTime = new DateTime(int.Parse(dates[2]), int.Parse(dates[0]), int.Parse(dates[1]),
                                int.Parse(HourList.SelectedValue), int.Parse(MinList.SelectedValue), 00);
            //If the schedule box is  weekly or bi-weekly
            if (ScheduleList.SelectedValue == "Weekly" || ScheduleList.SelectedValue == "Bi-Weekly" || ScheduleList.SelectedValue == "Frequent Reoccurance")
            {
                //Set the selected index of the daily check box list
                DailyList.SelectedIndex = (((int)startTime.DayOfWeek));
                //Set the image url
                DailyListVal.ImageUrl = "~/Images/Success.gif";
                //Disable the box for selecting
                DailyList.Enabled = false;
            }
            else if (ScheduleList.SelectedValue == "Daily")
            {
                //Enable the box for selecting
                DailyList.Enabled = true;
                //Set the image url
                DailyListVal.ImageUrl = "~/Images/Partial.gif"; 
            }

        }
        //update the panel
        ListPanel.Update();
    }
    
    //This method creates the controls for the masterpage
    private WebControl[] createControls()
    {
        //BUtton array to return
        WebControl[] returnArray = new WebControl[5];
        //create buttons to add
        ImageButton Submit = new ImageButton();
        ImageButton Validate = new ImageButton();
        ImageButton PerformanceT = new ImageButton();
        ImageButton Comment = new ImageButton();
        ImageButton Audit = new ImageButton();
        //set the button Css class
        Submit.CssClass = "LowerControlButtons";
        Validate.CssClass = "LowerControlButtons";
        PerformanceT.CssClass = "LowerControlButtons";
        Comment.CssClass = "LowerControlButtons";
        Audit.CssClass = "LowerControlButtons";
        //Set the buttons ID
        Submit.ID = "EnterButton";
        Validate.ID = "ValidateT";
        PerformanceT.ID = "PerformanceT";
        Comment.ID = "Comment";
        Audit.ID = "Audit";
        //Set the buttons tooltip
        Submit.ToolTip = "Submit Changes";
        Validate.ToolTip = "Validate Changes";
        PerformanceT.ToolTip = "Performance Information - Table";
        Comment.ToolTip = "Comment Information";
        Audit.ToolTip = "Event Audit Information";
        //Set the buttons imageurl
        Submit.ImageUrl = "~/Images/Ok.gif";
        Validate.ImageUrl = "~/Images/Validate.gif";
        PerformanceT.ImageUrl = "~/Images/PerF.gif";
        Comment.ImageUrl = "~/Images/Comment.gif";
        Audit.ImageUrl = "~/Images/Audit.gif";
        //Set the buttons causesvalidation to false
        Submit.CausesValidation = false;
        Validate.CausesValidation = false;
        PerformanceT.CausesValidation = false;
        Comment.CausesValidation = false;
        Audit.CausesValidation = false;
        //Set onclient click methods
        Submit.Click += submit_Event;
        Validate.Click += ValidateButton_Click;
        PerformanceT.OnClientClick = "PerGridSetup_Click(this)";
        Comment.OnClientClick = "ComGridSetup_Click(this)";
        Audit.OnClientClick = "AudGridSetup_Click(this)";
        //Create new label
        Label StatBottomT = new Label();
        //set status image ids
        StatBottomT.ID = "StatBottomT";
        //Set the css class
        StatBottomT.CssClass = "StatusTextCount";
        //Add buttons to the array
        returnArray[0] = Submit;
        returnArray[1] = Validate;
        returnArray[2] = PerformanceT;
        returnArray[3] = Comment;
        returnArray[4] = Audit;
        //return the array
        return returnArray;
    }

    //This method creates the modal popup extenders for the master page
    private AjaxControlToolkit.ModalPopupExtender[] createExtenders()
    {
        //BUtton array to return
        AjaxControlToolkit.ModalPopupExtender[] returnArray = new AjaxControlToolkit.ModalPopupExtender[3];
        //create modalpopupextenders
        AjaxControlToolkit.ModalPopupExtender PerTPop = new AjaxControlToolkit.ModalPopupExtender();
        AjaxControlToolkit.ModalPopupExtender ComTPop = new AjaxControlToolkit.ModalPopupExtender();
        AjaxControlToolkit.ModalPopupExtender AuditTPop = new AjaxControlToolkit.ModalPopupExtender();
        //set the button Css class
        PerTPop.BackgroundCssClass = "ModelBack";
        ComTPop.BackgroundCssClass = "ModelBack";
        AuditTPop.BackgroundCssClass = "ModelBack";
        //Set the buttons ID
        PerTPop.ID = "PerTPop";
        ComTPop.ID = "ComTPop";
        AuditTPop.ID = "AuditTPop";
        //Set the buttons target control id
        PerTPop.TargetControlID = "PerformanceT";
        ComTPop.TargetControlID = "Comment";
        AuditTPop.TargetControlID = "Audit";
        //Set the buttons popup control id
        PerTPop.PopupControlID = "masterMain_GridPan";
        ComTPop.PopupControlID = "masterMain_GridPan";
        AuditTPop.PopupControlID = "masterMain_GridPan";
        //Add buttons to the array
        returnArray[0] = PerTPop;
        returnArray[1] = ComTPop;
        returnArray[2] = AuditTPop;
        //return the array
        return returnArray;
    }

    //Create status message components
    private WebControl[] createStatus()
    {
        //BUtton array to return
        WebControl[] returnArray = new WebControl[1];
        //Create new label
        Label StatBottomT = new Label();
        //set status image ids
        StatBottomT.ID = "StatBottomT";
        //Set the css class
        StatBottomT.CssClass = "StatusLower";
        //Add buttons to the array
        returnArray[0] = StatBottomT;
        //return the array
        return returnArray;
    }

   //This method is used to ensure a date is correct
    private Boolean dateCheck(int month, int day)
    {
        //Arrays of days for months
        int[] thirty = { 9, 4, 6, 11 }; int[] thirtyO = { 1, 3, 4, 7, 8, 10, 12 }; int[] two = { 2 };
        //If the month and day are valid return true
        if ((thirty.Contains(month) && day <= 30) || (thirtyO.Contains(month) && day <= 31) || (two.Contains(month) && day < 30)){return true;}
        //return false
        else{return false;}
    }

    //Populate the drop down lists associated with the schedule with days and times
    protected void load_ListBoxes()
    {
        //Create a new array for days
        String[] days = { "Sun ", "Mon ", "Tues ", "Wed ", "Thurs ", "Fri ", "Sat " };
        //Numbers for hours
        String[] hours = { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", 
                         "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23"};
        //Numbers for minutes
        String[] minutes = { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", 
                         "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" , "24", "25", "26", 
                         "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", 
                         "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54",
                         "55", "56", "57", "58", "59" };
        //Numbers for days of month
        String[] daysM = { "Last", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", 
                         "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" , "24", "25", "26", 
                         "27", "28", "29", "30", "31"};
        //Options for frequent reoccurence
        String[] freqR = { "First", "Second", "Third", "Forth", "Last" };

        //Set the checkbox list data source
        DailyList.DataSource = days;
        //Set the hour source
        HourList.DataSource = hours;
        //set the minute source
        MinList.DataSource = minutes;
        //Set the monthday list source
        MonthDayList.DataSource = daysM;
        //Set the frequent reocurance
        OccurList.DataSource = freqR;

        //Bind the data sources
        DailyList.DataBind();
        HourList.DataBind();
        MinList.DataBind();
        MonthDayList.DataBind();
        OccurList.DataBind();
    }

    //This method sets masterpage variables
    private void loadMasterInfo()
    {
        //Set the page title
        Page.Title = "Event Creation";
        //Set the header text
        ((Label)Master.FindControl("mHeaderLabel")).Text = "Event Creation";
        //Get the lower right control panel from the master page
        Panel lowerRight = (Panel)Master.FindControl("masterLowerControlPR");
        //get the buttons to add
        WebControl[] toAdd = createControls();
        //get the buttons to add
        AjaxControlToolkit.ModalPopupExtender[] toAddExt = createExtenders();
        //If the hidden eid is greater than 0 and the title box is disabled signaling view
        if(HiddenEID.Value.Length > 0 && EventTitle.Enabled == false){
            //Add the submit button to the panel
            lowerRight.Controls.Add(toAdd[2]);
            //Add the validate button to the panel
            lowerRight.Controls.Add(toAdd[3]);
            //Add the validate button to the panel
            lowerRight.Controls.Add(toAdd[4]);
            //foreach buttton
            foreach (AjaxControlToolkit.ModalPopupExtender adding in toAddExt)
            {
                //Add the button to the panel
                lowerRight.Controls.Add(adding);
            }
        }
        //Else signals new event creation
        else
        {
            //Add the submit button to the panel
            lowerRight.Controls.Add(toAdd[0]);
            //Add the validate button to the panel
            lowerRight.Controls.Add(toAdd[1]);
        }
    }

    //This method deals with passed in parameters
    private void parameters()
    {
        //Parameters
        String param = Request.QueryString.ToString();
        //String for eid, copy status, and Status message
        String eid = ""; String viewCase = ""; String message = "";
        //Enable the components
        setEnabled(true);

        //if the parameter is not blank
        if (param.Length > 0)
        {
            //If there are multiple parameters
            if (param.Contains("&"))
            {
                //split the params into an array
                string[] inParams = param.Split("&".ToCharArray());
                //Assign the first variable to the eid
                eid = inParams[0];
                //if there is a second variable
                if (inParams.Length > 1)
                {
                    //assign the second to copy status
                    viewCase = inParams[1];
                }
            }
            else
            {
                //set eid equal to the passed in param
                eid = param;
            }
            //Integer to hold parse result
            int occur = -1;
            //Try parsing the value
            int.TryParse(eid, out occur);
            //If the result of the tryparse is 0 indicating failure blank the eid value
            if (occur == 0) { eid = ""; }
            //If there is a passed in eid
            if (eid.Length > 0)
            {
                //Set the hidden field to the eid value
                HiddenEID.Value = eid;
                //Set the label
                message = "Current Mode: Event Editing";
                //Integer to hold parse result
                occur = -1;
                //Try parsing the value
                int.TryParse(viewCase, out occur);
                //If the result of the tryparse is not 0 indicating success
                if (occur > 0)
                {
                    //Set the fields
                    setFields(HiddenEID.Value);
                    //Get the lower panel
                    Panel lowerLeft = (Panel)Master.FindControl("masterLowerControlPL");
                    //Create Status labels
                    WebControl[] toStat = createStatus();
                    //switch on the condition
                    switch (occur)
                    {
                        //New event case
                        case 1:
                            //Remove the EID hidden value signifying this will be a new event
                            HiddenEID.Value = "";
                            //Set the info label text
                            message = "Current Mode: Event Duplicating";
                            break;
                        //Event viewing case
                        case 2:
                            //Foreach label
                            foreach (WebControl adding in toStat)
                            {
                                //Add the button to the panel
                                lowerLeft.Controls.Add(adding);
                            }
                            //Set the info label text
                            message = "Current Mode: Event Viewing";
                            //Enable the components
                            setEnabled(false);
                            break;
                        //Event updated postback case
                        case 3:
                            //Foreach label
                            foreach (WebControl adding in toStat)
                            {
                                //Add the button to the panel
                                lowerLeft.Controls.Add(adding);
                            }
                            //Set the updated message
                            ((Label)(Master.FindControl("StatBottomT"))).Text = System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + " Event Updated";
                            break;
                        //Event created postback case
                        case 4:
                            //Foreach label
                            foreach (WebControl adding in toStat)
                            {
                                //Add the button to the panel
                                lowerLeft.Controls.Add(adding);
                            }
                            //Set the updated message
                            ((Label)(Master.FindControl("StatBottomT"))).Text = System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + " Event Created";
                            break;

                    }
                }
            }
        }
        else
        {
            //Set the info label text
            message = "Current Mode: Event Creation";
        }
        //Get the upper center label from the master page
        Label upperLabel = (Label)Master.FindControl("mHeaderLabelCenter");
        //Set title and css class
        upperLabel.Text = message;
        upperLabel.CssClass = "ForwardText";
    }

    //Populates the list boxes present
    private void populateBoxes(String selectedC, String selectedS)
    {
        //Get the categories from the Database
        List<Category> Cats = Database.select_EventCategory();
        List<Schedule> Sched = Database.select_EventSchedule();

        //Add a blank value to the array first <-- Added in asp to fasciliate validation
        //CategoryList.Items.Add("");
        //ScheduleList.Items.Add("");

        //Add the names of the categories to the list box
        foreach (Category c in Cats)
        {
            //Add the value to the array
            CategoryList.Items.Add(c.Name);
        }
        //Add the names of the categories to the list box
        foreach (Schedule s in Sched)
        {
            //Add the value to the array
            ScheduleList.Items.Add(s.Name);
        }
        //Set the selected item
        CategoryList.SelectedValue = selectedC;
        ScheduleList.SelectedValue = selectedS;
    }

    //This method controls the enabled status of the components
    private void setEnabled(bool Enabled)
    {
        //Set components enabled status according to passed in variable
        EventTitle.Enabled = Enabled;
        DocURL.Enabled = Enabled;
        URLTextBox.Enabled = Enabled;
        CategoryList.Enabled = Enabled;
        ScheduleList.Enabled = Enabled;
        OccurList.Enabled = Enabled;
        DateBox.Enabled = Enabled;
        DateBox.ReadOnly = (!Enabled);
        HourList.Enabled = Enabled;
        MinList.Enabled = Enabled;
        FinishTime.Enabled = Enabled;
        ActiveBox.Enabled = Enabled;
        StartDate.Enabled = Enabled;
        ToBox.Enabled = Enabled;
        CCBox.Enabled = Enabled;
        SubjectBox.Enabled = Enabled;
        BodyBox.Enabled = Enabled;
        EmailSave.Enabled = Enabled;
        ResetB.Enabled = Enabled;
        DailyList.Enabled = Enabled;
        //foreach item in the daily list
        foreach(ListItem item in DailyList.Items)
        {
            //disable the item
            item.Enabled = Enabled;
        }
    }

    //set the fields according to a record from the Database
    private void setFields(String EID)
    {
        //If the passed in value exists
        if (EID.Length > 0)
        {
            //Convert the EID to an integer
            int eid = int.Parse(EID);
            //Returned event
            Event result = Database.select_EventEdit(eid);
            //If the result is not null
            if (result.EID != 0)
            {
                //Set the event id hidden field
                HiddenEID.Value = result.EID.ToString();
                //set the event name
                EventTitle.Text = result.Name;
                //Set the url
                URLTextBox.Text = result.URL;
                //Set the documentation
                DocURL.Text = result.Documentation;
                //Set the category box
                CategoryList.SelectedValue = result.Category;
                //Set the hour drop down list
                HourList.SelectedValue = result.StartTime.ToString("HH");
                //Set the minute drope down list
                MinList.SelectedValue = result.StartTime.ToString("mm");
                //Set the date box date
                DateBox.Text = result.StartTime.ToString("MM/dd/yyyy");
                //Set the finish time checkbox
                FinishTime.Checked = result.FinishTime;
                //Set the active checkbox
                ActiveBox.Checked = result.Active;
                //Set the schedule box
                ScheduleList.SelectedValue = result.Schedule;
                //Pass the schedule box to the method responsible for event handling
                schedBox_Changed(ScheduleList, null);
                //If the schedule box is daily weekly or bi-weekly
                if (result.Schedule == "Daily" || result.Schedule == "Weekly" || result.Schedule == "Bi-Weekly")
                {
                    //String to hold schedule code
                    char[] days = result.ScheduleCode.ToCharArray();
                    //If the array has values
                    if (days.Length > 0)
                    {
                        //for each of the values in the array
                        foreach (char c in days)
                        {
                            //convert to an integer and subtract one
                            int index = (int.Parse(c.ToString()) - 1);
                            //set the checkbox in the checkboxlist to checked
                            ((ListItem)DailyList.Items[index]).Selected = true;
                        }
                        //If the schedule box is weekly or biweekly
                        if (result.Schedule == "Weekly" || result.Schedule == "Bi-Weekly")
                        {
                            //Set daily list readonly
                            DailyList.Enabled = false;
                            //set the image url
                            DailyListVal.ImageUrl = "~/Images/Partial.gif";
                        }
                    }
                }
                else if (result.Schedule == "Monthly")
                {
                    //Set the monthday list index to that of the schedule code
                    MonthDayList.SelectedIndex = int.Parse(result.ScheduleCode);
                }
                else if (result.Schedule == "Frequent Reoccurance")
                {
                    //Split the schedulecode into two variables
                    OccurList.SelectedIndex = (int.Parse(result.ScheduleCode.Substring(0, 1)) - 1);
                    //convert to an integer and subtract one
                    int index = (int.Parse(result.ScheduleCode.Substring(1, 1)) - 1);
                    //set the checkbox in the checkboxlist to checked
                    ((ListItem)DailyList.Items[index]).Selected = true;
                    //Set daily list readonly
                    DailyList.Enabled = false;
                    //set the image url
                    DailyListVal.ImageUrl = "~/Images/Partial.gif";
                }
                //if link is a mail to link
                if (result.URL.StartsWith("mailto"))
                {
                    //Url to break apart
                    string breakApart = result.URL;
                    //If the url contains to
                    if (breakApart.Contains("to:"))
                    {
                        //var to represent index
                        var index = breakApart.IndexOf("?");
                        //If the index is -1 set to length
                        if (index == -1) { index = breakApart.Length; }
                        //Set the to field
                        ToBox.Text = breakApart.Substring(breakApart.IndexOf("to:") + 3, (index - breakApart.IndexOf("to:") - 3));
                    }
                    //If the url contains cc
                    if (breakApart.Contains("cc="))
                    {
                        //var to represent index
                        var index = breakApart.IndexOf("&", breakApart.IndexOf("cc="));
                        //If the index is -1 set to length
                        if (index == -1) { index = breakApart.Length; }
                        //Set the to field
                        CCBox.Text = breakApart.Substring(breakApart.IndexOf("cc=") + 3, (index - breakApart.IndexOf("cc=") - 3));
                    }
                    //If the url contains to
                    if (breakApart.Contains("subject="))
                    {
                        //var to represent index
                        var index = breakApart.IndexOf("&", breakApart.IndexOf("subject="));
                        //If the index is -1 set to length
                        if (index == -1) { index = breakApart.Length; }
                        //Set the to field
                        SubjectBox.Text = breakApart.Substring(breakApart.IndexOf("subject=") + 8, (index - breakApart.IndexOf("subject=") - 8));
                    }
                    //If the url contains to
                    if (breakApart.Contains("body="))
                    {
                        //var to represent index
                        var index = breakApart.IndexOf("&", breakApart.IndexOf("body="));
                        //If the index is -1 set to length
                        if (index == -1) { index = breakApart.Length; }
                        //Set the to field
                        BodyBox.Text = breakApart.Substring(breakApart.IndexOf("body=") + 5, (index - breakApart.IndexOf("body=") - 5));
                    }
                }
            }
        }
    }

    //Deals with validation
    private void validation()
    {
        /*
         * Checks and changes the colour box to Green (valid) or Red (invalid)
         */
        //Check event name
        if (EventTitle.Text == "")
        {
            EventTitleVal.ImageUrl = "~/Images/Error.gif";
        }
        else
        {
            EventTitleVal.ImageUrl = "~/Images/Success.gif";
        }

        //Check doc URL
        if (DocURL.Text == "" && !(bool.Parse(DocForce.Value)))
        {
            DocVal.ImageUrl = "~/Images/Partial.gif";
        }
        else
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(DocURL.Text);
                request.Method = "GET";
                request.Credentials = CredentialCache.DefaultCredentials;
                request.PreAuthenticate = true;
                request.Timeout = 5000;
                DocValidation.IsValid = true;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                DocVal.ImageUrl = "~/Images/Success.gif";
            }
            catch (Exception e)
            {
                if (e.Message.ToString().Contains("401"))
                {
                    DocVal.ImageUrl = "~/Images/Success.gif";
                    DocValidation.IsValid = true;
                    DocPanel.Update();
                }
                else
                {
                    DocVal.ImageUrl = "~/Images/Error.gif";
                    DocValidation.IsValid = false;
                    DocPanel.Update();
                }
            }
            //Link completed validation without being forced
            DocForce.Value = "false";
        }
        //check url
        if (URLTextBox.Text == "")
        {
            URLVal.ImageUrl = "~/Images/Partial.gif";
            //Set text boxes to blank values
            ToBox.Text = "";
            CCBox.Text = "";
            SubjectBox.Text = "";
            BodyBox.Text = "";
        }
        else{
            //If the text doesnt start with mailto or is already valid
            if ((!URLTextBox.Text.StartsWith("mailto:")) && !(bool.Parse(UrlForce.Value)))
            {
                try
                {
                    if (URLTextBox.Text.StartsWith("\\"))
                    {
                        FileWebRequest request = (FileWebRequest)FileWebRequest.Create(URLTextBox.Text);
                        request.PreAuthenticate = true;
                        request.Credentials = CredentialCache.DefaultNetworkCredentials;
                        request.Timeout = 5000;
                        URLValidation.IsValid = true;
                        FileWebResponse response = (FileWebResponse)request.GetResponse();
                        URLVal.ImageUrl = "~/Images/Success.gif";
                    }
                    else
                    {
                        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URLTextBox.Text);
                        request.Method = "GET";
                        request.Credentials = CredentialCache.DefaultNetworkCredentials;
                        request.PreAuthenticate = true;
                        request.Timeout = 5000;
                        URLValidation.IsValid = true;
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        URLVal.ImageUrl = "~/Images/Success.gif";
                    }
                }
                catch (Exception e )
                {
                    if(e.Message.ToString().Contains("denied") || e.Message.ToString().Contains("401")){
                        URLVal.ImageUrl = "~/Images/Success.gif";
                        URLValidation.IsValid = true;
                        UrlPanel.Update();
                    }
                    else
                    {
                        URLVal.ImageUrl = "~/Images/Error.gif";
                        URLValidation.IsValid = false;
                        UrlPanel.Update();
                    }
                }
            }
            else if (URLTextBox.Text.StartsWith("mailto:"))
            {
                URLVal.ImageUrl = "~/Images/Success.gif";
                URLValidation.IsValid = true;
                UrlPanel.Update();
            }
            //Link completed validation successfully without being forced
            UrlForce.Value = "false";
        }
        //Check catagory
        if (CategoryList.SelectedIndex == 0)
        {
            CategoryListVal.ImageUrl = "~/Images/Error.gif";
        }
        else
        {
            CategoryListVal.ImageUrl = "~/Images/Success.gif";
        }
        //Check catagory
        if (ScheduleList.SelectedValue == "Daily")
        {
            int checkedCount = 0;
            foreach (ListItem items in DailyList.Items)
            {
                if (items.Selected)
                {
                    checkedCount++;
                }
            }
            if (checkedCount == 0)
            {
                DailyListVal.ImageUrl = "~/Images/Error.gif";
            }
            else
            {
                DailyListVal.ImageUrl = "~/Images/Success.gif";
            }

        }
        if (ScheduleList.SelectedValue == "Frequent Reoccurance")
        {
            int checkedCount = 0;
            foreach (ListItem items in DailyList.Items)
            {
                if (items.Selected)
                {
                    checkedCount++;
                }
            }
            if (checkedCount == 0)
            {
                DailyListVal.ImageUrl = "~/Images/Error.gif";
            }
            else
            {
                DailyListVal.ImageUrl = "~/Images/Success.gif";
            }

        }
        if (ScheduleList.SelectedIndex == 0)
        {
            ScheduleListVal.ImageUrl = "~/Images/Error.gif";
        }
        else
        {
            ScheduleListVal.ImageUrl = "~/Images/Success.gif";
        }

        //Check date
        if (DateBox.Text == "")
        {
            DateBoxVal.ImageUrl = "~/Images/Error.gif";
        }
        else
        {
            DateBoxVal.ImageUrl = "~/Images/Success.gif";
        }
    }


//**Region - Event Related Methods**
    //Occurs when the radio button list value changes on the grid panel
    protected void DateList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Cast thesender object to a image button
        RadioButtonList btn = (RadioButtonList)sender;
        //get the value of the selected radio 
        int occur = -1;
        //Try parsing the value
        int.TryParse(btn.SelectedValue, out occur);
        //If the id is a number greater than 0 and not null Set the occur value
        if (occur != 0) { occur = int.Parse(btn.SelectedValue); }
        //get the value of the methodID 
        int funcID = int.Parse(methodID.Value);
        //switch depending on method id
        switch (funcID)
        {
            case 1:
                //Bind data to the grid
                GridPanGrid.DataSource = Database.select_EventPerformance(occur, int.Parse(HiddenEID.Value),new DateTime(1900,1,1),new DateTime(1900,1,1));
                GridPanGrid.DataBind();
                break;
            case 2:
                //Bind data to the grid
                GridPanGrid.DataSource = Database.select_Comment(occur, int.Parse(HiddenEID.Value), new DateTime(1900, 1, 1), new DateTime(1900, 1, 1),false);
                GridPanGrid.DataBind();
                break;
            case 3:
                //Bind data to the grid
                GridPanGrid.DataSource = Database.select_EventAudit(occur, int.Parse(HiddenEID.Value), new DateTime(1900, 1, 1), new DateTime(1900, 1, 1));
                GridPanGrid.DataBind();
                break;
        }
        //Set the selected index to -1
        GridList.SelectedIndex = -1;
        //Set the text of the label
        TitleLabel.Text = TitleH.Value;
        GridEventTitle.Text = EventTitleH.Value;
        //Update the update panel
        GridPanUpper.Update();
        
    }

    //Ensure the day list button is correct if the appropriate schedule is selected
    protected void DateBox_TextChanged(object sender, EventArgs e)
    {
        //Ensure the daily list has the right value set
        checkDailyList();
    }

    //Occurs when the grid panel submit button is clicked
    protected void GridDateButton_Click(object sender, EventArgs e)
    {
        //Regex to ensure date is correct format
        Regex dateTest = new Regex("\\d\\d\\/\\d\\d\\/\\d\\d\\d\\d");
        //If the end date is not a valid format
        if (!dateTest.IsMatch(GridEndBox.Text))
        {
            //Set the background of the datebox to red
            GridEndBox.BackColor = System.Drawing.Color.Red;
        }
        //If the start date is not a valid format
        else if (!dateTest.IsMatch(GridStartBox.Text))
        {
            //Set the background of the datebox to red
            GridStartBox.BackColor = System.Drawing.Color.Red;
        }
        //else submit
        else
        {
            //Dates to pass out
            DateTime StartDateOut = new DateTime(); DateTime EndDateOut = new DateTime();
            //boolean indicating to process
            Boolean process = true;
            //Get the date from the end date box
            int startday = int.Parse(GridStartBox.Text.Split("/".ToCharArray())[1]);
            int startmonth = int.Parse(GridStartBox.Text.Split("/".ToCharArray())[0]);
            int startyear = int.Parse(GridStartBox.Text.Split("/".ToCharArray())[2]);
            //Get the date from the end date box
            int endday = int.Parse(GridEndBox.Text.Split("/".ToCharArray())[1]);
            int endmonth = int.Parse(GridEndBox.Text.Split("/".ToCharArray())[0]);
            int endyear = int.Parse(GridEndBox.Text.Split("/".ToCharArray())[2]);
            //If the start day and month are valid contruct a valid datetime
            if(dateCheck(startmonth,startday)){StartDateOut = new DateTime(startyear,startmonth,startday,00,00,00);}
            //set process to false and background to red
            else { process = false; GridStartBox.BackColor = System.Drawing.Color.Red; }
            //If the start day and month are valid contruct a valid datetime
            if (dateCheck(endmonth, endday)) { EndDateOut = new DateTime(endyear, endmonth, endday, 23, 59, 59); }
            //set process to false and background to red
            else { process = false; GridEndBox.BackColor = System.Drawing.Color.Red; }
            //If process is true
            if (process == true)
            {
                //get the value of the methodID 
                int funcID = int.Parse(methodID.Value);
                //switch depending on method id
                switch (funcID)
                {
                    case 1:
                        //Bind data to the grid
                        GridPanGrid.DataSource = Database.select_EventPerformance(0, int.Parse(HiddenEID.Value), StartDateOut, EndDateOut);
                        GridPanGrid.DataBind();
                        break;
                    case 2:
                        //Bind data to the grid
                        GridPanGrid.DataSource = Database.select_Comment(0, int.Parse(HiddenEID.Value), StartDateOut, EndDateOut, false);
                        GridPanGrid.DataBind();
                        break;
                    case 3:
                        //Bind data to the grid
                        GridPanGrid.DataSource = Database.select_EventAudit(0, int.Parse(HiddenEID.Value), StartDateOut, EndDateOut);
                        GridPanGrid.DataBind();
                        break;
                }
                //Set the text of the label
                TitleLabel.Text = TitleH.Value;
                //Update the update panel
                GridPanUpper.Update();
            }
        }
    }

    //Occurs when the type list in the grid panel changes
    protected void GridTypeList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //If Occur checkbox is checked
        if (GridTypeList.SelectedIndex == 0)
        {
            //Show the right components
            GridList.Visible = true;
            //Hide the appropriate components
            GridStartBox.Visible = false;
            GridEndBox.Visible = false;
            GridDateButton.Visible = false;
            GridInfoLabel.Text = "Number of Occurances";
            //Set the index of the radio list
            GridList.SelectedIndex = -1;
            
        }
        else if (GridTypeList.SelectedIndex == 1)
        {
            //Hide the appropriate components
            GridStartBox.Visible = true;
            GridEndBox.Visible = true;
            GridDateButton.Visible = true;
            //Show the right components
            GridList.Visible = false;
            //Set text boxes blank
            GridInfoLabel.Text = "Start Date - End Date";
            GridStartBox.Text = "";
            GridEndBox.Text = "";
        }
        //Set the grid to null
        GridPanGrid.DataSource = null;
        GridPanGrid.DataBind();
        //Set title components
        TitleLabel.Text = TitleH.Value;
        GridEventTitle.Text = EventTitleH.Value;
        //Update the update panel
        GridPanUpper.Update();
    }

    //This method is called when the index changes in the 
    //schedule box and shows/hides the appropriate controls
    protected void schedBox_Changed(object sender, EventArgs e)
    {
        //Get the current date from the sending calendar
        DropDownList box = (DropDownList)sender;
        //Hide all elements
        //hide the last day check box 
        MonthDayList.Visible = false;
        //hide the daily list
        DailyList.Visible = false;
        //Show the val image
        DailyListVal.Visible = false;
        //hide the occurance box
        OccurList.Visible = false;
        //Unlock the datebox
        DateBox.Enabled = true;
        //if the value is set to monthly
        if (box.SelectedItem.Value == "Monthly")
        {
            //Show the last day check box
            MonthDayList.Visible = true;
        }
        else if (box.SelectedItem.Value == "Daily" || box.SelectedItem.Value == "Weekly" || box.SelectedItem.Value == "Bi-Weekly")
        {
            //Show the list
            DailyList.Visible = true;
            //Show the val image
            DailyListVal.Visible = true;
            //if weekly or bi weekly
            if (box.SelectedItem.Value == "Weekly" || box.SelectedItem.Value == "Bi-Weekly")
            {
                //Set the checkbox list to readonly
                DailyList.Enabled = false;
                //Set the image url
                DailyListVal.ImageUrl = "~/Images/Partial.gif";
            }
            //ensure the correct value is set if the date exists
            checkDailyList();
        }
        else if (box.SelectedItem.Value == "Frequent Reoccurance")
        {
            //Show the list
            DailyList.Visible = true;
            //Show the val image
            DailyListVal.Visible = true;
            //Show the occurance box
            OccurList.Visible = true;
        }
        //Update the update panel
        ListPanel.Update();
    }

    //This submits changes from the event page to the Database
    protected void submit_Event(object sender, EventArgs e)
    {
        if ((EventTitleVal.ImageUrl == "~/Images/Error.gif" && EventTitleVal.Visible == true)
            || (!DocValidation.IsValid && DocVal.Visible == true)
            || (!URLValidation.IsValid && URLVal.Visible == true)
            || (CategoryListVal.ImageUrl == "~/Images/Error.gif" && CategoryListVal.Visible == true)
            || (ScheduleListVal.ImageUrl == "~/Images/Error.gif" && ScheduleListVal.Visible == true)
            || (DailyListVal.ImageUrl == "~/Images/Error.gif" && DailyListVal.Visible == true)
            || (DateBoxVal.ImageUrl == "~/Images/Error.gif" && DateBoxVal.Visible == true))
        {
            return;
        }

        //Variables of current values
        String schedCode = "";
        String name = EventTitle.Text;
        String doc = DocURL.Text;
        String url = URLTextBox.Text;
        String category = CategoryList.SelectedValue;
        String schedule = ScheduleList.SelectedValue;
        Boolean active = ActiveBox.Checked;
        Boolean finish = FinishTime.Checked;

        //Array to hold split date portions of the date box
        String[] dates = DateBox.Text.Split("/".ToCharArray());
        //Create a new date time object from the datebox and hour and minute lists
        DateTime startTime = new DateTime(int.Parse(dates[2]), int.Parse(dates[0]), int.Parse(dates[1]),
                            int.Parse(HourList.SelectedValue), int.Parse(MinList.SelectedValue), 00);

        //If daily is the current schedule
        if (ScheduleList.SelectedValue == "Daily")
        {
            //Foreach checkbox in the list
            for (int i = 0; i < DailyList.Items.Count; i++)
            {
                //If the box is checked
                if (((ListItem)DailyList.Items[i]).Selected == true)
                {
                    //Add the current index value plus 1 to the schedcode indicating the day of the week
                    schedCode += (i + 1).ToString();
                }
            }
        }
        else if (ScheduleList.SelectedValue == "Weekly" || ScheduleList.SelectedValue == "Bi-Weekly")
        {
            //Assign the day of the week variable to the schedCode variable
            schedCode = (((int)startTime.DayOfWeek)+1).ToString();

        }
        else if (ScheduleList.SelectedValue == "Monthly")
        {
            //Assign the month variable to the schedCode variable
            schedCode = MonthDayList.SelectedIndex.ToString();
        }
        else if (ScheduleList.SelectedItem.Value == "Frequent Reoccurance")
        {
            //Assign the occurance vairable plus the daily variable to schedCode
            schedCode = (OccurList.SelectedIndex + 1).ToString();
            //Foreach checkbox in the list
            for (int i = 0; i < DailyList.Items.Count; i++)
            {
                //If the box is checked
                if (((ListItem)DailyList.Items[i]).Selected == true)
                {
                    //Add the current index value plus 1 to the schedcode indicating the day of the week
                    schedCode += (i + 1).ToString();
                }
            }
        }
        if (HiddenEID.Value.Length > 0)
        {
            //Convert the EID to an integer
            int eid = int.Parse(HiddenEID.Value.ToString());
            //Returned event
            Event result = Database.select_EventEdit(eid);
            //If the EID exists in the database
            if (result.EID != 0)
            {
                //Call the update method
                Database.update_Event(int.Parse(HiddenEID.Value.ToString()), category, schedule, name, finish, doc, url, active, startTime, schedCode, "Event Updated", Page.User.Identity.Name.ToString());
                Response.Redirect(("~/Views/Event.aspx?" + HiddenEID.Value + "&3"));
            }
            else
            {
                //Go to the event creation page
                Response.Redirect(("~/Views/Event.aspx?"));
            }
        }
        else
        {
            //Create a new event
            String eid = Database.add_Event(category, schedule, name, finish, doc, url, active, startTime, schedCode, "Event Created", Page.User.Identity.Name.ToString());
            //redirect to this page
            Response.Redirect(("~/Views/Event.aspx?" + eid + "&4"));
        }

    }

    //Occurs when the validation box is clicked
    protected void ValidateButton_Click(object sender, EventArgs e)
    {
        //If the url validation image is invalid, set it valid
        if (!URLValidation.IsValid)
        {
            //Set image
            URLVal.ImageUrl = "~/Images/Success.gif";
            //Set valid
            URLValidation.IsValid = true;
            //Set hidden field
            UrlForce.Value = "true";
            //Update the panel
            UrlPanel.Update();
        }
    }
 
}