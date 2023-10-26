<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Viewinvoices.aspx.cs" Inherits="ZatcaWebApp.Viewinvoices" %>
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
         $('[id*=<%= listVAT.ClientID %>]').multiselect({
             enableFiltering: true,
             filterPlaceholder: 'Search',
             enableCaseInsensitiveFiltering: true,
             includeSelectAllOption: true,
             dropRight: true,
             maxHeight: 250,
             buttonWidth: '180px'
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
                        $('[id*=<%= listVAT.ClientID %>]').multiselect({
                            enableFiltering: true,
                            filterPlaceholder: 'Search',
                            enableCaseInsensitiveFiltering: true,
                            includeSelectAllOption: true,
                            dropRight: true,
                            maxHeight: 250,
                            buttonWidth: '180px'
                        });
                    });
                }
            });
     };
    </script>
    <table   border="0">
        <tr>
            <td align="center">
                   <table id="main0" style="width: 1315px !important;  " border="0">
        <tr>
            
            <td runat="server" style="width: 21%">&nbsp;</td>
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
                <asp:Label runat="server" Text="VAT No:" ID="Label1" />
                <br />
                   <asp:ListBox ID="listVAT" runat="server" SelectionMode="Multiple" CssClass="tb10" BorderStyle="Solid" Width="200"     ></asp:ListBox>
                
            </td>
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
            <td runat="server">Term No:<asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtTermNo" runat="server" Width="64px"></asp:TextBox>
            </td>
            <td runat="server">Trans No:<asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtTrxNo" runat="server" Width="78px"></asp:TextBox>
            </td>
            <td runat="server">Status:<br />
                <asp:DropDownList CssClass="tb10" BorderStyle="Solid" ID="status_ddl" runat="server"     Width="92px"    >
                </asp:DropDownList>
            </td>
            <td runat="server">Source:<br />
                <asp:DropDownList CssClass="tb10" BorderStyle="Solid" ID="ddlSource" runat="server"     Width="92px"    >
                    <asp:ListItem Text="ALL" Value="ALL"  />
                    <asp:ListItem Text="POS" Value="POS" />
                    <asp:ListItem Text="EXTERNAL" Value="EXTERNAL" />
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
                <table runat="server" visible="false" id="table_info" width="1300" style="font-size:15px;font-weight:bold">
                    <tr>
                        <td  runat="server" >Total:<span runat="server" id="sp_Total" style="color:#30a50f"></span></td> 
                        <td  runat="server" >Cleared:<span runat="server" id="sp_Cleared" style="color:#30a50f"></span></td> 
                        <td  runat="server" >Reported:<span runat="server" id="sp_Reported" style="color:#30a50f"></span></td> 
                        <td  runat="server" >ClearedWithWarning:<span runat="server" id="sp_ClearedWithWarning" style="color:#30a50f"></span></td> 
                        <td  runat="server" >ReportedWithWarning:<span runat="server" id="sp_ReportedWithWarning" style="color:#30a50f"></span></td> 
                        <td  runat="server" >ClearanceFailed:<span runat="server" id="sp_ClearanceFailed" style="color:#ea0f0f"></span></td> 
                        <td  runat="server" >ReportingFailed:<span runat="server" id="sp_ReportingFailed" style="color:#ea0f0f"></span></td> 
                        <td  runat="server" >TotalFailed:<span runat="server" id="sp_TotalFailed" style="color:#ea0f0f"></span></td>  

                    </tr>
                </table>
                <br />
            </td>      
             </tr>
        <tr>
            <td align="center">

                   <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                            <asp:GridView ID="GridView1" runat="server" class="table table-bordered table-condensed table-responsive table-hover " BorderColor="Transparent"  AutoGenerateColumns="False"
                    AllowPaging="True" AllowSorting="True" Width="1100px"    OnRowCommand="GridView1_RowCommand"  
        RowStyle-HorizontalAlign="Left"            PageSize="16" PagerSettings-Position="Top" DataSourceID="ObjectDataSource1"  >

                     

                    <RowStyle CssClass="eachRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <Columns>
                          
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnk_viewdetails" Text="Details" CommandName="view_Details" CommandArgument='<%# Eval("XMLFileName").ToString() %>'  ></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IRN" HeaderText="IRN" SortExpression="IRN" />
                        <asp:TemplateField HeaderText="XML File Name" SortExpression="XMLFileName">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID="tb111" Text='<%# Eval("XMLFileName").ToString()  %>'  Width="125" BorderColor="Transparent" ReadOnly="true" BackColor="Transparent" ></asp:TextBox>
                                 </ItemTemplate>
                        </asp:TemplateField> 
                        <asp:BoundField DataField="TaxTotal" HeaderText="TaxTotal" SortExpression="TaxTotal" />
                        <asp:BoundField DataField="DocType" HeaderText="DocType" SortExpression="DocType" />
                        <asp:BoundField DataField="Action" HeaderText="Action" SortExpression="Action" />
                          <asp:TemplateField HeaderText="Action Status" SortExpression="ActionStatus">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lbl1" Font-Bold="true" ForeColor='<%#Eval("ActionStatus").ToString().Equals("Cleared")||Eval("ActionStatus").ToString().Equals("Reported")?System.Drawing.Color.Green:System.Drawing.Color.Red  %>' Text='<%# Eval("ActionStatus").ToString() %>'></asp:Label>
                            <br />
                                      <asp:Button runat="server" ID="btnReport" Text="Report" 
                                    CommandName="report"
                                   Visible='<%# Eval("ActionStatus").ToString()=="ReportingFailed" %>' 
                                    CommandArgument='<%# Eval("XMLFileName").ToString()+","+ Eval("DocType").ToString()+","+ Eval("Source").ToString()+","+ Eval("ActionStatus").ToString() %>'></asp:Button>
                             </ItemTemplate>
                        </asp:TemplateField>
                            <asp:TemplateField HeaderText="TransactionDate" SortExpression="TransactionDate">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID="tb1111" Text='<%# Eval("TransactionDate").ToString()  %>' Width="125" BorderColor="Transparent" ReadOnly="true" BackColor="Transparent" ></asp:TextBox>
                              
                            </ItemTemplate>
                        </asp:TemplateField>
                   
                        <asp:BoundField DataField="StoreNo" HeaderText="StoreNo" SortExpression="StoreNo" />
                        <asp:BoundField DataField="TerminalNo" HeaderText="TermNo" SortExpression="TerminalNo" />
                        <asp:BoundField DataField="TrxNo" HeaderText="TrxNo" SortExpression="TrxNo" />
                        <asp:BoundField DataField="TimeleftToReport" HeaderText="Report_In" SortExpression="TimeleftToReport" />
                        <asp:BoundField DataField="ReportedIn" HeaderText="ReportedIn" SortExpression="ReportedIn" />
                        <asp:BoundField DataField="Source" HeaderText="Source" SortExpression="Source" />
                        <%--<asp:BoundField DataField="ActionCount" HeaderText="Action Count" SortExpression="ActionCount" />--%>

                         <asp:TemplateField HeaderText="Error Count" SortExpression="ErrorCount">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnk_viewErrors" Font-Bold="true" ForeColor='<%# Eval("ErrorCount").ToString() == "0" ? System.Drawing.Color.Black:System.Drawing.Color.Red %>' Font-Underline='<%# Eval("ErrorCount").ToString() == "0" ? false:true %>' Enabled='<%# Eval("ErrorCount").ToString() == "0" ? false:true %>' Text='<%# Eval("ErrorCount").ToString() %>' CommandName="lnk_viewErrors" CommandArgument='<%# Eval("Id").ToString() %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>

                         <asp:TemplateField HeaderText="Warning Count" SortExpression="WarningCount">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnk_viewWarnings"  Font-Bold="true" ForeColor='<%# Eval("WarningCount").ToString() == "0" ? System.Drawing.Color.Black:System.Drawing.Color.Brown %>'   Font-Underline='<%# Eval("WarningCount").ToString() == "0" ? false:true %>' Enabled='<%# Eval("WarningCount").ToString() == "0" ? false:true %>' Text='<%# Eval("WarningCount").ToString() %>' CommandName="lnk_viewWarnings" CommandArgument='<%# Eval("Id").ToString() %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--<asp:BoundField DataField="ErrorCount" HeaderText="Error Count" SortExpression="ErrorCount" />--%>
                        <%--<asp:BoundField DataField="WarningCount" HeaderText="Warning Count" SortExpression="WarningCount" />--%>
                          <asp:TemplateField HeaderText="UUID" SortExpression="UUID">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID="tb1211" Text='<%# Eval("UUID").ToString()  %>'  Width="100" BorderColor="Transparent" ReadOnly="true" BackColor="Transparent" ></asp:TextBox>
                                    </ItemTemplate>
                        </asp:TemplateField>
                        <%-- <asp:BoundField DataField="UUID" HeaderText="UUID" SortExpression="UUID" />--%>
                        <asp:BoundField DataField="CreatedDate" HeaderText="CreatedDate" SortExpression="CreatedDate" />
                        <%--<asp:BoundField DataField="UpdatedDate" HeaderText="UpdatedDate" SortExpression="UpdatedDate" />--%>
                    </Columns>
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
      <asp:HiddenField ID="hdnField" runat="server" />
    <ajaxtoolkit:modalpopupextender id="mp_err" runat="server" backgroundcssclass="modalBackground" okcontrolid="OKbutton" popupcontrolid="ModalPanel" PopupDragHandleControlID="ModalPanel" targetcontrolid="hdnField" />
    <asp:Panel ID="ModalPanel" runat="server" CssClass="modalPopup"  >
         <asp:GridView ID="gverr" runat="server"   AutoGenerateColumns="true"
                    AllowPaging="True" AllowSorting="True" Width="700px" class="table table-bordered table-condensed table-responsive table-hover " BorderColor="Transparent"
        RowStyle-HorizontalAlign="Right"> 

                    <RowStyle CssClass="eachRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
             <HeaderStyle CssClass="HeaderRow"    Font-Bold="false" />    

                    <SortedAscendingCellStyle />
                    <SortedAscendingHeaderStyle />
                    <SortedDescendingCellStyle />
                    <SortedDescendingHeaderStyle />
                </asp:GridView>
        <br />
        
<asp:Button ID="OKbutton" Text="OK" runat="server" />
 </asp:Panel>
  
        <%--this is required to work error/warning notification toaster.
             not requred when the page has requiredbvalidators.
             you can enable it in all pages individually or add in site.master, first option os preferred--%>
            <br />
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConStr %>" SelectCommand="SELECT [Id], [StoreNo], [StoreName], [StoreIP] FROM [Stores]"></asp:SqlDataSource>
            <br />

     <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="ODS_DATA" TypeName="ZatcaWebApp.Viewinvoices">
                <SelectParameters>
                    <asp:Parameter Name="table_required" Type="String" DefaultValue="DT1_STATIC" />
                </SelectParameters>
            </asp:ObjectDataSource>
    
  
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="valdssdd" />
    </asp:Content>

