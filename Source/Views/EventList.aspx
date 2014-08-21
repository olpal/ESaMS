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
<%@ page language="C#" autoeventwireup="true" masterpagefile="~/MasterPages/MasterBall.master" CodeFile="EventList.aspx.cs" inherits="Views_EventList" %>

<asp:content ID="styles" ContentPlaceHolderID="masterCSS" runat="server">
    <link href="../StyleSheets/EventListStyleSheet.css" type="text/css" rel="Stylesheet" />
    <link href="../StyleSheets/CalStyle.css" type="text/css" rel="Stylesheet" />
    <script type="text/javascript">
        //Check the key pressed in the box
        function CheckKey(e) {
            //Get the text box id
            textboxid = document.activeElement.id;
            //Get the textbox
            textbox = document.getElementById(textboxid);
            //Code of the key pressed
            var code;
            //Get the code 
            code = e.keyCode;
            //If the code is not 8 indicating backspace
            if (code != 8) {
                //If the code is the t key
                if (code == 84) {
                    //If there is an element
                    if (textboxid.length > 0) {
                        //Get the textbox
                        textbox = document.getElementById(textboxid);
                        //get the date
                        var now = new Date();
                        //get the hour
                        var hour = now.getHours();
                        //get the minute
                        var min = now.getMinutes();
                        //If the hour is only 1 digit
                        if (hour.toString().length == 1) {
                            //add a preceeding 0 to hour
                            hour = ("0" + hour);
                        }
                        //If the minute is only 1 digit
                        if (min.toString().length == 1) {
                            //add a preceeding 0 to hour
                            min = ("0" + min);
                        }
                        //Assign the time to the textbox
                        textbox.value = (hour + ":" + min)
                    }
                }
                else {
                    //Current textbox value
                    var tvalue;
                    //Get the current value
                    tvalue = textbox.value
                    //Regex expression for Text check
                    var tPres = new RegExp("[^0123456789:]", "g");
                    //Remove the invalid characters
                    textbox.value = tvalue.replace(tPres, "")
                    //Regex expression for double digit check
                    var dd = new RegExp("^[0-9][0-9]");
                    //Regex expression for double digit check exclusion
                    var dde = new RegExp("^[0-9][0-9]:");
                    //Regex expression for triple digit check 
                    var ddt = new RegExp("^[0-9][0-9][0-9]");
                    //Regex expression for double colon check 
                    var ddc = new RegExp("^[0-9][0-9]::");
                    //If the regex matches and the length of the value is 2
                    if (((dd.test(textbox.value)) && !(dde.test(textbox.value))) && tvalue.length == 2) {
                        //Add a colon
                        textbox.value += ":";
                    }
                        //If a tiple digit was entered
                    else if (ddt.test(textbox.value)) {
                        //add a colon in the appopriate place
                        var beg = textbox.value.toString().substring(0, 2);
                        var end = textbox.value.toString().substring(2);
                        textbox.value = beg + ":" + end;
                    }
                        //if a double colon is present
                    else if (ddc.test(textbox.value)) {
                        //add a colon in the appopriate place
                        textbox.value = textbox.value.toString().substring(0, 2) + ":";
                    }
                }
            }
            return false;
        }
        //Check the key pressed in the box
        function CheckDateKey(e) {
            //Get the text box id
            textboxid = document.activeElement.id;
            //Get the textbox
            textbox = document.getElementById(textboxid);
            //Code of the key pressed
            var code;
            //Get the code 
            code = e.keyCode;
            //If the code is the t key
            if (code == 84) {
                //get the date
                var now = new Date();
                //get the hour
                var year = now.getFullYear();
                //get the minute
                var mon = (now.getMonth() + 1);
                //get the minute
                var day = now.getDate();
                //If the hour is only 1 digit
                if (mon.toString().length == 1) {
                    //add a preceeding 0 to month
                    mon = ("0" + mon);
                }
                //If the minute is only 1 digit
                if (day.toString().length == 1) {
                    //add a preceeding 0 to day
                    day = ("0" + day);
                }
                //Assign the time to the textbox
                textbox.value = (mon + "/" + day + "/" + year)
            }
        }
        //Double click time event
        function MouseClick() {
            //Get the text box id
            textboxid = document.activeElement.id;
            //If there is an element
            if (textboxid.length > 0) {
                //Get the textbox
                textbox = document.getElementById(textboxid);
                //get the date
                var now = new Date();
                //get the hour
                var hour = now.getHours();
                //get the minute
                var min = now.getMinutes();
                //If the hour is only 1 digit
                if (hour.toString().length == 1) {
                    //add a preceeding 0 to hour
                    hour = ("0" + hour);
                }
                //If the minute is only 1 digit
                if (min.toString().length == 1) {
                    //add a preceeding 0 to hour
                    min = ("0" + min);
                }
                //Assign the time to the textbox
                textbox.value = (hour + ":" + min)
                //Get the id of the date box
                textboxdate = document.getElementById(textboxid.replace("Time", "Date"));
                //Set focus to the date box
                textboxdate.focus();
                //Call the date fill method
                MouseDateClick();
                //remove focus from the datebox
                textboxdate.blur();
            }

        }
        //Double click date event
        function MouseDateClick() {
            //Get the text box id
            textboxid = document.activeElement.id;
            //If there is an element
            if (textboxid.length > 0) {
                //Get the textbox
                datetextbox = document.getElementById(textboxid);
                //get the date
                var now = new Date();
                //get the hour
                var year = now.getFullYear();
                //get the minute
                var mon = (now.getMonth() + 1);
                //get the minute
                var day = now.getDate();
                //If the hour is only 1 digit
                if (mon.toString().length == 1) {
                    //add a preceeding 0 to month
                    mon = ("0" + mon);
                }
                //If the minute is only 1 digit
                if (day.toString().length == 1) {
                    //add a preceeding 0 to day
                    day = ("0" + day);
                }
                //Assign the time to the textbox
                datetextbox.value = (mon + "/" + day + "/" + year)
            }
        }
        //Sets up the performance event id in the Performance Panel
        function InfoSetup(button) {
            //Id of active button
            var activeButton = button.id;
            //If the button is the performance button
            if (activeButton.toString().indexOf("Performance") != -1) {
                //Replace the name of the button with the name of the EID field
                var eidID = activeButton.replace("PerformanceListB", "EIDH");
                //Get the hidden field value
                var EIDH = document.getElementById(eidID).value;
                //set the deid field
                document.getElementById('<%=PEIDH.ClientID%>').value = EIDH;
                //set the event title lable
                document.getElementById('<%=PerEventTitle.ClientID%>').innerHTML = button.parentNode.parentNode.cells[4].innerHTML;
                //set the event title hidden field
                document.getElementById('<%=PerEventTitleH.ClientID%>').value = button.parentNode.parentNode.cells[4].innerHTML;
                //Set the open panel id
                document.getElementById('<%=OpenPanel.ClientID%>').value = '<%=PerPan.ClientID%>';
                //Set the open panel background id
                document.getElementById('<%=OpenPanelBack.ClientID%>').value = (activeButton.replace("PerformanceListB", "PerExtend") + '_backgroundElement');
                //Ensure the datagrid is setup 
                PerGridSetup();
            }
            //If the button is the comment button
            else if (activeButton.indexOf("CommentB") != -1) {
                //Replace the name of the button with the name of the EID field
                var eidID = activeButton.replace("CommentB", "EIDH");
                //Replace the name of the button with the name of the SchedTime  field
                var SchedTime = activeButton.replace("CommentB", "SchedTimeH");
                //Get the hidden field value
                var EIDH = document.getElementById(eidID).value;
                //set the deid field
                document.getElementById('<%=CEIDH.ClientID%>').value = EIDH;
                //set the event title lable
                document.getElementById('<%=ComEventTitle.ClientID%>').innerHTML = button.parentNode.parentNode.parentNode.cells[4].innerHTML;
                //set the event title hidden field
                document.getElementById('<%=ComEventTitleH.ClientID%>').value = button.parentNode.parentNode.parentNode.cells[4].innerHTML;
                //Set the scheduled time
                document.getElementById('<%=ComSchedTime.ClientID%>').value = document.getElementById(SchedTime).value;
                //Set the open panel id
                document.getElementById('<%=OpenPanel.ClientID%>').value = '<%=ComPan.ClientID%>';
                //Set the open panel background id
                document.getElementById('<%=OpenPanelBack.ClientID%>').value = (activeButton.replace("CommentB", "CommentExtend") + '_backgroundElement');
                //Ensure the datagrid is setup 
                ComGridSetup();
            }
            //Else if the Forward Image Button
            else if (activeButton.indexOf("ForImageB") != -1) {
                //Set the open panel id
                document.getElementById('<%=OpenPanel.ClientID%>').value = '<%=ForPanel.ClientID%>';
                //Set the open panel background id
                document.getElementById('<%=OpenPanelBack.ClientID%>').value = 'ForPop_backgroundElement';
                //Setup the grid
                ForGridSetup();
            }
            //Else if the event image button
            else if (activeButton.indexOf("EventImageB") != -1) {
                //Set the open panel id
                document.getElementById('<%=OpenPanel.ClientID%>').value = '<%=EventPan.ClientID%>';
                //Set the open panel background id
                document.getElementById('<%=OpenPanelBack.ClientID%>').value = 'EveTPop_backgroundElement';
                //Setup the grid
                EventGridSetup();
            }
            //Show the window
            ShowHidePanel_Click();
        }
        //Function to show/hide performance window
        function ShowHidePanel_Click() {
            //get the panel
            var pan = document.getElementById(document.getElementById('<%=OpenPanel.ClientID%>').value);
            //back panel
            var backPan = document.getElementById(document.getElementById('<%=OpenPanelBack.ClientID%>').value);
            //Get the timer control
            var timer = document.getElementById("masterMain_updateTimer");
            //if the panel is hidden
            if (pan.style.visibility == 'hidden' || pan.style.visibility == '') {
                //stop the timer
                timer.control._stopTimer();
                //Show the panel
                pan.style.visibility = 'visible';
            }
            else {
                //start the timer
                timer.control._startTimer();
                //Hide the panel
                pan.style.visibility = 'hidden';  
            }
            //if backPan exists
            if (backPan != null) {
                //if the panel is hidden //Show the panel
                if (backPan.style.visibility == 'hidden' || backPan.style.visibility == '') { backPan.style.visibility = 'visible'; }
                    //Hide the panel
                else { backPan.style.visibility = 'hidden'; }
            }
        }
        //Sets the performance gird window components
        function PerGridSetup() {
            //get the performance datagrid
            var perGrid = document.getElementById('<%=PerGrid.ClientID%>');
            //if the grid is not null
            if (perGrid != null) {
                //set the grid to null
                document.getElementById('<%=PerGrid.ClientID%>').outerHTML = "";
            }
        }
        //Sets the comment gird window components
        function ComGridSetup() {
            //get the comment datagrid
            var comGrid = document.getElementById('<%=ComGrid.ClientID%>');
            //if the grid is not null
            if (comGrid != null) {
                //set the grid to null
                document.getElementById('<%=ComGrid.ClientID%>').outerHTML = "";
            }
            //Set the prior count value to 1
            document.getElementById('<%=PriorCountH.ClientID%>').value = "1";
        }
        //Sets the forward grid window components
        function ForGridSetup() {
            //get the datagrid
            var forGrid = document.getElementById('<%=ForGrid.ClientID%>');
            //if the grid is not null
            if (forGrid != null) {
                //set the grid to null
                document.getElementById('<%=ForGrid.ClientID%>').outerHTML = "";
            }
            //Set the prior count value to 1
            document.getElementById('<%=ForEventsNum.ClientID%>').innerHTML = "0";
        }
        //Sets the event adding window components
        function EventGridSetup() {
            //Reset all the event add panel components
            document.getElementById('<%=EventStatL.ClientID%>').innerHTML = "";
            document.getElementById('<%=EventTitle.ClientID%>').value = "";
            document.getElementById('<%=URLTextBox.ClientID%>').value = "";
            document.getElementById('<%=ResponseName.ClientID%>').innerHTML = "";
            document.getElementById('<%=ResponseDate.ClientID%>').innerHTML = "";
            document.getElementById('<%=DateBox.ClientID%>').value = "";
            document.getElementById('<%=CategoryList.ClientID%>').selectedIndex = 0;
            document.getElementById('<%=MinList.ClientID%>').selectedIndex = 0;
            document.getElementById('<%=HourList.ClientID%>').selectedIndex = 0;
            document.getElementById('<%=FinishTime.ClientID%>').checked = false;
        }
</script> 
</asp:content>
<asp:content ID="mainContent" ContentPlaceHolderID="masterMain" runat="server">
    <asp:Panel ID="GridPanel" runat="server">
    <asp:UpdatePanel ID="GridViewPanel" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
        <ContentTemplate> 
            <asp:HiddenField ID="CurrentShift" runat="server"/>
            <asp:HiddenField ID="OpenPanel" runat="server"/>
            <asp:HiddenField ID="OpenPanelBack" runat="server"/>
            <asp:Timer ID="updateTimer" runat="server" Enabled="true" Interval="600000" OnTick="updateControl" />
            <div style="margin:auto;padding-top:5px;text-align:center">
                <asp:Label ID="PageName" CssClass="InternalHeaderText" runat="server" ></asp:Label>
                <asp:Label ID="PageDate" CssClass="InternalSubHeaderText" runat="server" ></asp:Label>
            </div>
            <div style="margin:auto;padding:10px">
                <asp:GridView ID="ManageGrid" runat="server" CssClass="TableMain" HeaderStyle-CssClass="HeaderTextTable" GridLines="Horizontal" CellPadding="2" OnRowDataBound="rowDataGridBind" AutoGenerateColumns="false" HorizontalAlign="Center">
                    <Columns>
                     <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Image ID="StatusIm" CssClass="TableComponents" ToolTip="Record Status"  runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Time" DataField="SchedTime" ItemStyle-CssClass="TableCellTextC" />
                     <asp:TemplateField ItemStyle-HorizontalAlign="Center" >
                        <ItemTemplate>
                            <asp:ImageButton ID="PerformanceListB" CssClass="TableComponents" OnClientClick="InfoSetup(this)" TabIndex="-1" ImageUrl="~/Images/Time.gif" runat="server"  ToolTip="Performance" CausesValidation="False" />
                                <ajaxToolkit:ModalPopupExtender ID="PerExtend" runat="server" PopupControlID='<%#PerPan.ClientID%>' TargetControlID="PerformanceListB" BackgroundCssClass="ModelBack"></ajaxToolkit:ModalPopupExtender>     
                         </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Category" DataField="categoryName"  ItemStyle-CssClass="TableCellTextC"/>
                    <asp:BoundField HeaderText="Event Name" DataField="name" ItemStyle-CssClass="RowText" />
                    <asp:TemplateField HeaderText="Start Time" HeaderStyle-Width="128"  ItemStyle-CssClass="TableCellTextC">
                        <ItemTemplate>
                            <div id="StartColumn" style="text-align:center">
                            <asp:HiddenField ID="StartChanged" Value="false" runat="server" />
                            <asp:TextBox CssClass="TableTimeBox" ID="StartTimeBox" runat="server" MaxLength="5" OnTextChanged="rowEdited" ondblclick="MouseClick()" onkeyup="CheckKey(event)"></asp:TextBox><asp:TextBox CssClass="TableDateBox" OnTextChanged="rowEdited" ID="StartDateBox" ondblclick="MouseDateClick()" onkeyup="CheckDateKey(event)" MaxLength="10" runat="server"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="StartDateCal" CssClass="ajax__calendarDates" runat="server"  Format="MM/dd/yyyy" TargetControlID="StartDateBox"></ajaxToolkit:CalendarExtender>
                         </div>
                         </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="End Time" HeaderStyle-Width="128" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate> 
                        <asp:TextBox CssClass="TableTimeBox" ID="EndTimeBox" runat="server" MaxLength="5" OnTextChanged="rowEdited" ondblclick="MouseClick()" onkeyup="CheckKey(event)"></asp:TextBox><asp:TextBox CssClass="TableDateBox" OnTextChanged="rowEdited" ID="EndDateBox" ondblclick="MouseDateClick()" onkeyup="CheckDateKey(event)" MaxLength="10" runat="server"></asp:TextBox>
                        <ajaxToolkit:CalendarExtender ID="EndDateCal" CssClass="ajax__calendarDates" runat="server" Format="MM/dd/yyyy" TargetControlID="EndDateBox"></ajaxToolkit:CalendarExtender>
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Active" ItemStyle-CssClass="TableCellTextC">
                    <ItemTemplate>
                        <asp:CheckBox ID="ActiveCheck" TabIndex="-1" runat="server" AutoPostBack="true" Checked='<%# bool.Parse(Eval("active").ToString()) %>' Enabled="false"/>
                    </ItemTemplate>
                    </asp:TemplateField>                  
                    <asp:TemplateField HeaderText="Controls" HeaderStyle-Width="138">
                        <ItemTemplate>
                            <div style="text-align:justify">
                            <asp:HyperLink ID="DocB" TabIndex="-1" runat="server" Target="_blank" CausesValidation="False" Visible="True"><asp:Image runat="server" ID="DocLinkImg" ImageUrl="~/Images/Doc1Dis.gif" CssClass="ELTableComponents" /></asp:HyperLink>
                            <asp:HyperLink ID="UrlB" TabIndex="-1" Target="_blank" runat="server" CausesValidation="False" Visible="True"><asp:Image runat="server" ID="SiteLinkImg" ImageUrl="~/Images/LinkDis.gif" CssClass="ELTableComponents" /></asp:HyperLink>
                            <asp:ImageButton ID='CommentB' TabIndex="-1" ImageUrl="~/Images/Comment.gif" ToolTip="Comment" runat="server" CommandName="Comment" CssClass="ELTableComponents" OnClientClick="InfoSetup(this)" CausesValidation="False"/>   
                            <asp:ImageButton ID='StatusB' TabIndex="-1" ImageUrl="~/Images/Disable.gif" runat="server" ToolTip="Disable" CommandName="ActiveStatus" onClick="iButtonActive_Click"  CssClass="ELTableComponents" CausesValidation="False"/>
                            <ajaxToolkit:ModalPopupExtender ID="CommentExtend" runat="server" PopupControlID='<%#ComPan.ClientID%>' TargetControlID="CommentB" BackgroundCssClass="ModelBack"></ajaxToolkit:ModalPopupExtender>
                            <asp:HiddenField ID="ELIDH" runat="server"/>
                            <asp:HiddenField ID="EIDH" runat="server"/>
                            <asp:HiddenField ID="SchedTimeH" runat="server"/>
                            <asp:HiddenField ID="Forward" Value="false" runat="server" />
                            <asp:HiddenField ID="Saved" Value="false" runat="server"/>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
             </asp:GridView>
            </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:Panel CssClass="PerPan" ID="PerPan" runat="server">  
        <asp:UpdatePanel ID="PerData" UpdateMode="Conditional" runat="server">
            <ContentTemplate>   
                    <div class="ExtenderUpper">
                        <div></div>
                        <div style="float:right;padding-left:5px"><asp:ImageButton ID="CloseWinPer" OnClientClick="ShowHidePanel_Click();return false;" ImageUrl="~/Images/Close.jpg" Width="24" Height="16" ToolTip="Close Window" Text="Close" runat="server"/></div>
                        <div style="text-align:center;padding-left:30px"><asp:Label ID="TitleLabelPer" CssClass="PerPanTitle" Text="Performance History" runat="server" ></asp:Label></div>
                    </div>
                    <div style="min-height:95px;min-width:300px">
                        <asp:Label ID="InfoLabel" CssClass="PerPanSubTitle" Text="Number of Occurances" runat="server" ></asp:Label>  
                        <asp:RadioButtonList ID="DateListPer" CssClass="RadioList" RepeatDirection="Horizontal" ToolTip="The number occurances to load" AutoPostBack="true" OnSelectedIndexChanged="DateListPer_SelectedIndexChanged"  runat="server"><asp:ListItem Text="3" /><asp:ListItem Text="7" /><asp:ListItem Text="14" /></asp:RadioButtonList>
                        <asp:Label ID="PerEventTitle" CssClass="PerPanLabel" Enabled="true" runat="server" />
                        <asp:UpdateProgress ID="LoadProgressPer" AssociatedUpdatePanelID="PerData" runat="server">
                            <ProgressTemplate>           
                                <asp:Image ID="LoadImgPer" CssClass="LoadingGif" ImageUrl="~/Images/Loading.gif" runat="server" />         
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <asp:HiddenField ID="PEIDH" runat="server" />
                        <asp:HiddenField ID="PerEventTitleH" runat="server" />
                    </div>
                    <div style="padding:5px">
                        <asp:DataGrid ID="PerGrid" HeaderStyle-CssClass="PerGridHd" ItemStyle-CssClass="PerGridRow" AlternatingItemStyle-CssClass="PerGridRowOff" Enabled="false" runat="server"></asp:DataGrid>     
                    </div>
                        <div class="ExtenderLower"></div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:Panel CssClass="PerPan" ID="ForPanel" runat="server">  
        <asp:UpdatePanel ID="ForwardData" UpdateMode="Conditional" runat="server">
            <ContentTemplate>   
                    <div class="ExtenderUpper">
                        <div></div>
                        <div style="float:right;padding-left:5px"><asp:ImageButton ID="ForCloseButton" OnClientClick="ShowHidePanel_Click();return false;" ImageUrl="~/Images/Close.jpg" Width="24" Height="16" ToolTip="Close Window" Text="Close" runat="server"/></div>
                        <div style="text-align:center;padding-left:30px"><asp:Label ID="ForLabel" CssClass="PerPanTitle" Text="Forward Events Preview" runat="server" ></asp:Label></div>
                    </div>
                    <div style="text-align:center;padding-top:10px;"><asp:Label ID="ForEventsNum" runat="server" Text="0" CssClass="ForPanEventNum"/><asp:Label ID="ForNumLab" CssClass="ForPanEventNum" Text=" Events will forward" runat="server" ></asp:Label></div>
                    <div style="padding:10px">
                        <asp:Panel ID="ScrollGridPanel" CssClass="ForPan" ScrollBars="Vertical" runat="server">
                            <asp:GridView ID="ForGrid" runat="server" CssClass="TableMain" HeaderStyle-CssClass="HeaderTextTable" GridLines="Horizontal" CellPadding="4" AutoGenerateColumns="false" HorizontalAlign="Center" OnRowDataBound="ForGrid_RowDataBound">
                                <Columns>
                                <asp:BoundField HeaderText="Time" DataField="SchedTime" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="TableCellTextC" />
                                <asp:BoundField HeaderText="Category" DataField="categoryName" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="TableCellTextC"/>
                                <asp:BoundField HeaderText="Event Name" DataField="name" ItemStyle-CssClass="RowText" />
                                <asp:BoundField HeaderText="Start Time" DataField="StartTime" ItemStyle-CssClass="TableCellTextC"/>
                                <asp:BoundField HeaderText="Start Date" DataField="StartDate" ItemStyle-CssClass="TableCellTextC"/>
                            </Columns>
                         </asp:GridView>     
                        </asp:Panel>
                    </div>
                    <div>
                        <asp:Button ID="GetData" Text="View Events" OnClick="GetData_Click" runat="server" />
                    </div>
                    <div class="ExtenderLower"></div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:Panel CssClass="PerPan" ID="ComPan" runat="server">
        <asp:UpdatePanel ID="ComData" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
            <ContentTemplate>      
                    <div class="ExtenderUpper">
                        <div></div>
                        <div style="float:right;padding-left:5px"><asp:ImageButton ID="CloseWinCom" OnClientClick="ShowHidePanel_Click();return false;" CausesValidation="false" ImageUrl="~/Images/Close.jpg" Width="24" Height="16" ToolTip="Close Window" Text="Close" runat="server"/></div>
                        <div style="text-align:center;padding-left:30px"><asp:Label ID="TitleLabelCom" CssClass="PerPanTitle" Text="Comment History" runat="server" ></asp:Label></div>
                    </div>                   
                    <div style="min-height:95px">
                        <asp:Label ID="InfoLabelCom" CssClass="PerPanSubTitle" Text="Number of Occurances" runat="server" ></asp:Label>  
                        <asp:RadioButtonList ID="DateListCom" CssClass="RadioList" RepeatDirection="Horizontal" ToolTip="The number occurances to load" OnSelectedIndexChanged="DateListCom_SelectedIndexChanged" AutoPostBack="true" runat="server"><asp:ListItem Text="3" /><asp:ListItem Text="7" /><asp:ListItem Text="10" /></asp:RadioButtonList>
                        <asp:Label ID="ComEventTitle" CssClass="ComPanLabel" Enabled="true" runat="server" />
                        <asp:UpdateProgress ID="LoadProgressCom" AssociatedUpdatePanelID="ComData" runat="server">
                            <ProgressTemplate>           
                                <asp:Image ID="LoadImgCom" CssClass="LoadingGif" ImageUrl="~/Images/Loading.gif" runat="server" />         
                            </ProgressTemplate>
                        </asp:UpdateProgress>        
                    </div>
                    <div style="margin-left:auto;margin-right:auto;min-width:600px;padding:5px">
                        <asp:HiddenField ID="CEIDH" runat="server" />
                        <asp:HiddenField ID="PriorCountH" Value="1" runat="server" />
                        <asp:HiddenField ID="ComEventTitleH" runat="server" />
                        <asp:HiddenField ID="ComSchedTime" runat="server" />
                    <asp:GridView ID="ComGrid" HeaderStyle-CssClass="PerGridHd" RowStyle-CssClass="PerGridRow" AlternatingRowStyle-CssClass="PerGridRowOff" Enabled="true" runat="server"
                            AutoGenerateColumns="false" OnRowEditing="ComView_RowEditing" OnRowDeleting="ComGrid_RowDeleting" DataKeyNames="CID" OnRowDataBound="ComGrid_RowDataBound" OnRowUpdating="ComView_RowUpdating" OnRowCancelingEdit="ComView_RowCancelingEdit">
                        <EditRowStyle CssClass="CommentRowEdit" />    
                        <Columns>
                            <asp:BoundField HeaderText="Id" DataField="CID" ReadOnly="true" />
                            <asp:BoundField HeaderText="EventDate" DataField="EventDate" ReadOnly="true" />
                            <asp:BoundField HeaderText="EventTime" DataField="EventTime" ReadOnly="true" />
                            <asp:BoundField HeaderText="CommentDate" DataField="CommDate" ReadOnly="true" />
                            <asp:BoundField HeaderText="CommentTime" DataField="CommTime" ReadOnly="true" />
                            <asp:BoundField HeaderText="UserName" DataField="UserName" ReadOnly="true" />
                            <asp:TemplateField HeaderText="Comment" >
                                <ItemTemplate>
                                        <asp:TextBox ID="CommentBox" MaxLength="510" CssClass="CommentTexboxNormal" TextMode="MultiLine" Rows="2" runat="server" Text='<%# ( Eval("Comment").ToString()) %>' ReadOnly="true"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Controls">
                                <ItemTemplate>
                                  <asp:LinkButton ID="CancelLB" Text="Cancel" Visible="false" CommandName="Cancel" runat="server" />
                                  <asp:LinkButton ID="EditLB" Text="Edit" CommandName="Edit" runat="server" />
                                  <asp:LinkButton ID="UpdateLB" Text="Update" CommandName="Update" Visible="false" runat="server" />
                                  <asp:LinkButton ID="DeleteLB" Text="Delete" CommandName="Delete" runat="server" />
                                    <asp:HiddenField ID="Editable" runat="server" />
                                </ItemTemplate>
                           </asp:TemplateField>

                        </Columns>
                    </asp:GridView>
                    <asp:LinkButton ID="InsertLB" CommandName="Insert" OnClick="InsertLB_Click" Visible="true" runat="server" ><asp:Image ID="AddComImg" ToolTip="Add Comment" ImageUrl="~/Images/Plus.gif" CssClass="CommentButton" runat="server" /></asp:LinkButton>
                    </div>
                <div class="ExtenderLower"></div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:Panel CssClass="PerPan" ID="EventPan" runat="server">
        <asp:UpdatePanel ID="EventPanUpdate" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                    <div class="ExtenderUpper">
                        <div style="float:right;padding-left:5px"><asp:ImageButton ID="CloseButton" OnClientClick="ShowHidePanel_Click()" ImageUrl="~/Images/Close.jpg" Width="24" Height="16" ToolTip="Close Window" Text="Close" runat="server"/></div>
                        <div style="text-align:center;padding-left:30px"><asp:Label ID="TitleLabel" Text="One Time Event Adding" CssClass="PerPanTitle" runat="server" ></asp:Label></div>
                    </div>
                    <div style="text-align:left;padding:10px">               
                        <div class="EventAddDivTitle">
                            <asp:Label ID="EventTitleLabel" CssClass="AddEventLabel" Text="Event Name" runat="server"><asp:Label ID="ResponseName" CssClass="AddEventErrorText" Text="" runat="server"/></asp:Label>
                        </div>
                        <div>
                            <asp:TextBox ID="EventTitle" AutoPostBack="false" MaxLength="510" width="450" runat="server"></asp:TextBox>
                        </div>
                        <div class="EventAddDivTitle">
                            <asp:Label ID="URLLable" CssClass="AddEventLabel" Text="Event URL" runat="server"></asp:Label>
                        </div>
                        <div>
                            <asp:TextBox ID="URLTextBox" width="550" MaxLength="510" AutoPostBack="false" runat="server"></asp:TextBox>
                        </div>   
                        <div class="EventAddDivTitle">
                            <asp:Label ID="CategoryLabel" CssClass="AddEventLabel" Text="Category" runat="server"></asp:Label>
                        </div>
                        <div>
                            <asp:DropDownList ID="CategoryList" CssClass="AddEventText" runat="server" AutoPostBack="false" />
                        </div>
                        <div class="EventAddDivTitle">
                            <asp:Label ID="StartDateTitle" CssClass="AddEventLabel" Text="Start Date" runat="server"></asp:Label>
                        </div>
                        <div>
                            <asp:TextBox ID="DateBox" CssClass="DateBox" runat="server" AutoPostBack="false"></asp:TextBox>
                            <asp:Label ID="ResponseDate" CssClass="AddEventErrorText" Text="" runat="server"/>
                            <ajaxToolkit:CalendarExtender ID="StartDate" Format="MM/dd/yyyy" CssClass="ajax__calendarDates" runat="server" TargetControlID="DateBox"></ajaxToolkit:CalendarExtender>
                        </div>
                        <div class="EventAddDivTitle">
                            <asp:Label ID="TimeLabel" CssClass="AddEventLabel" Text="Start Time" runat="server"></asp:Label>
                        </div>
                        <div>
                            <asp:DropDownList ID="HourList" CssClass="AddEventText" runat="server"></asp:DropDownList>:
                            <asp:DropDownList ID="MinList" CssClass="AddEventText" runat="server"></asp:DropDownList>
                        </div>
                        <div class="EventAddDivTitle">
                            <asp:CheckBox ID="FinishTime" CssClass="AddEventText" Checked="false" Text="Finish Time Required?" runat="server"></asp:CheckBox>
                        </div>
                        <div style="text-align:center">
                            <asp:ImageButton ID="SubmitEvent" ImageUrl="~/Images/Plus.gif" CssClass="LowerControlButtons" ToolTip="Add Event" OnClick="SubmitEvent_Click" runat="server" />
                        </div>
                        <div style="text-align:center;padding-top:3px">
                            <asp:Label ID="EventStatL" CssClass="StatusLower" Text="" runat="server"/> 
                        </div>
                    </div> 
                    <div>
                        <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="EventPanUpdate" runat="server">
                            <ProgressTemplate>           
                                <asp:Image ID="LoadImgEve" CssClass="LoadingGif" ImageUrl="~/Images/Loading.gif" runat="server" />         
                            </ProgressTemplate>
                        </asp:UpdateProgress>  
                    </div>
                <div class="ExtenderLower"></div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
</asp:content>