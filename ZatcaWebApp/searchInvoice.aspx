<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="searchInvoice.aspx.cs" Inherits="ZatcaWebApp.searchInvoice" ValidateRequest="false" %>
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
       <table width="1200px" border="0">
        <tr>
            <td>
                   <table id="main0" style="width: 1000px !important; margin-left:60px " border="0">
      
        <tr>

           
            <td runat="server"  >Enter XML File Name : <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtXMLFileName" runat="server" Width="478px"></asp:TextBox>
            </td>
             
            <td style="vertical-align: bottom" align="left">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <asp:Button ID="btnSubmit" runat="server" CssClass="buttonYellow" OnClick="btnSubmit_Click" Text="SUBMIT" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnSubmit" />

                    </Triggers>
                </asp:UpdatePanel>

            </td>
              <td align="left" style="width:300px !important"> 
                  
            </td>
             
        </tr>
        

    </table>
            </td> 
        </tr>
           <tr>
               <td align="right" runat="server" visible="false" id="td_labels" style="padding-right:10px"> <br />   
                  <asp:Label runat="server" ID="lblDocType"   ForeColor="#000066"></asp:Label>
                   <br />
                   <asp:Label runat="server" ID="lblSource"   ForeColor="#000066"></asp:Label>
                   <br />
                   <asp:Label runat="server" ID="lblActionStatus"   ForeColor="#000066"></asp:Label>
                   <br />
                   <asp:Label runat="server" ID="lblPath"  ForeColor="#000066"></asp:Label>
                  
                      
               </td>
           </tr>
        <tr>
            <td align="left" runat="server" id="td_Err" visible="true"   style="color:red" >

            </td>
        </tr>
           <tr>
            <td align="left" runat="server" id="td_warning" visible="true"  style="color:brown"   >

            </td>
        </tr>
            <tr>
            <td align="left" runat="server" id="td_info" visible="true"  style="color:black"   >

            </td>
        </tr>
        <tr>
            <td align="left" runat="server" id="td_invoice_links" visible="false"   ><br />
                <table border="0" style="width:1200px">
                    <tr>
                        <td>
                              <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                              
                         <asp:LinkButton runat="server" ID="lnk_vewInvoiceSummary" Text="View Invoice Summary" OnClick="lnk_vewInvoiceSummary_Click" ></asp:LinkButton><br />
                         <asp:LinkButton runat="server" ID="lnk_SellerDetails" Text="Seller Details" OnClick="lnk_SellerDetails_Click"></asp:LinkButton><br />
                         <asp:LinkButton runat="server" ID="lnk_buyerDetails" Text="Buyer Details" OnClick="lnk_buyerDetails_Click"></asp:LinkButton><br />
                         <asp:LinkButton runat="server" ID="lnk_documentleveAllowance" Text="Allowance-Document Level" OnClick="lnk_documentleveAllowance_Click" ></asp:LinkButton><br />
                         <asp:LinkButton runat="server" ID="lnk_lineItems" Text="Line Items" OnClick="lnk_lineItems_Click"></asp:LinkButton><br />
                         <asp:LinkButton runat="server" ID="lnk_VAT" Text="VAT" OnClick="lnk_VAT_Click" ></asp:LinkButton><br />
                         <asp:LinkButton runat="server" ID="lnk_paymentInfo" Text="Payment Info" OnClick="lnk_paymentInfo_Click" ></asp:LinkButton><br />
                         <asp:LinkButton runat="server" ID="lnk_ViewXML" Text="View XML" OnClick="lnk_ViewXML_Click" ></asp:LinkButton><br />
                         <asp:LinkButton runat="server" ID="lnk_downloadXML" Text="DownLoad XML" OnClick="lnk_downloadXML_Click"></asp:LinkButton><br />
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="lnk_vewInvoiceSummary" />
                        <asp:PostBackTrigger ControlID="lnk_SellerDetails" />
                        <asp:PostBackTrigger ControlID="lnk_buyerDetails" /> 
                        <asp:PostBackTrigger ControlID="lnk_documentleveAllowance" />
                        <asp:PostBackTrigger ControlID="lnk_lineItems" />
                        <asp:PostBackTrigger ControlID="lnk_VAT" />
                        <asp:PostBackTrigger ControlID="lnk_paymentInfo" />
                        <asp:PostBackTrigger ControlID="lnk_ViewXML" />
                        <asp:PostBackTrigger ControlID="lnk_downloadXML" /> 

                    </Triggers>
                </asp:UpdatePanel>
                        </td>
                        <td align="right">
                            <asp:Image Width="150" Height="150" ID="imgQR" runat="server" />


                        </td>
                    </tr>
                </table>
                
               
       
            </td>
        </tr>
           <tr>
               <td>
                   
                                <asp:GridView ID="GridView1" runat="server" class="table table-bordered table-condensed table-responsive table-hover " BorderColor="Transparent" AutoGenerateColumns="true"
                   Width="1200px"  RowStyle-HorizontalAlign="Left" PageSize="16" PagerSettings-Position="Top"    > 

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
               </td>
           </tr>
    </table>

          <asp:HiddenField ID="hdnField" runat="server" />
    <ajaxtoolkit:modalpopupextender id="mp_xml" runat="server" backgroundcssclass="modalBackground" okcontrolid="OKbutton" popupcontrolid="ModalPanel" PopupDragHandleControlID="ModalPanel" targetcontrolid="hdnField" />
    <asp:Panel ID="ModalPanel" runat="server" CssClass="modalPopup"  >
        <asp:TextBox ID="TextBox1" runat="server" Width="700px" Height="400px"  ReadOnly="true"
 BorderStyle="None" BorderWidth="0" TextMode="MultiLine"  style="overflow:auto;"
 BackColor="#222222" ForeColor="White"></asp:TextBox>
        
        <br />
        
<asp:Button ID="OKbutton" Text="OK" runat="server" />
        </asp:Panel>
         
 
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="valdssdd" />
</asp:Content>

