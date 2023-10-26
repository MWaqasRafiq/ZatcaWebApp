

using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using SDKNETFrameWorkLib.BLL;
using SDKNETFrameWorkLib.GeneralLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using ZatcaWebApp.signDLL;

namespace ZatcaWebApp
{

    public class CertificateData
    {
        public string RawDataString { get; set; }
        public byte[] RawDataByte { get; set; }
        public string Certificate_issuer_name { get; set; }
        public string X509SerialNumber { get; set; }
        public string PublicKey { get; set; }
        public string privateKeytext { get; set; }
    }


    class util
    {

        public static string NS_XMLNS = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
        public static string NS_CBC = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        public static string NS_CAC = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
        public static string NS_EXT = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";


        public static CertificateData GetCertData()
        {
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert = new X509Certificate2(Basepage.cert_pem_path, Basepage.private_key_path,
                            X509KeyStorageFlags.MachineKeySet
                          | X509KeyStorageFlags.PersistKeySet);

            string certificate = Convert.ToBase64String(cert.RawData);

            CertificateData cdata = new CertificateData();
            cdata.RawDataString = certificate;
            cdata.Certificate_issuer_name = cert.IssuerName.ToString();
            cdata.X509SerialNumber = cert.SerialNumber;
            cdata.RawDataByte = cert.RawData;
            cdata.PublicKey = cert.PublicKey.ToString();

            string ppp = File.ReadAllText(Basepage.private_key_path);
            cdata.privateKeytext = ppp;

            return cdata;

        }
        public void logWrite(string msg, LOGTYPE lg)
        {
            string format = "ddMMyyyy";
            string filePath = Basepage.LocalFolder + lg.ToString() + DateTime.Now.ToString(format) + ".log";
            string version = "1.3.2.0";
            if (!File.Exists(filePath))
            {
                StreamWriter log;
                log = new StreamWriter(filePath);
                //var version = Assembly.GetExecutingAssembly().GetName().Version;

                log.WriteLine("[" + version.ToString() + "] " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss tt") + "==> " + msg);
                log.Close();
            }
            else
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter StreamWriters = new StreamWriter(fs))
                    {
                        StreamWriters.WriteLine("[" + version.ToString() + "] " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss tt") + "==> " + msg);
                        StreamWriters.Close();
                    }
                }


            }
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static XmlNamespaceManager get_nsmanager(XmlDocument xmlDocument)
        {

            var nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("d", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
            nsmgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

            nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            nsmgr.AddNamespace("xades", "http://uri.etsi.org/01903/v1.3.2#");
            nsmgr.AddNamespace("sac", "urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2");
            nsmgr.AddNamespace("sig", "urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2");
            nsmgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            nsmgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");

 
            return nsmgr;
        }
        public static string getXMLNodeValue(string xpath, string xmlFilePath)
        {
            //string xpath = @"//d:Invoice/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent/sig:UBLDocumentSignatures/sac:SignatureInformation/ds:Signature/ds:SignedInfo/ds:Reference[@URI='#xadesSignedProperties']/ds:DigestValue";



            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            xmlDocument.Load(xmlFilePath);

            XmlNamespaceManager nsmgr = get_nsmanager(xmlDocument);
            XmlNode el = xmlDocument.SelectSingleNode(xpath, nsmgr);


            if (el != null)
            {
                return el.InnerText;
            }
            else throw new InvalidCastException("el_DigestValue_0 empty");

        }

        
        public   string[] SignXML_Compliance(string xmlFilePath, string XML_filename, string compliance_cert,string privateKeytext)
        {

            XmlDocument myDoc = new XmlDocument();

            if (Path.GetFileName(xmlFilePath).StartsWith("B2B"))
            {
                string xml = File.ReadAllText(xmlFilePath, Encoding.UTF8);
                //if B2B, dont sign , only get invoice hash
                var _IHashingValidator = new SDKNETFrameWorkLib.BLL.HashingValidator();
                SDKNETFrameWorkLib.GeneralLogic.Result res = _IHashingValidator.GenerateEInvoiceHashing(xmlFilePath);

                return new string[] { xml, res.ResultedValue };
            }
            else
            {
                //string xslpath = Basepage.cert_pem_path.Replace(Path.GetFileName(Basepage.cert_pem_path), "invoice.xsl");
                //string tttt = ApplyXSLT(xmlFilePath, xslpath);

                // string xml=File.ReadAllText(xmlFilePath, Encoding.UTF8);
                //  xml = xml.Replace("CommonExtensionComponents-2\">\r\n  <cbc:ProfileID>", "CommonExtensionComponents-2\">\r\n  \r\n  <cbc:ProfileID>");
                //File.WriteAllText(xmlFilePath, xml);
                //signing the invoice
                // CertificateData certData = GetCertData();

                new Basepage().logWrite("compliance_cert:" + compliance_cert, LOGTYPE.CMPL);
                new Basepage().logWrite("privateKeytext:" + privateKeytext, LOGTYPE.CMPL);
               // compliance_cert = "MIIB8TCCAZigAwIBAgIGAYex+V5vMAoGCCqGSM49BAMCMBUxEzARBgNVBAMMCmVJbnZvaWNpbmcwHhcNMjMwNDI0MDYzNTE3WhcNMjgwNDIzMjEwMDAwWjBJMQswCQYDVQQGEwJTQTEPMA0GA1UECwwGUml5YWRoMQ8wDQYDVQQKDAZSRVRBSUwxGDAWBgNVBAMMD0VBZmFybUNsb3VkU2VydjBWMBAGByqGSM49AgEGBSuBBAAKA0IABGpFMbf5ZfUhsIItmZeNKYKfhHHL8wJbHj3l459So4A9hFixu3qhjhzKBt5lUYoEWUmvyMyP29tfSVUenyEFAVWjgaIwgZ8wDAYDVR0TAQH/BAIwADCBjgYDVR0RBIGGMIGDpIGAMH4xKjAoBgNVBAQMITEtSURPTHwyLUVHU0Nsb3VkU2VydnwzLTEyMzQ1Njc4OTEfMB0GCgmSJomT8ixkAQEMDzMwMDQ2NTM5NTUwMDAwMzENMAsGA1UEDAwEMTEwMDEPMA0GA1UEGgwGUml5YWRoMQ8wDQYDVQQPDAZSRVRBSUwwCgYIKoZIzj0EAwIDRwAwRAIgdssb4snaw7zseNulZ1I1bAc59b04V2hvlrPUKJ3VH04CIAK/C/3LgeiNmhvNaYEMVE9V4KMFgYEHPIKK0qL5CBJz";
               // privateKeytext = "MHQCAQEEIAbiibeov/kZrV2MKr+98p+8ssmC6X9q82MS0IkaA5EooAcGBSuBBAAKoUQDQgAEakUxt/ll9SGwgi2Zl40pgp+EccvzAlsePeXjn1KjgD2EWLG7eqGOHMoG3mVRigRZSa/IzI/b219JVR6fIQUBVQ==";

                ZatcaWebApp.signDLL.Result rrrr1 = new ZatcaWebApp.signDLL.signdll().SignDocument(xmlFilePath, compliance_cert, privateKeytext);



                //   EInvoiceSigningLogic eee = new EInvoiceSigningLogic();
               // SDKNETFrameWorkLib.GeneralLogic.Result rrrr1 = eee.SignDocument(xmlFilePath, compliance_cert, privateKeytext);

                 new Basepage().logWrite("IsValid:" + rrrr1.IsValid, LOGTYPE.CMPL);
                 new Basepage().logWrite("rrrr1:ErrorMessage" + rrrr1.ErrorMessage, LOGTYPE.CMPL);
                 //new Basepage().logWrite("rrrr1.ResultedValue:" + rrrr1.ResultedValue, LOGTYPE.CMPL);
                //string[] arr = Sign_ROOPA(xmlFilePath, compliance_cert, privateKeytext);

                // string invoiceHash = arr[0];
                // string xmlSigned = arr[1];

                for (int i = 0; i < rrrr1.lstSteps.Count; i++)
                {
                    new Basepage().log(rrrr1.lstSteps[i].Operation + ":" + rrrr1.lstSteps[i].IsValid, 1, LOGTYPE.CMPL); 
                }

                string fname = "Signed" + "_" + DateTime.Now.ToString("yyMMddHHmmss") + "_" + Path.GetFileNameWithoutExtension(XML_filename);
                 // fname = fname.ToLower().Replace(".xml", ".txt");
                new Basepage().logWrite("Saving  signed invoice file:" + fname, LOGTYPE.CMPL);

                string folder = Path.GetDirectoryName(xmlFilePath) + @"\signed\";
                string fpath = SaveFileNormal(rrrr1.ResultedValue, fname, folder, LOGTYPE.CMPL);
                // string fpath = SaveFileNormal(xmlSigned, fname, folder, LOGTYPE.CMPL);



                //string folder = Path.GetDirectoryName(xmlFilePath) + @"\signed\"+ fname+".xml";
                //var utf8WithoutBom = new System.Text.UTF8Encoding(false);
                //File.WriteAllText(folder, rrrr1.ResultedValue, utf8WithoutBom);

               // var _IHashingValidator = new signDLL.HashingValidator();
               // signDLL.Result res1 = _IHashingValidator.GenerateEInvoiceHashing(xmlFilePath);

                 
              //  signDLL.Result res = _IHashingValidator.GenerateEInvoiceHashing(fpath);

                // fpath = SetFields_to_XML(fpath, res.ResultedValue, "", LOGTYPE.CMPL);  //setting InvoiceHash

                //var _IQRValidator = new SDKNETFrameWorkLib.BLL.QRValidator();
                //Result res1 = _IQRValidator.GenerateEInvoiceQRCode(fpath);

                // fpath = SetFields_to_XML(fpath, "", res1.ResultedValue,  LOGTYPE.CMPL);  //setting QR

                //  new Basepage().logWrite("_IHashingValidator isValid:" + res.IsValid, LOGTYPE.CMPL);

                string[] arr = Get_Signed_Fields_XML(fpath, LOGTYPE.CMPL);
               // string[] arr = Get_Signed_Fields_XML(fpath, LOGTYPE.CMPL);
                // string xml = File.ReadAllText(xmlFilePath, Encoding.UTF8); 
                // return new string[] { xml, arr[1] };
                //return new string[] { rrrr1.ResultedValue, res.ResultedValue};
                //return new string[] { rrrr1.ResultedValue, arr[1] };
                //return new string[] { rrrr1.ResultedValue, arr[1] };
               // string xml = rrrr1.ResultedValue;//.Replace("<cac:AccountingSupplierParty>", "  <cac:AccountingSupplierParty>");
                return new string[] { rrrr1.ResultedValue, arr[1] };
            }
        }
        public string[] Sign_ROOPA(string xmlFilePath, string certificateContent, string privateKeyContent)
        {
            string invoiceHash = "";
            string digitalSignature = "";
            string certificateHash = "";

            sbyte[] array1;
            string xml = File.ReadAllText(xmlFilePath, Encoding.UTF8);
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                //invoice hash
                XmlDsigC14NTransform dsigC14Ntransform = new XmlDsigC14NTransform(false);
                dsigC14Ntransform.LoadInput((object)memoryStream);
                 array1 = ((IEnumerable<byte>)Utility.Sha256_hashAsBytes(Encoding.UTF8.GetString((dsigC14Ntransform.GetOutput() as MemoryStream).ToArray()))).Select<byte, sbyte>((Func<byte, sbyte>)(x => (sbyte)x)).ToArray<sbyte>();
                invoiceHash = Utility.ToBase64Encode((byte[])(Array)array1);
            }

                //digitalSignature
                signDLL.Result ress = new signdll().GetDigitalSignature(invoiceHash, privateKeyContent);
                digitalSignature = ress.ResultedValue;



                //certificate hash
                byte[] arr = Encoding.UTF8.GetBytes(certificateContent);
                X509Certificate2 x509Cert = new X509Certificate2((byte[])(Array)arr.Select<byte, sbyte>((Func<byte, sbyte>)(x => (sbyte)x)).ToArray<sbyte>());
                Org.BouncyCastle.X509.X509Certificate x509Certificate = DotNetUtilities.FromX509Certificate((System.Security.Cryptography.X509Certificates.X509Certificate)x509Cert);
                sbyte[] array = ((IEnumerable<byte>)SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(x509Certificate.GetPublicKey()).GetEncoded()).Select<byte, sbyte>((Func<byte, sbyte>)(x => (sbyte)x)).ToArray<sbyte>();
                byte[] arrwww = x509Cert.GetSerialNumber();
                BigInteger bigInteger = new BigInteger((byte[])(Array)arrwww.Select<byte, sbyte>((Func<byte, sbyte>)(x => (sbyte)x)).ToArray<sbyte>());
                if (x509Cert != null)
                { 
                        certificateHash = Utility.ToBase64Encode(Utility.Sha256_hashAsString(certificateContent));
                     
                }


            //signed properties
            XmlDocument xmlDocument1 = new XmlDocument();
            xmlDocument1.PreserveWhitespace = true; 
                xmlDocument1.Load(xmlFilePath);
            signDLL.Result result7 = new signDLL.signdll(). TransformXML(xmlDocument1.OuterXml);

            XmlDocument xmlDocument2 = new XmlDocument();
            xmlDocument2.PreserveWhitespace = true;
            xmlDocument2.LoadXml(result7.ResultedValue);
            Dictionary<string, string> nameSpacesMap =new signdll().getNameSpacesMap();
            signDLL.Result result8 = new signDLL.Result();
            signDLL.Result result9 = new signdll().PopulateSignedSignatureProperties(xmlDocument2, nameSpacesMap, certificateHash, new signdll().GetCurrentTimestamp(), x509Cert.IssuerName.Name, bigInteger.ToString());


            //QR

            new signdll().PopulateQRCode(xmlDocument2, array1, digitalSignature, invoiceHash, ((IEnumerable<byte>)x509Certificate.GetSignature()).Select<byte, sbyte>((Func<byte, sbyte>)(x => (sbyte)x)).ToArray<sbyte>());
            return new string[] { invoiceHash, xmlDocument2.OuterXml };
        }

        public static string ApplyXSLT(string xmlFilePath, string xsltFilePath)
        {
            StringBuilder output = new StringBuilder();
            using (XmlWriter results = XmlWriter.Create(output, new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8,
                Indent = false
            }))
            { 
                XmlReader stylesheet = XmlReader.Create(xsltFilePath);
                XslCompiledTransform compiledTransform = new XslCompiledTransform();
                compiledTransform.Load(stylesheet);
                compiledTransform.Transform(xmlFilePath, results);
            }
            return output.ToString();
        }
        

        private static string  SetFields_to_XML(string xmlFilepath, string invoiceHash,string QR, LOGTYPE TYP)
        {
             
            XmlNameTable table = null;
            XDocument xDoc = null;

            if (xDoc == null)
            {
                using (var sr = new StreamReader(xmlFilepath))
                {
                    XmlTextReader reader = new XmlTextReader(xmlFilepath);
                    xDoc = XDocument.Load(reader);
                    table = reader.NameTable;
                    reader.Close();
                    reader.Dispose();

                }
            }


            var nsmgr = new XmlNamespaceManager(table);
            nsmgr.AddNamespace("d", NS_XMLNS);
            nsmgr.AddNamespace("cbc", NS_CBC);
            nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            nsmgr.AddNamespace("xades", "http://uri.etsi.org/01903/v1.3.2#");
            nsmgr.AddNamespace("sac", "urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2");
            nsmgr.AddNamespace("sig", "urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2");
            nsmgr.AddNamespace("ext", NS_EXT);
            nsmgr.AddNamespace("cac", NS_CAC);

            if (QR.Equals("") == false)
            {
                
                xDoc.XPathSelectElement("*[local-name()='Invoice']//*[local-name()='AdditionalDocumentReference'][cbc:ID[text()='QR']]//*[local-name()='Attachment']", nsmgr).Value = QR;
            }
                //
                if (invoiceHash.Equals( "")==false)
            {
                xDoc.XPathSelectElement("*[local-name()='Invoice']//*[local-name()='UBLExtensions']//*[local-name()='UBLExtension']//*[local-name()='ExtensionContent']//*[local-name()='UBLDocumentSignatures']//*[local-name()='SignatureInformation']//*[local-name()='Signature']//*[local-name()='SignedInfo']//ds:Reference[@Id='invoiceSignedData']//*[local-name()='DigestValue']", nsmgr).Value = invoiceHash;

            }


            XmlWriterSettings xws = new XmlWriterSettings { OmitXmlDeclaration = true };
            using (XmlWriter xw = XmlWriter.Create(xmlFilepath, xws))
                xDoc.Save(xw); 

            return xmlFilepath;

        }


        private static string[] Get_Signed_Fields_XML(string xmlFilepath, LOGTYPE TYP )
        {

            XmlNameTable table = null;
            XDocument xDoc = null;

            if (xDoc == null)
            {
                using (var sr = new StreamReader(xmlFilepath))
                {
                    XmlTextReader reader = new XmlTextReader(xmlFilepath);
                    xDoc = XDocument.Load(reader);
                    table = reader.NameTable;
                    reader.Close();
                    reader.Dispose();

                }
            }




            var nsmgr = new XmlNamespaceManager(table);
            nsmgr.AddNamespace("d", NS_XMLNS);
            nsmgr.AddNamespace("cbc", NS_CBC);
            nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            nsmgr.AddNamespace("xades", "http://uri.etsi.org/01903/v1.3.2#");
            nsmgr.AddNamespace("sac", "urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2");
            nsmgr.AddNamespace("sig", "urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2");
            nsmgr.AddNamespace("ext", NS_EXT);
            nsmgr.AddNamespace("cac", NS_CAC);


            string QR = xDoc.XPathSelectElement("*[local-name()='Invoice']//*[local-name()='AdditionalDocumentReference'][cbc:ID[text()='QR']]//*[local-name()='Attachment']", nsmgr).Value.Trim();
            string InvoicHashEncoded = xDoc.XPathSelectElement("*[local-name()='Invoice']//*[local-name()='UBLExtensions']//*[local-name()='UBLExtension']//*[local-name()='ExtensionContent']//*[local-name()='UBLDocumentSignatures']//*[local-name()='SignatureInformation']//*[local-name()='Signature']//*[local-name()='SignedInfo']//ds:Reference[@Id='invoiceSignedData']//*[local-name()='DigestValue']", nsmgr).Value;

            string taxtotal = xDoc.XPathSelectElement("*[local-name()='Invoice']//*[local-name()='TaxTotal']//cbc:TaxAmount[@currencyID='SAR']", nsmgr).Value.Trim();

            return new string[] { QR, InvoicHashEncoded, taxtotal };
          

        }


        public static string SaveFileNormal(string xml, string filename, string folderpath, LOGTYPE lgtype)
        {

            try
            {
                var _IHashingValidator = new SDKNETFrameWorkLib.BLL.HashingValidator();
                var _IQRValidator = new SDKNETFrameWorkLib.BLL.QRValidator();
                var _IEInvoiceValidator = new EInvoiceValidator();
                var _IEInvoiceSigningLogic = new EInvoiceSigningLogic();


                new Basepage().logWrite("abc", lgtype);
                string extension = ".xml";

                string filePath1 = folderpath + filename + extension;

                string ff = filePath1.Replace(extension, ".tmp");
                //// Create a new file     
                //using (FileStream fs = File.Create(ff))
                //{
                //    Byte[] title = new UTF8Encoding(true).GetBytes(xml);
                //    fs.Write(title, 0, title.Length);

                //}

                new Basepage().logWrite("abc1", lgtype);
                xml = xml.Replace("<!-- Please note that the signature values are sample values only -->", "");
               // xml = Regex.Replace(xml, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);

                new Basepage().logWrite("abc2", lgtype);
                File.WriteAllText(ff, xml);
                string tt = "file created. fileName:" + Path.GetFileName(ff);
                new Basepage().logWrite(tt, lgtype);

                System.IO.File.Move(ff, filePath1);

                tt = "file renamed from .tmp to " + extension;
                new Basepage().logWrite(tt, lgtype);


                if (Basepage.logDebug == 1)
                {
                    SDKNETFrameWorkLib.GeneralLogic.Result res = _IQRValidator.ValidateEInvoiceQRCode(filePath1);
                    new Basepage().logWrite("QrCodeValidator isValid:" + res.IsValid, lgtype);

                    res = _IHashingValidator.ValidateEInvoiceHashing(filePath1);
                    new Basepage().logWrite("HashingValidator isValid:" + res.IsValid, lgtype);

                }


                return filePath1;


            }
            catch (Exception ex)
            {
                string msg = "saveFiletoPOS error:" + ex.ToString();
                new Basepage().logWrite(msg, lgtype);
                return "";

            }

        }


    }
}
