<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewErrorsWarnings.aspx.cs" Inherits="ZatcaWebApp.ViewErrorsWarnings" %>

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
            position:fixed;
            background-color: #FFFFFF;
            border-width: 3px;
            border-style: solid;
            border-color: black;
            padding-top: 10px;
            padding-left: 10px;
            width: 1330px;
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
    <table width="1200px" border="0">
        <tr>
            <td>
                   <table id="main0" style="width: 1200px !important;  " border="0">
        <tr>

            <td runat="server" id="tdFromdate" style="width: 21%">&nbsp;</td>
            <td runat="server" id="sdf" style="width: 21%">&nbsp;</td>
            <td runat="server" id="tdTosdfsdfdate" style="width: 21%">&nbsp;</td>
            <td runat="server" style="width: 21%">&nbsp;</td>
            <td runat="server" style="width: 21%">&nbsp;</td>
            <td runat="server" style="width: 21%">&nbsp;</td>
            <td runat="server" style="width: 21%">&nbsp;</td>
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
            <td runat="server">
                IRN :<br />
                <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtIRN" runat="server" Width="64px"></asp:TextBox>
            </td>
            <td runat="server">StoreNo:<br />
                 <asp:ListBox ID="StoreList" runat="server" SelectionMode="Multiple" CssClass="tb10" BorderStyle="Solid"  Width="92px"   ></asp:ListBox>
            </td>
            <td runat="server">Terminal No:<asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtTermNo" runat="server" Width="64px"></asp:TextBox>
            </td>
            <td runat="server">Trans No:<asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtTrxNo" runat="server" Width="78px"></asp:TextBox>
            </td>
            <td runat="server">Status:<br />
                <asp:DropDownList CssClass="tb10" BorderStyle="Solid" ID="status_ddl" runat="server"     Width="92px"    >
                </asp:DropDownList>
            </td>
            <td runat="server">Doc Type:<br />
                <asp:DropDownList CssClass="tb10" BorderStyle="Solid" ID="docType_ddl" runat="server"     Width="92px"    >
                    <asp:ListItem Text="ALL" Value="ALL"  />
                    <asp:ListItem Text="B2B" Value="B2B" />
                    <asp:ListItem Text="B2C" Value="B2C" />
                </asp:DropDownList>
                <br />
            </td>
              <td runat="server">Type:<br />
                <asp:DropDownList CssClass="tb10" BorderStyle="Solid" ID="DDLReportType" runat="server"     Width="122px"    >
                    <asp:ListItem Text="SUMMARY" Value="SUMMARY"  />
                    <asp:ListItem Text="DETAILED" Value="DETAILED" /> 
                </asp:DropDownList>
                <br />
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
            <td>&nbsp;</td>
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
        </tr> 

                      <tr>
            <td>
                <table runat="server" visible="false" id="table_info" width="1100" style="font-size:15px;font-weight:normal" border="0">
                    <tr>
                        <td  runat="server" align="left" >Total Errors: &nbsp;&nbsp;&nbsp;&nbsp;<span runat="server" id="sp_TotalErrors" style="color:#000000; "></span><br />
                            Total Warnings:<span runat="server" id="sp_TotalWarnings" style="color:#000000"></span>
                        </td> 
                       
                        <td  runat="server" align="left" >Current Errors:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span runat="server" id="sp_CurrentErrors" style="color:#000000"></span>
                            <br />
                            Current Warnings:<span runat="server" id="sp_currentwarnings" style="color:#000000"></span>
                        </td> 
                        <td  runat="server" colspan="2"  align="right"> 
                             <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                    <ContentTemplate>
                           <asp:CheckBox runat="server" ID="chk_show_errors" Text="Show Errors"   Checked="true" style="padding:15px;" AutoPostBack="true" OnCheckedChanged="chk_show_errors_CheckedChanged"/> 
                            <asp:CheckBox runat="server" ID="chk_show_warnings" Text="Show Warnings"  Checked="true" style="padding:15px;" AutoPostBack="true" OnCheckedChanged="chk_show_warnings_CheckedChanged"/>
                            <asp:CheckBox runat="server" ID="chk_show_info" Text="Show Info"  Checked="false" style="padding:15px;"  AutoPostBack="true" OnCheckedChanged="chk_show_info_CheckedChanged"/>
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="chk_show_errors" />
                        <asp:PostBackTrigger ControlID="chk_show_warnings" />
                        <asp:PostBackTrigger ControlID="chk_show_info" />

                    </Triggers>
                </asp:UpdatePanel>
                          
                        </td> 
                         
                         
                    </tr>
                </table>
                <br />
            </td>      
             </tr>
        <tr>
            <td align="center">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                         <asp:GridView ID="GridView1" class="table table-bordered table-condensed table-responsive table-hover " AllowSorting="true" BorderColor="Transparent"
            DataSourceID="ObjectDataSource1" AllowPaging="True" AutoGenerateColumns="False" OnRowCommand="GridView1_RowCommand"  PageSize="10" Width="1200px"
            runat="server"   PagerSettings-Position="Top"  >
                          
                              
                     

                    <RowStyle CssClass="eachRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <Columns>
                        <asp:BoundField DataField="Message" HeaderText="Message" SortExpression="Message" />
                        <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type" /> 
                            <asp:TemplateField HeaderText="Clearance[Latest]" SortExpression="LatestClearance">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnk_LatestClearance" Font-Bold="true" ForeColor="Black"   Font-Underline='<%# Eval("LatestClearance").ToString() == "0" ? false:true %>' Enabled='<%# Eval("LatestClearance").ToString() == "0" ? false:true %>' Text='<%# Eval("LatestClearance").ToString() %>' CommandName="lnk_viewLatestClearance" CommandArgument='<%# Eval("Message").ToString()+"<sep>"+ Eval("Type").ToString()+"<sep>"+ Eval("fkInvoices").ToString() %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>

                         <asp:TemplateField HeaderText="Reporting[Latest]" SortExpression="LatestReporting">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnk_LatestReporting" Font-Bold="true" ForeColor="Black"   Font-Underline='<%# Eval("LatestReporting").ToString() == "0" ? false:true %>' Enabled='<%# Eval("LatestReporting").ToString() == "0" ? false:true %>' Text='<%# Eval("LatestReporting").ToString() %>' CommandName="lnk_viewLatestReporting" CommandArgument='<%# Eval("Message").ToString()+"<sep>"+ Eval("Type").ToString()+"<sep>"+ Eval("fkInvoices").ToString() %>'></asp:LinkButton>
                            </ItemTemplate>
                      </asp:TemplateField>
                         <asp:TemplateField HeaderText="Clearance" SortExpression="Clearance">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnk_viewClearance" Font-Bold="true" ForeColor="Black"   Font-Underline='<%# Eval("Clearance").ToString() == "0" ? false:true %>' Enabled='<%# Eval("Clearance").ToString() == "0" ? false:true %>' Text='<%# Eval("Clearance").ToString() %>' CommandName="lnk_viewClearance" CommandArgument='<%# Eval("Message").ToString()+"<sep>"+ Eval("Type").ToString()+"<sep>"+ Eval("fkInvoices").ToString() %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>

                         <asp:TemplateField HeaderText="Reporting" SortExpression="Reporting">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnk_viewReporting" Font-Bold="true" ForeColor="Black"   Font-Underline='<%# Eval("Reporting").ToString() == "0" ? false:true %>' Enabled='<%# Eval("Reporting").ToString() == "0" ? false:true %>' Text='<%# Eval("Reporting").ToString() %>' CommandName="lnk_viewReporting" CommandArgument='<%# Eval("Message").ToString()+"<sep>"+ Eval("Type").ToString()+"<sep>"+ Eval("fkInvoices").ToString() %>'></asp:LinkButton>
                            </ItemTemplate>
                      </asp:TemplateField>
                        
                    </Columns>
                    <HeaderStyle CssClass="HeaderRow"    Font-Bold="false" />  

<PagerSettings Position="Top"></PagerSettings>

                    <PagerStyle CssClass="PagerRow" BorderColor="White" Height="20px" />

                    <SortedAscendingCellStyle />
                    <SortedAscendingHeaderStyle />
                    <SortedDescendingCellStyle />
                    <SortedDescendingHeaderStyle />
                </asp:GridView>

                   <asp:GridView ID="GridView2" runat="server"    AutoGenerateColumns="true" class="table table-bordered table-condensed table-responsive table-hover " BorderColor="Transparent"
                    AllowPaging="True" AllowSorting="True" Width="1200px"      DataSourceID="ObjectDataSource2"  
        RowStyle-HorizontalAlign="Left"         PageSize="16" PagerSettings-Position="Top"  >

                     

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
                        <asp:PostBackTrigger ControlID="GridView2" />

                    </Triggers>
                </asp:UpdatePanel>
              
            </td>
        </tr>
    </table>
        
            </td>
        </tr>
    </table>

        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                    <ContentTemplate>
      <asp:HiddenField ID="hdnField" runat="server" />
    <ajaxtoolkit:modalpopupextender id="mp_err" runat="server" backgroundcssclass="modalBackground" okcontrolid="OKbutton" popupcontrolid="ModalPanel" PopupDragHandleControlID="ModalPanel" targetcontrolid="hdnField" />
    <asp:Panel ID="ModalPanel" runat="server" CssClass="modalPopup"    >
        <table border="0" width="100%">
            <tr>
                <td align="center">
                    
         <asp:GridView ID="gverr" runat="server"  class="table table-bordered table-condensed table-responsive table-hover " BorderColor="Transparent" AutoGenerateColumns="true" OnRowCommand="gverr_RowCommand"
                    AllowPaging="True" AllowSorting="True" Width="700px"       DataSourceID="ObjectDataSource3" OnPageIndexChanged="gverr_PageIndexChanged"
             OnSorted="gverr_Sorted"
        RowStyle-HorizontalAlign="Right"> 

             <Columns>
                    <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnk_viewdetails" Text="Details" CommandName="view_Details" CommandArgument='<%# Eval("XMLFileName").ToString() %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
             </Columns>

                    <RowStyle CssClass="eachRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
             <HeaderStyle CssClass="HeaderRow"    Font-Bold="false" />    

                    <SortedAscendingCellStyle />
                    <SortedAscendingHeaderStyle />
                    <SortedDescendingCellStyle />
                    <SortedDescendingHeaderStyle />
                </asp:GridView>
                </td>
            </tr>
        </table>
        <br />
        
<asp:Button ID="OKbutton" Text="OK" runat="server" />
 </asp:Panel>
   </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="gverr" /> 

                    </Triggers>
                </asp:UpdatePanel>
        <%--this is required to work error/warning notification toaster.
             not requred when the page has requiredbvalidators.
             you can enable it in all pages individually or add in site.master, first option os preferred--%>
            <br />
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConStr %>" SelectCommand="SELECT [Id], [StoreNo], [StoreName], [StoreIP] FROM [Stores]"></asp:SqlDataSource>
            <br />
 
     <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="ODS_DATA" TypeName="ZatcaWebApp.ViewErrorsWarnings">
                <SelectParameters>
                    <asp:Parameter Name="table_required" Type="String" DefaultValue="DT1_STATIC" />
                </SelectParameters>
            </asp:ObjectDataSource>
       <asp:ObjectDataSource ID="ObjectDataSource2" runat="server" SelectMethod="ODS_DATA" TypeName="ZatcaWebApp.ViewErrorsWarnings">
                <SelectParameters>
                    <asp:Parameter Name="table_required" Type="String" DefaultValue="DT2_STATIC" />
                </SelectParameters>
            </asp:ObjectDataSource>
           <asp:ObjectDataSource ID="ObjectDataSource3" runat="server" SelectMethod="ODS_DATA" TypeName="ZatcaWebApp.ViewErrorsWarnings">
                <SelectParameters>
                    <asp:Parameter Name="table_required" Type="String" DefaultValue="DT3_STATIC" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="valdssdd" />
    </asp:Content>

