<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Roles.aspx.cs" Inherits="ZatcaWebApp.Roles" %>
 

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .modalBackground {
            background-color: Black;
            filter: alpha(opacity=90);
            opacity: 0.8;
        }

        .modalPopup {
            background-color: #FFFFFF;
            padding-top: 10px;
            padding-left: 10px;
            width: 500px;
            height: 473px;
        }

            .modalPopup
            tr td {
                padding-left: 10px;
            }

        .lastColumn {
            width: 100px !important;
            text-align: left;
        }
    </style>
    <script>
      function resetControls()
        {

           document.getElementById('<%= txt_name.ClientID %>').removeAttribute('readonly');
          
          document.getElementById('<%= txt_name.ClientID %>').value = "";  

          document.getElementById('<%= hdf_btnText.ClientID %>').value = "Add";
          document.getElementById('<%= btb_add.ClientID %>').value = "Add";
         
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">





    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConStr %>" SelectCommand="SELECT [Id], [PageId], [MainMenuId], [PageName] FROM [Z_PagesList] WHERE (([MainMenuId] IS NOT NULL) AND ([Id] &lt;&gt; @Id))">
        <SelectParameters>
            <asp:ControlParameter ControlID="lbl_homeId" Name="Id" PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>

    <br />
    <asp:Label ID="lbl_homeId" runat="server" Visible="False"></asp:Label>

    <br />
    <table style="width: 700px;">
        <tr>
            <td align="right">

                <asp:LinkButton Font-Underline="false" runat="server" ID="lnk_newRole" Font-Size="10" ForeColor="#333333" Width="90px">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/role_icon.png" Width="15" Style="padding-bottom: 3px" />
                    New Role
                </asp:LinkButton>
                <br />
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:GridView ID="GridView1" runat="server"  class="table table-bordered table-condensed table-responsive table-hover " BorderColor="Transparent"   
                    AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" GridLines="None" Width="500"
                    PageSize="16" PagerSettings-Position="Top"
                    OnRowCommand="GridView1_RowCommand" DataKeyNames="Id" DataSourceID="SqlDataSource2">
                    <Columns>
                        <%--  <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                        <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />--%>
                        <asp:BoundField DataField="Description" HeaderText="Role" SortExpression="Description" />

                        <asp:TemplateField HeaderText="View/Update">
                            <ItemTemplate>
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>

                                        <asp:LinkButton runat="server" CausesValidation="false" ID="link_btn_Select" CommandArgument='<%#Eval("AccessPageIds")+","+Eval("Id")+","+Eval("Code")+","+Eval("Description") %>'
                                            CommandName="edit_role" Font-Underline="false" ForeColor="Black">View and Update
                                            <img alt="" id="Image_edit" src="Images/edit.png" style="width: 20px;" />  
                                        </asp:LinkButton>
                                         

                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:PostBackTrigger ControlID="link_btn_Select" />
                                    </Triggers>
                                </asp:UpdatePanel>

                            </ItemTemplate>
                            <ItemStyle Width="300" />
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="AltRow" />
                    <EditRowStyle CssClass="EditRow" />
                    <FooterStyle CssClass="FooterRow" />
                    <HeaderStyle CssClass="HeaderRow" />
                    <PagerStyle CssClass="PagerRow" BorderColor="White" Height="20px" />
                    <RowStyle CssClass="eachRow" />
                    <SortedAscendingCellStyle />
                    <SortedAscendingHeaderStyle />
                    <SortedDescendingCellStyle />
                    <SortedDescendingHeaderStyle />

                </asp:GridView>
            </td>

        </tr>
    </table>

    <br />

    <br />
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ConStr %>" SelectCommand="SELECT * FROM [Z_Roles] where Code &lt;&gt;0 and Description &lt;&gt;'Admin'  order by Id desc"></asp:SqlDataSource>

    <br />

    <br />

    <ajaxtoolkit:ModalPopupExtender ID="mpe_role" runat="server" BackgroundCssClass="modalBackground" OkControlID="OKbutton"
        PopupControlID="ModalPanel" PopupDragHandleControlID="ModalPanel" TargetControlID="lnk_newRole" />


    <table id="ModalPanel" class="modalPopup" style="display:none">
        <tr style="height: 20px">
            <td colspan="2" style="text-align: right; vertical-align: top">
                <asp:Button ID="OKbutton" runat="server" BackColor="red" Text="X" ForeColor="#ffffcc" OnClientClick="resetControls();"  />
            </td>
        </tr>
        <tr style="height: 20px">

            <td align="center" colspan="2">

                <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/role_icon.png" Width="15" Style="padding-bottom: 6px" />
                <asp:Label runat="server" ID="label101" Font-Size="Large" Text="Role Details"></asp:Label>

            </td>

        </tr>
        <tr>
            <td style="width: 100px">
                <asp:Label ID="Label1" runat="server" Text="Role Name :"></asp:Label></td>
            <td>
                <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txt_name" runat="server" ValidationGroup="aa"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txt_name" ErrorMessage="Required *" ForeColor="Red" ValidationGroup="aa"></asp:RequiredFieldValidator>

            </td>

        </tr>
        <tr runat="server" visible="false">
            <td>
                <asp:Label ID="Label12" runat="server" Text="Role Code :" Visible="False"></asp:Label></td>
            <td>
                <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txt_roleCode" ReadOnly="true" runat="server" Visible="False"></asp:TextBox>
            </td>
        </tr>
        <tr>

            <td colspan="2" align="left">
                <asp:Label ID="Label13" runat="server" Font-Bold="false" Font-Size="Small" Font-Underline="false" Text="Select accessible pages:"></asp:Label><br />
                <br />
                <asp:CheckBoxList ID="CheckBoxList1" runat="server" DataSourceID="SqlDataSource1" DataTextField="PageName" DataValueField="Id" RepeatColumns="2" TextAlign="Right" >
                </asp:CheckBoxList></td>
        </tr>
        <tr>
            <td colspan="2" align="center" style="padding-left: 0px;vertical-align:bottom">

                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <asp:Button ID="btb_add" runat="server" CssClass="button" OnClick="btb_add_Click" Text="Add" Style="width: 100%" ValidationGroup="aa"/>
                       
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btb_add" />
                        
                    </Triggers>
                </asp:UpdatePanel>

            </td>
        </tr>


    </table>

    <asp:HiddenField id="hdf_btnText" runat="server" Value="Add" />

</asp:Content>


