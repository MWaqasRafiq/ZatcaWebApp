

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Data;
using System.Xml;
using RestSharp;
using System.CodeDom;
using System.Data.Common;
using System.Reflection;
using System.Xml.Serialization;
using System.Text;
using QRCoder;
using System.Drawing;

namespace ZatcaWebApp
{
    public partial class searchInvoice : System.Web.UI.Page
    {
        Basepage bp = new Basepage();
        //public static string CONFIG=""
        public bool isClientScrpiptActive = true;
        static Invoice invoice;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                string qq1 = "update dbo.Invoices set TimeleftToReport=( SELECT cast( FORMAT(CAST((SELECT DATEDIFF(SECOND, GETDATE(), TransactionDate) AS DateDiff)/86400.000 AS datetime), 'HH:mm:ss') as time(7)))  where DocType = 'B2C' and ActionStatus != 'Reported' ";

                bp.getDataDBQuery_Update(qq1);


                this.Master.Page_heading = "View Invoice Details";

                this.Master.TextMessage.Text = "";

                td_invoice_links.Visible = lblPath.Text != "";

                if (!IsPostBack)
                {
                    if (Session["userName"] == null)
                    {
                        Session.Clear();
                        Session.Abandon();
                        Response.Redirect("Login.aspx");
                    }
                    else
                    {
                        Session["PageId"] = "li_viewInvoiceDetails";
                        string qq = Request.QueryString["ViewXML"];
                        if (qq != null && qq.Equals("true"))
                        {
                            txtXMLFileName.Text = Session["ViewXML_filename"].ToString();
                            Load();
                        }

                        else txtXMLFileName.Text = "";

                    }





                }

            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }


        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";

                Load();

            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }
        }

        private void Load()
        {

            imgQR.ImageUrl = "";
            td_invoice_links.Visible = false;
            td_labels.Visible = false;

            string vatno = txtXMLFileName.Text.Trim().Split(new char[] { '_' })[0].Trim();

            bool found = false;
            vatno = vatno.Replace("u","");
            DataTable dt1 = bp.getDataDBQuery("select * from VATIDs where VATNo='" + vatno + "' or '" + vatno + "'=''",true,LOGTYPE.LG);
            bp.logWrite("vatno:" + vatno, LOGTYPE.LG);


            if (dt1 != null && dt1.Rows.Count > 0)
            {
                bp.logWrite("vatno found. in DB -VATIDs table" , LOGTYPE.LG);

                for (int i = 0; i < dt1.Rows.Count; i++)
                {

                    string constr = dt1.Rows[i]["ConnectionString"].ToString();
                    string InvoiceXMLFolder = dt1.Rows[i]["InvoiceXMLFolder"].ToString();

                    bp.logWrite("connectionString:" + constr, LOGTYPE.LG);
                    bp.logWrite("InvoiceXMLFolder:" + InvoiceXMLFolder, LOGTYPE.LG);


                    string qq = @"select doctype, XMLFileName,Action,ActionStatus,Source,
                     (SELECT  +type+'-'+ Message+'brrrrrr'
                                   FROM Errors where FkInvocies=Invoices.Id and IsLatest=1 and type='ERROR'
                                   FOR XML PATH (''))   as err,
                     (SELECT 'brrrrrr' +type+'-'+ Message
                                   FROM Errors where FkInvocies=Invoices.Id and IsLatest=1 and type='WARNING'
                                   FOR XML PATH (''))   as warning,
                     (SELECT 'brrrrrr' +type+'-'+ Message
                                   FROM Errors where FkInvocies=Invoices.Id and IsLatest=1 and type='INFO'
                                   FOR XML PATH (''))   as info
                     from Invoices where XMLFileName='" +
                     txtXMLFileName.Text.Trim() + "'";// or IRN='" + txtXMLFileName.Text.Trim() + "'";
                    DataTable dt = bp.getDataDBQuery(qq,constr);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        found = true;
                        string doctype = dt.Rows[0]["doctype"].ToString();
                        string Action = dt.Rows[0]["Action"].ToString();
                        string ActionStatus = dt.Rows[0]["ActionStatus"].ToString();
                        string Source = dt.Rows[0]["Source"].ToString();
                        string XMLFileName = dt.Rows[0]["XMLFileName"].ToString();
                        string err = dt.Rows[0]["err"].ToString();
                        string warning = dt.Rows[0]["warning"].ToString();
                        string info = dt.Rows[0]["info"].ToString();

                        Enum.TryParse(doctype, out DOCTYPE _doctype);
                        Enum.TryParse(Source, out SOURCE _source);
                        Enum.TryParse(ActionStatus, out ACTION_STATUS _ActionStatus);
                        XMLSTATUS _xmlstatus = bp.getXML_STATUS(_doctype, _ActionStatus);

                        //  string[] itemNames = System.Enum.GetNames(typeof(XMLSTATUS));
                        //for (int i = 0; i <= itemNames.Length - 1; i++)
                        {
                            //    Enum.TryParse(itemNames[i], out XMLSTATUS _xmlstatus);
                            string folder = Basepage.Get_Folder(_doctype, _source, _xmlstatus, XMLFileName, InvoiceXMLFolder);   //eg: C:\IDOL\ZATCA\varno\files\invoiceXML\EXTERNAL\B2C\FAILED

                            string fpath = folder + XMLFileName;
                            if (File.Exists(fpath))
                            {

                                LoadXML(fpath);

                                td_invoice_links.Visible = true;
                                lblPath.Text = fpath;
                                lblSource.Text = Source;
                                lblDocType.Text = doctype;
                                lblActionStatus.Text = ActionStatus;
                                td_labels.Visible = true;

                                //err.Replace("ERROR", "<div style='font-weight:bold'>ERROR</div>");
                                //warning.Replace("WARNING", "<div style='font-weight:bold'>WARNING</div>");
                                //info.Replace("INFO", "<div style='font-weight:bold'>INFO</div>");

                                td_Err.InnerHtml = err.Replace("brrrrrr", "<br/><br/>");
                                td_info.InnerHtml = info.Replace("brrrrrr", "<br/><br/>");
                                td_warning.InnerHtml = warning.Replace("brrrrrr", "<br/><br/>");

                                ViewInvoiceSummery();

                                //  break;
                            }
                            else
                            {
                                bp.logWrite("File not found." + fpath, LOGTYPE.LG);
                                this.Master.ShowMessage("File not found.", Alert.error, isClientScrpiptActive);
                            }

                        }


                        break;

                    }
                     
                        
                }
                if(!found)
                this.Master.ShowMessage("No data found", Alert.error, isClientScrpiptActive);
            }



        }

        private void LoadXML(string fpath)
        {
            var xmlSerializer = new XmlSerializer(typeof(Invoice));
            using (var reader = new StreamReader(fpath))
            {
                invoice = (Invoice)xmlSerializer.Deserialize(reader);
            }



        }



        public void ViewInvoiceSummery()
        {
            //DocumentSummery
            var dataTable = new DataTable();
            //  dataTable.Columns.Add("Invoice QR code");//
            dataTable.Columns.Add("Invoice number");
            dataTable.Columns.Add("Invoice Issue date");
            dataTable.Columns.Add("Invoice Issue time");
            dataTable.Columns.Add("Invoice type code");
            dataTable.Columns.Add("Transaction type");
            dataTable.Columns.Add("Invoice currency");
            dataTable.Columns.Add("Billing reference ID");
            dataTable.Columns.Add("Supply Start Date");
            dataTable.Columns.Add("Supply End Date");
            //dataTable.Columns.Add("Attempted Date");//
            //dataTable.Columns.Add("Attempted Time");//
            //dataTable.Columns.Add("Contract ID");//
            //dataTable.Columns.Add("Invoice note");//
            //dataTable.Columns.Add("Purchase Order Id");

            string QR = "";
            for (int i = 0; i < invoice.AdditionalDocumentReference.Length; i++)
            {
                if (invoice.AdditionalDocumentReference[i].ID.Value.Equals("QR"))
                {
                    QR = invoice.AdditionalDocumentReference[i].Attachment.EmbeddedDocumentBinaryObject.Value;
                }
            }

            dataTable.Rows.Add(invoice.ID.Value, invoice.IssueDate, invoice.IssueTime, invoice.InvoiceTypeCode.Value, invoice.InvoiceTypeCode.name, invoice.DocumentCurrencyCode, invoice?.billingReferenceField?.invoiceDocumentReferenceField?.ID, invoice?.deliveryField?.actualDeliveryDateField, invoice?.deliveryField?.latestDeliveryDateField);

            GridView1.DataSource = dataTable;
            GridView1.DataBind();



            //generate QR 
            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(QR, QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCode qrCode = new QRCoder.QRCode(QrCodeInfo);

            using (Bitmap bitMap = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    imgQR.ImageUrl = QR.Equals("") ? "" : "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }

            }
            imgQR.Visible = (QR.Equals("") == false);
        }

        public void SellerDetails()
        {

            //AccountingSupplierParty

            var dataTable = new DataTable();

            dataTable.Columns.Add("Seller Name");
            dataTable.Columns.Add("VAT/Group VAT Number");
            dataTable.Columns.Add("Other Seller ID");
            dataTable.Columns.Add("Other Seller ID Number");
            dataTable.Columns.Add("Street");
            dataTable.Columns.Add("Building Number");
            //dataTable.Columns.Add("Additional Number");
            dataTable.Columns.Add("City Name");
            dataTable.Columns.Add("Seller Postal Code");
            dataTable.Columns.Add("city sub division name");
            //dataTable.Columns.Add("Neighborhood");
            //dataTable.Columns.Add("City Sub Entity");
            dataTable.Columns.Add("Country Code");



            dataTable.Rows.Add(invoice?.AccountingSupplierParty?.Party?.PartyLegalEntity?.RegistrationName,  //not working
                invoice?.AccountingSupplierParty?.Party?.PartyTaxScheme?.CompanyID,
                               invoice?.AccountingSupplierParty?.Party?.PartyTaxScheme?.CompanyID,
                               invoice?.AccountingSupplierParty?.Party?.PartyTaxScheme?.CompanyID,
                               invoice?.AccountingSupplierParty?.Party?.PartyIdentification?.ID?.Value,
                               invoice?.AccountingSupplierParty?.Party?.PartyIdentification?.ID?.schemeID,
                               invoice?.AccountingSupplierParty?.Party?.PostalAddress?.StreetName,
                               invoice?.AccountingSupplierParty?.Party?.PostalAddress?.BuildingNumber,
                               invoice?.AccountingSupplierParty?.Party?.PostalAddress?.CityName,
                               invoice?.AccountingSupplierParty?.Party?.PostalAddress?.PostalZone,
                               invoice?.AccountingSupplierParty?.Party?.PostalAddress?.CitySubdivisionName,
                               invoice?.AccountingSupplierParty?.Party?.PostalAddress?.Country?.IdentificationCode);

            GridView1.DataSource = dataTable;
            GridView1.DataBind();
        }

        public void BuyerDetails()
        {

            //AccountingCustomerParty

            var dataTable = new DataTable();
            //dataTable.Columns.Add("Seller Name");
            dataTable.Columns.Add("VAT/Group VAT Number");
            dataTable.Columns.Add("Other Buyer ID");
            dataTable.Columns.Add("Other Buyer ID Number");
            dataTable.Columns.Add("Street");
            dataTable.Columns.Add("Building Number");
            //dataTable.Columns.Add("Additional Number");
            dataTable.Columns.Add("City Name");
            dataTable.Columns.Add("Seller Postal Code");
            dataTable.Columns.Add("city sub division name");
            //dataTable.Columns.Add("Neighborhood");
            //dataTable.Columns.Add("City Sub Entity");
            dataTable.Columns.Add("Country Code");



            dataTable.Rows.Add(invoice?.AccountingCustomerParty?.Party?.PartyTaxScheme?.TaxScheme?.ID.Value, 
                               invoice?.AccountingCustomerParty?.Party?.PartyIdentification?.ID?.Value,
                               invoice?.AccountingCustomerParty?.Party?.PartyIdentification?.ID?.schemeID,
                               invoice?.AccountingCustomerParty?.Party?.PostalAddress?.StreetName,
                               invoice?.AccountingCustomerParty?.Party?.PostalAddress?.BuildingNumber,
                               invoice?.AccountingCustomerParty?.Party?.PostalAddress?.CityName,
                               invoice?.AccountingCustomerParty?.Party?.PostalAddress?.PostalZone, 
                               invoice?.AccountingCustomerParty?.Party?.PostalAddress?.CitySubdivisionName,
                               invoice?.AccountingCustomerParty?.Party?.PostalAddress?.Country?.IdentificationCode);

            GridView1.DataSource = dataTable;
            GridView1.DataBind();
        }


        public void AllowanceDocumentLevel()
        {
            //AllowanceDocumentLevel
            var dataTable = new DataTable();
            dataTable.Columns.Add("Allowance Indicator");
            dataTable.Columns.Add("Allowance Amount");
            dataTable.Columns.Add("VAT Rate");
            //dataTable.Columns.Add("Allowance Base Amount");
            dataTable.Columns.Add("VAT Category Code");
            //dataTable.Columns.Add("Allowance Percentage");
            //dataTable.Columns.Add("Tax Scheme ID");

            dataTable.Rows.Add(invoice.allowanceChargeField?.ChargeIndicator, invoice.allowanceChargeField?.Amount, invoice.allowanceChargeField?.TaxCategory?.Percent
                , invoice.allowanceChargeField?.TaxCategory?.ID);


            GridView1.DataSource = dataTable;
            GridView1.DataBind();
        }

        public void LineItems()
        {
            //AllowanceDocumentLevel
            var dataTable = new DataTable();
            dataTable.Columns.Add("Invoice Line Identifier");
            dataTable.Columns.Add("Item Name");
            //dataTable.Columns.Add("Standard Identifier");
            dataTable.Columns.Add("Unit of Measure");
            dataTable.Columns.Add("Quantity");
            dataTable.Columns.Add("Price Amount");
            //dataTable.Columns.Add("Item Price Discount");
            //dataTable.Columns.Add("Item Net Price");
            //dataTable.Columns.Add("Allowance Indicator");
            //dataTable.Columns.Add("Allowance Base Amount");
            //dataTable.Columns.Add("Allowance percentage");
            dataTable.Columns.Add("Allowance Amount");
            dataTable.Columns.Add("Allowance Reason");
            dataTable.Columns.Add("Line Extension Amount");
            dataTable.Columns.Add("VAT Category Code");
            dataTable.Columns.Add("VAT Rate");
            dataTable.Columns.Add("Tax amount");
            //dataTable.Columns.Add("Scheme id");
            dataTable.Columns.Add("Rounding Amount");
            //dataTable.Columns.Add("Buyers Identifier");
            //dataTable.Columns.Add("Sellers Identifier");
            //dataTable.Columns.Add("Base Quantity");
            //dataTable.Columns.Add("UQM");
            //dataTable.Columns.Add("Custom Field 1");
            /*dataTable.Columns.Add("Custom Field 2");
            dataTable.Columns.Add("Custom Field 3");
            dataTable.Columns.Add("Custom Field 4");
            dataTable.Columns.Add("Custom Field 5");
            dataTable.Columns.Add("Custom Field 6");
*/
            for (int i = 0; i < invoice.InvoiceLine.Length; i++)
            {
                dataTable.Rows.Add(invoice?.InvoiceLine[i]?.ID.Value, invoice?.InvoiceLine[i]?.Item?.Name, invoice?.InvoiceLine[i]?.InvoicedQuantity?.unitCode, invoice?.InvoiceLine[i]?.InvoicedQuantity?.Value
                    , invoice?.InvoiceLine[i]?.Price?.PriceAmount.Value, invoice?.InvoiceLine[i]?.Price?.AllowanceCharge?.Amount, invoice?.InvoiceLine[i]?.Price?.AllowanceCharge?.AllowanceChargeReason
                    , invoice?.InvoiceLine[i]?.LineExtensionAmount.Value, invoice?.InvoiceLine[i]?.Item?.ClassifiedTaxCategory?.ID.Value, invoice?.InvoiceLine[i]?.Item?.ClassifiedTaxCategory?.Percent
                    , invoice?.InvoiceLine[i]?.TaxTotal?.TaxAmount.Value, invoice?.InvoiceLine[i]?.TaxTotal?.RoundingAmount.Value);
            }

            GridView1.DataSource = dataTable;
            GridView1.DataBind();
        }

        public void VAT()
        {
            //VAT BreakDown
            var dataTable = new DataTable();
            dataTable.Columns.Add("Taxable Amount");
            dataTable.Columns.Add("Tax Amount");
            dataTable.Columns.Add("VAT Code");
            dataTable.Columns.Add("VAT Rate");
            dataTable.Columns.Add("Exemption code");
            dataTable.Columns.Add("Exemption reason");
            //dataTable.Columns.Add("Scheme Id");

            for (int i = 0; i < invoice.TaxTotal.Length; i++)
            {

                if (invoice.TaxTotal[i].TaxSubtotal != null)
                {
                    for (int j = 0; j < invoice.TaxTotal[i].TaxSubtotal.Length; j++)
                    {
                        dataTable.Rows.Add(invoice?.TaxTotal[i]?.TaxSubtotal[j]?.TaxableAmount.Value, invoice?.TaxTotal[i]?.TaxSubtotal[j]?.TaxAmount.Value,
                            invoice?.TaxTotal[i]?.TaxSubtotal[j]?.TaxCategory?.ID.Value, invoice?.TaxTotal[i]?.TaxSubtotal[j]?.TaxCategory?.taxExemptionReasonCode,
                            invoice?.TaxTotal[i]?.TaxSubtotal[j]?.TaxCategory?.taxExemptionReason);
                    }
                }



            }

            GridView1.DataSource = dataTable;
            GridView1.DataBind();
        }

        public void PayementInfo()
        {
            //Payement Information
            var dataTable = new DataTable();
            dataTable.Columns.Add("Payement Type Code");
            dataTable.Columns.Add("Instruction Note");
            //dataTable.Columns.Add("Reason for CN/DN");
            //dataTable.Columns.Add("Payement Terms");

            dataTable.Rows.Add(invoice?.PaymentMeans?.PaymentMeansCode, invoice?.PaymentMeans?.InstructionNote);

            GridView1.DataSource = dataTable;
            GridView1.DataBind();
        }

        protected void lnk_vewInvoiceSummary_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                ViewInvoiceSummery();
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }

        }

        protected void lnk_SellerDetails_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                SellerDetails();
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }
        }

        protected void lnk_buyerDetails_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                BuyerDetails();
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }
        }



        protected void lnk_documentleveAllowance_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                AllowanceDocumentLevel();
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }
        }

        protected void lnk_lineItems_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                LineItems();
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }
        }

        protected void lnk_VAT_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                VAT();
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }
        }

        protected void lnk_paymentInfo_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                PayementInfo();
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }
        }

        protected void lnk_ViewXML_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                string xml = File.ReadAllText(lblPath.Text);
                TextBox1.Text = xml;
                mp_xml.Show();
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }
        }

        protected void lnk_downloadXML_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";

                if (lblPath.Text != string.Empty)
                {

                    Response.ContentType = ContentType;
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(lblPath.Text));
                    Response.WriteFile((lblPath.Text));
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }
        }
    }
}
