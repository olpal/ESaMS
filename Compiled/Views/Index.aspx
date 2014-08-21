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
<%@ page language="C#" autoeventwireup="true" masterpagefile="~/MasterPages/MasterBall.master" inherits="Views_Default, App_Web_j4ivqzes" maintainscrollpositiononpostback="true" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
<asp:content ID="mainContent" ContentPlaceHolderID="masterMain" runat="server">
    <script type="text/JavaScript" language="JavaScript">
    //method to show/hide comment window
    function ShowHideCat_Click() {
        //get the panel
        var pan = document.getElementById('<%=CatPan.ClientID%>');
        //if the panel is hidden //Show the panel
        if (pan.style.visibility == 'hidden' || pan.style.visibility == '') { pan.style.visibility = 'visible'; }
            //Hide the panel
        else { pan.style.visibility = 'hidden'; }
        //get the panel
        var backPan = document.getElementById("CatPop_backgroundElement")
        //if backPan exists
        if (backPan != null) {
            //if the panel is hidden //Show the panel
            if (backPan.style.visibility == 'hidden' || backPan.style.visibility == '') { backPan.style.visibility = 'visible'; }
                //Hide the panel
            else { backPan.style.visibility = 'hidden'; }
        }
    }
</script>
        <table>
        <tr>
            <td></td>
            <td>
            <div style="padding:10px">
                <asp:HyperLink CssClass="MainButtons" ID="LisLink" ToolTip="Current Shift Event List" NavigateUrl="~/Views/EventList.aspx" runat="server"><asp:Image ID="EListImg" BorderStyle="None" ImageUrl="~/Images/EventList.jpg" height="128" Width="128" runat="server"/></asp:HyperLink>
                <asp:HyperLink CssClass="MainButtons" ID="ConLink" ToolTip="Event Management Page" NavigateUrl="~/Views/Management.aspx" runat="server"><asp:Image ID="EMListImg" BorderStyle="None" ImageUrl="~/Images/EventManage.jpg" height="128" Width="128" runat="server"/></asp:HyperLink>
                <asp:HyperLink CssClass="MainButtons" ID="EveLink" ToolTip="Event Creation Page" NavigateUrl="~/Views/Event.aspx" runat="server"><asp:Image ID="EAListImg" BorderStyle="None" ImageUrl="~/Images/EventMod.jpg" height="128" Width="128" runat="server"/></asp:HyperLink>
                <asp:HyperLink CssClass="MainButtons" ID="RepLink" ToolTip="Event Reporting Page" NavigateUrl="~/Views/Reporting.aspx" runat="server"><asp:Image ID="ERListImg" BorderStyle="None" ImageUrl="~/Images/EventReport.jpg" height="128" Width="128" runat="server"/></asp:HyperLink>
                <asp:HyperLink CssClass="MainButtons" ID="DbLink" ToolTip="Database Viewing Page" NavigateUrl="~/Views/DatabaseView.aspx" runat="server"><asp:Image ID="DbListImg" BorderStyle="None" ImageUrl="~/Images/EventDatabase.jpg" height="128" Width="128" runat="server"/></asp:HyperLink>
                </div>
             </td>
            <td></td>
        </tr>
     </table>
    <asp:Panel CssClass="PerPan" ID="CatPan" runat="server">
            <asp:UpdatePanel ID="CatData" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <div class="ExtenderUpper">
                        <div></div>
                        <div style="float:right;padding-left:5px"><asp:ImageButton ID="CloseWinCom" CausesValidation="false" OnClientClick="ShowHideCat_Click()" ImageUrl="~/Images/Close.jpg" Width="24" Height="16" ToolTip="Close Window" Text="Close" runat="server"/></div>
                        <div style="text-align:center;padding-left:30px"><asp:Label ID="TitleLabelCom" CssClass="PerPanTitle" Text="License" runat="server" ></asp:Label></div>
                    </div>                    
                    <div style="padding-left:15px;padding-right:15px;padding-top:15px;text-align:left">
                        <asp:Label ID="LicenseText" runat="server" 
                            Text="The Event Scheduling and Management System (ESaMS) is designed</br>to manage, schedule, display and track a repository of events.</br>Copyright (C) 2014 AJ</br></br>This program is free software: you can redistribute it and/or modify it</br>under the terms of the GNU General Public License as published by</br>the Free Software Foundation, version 3.</br></br>This program is distributed in the hope that it will be useful,</br>but WITHOUT ANY WARRANTY; without even the implied warranty of</br>MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.</br>See the GNU General Public License for more details. </br></br>You should have received a copy of the GNU General Public License</br>along with this program.  If not, see http://www.gnu.org/licenses/" />
                    </div>
                    <div style="padding-left:15px"><asp:Label ID="VersionLabel" CssClass="StatusLower" runat="server" /></div>
                    <div class="ExtenderLower"></div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
</asp:content>

