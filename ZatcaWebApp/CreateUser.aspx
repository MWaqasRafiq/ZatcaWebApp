<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateUser.aspx.cs" Inherits="ZatcaWebApp.CreateUser" %>
 
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 
 

    <script type="text/javascript">

 
    function view_hide(src,id) {
        if (src.includes("view.png")) {
          
                if (id == 'Image4') {
                    document.getElementById('<%= password1.ClientID %>').type = 'SingleLine';
                    document.getElementById('Image4').src = 'Images/hide.png';
                }
            else if(id == 'Image5') 
                
                {
                    document.getElementById('<%= repeat_pw1.ClientID %>').type = 'SingleLine';
                    document.getElementById('Image5').src = 'Images/hide.png';
                }  
               
            }
            else {
                if (id == 'Image4') {
                    document.getElementById('<%= password1.ClientID %>').type = 'Password';
                     document.getElementById('Image4').src = 'Images/view.png';
                }
                else if (id == 'Image5') {
                    document.getElementById('<%= repeat_pw1.ClientID %>').type = 'Password';
                     document.getElementById('Image5').src = 'Images/view.png';
                }
             
            }
        } 
    </script>
    <style>
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
            height: 373px;
           
        }
         .modalPopup
         tr td{
             padding-left:10px;
         }

        .lastColumn {
            width: 100px !important;
            text-align: left;
        }

        .auto-style4 {
            width: 142px;
        }

   

        .auto-style6 {
            margin-left: 455px;
            margin-top: -10px;
            padding-top: -20px;
            /*background-image:url(/images/close.png);*/
            background-color: red;
            color:#ffffcc;
        }
    </style>
    <br />
    <br />
    <br />
    <br />
    <table>
        <tr>
            <td align="right">

                <asp:LinkButton Font-Underline="false" runat="server" ID="lnk_newUser" Font-Size="10" ForeColor="#333333" Width="90px">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/New_user_icon.png" Width="20" Style="padding-bottom: 3px" />
                    New User
                </asp:LinkButton>
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <div class="rounded-corners">
                    <asp:GridView ID="GridView1" runat="server"  class="table table-bordered table-condensed table-responsive table-hover " BorderColor="Transparent"  ForeColor="#333333" AutoGenerateColumns="False" OnRowCommand="gvReport_RowCommand"
                        DataSourceID="SqlDataSource1" AllowPaging="True" AllowSorting="True" Width="1000px" GridLines="None"  
                        PageSize="16" PagerSettings-Position="Top">
                        <Columns>

                            <asp:BoundField DataField="FirstName" HeaderText="First Name" SortExpression="FirstName" ItemStyle-Width="200px" />
                            <asp:BoundField DataField="UserName" HeaderText="User Name" SortExpression="UserName" ItemStyle-Width="200px" />
                            <asp:BoundField DataField="role" HeaderText="Role" ReadOnly="True" SortExpression="role" ItemStyle-Width="100px" />

                          
                            <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" ReadOnly="True" ItemStyle-Width="100px" />
                            <asp:TemplateField HeaderText="Change Status" ItemStyle-Width="100px">
                                <ItemTemplate> 
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <asp:LinkButton runat="server" ID="link_ChangeStatus" CausesValidation="false" ForeColor="Black" Font-Underline="false" CommandArgument='<%#Eval("Id")%>'
                                                CommandName='<%# IsStatusActive(Eval("Status").ToString()) %>'> <%# IsStatusActive(Eval("Status").ToString()) %></asp:LinkButton>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:PostBackTrigger ControlID="link_ChangeStatus" />
                                        </Triggers>
                                    </asp:UpdatePanel> 
                                </ItemTemplate>
                            </asp:TemplateField>
                              <asp:TemplateField HeaderText="Edit" ItemStyle-Width="100px">
                                <ItemTemplate> 

                                    <asp:LinkButton runat="server" ID="link_btn_Select" CommandArgument='<%#Eval("Id")+","+ Eval("FirstName")+","+ Eval("UserName")+","+ Eval("role")
                                            +","+Eval("roleCode")+","+Eval("Password")%>' ForeColor="#111111"
                                        CommandName="edit_user" CausesValidation="false">    <img alt="" id="Image_edit" src="Images/edit.png" style="width: 20px;" /> 
                                       
                                    </asp:LinkButton>

                                </ItemTemplate>

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
                </div>
            </td>
        </tr>
    </table>

    <asp:HiddenField ID="hdnField" runat="server" />
    <!-- Add User popup ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------>
 <br />
    <ajaxtoolkit:ModalPopupExtender ID="mpe_addUser" runat="server" BackgroundCssClass="modalBackground" OkControlID="OKbutton"
        PopupControlID="ModalPanel" PopupDragHandleControlID="ModalPanel" TargetControlID="lnk_newUser" />

     
        
        <table   id="ModalPanel" class="modalPopup" style="display:none" border="0">
            <tr>
                <td colspan="2" style="text-align:right;vertical-align:top">
                     <asp:Button ID="OKbutton" runat="server" BackColor="red"   Text="X" ForeColor="#ffffcc" />
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                   
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/New_user_icon.png" Width="20" Style="padding-bottom: 6px" />
                    <asp:Label runat="server" ID="Label11" Font-Size="Large" Text="Create User"></asp:Label>
                    <br />
                    <br />
                </td>

            </tr>
            <tr>
                <td class="auto-style4">
                    <asp:Label ID="l1" runat="server" Text="Name :"></asp:Label>
                </td>
                <td>
                    <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="name" runat="server"></asp:TextBox>

                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="name" ErrorMessage="Required *" ForeColor="Red" ValidationGroup="create"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="auto-style4">
                    <asp:Label ID="l2" runat="server" Text="User Name :"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="uname" runat="server" BorderStyle="Solid" CssClass="tb10"></asp:TextBox>

                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="uname" ErrorMessage="Required *" ForeColor="Red" ValidationGroup="create"></asp:RequiredFieldValidator>
                </td>
            </tr>

            <tr>
                <td class="auto-style4">
                    <asp:Label ID="l5" runat="server" Text="Role :"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList CssClass="tb10" BorderStyle="Solid" ID="role_ddl" runat="server">
                    </asp:DropDownList>

                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="role_ddl" ErrorMessage="Required *" ForeColor="Red" ValidationGroup="create"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="auto-style4">
                    <asp:Label ID="l3" runat="server" Text="Password :"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="password" runat="server" BorderStyle="Solid" CssClass="tb10" placeholder="****" TextMode="Password"> </asp:TextBox>

                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="password" ErrorMessage="Required *" ForeColor="Red" ValidationGroup="create"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>

                <td class="auto-style4">
                    <asp:Label ID="l4" runat="server" Text="Repeat Password :"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="repeat_pw" runat="server" BorderStyle="Solid" CssClass="tb10" placeholder="****" TextMode="Password"></asp:TextBox>

                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="repeat_pw" ErrorMessage="Required *" ForeColor="Red" ValidationGroup="create"></asp:RequiredFieldValidator>
                    <br />
                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="password" ControlToValidate="repeat_pw" ErrorMessage="Passwords do not match..!!" ForeColor="Red" ValidationGroup="create"></asp:CompareValidator>
                </td>
            </tr>
            <tr>

                <td colspan="3">&nbsp;</td>

            </tr>
            <tr>

                <td colspan="2" style="padding-left:0px" >
                       <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                           <ContentTemplate>
                               <asp:Button ID="create_btn" runat="server" CssClass="button" Text="Create" ValidationGroup="create" OnClick="create_btn_Click" style="width:100%" />
                           </ContentTemplate>
                           <Triggers>
                               <asp:PostBackTrigger ControlID="create_btn" />
                           </Triggers>
                       </asp:UpdatePanel>



                </td>

            </tr>

        </table>

     

    <!-- Add User popup -------------------------------------------------------------------end----------------------------------------------------------------------------------------------------------->

    <!-- Update User popup ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------>
    <asp:HiddenField ID="HiddenField1" runat="server" />
    <ajaxtoolkit:ModalPopupExtender ID="mpe_updateuser" runat="server" BackgroundCssClass="modalBackground" OkControlID="OKbutton1"
        PopupControlID="ModalPanel1" PopupDragHandleControlID="ModalPanel1" TargetControlID="HiddenField1" />

    
        
        <table  id="ModalPanel1"  class="modalPopup" style="display:none">
             <tr>
                <td colspan="2" style="text-align:right;vertical-align:top">
                     <asp:Button ID="OKbutton1" runat="server" BackColor="red"   Text="X" ForeColor="#ffffcc" />
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Image ID="Image3" runat="server" ImageUrl="~/Images/update_user.png" Width="25" Style="padding-bottom: 6px" />
                    <asp:Label runat="server" ID="Label1" Font-Size="Large" Text="Update User"></asp:Label>
                    <br />
                    <br />
                </td>

            </tr>
            <tr>
                <td class="auto-style4">
                    <asp:Label ID="Label2" runat="server" Text="Name :"></asp:Label>
                </td>
                <td>
                    <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="name1" runat="server"></asp:TextBox>

                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="name1" ErrorMessage="Required *" ValidationGroup="update" ForeColor="Red"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="auto-style4">
                    <asp:Label ID="Label3" runat="server" Text="User Name :"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="uname1" runat="server" BorderStyle="Solid" CssClass="tb10"></asp:TextBox>

                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="uname1" ErrorMessage="Required *" ValidationGroup="update" ForeColor="Red"></asp:RequiredFieldValidator>
                </td>
            </tr>

            <tr>
                <td class="auto-style4">
                    <asp:Label ID="Label4" runat="server" Text="Role :"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList CssClass="tb10" BorderStyle="Solid" ID="role_ddl1" runat="server">
                    </asp:DropDownList>

                    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="role_ddl1" ErrorMessage="Required *" ValidationGroup="update" ForeColor="Red"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="auto-style4">
                    <asp:Label ID="Label5" runat="server" Text="Password :"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="password1" runat="server" BorderStyle="Solid" CssClass="tb10" placeholder="****" TextMode="Password" width="170px"   > </asp:TextBox>
            <img alt="" id="Image4" src="Images/view.png" style="width: 25px;height:27px; border:none;background-color:transparent;margin-bottom:2px" onclick="view_hide(this.src,this.id);" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="password1" ErrorMessage="Required *" ValidationGroup="update" ForeColor="Red"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>

                <td class="auto-style4">
                    <asp:Label ID="Label6" runat="server" Text="Repeat Password :"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="repeat_pw1" runat="server" BorderStyle="Solid" CssClass="tb10" placeholder="****" TextMode="Password"  width="170px"  ></asp:TextBox>
                     <img alt="" id="Image5" src="Images/view.png" style="width: 25px;height:27px; border:none;background-color:transparent;margin-bottom:2px" onclick="view_hide(this.src,this.id);" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="repeat_pw1" ErrorMessage="Required *" ForeColor="Red" ValidationGroup="update"></asp:RequiredFieldValidator>
                    <br />
                    <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToCompare="password1" ControlToValidate="repeat_pw1" ErrorMessage="Passwords do not match..!!" ForeColor="Red" ValidationGroup="update"></asp:CompareValidator>
                </td>
            </tr>
            <tr>

                <td colspan="3">&nbsp;</td>

            </tr>
            <tr>

                <td colspan="2"   style="padding-left:0px" >
                   <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                       <ContentTemplate>
                           <asp:Button ID="updateBtn" runat="server" CssClass="button" Text="Update" ValidationGroup="update" OnClick="update_Click" style="width:100%" />
                           <asp:Label runat="server" ID="lbl_id_update" Visible="false"></asp:Label>


                       </ContentTemplate>
                       <Triggers>
                           <asp:PostBackTrigger ControlID="updateBtn" />
                       </Triggers>
                   </asp:UpdatePanel>
                     
                </td>

            </tr>

        </table>



    <!-- Update User popup -------------------------------------------------------------------end----------------------------------------------------------------------------------------------------------->
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConStr %>" SelectCommand="  select U.Id, U.FirstName,U.UserName,
  U.role as[roleCode],
  (Select R.Description from [Z_Roles] R where R.Code=U.role)  as [role]  ,
				Password,
        case [Status] when 0 then 'Not Active' when 1 then 'Active' end as Status
         from [Z_Users] U order by Id desc"></asp:SqlDataSource>


</asp:Content>
