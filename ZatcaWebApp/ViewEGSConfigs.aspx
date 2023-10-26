<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewEGSConfigs.aspx.cs" Inherits="ZatcaWebApp.ViewEGSConfigs" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table>
        <tr>
             <td align="left" > 
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:Button ID="btnExport" runat="server" CssClass="export" OnClick="btnExport_Click" Text="EXPORT" Height="30" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnExport" />

                    </Triggers>
                </asp:UpdatePanel>
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td>
                 <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="GridView1" runat="server"  class="table table-bordered table-condensed table-responsive table-hover " BorderColor="Transparent"   AutoGenerateColumns="False"
                            AllowPaging="True" AllowSorting="True" Width="900px"    AutoGenerateEditButton="True" 
                            OnRowUpdated="GridView1_RowUpdated" OnRowCommand="GridView1_RowCommand"
                            DataSourceID="SqlDataSource1" PageSize="16" PagerSettings-Position="Top" DataKeyNames="Id">

                            <Columns>
                               <%-- <asp:BoundField HeaderText="Id" DataField="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />--%>
                              <%--  <asp:TemplateField>
                                    <ItemTemplate>
                                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <asp:LinkButton runat="server" ID="LinkButton1" ForeColor="Blue" Font-Bold="true"
                                            Text="Config" CommandName="downloadConfig"
                                            CommandArgument='<%# Eval("Id").ToString()%>'></asp:LinkButton>
                                               <asp:LinkButton runat="server" ID="LinkButton2" ForeColor="Blue" Font-Bold="true"
                                            Text="Properties" CommandName="downloadProperties"
                                            CommandArgument='<%# Eval("Id").ToString()%>'></asp:LinkButton>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:PostBackTrigger ControlID="LinkButton1" />
                                        <asp:PostBackTrigger ControlID="LinkButton2" />

                                    </Triggers>
                                </asp:UpdatePanel>
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                                <asp:BoundField HeaderText="egs_name" DataField="egs_name" SortExpression="egs_name" />
                                <asp:BoundField HeaderText="X509SerialNumber" DataField="X509SerialNumber" SortExpression="X509SerialNumber" />
                                <asp:BoundField HeaderText="csr_common_name" DataField="csr_common_name" SortExpression="csr_common_name" />
                                <asp:BoundField HeaderText="csr_serial_number" DataField="csr_serial_number" SortExpression="csr_serial_number" />
                                <asp:BoundField HeaderText="csr_organization_identifier" DataField="csr_organization_identifier" SortExpression="csr_organization_identifier" /> 
                                <asp:BoundField DataField="csr_organization_unit_name" HeaderText="csr_organization_unit_name" SortExpression="csr_organization_unit_name" />
                                <asp:BoundField DataField="csr_organization_name" HeaderText="csr_organization_name" SortExpression="csr_organization_name" />
                                <asp:BoundField DataField="csr_country_name" HeaderText="csr_country_name" SortExpression="csr_country_name" />
                                <asp:BoundField DataField="csr_invoice_type" HeaderText="csr_invoice_type" SortExpression="csr_invoice_type" />
                                <asp:BoundField DataField="csr_location_address" HeaderText="csr_location_address" SortExpression="csr_location_address" />
                                <asp:BoundField DataField="csr_industry_business_category" HeaderText="csr_industry_business_category" SortExpression="csr_industry_business_category" />
                                <asp:BoundField DataField="seller_identification_schemeID" HeaderText="seller_identification_schemeID" SortExpression="seller_identification_schemeID" />
                                <asp:BoundField DataField="seller_identification_ID" HeaderText="seller_identification_ID" SortExpression="seller_identification_ID" />
                                <asp:BoundField DataField="streetName" HeaderText="streetName" SortExpression="streetName" />
                                <asp:BoundField DataField="buildingNumber" HeaderText="buildingNumber" SortExpression="buildingNumber" />
                                <asp:BoundField DataField="PlotIdentification" HeaderText="PlotIdentification" SortExpression="PlotIdentification" />
                                <asp:BoundField DataField="citySubdivisionName" HeaderText="citySubdivisionName" SortExpression="citySubdivisionName" />
                                <asp:BoundField DataField="cityName" HeaderText="cityName" SortExpression="cityName" />
                                <asp:BoundField DataField="postalZone" HeaderText="postalZone" SortExpression="postalZone" />
                                <asp:BoundField DataField="countryIdentificationCode" HeaderText="countryIdentificationCode" SortExpression="countryIdentificationCode" />
                                <asp:BoundField DataField="partyTaxSchemeCompanyId" HeaderText="partyTaxSchemeCompanyId" SortExpression="partyTaxSchemeCompanyId" />
                                <asp:BoundField DataField="partyLegalEntityRegistrationName" HeaderText="partyLegalEntityRegistrationName" SortExpression="partyLegalEntityRegistrationName" />



                            </Columns>

                           <RowStyle CssClass="eachRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <HeaderStyle CssClass="HeaderRow" />  
                            <PagerSettings Position="Top" />
                            <PagerStyle CssClass="PagerRow" BorderColor="White" Height="20px" />

                            <SortedAscendingCellStyle />
                            <SortedAscendingHeaderStyle />
                            <SortedDescendingCellStyle />
                            <SortedDescendingHeaderStyle />
                        </asp:GridView>
                        <br />
                      
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="GridView1" />

                    </Triggers>
                </asp:UpdatePanel>
                  <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConStr %>" 
                      SelectCommand="SELECT * FROM [EGSconfig] ORDER BY [Id] DESC"
                      
                      UpdateCommand="UPDATE EGSconfig SET 
      egs_name=@egs_name
      ,csr_common_name=@csr_common_name
      ,csr_serial_number=@csr_serial_number
      ,csr_organization_identifier=@csr_organization_identifier
      ,csr_organization_unit_name=@csr_organization_unit_name
      ,csr_organization_name=@csr_organization_name
      ,csr_country_name=@csr_country_name
      ,csr_invoice_type=@csr_invoice_type
      ,csr_location_address=@csr_location_address
      ,csr_industry_business_category=@csr_industry_business_category
      ,seller_identification_schemeID=@seller_identification_schemeID
      ,seller_identification_ID=@seller_identification_ID
      ,streetName=@streetName
      ,buildingNumber=@buildingNumber
      ,PlotIdentification=@PlotIdentification
      ,citySubdivisionName=@citySubdivisionName
      ,cityName=@cityName
      ,postalZone=@postalZone
      ,countryIdentificationCode=@countryIdentificationCode
      ,partyTaxSchemeCompanyId=@partyTaxSchemeCompanyId
      ,partyLegalEntityRegistrationName=@partyLegalEntityRegistrationName  
                      where Id=@Id
"
                     
                      >
                           <UpdateParameters>
                        <asp:Parameter Name="egs_name" />
      <asp:Parameter Name="csr_common_name" />
      <asp:Parameter Name="csr_serial_number" />
      <asp:Parameter Name="csr_organization_identifier" />
      <asp:Parameter Name="csr_organization_unit_name" />
      <asp:Parameter Name="csr_organization_name" />
      <asp:Parameter Name="csr_country_name" />
      <asp:Parameter Name="csr_invoice_type" />
      <asp:Parameter Name="csr_location_address" />
      <asp:Parameter Name="csr_industry_business_category" />
      <asp:Parameter Name="seller_identification_schemeID" />
      <asp:Parameter Name="seller_identification_ID" />
      <asp:Parameter Name="streetName" />
      <asp:Parameter Name="buildingNumber" />
      <asp:Parameter Name="PlotIdentification" />
      <asp:Parameter Name="citySubdivisionName" />
      <asp:Parameter Name="cityName" />
      <asp:Parameter Name="postalZone" />
      <asp:Parameter Name="countryIdentificationCode" />
      <asp:Parameter Name="partyTaxSchemeCompanyId" />
      <asp:Parameter Name="partyLegalEntityRegistrationName" />
                               <asp:Parameter Name="Id" />
                    </UpdateParameters>
                  </asp:SqlDataSource>
            </td>
        </tr>
    </table>

     <%--this is required to work error/warning notification toaster.
             not requred when the page has requiredbvalidators.
             you can enable it in all pages individually or add in site.master, first option os preferred--%>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="valdssdd" />
</asp:Content>
