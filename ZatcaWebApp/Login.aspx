<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ZatcaWebApp.Login" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" >

    <br />
    <br />
    <br />
    <table style="width:500px !important;margin-left:300px" >
       
         
       
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" Text="User code" Font-Size="Large"></asp:Label>
            </td>
            <td>
                <asp:TextBox CssClass="tb10" BorderStyle="Solid"  ID="txtUserName" runat="server"  Font-Size="Large"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtUserName" ErrorMessage="This field is required!" ValidationGroup="val">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label2" runat="server" Text="Password" Font-Size="Large"></asp:Label>
            </td>
            <td>
                <asp:TextBox CssClass="tb10" BorderStyle="Solid"  ID="txtPassword" runat="server"  TextMode="Password" Font-Size="Large"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPassword" ErrorMessage="This field is required!" ValidationGroup="val">*</asp:RequiredFieldValidator>
                <br /> 
            </td>
        </tr>
        <tr>
            <td ></td>
            <td>
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="val" />
            </td>
        </tr>
        <tr>
            <td ></td>
            <td >

               
                
<asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                  <asp:Button CssClass="buttonYellow" ID="btnSubmit" runat="server"    OnClick="btnSubmit_Click" Text="Login" ValidationGroup="val"   />
        
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnSubmit" />
                
                    </Triggers>
                </asp:UpdatePanel>

            </td>
        </tr>
        
    </table>
         
<div style="height:267px;">

</div>

        <%--this is required to work error/warning notification toaster.
             not requred when the page has requiredbvalidators.
             you can enable it in all pages individually or add in site.master, first option os preferred--%>
            <asp:ValidationSummary ID="ValidationSummary2" runat="server" ValidationGroup="valdssdd" />
</asp:Content>
