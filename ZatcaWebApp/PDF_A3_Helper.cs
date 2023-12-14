using System;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html.table;
using QRCoder;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;
using System.Drawing.Imaging;

namespace ZatcaWebApp
{
    public class PDF_A3_Helper
    {
        static string logo = @"C:\IDOL\Farmlogo.bmp";
        static iTextSharp.text.Font fontNum = new iTextSharp.text.Font();
        static iTextSharp.text.Font fontLBold = new iTextSharp.text.Font();
        static iTextSharp.text.Font fontAR = new iTextSharp.text.Font();
        static iTextSharp.text.Font fontARBold = new iTextSharp.text.Font();
        static iTextSharp.text.Font fontARLargeBold = new iTextSharp.text.Font();
        public static byte[] QRBitmapArray;

        /// <summary>
        /// Main method for creating PDF A3 and saving that to file
        /// </summary>
        /// <param name="xmlFile"></param>
        public static string ConvertXML2PDFA(string xmlFile)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string folderPath = @"D:\IDOL\Self_Checkout\Testing Printer\OposPrinter\OposPrinter";
            string name = folderPath + @"\files\PDF_A3.pdf";

            //setting up font
            SetupFonts(folderPath);

            ICC_Profile icc = ICC_Profile.GetInstance(Path.Combine(folderPath, "fonts", "sRGB_CS_profile.icm"));

            //Opening PDF
            Document document = new Document(PageSize.A3.Rotate(), 10f, 10f, 20f, 20f);
            //if(File.Exists(name)) { File.Delete(name);}
            PdfAWriter writer = PdfAWriter.GetInstance(document,
                   new FileStream(name, FileMode.Create),
                   PdfAConformanceLevel.PDF_A_3A);
            document.Open();
            writer.CreateXmpMetadata();
            writer.Open();
            writer.SetOutputIntents("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", icc);
            document.AddAuthor("IDOL Technology");
            document.AddTitle("Zatca Invoice");
            writer.SetLanguage("en-GB");

            PdfDictionary param = new PdfDictionary();
            param.Put(PdfName.MODDATE, new PdfDate());
            param.Put(PdfName.TITLE, new PdfString("Attachment Title"));
            PdfDictionary markInfoDict = new PdfDictionary();
            markInfoDict.Put(PdfName.MARKED, new PdfBoolean(true));

            param.Put(PdfName.MARKINFO, markInfoDict);

            //Add XML Attachment
            PdfFileSpecification specification = PdfFileSpecification.FileEmbedded(writer, xmlFile, xmlFile, null, "application/xml", param, 0);
            specification.Put(new PdfName("AFRelationship"), new PdfName("Data"));
            writer.AddFileAttachment("Description for attachment", specification);

            PdfArray array = new PdfArray();
            array.Add(specification.Reference);

            writer.ExtraCatalog.Put(new PdfName("AF"), array);
            writer.ExtraCatalog.Put(PdfName.MARKINFO, markInfoDict);

            var xmlData = ReadXML(xmlFile);

            if (xmlData.InvoiceLine.Count() > 5)
            {
                int pageSize = 5;
                int page = Convert.ToInt32(Math.Round(Convert.ToDecimal(xmlData.InvoiceLine.Count()) / Convert.ToDecimal(pageSize), 0, MidpointRounding.ToEven));

                //Line invoice items
                for (int i = 1; i <= page; i++)
                {
                    var tableSupplier = GetPdfHeadSupplier(xmlData, i, page);
                    var tableCustomer = GetPdfHeadCustomer(xmlData, i, page);
                    var tableInvoiceItem = GetPdfInvoiceLine(xmlData, i, pageSize);
                    document.NewPage();
                    PdfPTable pdfPTable = new PdfPTable(2);
                    pdfPTable.WidthPercentage = 100;
                    pdfPTable.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right
                    pdfPTable.SpacingBefore = 20f;
                    pdfPTable.SpacingAfter = 20f;
                    //Head with logo
                    PdfPCell cell = new PdfPCell(tableSupplier);
                    cell.Border = 0;
                    pdfPTable.AddCell(cell);

                    //Customer info
                    cell = new PdfPCell(tableCustomer);
                    cell.Border = 0;
                    pdfPTable.AddCell(cell);

                    cell = new PdfPCell(tableInvoiceItem);
                    cell.Border = 0;
                    cell.Colspan = 2;
                    pdfPTable.AddCell(cell);

                    if (i == page)
                    {
                        //Footer
                        var tableFooter = GetPdfFooter(xmlData);

                        var dtCount = xmlData.InvoiceLine.Skip((page - 1) * pageSize).Take(pageSize).Count();
                        if (dtCount > 3)
                        {
                            document.Add(pdfPTable);
                            document.NewPage();
                            pdfPTable = new PdfPTable(2);
                            pdfPTable.WidthPercentage = 100;
                            pdfPTable.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right
                            pdfPTable.SpacingBefore = 20f;
                            pdfPTable.SpacingAfter = 20f;
                            //Head with logo
                            cell = new PdfPCell(tableSupplier);
                            cell.Border = 0;
                            pdfPTable.AddCell(cell);

                            //Customer info
                            cell = new PdfPCell(tableCustomer);
                            cell.Border = 0;
                            pdfPTable.AddCell(cell);

                            cell = new PdfPCell(tableFooter);
                            cell.Border = 0;
                            cell.Colspan = 2;
                            pdfPTable.AddCell(cell);
                            document.Add(pdfPTable);
                        }
                        else
                        {
                            cell = new PdfPCell(tableFooter);
                            cell.Border = 0;
                            cell.Colspan = 2;
                            pdfPTable.AddCell(cell);
                            document.Add(pdfPTable);
                        }
                    }
                    else
                    {
                        document.Add(pdfPTable);
                    }
                }
            }
            else
            {
                var tableSupplier = GetPdfHeadSupplier(xmlData, 1, 1);
                var tableCustomer = GetPdfHeadCustomer(xmlData, 1, 1);
                var tableFooter = GetPdfFooter(xmlData);
                PdfPTable pdfPTable = new PdfPTable(2);
                pdfPTable.WidthPercentage = 100;
                pdfPTable.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right
                pdfPTable.SpacingBefore = 20f;
                pdfPTable.SpacingAfter = 20f;

                //Head with logo
                PdfPCell cell = new PdfPCell(tableSupplier);
                cell.Border = 0;
                pdfPTable.AddCell(cell);

                //Customer info
                cell = new PdfPCell(tableCustomer);
                cell.Border = 0;
                pdfPTable.AddCell(cell);

                //Footer 
                cell = new PdfPCell(tableFooter);
                cell.Border = 0;
                cell.Colspan = 2;
                pdfPTable.AddCell(cell);

                document.Add(pdfPTable);
            }

            document.Close();
            writer.Close();
            return name;
        }

        /// <summary>
        /// Tabular data for Suplier Head section
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        private static PdfPTable GetPdfHeadSupplier(Invoice xmlData, int currentPage, int totalPages)
        {
            // PDF Content
            PdfPTable table = new PdfPTable(3);
            PdfPCell cell = new PdfPCell();
            Phrase ph = new Phrase();
            string data = string.Empty;
            table.WidthPercentage = 50;
            table.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 0f;
            table.SpacingAfter = 0f;

            //-------------------------Table For Image, QR & Supplier Name-------------------------
            PdfPTable tableImage = new PdfPTable(2);
            tableImage.WidthPercentage = 100;
            tableImage.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right
            tableImage.SpacingBefore = 0f;
            tableImage.SpacingAfter = 10f;

            //-------------------------Logo-------------------------
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(logo);
            image.ScaleAbsolute(190f, 65f);
            cell = new PdfPCell(image);
            cell.Border = 0;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.PaddingLeft = 30f;
            tableImage.AddCell(cell);

            //-------------------------QR Code-------------------------
            //xmlData.AdditionalDocumentReference[2].Attachment.EmbeddedDocumentBinaryObject.Value, QRCodeGenerator.ECCLevel.Q);
            if ((QRBitmapArray == null || QRBitmapArray.Length <= 0) && xmlData.AdditionalDocumentReference.Length > 0)
            {
                data = xmlData.AdditionalDocumentReference[2].Attachment != null ? xmlData.AdditionalDocumentReference[2].Attachment.EmbeddedDocumentBinaryObject.Value : "";
                if (!string.IsNullOrEmpty(data))
                {
                    QRCodeGenerator QrGenerator = new QRCodeGenerator();
                    QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                    QRCoder.QRCode QrCode = new QRCoder.QRCode(QrCodeInfo);
                    Bitmap QrBitmap = QrCode.GetGraphic(10);
                    QRBitmapArray = QrBitmap.BitmapToByteArray();
                }
            }
            if (QRBitmapArray != null && QRBitmapArray.Length > 0)
            {
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(QRBitmapArray);
                img.ScaleAbsolute(100f, 100f);
                cell = new PdfPCell(img);
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.NoWrap = true;
                tableImage.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell();
                cell.Border = 0;
                tableImage.AddCell(cell);
            }


            //-------------------------Supplier Name-------------------------
            ph = new Phrase(xmlData.AccountingSupplierParty.Party.PartyLegalEntity.RegistrationName, fontARLargeBold);//new iTextSharp.text.Font(BaseFont.CreateFont(Path.Combine(folderPath, "NotoSansArabic-Regular.TTF"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 11));//FontFactory.GetFont("Arial", 15, Font.NORMAL, BaseColor.PINK));
            cell = new PdfPCell(ph);
            cell.Border = 0;
            cell.Colspan = 2;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.ExtraParagraphSpace = 15f;
            cell.PaddingLeft = 20f;
            tableImage.AddCell(cell);

            cell = new PdfPCell(tableImage);
            cell.Border = 0;
            cell.Colspan = 3;
            cell.ExtraParagraphSpace = 15f;
            table.AddCell(cell);

            //-------------------------Invoice Number-------------------------
            ph = new Phrase(1f, "Invoice#(IRN): ", fontLBold);
            cell = new PdfPCell(ph);
            cell.Border = 0;
            cell.ExtraParagraphSpace = 2f;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell);

            data = xmlData.ID != null ? xmlData.ID.Value : "";
            ph = new Phrase(1f, data, fontLBold);
            cell = new PdfPCell(ph);
            cell.Border = 0;
            cell.Colspan = 2;
            cell.ExtraParagraphSpace = 2f;
            table.AddCell(cell);

            //-------------------------Invoice Date-------------------------
            ph = new Phrase(1f, "Invoice Date: ", fontLBold);
            cell = new PdfPCell(ph);
            cell.Border = 0;
            cell.ExtraParagraphSpace = 5f;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell);

            data = Convert.ToDateTime(xmlData.IssueDate).ToString("dd-MMM-yyyy") + " " + xmlData.IssueTime;
            ph = new Phrase(1f, data, fontLBold);
            cell = new PdfPCell(ph);
            cell.Border = 0;
            cell.Colspan = 2;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);

            //Supplier Information
            HeadTableSupplier(ref table, xmlData, currentPage, totalPages);

            return table;
        }

        /// <summary>
        /// Tabular data for Suplier detailed info
        /// </summary>
        /// <param name="table"></param>
        /// <param name="xmlData"></param>
        private static void HeadTableSupplier(ref PdfPTable table, Invoice xmlData, int currentPage, int totalPages)
        {
            string data = string.Empty;
            var cell = new PdfPCell();
            PdfPTable table1 = new PdfPTable(3);
            table1.WidthPercentage = 100;
            table1.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right
            table1.SpacingBefore = 0f;
            table1.SpacingAfter = 0f;

            string telephoneNo = "(xxx) xxxxxxxxxx";
            string emailAddress = "xxxxx@xxxxx.xxx";

            //-------------------------Telephone, Fax-------------------------
            data = "Tel: " + telephoneNo;
            data += " Dammam(HO): " + telephoneNo;
            data += " – Fax: " + telephoneNo;
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.ExtraParagraphSpace = 3f;
            cell.Colspan = 3;
            table1.AddCell(cell);

            //-------------------------Tex/Fax, Tx/Fax-------------------------
            data = "Riyadh Tex/Fax: " + telephoneNo + "  Jeddah Tx/Fax: " + telephoneNo;
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.ExtraParagraphSpace = 3f;
            table1.AddCell(cell);

            //-------------------------Vat No-------------------------
            data = "VAT No: ";
            data += xmlData.AccountingSupplierParty.Party.PartyTaxScheme != null ? xmlData.AccountingSupplierParty.Party.PartyTaxScheme.CompanyID : "";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.ExtraParagraphSpace = 3f;
            table1.AddCell(cell);

            //-------------------------Page No-------------------------
            data = "Page " + currentPage.ToString() + " of " + totalPages.ToString();
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table1.AddCell(cell);

            //-------------------------C.R.No-------------------------
            data = "C.R.No: ";
            data += xmlData.AccountingSupplierParty.Party.PartyIdentification.ID != null ? xmlData.AccountingSupplierParty.Party.PartyIdentification.ID.Value : "";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.ExtraParagraphSpace = 5f;
            table1.AddCell(cell);

            //-------------------------Sales Order No----------------------------
            data = "Sales Order#";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = 0;
            cell.ExtraParagraphSpace = 5f;
            table1.AddCell(cell);

            data = "xxxxxxxxxxx";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = 0;
            cell.ExtraParagraphSpace = 3f;
            table1.AddCell(cell);

            data = "امر البيع";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.ExtraParagraphSpace = 3f;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            table1.AddCell(cell);

            //-------------------------Address-------------------------
            data = string.Empty;
            if (xmlData.AccountingSupplierParty.Party.PostalAddress != null)
            {
                data = xmlData.AccountingSupplierParty.Party.PostalAddress.StreetName;
                data += ",bld# " + xmlData.AccountingSupplierParty.Party.PostalAddress.BuildingNumber;
                data += ", " + xmlData.AccountingSupplierParty.Party.PostalAddress.PlotIdentification;
                data += ", " + xmlData.AccountingSupplierParty.Party.PostalAddress.CitySubdivisionName;
                data += ", " + xmlData.AccountingSupplierParty.Party.PostalAddress.CityName;
                data += ",Zip# " + xmlData.AccountingSupplierParty.Party.PostalAddress.PostalZone;
            }

            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.Colspan = 3;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.ExtraParagraphSpace = 3f;
            table1.AddCell(cell);

            //------------------------Shipping Location-----------------------------
            data = "Store Name";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = 0;
            cell.ExtraParagraphSpace = 3f;
            table1.AddCell(cell);

            data = "xxxxxxxxxxxxxxxx";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            table1.AddCell(cell);

            data = "اسم المتجر";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            table1.AddCell(cell);
            //-------------------------Email----------------------------
            data = "Email";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = 0;
            cell.ExtraParagraphSpace = 3f;
            table1.AddCell(cell);

            data = emailAddress;
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            table1.AddCell(cell);

            data = "عنوان البريد الإلكتروني";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            table1.AddCell(cell);
            //-------------------------Mobile----------------------------
            data = "Mobile No";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = 0;
            cell.ExtraParagraphSpace = 3f;
            table1.AddCell(cell);

            data = telephoneNo;
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            table1.AddCell(cell);

            data = "رقم الهاتف المحمول";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            table1.AddCell(cell);
            //-----------------------Delivery Note------------------------------
            data = "Delivery Note#";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = 0;
            cell.ExtraParagraphSpace = 3f;
            table1.AddCell(cell);

            data = "xxxxxxxxxx";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            table1.AddCell(cell);

            data = "سند تسليم";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            table1.AddCell(cell);
            //-----------------------Sales Rep------------------------------
            data = "Sales Rep Name";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = 0;
            cell.ExtraParagraphSpace = 3f;
            table1.AddCell(cell);

            data = "xxxxxxxxxxxxxxx";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            table1.AddCell(cell);

            data = "اسم المندوب";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            table1.AddCell(cell);
            //-----------------------Sales Rep Tel------------------------------
            data = "Sales Rep Telephone";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = 0;
            cell.ExtraParagraphSpace = 3f;
            table1.AddCell(cell);

            data = telephoneNo;
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            table1.AddCell(cell);

            data = "رقم تليفون المندوب";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            table1.AddCell(cell);

            //-----------------------------------------------------
            // add inner table to head table
            cell = new PdfPCell(table1);
            cell.Border = 0;
            cell.Colspan = 3;
            table.AddCell(cell);
        }

        /// <summary>
        /// Tabular data for Customer Head section
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        private static PdfPTable GetPdfHeadCustomer(Invoice xmlData, int currentPage, int totalPages)
        {
            // PDF Content
            PdfPTable table = new PdfPTable(4);
            table.WidthPercentage = 50;
            table.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 0f;
            table.SpacingAfter = 0f;

            PdfPCell cell = new PdfPCell();
            Phrase ph = new Phrase();
            string data = string.Empty;
            cell.Border = 0;
            string IRN = xmlData.ID != null ? xmlData.ID.Value : "";
            string storeNo = string.Empty;
            string termNo = string.Empty;
            string transNo = string.Empty;
            if (!string.IsNullOrEmpty(IRN) && IRN.Length == 22)
            {
                storeNo = IRN.Substring(0, 5);
                termNo = IRN.Substring(15, 3);
                transNo = IRN.Substring(18, 4);
            }
            //---------------------------Terminal No--------------------------
            data = "Terminal No : ";
            cell = new PdfPCell(new Phrase(2f, data, fontARBold));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.ExtraParagraphSpace = 3f;
            table.AddCell(cell);

            data = termNo;
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.ExtraParagraphSpace = 3f;
            table.AddCell(cell);

            data = "رقم المحطة";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.ExtraParagraphSpace = 3f;
            table.AddCell(cell);
            //---------------------------Transaction No--------------------------
            data = "Transaction No : ";
            cell = new PdfPCell(new Phrase(2f, data, fontARBold));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);

            data = transNo;
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);

            data = "رقم التحويلة";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            //---------------------------Operator No--------------------------
            data = "Operator No : ";
            cell = new PdfPCell(new Phrase(2f, data, fontARBold));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.ExtraParagraphSpace = 20f;
            table.AddCell(cell);

            data = "xxxxxxxxx";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.ExtraParagraphSpace = 20f;
            table.AddCell(cell);

            data = "رقم المشغل";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.ExtraParagraphSpace = 20f;
            table.AddCell(cell);
            //---------------------------Customer Name--------------------------
            data = "Customer Name : ";
            cell = new PdfPCell(new Phrase(2f, data, fontARBold));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);

            data = xmlData.AccountingCustomerParty.Party.PartyLegalEntity != null ? xmlData.AccountingCustomerParty.Party.PartyLegalEntity.RegistrationName : "";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);

            data = "اسم العميل";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            //--------------------------Customer No---------------------------
            data = "Customer No : ";
            cell = new PdfPCell(new Phrase(2f, data, fontARBold));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);

            data = xmlData.AccountingCustomerParty.Party.PartyIdentification != null ? xmlData.AccountingCustomerParty.Party.PartyIdentification.ID.Value : "";
            cell = new PdfPCell(new Phrase(data, fontNum));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);

            data = "رقم العميل";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            //---------------------------Payment Terms--------------------------
            data = "Payment Terms : ";
            cell = new PdfPCell(new Phrase(2f, data, fontARBold));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);

            data = "Immediate";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);

            data = "شروط الدفع";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            //--------------------------Customer VAT#---------------------------
            data = "Customer VAT# : ";
            cell = new PdfPCell(new Phrase(2f, data, fontARBold));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);

            data = xmlData.AccountingCustomerParty.Party.PartyTaxScheme.TaxScheme.ID != null ? xmlData.AccountingCustomerParty.Party.PartyTaxScheme.TaxScheme.ID.Value : "";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);

            data = "رقم ضريبة القيمة المضافة للعميل";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            //-------------------------Customer C.R.No----------------------------
            data = "Customer C.R.No : ";
            cell = new PdfPCell(new Phrase(2f, data, fontARBold));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);

            data = xmlData.AccountingCustomerParty.Party.PartyIdentification != null ? xmlData.AccountingCustomerParty.Party.PartyIdentification.ID.Value : "";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);

            data = "رقم السجل التجاري";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);

            //-----------------------------------------------------
            cell = new PdfPCell();
            cell.Border = 0;
            table.AddCell(cell);

            data = "Customer Site Address :";
            cell = new PdfPCell(new Phrase(2f, data, fontARBold));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.ExtraParagraphSpace = 3f;
            table.AddCell(cell);

            data = "عنوان الفاتورة";
            cell = new PdfPCell(new Phrase(data, fontAR));
            cell.Border = 0;
            cell.ExtraParagraphSpace = 3f;
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            table.AddCell(cell);

            //-----------------------------------------------------
            cell = new PdfPCell();
            cell.Border = 0;
            table.AddCell(cell);

            data = string.Empty;
            if (xmlData.AccountingCustomerParty.Party.PostalAddress != null)
            {
                data += xmlData.AccountingCustomerParty.Party.PostalAddress.StreetName;
                data += ", " + xmlData.AccountingCustomerParty.Party.PostalAddress.BuildingNumber;
                data += ", " + xmlData.AccountingCustomerParty.Party.PostalAddress.CitySubdivisionName;
                data += ", " + xmlData.AccountingCustomerParty.Party.PostalAddress.CityName;
            }

            cell = new PdfPCell(new Phrase(2f, data, fontAR));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Border = 0;
            cell.ExtraParagraphSpace = 8f;
            cell.Colspan = 3;
            table.AddCell(cell);
            //-----------------------------------------------------
            cell = new PdfPCell();
            cell.Border = 0;
            table.AddCell(cell);

            data = "Bld# ";
            if (xmlData.AccountingCustomerParty.Party.PostalAddress != null)
            {
                data += xmlData.AccountingCustomerParty.Party.PostalAddress.BuildingNumber;
            }

            cell = new PdfPCell(new Phrase(2f, data, fontAR));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Border = 0;
            cell.ExtraParagraphSpace = 5f;
            cell.Colspan = 3;
            table.AddCell(cell);
            //-----------------------------------------------------
            data = xmlData.DocumentCurrencyCode;
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = 0;
            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            table.AddCell(cell);

            data = "Zip# ";
            if (xmlData.AccountingCustomerParty.Party.PostalAddress != null)
            {
                data += xmlData.AccountingCustomerParty.Party.PostalAddress.PostalZone;
            }
            cell = new PdfPCell(new Phrase(2f, data, fontAR));
            cell.Border = 0;
            cell.Colspan = 3;
            table.AddCell(cell);

            //-----------------------------------------------------
            return table;
        }

        /// <summary>
        /// Tabular data for Customer Head section
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        private static PdfPTable GetPdfInvoiceLine(Invoice xmlData, int page, int pageSize)
        {
            // PDF Content
            PdfPTable table = new PdfPTable(11);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 10f;
            table.SpacingAfter = 0f;
            table.SetWidths(new int[] { 1, 2, 3, 1, 1, 1, 1, 1, 1, 1, 1 });

            PdfPCell cell = new PdfPCell();
            Phrase ph = new Phrase();
            string data = string.Empty;
            cell.Border = 0;
            #region head
            //-----------------------------------------------------
            data = "تسلسل";
            data += "\r\n\r\n S.No";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            //-----------------------------------------------------
            data = "رقم الصنف";
            data += "\r\n\r\n Item Code";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            //-----------------------------------------------------
            data = "البيان";
            data += "\r\n\r\n Description";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            //-----------------------------------------------------
            data = "الكمية";
            data += "\r\n\r\n Qty";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            //-----------------------------------------------------
            data = "سعر الوحدة";
            data += "\r\n\r\n Unit Price";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            //-----------------------------------------------------
            data = "القيمة الاجمالية";
            data += "\r\n\r\n Gross Value";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            //-----------------------------------------------------
            data = "الخصم";
            data += "\r\n\r\n Discount";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            //-----------------------------------------------------
            data = "صافي القيمة";
            data += "\r\n\r\n Net Value";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            //-----------------------------------------------------
            data = "نسبةالضريبة";
            data += "\r\n\r\n %VAT";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            //-----------------------------------------------------
            data = "قيمة الضريبة";
            data += "\r\n\r\n VAT Amount";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            //-----------------------------------------------------
            data = "الاجمالي";
            data += "\r\n\r\n Total";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER | PdfPCell.RIGHT_BORDER;
            cell.ExtraParagraphSpace = 5f;
            table.AddCell(cell);
            #endregion
            //-----------------Line Items--------------------------
            var dt = xmlData.InvoiceLine.Skip((page - 1) * pageSize).Take(pageSize);
            int count = dt.Count();
            foreach (var item in dt)
            {
                count--;
                var unitPrice = item.Price.PriceAmount != null ? item.Price.PriceAmount.Value : "0.00";
                var grossValue = item.LineExtensionAmount != null ? item.LineExtensionAmount.Value : "0.00";
                var vatValue = item.TaxTotal != null ? item.TaxTotal.TaxAmount != null ? item.TaxTotal.TaxAmount.Value : "0.00" : "0";
                string total = string.Empty;
                if (item.TaxTotal != null && item.TaxTotal.RoundingAmount != null)
                {
                    total = item.TaxTotal.RoundingAmount.Value;
                }
                else
                {
                    total = grossValue;
                }

                //S.No
                cell = new PdfPCell(new Phrase(item.ID != null ? item.ID.Value : "", fontAR));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.Border = count == 0 ? PdfPCell.BOTTOM_BORDER | PdfPCell.LEFT_BORDER : PdfPCell.LEFT_BORDER;
                cell.MinimumHeight = 45f;
                cell.ExtraParagraphSpace = 3f;
                table.AddCell(cell);

                //Item Code
                cell = new PdfPCell(new Phrase(item.Item != null ? item.Item.Name : "", fontAR));
                cell.Border = count == 0 ? PdfPCell.BOTTOM_BORDER : 0;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.MinimumHeight = 45f;
                cell.ExtraParagraphSpace = 3f;
                table.AddCell(cell);

                //Description
                cell = new PdfPCell(new Phrase(item.Item != null ? item.Item.Name : "", fontAR));
                cell.Border = count == 0 ? PdfPCell.BOTTOM_BORDER : 0;
                cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.MinimumHeight = 45f;
                cell.ExtraParagraphSpace = 3f;
                table.AddCell(cell);

                //QTY
                cell = new PdfPCell(new Phrase(item.InvoicedQuantity != null ? item.InvoicedQuantity.Value : "0", fontNum));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = count == 0 ? PdfPCell.BOTTOM_BORDER : 0;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.MinimumHeight = 45f;
                cell.ExtraParagraphSpace = 3f;
                table.AddCell(cell);

                //Unit Price
                cell = new PdfPCell(new Phrase(unitPrice, fontNum));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Border = count == 0 ? PdfPCell.BOTTOM_BORDER : 0;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.MinimumHeight = 45f;
                cell.ExtraParagraphSpace = 3f;
                table.AddCell(cell);

                //Gross Value
                cell = new PdfPCell(new Phrase(grossValue, fontNum));
                cell.Border = count == 0 ? PdfPCell.BOTTOM_BORDER : 0;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.MinimumHeight = 45f;
                cell.ExtraParagraphSpace = 3f;
                table.AddCell(cell);

                //Discount
                cell = new PdfPCell(new Phrase("0.00", fontNum));
                cell.Border = count == 0 ? PdfPCell.BOTTOM_BORDER : 0;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.MinimumHeight = 45f;
                cell.ExtraParagraphSpace = 3f;
                table.AddCell(cell);

                //Net Value
                cell = new PdfPCell(new Phrase(grossValue, fontNum));
                cell.Border = count == 0 ? PdfPCell.BOTTOM_BORDER : 0;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.MinimumHeight = 45f;
                cell.ExtraParagraphSpace = 3f;
                table.AddCell(cell);

                //VAT%
                cell = new PdfPCell(new Phrase(item.Item != null ? item.Item.ClassifiedTaxCategory != null ? item.Item.ClassifiedTaxCategory.Percent : "0" : "0", fontNum));
                cell.Border = count == 0 ? PdfPCell.BOTTOM_BORDER : 0;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.MinimumHeight = 45f;
                cell.ExtraParagraphSpace = 3f;
                table.AddCell(cell);

                //VAT Amount
                cell = new PdfPCell(new Phrase(vatValue, fontNum));
                cell.Border = count == 0 ? PdfPCell.BOTTOM_BORDER : 0;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.MinimumHeight = 45f;
                cell.ExtraParagraphSpace = 3f;
                table.AddCell(cell);

                //Total
                cell = new PdfPCell(new Phrase(total, fontNum));
                cell.Border = count == 0 ? PdfPCell.BOTTOM_BORDER | PdfPCell.RIGHT_BORDER : PdfPCell.RIGHT_BORDER;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.MinimumHeight = 45f;
                cell.ExtraParagraphSpace = 3f;
                table.AddCell(cell);
            }
            return table;
        }

        /// <summary>
        /// Set Footer last page data
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        private static PdfPTable GetPdfFooter(Invoice xmlData)
        {
            // PDF Content
            PdfPTable table = new PdfPTable(3);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 10f;
            table.SpacingAfter = 0f;

            PdfPTable tableInfo = new PdfPTable(1);
            tableInfo.WidthPercentage = 30;
            tableInfo.HorizontalAlignment = 0;
            tableInfo.SpacingBefore = 0f;
            tableInfo.SpacingAfter = 0f;

            PdfPTable tableSign = new PdfPTable(1);
            tableSign.WidthPercentage = 30;
            tableSign.HorizontalAlignment = 1;
            tableSign.SpacingBefore = 0f;
            tableSign.SpacingAfter = 0f;

            PdfPTable tableSales = new PdfPTable(3);
            tableSales.WidthPercentage = 30;
            tableSales.HorizontalAlignment = 2;
            tableSales.SpacingBefore = 0f;
            tableSales.SpacingAfter = 0f;

            PdfPCell cell = new PdfPCell();
            Phrase ph = new Phrase();
            string data = string.Empty;

            //--------------------table Info-------------------------
            data = "ONLY";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableInfo.AddCell(cell);

            data = "Signatures";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableInfo.AddCell(cell);

            data = "We Confirm that we have received the above products in good condition";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
            cell.ExtraParagraphSpace = 20f;
            tableInfo.AddCell(cell);

            data = "Receiver Name and Signature";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableInfo.AddCell(cell);

            //--------------------table Sign-------------------------
            data = "ZATCA Authorized Signatory :";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
            cell.ExtraParagraphSpace = 30f;
            tableSign.AddCell(cell);

            data = "Salesman :";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableSign.AddCell(cell);

            data = "Sales Manager :";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableSign.AddCell(cell);

            data = "Store Keeper :";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableSign.AddCell(cell);

            //--------------------table Sales-------------------------

            //--------- ITEM--------
            data = "Gross Sales";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableSales.AddCell(cell);

            data = "اجمالي المبيعات";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.Border = PdfPCell.TOP_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableSales.AddCell(cell);

            data = xmlData.LegalMonetaryTotal.LineExtensionAmount.Value;
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.RIGHT_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.ExtraParagraphSpace = 5f;
            tableSales.AddCell(cell);

            //--------- ITEM --------
            data = "Discount";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.LEFT_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableSales.AddCell(cell);

            data = "الخصم";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.Border = PdfPCell.NO_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableSales.AddCell(cell);

            data = "0.00";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.RIGHT_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.ExtraParagraphSpace = 5f;
            tableSales.AddCell(cell);

            //--------- ITEM --------
            data = "Net Sales";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.LEFT_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableSales.AddCell(cell);

            data = "صافي المبيعات";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.Border = PdfPCell.NO_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableSales.AddCell(cell);

            data = xmlData.LegalMonetaryTotal.LineExtensionAmount.Value;
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.RIGHT_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.ExtraParagraphSpace = 5f;
            tableSales.AddCell(cell);

            //--------- ITEM --------
            data = "VAT SAR";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.LEFT_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableSales.AddCell(cell);

            data = "قيمة المضافة";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.Border = PdfPCell.NO_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableSales.AddCell(cell);

            data = xmlData.TaxTotal[0].TaxAmount.Value;
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.RIGHT_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.ExtraParagraphSpace = 5f;
            tableSales.AddCell(cell);
            //--------- ITEM --------
            data = "Total";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.LEFT_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableSales.AddCell(cell);

            data = "اجمالي مبلغ الفاتورة";
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            cell.Border = PdfPCell.BOTTOM_BORDER;
            cell.ExtraParagraphSpace = 5f;
            tableSales.AddCell(cell);

            data = xmlData.LegalMonetaryTotal.PayableAmount.Value;
            cell = new PdfPCell(new Phrase(data, fontARBold));
            cell.Border = PdfPCell.BOTTOM_BORDER | PdfPCell.RIGHT_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.ExtraParagraphSpace = 5f;
            tableSales.AddCell(cell);

            //----------------------------------------------------
            cell = new PdfPCell(tableInfo);
            cell.Border = 0;// PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
            cell.Padding = 3f;
            table.AddCell(cell);

            cell = new PdfPCell(tableSign);
            cell.Border = 0;// PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
            cell.Padding = 3f;
            table.AddCell(cell);

            cell = new PdfPCell(tableSales);
            cell.Border = 0;// PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER;
            cell.Padding = 3f;
            table.AddCell(cell);
            //-----------------------------------------------------
            return table;
        }

        /// <summary>
        /// Read XML file into Invoice
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        private static Invoice ReadXML(string xmlPath = "")
        {
            Invoice invoice = new Invoice();
            if (File.Exists(xmlPath))
            {
                var xmlSerializer = new XmlSerializer(typeof(Invoice));
                using (var reader = new StreamReader(xmlPath))
                {
                    invoice = (Invoice)xmlSerializer.Deserialize(reader);
                }
            }
            return invoice;
        }

        /// <summary>
        /// Setting up the fonts for PDF
        /// </summary>
        /// <param name="folderPath"></param>
        private static void SetupFonts(string folderPath)
        {
            folderPath += @"\fonts";
            BaseFont baseFont3 = BaseFont.CreateFont(Path.Combine(folderPath, "Courier_2.TTF"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            fontLBold = new iTextSharp.text.Font(baseFont3, 14);
            fontNum = new iTextSharp.text.Font(baseFont3, 12);

            BaseFont bf = BaseFont.CreateFont(Path.Combine(folderPath, "HelveticaNeueLT-Arabic-75.TTF"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            fontAR = new iTextSharp.text.Font(bf, 12);

            bf = BaseFont.CreateFont(Path.Combine(folderPath, "HelveticaNeueLT-Arabic-75-Bold.TTF"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            fontARBold = new iTextSharp.text.Font(bf, 12);
            fontARLargeBold = new iTextSharp.text.Font(bf, 15);
        }
    }
    public static class BitmapExtension
    {
        public static byte[] BitmapToByteArray(this Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}