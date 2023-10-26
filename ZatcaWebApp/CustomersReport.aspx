<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CustomersReport.aspx.cs" Inherits="ZatcaWebApp.CustomersReport" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.0.0/dist/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
    <script src="https://code.jquery.com/jquery-1.10.2.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/popper.js@1.12.9/dist/umd/popper.min.js" integrity="sha384-ApNbgh9B+Y1QKtv3Rn7W3mgPxhU9K/ScQsAP7hUibX39j7fakFPskvXusvfa0b4Q" crossorigin="anonymous"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.0.0/dist/js/bootstrap.min.js" integrity="sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl" crossorigin="anonymous"></script>
    <script src="../lib/device-uuid.js" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <br />
    <table style="margin-left: 40px; width: 800px !important" border="0">
        <tr>
            <td>
                <asp:TextBox CssClass="form-control" ID="TextBox1" runat="server" Width="320px" placeholder="Enter VAT Number or Name to search" AutoPostBack="true" CausesValidation="false" OnTextChanged="TextBox1_TextChanged"></asp:TextBox>
            </td>
        </tr>
     
        <tr>
            <td>
                <div style="text-align: -webkit-center; padding-top: 25px; border-color: transparent">
                    <asp:GridView ID="datagrid1" class="table table-bordered table-condensed table-responsive table-hover " AllowSorting="True" BorderColor="Transparent"
                        DataSourceID="SqlDataSource1" AllowPaging="True" AutoGenerateColumns="False"
                        runat="server" Width="1200px" EmptyDataText="There are no data records to display." DataKeyNames="Id">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <%-- <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" SortExpression="Id" InsertVisible="False" /> --%>
                            <asp:BoundField DataField="PartyIdentificationSchemeID" HeaderText="SchemeID" SortExpression="PartyIdentificationSchemeID" />
                            <asp:BoundField DataField="PartyIdentificationID" HeaderText="SchemeIDVal" SortExpression="PartyIdentificationID" />
                            <asp:BoundField DataField="StreetName" HeaderText="StreetName" SortExpression="StreetName" />
                            <asp:BoundField DataField="StreetNameArabic" HeaderText="StreetNameArb" SortExpression="StreetNameArabic" />
                            <asp:BoundField DataField="BuildingNumber" HeaderText="BuildingNo" SortExpression="BuildingNumber" />
                            <asp:BoundField DataField="PlotIdentification" HeaderText="PlotId" SortExpression="PlotIdentification" />
                            <asp:BoundField DataField="CitySubdivisionName" HeaderText="CitySubdivisionName" SortExpression="CitySubdivisionName" />
                            <asp:BoundField DataField="CitySubdivisionNameArabic" HeaderText="CitySubdivisionNameArb" SortExpression="CitySubdivisionNameArabic" />
                            <asp:BoundField DataField="CityName" HeaderText="CityName" SortExpression="CityName" />
                            <asp:BoundField DataField="CityNameArabic" HeaderText="CityNameArb" SortExpression="CityNameArabic" />
                            <asp:BoundField DataField="PostalZone" HeaderText="PostalZone" SortExpression="PostalZone" />
                            <asp:BoundField DataField="CountryIdentificationCode" HeaderText="Country" SortExpression="CountryIdentificationCode" />
                            <asp:BoundField DataField="TaxScheme" HeaderText="TaxScheme" SortExpression="TaxScheme" />
                            <asp:BoundField DataField="PartyTaxSchemeCompanyID" HeaderText="VATNo" SortExpression="PartyTaxSchemeCompanyID" />
                            <asp:BoundField DataField="RegistrationName" HeaderText="RegistrationName" SortExpression="RegistrationName" />
                            <asp:BoundField DataField="RegistrationNameArabic" HeaderText="RegistrationNameArb" SortExpression="RegistrationNameArabic" />
                            <asp:BoundField DataField="CreatedUser" HeaderText="CreatedUser" SortExpression="CreatedUser" />
                            <asp:BoundField DataField="CreatedDate" HeaderText="CreatedDate" SortExpression="CreatedDate" />
                            <asp:BoundField DataField="UpdatedDate" HeaderText="UpdatedDate" SortExpression="UpdatedDate" />
                        </Columns>
                        <HeaderStyle BackColor="#555555" Font-Bold="true" Font-Size="Larger" ForeColor="White" />
                        <RowStyle BackColor="#f5f5f5" />
                        <SelectedRowStyle BackColor="#669999" Font-Bold="true" ForeColor="White" />
                    </asp:GridView>
                </div>

            </td>
        </tr>
           <tr>
       <td align="center">
           <br />
           <asp:UpdatePanel ID="UpdatePanel1" runat="server">
               <ContentTemplate>
                   <asp:Button ID="btnExport" runat="server" CssClass="export" OnClick="btnExport_Click" Text="EXPORT" Height="30" />
               </ContentTemplate>
               <Triggers>
                   <asp:PostBackTrigger ControlID="btnExport" />

               </Triggers>
           </asp:UpdatePanel> 
       </td>
   </tr>
    </table>




    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConStrVATDB %>"
        SelectCommand="SELECT * FROM [Customers]
        where 
        ([PartyTaxSchemeCompanyID] LIKE '%' + @searchval + '%') OR ([RegistrationName] LIKE '%' + @searchval + '%') OR ([RegistrationNameArabic] LIKE '%' + @searchval + '%') 
        ORDER BY [CreatedDate] DESC">
        <SelectParameters>
            <asp:ControlParameter ControlID="TextBox1" Name="searchval" PropertyName="Text" Type="String" ConvertEmptyStringToNull="false" />
        </SelectParameters>
    </asp:SqlDataSource>

</asp:Content>

