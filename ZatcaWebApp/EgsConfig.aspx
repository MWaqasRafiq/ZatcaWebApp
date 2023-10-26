<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EgsConfig.aspx.cs" Inherits="ZatcaWebApp.EgsConfig" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <%-- <style type="text/css">
        .auto-style1 {
            width: 40%;
        }
        .auto-style2 {
            width: 34%;
        }
        .auto-style3 {
            width: 29%;
        }
        .auto-style4 {
            width: 295px;
        }
        .auto-style5 {
            width: 251px;
        }
        .auto-style6 {
            width: 68px;
        }
        .auto-style7 {
            width: 18%;
        }
    </style>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     
    <table id="main" style="    margin-top:-59px; text-align:left;height:550px">
      
          <tr  style="height:50px">
            <td class="auto-style7">EGS unit :</td>
            <td>
                <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtEGS" runat="server" placehoder="Store/server name"></asp:TextBox> 
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtEGS" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
              </td>

            <td class="auto-style6">
                &nbsp;</td>

            <td class="auto-style5">
               Seller Identification SchemeID :</td>

            <td class="auto-style4">
                <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtseller_identification_schemeID" runat="server"  ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator23" runat="server" ControlToValidate="txtseller_identification_schemeID" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
              </td>

        </tr>
        <tr>
            <td class="auto-style7">CSR Common Name:</td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtcsr_common_name" runat="server"  ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtcsr_common_name" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
           <br />  </td>

            <td class="auto-style6">
                &nbsp;</td>

            <td class="auto-style5">
               Seller Identification ID</td>

            <td class="auto-style4">
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtseller_identification_ID" runat="server"  ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator22" runat="server" ControlToValidate="txtseller_identification_ID" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
              </td>

        </tr>
    
        <tr>
            <td class="auto-style7">CSR Serial Number:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtcsr_serial_number" runat="server"  ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtcsr_serial_number" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
                <br />
            </td>

            <td class="auto-style6">
                &nbsp;</td>

            <td class="auto-style5">
               Street Name:</td>

             <td class="auto-style4">
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtstreetName" runat="server"  ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator21" runat="server" ControlToValidate="txtstreetName" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
              </td>

        </tr>
        <tr>
            <td class="auto-style7">CSR Organization Identifier:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtcsr_organization_identifier" runat="server" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtcsr_organization_identifier" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
                <br />
            </td>

            <td class="auto-style6">
                &nbsp;</td>

            <td class="auto-style5">
              Building Number:</td>

            <td class="auto-style4">
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtbuildingNumber" runat="server"  ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator20" runat="server" ControlToValidate="txtbuildingNumber" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
              </td>

        </tr>
  
        <tr>
            <td class="auto-style7">CSR Organization Unit Name:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtcsr_organization_unit_name" runat="server"  ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="txtcsr_organization_unit_name" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
            </td>

            <td class="auto-style6">
                &nbsp;</td>

            <td class="auto-style5">
              Plot Identification:</td>

           <td class="auto-style4">
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtPlotIdentification" runat="server"  ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator19" runat="server" ControlToValidate="txtPlotIdentification" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
              </td>

        </tr>
   
  
        <tr>
            <td class="auto-style7"> CSR Organization Name:</td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtcsr_organization_name" runat="server"  ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="txtcsr_organization_name" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
            </td>

            <td class="auto-style6">
                &nbsp;</td>

            <td class="auto-style5">
               City Sub-division Name:</td>

            <td class="auto-style4">
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtcitySubdivisionName" runat="server"  ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server" ControlToValidate="txtcitySubdivisionName" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
              </td>
        </tr>
        <tr>
            <td class="auto-style7">&nbsp;</td>
            <td>
                &nbsp;</td>

            <td class="auto-style6">
                &nbsp;</td>

            <td class="auto-style5">
                City Name:</td>

            <td class="auto-style4">
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtcityName" runat="server"  ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server" ControlToValidate="txtcityName" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
              </td>

        </tr>
           <tr>
            <td class="auto-style7">CSR Country Name:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtcsr_country_name" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="txtcsr_country_name" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
                <br />
                 
            </td>

            <td class="auto-style6">
                &nbsp;</td>

            <td class="auto-style5">
                Postal Zone:</td>

            <td class="auto-style4">
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtpostalZone" runat="server"  ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" ControlToValidate="txtpostalZone" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
               </td>

        </tr>
          <tr>
            <td class="auto-style7">CSR Invoice Type:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtcsr_invoice_type" runat="server" ></asp:TextBox>
                 
                <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="txtcsr_invoice_type" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
                <br />
            </td>

            <td class="auto-style6">
                &nbsp;</td>

            <td class="auto-style5">
                Country Identification Code:</td>
  <td class="auto-style4">
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtcountryIdentificationCode" runat="server"  ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" ControlToValidate="txtcountryIdentificationCode" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
              </td>

        </tr>
          <tr>
            <td class="auto-style7">CSR Location Address:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtcsr_location_address" runat="server" ></asp:TextBox>
                 
                <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="txtcsr_location_address" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
                <br />
            </td>

            <td class="auto-style6">
                &nbsp;</td>

            <td class="auto-style5">
               Party TaxScheme CompanyId:</td>

             <td class="auto-style4">
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtpartyTaxSchemeCompanyId" runat="server"  ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" ControlToValidate="txtpartyTaxSchemeCompanyId" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
              </td>

        </tr>
          <tr>
            <td class="auto-style7">CSR Industry Business Category:
            </td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtcsr_industry_business_category" runat="server" ></asp:TextBox>
                 
                <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ControlToValidate="txtcsr_industry_business_category" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
                <br />
            </td>

            <td class="auto-style6">
                &nbsp;</td>

            <td class="auto-style5">
               Party Legal Entity Registration Name:</td>

            <td class="auto-style4">
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtpartyLegalEntityRegistrationName" runat="server"  ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" ControlToValidate="txtpartyLegalEntityRegistrationName" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
              </td>

        </tr>

   
        <tr>
            <td class="auto-style7"></td>
            <td   align="left">
       
            </td>

            <td   align="left" class="auto-style6">
                        <br />
                   <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                         <asp:Button ID="btnSubmit" runat="server" CssClass="buttonYellow" OnClick="btnSubmit_Click" Text="Submit" Height="39px"  ValidationGroup="vv"/>
 </ContentTemplate>
                                        <Triggers>
                                            <asp:PostBackTrigger ControlID="btnSubmit" />

                                        </Triggers>
                                    </asp:UpdatePanel></td>

            <td   align="left" class="auto-style5">
                &nbsp;</td>

            <td   align="left" class="auto-style4">
                &nbsp;</td>

        </tr>

    </table>
        <%--this is required to work error/warning notification toaster.
             not requred when the page has requiredbvalidators.
             you can enable it in all pages individually or add in site.master, first option os preferred--%>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="valdssdd" />
    </asp:Content>