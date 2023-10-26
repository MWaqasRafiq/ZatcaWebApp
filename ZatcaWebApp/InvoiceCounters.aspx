<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InvoiceCounters.aspx.cs" Inherits="ZatcaWebApp.InvoiceCounters" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
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
            width: 1230px; 
            text-align:center
        }

         #test {
    width:100%;
    height:100%;
  }
  #table1 {
    margin: 0 auto; /* or margin: 0 auto 0 auto */
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
    <table border="0">
        <tr>
            <td align="center">
                <table id="main0" style="width: 750px !important;" border="0">
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
                            <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtFrom" placeholder="DD/MM/YYYY" runat="server"></asp:TextBox>
                        </td>
                        <td runat="server">
                            <asp:Label runat="server" Text="To(Date):" ID="lblTo" />
                            <br />
                            <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtTo" placeholder="DD/MM/YYYY" runat="server"></asp:TextBox>
                        </td>

                        <td runat="server">StoreNo:<br />
                            <asp:ListBox ID="StoreList" runat="server" SelectionMode="Multiple" CssClass="tb10" BorderStyle="Solid" Width="92px"></asp:ListBox>
                        </td>
                        <td runat="server">X509SerialNumber:<br />
                            <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtX509SerialNumber" runat="server"></asp:TextBox>
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
                        <td style="vertical-align: bottom"></td>
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
                        <td align="center">
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
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">

                            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="GridView1" runat="server" class="table table-bordered table-condensed table-responsive table-hover " BorderColor="Transparent"   AppendDataBoundItems="true"
                                        AllowPaging="True" AllowSorting="True" Width="1100px" OnRowCommand="GridView1_RowCommand" 
                                        RowStyle-HorizontalAlign="Left" PageSize="16" PagerSettings-Position="Top" DataSourceID="ObjectDataSource1">

                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:LinkButton runat="server" ID="lnkView" CommandName="ViewMissingDuplicate" Text="View" 
                                                        CommandArgument='<%# Eval("X509SerialNumber") %>' Visible='<%# !Eval("Missing").Equals("0") %>' ></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>

                                        <RowStyle CssClass="eachRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />

                                        <HeaderStyle CssClass="HeaderRow" Font-Bold="false" />

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

    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="ODS_DATA" TypeName="ZatcaWebApp.InvoiceCounters">
        <SelectParameters>
            <asp:Parameter Name="table_required" Type="String" DefaultValue="DT1_STATIC" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="ObjectDataSource2" runat="server" SelectMethod="ODS_DATA" TypeName="ZatcaWebApp.InvoiceCounters">
        <SelectParameters>
            <asp:Parameter Name="table_required" Type="String" DefaultValue="DT2_STATIC" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="ObjectDataSource3" runat="server" SelectMethod="ODS_DATA" TypeName="ZatcaWebApp.InvoiceCounters">
        <SelectParameters>
            <asp:Parameter Name="table_required" Type="String" DefaultValue="DT3_STATIC" />
        </SelectParameters>
    </asp:ObjectDataSource>














    <asp:HiddenField ID="hdnField" runat="server" />
    <ajaxtoolkit:ModalPopupExtender ID="mp_inv"
        drag="false"  
        runat="server" BackgroundCssClass="modalBackground" OkControlID="OKbutton" PopupControlID="ModalPanel" PopupDragHandleControlID="ModalPanel" TargetControlID="hdnField" />
    <asp:Panel ID="ModalPanel" runat="server" CssClass="modalPopup">
        <div style="overflow:scroll;height:600px;text-align:center;align-content:center;" id="test" >
        <table style="height:auto" border="0" id="table1">
            <tr>
                <td>Duplicate ICVs</td>
              </tr>
            <tr>
             
                <td align="center">
                       <asp:UpdatePanel ID="UpdatePanel4" runat="server">
            <ContentTemplate>
                <asp:GridView ID="GridView2" runat="server" class="table table-bordered table-condensed table-responsive table-hover " BorderColor="Transparent" AutoGenerateColumns="false"
                    AllowPaging="True" AllowSorting="True" Width="500px" OnPageIndexChanged="GridView2_PageIndexChanged"
             OnSorted="GridView2_Sorted"
                    RowStyle-HorizontalAlign="Left" PageSize="16" PagerSettings-Position="Top" DataSourceID="ObjectDataSource2">
                    <Columns>
                        <asp:TemplateField HeaderText="ICV">
                            <ItemTemplate> 
                                <asp:TextBox runat="server" ID="txt1" Width="70" Text='<%#Eval("icv") %>' Enabled="false" BorderColor="Transparent" BackColor="Transparent"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Count">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID="txt2" Width="40" Text='<%#Eval("Count") %>' Enabled="false" BorderColor="Transparent" BackColor="Transparent"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="X509SerialNumber">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID="txt3" Width="320"  Text='<%#Eval("X509SerialNumber") %>' Enabled="false" BorderColor="Transparent" BackColor="Transparent"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="MaxIRN">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID="txt4" Text='<%#Eval("MaxIRN") %>' Enabled="false" BorderColor="Transparent" BackColor="Transparent"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="MinIRN">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID="txt5" Text='<%#Eval("MinIRN") %>' Enabled="false" BorderColor="Transparent" BackColor="Transparent"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="IRNs">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID="TextBox1" Text='<%#Eval("IRNs") %>' ToolTip='<%#Eval("IRNs") %>' Enabled="false" BorderColor="Transparent" BackColor="Transparent"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="MaxDate">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID="txt6" Width="100" Text='<%#Eval("MaxDate") %>' Enabled="false" BorderColor="Transparent" BackColor="Transparent"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="eachRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <HeaderStyle CssClass="HeaderRow" Font-Bold="false" />
                    <PagerSettings Position="Top"></PagerSettings>
                    <PagerStyle CssClass="PagerRow" BorderColor="White" Height="20px" />
                    <SortedAscendingCellStyle />
                    <SortedAscendingHeaderStyle />
                    <SortedDescendingCellStyle />
                    <SortedDescendingHeaderStyle />
                </asp:GridView>

            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="GridView2" /> 
            </Triggers>
        </asp:UpdatePanel>
                </td>
                 </tr>
            <tr>
                  <td>Missing ICVs</td>
                 </tr>
            <tr>
                <td align="center"  >
                      <asp:UpdatePanel ID="UpdatePanel5" runat="server">
            <ContentTemplate>
                <asp:GridView ID="GridView3" runat="server" class="table table-bordered table-condensed table-responsive table-hover " 
                    BorderColor="Transparent" AutoGenerateColumns="false"
                    AllowPaging="True" AllowSorting="True" Width="300px" OnPageIndexChanged="GridView2_PageIndexChanged" OnRowCommand="GridView3_RowCommand"
             OnSorted="GridView2_Sorted"
                    RowStyle-HorizontalAlign="Left" PageSize="16" PagerSettings-Position="Top" DataSourceID="ObjectDataSource3">
                    <Columns>
                       <asp:TemplateField HeaderText="MissingICV">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID="txt1" Text='<%#Eval("MissingICV") %>' Enabled="false" BorderColor="Transparent" BackColor="Transparent"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                       <asp:TemplateField HeaderText="Prev_IRN">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID="txt2" Text='<%#Eval("Prev_IRN") %>' Enabled="false" BorderColor="Transparent" BackColor="Transparent"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                 <asp:Button runat="server" ToolTip="Finds all invoices that are in Invoices table but not in ICV table,with IRN starting with first 11 digits of prev IRN and move them to SIGNED folder for re reporting.This will insert to ICV table and in turn update the records in case of missing ICVs. "
                                     ID="btnchk" Text="Reload from Invoices" CommandName="CheckMissingfromInvoices" CommandArgument='<%#Eval("MissingICV")+","+Eval("Prev_IRN")  %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="eachRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <HeaderStyle CssClass="HeaderRow" Font-Bold="false" />
                    <PagerSettings Position="Top"></PagerSettings>
                    <PagerStyle CssClass="PagerRow" BorderColor="White" Height="20px" />
                    <SortedAscendingCellStyle />
                    <SortedAscendingHeaderStyle />
                    <SortedDescendingCellStyle />
                    <SortedDescendingHeaderStyle />
                </asp:GridView>

            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="GridView3" /> 
            </Triggers>
        </asp:UpdatePanel>
                </td> 
            </tr>
        </table>
     
       </div> 
     

        <br />

        <asp:Button ID="OKbutton" Text="OK" runat="server" />
    </asp:Panel>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="valdssdd" />
</asp:Content>



