<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StoresList.aspx.cs" Inherits="ZatcaWebApp.StoresList" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 
        <table id="main" style="width: 450px; margin-left:260px;text-align:left">
      
         
          <tr  style="height:50px">
            <td style="width:40%">Store Name:</td>
              <td>
                   <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtStoreName" runat="server"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtStoreName" ErrorMessage=" Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
             
              </td>
            

        </tr>
        <tr>
            <td style="width:40%">Store No:</td>
            <td>
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtStoreNo" runat="server"></asp:TextBox>
            </td>

        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtStoreNo" ErrorMessage="Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
            </td>

        </tr>
        <tr>
            <td class="auto-style1">Store IP:</td>
            <td class="auto-style1">
               <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtStoreIP" runat="server"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtStoreIP" ErrorMessage="Required" ForeColor="Red" ValidationGroup="vv"></asp:RequiredFieldValidator>
                <br />
            </td>

        </tr>
               <tr>
            <td class="auto-style1">Status:</td>
            <td class="auto-style1">
               <asp:DropDownList ID="status_Ddl" runat="server"   BorderStyle="Solid" CssClass="tb10"  > 
                            <asp:ListItem Text="Active-1" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Not Active-0" Value="0"></asp:ListItem> 
                        </asp:DropDownList>
                <br />
                
                <br />
            </td>

        </tr>
        <tr>
            <td>Open Hours:</td>
            <td  >
                <br />
                <table style="margin-left: -100px"  >
                    <tr>
                                  <td>From Time:</td>
                    <td></td>
                    <td>To Time:</td>
                    </tr>
                      <tr>
                          <td>
                              <table>
                                  <tr>
                                            <td>
                                    <asp:TextBox runat="server" ID="txtfromHOUR"   Text="00" BorderStyle="Solid" Width="25px" Height="38px" BorderWidth="1" BorderColor="#003366" Style="padding-left: 3px;text-align:center;vertical-align:middle" MaxLength="2"></asp:TextBox></td>
                                <td>:</td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtfromMINUTE" Text="00" BorderStyle="Solid" Width="25px" Height="38px" BorderWidth="1" BorderColor="#003366" Style="padding-left: 3px;text-align:center;vertical-align:central" MaxLength="2"></asp:TextBox></td>
                           
                                  </tr>
                              </table>
                          </td>
                             <td></td>
                          <td>
                              <table>
                                  <tr>
                                              <td>
                                     <asp:TextBox runat="server" ID="txttoHOUR" Text="23" BorderStyle="Solid" Width="25px" Height="38px" BorderWidth="1" BorderColor="#003366" Style="padding-left: 3px" MaxLength="2"></asp:TextBox></td>
                                <td>:</td>
                                <td>
                                    <asp:TextBox runat="server" ID="txttoMINUTE" Text="59" BorderStyle="Solid" Width="25px" Height="38px" BorderWidth="1" BorderColor="#003366" Style="padding-left: 3px" MaxLength="2"></asp:TextBox></td>
                           
                                  </tr>
                              </table>
                          </td>
                           </tr>
                    <tr>
                         <td align="left">
                        
                         <asp:RangeValidator runat="server" ID="cmp1" Text="Invalid HR" ForeColor="Red"   Type="Integer" ControlToValidate="txtfromHOUR" MaximumValue="23" MinimumValue="0" ValidationGroup="vv"/>
                       <br />
                         <asp:RangeValidator runat="server" ID="RangeValidator1" Text="Invalid MIN" ForeColor="Red"   Type="Integer" ControlToValidate="txtfromMINUTE" MaximumValue="59" MinimumValue="0" ValidationGroup="vv" />
                        
                     </td>
                      <td align="center">&nbsp;</td>
                       <td align="left">
                        
                         <asp:RangeValidator runat="server" ID="RangeValidator2" Text="Invalid HR" ForeColor="Red"   Type="Integer" ControlToValidate="txttoHOUR" MaximumValue="23" MinimumValue="0" ValidationGroup="vv" />
                       <br />
                         <asp:RangeValidator runat="server" ID="RangeValidator3" Text="Invalid MIN" ForeColor="Red"   Type="Integer" ControlToValidate="txttoMINUTE" MaximumValue="59" MinimumValue="0" ValidationGroup="vv"/>
                        
                     </td>
                    </tr>
                </table>
            </td>

        </tr>
        
        <tr>
            <td></td>
            <td   align="left">
               <br />
                   <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                         <asp:Button ID="btnSubmit" runat="server" CssClass="buttonYellow" OnClick="btnSubmit_Click" Text="Submit" Height="39px" ValidationGroup="vv" />
 </ContentTemplate>
                                        <Triggers>
                                            <asp:PostBackTrigger ControlID="btnSubmit" />

                                        </Triggers>
                                    </asp:UpdatePanel>
            </td>

        </tr>

    </table>

     <table>
        <tr>
            <td>
                 <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="GridView1" runat="server" class="table table-bordered table-condensed table-responsive table-hover " BorderColor="Transparent" AutoGenerateColumns="False"
                            AllowPaging="True" AllowSorting="True" Width="900px"    AutoGenerateEditButton="True"  
                           
                            DataSourceID="SqlDataSource1" PageSize="16" PagerSettings-Position="Top" DataKeyNames="Id">

                            <Columns>
                             
                                <asp:BoundField HeaderText="Id" DataField="Id" SortExpression="Id" InsertVisible="False" ReadOnly="True" />
                                <asp:BoundField HeaderText="StoreNo" DataField="StoreNo" SortExpression="StoreNo" />
                                <asp:BoundField HeaderText="StoreName" DataField="StoreName" SortExpression="StoreName" />
                                <asp:BoundField HeaderText="StoreIP" DataField="StoreIP" SortExpression="StoreIP" /> 
                                <asp:BoundField DataField="OpenHoursStart" HeaderText="OpenHoursStart" SortExpression="OpenHoursStart" />
                                <asp:BoundField DataField="OpenHoursEnd" HeaderText="OpenHoursEnd" SortExpression="OpenHoursEnd" />
                                <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                                <asp:BoundField DataField="CreatedDate" HeaderText="CreatedDate" SortExpression="CreatedDate" />
                                <asp:BoundField DataField="UpdatedDate" HeaderText="UpdatedDate" SortExpression="UpdatedDate" />



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
                      SelectCommand="SELECT [Id], [StoreNo], [StoreName], [StoreIP], [OpenHoursStart], [OpenHoursEnd],case [Status] when 1 then 'Active' when 0 then 'Not Active' end as Status, [CreatedDate], [UpdatedDate] FROM [Stores] ORDER BY [Id] DESC" 
                      UpdateCommand="UPDATE [dbo].[Stores]
   SET [StoreNo] = @StoreNo
      ,[StoreName] =@StoreName
      ,[StoreIP] = @StoreIP
      ,[OpenHoursStart] = @OpenHoursStart
      ,[OpenHoursEnd] =@OpenHoursEnd
      ,[Status] = case @Status when 'Active' then 1 else 0 end
      ,[UpdatedDate] = getdate() where Id=@Id"
                     
                      >
                      <UpdateParameters>
                          <asp:Parameter Name="StoreNo" />
                          <asp:Parameter Name="StoreName" />
                          <asp:Parameter Name="StoreIP" />
                          <asp:Parameter Name="OpenHoursStart" />
                          <asp:Parameter Name="OpenHoursEnd" />
                          <asp:Parameter Name="Status" />
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

