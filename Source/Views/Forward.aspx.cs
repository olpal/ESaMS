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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Threading;
using System.Globalization;

public partial class Views_Forward : System.Web.UI.Page
{
    //Create a new database object
    Database Database = new Database();

//**Region - Page Related Methods**
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
        //Load master file variables
        loadMasterInfo();
    }

    //Occurs when the page laods
    protected void Page_Load(object sender, EventArgs e)
    {
        //If the load is not a postback
        if (!Page.IsPostBack)
        {
            //Populate the Main table
            populateGrid();        
        }
        MaintainScrollPositionOnPostBack = true;
    }


//**Region - Data Processing Methods**
    //This method selects and binds comment data to the comment gridview
    protected void bindData(int count)
    {
        //Get the eid from the hidden field
        int EID = int.Parse(CEIDH.Value);
        //Get the datatable
        DataTable dt = Database.select_Comment(count, EID, new DateTime(1900, 1, 1), new DateTime(1900, 1, 1),false);
        //Create the data table
        ComGrid.DataSource = dt;
        //Bind the data
        ComGrid.DataBind();
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
        AllT.Text = "0 total events";
        //addcomponents in the correct order
        returnArray[0] = AllStat;
        returnArray[1] = AllT;
        //return the array
        return returnArray;
    }

    //This method creates the controls for the masterpage
    private WebControl[] createControls()
    {
        //BUtton array to return
        WebControl[] returnArray = new WebControl[1];
        //create buttons to add
        ImageButton Submit = new ImageButton();
        //set the button Css class
        Submit.CssClass = "LowerControlButtons";
        //Set the buttons ID
        Submit.ID = "EnterButton";
        //Set the buttons tooltip
        Submit.ToolTip = "Accept Forwarded events";
        //Set the buttons imageurl
        Submit.ImageUrl = "~/Images/Ok.gif";
        //Set the buttons causesvalidation to false
        Submit.CausesValidation = false;
        //Set the button invisible
        Submit.Visible = false;
        //Set onclient click methods
        Submit.Click += create_EventList;
        //Add buttons to the array
        returnArray[0] = Submit;
        //return the array
        return returnArray;
    }

    //Used to control edit mode
    private void editMode(Boolean edit, int index)
    {
        //Get the datarow in question
        GridViewRow dataRow = ComGrid.Rows[index];
        //Get relevant link buttons
        LinkButton ELB = (LinkButton)dataRow.FindControl("EditLB");
        LinkButton ULB = (LinkButton)dataRow.FindControl("UpdateLB");
        LinkButton CLB = (LinkButton)dataRow.FindControl("CancelLB");
        TextBox ComBox = (TextBox)dataRow.FindControl("CommentBox");
        //If edit is true
        if (edit == true)
        {
            //Set the relevant link buttons
            ELB.Visible = false;
            ULB.Visible = true;
            CLB.Visible = true;
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
            InsertLB.Visible = true;
            ComBox.ReadOnly = true;
        }
    }

    //This method sets masterpage variables
    private void loadMasterInfo()
    {
        //Set the oage Title
        Page.Title = "Forwarding Events";
        //Set the header text
        ((Label)Master.FindControl("mHeaderLabel")).Text = "Forwarding Events";
        //Get the upper center label from the master page
        Label upperLabel = (Label)Master.FindControl("mHeaderLabelCenter");
        //Set title and css class
        upperLabel.Text = "The following events will be forwarded to the next Event List";
        upperLabel.CssClass = "ForwardText";
        //Get the lower left control panel from the master page
        Panel lowerLeft = (Panel)Master.FindControl("masterLowerControlPL");
         //Get the lower right control panel from the master page
        Panel lowerRight = (Panel)Master.FindControl("masterLowerControlPR");
        //get the buttons to add
        WebControl[] toAdd = createControls();
        //foreach buttton
        foreach (WebControl adding in toAdd)
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
    private void populateGrid()
    {
        //New datatable object
        DataTable dt = Database.select_EventListForward();
        //Set the data source
        ManageGrid.DataSource = dt;
        //If complete is true
        if (dt.Rows.Count == 0)
        {
            //Show the validation button
            ImageButton accept = (ImageButton)Master.FindControl("EnterButton");
            //Show the Ok button
            accept.Visible = true;
            //Refresh the update panel
            ((UpdatePanel)Master.FindControl("masterTableRightLPan")).Update();
        }
        
        //Update the event counts
        updateEventCount(dt);
        //Bind the datatable to the grid
        ManageGrid.DataBind();
        //Call the UpdatePanels update method
        GridViewPanel.Update();
    }

    //This method counts the rows in a datatable based on thier StatusCode
    //and updates an info textbox with the results
    protected void updateEventCount(DataTable dt)
    {
        //Get the number of forwarded rows
        int fcount = (Int32)(dt.Compute("COUNT(Forward)", "ELID > 0"));
        //Set the appropriate components text
        ((Label)(Master.FindControl("AllT"))).Text = (fcount.ToString() + " Events") ;
    }


//**Region - Event Related Methods**
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
            //Get the doc hidden field
            HiddenField Edit = (HiddenField)dataRow.FindControl("Editable");
            //Get relevant link buttons
            LinkButton ELB = (LinkButton)dataRow.FindControl("EditLB");
            //date time of comment
            DateTime comment = System.DateTime.ParseExact((Values[3] + " " + Values[4]), "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
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
            if ((comment < System.DateTime.Now.AddHours(-12)) || !(commentor.Equals(Page.User.Identity.Name.ToString())) || editable == false) { ELB.Visible = false; }
        }
    }

    //Occurs when edit mode is canceled on a row
    protected void ComView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        //Get the datarow in question
        GridViewRow dataRow = ComGrid.Rows[e.RowIndex];
        //Set the edit index to -1 signifying no edit more
        ComGrid.EditIndex = -1;
        //Set the edit mode variabes
        editMode(false, e.RowIndex);
        //If no comment was added - Delete the row
        if (((TextBox)dataRow.FindControl("CommentBox")).Text.Length <= 0) 
        {
            //Call the update method
            int rowDel = Database.delete_EventComment(int.Parse(dataRow.Cells[0].Text));
        }
        //Bind data
        bindData(int.Parse(PriorCountH.Value));
        //Update the panel
        ComData.Update();
    }

    //Occurs when a row enters editing mode
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

    //Occurs when a row is updating
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

    //Occurs everytime a check box is checked or unchecked
    protected void Confirmed_CheckedChanged(object sender, EventArgs e)
    {
        //Boolean representing complete
        bool complete = true;
        //Integer representing row position
        int rowPos = 0;
        //While boolean is true
        while ((complete == true) && rowPos < ManageGrid.Rows.Count)
        {
            //get the row in question
            GridViewRow gvRow = ManageGrid.Rows[rowPos];
            //Search for the active box
            CheckBox box = (CheckBox)gvRow.FindControl("Confirmed");
            //If the checkbox is not checked
            if (box.Checked != true)
            {
                //set complete to false
                complete = false;
            }
            //Increment the row count
            rowPos++;
        }
        //Show the validation button
        ImageButton accept = (ImageButton)Master.FindControl("EnterButton");
        //If complete is true
        if (complete == true)
        {
            //Show the Ok button
            accept.Visible = true;
        }
        else
        {
            //Hide the Ok button
            accept.Visible = false;
        }
        //Refresh the update panel
        ((UpdatePanel)Master.FindControl("masterTableRightLPan")).Update();

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
            //Get hidden fields
            HiddenField ELID = (HiddenField)dataRow.FindControl("ELIDH");
            //Event id hidden field
            HiddenField EID = (HiddenField)dataRow.FindControl("EIDH");
            //Get the hidden scheduled time field
            HiddenField SchedTime = (HiddenField)dataRow.FindControl("SchedTimeH");

            //Get the object array behind it
            Object[] Values = rowView.Row.ItemArray;
            //Get the values from the row
            String ElidS = Values[0].ToString();
            String EidS = Values[18].ToString();
            String CatCol = Values[19].ToString();
            //Set fields
            ELID.Value = ElidS;
            EID.Value = EidS;
            SchedTime.Value = Values[1].ToString();
            //Set the boundfield cell to red
            dataRow.Cells[0].CssClass = "Forward";
            //Set the color
            dataRow.Cells[1].ForeColor = System.Drawing.ColorTranslator.FromHtml("#" + CatCol);
        }
    }

    //This method generates an event list. THe method uses the current time
    //and a value from the Database to determine whether or not to create a new 
    //event list, and which one day/night to create.
    protected void create_EventList(Object obj, EventArgs ags)
    {
        //Current Date
        DateTime Date = System.DateTime.Now.Date;
        //Current datetime
        TimeSpan Current = DateTime.Now.TimeOfDay;
        //Day shift time
        TimeSpan dayShift = new TimeSpan(08, 00, 00);
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
            //Call the create procedure for a day shift
            Database.create_EventList(new DateTime(Date.Year, Date.Month, Date.Day, dayShift.Hours, dayShift.Minutes, 00), (new DateTime(Date.Year, Date.Month, Date.Day, nightShift.Hours, nightShift.Minutes, 00)).AddMilliseconds(-100), true, false);
            //Go to the event list
            Response.Redirect("~/Views/EventList.aspx");
        }
        //If the current time is after nightshift or before day shift and the todays date is greater than or equal to the last generated shift list and the type of shift is not already in play OR Today's datetime is a full day past the last generated shift event list
        else if (((Current > nightShift || Current < dayShift)) &&
            (((Date >= nightrow.Field<DateTime>("LastCreated").Date) && (nightrow.Field<bool>("InPlay") == false)) || (System.DateTime.Now > nightrow.Field<DateTime>("LastCreated").AddDays(1))))
        {
            //Date times to pass to the create event list
            DateTime nightShiftOut;
            DateTime dayShiftOut;
            //If the event list is being generated before midnight
            if (Current > nightShift && Current > dayShift)
            {
                //create date times
                nightShiftOut = new DateTime(Date.Year, Date.Month, Date.Day, nightShift.Hours, nightShift.Minutes, 00);
                //create date times
                dayShiftOut = new DateTime(Date.AddDays(1).Year, Date.AddDays(1).Month, Date.AddDays(1).Day, dayShift.Hours, dayShift.Minutes, 00);
            }
            //Else the event list is being generated after midnight but before dayshift
            else
            {
                //create date times
                nightShiftOut = new DateTime(Date.AddDays(-1).Year, Date.AddDays(-1).Month, Date.AddDays(-1).Day, nightShift.Hours, nightShift.Minutes, 00);
                //create date times
                dayShiftOut = new DateTime(Date.Year, Date.Month, Date.Day, dayShift.Hours, dayShift.Minutes, 00);
            }
            //Call the create procedure for a night shift
            Database.create_EventList(nightShiftOut, dayShiftOut.AddMilliseconds(-100), false, true);
            //Go to the event list
            Response.Redirect("~/Views/EventList.aspx");
        }
    }

    //This method is used to populate comment data when the index of the radio list changes
    protected void DateListCom_SelectedIndexChanged(object sender, EventArgs e)
    {
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
        //Refresh the update panel
        ComData.Update();
        //Set the event title
        ComEventTitle.Text = ComEventTitleH.Value;
        //Set the selected index to -1
        DateListCom.SelectedIndex = -1;
    }
    
    //Occurs when the insert button is clickeds
    protected void InsertLB_Click(object sender, EventArgs e)
    {
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
        //Call the method that sets up the row
        controlRowDisplay(sender, e);
    }

}