<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Onboard.aspx.cs" Inherits="ZatcaWebApp.Onboard" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Label runat="server" Text="CSR and Private key needs to be generated and loaded in the correct folder before doing the onboarding." 
       ForeColor="Brown" Font-Bold="true" Font-Size="16px" />

    <br /><br />
    <table id="main" style="width: auto; margin-left:60px;text-align:left">
      
         
          <tr  style="height:50px">
            <td style="width:40%">OTP :</td>
            <td>
                <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtOTP" runat="server"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtOTP" ErrorMessage=" Required" ForeColor="Red"></asp:RequiredFieldValidator>
              </td>

        </tr>
        <tr>
            <td style="width:40%">CSR Location:</td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtCSRLocation" runat="server" Width="420px"></asp:TextBox>
            </td>

        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtCSRLocation" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>

        </tr>
        <tr>
            <td>Private Key location:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtPrivKeyLocation" runat="server" Width="420px"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtPrivKeyLocation" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                <br />
            </td>

        </tr>
        <tr>
            <td>Compliance Invoice XML folder:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtComplInvLocation" runat="server" Width="420px"></asp:TextBox>
                <br />
            </td>

        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="txtComplInvLocation" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>

        </tr>
        <tr>
            <td>Certificate to be generated(Path):
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtCertLocation" runat="server" Width="420px"></asp:TextBox>
            </td>

        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="txtCertLocation" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                 
            </td>

        </tr>
      
        <tr>
            <td>&nbsp;</td>
            <td>
                &nbsp;</td>

        </tr>
        <tr>
            <td> Compliance CSID API:</td>
            <td>
                <br />
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtComplianceCSIDapi" runat="server" Width="420px"></asp:TextBox>
            </td>

        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="txtComplianceCSIDapi" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                 
            </td>

        </tr>
           <tr>
            <td>Compliance Invoice API:
            </td>
            <td>
                <br />
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtComplianceInvoiceAPI" runat="server" Width="420px"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="txtComplianceInvoiceAPI" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                 
            </td>

        </tr>
          <tr>
            <td>Production CSID API:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtProdCSIDapi" runat="server" Width="420px"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="txtProdCSIDapi" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                 
                <br />
            </td>

        </tr>
          <tr>
            <td>&nbsp;</td>
            <td>
                <asp:CheckBox ID="chk_loadauthtext" runat="server" Text="Load Auth Text from file"  Checked="false"/>
            </td>

        </tr>
        <tr>
            <td></td>
            <td   align="left">
               <br />
                   <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                         <asp:Button ID="btnOnboard" runat="server" CssClass="buttonYellow" OnClick="btnSubmit_Click" Text="Submit" Height="39px" />
 </ContentTemplate>
                                        <Triggers>
                                            <asp:PostBackTrigger ControlID="btnOnboard" />

                                        </Triggers>
                                    </asp:UpdatePanel>
            </td>

        </tr>

    </table>
        <%--this is required to work error/warning notification toaster.
             not requred when the page has requiredbvalidators.
             you can enable it in all pages individually or add in site.master, first option os preferred--%>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="valdssdd" />
    </asp:Content>
