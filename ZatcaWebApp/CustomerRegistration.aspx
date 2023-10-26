<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CustomerRegistration.aspx.cs" Inherits="ZatcaWebApp.CustomerRegistration" %>
 <%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .tbm tr td {
            padding-bottom:15px
        }
    </style>
 
    <table id="main" style="width: 800px; margin-left:60px;text-align:left" border="0" class="tbm">
      
           <tr>
            <td>Party Tax Scheme Company ID:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtPartyTaxSchemeCompanyID" runat="server" ></asp:TextBox>
                 <asp:Image src="images/info.png" alt="info" width="20" style="padding-top:13px" Tooltip="Customer VAT Number-15 digits" runat="server" />
                <br />
                <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtPartyTaxSchemeCompanyID" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>--%>
                 
                
            </td>

        </tr>
          <tr>
            <td  >Party Identification Scheme ID :</td>
            <td style="vertical-align:bottom">
                <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtPartyIdentificationSchemeID" runat="server"></asp:TextBox>
                <asp:Image src="images/info.png" alt="info" width="20" style="padding-top:13px" Tooltip="Eg:CRN,MOM,MLS,700,SAG,OTH" runat="server" />
                <br />
                <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtPartyIdentificationSchemeID" ErrorMessage=" Required" ForeColor="Red"></asp:RequiredFieldValidator>--%>
              </td>

        </tr>
        <tr>
            <td  >Party Identification ID:</td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtPartyIdentificationID" runat="server" ></asp:TextBox>
                 <asp:Image src="images/info.png" alt="info" width="20" style="padding-top:13px" Tooltip="Alphanumeric Party Identification Scheme value" runat="server" />
                <br />
                   <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPartyIdentificationID" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>--%>
         
            </td>

        </tr>
       
        <tr>
            <td>Street Name:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtStreetName" runat="server" PlaceHolder="Street name English" ></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtStreetName" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                <br />
            </td>
             <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtStreetNameArabic" runat="server"  PlaceHolder="Street name Arabic"  ></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ControlToValidate="txtStreetNameArabic" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                <br />
            </td>

        </tr>
        <tr>
            <td>Building Number:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtBuildingNumber" runat="server" ></asp:TextBox>
                 <asp:Image src="images/info.png" alt="info" width="20" style="padding-top:13px" Tooltip="Four digits Number" runat="server" />
                <br />
                  <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="txtBuildingNumber" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
            
            </td>

        </tr>
        
        <tr>
            <td>Plot Identification:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtPlotIdentification" runat="server" ></asp:TextBox>
                  <asp:Image src="images/info.png" alt="info" width="20" style="padding-top:13px" Tooltip="Four digits Seller Address Additional Number" runat="server" />
                <br />
                 <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="txtPlotIdentification" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
               
            </td>

        </tr>
      
      
         
        <tr>
            <td>City Subdivision Name:</td>
            <td>
                <br />
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtCitySubdivisionName" runat="server" ></asp:TextBox>
                   <asp:Image src="images/info.png" alt="info" width="20" style="padding-top:13px" Tooltip="Address - District" runat="server" />
            </td>
              <td>
                <br />
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtCitySubdivisionNameArabic" runat="server" ></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="txtCitySubdivisionName" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                 
            </td>
             <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" ControlToValidate="txtCitySubdivisionNameArabic" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                 
            </td>

        </tr>
           <tr>
            <td>City Name:
            </td>
            <td>
                <br />
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtCityName" runat="server" PlaceHolder="City name English"  ></asp:TextBox>
                  <asp:Image src="images/info.png" alt="info" width="20" style="padding-top:13px" Tooltip="Eg: Riyadh" runat="server" />
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="txtCityName" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                 
            </td>
                 <td>
                <br />
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtCityNameArabic" runat="server" PlaceHolder="City name Arabic" ></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" ControlToValidate="txtCityNameArabic" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                 
            </td>

        </tr>
          <tr>
            <td>Postal Zone:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtPostalZone" runat="server" ></asp:TextBox>
                 <asp:Image src="images/info.png" alt="info" width="20" style="padding-top:13px" Tooltip="5 Digits Number" runat="server" />
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="txtPostalZone" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                 
                <br />
            </td>

        </tr> 
         <tr>
            <td>Country Identification Code:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtCountryIdentificationCode" runat="server"  Text="SA"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtCountryIdentificationCode" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                 
                <br />
            </td>

        </tr>
         <tr>
            <td>Tax Scheme:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtTaxScheme" runat="server" Text="VAT" ></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtTaxScheme" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                 
                <br />
            </td>

        </tr>
       
            <tr>
            <td>Registration Name:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtRegistrationName" runat="server"  PlaceHolder="Registration Name English" width="300" ></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" ControlToValidate="txtRegistrationName" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                 
                <br />
            </td>
                 <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtRegistrationNameArabic" runat="server" PlaceHolder="Registration Name Arabic"   width="300"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" ControlToValidate="txtRegistrationNameArabic" ErrorMessage="Required" ForeColor="Red"></asp:RequiredFieldValidator>
                 
                <br />
            </td>
        </tr>
        <tr>
            <td></td>
            <td   align="left">
               <br />
                   <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                         <asp:Button ID="btnSubmit" runat="server" CssClass="buttonYellow" OnClick="btnSubmit_Click" Text="Submit" Height="39px" />
 </ContentTemplate>
                                        <Triggers>
                                            <asp:PostBackTrigger ControlID="btnSubmit" />

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

