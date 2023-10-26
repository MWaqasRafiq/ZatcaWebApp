<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="ZatcaWebApp.Home" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
         <tr>
                            <td colspan="3" align="right" style="height: 50px !important;padding-bottom:20px;padding-right:20px" runat="server" id="TD_VATID">
                         VAT No:       <asp:DropDownList ID="ddlVAT" runat="server" CssClass="tb10" AutoPostBack="true" OnSelectedIndexChanged="ddlVAT_SelectedIndexChanged"></asp:DropDownList>
                            </td>
                        </tr>
        <tr>
            <td>
                <asp:chart id="Chart1" runat="server" 
                                     Height="369px" Width="388px" DataSourceID="SqlDataSource1">
                          <titles>
                            <asp:Title  Name="Title1" Text="E-Invoices Failed          " Font="Times New Roman, 16pt, style=Bold"   />
                          </titles>
                          <legends>
                            <asp:Legend Alignment="Center" Docking="Bottom" 
                                        IsTextAutoFit="false" Name="Default" 
                                        LegendStyle="Row" />
                          </legends>
                          <series >
                            <asp:Series Name="Default" ChartType="Pie"  XValueMember="Field"    YValueMembers="Val" IsValueShownAsLabel="true"/>
                          </series>
                          <chartareas>
                            <asp:ChartArea Name="ChartArea1" Area3DStyle-Enable3D="true"
                                             BorderWidth="0" />
                          </chartareas>
                        </asp:chart>
            </td>
            <td>
                  <asp:chart id="Chart2" runat="server" 
                                     Height="369px" Width="388px" DataSourceID="SqlDataSource2">
                          <titles>
                            <asp:Title  Name="Title1" Text="Pending Invoices" Font="Times New Roman, 16pt, style=Bold"   />
                          </titles>
                          <legends>
                            <asp:Legend Alignment="Center" Docking="Bottom" 
                                        IsTextAutoFit="false" Name="Default" 
                                        LegendStyle="Row" />
                          </legends>
                          <series >
                            <asp:Series Name="Default" ChartType="Pie"  XValueMember="Field"    YValueMembers="Val" IsValueShownAsLabel="true" />
                          </series>
                          <chartareas>
                            <asp:ChartArea Name="ChartArea1" Area3DStyle-Enable3D="true"
                                             BorderWidth="0" />
                          </chartareas>
                        </asp:chart>
            </td>
      
            <td>
                   <asp:chart id="Chart3" runat="server" 
                                     Height="369px" Width="388px" DataSourceID="SqlDataSource3">
                          <titles>
                            <asp:Title  Name="Title1" Text="Standard Invoices" Font="Times New Roman, 16pt, style=Bold"   />
                          </titles>
                          <legends>
                            <asp:Legend Alignment="Center" Docking="Bottom" 
                                        IsTextAutoFit="false" Name="Default" 
                                        LegendStyle="Row" />
                          </legends>
                          <series >
                            <asp:Series Name="Default" ChartType="Pie"  XValueMember="Field"    YValueMembers="Val" IsValueShownAsLabel="true" />
                          </series>
                          <chartareas>
                            <asp:ChartArea Name="ChartArea1" Area3DStyle-Enable3D="true"
                                             BorderWidth="0" />
                          </chartareas>
                        </asp:chart>
            </td>
          
        </tr>

        <tr>
            <td align="center">
                <table border="0">
                    <tr>
                        <td>Total VAT(Submitted):</td>
                        <td style="padding-left:10px"><asp:Label runat="server" ID="lblTotalVatSubmitted" ></asp:Label></td>
                    </tr>
                     <tr>
                        <td>VAT (Failed):</td>
                        <td style="padding-left:10px"><asp:Label runat="server" ID="lblVatFailed" ></asp:Label></td>
                    </tr>
                </table>
            </td>
           <td align="center">
                 <table border="0">
                    <tr>
                        <td>Count(Pending):</td>
                        <td style="padding-left:10px"><asp:Label runat="server" ID="lblPending" ></asp:Label></td>
                    </tr>
                     <tr>
                        <td>Count (Processed):</td>
                        <td style="padding-left:10px"><asp:Label runat="server" ID="lblProcessed" ></asp:Label></td>
                    </tr>
                </table>
            </td>
            <td align="center">
                 <table border="0">
                    <tr>
                        <td>VAT(B2B):</td>
                        <td style="padding-left:10px"><asp:Label runat="server" ID="lblVatB2B" ></asp:Label></td>
                    </tr>
                     <tr>
                        <td>VAT (B2C):</td>
                        <td style="padding-left:10px"><asp:Label runat="server" ID="lblVatB2C" ></asp:Label></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

        <asp:SqlDataSource ID="SqlDataSource1" runat="server"   SelectCommand="   select 'Success' as Field,(select count(*) from invoices where LTRIM(RTRIM(ActionStatus))  in('Cleared','Reported') ) as Val from Invoices
   union
   select 'Failed' as Field,(select count(*) from invoices where LTRIM(RTRIM(ActionStatus)) not in('Cleared','Reported') ) as Val from Invoices">
            </asp:SqlDataSource>
      <asp:SqlDataSource ID="SqlDataSource2" runat="server"   SelectCommand="    select 'Processed' as Field,(select count(*) from invoices ) as Val from Invoices
   union
   select 'Pending' as Field,@PendingCount as Val  ">
          <SelectParameters>
              <asp:ControlParameter ControlID="lblPending" DefaultValue="0" Name="PendingCount" PropertyName="Text" />
          </SelectParameters>
           
            </asp:SqlDataSource>
       <asp:SqlDataSource ID="SqlDataSource3" runat="server"   SelectCommand="     
   select 'B2B' as Field,(select count(*) from invoices where DocType='B2B' ) as Val from Invoices
   union
   select 'B2C' as Field,(select count(*) from invoices where DocType='B2C' ) as Val from Invoices">
            </asp:SqlDataSource> 
</asp:Content>
