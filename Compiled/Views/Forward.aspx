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
<%@ page language="C#" autoeventwireup="true" masterpagefile="~/MasterPages/MasterBall.master" inherits="Views_Forward, App_Web_j4ivqzes" maintainscrollpositiononpostback="true" %>

<asp:content ID="styles" ContentPlaceHolderID="masterCSS" runat="server">
<link href="../StyleSheets/EventListStyleSheet.css" type="text/css" rel="Stylesheet" />
<script type="text/javascript">
    //Sets up the performance event id in the Performance Panel
    function InfoSetup(button) {
        //Id of active button
        var activeButton = button.id;
        //Replace the name of the button with the name of the EID field
        var eidID = activeButton.replace("CommentB", "EIDH");
        //Replace the name of the button with the name of the SchedTime  field
        var SchedTime = activeButton.replace("CommentB", "SchedTimeH");
        //Get the hidden field value
        var EIDH = document.getElementById(eidID).value;
        //set the peid field
        document.getElementById('<%=CEIDH.ClientID%>').value = EIDH;
        //set the pertitle field
        document.getElementById('<%=ComEventTitle.ClientID%>').innerHTML = button.parentNode.parentNode.parentNode.cells[2].innerHTML;
        document.getElementById('<%=ComEventTitleH.ClientID%>').value = button.parentNode.parentNode.parentNode.cells[2].innerHTML;
        document.getElementById('<%=ComSchedTime.ClientID%>').value = document.getElementById(SchedTime).value;
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
</asp:content>
<asp:content ID="mainContent" ContentPlaceHolderID="masterMain" runat="server">
    <div>
    <asp:UpdatePanel ID="GridViewPanel" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
        <ContentTemplate>
            <div style="padding:10px">
                <asp:GridView ID="ManageGrid" runat="server" HeaderStyle-CssClass="HeaderTextTable" RowStyle-CssClass="RowText" GridLines="Horizontal" CellPadding="2" Font-Size="Small" OnRowDataBound="rowDataGridBind" AutoGenerateColumns="false" HorizontalAlign="Center">
                    <Columns>
                    <asp:BoundField HeaderText="Time" DataField="SchedTime" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField HeaderText="Category" DataField="categoryName" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField HeaderText="Event Name" DataField="name" />
                    <asp:BoundField HeaderText="Start Time" DataField="StartTime"/>
                    <asp:BoundField HeaderText="Start Date" DataField="StartDate"/>
                    <asp:TemplateField HeaderText="Confirm" ItemStyle-CssClass="TableCellTextC">
                        <ItemTemplate>
                            <asp:CheckBox ID="Confirmed" runat="server" AutoPostBack="true" Checked="false" Enabled="true" OnCheckedChanged="Confirmed_CheckedChanged"/>
                        </ItemTemplate>
                    </asp:TemplateField>                 
                    <asp:TemplateField HeaderText="Controls" HeaderStyle-Width="128">
                        <ItemTemplate>
                            <div style="text-align:center">
                            <asp:ImageButton ID='CommentB' TabIndex="-1" ImageUrl="~/Images/Comment.gif" runat="server" ToolTip="Comment" CommandName="Comment" CssClass="TableComponents" OnClientClick="InfoSetup(this); return false;" CausesValidation="False"/>   
                            <ajaxToolkit:ModalPopupExtender ID="CommentExtend" runat="server" PopupControlID='<%#ComPan.ClientID%>' TargetControlID="CommentB" BackgroundCssClass="ModelBack"></ajaxToolkit:ModalPopupExtender>
                            <asp:HiddenField ID="ELIDH" runat="server"/>
                            <asp:HiddenField ID="EIDH" runat="server"/>
                            <asp:HiddenField ID="SchedTimeH" runat="server"/>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
             </asp:GridView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
        <asp:Panel CssClass="PerPan" ID="ComPan" runat="server">
            <asp:UpdatePanel ID="ComData" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <div  class="ExtenderUpper">
                        <div style="float:right;padding-left:5px"><asp:ImageButton ID="CloseWinCom" OnClientClick="ShowHideCom_Click()" ImageUrl="~/Images/Close.jpg" Width="24" Height="16" ToolTip="Close Window" Text="Close" runat="server"/></div>
                        <div style="text-align:center;padding-left:30px"><asp:Label ID="TitleLabelCom" CssClass="PerPanTitle" Text="Comment History" runat="server" ></asp:Label></div>
                    </div>                   
                    <div style="min-height:95px">
                        <asp:Label ID="InfoLabelCom" Font-Size="Small" Text="Number of Occurances" runat="server" ></asp:Label>  
                        <asp:RadioButtonList ID="DateListCom" CssClass="RadioList" RepeatDirection="Horizontal" ToolTip="The number occurances to load" OnSelectedIndexChanged="DateListCom_SelectedIndexChanged" AutoPostBack="true" runat="server"><asp:ListItem Text="1" /><asp:ListItem Text="3" /><asp:ListItem Text="7" /><asp:ListItem Text="10" /></asp:RadioButtonList>
                        <asp:Label ID="ComEventTitle" CssClass="ComPanLabel" Enabled="true" runat="server" />
                        <asp:UpdateProgress ID="LoadProgressCom" AssociatedUpdatePanelID="ComData" runat="server">
                            <ProgressTemplate>           
                                <asp:Image ID="LoadImgCom" CssClass="LoadingGif" ImageUrl="~/Images/Loading.gif" runat="server" />         
                            </ProgressTemplate>
                        </asp:UpdateProgress>        
                    </div>
                    <div style="margin-left:auto;margin-right:auto;min-width:600px">
                        <asp:HiddenField ID="CEIDH" runat="server" />
                        <asp:HiddenField ID="PriorCountH" value="1" runat="server" />
                        <asp:HiddenField ID="BackPanIdCom" runat="server" />
                        <asp:HiddenField ID="ComEventTitleH" runat="server" />
                        <asp:HiddenField ID="ComSchedTime" runat="server" />
                    <asp:GridView ID="ComGrid" HeaderStyle-CssClass="PerGridHd" RowStyle-CssClass="PerGridRow" AlternatingRowStyle-CssClass="PerGridRowOff" Enabled="true" runat="server"
                            AutoGenerateColumns="false" OnRowEditing="ComView_RowEditing" DataKeyNames="CID" OnRowDataBound="ComGrid_RowDataBound"  OnRowUpdating="ComView_RowUpdating" OnRowCancelingEdit="ComView_RowCancelingEdit">
                        <EditRowStyle CssClass="CommentRowEdit" />    
                        <Columns>
                            <asp:BoundField HeaderText="CommentId" DataField="CID" ReadOnly="true" />
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
    </div>
</asp:content>
