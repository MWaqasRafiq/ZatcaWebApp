<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Change_pw.aspx.cs" Inherits="ZatcaWebApp.Change_pw" %>
 
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <p style="width: 100%">
        <table>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="Current Password"></asp:Label>
                    &nbsp;</td>
                <td>
                    <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="Current_pw" runat="server" TextMode="Password"></asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Current_pw" ErrorMessage="Required**" ForeColor="Red"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="New Password"></asp:Label>
                    &nbsp;</td>
                <td>
                    <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="new_pw" runat="server" TextMode="Password"></asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="new_pw" ErrorMessage="Required**" ForeColor="Red"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label3" runat="server" Text="Confirm Password"></asp:Label>
                    &nbsp;</td>
                <td>
                    <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="confirm_pw" runat="server" TextMode="Password"></asp:TextBox>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="confirm_pw" ErrorMessage="Required**" ForeColor="Red"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="new_pw" ControlToValidate="confirm_pw" ErrorMessage="Passwords do not match..!" ForeColor="Red"></asp:CompareValidator>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>



                            <asp:Button CssClass="button" ID="Button1" runat="server" Text="Submit" OnClientClick="return confirmationBox()" OnClick="Button1_Click" />

                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="Button1" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td></td>
            </tr>
            <tr>
                <td></td>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
    </p>

</asp:Content>


