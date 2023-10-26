<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BranchInvReport.aspx.cs" Inherits="ZatcaWebApp.BranchInvReport" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
 
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxtoolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 

     <style>
        .modalBackground {
            background-color: Black;
            filter: alpha(opacity=90);
            opacity: 0.8;
        }

        .modalPopup {
            background-color: #FFFFFF;
            border-width: 3px;
            border-style: solid;
            border-color: black;
            padding-top: 10px;
            padding-left: 10px;
            width: 730px;
        }
    </style>
    <script type="text/javascript">

     $(document).ready(function () {

         $("#<%=txtFrom.ClientID %>").dynDateTime({
                  showsTime: false,
                  ifFormat: "%d/%m/%Y",
                  daFormat: "%l;%M %p, %e %m, %Y",
                  align: "BR",
                  electric: false,
                  singleClick: false,
                  displayArea: ".siblings('.dtcDisplayArea')",
                  button: ".next()"
              });
              $("#<%=txtTo.ClientID %>").dynDateTime({
                showsTime: false,
                ifFormat: "%d/%m/%Y",
                daFormat: "%l;%M %p, %e %m, %Y",
                align: "BR",
                electric: false,
                singleClick: false,
                displayArea: ".siblings('.dtcDisplayArea')",
                button: ".next()"
              });


         $('[id*=<%= StoreList.ClientID %>]').multiselect({
             enableFiltering: true,
             filterPlaceholder: 'Search',
             enableCaseInsensitiveFiltering: true,
             includeSelectAllOption: true,
             dropRight: true,
             maxHeight: 250,
             buttonWidth: '150px'
         });
 
        });
 

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        if (prm != null) {
            prm.add_endRequest(function (sender, e) {
                if (sender._postBackSettings.panelsToUpdate != null) {
                     
                    $(function () { 
                        $("#<%=txtFrom.ClientID %>").dynDateTime({
                            showsTime: false,
                            ifFormat: "%d/%m/%Y",
                            daFormat: "%l;%M %p, %e %m, %Y",
                            align: "BR",
                            electric: false,
                            singleClick: false,
                            displayArea: ".siblings('.dtcDisplayArea')",
                            button: ".next()"
                        });
                        $("#<%=txtTo.ClientID %>").dynDateTime({
                            showsTime: false,
                            ifFormat: "%d/%m/%Y",
                            daFormat: "%l;%M %p, %e %m, %Y",
                            align: "BR",
                            electric: false,
                            singleClick: false,
                            displayArea: ".siblings('.dtcDisplayArea')",
                            button: ".next()"
                        });


                        $('[id*=<%= StoreList.ClientID %>]').multiselect({
                            enableFiltering: true,
                            filterPlaceholder: 'Search',
                            enableCaseInsensitiveFiltering: true,
                            includeSelectAllOption: true,
                            dropRight: true,
                            maxHeight: 250,
                            buttonWidth: '150px'
                        });
                    });
                }
            });
     };
    </script>
    <table   border="0">
        <tr>
            <td align="center">
                   <table id="main0" style="width: 750px !important;  " border="0">
        <tr>

            <td runat="server" id="tdFromdate" style="width: 21%">&nbsp;</td>
            <td runat="server" id="sdf" style="width: 21%">&nbsp;</td> 
            <td runat="server" style="width: 21%">&nbsp;</td>
            <td runat="server" style="width: 21%">&nbsp;</td>  
            <td style="width: 18px"></td>
            <td></td>
        </tr>
        <tr>

            <td runat="server">
                <asp:Label runat="server" Text="From(Date):" ID="lblFrom" />
                <br />
                <asp:TextBox CssClass="tb10" BorderStyle="Solid"  ID="txtFrom" placeholder="DD/MM/YYYY"  runat="server"  ></asp:TextBox>
            </td>
            <td runat="server">
                <asp:Label runat="server" Text="To(Date):" ID="lblTo" />
                <br />
                <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtTo" placeholder="DD/MM/YYYY" runat="server"></asp:TextBox>
            </td>
          
            <td runat="server">StoreNo:<br /> 
                   <asp:ListBox ID="StoreList" runat="server" SelectionMode="Multiple" CssClass="tb10" BorderStyle="Solid"  Width="92px"   ></asp:ListBox>
            </td>
            <td runat="server">Term No:<br />
                <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtTermNo" runat="server" Width="64px"></asp:TextBox>
            </td>
            
            
           
            <td style="vertical-align: bottom">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <asp:Button ID="btnSubmit" runat="server" CssClass="buttonYellow" OnClick="btnSubmit_Click" Text="SUBMIT" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnSubmit" />

                    </Triggers>
                </asp:UpdatePanel>

            </td>
            <td style="vertical-align: bottom">
                <%--   <asp:UpdatePanel ID="UpdatePanel311" runat="server">
                                        <ContentTemplate>
                                              <asp:Button ID="btnExport" runat="server" CssClass="export" OnClick="btnExport_Click" Text="EXPORT" Height="30" />
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:PostBackTrigger ControlID="btnExport" />

                                        </Triggers>
                                    </asp:UpdatePanel>--%>
              
            </td>
        </tr>
        <tr>

            <td>&nbsp;
                 </td>
            <td>&nbsp;&nbsp;
                </td> 
            <td>&nbsp;</td>
            <td>&nbsp;</td> 
            <td>&nbsp;</td>
            <td>&nbsp;</td> 
        </tr>

    </table>
            </td>
        </tr>
        <tr>
            <td>
                  <table border="0">

        <tr>
            <td align="center" > 
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
        </tr><tr>
            <td> 
                <br />
            </td>      
             </tr>
        <tr>
            <td align="center">

                   <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                            <asp:GridView ID="GridView1" runat="server" class="table table-bordered table-condensed table-responsive table-hover " BorderColor="Transparent"  AutoGenerateColumns="true"
                    AllowPaging="True" AllowSorting="True" Width="1100px"     OnRowCommand="GridView1_RowCommand"  
        RowStyle-HorizontalAlign="Left"            PageSize="16" PagerSettings-Position="Top" DataSourceID="ObjectDataSource1"  >

                     

                    <RowStyle CssClass="eachRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    
                    <HeaderStyle CssClass="HeaderRow"    Font-Bold="false" />  

<PagerSettings Position="Top"></PagerSettings>

                    <PagerStyle CssClass="PagerRow" BorderColor="White" Height="20px" />

                    <SortedAscendingCellStyle />
                    <SortedAscendingHeaderStyle />
                    <SortedDescendingCellStyle />
                    <SortedDescendingHeaderStyle />
                </asp:GridView>

                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="GridView1" />

                    </Triggers>
                </asp:UpdatePanel>

            
            </td>
        </tr>
    </table>
        
            </td>
        </tr>
    </table>
       
  
        <%--this is required to work error/warning notification toaster.
             not requred when the page has requiredbvalidators.
             you can enable it in all pages individually or add in site.master, first option os preferred--%>
            <br />
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConStr %>" SelectCommand="SELECT [Id], [StoreNo], [StoreName], [StoreIP] FROM [Stores]"></asp:SqlDataSource>
            <br />

     <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="ODS_DATA" TypeName="ZatcaWebApp.BranchInvReport">
                <SelectParameters>
                    <asp:Parameter Name="table_required" Type="String" DefaultValue="DT1_STATIC" />
                </SelectParameters>
            </asp:ObjectDataSource>
    
  
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="valdssdd" />
    </asp:Content>


