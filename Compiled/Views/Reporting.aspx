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
<%@ page language="C#" autoeventwireup="true" masterpagefile="~/MasterPages/MasterBall.master" inherits="Views_Reporting, App_Web_j4ivqzes" maintainscrollpositiononpostback="true" %>

<asp:content ID="styles" ContentPlaceHolderID="masterCSS" runat="server">
    <link href="../StyleSheets/CalStyle.Css" type="text/css" rel="Stylesheet" />
</asp:content>
<asp:content ID="mainContent" ContentPlaceHolderID="masterMain" runat="server">
<script type="text/JavaScript" language="JavaScript">
    //function called on page load
    function pageLoad() {
        //get the page manager
        var pageMan = Sys.WebForms.PageRequestManager.getInstance();
        //Add begin and end methods
        pageMan.add_beginRequest(beginLoad);
        //set the panel size
        setPanelSize();
        //focus on enter
        focusEnter();
    }
    //function called when updatepanel is begining to load
    function beginLoad(sender, args) {
        var progress = document.getElementById('<%=LoadProgressReport.ClientID%>');
        //Set css display to block
        progress.style.display = "block";
    }
        //Sets up the performance event id in the Performance Panel
    function InfoSetup(button) {
            //Id of active button
            var activeButton = button.id;
            //Replace the name of the button with the name of the EID field
            var eidID = activeButton.replace("CommentB", "EIDH");
            //Get the hidden field value
            var EIDH = document.getElementById(eidID).value;
            //set the peid field
            document.getElementById('<%=CEIDH.ClientID%>').value = EIDH;
            //set the peid field
            document.getElementById('<%=ComCount.ClientID%>').innerHTML = "0 Comments";
            //Set the backpanel id
            document.getElementById('<%=BackPanIdCom.ClientID%>').value = (activeButton.replace("CommentB", "CommentExtend") + "_backgroundElement");
            //Clear panel datasources
            ComGridSetup();
            //show the panel
            ShowHideCom_Click();
        }
        //method to show/hide comment window
    function ShowHideCom_Click() {
            //get the panel
            var pan = document.getElementById('<%=ComPan.ClientID%>');
            //if the panel is hidden //Show the panel
            if (pan.style.visibility == 'hidden' || pan.style.visibility == '') { pan.style.visibility = 'visible'; }
                //Hide the panel
            else { pan.style.visibility = 'hidden'; }
            //get the panel
            var backPan = document.getElementById(document.getElementById('<%=BackPanIdCom.ClientID%>').value);
            //if backPan exists
            if (backPan != null) {
                //if the panel is hidden //Show the panel
                if (backPan.style.visibility == 'hidden' || backPan.style.visibility == '') { backPan.style.visibility = 'visible'; }
                    //Hide the panel
                else { backPan.style.visibility = 'hidden'; }
            }
        }
        //Sets the datagrid sources accordingly
    function ComGridSetup() {
            //get the datagrid
            var comGrid = document.getElementById('<%=ComGrid.ClientID%>');
            //if the grid is not null
            if (comGrid != null) {
                //set the grid to null
                document.getElementById('<%=ComGrid.ClientID%>').outerHTML = "";
            }
        }
</script>
        <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
        <ContentTemplate>
            <asp:Panel ID="RepPan" runat="server">
                <asp:Label ID="CondLabel" Text="Report Generation Options" CssClass="RepTitle" runat="server"/>
                <table style="margin-left:auto;margin-right:auto"><tr>
                <td style="vertical-align:bottom">
                    <div style="padding:10px"><asp:CheckBox ID="EventListC" CssClass="AddEventText" Text="By Shift" Checked="True" ToolTip="Query data based on preset shift times" AutoPostBack="true" OnCheckedChanged="EventListC_CheckedChanged" runat="server" /></div>
                    <div style="padding:10px"><asp:CheckBox ID="FreeC" CssClass="AddEventText" Text="By Custom Time" Checked="false" ToolTip="Query data based on custom times" AutoPostBack="true" OnCheckedChanged="FreeC_CheckedChanged" runat="server" /></div>
                    <div style="padding:10px"><asp:CheckBox ID="EventAudit" CssClass="AddEventText" Text="Event Audit" Checked="false" ToolTip="Query past event list data" runat="server" /></div>
                </td>
                <td style="vertical-align:bottom;text-align:right">
                    <div style="padding:10px"><asp:Label ID="StartDateTitle" CssClass="AddEventText" Text="Shift Type" runat="server"></asp:Label>
                    <asp:TextBox ID="DateBoxStart" CssClass="DateBox" onkeydown="EnterKey(event)" Visible="false" runat="server"></asp:TextBox><asp:DropDownList CssClass="AddEventText" ID="ShiftList" visible="true" runat="server"></asp:DropDownList>
                    <ajaxToolkit:CalendarExtender CssClass="ajax__calendarDates" Format="MM/dd/yyyy" ID="StartDate" runat="server" TargetControlID="DateBoxStart"></ajaxToolkit:CalendarExtender></div>
                    <div style="padding:10px"><asp:Label ID="EndDateLabel" CssClass="AddEventText" Text="Shift Date" runat="server"></asp:Label>
                    <asp:TextBox ID="DateBoxEnd" CssClass="DateBox" onkeydown="EnterKey(event)" runat="server"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender CssClass="ajax__calendarDates" Format="MM/dd/yyyy" ID="EndDate" runat="server" TargetControlID="DateBoxEnd"></ajaxToolkit:CalendarExtender></div>
                    <div style="padding:10px"><asp:DropDownList CssClass="AddEventText" ID="CatList" OnSelectedIndexChanged="cat_Changed" AutoPostBack="true" runat="server"/></div>
                </td><td style="vertical-align:Top;text-align:right">
                    <div style="padding:10px"><asp:Label CssClass="AddEventText" ID="StartTimeLabel" Text="Start Time" runat="server"></asp:Label>
                    <asp:DropDownList ID="HourListStart" CssClass="AddEventText" enabled="false" runat="server"></asp:DropDownList><asp:Label CssClass="AddEventText" Visible="false" ID="Colon1" Text=":" runat="server"/>
                    <asp:DropDownList ID="MinListStart" CssClass="AddEventText" enabled="false" runat="server"></asp:DropDownList></div>
                    <div style="padding:10px"><asp:Label ID="EndTimeLabel" CssClass="AddEventText" Text="End Time" runat="server"></asp:Label>
                    <asp:DropDownList ID="HourListEnd" CssClass="AddEventText" Enabled="false" runat="server"></asp:DropDownList><asp:Label CssClass="AddEventText" Visible="false" ID="Colon2" Text=":" runat="server"/>
                    <asp:DropDownList ID="MinListEnd" CssClass="AddEventText" enabled="false" runat="server"></asp:DropDownList></div>
                </td>
                </tr>
                    <tr>
                    <td></td>
                    <td style="text-align:center">
                    <asp:UpdateProgress ID="LoadProgressReport" AssociatedUpdatePanelID="GridViewPanel" runat="server">
                        <ProgressTemplate>           
                            <asp:Image ID="LoadImg" CssClass="LoadingGif" ImageUrl="~/Images/Loading.gif" runat="server" />         
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    </td>
                    <td></td></tr>
                </table>
            </asp:Panel>
     </ContentTemplate>
    </asp:UpdatePanel>   
    <asp:UpdatePanel ID="GridViewPanel" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
        <ContentTemplate>
            <div style="margin-left:auto;margin-right:auto">
            <asp:GridView ID="ManageGrid" runat="server" CellPadding="4" GridLines="Horizontal" HorizontalAlign="Center" HeaderStyle-CssClass="HeaderTextTable" RowStyle-CssClass="RowText" OnRowDataBound="ManageGrid_RowDataBound" AutoGenerateColumns="false">
                <Columns>
                    <asp:BoundField HeaderText="EventID" DataField="eid" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField HeaderText="Shift Date" DataField="StartDate" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField HeaderText="Time" DataField="StartTime" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField HeaderText="Category" DataField="categoryName" ItemStyle-HorizontalAlign="Left"/>
                    <asp:BoundField HeaderText="Event Name" DataField="name" HeaderStyle-HorizontalAlign="Center"/>
                    <asp:TemplateField HeaderText="Controls" HeaderStyle-Width="64">
                        <ItemTemplate>
                            <div style="text-align:left">
                            <asp:HyperLink ID="DocB" runat="server" Target="_blank" CommandName="Documentation" CausesValidation="False"  Visible="True"><asp:Image runat="server" ID="DocLinkImg" ImageUrl="~/Images/Doc1Dis.gif" BorderStyle="None" CssClass="ELTableComponents" /></asp:HyperLink>
                            <asp:HyperLink ID="UrlB" Target="_blank" runat="server" CommandName="SiteLink" Visible="True"><asp:Image runat="server" ID="SiteLinkImg" ImageUrl="~/Images/LinkDis.gif" BorderStyle="None" CssClass="ELTableComponents" /></asp:HyperLink>
                           </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
             </asp:GridView>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="CatList" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="GridViewPanel2" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
        <ContentTemplate>
            <div style="margin-left:auto;margin-right:auto;padding:10px">
             <asp:GridView ID="AuditGrid" runat="server" GridLines="Horizontal" CellSpacing="2" HorizontalAlign="Center" HeaderStyle-CssClass="HeaderTextTable" RowStyle-CssClass="RowText" OnRowDataBound="AuditGrid_RowDataBound"  AutoGenerateColumns="false">
                <Columns>
                    <asp:BoundField HeaderText="ID" DataField="eid" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField HeaderText="Event Date" DataField="ScheduledDate" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField HeaderText="Time" DataField="ScheduledTime" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField HeaderText="Start Date" DataField="StartDate" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField HeaderText="Time" DataField="StartTime" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField HeaderText="End Date" DataField="EndDate" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField HeaderText="Time" DataField="EndTime" ItemStyle-HorizontalAlign="Center"/>        
                    <asp:BoundField HeaderText="Category" DataField="categoryName" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField HeaderText="Event Name" DataField="name" HeaderStyle-HorizontalAlign="Center"/>
                    <asp:TemplateField HeaderText="Controls" HeaderStyle-Width="96">
                        <ItemTemplate>
                            <div style="text-align:left">
                           <asp:HyperLink ID="DocB" runat="server" Target="_blank" CommandName="Documentation" Visible="True"><asp:Image runat="server" ID="DocLinkImg" ImageUrl="~/Images/Doc1Dis.gif" CssClass="ELTableComponents" /></asp:HyperLink>
                           <asp:HyperLink ID="UrlB" Target="_blank" runat="server"  CommandName="SiteLink" Visible="True"><asp:Image runat="server" ID="SiteLinkImg" ImageUrl="~/Images/LinkDis.gif" CssClass="ELTableComponents" /></asp:HyperLink>
                           <asp:ImageButton ID='CommentB' TabIndex="-1" ImageUrl="~/Images/Comment.gif" runat="server" ToolTip="Comment" CommandName="Comment" CssClass="ELTableComponents" OnClientClick="InfoSetup(this); return false;" CausesValidation="False"/>   
                           <ajaxToolkit:ModalPopupExtender ID="CommentExtend" runat="server" PopupControlID='<%#ComPan.ClientID%>' TargetControlID="CommentB" BackgroundCssClass="ModelBack"></ajaxToolkit:ModalPopupExtender>
                           <asp:HiddenField ID="EIDH" runat="server"/>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="Forwarded" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:CheckBox ID="ForCheck" runat="server" AutoPostBack="false" Checked="false" Enabled="false"/>
                    </ItemTemplate>
                    </asp:TemplateField> 
                    <asp:TemplateField HeaderText="Active" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:CheckBox ID="ActiveCheck" runat="server" AutoPostBack="false" Checked='<%# bool.Parse( Eval("active").ToString()) %>' Enabled="false"/>
                    </ItemTemplate>
                    </asp:TemplateField>  
                    <asp:BoundField HeaderText="User" DataField="UserName" HeaderStyle-HorizontalAlign="Center"/>
                </Columns>
             </asp:GridView>
            </div>           
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="CatList" />
        </Triggers>
    </asp:UpdatePanel>
     <asp:Panel CssClass="PerPan" ID="ComPan" runat="server">
            <asp:UpdatePanel ID="ComData" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <div  class="ExtenderUpper">
                        <div style="float:right;padding-left:5px"><asp:ImageButton ID="CloseWinCom" OnClientClick="ShowHideCom_Click()" ImageUrl="~/Images/Close.jpg" Width="24" Height="16" ToolTip="Close Window" Text="Close" runat="server"/></div>
                        <div style="text-align:center;padding-left:30px"><asp:Label ID="TitleLabelCom" CssClass="PerPanTitle" Text="Comment History" runat="server" ></asp:Label></div>
                    </div>                   
                    <div style="min-height:65px">
                        <asp:Label ID="ComCount" runat="server" CssClass="ComPanLabel" ></asp:Label>  
                        <asp:UpdateProgress ID="LoadProgressPer" AssociatedUpdatePanelID="ComData" runat="server">
                            <ProgressTemplate>           
                                <asp:Image ID="LoadImgPer" CssClass="LoadingGif" ImageUrl="~/Images/Loading.gif" runat="server" />         
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </div>
                    <div style="margin-left:auto;margin-right:auto;min-width:600px">
                        <asp:HiddenField ID="CEIDH" runat="server" />
                        <asp:HiddenField ID="BackPanIdCom" runat="server" />
                        <asp:GridView ID="ComGrid" HeaderStyle-CssClass="PerGridHd" RowStyle-CssClass="PerGridRow" AlternatingRowStyle-CssClass="PerGridRowOff" Enabled="true" runat="server" AutoGenerateColumns="false" DataKeyNames="CID" >
                        <EditRowStyle CssClass="CommentRowEdit" />    
                        <Columns>
                            <asp:BoundField HeaderText="ID" DataField="CID" ReadOnly="true" />
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
                        </Columns>
                    </asp:GridView>
                    <asp:Button ID="GridDateButton" Text="Get Comments" ToolTip="Click to get comments" runat="server" OnClick="GridDateButton_Click" Visible="true"></asp:Button>     
                    </div>
                    <div class="ExtenderLower"></div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
</asp:content>
