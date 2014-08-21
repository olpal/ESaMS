using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Views_MasterBall : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Create the navigation buttons
        createButtons();
    }

    //this function creates the necessary buttons for the page
    private void createButtons()
    {      
        //create buttons to add
        HyperLink index = new HyperLink();
        HyperLink manage = new HyperLink();
        HyperLink report = new HyperLink();
        HyperLink add = new HyperLink();
        HyperLink list = new HyperLink();
        HyperLink db = new HyperLink();
        //set the button Css class
        index.CssClass = "UpperControlButtons";
        manage.CssClass = "UpperControlButtons";
        report.CssClass = "UpperControlButtons";
        add.CssClass = "UpperControlButtons";
        list.CssClass = "UpperControlButtons";
        db.CssClass = "UpperControlButtons";
        //Set the buttons ID
        index.ID="IndexImageB";
        manage.ID="ManageImageB";
        report.ID="ReportImageB";
        add.ID="AddImageB";
        add.ID="ListImageB";
        db.ID = "DbImageB";
        //Set the buttons tooltip
        index.ToolTip="Event Home";
        manage.ToolTip="Event Management";
        report.ToolTip="Event Reporting";
        add.ToolTip="Event Creation";
        list.ToolTip="Current Shift View";
        db.ToolTip = "Database Viewer";
        //set image buttons event handler
        index.NavigateUrl = ("~/views/Index.aspx");
        manage.NavigateUrl = ("~/views/Management.aspx");
        report.NavigateUrl = ("~/views/Reporting.aspx");
        add.NavigateUrl = ("~/views/Event.aspx");
        list.NavigateUrl = ("~/views/EventList.aspx");
        db.NavigateUrl = ("~/views/DatabaseView.aspx");
        //Create status images        
        Image indexIm = new Image();
        Image manageIm = new Image();
        Image reportIm = new Image();
        Image addIm = new Image();
        Image listIm = new Image();
        Image dbIm = new Image();
        //set status image ids
        indexIm.ID = "indexIm";
        manageIm.ID = "manageIm";
        reportIm.ID = "reportIm";
        addIm.ID = "addIm";
        listIm.ID = "listIm";
        dbIm.ID = "dbIm";
        //set status image urls
        indexIm.ImageUrl = "~/Images/Home.gif";
        manageIm.ImageUrl = "~/Images/EventManageNav.gif";
        reportIm.ImageUrl = "~/Images/EventReportNav.gif";
        addIm.ImageUrl = "~/Images/EventModNav.gif";
        listIm.ImageUrl = "~/Images/EventListNav.gif";
        dbIm.ImageUrl = "~/Images/DatabaseNav.gif";
        //set the button Css class
        indexIm.CssClass = "UpperControlButtons";
        manageIm.CssClass = "UpperControlButtons";
        reportIm.CssClass = "UpperControlButtons";
        addIm.CssClass = "UpperControlButtons";
        listIm.CssClass = "UpperControlButtons";
        dbIm.CssClass = "UpperControlButtons";
        //remove from the tab index
        indexIm.TabIndex = -1;
        manageIm.TabIndex = -1;
        reportIm.TabIndex = -1;
        addIm.TabIndex = -1;
        listIm.TabIndex = -1;
        dbIm.TabIndex = -1;
        //Add buttons to their hyperlink control
        index.Controls.Add(indexIm);
        list.Controls.Add(listIm);
        manage.Controls.Add(manageIm);
        add.Controls.Add(addIm);
        report.Controls.Add(reportIm);
        db.Controls.Add(dbIm);
        //Add controls to the panel
        masterUpperControlPR.Controls.Add(index);
        masterUpperControlPR.Controls.Add(list);
        masterUpperControlPR.Controls.Add(manage);
        masterUpperControlPR.Controls.Add(add);
        masterUpperControlPR.Controls.Add(report);
        masterUpperControlPR.Controls.Add(db);
    }
}
