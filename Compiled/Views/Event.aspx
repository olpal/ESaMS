<%--
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
--%>
<%@ page language="C#" autoeventwireup="true" maintainscrollpositiononpostback="true" masterpagefile="~/MasterPages/MasterBall.master" inherits="Views_Event, App_Web_j4ivqzes" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>

<asp:content ID="styles" ContentPlaceHolderID="masterCSS" runat="server">
    <link href="../StyleSheets/CalStyle.Css" type="text/css" rel="Stylesheet" />
</asp:content>
<asp:content ID="mainContent" ContentPlaceHolderID="masterMain" runat="server">
<script type="text/javascript">
    //Generates and email string
    function EmailSave_Click() {
        //Get the values of the text boxes
        tobox = document.getElementById('<%=ToBox.ClientID%>').value;
        ccbox = document.getElementById('<%=CCBox.ClientID%>').value;
        subbox = document.getElementById('<%=SubjectBox.ClientID%>').value;
        bodybox = document.getElementById('<%=BodyBox.ClientID%>').value;
        //Clear the fields before post back *If this isnt done <> will cause errors on postback if present
        document.getElementById('<%=ToBox.ClientID%>').value = "";
        document.getElementById('<%=CCBox.ClientID%>').value = "";
        //Variable for email string
        var emailString = "";
        //variable for write back string
        var writeBack = "";
        //if too is not blank start construction
        if (tobox != "") {
            //add to box value to the string
            emailString = "mailto:";
            //Split the address boxes on semi colons
            var tos = tobox.split(";"); 
            //variable to control while loop
            var pos = 0;
            //while pos is less than length of the array
            while (pos < tos.length) {
                //formatted address
                var forAdd = formatEmail(tos[pos]);
                //if pos is greater than 0
                if (pos > 0) { emailString += ";"; writeBack += ";"; }
                //add the scrubed address
                emailString += forAdd;
                //add to write back
                writeBack += forAdd;
                //Increment by 1
                pos++;
            }
            //replace the to box values with proper formatted addresses
            document.getElementById('<%=ToBox.ClientID%>').value = (writeBack);
            //reset pos
            pos = 0;
            //Reset write back string
            writeBack = "";
            //if too is not blank start construction
            if (ccbox != "") {
                //Split the box
                var ccs = ccbox.split(";"); emailString += "?cc=";
                //while pos is less than length of the array
                while (pos < ccs.length) {
                    //formatted address
                    var forAdd = formatEmail(ccs[pos]);
                    //if pos is greater than 0
                    if (pos > 0) { emailString += ";"; writeBack += ";"; }
                    //add the scrubed address
                    emailString += forAdd;
                    //add to write back
                    writeBack += forAdd;
                    //Increment by 1
                    pos++;
                }
                //replace the to box values with proper formatted addresses
                document.getElementById('<%=CCBox.ClientID%>').value += (writeBack);
            }
            //if subject box is not null
            if (subbox != "") {
                //add to box value to the string
                emailString += "?subject=" + subbox + "&";
            }
            //if body box is not null
            if (bodybox != "") {
                //add to box value to the string
                emailString += "?body=" + bodybox + "&";
            }
            //If the email string is greater than 1 remove the final character as it is likely an & or ?
            while ((emailString.lastIndexOf("?") == (emailString.length - 1)) || (emailString.lastIndexOf("&") == (emailString.length - 1))) {
                //remove the last character
                emailString = emailString.substring(0, (emailString.length - 1));
            }
        }
        //Set the email box with the email string
        document.getElementById('<%=URLTextBox.ClientID%>').value = emailString;
        //Handle visablity
        ShowHideEmail();
    }
    //this function formats an email string
    function formatEmail(address)
    {
        //var to return, open bracket position, close bracket position;
        var returnAdd = address; var openBrac = address.indexOf("<"); var closeBrac = address.indexOf(">") ;
        //If the address contains an open bracket
        if (openBrac >= 0 && closeBrac > 0)
        {
            //set the return address to the address inside the <>
            returnAdd = address.substring(openBrac + 1, closeBrac);
        }
        //return the address
        return returnAdd;
    }

    //function resets email form
    function EmailReset_Click() {
        //Get the values of the text boxes
        document.getElementsByName('<%=ToBox.ClientID%>')[0].value = "";
        document.getElementsByName('<%=CCBox.ClientID%>')[0].value = "";
        document.getElementsByName('<%=SubjectBox.ClientID%>')[0].value = "";
        document.getElementsByName('<%=BodyBox.ClientID%>')[0].value = "";
    }

    //function to showhide panel
    function ShowHideEmail() {
        //get the panel
        var pan = document.getElementById('<%=EmailPan.ClientID%>');
        //get the panel
        var backPan = document.getElementById((document.getElementById('<%=EmailPan.ClientID%>').id).replace("EmailPan", "EmailPanBack") + "_backgroundElement");
        //if backpan is not null
        if (pan != null) {
            //if the panel is hidden
            if (pan.style.visibility == 'visible') {
                //Showthe panel
                pan.style.visibility = 'hidden';
            }
            else {
                //Hide the panel
                pan.style.visibility = 'visible';
            }
        }
        //if backpan is not null
        if (backPan != null) {
            //if the backpanel is hidden
            if (backPan.style.visibility == 'visible') {
                //Show the backpanel
                backPan.style.visibility = 'hidden';
            }
            else {
                //Hide the backpanel
                backPan.style.visibility = 'visible';
            }
        }
    }
    //function to set the variables of the GirdPanelControl to performance
    function PerGridSetup_Click(button) {
        //Id of active button
        var activeButton = button.id;
        //Set the hidden methodId field 1=Performance 2=comment 3=audit
        document.getElementById('<%=methodID.ClientID%>').value = 1;
        //Set the title label
        document.getElementById('<%=TitleLabel.ClientID%>').innerHTML = "Event Performance Data";
        //Set the title hiddenfield
        document.getElementById('<%=TitleH.ClientID%>').value = "Event Performance Data";
        //Set the grid panel name hiddenfield
        document.getElementById('<%=GridPanName.ClientID%>').value = activeButton.replace("PerformanceT", "PerTPop");
        //Set the event title label
        document.getElementById('<%=GridEventTitle.ClientID%>').innerHTML = document.getElementById('<%=EventTitle.ClientID%>').value;
        //Set the event title hiddenfield
        document.getElementById('<%=EventTitleH.ClientID%>').value = document.getElementById('<%=EventTitle.ClientID%>').value;
        //ShowHide the control
        ShowHidePer_Click();
    }
    //function to set the variables of the GirdPanelControl to Comment
    function ComGridSetup_Click(button) {
        //Id of active button
        var activeButton = button.id;
        //Set the hidden methodId field 1=Performance 2=comment 3=audit
        document.getElementById('<%=methodID.ClientID%>').value = 2;
        //Set the title label
        document.getElementById('<%=TitleLabel.ClientID%>').innerHTML = "Event Comment Data";
        //Set the title hiddenfield
        document.getElementById('<%=TitleH.ClientID%>').value = "Event Comment Data";
        //Set the grid panel name hiddenfield
        document.getElementById('<%=GridPanName.ClientID%>').value = activeButton.replace("Comment", "ComTPop");
        //Set the event title label
        document.getElementById('<%=GridEventTitle.ClientID%>').innerHTML = document.getElementById('<%=EventTitle.ClientID%>').value;
        //Set the event title hiddenfield
        document.getElementById('<%=EventTitleH.ClientID%>').value = document.getElementById('<%=EventTitle.ClientID%>').value;
        //ShowHide the control
        ShowHidePer_Click();
    }
    //function to set the variables of the GirdPanelControl to Audit
    function AudGridSetup_Click(button) {
        //Id of active button
        var activeButton = button.id;
        //Set the hidden methodId field 1=Performance 2=comment 3=audit
        document.getElementById('<%=methodID.ClientID%>').value = 3;
        //Set the title label
        document.getElementById('<%=TitleLabel.ClientID%>').innerHTML = "Event Audit Data";
        //Set the title hiddenfield
        document.getElementById('<%=TitleH.ClientID%>').value = "Event Audit Data";
        //Set the grid panel name hiddenfield
        document.getElementById('<%=GridPanName.ClientID%>').value = activeButton.replace("Audit", "AuditTPop");
        //Set the event title label
        document.getElementById('<%=GridEventTitle.ClientID%>').innerHTML = document.getElementById('<%=EventTitle.ClientID%>').value;
        //Set the event title hiddenfield
        document.getElementById('<%=EventTitleH.ClientID%>').value = document.getElementById('<%=EventTitle.ClientID%>').value;
        //ShowHide the control
        ShowHidePer_Click();
    }
    //function to show/hide performance window
    function ShowHidePer_Click() {
        //get the panel
        var pan =  document.getElementById('<%=GridPan.ClientID%>');
        //back panel
        var backpan = document.getElementById(document.getElementById('<%=GridPanName.ClientID%>').value + "_backgroundElement");
        //if the panel is hidden
        if (pan.style.visibility == 'hidden' || pan.style.visibility == '') {
            //reset the data grid
            GridSetup();
            //Show the panel
            pan.style.visibility = 'visible';
            backpan.style.visibility = 'visible';
        }
        else {
            //Hide the panel
            pan.style.visibility = 'hidden';
            backpan.style.visibility = 'hidden';
        }
    }
    //Sets the datagrid source accordingly
    function GridSetup() {
        //get the datagrid
        var dataGrid = document.getElementById('<%=GridPanGrid.ClientID%>');
        //startbox variable
        var startBox =  document.getElementById('<%=GridStartBox.ClientID%>');
            //endbox variable
        var endBox = document.getElementById('<%=GridEndBox.ClientID%>');
        //if the grid is not null
        if (dataGrid != null) {
            //set the grid to null
            document.getElementById('<%=GridPanGrid.ClientID%>').outerHTML = "";
        }
        //if the startBox is not null
        if (startBox != null) {
            //set the grid to null
            document.getElementById('<%=GridStartBox.ClientID%>').value = "";
        }
        //if the endBox is not null
        if (endBox != null) {
            //set the grid to null
            document.getElementById('<%=GridEndBox.ClientID%>').value = "";
        }
    }
    //method to hide performance window
    function CloseWin_Click() {
        //Showhide panel components
        ShowHidePer_Click();
    }
    //method adds a new empty line to the text box if the enter key is pressed
    function NewLine(e) {
        //iF the key is the enter key
        if (e.keyCode == 13) {
            //select the save button
            document.getElementById('<%=EmailSave.ClientID%>').click();
        }
    }

</script>
    <div>
        <div>
            <div class="EventDivTitle">
                <asp:Label ID="EventTitleLabel" CssClass="AddEventLabel" Text="Event Name" runat="server"></asp:Label>
            </div>
            <div class="EventDivRow">
                <asp:UpdatePanel ID="EventTitlePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                    <div class="EventDivImg">
                        <asp:Image Height="24" Width="24" ID="EventTitleVal" runat="server" />
                    </div>
                    <div>
                        <asp:HiddenField ID="HiddenEID" runat="server"/>
                        <asp:TextBox ID="EventTitle" AutoPostBack="true" MaxLength="510" width="450" runat="server"></asp:TextBox>
                    </div>
                    </ContentTemplate>
                    <Triggers><asp:AsyncPostBackTrigger ControlID="EventTitle" /></Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
        <div>
            <div class="EventDivTitle">
                <asp:Label ID="DocURLLabel" CssClass="AddEventLabel" Text="Event Documentation" runat="server"></asp:Label>
            </div>
            <div>
                <asp:UpdatePanel ID="DocPanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                    <div class="EventDivImg">
                        <asp:Image Height="24" Width="24" ID="DocVal" runat="server"  />
                    </div>
                    <div>
                        <asp:TextBox ID="DocURL" width="550" MaxLength="510" AutoPostBack="true" runat="server"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="DocValidation" CssClass="AddEventErrorText" ControlToValidate="DocURL"
                              ValidationExpression="http(s)?://([a-zA-Z0-9- ./?%&_=]*)"
                              ErrorMessage="Input valid URL!" runat="server"></asp:RegularExpressionValidator>
                        <asp:Label ID="ResponseCode1" CssClass="AddEventErrorText" Text="" runat="server"></asp:Label>
                        <asp:HiddenField ID="DocForce" Value="false" runat="server" />
                    </div>
                    </ContentTemplate>
                    <Triggers><asp:AsyncPostBackTrigger ControlID="DocURL"  /></Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
            <div>
                <div class="EventDivTitle">
                    <asp:Label ID="URLLable" CssClass="AddEventLabel" Text="Event URL" runat="server"></asp:Label>
                </div>
                <div>
                    <asp:UpdatePanel ID="UrlPanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>
                            <div class="EventDivImg">
                                <asp:Image Height="24" Width="24" ID="URLVal" runat="server"  />
                            </div>
                            <div>    
                                <div>
                                    <asp:TextBox ID="URLTextBox" width="550" MaxLength="510" AutoPostBack="true" runat="server"></asp:TextBox><asp:ImageButton ID="Email" CausesValidation="false" OnClientClick="ShowHideEmail()" CssClass="EmailEvent" ImageUrl="~/Images/Email.gif" runat="server" />
                                    <asp:RegularExpressionValidator ID="URLValidation" CssClass="AddEventErrorText" ControlToValidate="URLTextbox"
                                      ValidationExpression="(http(s)?://([a-zA-Z0-9- ./?%_&=]*))|(mailto:([a-zA-Z0-9- ;@:./_?%&=]*))|(\\([a-zA-Z0-9- ;@:./_?%&=]*))"
                                      ErrorMessage="Input valid URL!" runat="server"></asp:RegularExpressionValidator>
                                    <asp:Label CssClass="AddEventErrorText" ID="ResponseCode2" Text="" runat="server"></asp:Label>
                                    <ajaxToolkit:ModalPopupExtender ID="EmailPanBack" BackgroundCssClass="ModelBack" PopupControlID="EmailPan" TargetControlID="Email"  runat="server"></ajaxToolkit:ModalPopupExtender>    
                                    <asp:HiddenField ID="UrlForce" Value="false" runat="server" />
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers><asp:AsyncPostBackTrigger ControlID="URLTextBox" /></Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>   
            <div>
                <div class="EventDivTitle">
                    <asp:Label ID="CategoryLabel" CssClass="AddEventLabel" Text="Category" runat="server"></asp:Label>
                </div>
                <div>
                    <asp:UpdatePanel ID="CatListPanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>
                            <div class="EventDivImg">
                                <asp:Image Height="24" Width="24" ID="CategoryListVal" runat="server"   />            
                            </div>
                            <div>
                                <asp:DropDownList ID="CategoryList" CssClass="AddEventText" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0" Text="" /></asp:DropDownList>
                                <asp:CompareValidator ID="ComValCate" runat="server" ControlToValidate="CategoryList" CssClass="AddEventErrorText" ErrorMessage="Required Field. Please select a category."
                                    Operator="NotEqual" ValueToCompare="0"/>
                            </div>
                        </ContentTemplate>
                        <Triggers><asp:AsyncPostBackTrigger ControlID="CategoryList" /></Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
            <div>
                <div class="EventDivTitle">
                    <asp:Label ID="ScheduleLabel" CssClass="AddEventLabel" Text="Schedule" runat="server"></asp:Label>
                </div>
                <div>
                    <asp:UpdatePanel ID="SchedList" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                        <ContentTemplate>
                            <div class="EventDivImg">
                                <asp:Image Height="24" Width="24" ID="ScheduleListVal" runat="server"  />
                            </div>
                            <div>
                                <asp:DropDownList ID="ScheduleList" CssClass="AddEventText" OnSelectedIndexChanged="schedBox_Changed" 
                                    runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0" Text="" /></asp:DropDownList>
                                <asp:CompareValidator ID="ComValSche" runat="server" CssClass="AddEventErrorText" ControlToValidate="ScheduleList" ErrorMessage="Required Field. Please select a schedule."
                                    Operator="NotEqual" ValueToCompare="0"/>
                            </div>
                        </ContentTemplate>
                        <Triggers><asp:AsyncPostBackTrigger ControlID="ScheduleList" /></Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
            <asp:UpdatePanel ID="ListPanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <div style="padding-top:30px">
                    <div class="EventDivImg">
                        <asp:Image Height="24" Width="24" ID="DailyListVal" runat="server" Visible="false"  />
                    </div>
                    <div>
                        <div style="padding-left:50px"><asp:DropDownList ID="MonthDayList" CssClass="AddEventText" visible="false" runat="server" 
                                AutoPostBack="True"></asp:DropDownList>
                        <asp:DropDownList ID="OccurList" CssClass="AddEventText" visible="false" runat="server"  
                                AutoPostBack="True"></asp:DropDownList>
                        <asp:CheckBoxList ID="DailyList" Visible="false" AutoPostBack="true" CssClass="AddEventText" runat="server" RepeatDirection="Horizontal"></asp:CheckBoxList></div>
                    </div>
                    </div>
                    <div class="EventDivTitle">
                        <asp:Label ID="StartDateTitle" CssClass="AddEventLabel" Text="Start Date" runat="server"></asp:Label>
                    </div>
                    <div>
                        <div class="EventDivImg">
                            <asp:Image Height="24" Width="24" ID="DateBoxVal" runat="server"  />
                        </div>
                        <div>
                            <asp:TextBox ID="DateBox" CssClass="DateBox" runat="server" OnTextChanged="DateBox_TextChanged" AutoPostBack="true"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="StartDate" Format="MM/dd/yyyy" CssClass="ajax__calendarDates" runat="server" TargetControlID="DateBox"></ajaxToolkit:CalendarExtender>
                        </div>
                    </div>
                    <div class="EventDivTitle">
                        <asp:Label ID="TimeLabel" CssClass="AddEventLabel" Text="Start Time" runat="server"></asp:Label>
                    </div>
                    <div style="padding-left:50px">
                            <asp:DropDownList ID="HourList" CssClass="AddEventText" runat="server"></asp:DropDownList>:
                            <asp:DropDownList ID="MinList" CssClass="AddEventText" runat="server"></asp:DropDownList>
                    </div>
                    <div class="EventDivTitle">
                        <asp:CheckBox ID="FinishTime" CssClass="AddEventText"  Text="Finish Time Required?" runat="server"></asp:CheckBox>
                            <asp:CheckBox ID="ActiveBox" CssClass="AddEventText" Text="Active" Checked="true" runat="server"></asp:CheckBox>
                    </div>
                 </ContentTemplate>
                <Triggers><asp:AsyncPostBackTrigger ControlID="HourList" /><asp:AsyncPostBackTrigger ControlID="MinList" /><asp:AsyncPostBackTrigger ControlID="DateBox" /><asp:AsyncPostBackTrigger ControlID="MonthDayList" /><asp:AsyncPostBackTrigger ControlID="OccurList" /><asp:AsyncPostBackTrigger ControlID="DailyList" /></Triggers>
            </asp:UpdatePanel>     
        </div>
    <div>
        <asp:Panel CssClass="EmailPan" ID="EmailPan" onkeydown="NewLine(event)" runat="server">
        <asp:UpdatePanel ID="EmailUpdate" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server"><ContentTemplate> 
            <div class="ExtenderUpper">
                <div style="float:right;padding-left:5px"><asp:ImageButton ID="CloseEmaile" OnClientClick="ShowHideEmail()" CausesValidation="false" ImageUrl="~/Images/Close.jpg" Width="24" Height="16" ToolTip="Close Window" Text="Close" runat="server"/></div>
            <asp:Label ID="EmailLinkL" CssClass="PerPanTitle" Text="Email Link" runat="server" ></asp:Label> </div>
            <div style="padding:5px">
                <div style="text-align:left">
                <asp:Label ID="ToL" CssClass="AddEventText" Text="To:" runat="server" ></asp:Label>
                </div>
                <div style="text-align:left">
                <asp:TextBox ID="ToBox" CssClass="AddEventText" TextMode="MultiLine" Rows="2" width="543" runat="server"></asp:TextBox>
                </div>
                <div style="text-align:left">
                <asp:Label ID="CCL" CssClass="AddEventText" Text="CC:" runat="server" ></asp:Label>
                </div>
                <div style="text-align:left">
                <asp:TextBox ID="CCBox" CssClass="AddEventText" width="543" runat="server"></asp:TextBox>
                </div>
                <div style="text-align:left">
                <asp:Label ID="SubjectL" CssClass="AddEventText" Text="Subject:" runat="server" ></asp:Label>
                </div>
                <div style="text-align:left">
                <asp:TextBox ID="SubjectBox" CssClass="AddEventText" width="543" runat="server"></asp:TextBox>
                </div>
                <div style="text-align:left">
                <asp:Label ID="BodyL" CssClass="AddEventText" Text="Body:" runat="server" ></asp:Label>
                </div>
                <div style="text-align:left">
                <asp:TextBox ID="BodyBox" CssClass="AddEventText" TextMode="MultiLine" AutoPostBack="false" Rows="3" width="543" runat="server"></asp:TextBox>   
                </div>
                <div><asp:Button ID="EmailSave" Text="SaveEmail" OnClientClick="EmailSave_Click()" UseSubmitBehavior="false" Enabled="true" CausesValidation="false" runat="server" />
                    <asp:Button ID="ResetB" Enabled="true" Text="Reset" OnClientClick="EmailReset_Click()" CausesValidation="false" runat="server" />
               </div>
            </div>
            <div class="ExtenderLower"></div>
            </ContentTemplate></asp:UpdatePanel>
        </asp:Panel>
    </div>
     <asp:Panel CssClass="PerPan" ID="GridPan" runat="server">
         <asp:UpdatePanel ID="GridPanUpper" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <div class="ExtenderUpper">
                        <div style="float:right;padding-left:5px"><asp:ImageButton ID="CloseWinPer" OnClientClick="CloseWin_Click()"  ImageUrl="~/Images/Close.jpg" Width="24" Height="16" ToolTip="Close Window" Text="Close" runat="server"/></div>
                        <div style="text-align:center;padding-left:30px"><asp:Label ID="TitleLabel" Text="" CssClass="PerPanTitle" runat="server" ></asp:Label></div>
                    </div>                
                    <div style="text-align:center;margin-right:auto;margin-left:auto;min-width:300px">
                        <div style="padding-top:5px;"><asp:Label ID="GridEventTitle" CssClass="ComPanLabel" Enabled="true" runat="server" /></div>
                        <div>
                            <asp:RadioButtonList ID="GridTypeList" CssClass="RadioList" RepeatDirection="Horizontal" OnSelectedIndexChanged="GridTypeList_SelectedIndexChanged" AutoPostBack="true" runat="server"><asp:ListItem Text="Occurances" Selected="True" /><asp:ListItem Text="Dates" Selected="False"/></asp:RadioButtonList>
                        </div>
                        <div>  
                            <asp:Label ID="GridInfoLabel" CssClass="PerPanSubTitle" Text="Number of Occurances" runat="server" ></asp:Label>
                            <asp:RadioButtonList ID="GridList" CssClass="RadioList" RepeatDirection="Horizontal" ToolTip="Number of occurances to load" OnSelectedIndexChanged="DateList_SelectedIndexChanged" AutoPostBack="true" runat="server"><asp:ListItem Text="7" /><asp:ListItem Text="14" /><asp:ListItem Text="30" /></asp:RadioButtonList>
                        </div>
                        <div>
                            <asp:TextBox ID="GridStartBox" CssClass="DateBox" runat="server" AutoPostBack="true" Visible="false"></asp:TextBox>
                            <asp:TextBox ID="GridEndBox" CssClass="DateBox" runat="server" AutoPostBack="true" Visible="false"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="StartDateExt" Format="MM/dd/yyyy" CssClass="ajax__calendarDates" runat="server" TargetControlID="GridStartBox"></ajaxToolkit:CalendarExtender>
                            <ajaxToolkit:CalendarExtender ID="EndDateExt" Format="MM/dd/yyyy" CssClass="ajax__calendarDates" runat="server" TargetControlID="GridEndBox"></ajaxToolkit:CalendarExtender>
                            <asp:Button ID="GridDateButton" CssClass="PerPanButton" Text="Submit" ToolTip="Click to get results" runat="server" OnClick="GridDateButton_Click" Visible="false" CausesValidation="false"></asp:Button>
                        </div>
                    </div>
                    <div style="margin-left:auto;margin-right:auto">
                        <asp:HiddenField ID="methodID" runat="server" />
                        <asp:HiddenField ID="TitleH" runat="server" />
                        <asp:HiddenField ID="EventTitleH" runat="server" />
                        <asp:HiddenField ID="GridPanName" runat="server" />
                        <asp:UpdateProgress ID="LoadProgress" AssociatedUpdatePanelID="GridPanUpper" runat="server">
                            <ProgressTemplate>           
                                <asp:Image ID="LoadImg" CssClass="LoadingGif" ImageUrl="~/Images/Loading.gif" runat="server" />         
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <asp:DataGrid ID="GridPanGrid" CssClass="PerGridDataGrid" HeaderStyle-CssClass="PerGridHd" ItemStyle-CssClass="PerGridRow" AlternatingItemStyle-CssClass="PerGridRowOff" Enabled="false" runat="server"></asp:DataGrid>
                    </div>
                    <div class="ExtenderLower"></div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
</asp:content>
