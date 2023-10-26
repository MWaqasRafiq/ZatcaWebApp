<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditInvoice.aspx.cs" Inherits="ZatcaWebApp.EditInvoice" validateRequest="false" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     
       <table width="1200px" border="0">
        <tr>
            <td>
                   <table id="main0" style="width: 1000px !important; margin-left:60px " border="0">
      
        <tr>

           
            <td runat="server">Enter XML File Name : <asp:TextBox CssClass="tb10" BorderStyle="Solid" ID="txtXMLFileName" runat="server" Width="478px"></asp:TextBox>
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
              <td align="left" style="width:300px !important"> <br/>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                         <asp:Button ID="btnDownload" runat="server" CssClass="buttonYellow" OnClick="btnDownload_Click" Text="DownLoad XML" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnDownload" />

                    </Triggers>
                </asp:UpdatePanel> 
            </td>
             
        </tr>
        

    </table>
            </td> 
        </tr>
           <tr>
               <td align="left" runat="server" visible="false" id="td_labels" style="padding-left:40px"> <br />   
                 Path:  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label runat="server" ID="lblPath"  ForeColor="#000066"></asp:Label>
                   <br />
                  Source:&nbsp;&nbsp; &nbsp;&nbsp;<asp:Label runat="server" ID="lblSource"   ForeColor="#000066"></asp:Label>
                    <br />
                  DocType:&nbsp;&nbsp; <asp:Label runat="server" ID="lblDocType"   ForeColor="#000066"></asp:Label>
                   <br />
                 Status:&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label runat="server" ID="lblActionStatus"   ForeColor="#000066"></asp:Label>
                     
               </td>
           </tr>
        <tr>
            <td align="left" >
                        <br />   
                <asp:TextBox style="margin-left:20px" TextMode="MultiLine" Width="1200" Height="1000" runat="server" ID="txtXML"  ></asp:TextBox>


        
                        <br />
                        <br />

                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <asp:Button ID="btnSave" runat="server" CausesValidation="true"  OnClientClick= "return confirm('Are you sure you want to continue?. Click Ok to continue');"  CssClass="buttonYellow" OnClick="btnSave_Click" Text="Sign and Report" Visible="false" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnSave" />

                    </Triggers>
                </asp:UpdatePanel>
        
            </td>
        </tr>
    </table>

    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="valdssdd" />
</asp:Content>
