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
<%@ page language="C#" autoeventwireup="true" masterpagefile="~/MasterPages/MasterBall.master" inherits="Views_Configuration, App_Web_j4ivqzes" maintainscrollpositiononpostback="true" %>

<asp:content ID="styles" ContentPlaceHolderID="masterCSS" runat="server">
    <link href="../StyleSheets/CalStyle.Css" type="text/css" rel="Stylesheet" />
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
    <div style="text-align:center;padding:10px;">
        <asp:Label ID="CatLabel" CssClass="AddEventText" Text="Category:" runat="server"/><asp:DropDownList CssClass="AddEventText" ID="CatList"  OnSelectedIndexChanged="List_Changed" AutoPostBack="true" runat="server"></asp:DropDownList>
        <asp:Label ID="SchLabel" CssClass="AddEventText" Text="Schedule:" runat="server"/><asp:DropDownList ID="SchedList" CssClass="AddEventText"  OnSelectedIndexChanged="List_Changed" AutoPostBack="true" runat="server"></asp:DropDownList>
        <asp:UpdateProgress ID="LoadProgressReport" AssociatedUpdatePanelID="GridViewPanel" runat="server">
            <ProgressTemplate>           
                <asp:Image ID="LoadImg" CssClass="LoadingGif" ImageUrl="~/Images/Loading.gif" runat="server" />         
            </ProgressTemplate>
        </asp:UpdateProgress>
    </div>
    <asp:UpdatePanel ID="GridViewPanel" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
        <ContentTemplate>
            <div style="margin-left:auto;margin-right:auto;padding:10px">
            <asp:GridView ID="ManageGrid" runat="server" GridLines="Horizontal" CellPadding="2" OnRowDataBound="ManageGrid_RowDataBound" HeaderStyle-CssClass="HeaderTextTable" RowStyle-CssClass="RowText" HorizontalAlign="Center" AutoGenerateColumns="false" >
                <Columns>
                    <asp:BoundField HeaderText="ID" DataField="EID" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField HeaderText="Category" DataField="categoryName" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField HeaderText="Event Name" DataField="name" ItemStyle-CssClass="RowText" />
                    <asp:BoundField HeaderText="Time" ItemStyle-Font-Bold="true" DataField="StartTime" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField HeaderText="Schedule" DataField="schedule" ItemStyle-HorizontalAlign="Center"/> 
                    <asp:TemplateField HeaderText="DayID" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label ID="SchedCodeL" runat="server" />
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Active" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:CheckBox ID="ActiveCheck" runat="server" Checked='<%# bool.Parse( Eval("active").ToString()) %>' Enabled="false"/>
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Modify" HeaderStyle-Width="160">
                        <ItemTemplate>
                            <div style="text-align:center">
                            <asp:HyperLink ID="View" TabIndex="-1" runat="server" Target="_blank" ToolTip="View Event" NavigateUrl= '<%#"~/Views/Event.aspx?" +  Eval("EID").ToString() + "&2" %>' CausesValidation="False" Visible="True"><asp:Image runat="server" ID="ViewI"  ImageUrl="~/Images/View.gif" CssClass="ELTableComponents" /></asp:HyperLink>
                            <asp:HyperLink ID="Edit" TabIndex="-1" runat="server" Target="_blank" ToolTip="Edit Event" NavigateUrl= '<%#"~/Views/Event.aspx?" +  Eval("EID").ToString() %>' CausesValidation="False" Visible="True"><asp:Image runat="server" ID="EditI" ImageUrl="~/Images/Edit.gif" CssClass="ELTableComponents" /></asp:HyperLink>
                            <asp:HyperLink ID="Copy" TabIndex="-1" runat="server" Target="_blank" ToolTip="Copy Event" NavigateUrl= '<%# "~/Views/Event.aspx?" + Eval("EID").ToString() + "&1" %>' CausesValidation="False"  Visible="True"><asp:Image runat="server" ID="CopyI" ImageUrl="~/Images/Copy.gif" CssClass="ELTableComponents" /></asp:HyperLink>
                            <asp:ImageButton ID='Status' runat="server" ToolTip="Disable" CommandName="Status" onClick="iButtonActive_Click" CausesValidation="False" ImageUrl="~/Images/Disable.gif"  CssClass="ELTableComponents"/>
                            <asp:ImageButton ID='Delete' runat="server" ToolTip="Delete Event" CommandName="Delete" onClick="iButtonDelete_Click" ImageUrl="~/Images/Garbage.gif"  CssClass="ELTableComponents"/>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
             </asp:GridView>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="CatList" />
            <asp:AsyncPostBackTrigger ControlID="SchedList" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:Panel CssClass="PerPan" ID="CatPan" runat="server">
            <asp:UpdatePanel ID="CatData" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <div class="ExtenderUpper">
                        <div></div>
                        <div style="float:right;padding-left:5px"><asp:ImageButton ID="CloseWinCom" CausesValidation="false" OnClientClick="ShowHideCat_Click()" ImageUrl="~/Images/Close.jpg" Width="24" Height="16" ToolTip="Close Window" Text="Close" runat="server"/></div>
                        <div style="text-align:center;padding-left:30px"><asp:Label ID="TitleLabelCom" CssClass="PerPanTitle" Text="Category Management" runat="server" ></asp:Label></div>
                    </div>                    
                    <div style="margin-left:auto;margin-right:auto;padding:25px">
                    <asp:GridView ID="CatGrid" HeaderStyle-CssClass="PerGridHd" RowStyle-CssClass="PerGridRow" AlternatingRowStyle-CssClass="PerGridRowOff" Enabled="true" runat="server"
                            AutoGenerateColumns="false" DataKeyNames="ECID" OnRowEditing="CatView_RowEditing" OnRowDeleting="CatGrid_RowDeleting" OnRowUpdating="CatView_RowUpdating" OnRowCancelingEdit="CatView_RowCancelingEdit">
                        <EditRowStyle CssClass="CommentRowEdit" />    
                        <Columns>
                            <asp:BoundField HeaderText="ID" DataField="ECID" ReadOnly="true" />
                            <asp:TemplateField HeaderText="Name" >
                            <ItemTemplate>
                                        <asp:TextBox ID="NameBox" MaxLength="254" CssClass="CatTextBox" runat="server" Text='<%# ( Eval("CategoryName").ToString()) %>' ReadOnly="true"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Color" >
                            <ItemTemplate>
                                        <asp:TextBox ID="ColorBox" MaxLength="7" CssClass="CatTextBoxCol" runat="server" Text='<%# ( Eval("CategoryColor").ToString()) %>' ReadOnly="true"></asp:TextBox>
                                        <ajaxToolkit:ColorPickerExtender ID="ColorExt" TargetControlID="ColorBox" SampleControlID="ColorBox" Enabled="false" runat="server" />    
                            </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Controls">
                                <ItemTemplate>
                                  <asp:LinkButton ID="CancelLB" Text="Cancel" Visible="false" CommandName="Cancel" CausesValidation="false" runat="server" />
                                  <asp:LinkButton ID="EditLB" Text="Edit" CommandName="Edit" CausesValidation="false" runat="server" />
                                  <asp:LinkButton ID="UpdateLB" Text="Update" CommandName="Update" Visible="false" CausesValidation="false" runat="server" />
                                  <asp:LinkButton ID="DeleteLB" Text="Delete" CommandName="Delete" runat="server" CausesValidation="false" />
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
</asp:content>