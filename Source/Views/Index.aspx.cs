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
using System.Data.SqlClient;

public partial class Views_Default : System.Web.UI.Page
    {
        //Software version
    String Version = "Version: 1.2";

//**Region - Page Related Methods**
        protected void Page_Load(object sender, EventArgs e)
        {
            //Load master page info
            loadMasterInfo();
            //Set the version
            VersionLabel.Text = Version;
        }

        
//**Region - Data Processing Methods**

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
            newIB.ToolTip = "License";
            //Set the buttons imageurl
            newIB.ImageUrl = "~/Images/CatManage.gif";
            //set image buttons event handler
            newIB.OnClientClick = "ShowHideCat_Click(this)";
            //Add buttons to the array
            returnArray[0] = newIB;
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

    //This method creates the status objects for the masterpage
    private WebControl[] createStatus()
    {
        //BUtton array to return
        WebControl[] returnArray = new WebControl[1];
        //Create status images        
        Label AllU = new Label();
        //set status image ids
        AllU.ID = "UserN";
        //Set the css class
        AllU.CssClass = "StatusBox";
        AllU.Text = "User: " + Page.User.Identity.Name.ToString();
        //addcomponents in the correct order
        returnArray[0] = AllU;
        //return the array
        return returnArray;
    }

    //This method sets masterpage variables
    private void loadMasterInfo()
        {
            //Set page title
            Page.Title = "Event System Home";
            //Set the document title
            ((Label)Master.FindControl("mHeaderLabel")).Text = "Event System Home";
            //get the buttons to add
            WebControl[] toAddCon = createStatus();
            //Get the lower right control panel from the master page
            Panel lowerRight = (Panel)Master.FindControl("masterLowerControlPR");
            //Get the lower left control panel from the master page
            Panel lowerLeft = (Panel)Master.FindControl("masterLowerControlPL");
            //foreach buttton
            foreach (WebControl adding in toAddCon)
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


    }