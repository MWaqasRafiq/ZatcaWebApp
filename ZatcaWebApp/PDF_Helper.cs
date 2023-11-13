using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZatcaWebApp
{
    public class PDF_Helper
    {
        public void ConvertPDFA3(string xmlFile)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string folderPath = @"D:\IDOL\Self_Checkout\Testing Printer\OposPrinter\OposPrinter";
            string name = folderPath + @"\PDF_A3.pdf";

            BaseFont baseFont = BaseFont.CreateFont(Path.Combine(folderPath, "CourierRegular.TTF"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            Font font = new Font(baseFont, 12);

            ICC_Profile icc = ICC_Profile.GetInstance(Path.Combine(folderPath, "sRGB_CS_profile.icm"));

            //Opening PDF
            Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 20f, 20f);
            //if(File.Exists(name)) { File.Delete(name); name = "PDF_A3.pdf"; }
            PdfAWriter writer = PdfAWriter.GetInstance(document,
                   new FileStream(name, FileMode.Create),
                   PdfAConformanceLevel.PDF_A_3A);
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(name, FileMode.Create));//, PdfAConformanceLevel.PDF_A_3A);
            //writer.PDFXConformance = 6;
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

            // Add the MarkInfo dictionary to the catalog dictionary.
            param.Put(PdfName.MARKINFO, markInfoDict);

            PdfFileSpecification specification = PdfFileSpecification.FileEmbedded(writer, xmlFile, xmlFile, null, "application/xml", param, 0);
            specification.Put(new PdfName("AFRelationship"), new PdfName("Data"));
            writer.AddFileAttachment("Description for attachment", specification);

            PdfArray array = new PdfArray();
            array.Add(specification.Reference);

            writer.ExtraCatalog.Put(new PdfName("AF"), array);
            writer.ExtraCatalog.Put(PdfName.MARKINFO, markInfoDict);

            // PDF Content
            document.Add(new Paragraph("Test Document for A3 PDF", font));
            document.Close();
            writer.Close();

        }
    }
}