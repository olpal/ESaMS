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

public partial class Views_Configuration : System.Web.UI.Page
{
//**Region - Page Variables**
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

    //Occurs when the page is loading
    protected void Page_Load(object sender, EventArgs e)
    {
        //If the load is not a postback
        if (!Page.IsPostBack)
        {
            //Populate the category box
            populateBoxes("All","All");
            //Populate the Main table
            populateGrid("*", "*");
            //Register triggers
            ((ScriptManager)Master.FindControl("MasterScriptManager")).RegisterAsyncPostBackControl(CatList);
            ((ScriptManager)Master.FindControl("MasterScriptManager")).RegisterAsyncPostBackControl(SchedList);
            //Populate category gird
            bindData();
        }
        
    }


//**Region - Data Processing Methods**
    private void bindData()
    {
        //Get the datatable
        DataTable dt = Database.select_EventCategoryTable();
        //Create the data table
        CatGrid.DataSource = dt;
        //Bind the data
        CatGrid.DataBind();
    }

    //This method creates the controls for the masterpage
    private ImageButton[] createControls()
    {
        //BUtton array to return
        ImageButton[] returnArray = new ImageButton[1];
        //create buttons to add
        ImageButton newIB = new ImageButton();
        //set the button Css class
        newIB.CssClass = "LowerControlButtons";
        //Set the buttons ID
        newIB.ID = "CatImageB";
        //Set the buttons tooltip
        newIB.ToolTip = "Manage Event System Categories";
        //Set the buttons imageurl
        newIB.ImageUrl = "~/Images/CatManage.gif";
        //set image buttons event handler
        newIB.OnClientClick = "ShowHideCat_Click(this)";
        //Add buttons to the array
        returnArray[0] = newIB;
        //return the array
        return returnArray;
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
        //addcomponents in the correct order
        returnArray[0] = AllStat;
        returnArray[1] = AllT;
        //return the array
        return returnArray;
    }

    //This method creates the modal popup extenders for the master page
    private AjaxControlToolkit.ModalPopupExtender[] createExtenders()
    {
        //BUtton array to return
        AjaxControlToolkit.ModalPopupExtender[] returnArray = new AjaxControlToolkit.ModalPopupExtender[1];
        //create modalpopupextenders
        AjaxControlToolkit.ModalPopupExtender EveTPop = new AjaxControlToolkit.ModalPopupExtender();
        //set the button Css class
        EveTPop.BackgroundCssClass = "ModelBack";
        //Set the extender ID
        EveTPop.ID = "CatPop";
        //Set the extenders target control id
        EveTPop.TargetControlID = "CatImageB";
        //Set the extenders popup control id
        EveTPop.PopupControlID = "masterMain_CatPan";
        //Add extender to the array
        returnArray[0] = EveTPop;
        //return the array
        return returnArray;
    }

    //Used to control edit mode on the comment grid
    private void editMode(Boolean edit, int index)
    {
        //Get the datarow in question
        GridViewRow dataRow = CatGrid.Rows[index];
        //Get relevant link buttons
        LinkButton ELB = (LinkButton)dataRow.FindControl("EditLB");
        LinkButton DLB = (LinkButton)dataRow.FindControl("DeleteLB");
        LinkButton ULB = (LinkButton)dataRow.FindControl("UpdateLB");
        LinkButton CLB = (LinkButton)dataRow.FindControl("CancelLB");
        TextBox nBox = (TextBox)dataRow.FindControl("NameBox");
        TextBox cBox = (TextBox)dataRow.FindControl("ColorBox");
        AjaxControlToolkit.ColorPickerExtender ext = (AjaxControlToolkit.ColorPickerExtender)dataRow.FindControl("ColorExt");
        //If edit is true
        if (edit == true)
        {
            //Set the relevant link buttons
            ELB.Visible = false;
            DLB.Visible = false;
            ULB.Visible = true;
            CLB.Visible = true;
            InsertLB.Visible = false;
            nBox.ReadOnly = false;
            nBox.BackColor = System.Drawing.ColorTranslator.FromHtml("#FEE2E2");
            nBox.BorderStyle = BorderStyle.Solid;
            nBox.BorderColor = System.Drawing.ColorTranslator.FromHtml("#7C0604");
            nBox.BorderWidth = Unit.Parse("1");
            cBox.ReadOnly = false;
            ext.Enabled = true;
        }
        else
        {
            //Set the relevant link buttons
            ELB.Visible = true;
            DLB.Visible = true;
            ULB.Visible = false;
            CLB.Visible = false;
            InsertLB.Visible = true;
            nBox.ReadOnly = true;
            cBox.ReadOnly = true;
            ext.Enabled = false;
        }
    }

    //This method sets masterpage variables
    private void loadMasterInfo()
    {
        //Set the oage Title
        Page.Title = "Event Management";
        //Set the header text
        ((Label)Master.FindControl("mHeaderLabel")).Text = "Event Management";
        //Get the lower right control panel from the master page
        Panel lowerRight = (Panel)Master.FindControl("masterLowerControlPR");
        //Get the lower left control panel from the master page
        Panel lowerLeft = (Panel)Master.FindControl("masterLowerControlPL");
        //get the buttons to add
        WebControl[] toAddStat = createStatus();
        //foreach buttton
        foreach (WebControl adding in toAddStat)
        {
            //Add the button to the panel
            lowerLeft.Controls.Add(adding);
        }
        //get the buttons to add
        WebControl[] toAddCont = createControls();
        //foreach buttton
        foreach (WebControl adding in toAddCont)
        {
            //Add the button to the panel
            lowerRight.Controls.Add(adding);
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

    //Populates the list boxes present
    private void populateBoxes(String selectedC, String selectedS)
    {
        //Get the categories from the Database
        List<Category> Cats = Database.select_EventCategory();
        List<Schedule> Sched = Database.select_EventSchedule();
        
        //Add the all value to the array first
        CatList.Items.Add("All");
        SchedList.Items.Add("All");

        //Add the names of the categories to the list box
        foreach(Category c in Cats){
            //Add the value to the array
            CatList.Items.Add(c.Name);
        }
        //Add the names of the categories to the list box
        foreach (Schedule s in Sched)
        {
            //Add the value to the array
            SchedList.Items.Add(s.Name);
        }
        //Set the selected item
        CatList.SelectedValue = selectedC;
        SchedList.SelectedValue = selectedS;
    }

    //Populates the main table with data from the SQL Database
    private void populateGrid(String Category, String Schedule)
    {
        //New datatable object
        DataTable dt = Database.select_Management(Category, Schedule);
        //Update the all events label
        ((Label)(Master.FindControl("AllT"))).Text = dt.Rows.Count.ToString() + " Events";
        //Set the data source
        ManageGrid.DataSource = dt;
        //Bind the datatable to the grid
        ManageGrid.DataBind();
        //Update the gridview panel
        GridViewPanel.Update();
    }

    //This method updates the status bar with the provided message
    private void updateStatus(String message)
    {
        //If the message is greater than a certain length trim it down
        if (message.Length > 180) { message = (message.Substring(0, 179)+"..."); }
        //GEt the status label from the master panel
        Label StatusBox = (Label)Master.FindControl("mHeaderLabelCenter");
        //Set the status text
        StatusBox.Text = message;
        //Update the update panel
        ((UpdatePanel)Master.FindControl("masterUpperUpdate")).Update();
    }

//**Region - Event Related Methods**
    //This occurs when the add event  button is clicked
    protected void add_Event(Object obj, EventArgs ags)
    {
        //Transfer to the event page
        Response.Redirect("~/Views/Event.aspx");
    }

    //Occurs when edit mode is canceled on a row in the comment window
    protected void CatView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        //Set the edit index to -1 signifying no edit more
        CatGrid.EditIndex = -1;
        //Set the edit mode variabes
        editMode(false, e.RowIndex);
        //Bind data
        bindData();
        //Update the panel
        CatData.Update();
    }

    //Occurs when a row enters editing mode in the comment window
    protected void CatView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        //Set the new edit index
        CatGrid.EditIndex = e.NewEditIndex;
        //bind the data
        bindData();
        //Set the edit mode variabes
        editMode(true, e.NewEditIndex);
        //Update the panel
        CatData.Update();
    }

    //Occurs when a row is updating in the comment window
    protected void CatView_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        //Get the datarow in question
        GridViewRow dataRow = CatGrid.Rows[e.RowIndex];
        //Call the update method
        Database.update_Category(int.Parse(dataRow.Cells[0].Text), ((TextBox)dataRow.FindControl("NameBox")).Text.ToString().Trim(), 
                                                                   ((TextBox)dataRow.FindControl("ColorBox")).Text.ToString().Trim());
        //set edit mode
        editMode(false, e.RowIndex);
        //Set the edit index to -1 signifying no edit more
        CatGrid.EditIndex = -1;
        //Bind Data
        bindData();
        //Refresh the panel
        CatData.Update();
    }

    //Occurs when a row is delete from the categor grid
    protected void CatGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        //Get the datarow in question
        GridViewRow dataRow = CatGrid.Rows[e.RowIndex];
        //Delete the row from the database
        Database.delete_EventCategory(int.Parse(dataRow.Cells[0].Text));
        //bind the data
        bindData();
        //Update the panel
        CatData.Update();
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
            //Get the object array behind it
            Object[] Values = rowView.Row.ItemArray;
            //Get variables from datarow
            String CatCol = Values[7].ToString();
            String SchedCode = Values[6].ToString();
            String Sched = Values[4].ToString();
            bool active = bool.Parse(Values[3].ToString());
            //Break the date into its variables
            String[] dateVars = Values[8].ToString().Split("/".ToCharArray()[0]);
            //Construct date time from the original startdate
            DateTime StartDate = new DateTime(int.Parse(dateVars[2]), int.Parse(dateVars[0]), int.Parse(dateVars[1]));
            //Schedule code label
            Label SchedCodeL = (Label)dataRow.FindControl("SchedCodeL");
            //If a color exists for the category
            if (CatCol.Length > 0)
            {
                //Set the color
                dataRow.Cells[1].ForeColor = System.Drawing.ColorTranslator.FromHtml("#" + CatCol);
            }
            switch (Sched)
           {
                case "Daily":
                    //Replace the numbers with days of the weeke
                    SchedCode = SchedCode.Replace("1", "S,");
                    SchedCode = SchedCode.Replace("2", "M,");
                    SchedCode = SchedCode.Replace("3", "T,");
                    SchedCode = SchedCode.Replace("4", "W,");
                    SchedCode = SchedCode.Replace("5", "R,");
                    SchedCode = SchedCode.Replace("6", "F,");
                    SchedCode = SchedCode.Replace("7", "S,");
                    //If the code ends with a comma remove it
                    if (SchedCode.EndsWith(",")) { SchedCode = SchedCode.Substring(0, SchedCode.Length - 1); }
                    break;
                case "Weekly":
                    //Get the day of the week
                    SchedCode = StartDate.DayOfWeek.ToString();
                    break;
                case "Bi-Weekly":
                    //Get the day of the week
                    SchedCode = StartDate.DayOfWeek.ToString();
                    break;
                case "Quarterly":
                    //Get the day of the week
                    SchedCode = StartDate.ToString("MMM") + "-" + StartDate.Day.ToString();
                    break;
                case "One-Time":
                    //Get the date of the one time event
                    SchedCode = Values[8].ToString();
                    break;
                case "Frequent Reoccurance":
                    //Get the occurance
                    String occur = SchedCode.Substring(0, 1);
                    //Get the day
                    String occurDay = SchedCode.Substring(1, 1);
                    //Replace the numbers with days of the weeke
                    occurDay = occurDay.Replace("1", "Sun,");
                    occurDay = occurDay.Replace("2", "Mon,");
                    occurDay = occurDay.Replace("3", "Tue,");
                    occurDay = occurDay.Replace("4", "Wed,");
                    occurDay = occurDay.Replace("5", "Thu,");
                    occurDay = occurDay.Replace("6", "Fri,");
                    occurDay = occurDay.Replace("7", "Sat,");
                    //Set the schedule code
                    SchedCode = occur + "-" + occurDay;
                    break;
            }
            //Assign the code to the label
            SchedCodeL.Text = SchedCode;
            //IF the event is not active change the image of the enable/disable button
            if (!active)
            {
                //Get the disable button
                ImageButton StatB = (ImageButton)dataRow.FindControl("Status");
                //Set the tooltip of the button
                StatB.ImageUrl = "~/Images/Enable.gif";
                StatB.ToolTip = "Enable";
            }
        }

    }

    //This occurs when toggle active status image button is clicked
    protected void iButtonActive_Click(Object obj, EventArgs ags)
    {
        //Cast thesender object to a image button
        ImageButton btn = (ImageButton)obj;
        //Get the sending row
        GridViewRow row = (GridViewRow)btn.NamingContainer;
        //Get the EID from the row
        int eid = Convert.ToInt32(row.Cells[0].Text);
        //Get the Active status
        CheckBox checkStat = (CheckBox)row.Cells[2].FindControl("ActiveCheck");
        //Call the Database method
        Database.update_ActiveStatus(eid, checkStat.Checked);
        //Reload the grid
        populateGrid(CatList.SelectedValue, SchedList.SelectedValue);
    }

    //This occurs when the delete image button is clicked
    protected void iButtonDelete_Click(Object obj, EventArgs ags)
    {
        //Cast thesender object to a image button
        ImageButton btn = (ImageButton)obj;
        //Get the sending row
        GridViewRow row = (GridViewRow)btn.NamingContainer;
        //Get the EID from the row
        int eid = Convert.ToInt32(row.Cells[0].Text);
        //Get the name of the event being deleted
        string eventName = row.Cells[2].Text;
        //delete the event
        bool success = Database.delete_Event(eid,(Page.User.Identity.Name.ToString()));
        //IF the deletion was successful
        if (success == true)
        {
            //Set the upper status label
            updateStatus(System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + " Event deleted Event ID: " + eid + " Event Name: " + eventName);
        }
        else
        {
            //Set the upper status label
            updateStatus(System.DateTime.Now.ToString("MM/dd/yyyy HH:mm") + " Event deletion Failed, event may exist in Shift Event List. Event ID: " + eid + " Event Name: " + eventName);
        }
        //Reload the grid
        populateGrid(CatList.SelectedValue, SchedList.SelectedValue); 
        
    }

    //Occurs when the insert button is clickeds in the comment window
    protected void InsertLB_Click(object sender, EventArgs e)
    {
        //Call the add comment method
        Database.add_EventCategory("", "");
        //edit index
        int edit = CatGrid.Rows.Count;
        //Bind Data
        bindData();
        //set edit mode
        editMode(true, edit);
        //Update the panel
        CatData.Update();
    }

    //Occurs when the CategoryList box changes
    protected void List_Changed(Object obj, EventArgs evs)
    {
        //Populate the table with the change in category
        populateGrid(CatList.SelectedValue, SchedList.SelectedValue);
    }

    //Occurs when a row is bound to the datagrid
    protected void ManageGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //Call the control row display
        controlRowDisplay(sender, e);
    }

    //Occurs when the home button is clicked
    protected void ReturnButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/views/Index.aspx");
    }

}