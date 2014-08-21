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
<%@ page language="C#" autoeventwireup="true" masterpagefile="~/MasterPages/MasterBall.master" inherits="Views_Database, App_Web_j4ivqzes" maintainscrollpositiononpostback="true" %>
<asp:content ID="styles" ContentPlaceHolderID="masterCSS" runat="server">
    <link href="../StyleSheets/DbvStyleSheet.Css" type="text/css" rel="Stylesheet" />
</asp:content>
<asp:content ID="mainContent" ContentPlaceHolderID="masterMain" runat="server">
    <script type="text/JavaScript" language="JavaScript">
        //method called on page load
        function pageLoad() {
            //get the page manager
            var pageMan = Sys.WebForms.PageRequestManager.getInstance();
            //Add begin and end methods
            pageMan.add_beginRequest(beginLoad);
            //set the panel size
            setPanelSize();
        }
        //method called when updatepanel is begining to load
        function beginLoad(sender, args) {
            var progress = document.getElementById('<%=LoadProgressReport.ClientID%>');
            //Set css display to block
            progress.style.display = "block";
        }
    </script>
        <asp:UpdatePanel ID="CondPanUp" runat="server" ChildrenAsTriggers="true" UpdateMode="Always">
            <ContentTemplate>
                <div style="text-align:center;padding:10px;display:block">
                    <asp:Label ID="TabLabel" Text="Tables" CssClass="AddEventText" runat="server" />
                    <asp:DropDownList CssClass="AddEventText" ID="TabList"  OnSelectedIndexChanged="List_Changed" AutoPostBack="true" runat="server"></asp:DropDownList><asp:ImageButton ID="CondExtB" CssClass="ExtButton" ToolTip="Extend SQL Conidtions Panel" Text="Conditions" CausesValidation="false" runat="server" Width="20" Height="20" /><asp:ImageButton ID="OrdExt" CssClass="ExtButton" Text="Conditions" ToolTip="Extend Table Order By Panel" CausesValidation="false" Width="20" Height="20" runat="server" />     
                    <ajaxToolkit:CollapsiblePanelExtender ID="CondExtend" TargetControlID="Conditions" ExpandControlID="CondExtB"
                        CollapsedSize="0" CollapseControlID="CondExtB" CollapsedImage="~/Images/DropDown.gif" ExpandedImage="~/Images/DropUp.gif" ImageControlID="CondExtB" Collapsed="true" runat="server" />
                    <ajaxToolkit:CollapsiblePanelExtender ID="OrdExtend" TargetControlID="Order" ExpandControlID="OrdExt"
                        CollapsedSize="0" CollapseControlID="OrdExt" CollapsedImage="~/Images/DropDown.gif" ExpandedImage="~/Images/DropUp.gif" ImageControlID="OrdExt" Collapsed="true" runat="server" />
                </div>
                <asp:Panel ID="Conditions" CssClass="CondPan" runat="server">
                    <div>
                        <asp:Label ID="CondLabel" Text="SQL Search Conditions" CssClass="CondTitle" runat="server"/>
                        <asp:Panel ID="UpperConditions" runat="server"/>
                        <asp:Panel ID="LowerConditions" CssClass="CondPanLow" runat="server"/>
                    </div>
                </asp:Panel>
                <asp:Panel ID="Order" CssClass="OrdPan" runat="server">
                    <asp:Label ID="OrderText" Text="Table Order By" CssClass="CondTitle" runat="server"/>
                    <asp:Panel ID="OrderPan" runat="server"/>
                    <asp:Panel ID="OrderLower" CssClass="CondPanLow" runat="server"/>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="GridViewPanel" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
            <ContentTemplate>
                <div style="margin-left:auto;margin-right:auto;padding:10px;text-align:center">
                    <asp:UpdateProgress ID="LoadProgressReport" AssociatedUpdatePanelID="GridViewPanel" runat="server">
                        <ProgressTemplate>           
                            <asp:Image ID="LoadImg" CssClass="LoadingGif" ImageUrl="~/Images/Loading.gif" runat="server" />         
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <asp:GridView ID="ManageGrid" AllowPaging="true" PagerStyle-CssClass="PagingControl" PageSize="50" OnPageIndexChanging="ManageGrid_PageIndexChanging" runat="server" GridLines="Both" CellPadding="5" HeaderStyle-CssClass="HeaderTextTable" RowStyle-CssClass="RowText" HorizontalAlign="Center" AutoGenerateColumns="true" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
</asp:content>