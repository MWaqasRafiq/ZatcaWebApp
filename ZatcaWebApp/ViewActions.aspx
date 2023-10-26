<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewActions.aspx.cs" Inherits="ZatcaWebApp.ViewActions" %>
 
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
    <table width="1200px" border="0">
        <tr>
            <td>
                   <table id="main0" border="0"  >
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
             <%-- <td runat="server">Type:<br />
                <asp:DropDownList CssClass="tb10" BorderStyle="Solid" ID="DDLReportType" runat="server"     Width="122px"    >
                    <asp:ListItem Text="SUMMARY" Value="SUMMARY"  />
                    <asp:ListItem Text="DETAILED" Value="DETAILED" /> 
                </asp:DropDownList>
                <br />
            </td>--%>
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
                   <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                    <ContentTemplate>
                           <asp:CheckBox runat="server" Visible="false" ID="chk_show_errors_only" Text="Only Show Invices with errors"  style="padding:15px;" AutoPostBack="true" OnCheckedChanged="chk_show_errors_only_CheckedChanged"/> 
                          </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="chk_show_errors_only" /> 

                    </Triggers>
                </asp:UpdatePanel>
                          
                      
            </td>      
             </tr>
        <tr>
            <td align="center">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                         <asp:GridView ID="GridView1" runat="server" class="table table-bordered table-condensed table-responsive table-hover " BorderColor="Transparent"  AutoGenerateColumns="False"
                    AllowPaging="True" AllowSorting="True" Width="1200px"     OnRowCommand="GridView1_RowCommand"
                           DataSourceID="ObjectDataSource1"
        RowStyle-HorizontalAlign="Left"     PageSize="16" PagerSettings-Position="Top"    >

                       <Columns>
                           <asp:BoundField DataField="IRN" HeaderText="IRN" SortExpression="IRN" />
                        <asp:BoundField DataField="DocType" HeaderText="DocType" SortExpression="DocType" /> 
                        <asp:BoundField DataField="Action" HeaderText="Action" SortExpression="Action" /> 
                        <asp:BoundField DataField="HTTPResponseCode" HeaderText="HTTPResponseCode" SortExpression="HTTPResponseCode" />
                        <asp:BoundField DataField="CreatedDate" HeaderText="CreatedDate" SortExpression="CreatedDate" />
                        <asp:BoundField DataField="ProcessType" HeaderText="ProcessType" SortExpression="ProcessType" /> 


                           
                             <asp:TemplateField HeaderText="Errors" SortExpression="ErrorCount">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnk_viewErrors" Font-Bold="true" ForeColor='<%# Eval("ErrorCount").ToString() == "0" ? System.Drawing.Color.Black:System.Drawing.Color.Red %>' Font-Underline='<%# Eval("ErrorCount").ToString() == "0" ? false:true %>' Enabled='<%# Eval("ErrorCount").ToString() == "0" ? false:true %>' Text='<%# Eval("ErrorCount").ToString() %>' CommandName="lnk_viewErrors" CommandArgument='<%# Eval("FkInvoices").ToString() %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>

                         <asp:TemplateField HeaderText="Warnings" SortExpression="WarningCount">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnk_viewWarnings"  Font-Bold="true" ForeColor='<%# Eval("WarningCount").ToString() == "0" ? System.Drawing.Color.Black:System.Drawing.Color.Brown %>'   Font-Underline='<%# Eval("WarningCount").ToString() == "0" ? false:true %>' Enabled='<%# Eval("WarningCount").ToString() == "0" ? false:true %>' Text='<%# Eval("WarningCount").ToString() %>' CommandName="lnk_viewWarnings" CommandArgument='<%# Eval("FkInvoices").ToString() %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Info" SortExpression="InfoCount">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnk_viewInfo" Font-Bold="true" ForeColor='<%# Eval("InfoCount").ToString() == "0" ? System.Drawing.Color.Black:System.Drawing.Color.Black %>' Font-Underline='<%# Eval("InfoCount").ToString() == "0" ? false:true %>' Enabled='<%# Eval("InfoCount").ToString() == "0" ? false:true %>' Text='<%# Eval("InfoCount").ToString() %>' CommandName="lnk_viewInfo" CommandArgument='<%# Eval("FkInvoices").ToString() %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>

                           <asp:TemplateField HeaderText="FullResponse" SortExpression="FullResponse">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnk_viewFullResponse"    Text='<%# Eval("FullResponse").ToString()==""?"":"View" %>' CommandName="lnk_viewFullResponse" CommandArgument='<%# Eval("Id").ToString() %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnk_viewdetails" Text="Details" CommandName="view_Details" CommandArgument='<%# Eval("XMLFileName").ToString() %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                     
                    </Columns>

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
      <asp:HiddenField ID="hdnField" runat="server" />
    <ajaxtoolkit:modalpopupextender id="mp_err" runat="server" backgroundcssclass="modalBackground" okcontrolid="OKbutton" popupcontrolid="ModalPanel" PopupDragHandleControlID="ModalPanel" targetcontrolid="hdnField" />
    <asp:Panel ID="ModalPanel" runat="server" CssClass="modalPopup"  >
         <asp:GridView ID="gverr" runat="server" class="table table-bordered table-condensed table-responsive table-hover " BorderColor="Transparent"  AutoGenerateColumns="true"
                    AllowPaging="True" AllowSorting="True" Width="700px"   
        RowStyle-HorizontalAlign="Right"> 

                    <RowStyle CssClass="eachRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
             <HeaderStyle CssClass="HeaderRow"    Font-Bold="false" />    

                    <SortedAscendingCellStyle />
                    <SortedAscendingHeaderStyle />
                    <SortedDescendingCellStyle />
                    <SortedDescendingHeaderStyle />
                </asp:GridView>
        
     
 <asp:TextBox ID="TextBox1" runat="server" Width="700px" Height="400px"  ReadOnly="true"
 BorderStyle="None" BorderWidth="0" TextMode="MultiLine"  style="overflow:auto;"
 BackColor="#222222" ForeColor="White"></asp:TextBox>
        
        <br />
        
<asp:Button ID="OKbutton" Text="OK" runat="server" />
 </asp:Panel>
  
        <%--this is required to work error/warning notification toaster.
             not requred when the page has requiredbvalidators.
             you can enable it in all pages individually or add in site.master, first option os preferred--%>
            <br />
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConStr %>" SelectCommand="SELECT [Id], [StoreNo], [StoreName], [StoreIP] FROM [Stores]"></asp:SqlDataSource>
            <br />
 
       <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="ODS_DATA" TypeName="ZatcaWebApp.ViewActions">
                <SelectParameters>
                    <asp:Parameter Name="table_required" Type="String" DefaultValue="DT1_STATIC" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="valdssdd" />
    </asp:Content>


