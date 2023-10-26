<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UploadInvoice.aspx.cs" Inherits="ZatcaWebApp.UploadInvoice" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
      <script type="text/javascript" language="javascript" >
        function validate() {
            if (Page_ClientValidate())
                return confirm('Are you sure you want to continue?. Click Ok to continue');
        }
      </script>
      
                   <table id="main0" style="margin-left:60px" border="0"  >
      
        <tr>

           
            <td runat="server" style=" font-weight:bold;font-size:12px; color:brown;text-align:left" colspan="4"  > 
                <br />
                <br />
                Uploading the B2B XML will be sent for clearance.<br />
                Uploading B2C XML will be sent for reporting.

                <br /> <br /> <br />
            <br /></td>
             
            
        </tr>
        

        <tr>

           
            
              <td align="left" style="width:150px !important"> 
           <asp:RadioButtonList runat="server" ID="rd_doctype" RepeatDirection="Horizontal">
               <asp:ListItem Text="B2B" Value="B2B" Selected="True"></asp:ListItem>
               <asp:ListItem Text="B2C" Value="B2C"  ></asp:ListItem>
           </asp:RadioButtonList>

            </td>
              <td align="left" style="width:150px !important"> 
           <asp:RadioButtonList runat="server" ID="rd_source" RepeatDirection="Horizontal">
               <asp:ListItem Text="EXTERNAL" Value="EXTERNAL" Selected="True"></asp:ListItem>
               <asp:ListItem Text="STORE" Value="POS"  ></asp:ListItem>
           </asp:RadioButtonList>

            </td>
            <td runat="server">   <asp:FileUpload ID="FileUpload1" runat="server"   /></td>
            <td style="vertical-align: bottom" align="left">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                      
                        <asp:Button ID="btnSubmit" runat="server" CssClass="buttonYellow" OnClick="btnSubmit_Click" Text="Upload and Submit"
                            OnClientClick="return confirm('Are you sure you want to continue?. Click Ok to continue');"/>
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnSubmit" />

                    </Triggers>
                </asp:UpdatePanel>

            </td>
             
             
        </tr>
        

    </table>
           

    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="valdssdd" />
</asp:Content>

