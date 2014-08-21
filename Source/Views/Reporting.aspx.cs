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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Globalization;

public partial class Views_Reporting : System.Web.UI.Page
{
//**Region - Page Related Variables**
    //Create a new database object
    Database Database = new Database();

//**Region - Page Related Methods**
    //Occurs pre page load
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
            //Populate the category box
            populateBoxes("All");
            //Register triggers
            triggers();
        }
        //Check the boxes to ensure valid dates
        checkBoxes();
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
    //This method is used to setup the textboxes on the page
    protected void checkBoxes()
    {
        //Regex to ensure date is correct format
        Regex dateTest = new Regex("\\d\\d\\/\\d\\d\\/\\d\\d\\d\\d");
        //If the startdatebox has data
        if (DateBoxStart.Text.Length > 0)
        {
            //If the date is not valid
            if (!dateTest.IsMatch(DateBoxStart.Text))
            {
                //set the background color to red
                DateBoxStart.BackColor = System.Drawing.Color.Red;
            }
            else
            {
                //Set the background color to light grey
                DateBoxStart.BackColor = System.Drawing.Color.LightGray;
            }
        }
        //If the enddatebox has data
        if (DateBoxEnd.Text.Length > 0)
        {
            //If the date is not valid
            if (!dateTest.IsMatch(DateBoxEnd.Text))
            {
                //set the background color to red
                DateBoxEnd.BackColor = System.Drawing.Color.Red;
            }
            else
            {
                //Set the background color to light grey
                DateBoxEnd.BackColor = System.Drawing.Color.LightGray;
            }
        }
    }

    //This method creates the controls for the masterpage
    private ImageButton[] createControls()
    {
        //BUtton array to return
        ImageButton[] returnArray = new ImageButton[1];
        
        //Create new image button
        ImageButton GenerateReport = new ImageButton();
        //set the button Css class
        GenerateReport.CssClass = "LowerControlButtons";
        //Set the buttons ID
        GenerateReport.ID = "EnterButton";
        //Set the buttons tooltip
        GenerateReport.ToolTip = "Generate Report";
        //Set the buttons imageurl
        GenerateReport.ImageUrl = "~/Images/CreateNew.gif";
        //set image buttons event handler
        GenerateReport.Click += new ImageClickEventHandler(submit_Report);
        //Add buttons to the array
        returnArray[0] = GenerateReport;
        //return the array
        return returnArray;
    } 

    //Create date time objects from boxes
    private DateTime[] create_dates(Boolean inBool)
    {
        //Array to return
        DateTime[] returnTimes = new DateTime[2];
        //Regex to ensure date is correct format
        Regex dateTest = new Regex("\\d\\d\\/\\d\\d\\/\\d\\d\\d\\d");
        //Final datetime variables for start and end times
        //1900,1,1 represents an invalid date. If this date is passed to the SQL stored procedure
        //It will be ignored
        DateTime StartTime = new DateTime(1900, 1, 1);
        DateTime EndTime = new DateTime(1900, 1, 1);
        //if in bool is true meaning use both datetime boxes to construct start and end times
        if (inBool == true)
        {
            //if the datetime is the correct format
            if (dateTest.IsMatch(DateBoxEnd.Text) && dateTest.IsMatch(DateBoxStart.Text))
            {
                //Construct integers for time and date variables
                int startHour = int.Parse(HourListStart.SelectedItem.Text);
                int startMin = int.Parse(MinListStart.SelectedItem.Text);
                int endHour = int.Parse(HourListEnd.SelectedItem.Text);
                int endMin = int.Parse(MinListEnd.SelectedItem.Text);
                int startday = int.Parse(DateBoxStart.Text.Split("/".ToCharArray())[1]);
                int startmonth = int.Parse(DateBoxStart.Text.Split("/".ToCharArray())[0]);
                int startyear = int.Parse(DateBoxStart.Text.Split("/".ToCharArray())[2]);
                int endday = int.Parse(DateBoxEnd.Text.Split("/".ToCharArray())[1]);
                int endmonth = int.Parse(DateBoxEnd.Text.Split("/".ToCharArray())[0]);
                int endyear = int.Parse(DateBoxEnd.Text.Split("/".ToCharArray())[2]);
                //Create datetime objects
                StartTime = new DateTime(startyear, startmonth, startday, startHour, startMin, 0);
                EndTime = new DateTime(endyear, endmonth, endday, endHour, endMin, 0);
            }
            else
            {
                if (!dateTest.IsMatch(DateBoxStart.Text))
                {
                    //Set the background of the datebox to red
                    DateBoxStart.BackColor = System.Drawing.Color.Red;
                }
                if (!dateTest.IsMatch(DateBoxEnd.Text))
                {
                    //Set the background of the datebox to red
                    DateBoxEnd.BackColor = System.Drawing.Color.Red;
                }
            }
        }
        else
        {
            //if the datetime is the correct format
            if (dateTest.IsMatch(DateBoxEnd.Text))
            {
                //Get the date from the date box
                int endday = int.Parse(DateBoxEnd.Text.Split("/".ToCharArray())[1]);
                int endmonth = int.Parse(DateBoxEnd.Text.Split("/".ToCharArray())[0]);
                int endyear = int.Parse(DateBoxEnd.Text.Split("/".ToCharArray())[2]);
                //If dayshift is selected
                if (ShiftList.SelectedItem.ToString() == "DayShift")
                {
                    //Set the start time
                    StartTime = new DateTime(endyear, endmonth, endday, 08, 00, 00);
                    //Set the end time
                    EndTime = new DateTime(endyear, endmonth, endday, 19, 59, 59);
                }
                else
                {
                    //Set the start time
                    StartTime = new DateTime(endyear, endmonth, endday, 20, 00, 00);
                    //Set the end time
                    EndTime = new DateTime(endyear, endmonth, endday, 07, 59, 59);
                    //increment the end time by 1 day
                    EndTime = EndTime.AddDays(1);
                }
                
            }
            else
            {
                //Set the background of the datebox to red
                DateBoxEnd.BackColor = System.Drawing.Color.Red;
            }
        }
        //Add times to return array
        returnTimes[0] = StartTime;
        returnTimes[1] = EndTime;
        //Return the array of times
        return returnTimes;
    }

    //This method creates the status objects for the masterpage
    private WebControl[] createStatus()
    {
        //BUtton array to return
        WebControl[] returnArray = new WebControl[2];
        //Create status images        
        Image AllStat = new Image();
        //set status image ids
        AllStat.ID = "AllStat";
        //set status image urls
        AllStat.ImageUrl = "~/Images/Allevents.gif";
        //set status image tooltips
        AllStat.ToolTip = "Count of all events";
        //Set the css class
        AllStat.CssClass = "LowerControlStatus";
        //Create status images        
        Label AllT = new Label();
        //set status image ids
        AllT.ID = "AllT";
        //Set the css class
        AllT.CssClass = "StatusTextCount";
        AllT.Text = "0 events";
        //addcomponents in the correct order
        returnArray[0] = AllStat;
        returnArray[1] = AllT;
        //return the array
        return returnArray;
    }

    //This method sets masterpage variables
    private void loadMasterInfo()
    {
        //Set the Page title
        Page.Title = "Event Reporting";
        //Set the header text
        ((Label)Master.FindControl("mHeaderLabel")).Text = "Event Reporting";
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
        WebControl[] toAddCon = createStatus();
        //foreach buttton
        foreach (WebControl adding in toAddCon)
        {
            //Add the button to the panel
            lowerLeft.Controls.Add(adding);
        }
    }

    //Populates the main table with data from the SQL Database
    private void populateGrid(String Category, DateTime StartTime, DateTime EndTime)
    {
        //Get the status label from the master panel
        Label AllT = (Label)(Master.FindControl("AllT"));
        if (EventAudit.Checked)
        {
            //Disable/enable grids
            ManageGrid.Enabled = false;
            ManageGrid.Visible = false;
            AuditGrid.Enabled = true;
            AuditGrid.Visible = true;
            //New datatable object
            System.Data.DataTable dt = Database.select_AuditReport(Category, StartTime, EndTime);
            //Set the data source
            AuditGrid.DataSource = dt;
            //Count the events
            AllT.Text = dt.Rows.Count.ToString() + " events";
            //Bind the datatable to the grid
            AuditGrid.DataBind();
        }
        else
        {
            //Disable/enable grids
            ManageGrid.Enabled = true;
            ManageGrid.Visible = true;
            AuditGrid.Enabled = false;
            AuditGrid.Visible = false;
            //New datatable object
            System.Data.DataTable dt = Database.select_Reporting(Category);
            //Set the data source
            ManageGrid.DataSource = dt;
            //Count the events
            AllT.Text = dt.Rows.Count.ToString() + " events";
            //Bind the datatable to the grid
            ManageGrid.DataBind();
        }
        //Update the gridview panels
        GridViewPanel2.Update();
        GridViewPanel.Update();
    }

    //Populates the list boxes present
    private void populateBoxes(String selectedC)
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

        //Get the categories from the Database
        List<Category> Cats = Database.select_EventCategory();
        List<Schedule> Sched = Database.select_EventSchedule();
        
        //Add the all value to the array first
        CatList.Items.Add("All");

        //Add the names of the categories to the list box
        foreach(Category c in Cats){
            //Add the value to the array
            CatList.Items.Add(c.Name);
        }

        //Set the selected item
        CatList.SelectedValue = selectedC;
        
        //Set the hour source for start hour
        HourListStart.DataSource = hours;
        //set the minute source for start min
        MinListStart.DataSource = minutes;
        //Set the hour source for end hour
        HourListEnd.DataSource = hours;
        //set the minute source for end min
        MinListEnd.DataSource = minutes;

        //Bind the data sources
        HourListStart.DataBind();
        MinListStart.DataBind();
        HourListEnd.DataBind();
        MinListEnd.DataBind();

        //Populate the shift box
        ShiftList.Items.Add("DayShift");
        ShiftList.Items.Add("NightShift");
    }

    //This method disables or enables the time based components
    private void TimeBoxStatus(Boolean inBool)
    {
        //Set the enabled status of time components
        DateBoxStart.Enabled = inBool;
        HourListStart.Enabled = inBool;
        MinListStart.Enabled = inBool;
        HourListEnd.Enabled = inBool;
        MinListEnd.Enabled = inBool;
        StartTimeLabel.Enabled = inBool;
        EndDateLabel.Enabled = inBool;
    }

    //This method registers triggers with update script managers and update panels
    private void triggers()
    {
        //Get the enter button from the master page
        ImageButton EnterButton = (ImageButton)Master.FindControl("EnterButton");
        //Create a new async trigger
        AsyncPostBackTrigger trig = new AsyncPostBackTrigger();
        //Get the id of the enter button
        trig.ControlID = EnterButton.ClientID;
        //Register the trigger with update panels
        GridViewPanel.Triggers.Add(trig);
        GridViewPanel2.Triggers.Add(trig);
        //Register triggers
        ((ScriptManager)Master.FindControl("MasterScriptManager")).RegisterAsyncPostBackControl(EnterButton);
        ((ScriptManager)Master.FindControl("MasterScriptManager")).RegisterAsyncPostBackControl(CatList);
    }


//**Region - Event Related Methods**
    //Occurs everytime a row is bound to the audit grid
    protected void AuditGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //Set the row display
        controlRowDisplay(sender, e);
    }

    //This method is called when the index changes in the category box 
    protected void cat_Changed(object sender, EventArgs e)
    {
        //Dates
        DateTime[] dates = create_dates(FreeC.Checked);
        //Populate the grid according to the results
        populateGrid(CatList.SelectedItem.Text, dates[0], dates[1]);
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
            //Get the values from the row
            String UrlS = "";
            String DocS = "";
            String CatCol = "";
            //Get the object array behind it
            Object[] Values = rowView.Row.ItemArray;
            //If audit is checked
            if (EventAudit.Checked)
            {
                //Event id hidden field
                HiddenField EID = (HiddenField)dataRow.FindControl("EIDH");
                //Forward check box
                CheckBox For = (CheckBox)dataRow.FindControl("ForCheck");
                //Get the values from the row
                UrlS = Values[9].ToString();
                DocS = Values[10].ToString();
                CatCol = Values[13].ToString();
                EID.Value = Values[0].ToString();
                //if the length of the forward field is > 0
                if (Values[14].ToString().Length > 0)
                {
                    //Set the forward box
                    For.Checked = bool.Parse(Values[14].ToString());
                }
                //If a color exists for the category
                if (CatCol.Length > 0)
                {
                    //Set the color
                    dataRow.Cells[7].ForeColor = System.Drawing.ColorTranslator.FromHtml("#" + CatCol);
                }
            }
            else
            {
               //Get the values from the row
                UrlS = Values[5].ToString();
                DocS = Values[6].ToString();
                CatCol = Values[7].ToString();
                //If a color exists for the category
                if (CatCol.Length > 0)
                {
                    //Set the color
                    dataRow.Cells[3].ForeColor = System.Drawing.ColorTranslator.FromHtml("#" + CatCol);
                }
            }

            //If a document is provided
            if (DocS.Length > 0)
            {
                //Get the url image
                Image DocImg = (Image)dataRow.FindControl("DocLinkImg");
                //Get the doc hidden field
                HyperLink Doc = (HyperLink)dataRow.FindControl("DocB");
                //set the doc hidden field
                Doc.NavigateUrl = DocS;
                //Set the image url
                DocImg.ImageUrl = "~/Images/Doc1.gif";
                //Add tool tip
                Doc.ToolTip = "Documentation Link";
            }
            //If a url is provided
            if (UrlS.Length > 0)
            {
                //Get the url image
                Image URLImg = (Image)dataRow.FindControl("SiteLinkImg");
                //Get the url hyperlink
                HyperLink URLAdd = (HyperLink)dataRow.FindControl("UrlB");
                if (UrlS.StartsWith("mailto:"))
                {
                    //Set the image to the mail icon
                    URLImg.ImageUrl = "~/Images/Email.gif";
                    //Add tool tip
                    URLImg.ToolTip = "Email Link";
                    
                }
                else
                {
                    //Set the image url
                    URLImg.ImageUrl = "~/Images/Link.gif";
                    //Add tool tip
                    URLImg.ToolTip = "Site Link";
                }
                //set the doc hidden field
                URLAdd.NavigateUrl = UrlS;
            }
        }

    }

    //This occurs when eventlistc shift selection checkboxes is changed
    protected void EventListC_CheckedChanged(object sender, EventArgs e)
    {
        //If EventListC is checked
        if (EventListC.Checked == true)
        {
            //Set label text
            StartDateTitle.Text = "Shift Type";
            EndDateLabel.Text = "Shift Date";
            //Hide the date start box
            DateBoxStart.Visible = false;
            //Disable components
            TimeBoxStatus(false);
            //Enable and unhide the shift drop box
            ShiftList.Enabled = true;
            ShiftList.Visible = true;
            //check the opposing box
            FreeC.Checked = false;
        }
        else
        {
            //Set label text
            StartDateTitle.Text = "Start Date";
            EndDateLabel.Text = "End Date";
            //UnHide the date start box
            DateBoxStart.Visible = true;
            //Enable components
            TimeBoxStatus(true);
            //Enable and unhide the shift drop box
            ShiftList.Enabled = false;
            ShiftList.Visible = false;
            //Uncheck the opposing box
            FreeC.Checked = true;
        }
        //Update the upper panel
        UpdatePanel1.Update();
    }

    //This occurs when eventlistc shift selection checkboxes is changed
    protected void FreeC_CheckedChanged(object sender, EventArgs e)
    {
        //If EventListC is checked
        if (FreeC.Checked == false)
        {
            //Set label text
            StartDateTitle.Text = "Shift Type";
            EndDateLabel.Text = "Shift Date";
            //Disable components
            TimeBoxStatus(false);
            //Hide the date start box
            DateBoxStart.Visible = false;
            //Enable and unhide the shift drop box
            ShiftList.Enabled = true;
            ShiftList.Visible = true;
            //check the opposing box
            EventListC.Checked = true;
        }
        else
        {
            //Set label text
            StartDateTitle.Text = "Start Date";
            EndDateLabel.Text = "End Date";
            //Hide the date start box
            DateBoxStart.Visible = true;
            //Eanble components
            TimeBoxStatus(true);
            //Enable and unhide the shift drop box
            ShiftList.Enabled = false;
            ShiftList.Visible = false;
            //Uncheck the opposing box
            EventListC.Checked = false;
        }
        //Update the upper panel
        UpdatePanel1.Update();
    }

    //Occurs when the grid panel submit button is clicked
    protected void GridDateButton_Click(object sender, EventArgs e)
    {
        //Dates to pass out
        DateTime StartDateOut = new DateTime(); DateTime EndDateOut = new DateTime();
        //Start variables
        int startday; int startmonth; int startyear; int starthour;
        //End Variables
        int endday; int endmonth; int endyear; int endhour;
        //If select by time is checked
        if (FreeC.Checked == false)
        {
            //Get the date from the end date box
            startday = int.Parse(DateBoxEnd.Text.Split("/".ToCharArray())[1]);
            startmonth = int.Parse(DateBoxEnd.Text.Split("/".ToCharArray())[0]);
            startyear = int.Parse(DateBoxEnd.Text.Split("/".ToCharArray())[2]);
            //If day shift is selected
            if (ShiftList.SelectedIndex == 0)
            {
                //Set hours
                starthour = 8;
                endhour = 20;
                //Create start end date times
                StartDateOut = new DateTime(startyear, startmonth, startday, starthour, 0, 0);
                EndDateOut = new DateTime(startyear, startmonth, startday, endhour, 0, 0);
            }
            else
            {
                //Set hours
                starthour = 20;
                endhour = 8;
                //Create start end date times
                StartDateOut = new DateTime(startyear, startmonth, startday, starthour, 0, 0);
                EndDateOut = new DateTime(startyear, startmonth, startday, endhour, 0, 0);
                //Add a day to the endDateOut
                EndDateOut = EndDateOut.AddDays(1);
            }
        }
        else
        {
            //Get the date from the end date box
            startday = int.Parse(DateBoxStart.Text.Split("/".ToCharArray())[1]);
            startmonth = int.Parse(DateBoxStart.Text.Split("/".ToCharArray())[0]);
            startyear = int.Parse(DateBoxStart.Text.Split("/".ToCharArray())[2]);
            //Get the date from the end date box
            endday = int.Parse(DateBoxEnd.Text.Split("/".ToCharArray())[1]);
            endmonth = int.Parse(DateBoxEnd.Text.Split("/".ToCharArray())[0]);
            endyear = int.Parse(DateBoxEnd.Text.Split("/".ToCharArray())[2]);
            //Create start end date times
            StartDateOut = new DateTime(startyear, startmonth, startday, int.Parse(HourListStart.SelectedItem.Value.ToString()), int.Parse(MinListStart.SelectedItem.Value.ToString()), 0);
            EndDateOut = new DateTime(endyear, endmonth, endday, int.Parse(HourListEnd.SelectedItem.Value.ToString()), int.Parse(MinListEnd.SelectedItem.Value.ToString()), 0);
        }
        //Bind data to the grid
        DataTable dt = Database.select_Comment(0, int.Parse(CEIDH.Value), StartDateOut, EndDateOut,false);
        ComGrid.DataSource = dt;
        //Set the comment count
        ComCount.Text = (dt.Rows.Count).ToString() + " Comments";
        ComGrid.DataBind();
        //Update the update panel
        ComData.Update();
    }

    //This method is called eveytime a row is bound to the data grid
    protected void ManageGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //Set the row display
        controlRowDisplay(sender,e);
    }

    //This method is called when the submit report button is clicked
    protected void submit_Report(object sender, EventArgs e)
    {
        //Get the selected category
        String Cat = CatList.SelectedItem.Text;
        //Dates
        DateTime[] dates = new DateTime[2];
        if (FreeC.Checked == true)
        {
            //Call the create dates method
            dates = create_dates(true);
        }
        else
        {
            //Call the create dates method
            dates = create_dates(false);
        }
        //If both dates are valid by not having a year of 1900
        if (!(dates[0].Year == 1900) && !(dates[1].Year == 1900))
        {
            //if Event audit is checked
            if (EventAudit.Checked)
            {
                //Select the results
                populateGrid(Cat, dates[0], dates[1]);
            }
            else
            {
                //Create the report list
                Database.create_Report(dates[0], dates[1]);
                //Select the results
                populateGrid(Cat, dates[0], dates[1]);
            }
        }
        else
        {
            //If the date is bad
            if ((dates[0].Year == 1900))
            {
                //set the color of the box to red
                DateBoxStart.BackColor = System.Drawing.Color.Red;
            }
            else if ((dates[1].Year == 1900))
            {
                //set the color of the box to red
                DateBoxEnd.BackColor = System.Drawing.Color.Red;
            }
        }
        //Update the upper panel
        UpdatePanel1.Update();
    }

}