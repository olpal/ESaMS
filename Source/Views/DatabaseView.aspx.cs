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

public partial class Views_Database : System.Web.UI.Page
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

    //This method occurs pre page load
    protected void Page_init(object sender, EventArgs e)
    {
        //Load master file variables
        loadMasterInfo();
        //Deal with paremeters
        //parameters();
    }

    //This method occurs when the page laods
    protected void Page_Load(object sender, EventArgs e)
    {
        //If the load is not a postback
        if (!Page.IsPostBack)
        {
            //Populate the category box
            populateBoxes("");
        }
        //create condition panels
        AddPanel(5);
        //create order panels
        AddOrder();
    }


//**Region - Data Processing Methods**
    //This method is used to construct the condition table for the collapsable panel
    protected void AddPanel(int Conditions)
    {
        //new asp table
        Table table = new Table();
        //New hedder row
        TableHeaderRow hr = new TableHeaderRow();
        //New Header Cells
        TableHeaderCell hc1 = new TableHeaderCell();
        TableHeaderCell hc2 = new TableHeaderCell();
        TableHeaderCell hc3 = new TableHeaderCell();
        TableHeaderCell hc4 = new TableHeaderCell();
        TableHeaderCell hc5 = new TableHeaderCell();
        //Set header cells text
        hc1.Text = "Column";
        hc2.Text = "Condition";
        hc3.Text = "Info 1";
        hc4.Text = "Info 2";
        hc5.Text = "Operator";
        //Add the header cells to the header row
        hr.Cells.Add(hc1);
        hr.Cells.Add(hc2);
        hr.Cells.Add(hc3);
        hr.Cells.Add(hc4);
        hr.Cells.Add(hc5);
        //Set the css classes
        hr.CssClass = "CondHeader";
        table.CssClass = "CondTable";
        //set grid lines
        table.GridLines = GridLines.Horizontal;
        //Add the header row to the table
        table.Rows.Add(hr);
        //Integer to control while loop pos
        int pos = 0;
        //While pos is less than conditions
        while (pos < Conditions)
        {
            //Create a new table row
            TableRow tr = new TableRow();
            //Create 5 new cells
            TableCell tc1 = new TableCell();
            TableCell tc2 = new TableCell();
            TableCell tc3 = new TableCell();
            TableCell tc4 = new TableCell();
            TableCell tc5 = new TableCell();
            //Create new drop down lists
            DropDownList columns = new DropDownList();
            DropDownList condition = new DropDownList();
            DropDownList oper = new DropDownList();
            //Create the text boxes
            TextBox info1 = new TextBox();
            TextBox info2 = new TextBox();
            //Set IDs
            oper.ID = "condOper" + pos;
            columns.ID = "condCols" + pos;
            condition.ID = "condCond" + pos;
            info1.ID = "condInf1" + pos;
            info2.ID = "condInf2" + pos;
            //Set css classes
            oper.CssClass = "CondPanCom";
            condition.CssClass = "CondPanCom";
            columns.CssClass = "CondPanCom";
            info1.CssClass = "CondBox";
            info2.CssClass = "CondBox";
            //Set info2's visabilities
            info2.Visible = false;
            //Set postback
            condition.AutoPostBack = true;
            //Set event handler
            condition.SelectedIndexChanged += colBox_SelectedIndexChanged;
            //Set the datasources
            condition.DataSource = new string[] { "", "Equals", "Less than", "Greater than", "Between" };
            oper.DataSource = new string[] { "", "And", "Or" };
            //Bind the data sources
            condition.DataBind();
            oper.DataBind();
            columns.DataBind();
            //Add the components to the panel
            tc1.Controls.Add(columns);
            tc2.Controls.Add(condition);
            tc3.Controls.Add(info1);
            tc4.Controls.Add(info2);
            //Set the css class of the cell if needed
            tc1.CssClass = "CondCell";
            tc4.CssClass = "CondCell";
            //If pos is less than the last number
            if (pos < (Conditions - 1))
            {
                //Add the condition dropdownlist
                tc5.Controls.Add(oper);
            }
            //Add the cells to the table row
            tr.Cells.Add(tc1);
            tr.Cells.Add(tc2);
            tr.Cells.Add(tc3);
            tr.Cells.Add(tc4);
            tr.Cells.Add(tc5);
            //Add the row to the table
            table.Rows.Add(tr);
            //Increment pos by 1
            pos++;
        }
        //Add the table to the panel
        UpperConditions.Controls.Add(table);
        //Create button
        Button update = new Button();
        //Set text
        update.Text = "Update";
        //Set the click method
        update.Click += UpdateGrid_Click;
        //Add to the lower panel
        LowerConditions.Controls.Add(update);
        //Create button
        Button clear = new Button();
        //Set text
        clear.Text = "Reset";
        //Set the click method
        clear.Click += clear_Click;
        //Add to the lower panel
        LowerConditions.Controls.Add(clear);
    }

    //This method is used to construct the condition order for the collapsable panel
    protected void AddOrder()
    {
        //new asp table
        Table table = new Table();
        //New hedder row
        TableHeaderRow hr = new TableHeaderRow();
        //New Header Cells
        TableHeaderCell hc1 = new TableHeaderCell();
        TableHeaderCell hc2 = new TableHeaderCell();
        //Set header cells text
        hc1.Text = "Column";
        hc2.Text = "Order";
        //Add the header cells to the header row
        hr.Cells.Add(hc1);
        hr.Cells.Add(hc2);
        //Set the css classes
        hr.CssClass = "CondHeader";
        table.CssClass = "CondTable";
        //set grid lines
        table.GridLines = GridLines.Horizontal;
        //Add the header row to the table
        table.Rows.Add(hr);
        //Create a new table row
        TableRow tr = new TableRow();
        //Create 5 new cells
        TableCell tc1 = new TableCell();
        TableCell tc2 = new TableCell();
        //Create a new panel
        Panel condPan = new Panel();
        //Create new drop down lists
        DropDownList columns = new DropDownList();
        DropDownList order = new DropDownList();
        //Set IDs
        columns.ID = "ordCols";
        order.ID = "ordOrde";
        //Set css classes
        order.CssClass = "CondPanCom";
        columns.CssClass = "CondPanCom";
        //Set the datasources
        order.DataSource = new string[] { "", "ASC", "DESC" };
        //Bind the data sources
        order.DataBind();
        //Add the components to the panel
        tc1.Controls.Add(columns);
        tc2.Controls.Add(order);
        //Set the css class of the cell if needed
        tc1.CssClass = "CondCell";
        //Add the cells to the table row
        tr.Cells.Add(tc1);
        tr.Cells.Add(tc2);
        //Add the row to the table
        table.Rows.Add(tr);
        //Add the table to the panel
        OrderPan.Controls.Add(table);
        //Create button
        Button update = new Button();
        //Set text
        update.Text = "Update";
        //Set the click method
        update.Click += UpdateOrder_Click;
        //Add to the lower panel
        OrderLower.Controls.Add(update);
        //Create button
        Button clear = new Button();
        //Set text
        clear.Text = "Reset";
        //Set the click method
        clear.Click += clearOrder_Click;
        //Add to the lower panel
        OrderLower.Controls.Add(clear);
    }

    //Reset all components in the condition panel
    private void clearConditions()
    {
        int pos = 0;
        //foreach column
        while (pos < 5)
        {
            //Set the combo boxes index to 0
            ((DropDownList)UpperConditions.FindControl("condCols" + pos)).SelectedIndex = 0;
            ((DropDownList)UpperConditions.FindControl("condCond" + pos)).SelectedIndex = 0;
            DropDownList oper = ((DropDownList)UpperConditions.FindControl("condOper" + pos));
            //If the drop down exists
            if (oper != null)
            {
                //Set the index to 0
                oper.SelectedIndex = 0;
            }
            //Set the text boxes blank
            ((TextBox)UpperConditions.FindControl("condInf1" + pos)).Text = "";
            ((TextBox)UpperConditions.FindControl("condInf2" + pos)).Text = "";
            //Increment pos
            pos++;
        }
    }

    //Reset all components in the order panel
    private void clearOrder()
    {
        //Set the dropdowns index to 0
        ((DropDownList)UpperConditions.FindControl("ordCols")).SelectedIndex = 0;
        ((DropDownList)UpperConditions.FindControl("OrdOrde")).SelectedIndex = 0;
    }

    //This method is used to contruct an SQL statement based on inputed data
    private string createSQL()
    {
        //String for final sql
        string finalSQL = "";
        //List of check box names
        string[] condBox = new string[] { "condCols0", "condCols1", "condCols2", "condCols3", "condCols4" };
        //integer to control while loop
        int pos = 0;
        //while pos less than condBox array
        while (pos < condBox.Length)
        {
            //Get the box name
            string box = condBox[pos];
            //Get the column dropdownlist 
            DropDownList col = (DropDownList)UpperConditions.FindControl(box);
            //If the box is not null and has an index greater than 0
            if (col != null && col.SelectedIndex > 0)
            {
                //Column name
                string colName = col.SelectedValue;
                //Get the operation control
                DropDownList cond = (DropDownList)UpperConditions.FindControl(box.Replace("condCols", "condCond"));
                //contruct based on operations
                switch (cond.SelectedValue)
                {
                    case "Equals":
                        //Add to the string
                        finalSQL += colName + "='" + ((TextBox)UpperConditions.FindControl(box.Replace("condCols", "condInf1"))).Text.Trim() + "'";
                        break;
                    case "Less than":
                        //Add to the string
                        finalSQL += colName + "<'" + ((TextBox)UpperConditions.FindControl(box.Replace("condCols", "condInf1"))).Text.Trim() + "'";
                        break;
                    case "Greater than":
                        //Add to the string
                        finalSQL += colName + ">'" + ((TextBox)UpperConditions.FindControl(box.Replace("condCols", "condInf1"))).Text.Trim() + "'";
                        break;
                    case "Between":
                        //Add to the string
                        finalSQL += colName + " BETWEEN '" + ((TextBox)UpperConditions.FindControl(box.Replace("condCols", "condInf1"))).Text.Trim() +
                            "' AND '" + ((TextBox)UpperConditions.FindControl(box.Replace("condCols", "condInf2"))).Text.Trim() + "'";
                        break;
                    case "":
                        break;
                }
            }
            //Get the column dropdownlist 
            DropDownList oper = (DropDownList)UpperConditions.FindControl(box.Replace("condCols", "condOper"));
            //If the box is not null and has an index greater than 0
            if (oper != null)
            {
                //Get the next column dropdownlist 
                DropDownList nextCol = (DropDownList)UpperConditions.FindControl(condBox[pos + 1]);
                //If the next column has a selected value
                if (nextCol != null && nextCol.SelectedIndex > 0)
                {
                    //If a value has been selected from the oper box
                    if (oper.SelectedIndex > 0)
                    {
                        //add the operation
                        finalSQL += (" " + oper.SelectedValue + " ");
                    }
                    else
                    {
                        //Set the index to 1
                        oper.SelectedIndex = 1;
                        //add the operation
                        finalSQL += (" " + oper.Items[1] + " ");
                    }
                }
            }
            //increment pos
            pos++;
        }
        //If there was text added
        if (finalSQL.Length > 0)
        {
            //add where clause to begining
            finalSQL = " WHERE " + finalSQL;
        }
        //Get the order column dropdownlist 
        DropDownList ordCols = (DropDownList)UpperConditions.FindControl("ordCols");
        DropDownList ordOrde = (DropDownList)UpperConditions.FindControl("ordOrde");
        //If both are not null and have a selected index
        if ((ordCols != null && ordOrde != null) && (ordCols.SelectedIndex > 0 && ordOrde.SelectedIndex > 0))
        {
            //Add the order by statement to the SQL
            finalSQL += " ORDER BY " + ordCols.SelectedValue + " " + ordOrde.SelectedValue;
        }
        //Return the string
        return finalSQL;
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

    //This method sets masterpage variables
    private void loadMasterInfo()
    {
        //Set the oage Title
        Page.Title = "Database Viewer";
        //Set the header text
        ((Label)Master.FindControl("mHeaderLabel")).Text = "Database Viewer";
        //Get the lower left control panel from the master page
        Panel lowerLeft = (Panel)Master.FindControl("masterLowerControlPL");
        //get the status labels to add
        WebControl[] toAddStat = createStatus();
        //foreach buttton
        foreach (WebControl adding in toAddStat)
        {
            //Add the button to the panel
            lowerLeft.Controls.Add(adding);
        }
    }

    //Populates the list boxes present
    private void populateBoxes(String Table)
    {
        //Array to hold table names
        string[] tables = new string[] { "",  "Event_Audit", "Event_Category", "Event_Comments", "Event_List", "Event_Performance", "Event_Schedule", "Event_ScheduleCodes",
            "Event_Timing", "Events", "EventList_Audit","EventList_Codes","EventList_Status", "Report_List" };
        //Set the datasources
        TabList.DataSource = tables;
        //Bind the data sources
        TabList.DataBind();
        //Set the selected item
        TabList.SelectedValue = Table;
    }

    //Populates the main table with data from the SQL Database
    private void populateGrid(String Table, String Cond, bool ColRef)
    {
        //New datatable object
        DataTable dt = Database.select_Table(Table, Cond);
        //Update the all events label
        ((Label)(Master.FindControl("AllT"))).Text = dt.Rows.Count.ToString() + " Rows";
        //Set the data source
        ManageGrid.DataSource = dt;
        //If col refresh is set to true
        if (ColRef == true)
        {
            //Update the column boxes in the conditions panel
            updateColumns(dt);
        }
        //Bind the datatable to the grid
        ManageGrid.DataBind();
        //Update the gridview panel
        GridViewPanel.Update();
    }

    //This method updates the column boxes
    private void updateColumns(DataTable dt)
    {
        //Array of column box names
        string[] colBoxes = new string[] { "condCols0", "condCols1", "condCols2", "condCols3", "condCols4", "ordCols" };
        //If a table is loaded
        if ((dt != null))
        {
            //New array to hold column names
            string[] colNames = new string[(dt.Columns.Count + 1)];
            //Add a blank value to the colNames array
            colNames[0] = "";
            //integer for while loop
            int pos = 0;
            //foreach column
            while (pos < dt.Columns.Count)
            {
                //add the name to the colnames array
                colNames[pos + 1] = dt.Columns[pos].ColumnName;
                //increment pos
                pos++;
            }
            //for each of the colBoxe names in the array
            foreach (string col in colBoxes)
            {
                //Get the dropdown list
                DropDownList colBox = (DropDownList)UpperConditions.FindControl(col);
                //if the colBox is not null
                if (colBox != null)
                {
                    //Set the datasource
                    colBox.DataSource = colNames;
                    //Bind the datasource
                    colBox.DataBind();
                }
            }

        }
    }


//**Region - Event Related Methods**
    //This method occurs everytime the clear button is clicked
    protected void clear_Click(object sender, EventArgs e)
    {
        //clear condition components
        clearConditions();
        //Populdate the grid
        populateGrid(TabList.SelectedValue, createSQL(), true);
    }

    //This method occurs everytime the clear button is clicked
    protected void clearOrder_Click(object sender, EventArgs e)
    {
        //Clear order panel components
        clearOrder();
        //Populdate the grid
        populateGrid(TabList.SelectedValue, createSQL(), true);
    }

    //This method occurs everytime the index of the condition columns box changes 
    protected void colBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        //cast sender object to dropdown
        DropDownList colBox = (DropDownList)sender;
        //Get the id of the box
        string colId = colBox.ID;
        //Replace with the name of the info2 box
        string infoId = colId.Replace("condCond", "condInf2");
        //Get the textbox control
        TextBox info2 = (TextBox)(UpperConditions.FindControl(infoId));
        //If selected item equals between
        if (colBox.SelectedValue == "Between")
        {
            //If the textbox is not null
            if (info2 != null)
            {
                //Set the status to visible
                info2.Visible = true;
            }
        }
        else
        {
            //If the textbox is not null
            if (info2 != null)
            {
                //Set the status to visible
                info2.Visible = false;
            }
        }
    }

    //Occurs when the Table List box changes
    protected void List_Changed(Object obj, EventArgs evs)
    {
        //Set the managegrids page index to 0
        ManageGrid.PageIndex = 0;
        //Populate the table with the change in category
        populateGrid(TabList.SelectedValue,"",true);
        //Clear the condition panel
        clearConditions();
        //Clear the order panel
        clearOrder();
    }

    //Occurs everytime the page index changes
    protected void ManageGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //Set the new index
        ManageGrid.PageIndex = e.NewPageIndex;
        //Refresh the data
        populateGrid(TabList.SelectedValue, createSQL(),false);
    }

    //This method occurs everytime the update button is clicked
    protected void UpdateGrid_Click(object sender, EventArgs e)
    {
        //if the table is set
        if (TabList.SelectedIndex > 0)
        {
            //Update the grid
            populateGrid(TabList.SelectedValue, createSQL(),false);
        }
    }

    //This method occurs everytime the update button is clicked
    protected void UpdateOrder_Click(object sender, EventArgs e)
    {
        //if the table is set
        if (TabList.SelectedIndex > 0)
        {
            //Update the grid
            populateGrid(TabList.SelectedValue, createSQL(), false);
        }
    }

}