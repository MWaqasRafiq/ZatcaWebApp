using ExtensionMethod;
using GatewayService;
using java.lang.reflect;
using java.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SDKNETFrameWorkLib.BLL;
using SDKNETFrameWorkLib.GeneralLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using static net.sf.saxon.functions.Minimax;
using static ZATCA.BaseData;

namespace ZATCA
{
    public enum InvoiceType
    {
        STD_INVOICE,
        STD_CN,
        STD_DN,
        SIMP_INVOICE,
        SIMP_CN,
        SIMP_DN
    }
    public class CertificateData
    {
        public string RawDataString { get; set; }
        public string RawDataStringB64 { get; set; }
        public byte[] RawDataByte { get; set; }
        public string Certificate_issuer_name { get; set; }
        public string X509SerialNumber { get; set; }
        public string PublicKey { get; set; }
        public string privateKeytext { get; set; }
        public string secret { get; set; }
    }
    public class invoice
    {
        Basepage bp = new Basepage();
        Validators _validators = new Validators();

        HashingValidator _IHashingValidator = new HashingValidator();
        QRValidator _IQRValidator = new QRValidator();
        EInvoiceValidator _IEInvoiceValidator = new EInvoiceValidator();


        public static string NS_XMLNS = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
        public static string NS_CBC = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        public static string NS_CAC = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
        public static string NS_EXT = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";

        internal DataReturn Process(string sss, DOCTYPE documentType, LOGTYPE TYP, SOURCE src)
        {
            //when a b2c trx comes, generate xml, sign it,save the signed xml and send the QR in the response,
            //when a b2b trx comes, generate the xml, save it, and send for clearance to zatca,
            //if successfully cleared, QR will be returned from zatca, send that QR to the client.

            DataReturn RET = new DataReturn();
            BaseData.Root DATA = JsonConvert.DeserializeObject<BaseData.Root>(sss);


            string[] arr1 = CheckInvoiceQR_Exists("", DATA, TYP);
            string qr = arr1[0];
            string irn = arr1[1];
            string uuid1 = arr1[2];
            string invHash = arr1[3];
            if (qr != "")
            {
                RET = new DataReturn()
                {
                    QR = qr,
                    InvReferenceNumber = irn,
                    UUID = uuid1,
                    DocumentType = documentType,
                    InvoiceHash = invHash,
                    RESPMSG = "SUCCESS",
                    Warnings = new List<string>() { "QR already generated for the specified IRN." },
                    RC = 0,
                    change_icv_pih = false
                };
            }
            else
            {

                //check documentType= document type in the DATA
                if (documentType != DATA.Invoice.documentType)
                {
                    bp.log(documentType + " " + DATA.Invoice.documentType, 1, TYP);
                    throw new InvalidCastException(ERR.custerr_Document_type_mismatch.ToString());
                }
                if (DATA.Invoice.documentType == DOCTYPE.B2B && DATA.Invoice.EInvoice.InvoiceTypeCode.name.StartsWith("01") == false)
                {
                    bp.log(documentType + " " + DATA.Invoice.EInvoice.InvoiceTypeCode.name + "mismatch", 1, TYP);
                    throw new InvalidCastException("document type and InvoiceTypeCode.name are mismatching");
                }
                if (DATA.Invoice.documentType == DOCTYPE.B2C && DATA.Invoice.EInvoice.InvoiceTypeCode.name.StartsWith("02") == false)
                {
                    bp.log(documentType + " " + DATA.Invoice.EInvoice.InvoiceTypeCode.name + "mismatch", 1, TYP);
                    throw new InvalidCastException("document type and InvoiceTypeCode.name are in mismatch");
                }





                //setting UUID
                string uuid = System.Guid.NewGuid().ToString();
                DATA.Invoice.InvoiceUUID = uuid;


                //Setting ICV,PIH
                // List<string> arr = readIcvPih_fromFile(TYP);
                List<string> result = new List<string>();
                string myLine;
                bp.log("Reading ICV_PIH file:", 1, TYP);
                FileStream myFileStream;
                while (true)
                {
                    try
                    {
                        myFileStream = new FileStream(Basepage.ICV_PIH_file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(20);  //if the file is used by any other process, it waits for 20ms and tries again
                    }
                }
                StreamReader myStreamReader = new StreamReader(myFileStream);
                while ((myLine = myStreamReader.ReadLine()) != null)
                {
                    result.Add(myLine.Substring(4));
                }
                bp.log("ICV:" + result[0], 1, LOGTYPE.COUNTER);
                bp.log("PIH:" + result[1], 1, LOGTYPE.COUNTER);

                bp.log("ICV_PIH:" + result[0] + " " + result[1], 1, TYP);

                if (result[0] == null || result[0].Equals("")) throw new InvalidCastException("ICV error.Try again");
                if (result[1] == null || result[1].Equals("")) throw new InvalidCastException("PIH error.Try again");

                DATA.Invoice.ICV = result[0];
                DATA.Invoice.PIH = result[1];

                FileNameFields ff = new FileNameFields()
                {
                    partyTaxSchemeCompanyId = DATA.Invoice.EInvoice.AccountingSupplierParty.Party.PartyTaxScheme.CompanyID,
                    irn = DATA.Invoice.EInvoice.ID.en,
                    IssueDate_YYYY_MM_DD = DATA.Invoice.EInvoice.IssueDate,
                    IssueTime_HH_MM_SS = DATA.Invoice.EInvoice.IssueTime
                };

                //  if partyTaxSchemeCompanyId is not Send_to_Clearance_B2B in the API, set from code
                //if validation error is there, still generate xml, but put the xml in a different folder,
                //otherwise , upon resending the same invoice, xml file will be overriden bcz of same file name,
                //also no need to try for reporting or clearance if there is validation error.

                string error = _validators.Do_Validations(DATA, TYP);
                Generated_OUT gen = generateXml(DATA, TYP, ff, error, src);
                string xmlFilePath = gen.xmlFilePath;


                if (!error.Equals("")) throw new InvalidCastException(error);


                if (documentType == DOCTYPE.B2C)
                {

                    RET = SignXML(xmlFilePath, ff, TYP, src, documentType);
                }
                else   //B2B
                {

                    RET = Send_to_Clearance_B2B(xmlFilePath, ff, TYP, uuid, src);

                }

                string invType = DATA.Invoice.EInvoice.InvoiceTypeCode.value;
                RET.InvoiceType = Basepage.getInvoiceTypeCode(invType);


                if (RET.change_icv_pih == true)
                {
                    string PIHnew = RET.InvoiceHash;
                    int icv = int.Parse(result[0]);
                    icv = icv + 1;
                    // writeIcvPih_File(icv, PIHnew, TYP);

                    bp.log("writing ICV_PIH:icv=" + icv + ";PIH:" + PIHnew, 1, TYP);
                    StreamWriter myStreamWriter = new StreamWriter(myFileStream);
                    myFileStream.Seek(0, SeekOrigin.Begin);
                    myFileStream.SetLength(0);

                    myStreamWriter.WriteLine("ICV=" + icv);
                    myStreamWriter.WriteLine("PIH=" + PIHnew);
                    myStreamWriter.Close();
                }

                myStreamReader.Close();
                myFileStream.Close();
                myFileStream.Dispose();

                if (RET.QR != null && !"".Equals(RET.QR))
                {
                    SaveQR(RET.InvReferenceNumber, RET.QR, uuid, TYP);

                    RET.UUID = uuid;
                    RET.RESPMSG = "SUCCESS";
                    RET.RC = 0;

                }
                else
                {
                    RET.QR = "";
                    RET.RESPMSG = RET.RESPMSG == "" ? "FAILED" : RET.RESPMSG;
                    RET.RC = 1;
                }
                RET.TaxTotal = gen.TaxTotal;
                RET.InvoiceTotal = gen.InvoiceTotal;

                string fname = Path.GetFileName(xmlFilePath);


                DB_UPDAte(RET, DATA.Invoice.EInvoice.IssueDate, DATA.Invoice.EInvoice.IssueTime, DATA.Invoice.EInvoice.ID.arb, DATA.Invoice.InvoiceUUID, INVOICE_OPERATION.INSERT_INVOICE, src, TYP, fname, documentType, 1, PROCESS_TYPE.API);
            }
            return RET;


        }


        internal DataReturn Process_POS(string xmlFilePath, DOCTYPE documentType, LOGTYPE TYP, SOURCE src)
        {
            //B2C
            //WHEN an xml comes from pos,(Local server) the xml file is saved, opened and updated ICV,PIH,UUID
            //then sign the xml ,save it, send the QR to POS.
            //no database for this in the local server.

            //B2B
            //WHEN an xml comes from pos controller to HO Server, the xml file is saved, opened and updated ICV,PIH,UUID and sent directly to ZATCA for clearance
            //QR is obrained from zatca response.

            //This function is also used to sign the xml that is uploaded from the central server portal page.

            //if same request comes with same IRN, from pos, then check if QR already generated for that trx, if yes send that trx's QR
            //this is reqired bcz if pos send a request, xml is generated on store server, but failed to send this to controller,
            //and if arun sends the request again for the same trx, for one trx multiple invoices will be generated on zatca side.
            //to avoid this.

            DataReturn RET;


            string[] arr = CheckInvoiceQR_Exists(xmlFilePath, null, TYP);
            string qr = arr[0];
            string irn = arr[1];
            string uuid1 = arr[2];
            string invHash = arr[3];
            if (qr != "")
            {
                bp.log("QR already generated for the specified IRN." + xmlFilePath, 1, TYP);

                RET = new DataReturn()
                {
                    QR = qr,
                    InvReferenceNumber = irn,
                    UUID = uuid1,
                    DocumentType = documentType,
                    InvoiceHash = invHash,
                    RESPMSG = "SUCCESS",
                    Warnings = new List<string>() { "QR already generated for the specified IRN." },
                    RC = 0,
                    change_icv_pih = false
                };
            }
            else
            {
                //List<string> result = readIcvPih_fromFile(TYP);
                List<string> result = new List<string>();
                string myLine;
                bp.log("Reading ICV_PIH file:", 1, TYP);
                FileStream myFileStream;
                while (true)
                {
                    try
                    {
                        myFileStream = new FileStream(Basepage.ICV_PIH_file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(20);  //if the file is used by any other process, it waits for 20ms and tries again
                    }
                }
                StreamReader myStreamReader = new StreamReader(myFileStream);
                while ((myLine = myStreamReader.ReadLine()) != null)
                {
                    result.Add(myLine.Substring(4));
                }
                bp.log("ICV:" + result[0], 1, LOGTYPE.COUNTER);
                bp.log("PIH:" + result[1], 1, LOGTYPE.COUNTER);

                bp.log("ICV_PIH:" + result[0] + " " + result[1], 1, TYP);

                if (result[0] == null || result[0].Equals("")) throw new InvalidCastException("ICV error.Try again");
                if (result[1] == null || result[1].Equals("")) throw new InvalidCastException("PIH error.Try again");


                string ICV = result[0];
                string pih = result[1];

                string invType = Basepage.getXMLNodeValue("d:Invoice/cbc:InvoiceTypeCode", xmlFilePath);    //0200000388
                invType = invType.Substring(7, 3);


                string uuid = Guid.NewGuid().ToString();

                FileNameFields ffff = SetFields_to_XML_NEW(xmlFilePath, ICV, pih, uuid, TYP);

                if (documentType == DOCTYPE.B2B)
                {
                    RET = Send_to_Clearance_B2B(xmlFilePath, ffff, TYP, uuid, src);
                }
                else
                {
                    RET = SignXML(xmlFilePath, ffff, TYP, src, documentType);
                }

                RET.DocumentType = documentType;
                RET.InvoiceType = Basepage.getInvoiceTypeCode(invType);



                bp.log("change_icv_pih:" + RET.change_icv_pih, 1, TYP);
                if (RET.change_icv_pih == true)
                {
                    string PIHnew = RET.InvoiceHash;
                    int icv = int.Parse(ICV);
                    icv = icv + 1;
                    // writeIcvPih_File(icv, PIHnew, TYP);
                    bp.log("writing ICV_PIH:icv=" + icv + ";PIH:" + PIHnew, 1, TYP);
                    StreamWriter myStreamWriter = new StreamWriter(myFileStream);
                    myFileStream.Seek(0, SeekOrigin.Begin);
                    myFileStream.SetLength(0);

                    myStreamWriter.WriteLine("ICV=" + icv);
                    myStreamWriter.WriteLine("PIH=" + PIHnew);

                    myStreamWriter.Close();
                }
                bp.log("done:" + RET.change_icv_pih, 1, TYP);

                myStreamReader.Close();
                myFileStream.Close();
                myFileStream.Dispose();


                if (RET.QR != null && !"".Equals(RET.QR))
                {
                    SaveQR(RET.InvReferenceNumber, RET.QR, uuid, TYP);


                    RET.UUID = uuid;
                    RET.RESPMSG = "SUCCESS";
                    RET.RC = 0;
                }
                else
                {
                    RET.QR = "";
                    RET.RESPMSG = RET.RESPMSG == "" ? "FAILED" : RET.RESPMSG;
                    RET.RC = 1;
                }

                string fname = Path.GetFileName(xmlFilePath);

                if (Basepage.is_localServer == "NO")  //do db update if central server.for store server, sign xml and save only.later it will be taken to cloud server by FTP process.
                    DB_UPDAte(RET, ffff.IssueDate_YYYY_MM_DD, ffff.IssueTime_HH_MM_SS, ffff.irn, uuid, INVOICE_OPERATION.INSERT_INVOICE, src, TYP, fname, documentType, 1, PROCESS_TYPE.SYSTEM);

            }


            return RET;
        }

        public void SaveQR(string invReferenceNumber, string qrCode, string uuid, LOGTYPE typ)
        {

            //saving file with name IRN and content=>uuid and QR
            //in case for a transaction, if xml generated and saved, while sending to pos, gets pipe error and 
            //pos could not get the QR, in that case , 

            bp.log("Saving QR:" + invReferenceNumber, 1, typ);


            string dir = Basepage.invoiceXML_folder + "QR";
            string filePath1 = dir + @"\" + invReferenceNumber + ".txt";



            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            //  string path = Path.Combine(dir, "Test.txt");
            if (!File.Exists(filePath1))
            {
                StreamWriter str;
                str = new StreamWriter(filePath1);

                str.WriteLine(uuid);
                str.WriteLine(qrCode);
                str.Close();

                bp.log("File saved:" + filePath1, 1, typ);
            }
            else if (File.Exists(filePath1))
                bp.log("Critical error on  SaveQR::Filepath already exists.filePath: " + filePath1 + " invReferenceNumber:" + invReferenceNumber + " uuid:" + uuid + " qrCode:" + qrCode, 1, typ);


        }


        public string[] CheckInvoiceQR_Exists(string xmlFilePath, BaseData.Root bd, LOGTYPE lg)
        {
            string QR = "";


            bp.log("Checking if QR already generated:" + xmlFilePath, 1, lg);

            string uuid;
            string uuid_from_qr_file = "";
            string irn;
            string InvoicHashEncoded = "";

            if (!xmlFilePath.Equals(""))
            {
                uuid = Basepage.getXMLNodeValue("d:Invoice/cbc:UUID", xmlFilePath);
                irn = Basepage.getXMLNodeValue("d:Invoice/cbc:ID", xmlFilePath);



                try
                {  //loads only if the xml is signed
                    InvoicHashEncoded = Basepage.getXMLNodeValue("*[local-name()='Invoice']//*[local-name()='UBLExtensions']//*[local-name()='UBLExtension']//*[local-name()='ExtensionContent']//*[local-name()='UBLDocumentSignatures']//*[local-name()='SignatureInformation']//*[local-name()='Signature']//*[local-name()='SignedInfo']//ds:Reference[@Id='invoiceSignedData']//*[local-name()='DigestValue']", xmlFilePath);
                }
                catch (Exception ex) { }

            }
            else
            {
                uuid = bd.Invoice.InvoiceUUID;
                irn = bd.Invoice.EInvoice.ID.en.ToString();
                InvoicHashEncoded = "";
            }

            string filePath1 = Basepage.invoiceXML_folder + @"QR\" + irn + ".txt";

            bp.log("Qr path:" + filePath1, 1, lg);

            if (File.Exists(filePath1))
            {
                bp.log("Qr exists:" + filePath1, 1, lg);

                string[] content = File.ReadAllLines(filePath1);
                uuid_from_qr_file = content[0];
                QR = content[1];

                if (uuid.Equals("0") == false)
                {
                    //UUID comes as 0 from POS for same invoice for every retry, so if an invoice already generated QR,with UUID "x", but failed to deliver response to 
                    //POS, when the same invoice comes in retry mechanism with uuid value as 0, 
                    //then it shouldnt check below condition, because here the uuid in txt file is "x" and
                    //uuid coming from file is 0.
                    //example scenario: Farm zatca prod api log for 0002423101911141060085 on 19 oct 2023.
                    //same is applicable for an invoice coming thru api, uuid will be always empty or null in api

                    //may be this condition needs to be removed ??? because we dont care the uuid coming thru api, we always override it.

                    //removed below condition, since whatever be the UUID, if an IRN that already signed/cleared comes again in a request, the response is sent from file.
                    // if (uuid_from_qr_file.Equals(uuid) == false)  //will be always equal
                    //     throw new InvalidCastException("UUID different for same IRN.uuid from xml file:" + uuid + " UUID saved in txt file:" + uuid_from_qr_file);

                }
            }
            else
            {  //for invoices that are signed from pos, and reported from cloud server, but in case if the user download the invoice from portal and try to upload,
               //it will be signed from cloud server and sent as another invoice with different civ and pih
               //to avoid this, check also if the invoice is available in DB and its status is Reported/cleared
                if (Basepage.is_localServer.Equals("NO"))
                {
                    string qq = "select * from invoices where IRN='" + irn + "' and ActionStatus in ('Reported','Cleared')";
                    DataTable dt = bp.getDataDBQuery(qq, false, lg);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        //invoice is already reported/cleared

                        string doctype = dt.Rows[0]["DocType"].ToString();
                        string ActionStatus = dt.Rows[0]["ActionStatus"].ToString();
                        string Source = dt.Rows[0]["Source"].ToString();
                        string XMLFileName = dt.Rows[0]["XMLFileName"].ToString();

                        bp.log("invoice status in DB is :" + ActionStatus+"  IRN:"+irn, 1, lg);


                        if (xmlFilePath.Equals(""))
                        {
                            //find xmlpath 

                            Enum.TryParse(doctype, out DOCTYPE _doctype);
                            Enum.TryParse(Source, out SOURCE _source);
                            Enum.TryParse(ActionStatus, out ACTION_STATUS _ActionStatus);
                            XMLSTATUS _xmlstatus = getXML_STATUS(_doctype, _ActionStatus);

                            string folder = Get_Folder(_doctype, _source, _xmlstatus, XMLFileName);   //eg: C:\IDOL\ZATCA\varno\files\invoiceXML\EXTERNAL\B2C\FAILED

                            xmlFilePath = folder + XMLFileName;
                        }
                        DataReturn dddd = Get_Signed_Fields_XML(xmlFilePath, lg, new DataReturn());
                        QR = dddd.QR;
                        uuid_from_qr_file = dddd.UUID;
                        InvoicHashEncoded = dddd.InvoiceHash;

                    }

                }

            }

            return new string[] { QR, irn, uuid_from_qr_file, InvoicHashEncoded };


        }
        
        internal ZATCAReturnAPI GetZatcaResponse(string sss, DOCTYPE ddd, LOGTYPE lg, SOURCE eXTERNAL)
        {
            var data = (JObject)JsonConvert.DeserializeObject(sss);
            string IRN = data["IRN"]?.Value<string>();


            string qq = @"select  a.FullResponse,i.DocType,i.TaxTotal,i.UUID,a.CreatedDate from  (select * from Invoices where IRN='" + IRN.Trim() + @"') i
                              left   join  [Actions] a on     i.IRN=a.IRN and (a.Action='REPORTING' or a.Action='CLEARANCE')
                          order by a.CreatedDate desc";
            DataTable dt = bp.getDataDBQuery(qq, false, lg);


            if (dt != null && dt.Rows.Count > 0)
            {
                string xmlpath = getXMLPath_from_IRN(IRN.Trim(), lg);
                bp.log("xmlpath:" + xmlpath, 1, lg);
                Enum.TryParse(dt.Rows[0]["DocType"].ToString(), out DOCTYPE docc);

                if (docc != ddd) throw new InvalidCastException("Document type mismatch");

                DataReturn rrrr = Get_Signed_Fields_XML(xmlpath, lg, new DataReturn());

                ZATCAReturnAPI RET = new ZATCAReturnAPI()
                {
                    QR = rrrr.QR,
                    InvReferenceNumber = IRN,
                    UUID = dt.Rows[0]["UUID"].ToString(),
                    DocType = docc.ToString(),
                    InvoiceHash = rrrr.InvoiceHash,
                    TaxTotal = rrrr.TaxTotal,
                    RESPMSG = "SUCCESS",
                    FullResponse = dt.Rows[0]["FullResponse"].ToString(),
                    RC = 0
                };

                return RET;
            }
            else
            {
                ZATCAReturnAPI RET = new ZATCAReturnAPI()
                {
                    QR = "",
                    InvReferenceNumber = IRN,
                    UUID = "",
                    DocType = ddd.ToString(),
                    InvoiceHash = "",
                    TaxTotal = "",
                    RESPMSG = "Invoice not found",
                    FullResponse = "",
                    RC = 0
                };
                return RET;
            }



        }

        internal ReturnB2BCustomer getB2BCustomer_from_DB(string sss, DOCTYPE ddd, LOGTYPE lg, SOURCE eXTERNAL)
        {
            var data = (JObject)JsonConvert.DeserializeObject(sss);
            string VATNO = data["VATNO"]?.Value<string>();

            bp.log("getting customer data from db for vat:" + VATNO, 1, lg);

            string qq = @"select * from Customers where PartyTaxSchemeCompanyID='" + VATNO.Trim() + "'";
            DataTable dt = bp.getDataDBQuery(qq, false, Basepage.ConStrCustomerVAT, lg);


            if (dt != null && dt.Rows.Count > 0)
            {
                bp.log("customer data found in db for vat:" + VATNO, 1, lg);


                ReturnB2BCustomer RET = new ReturnB2BCustomer()
                {
                    RegistrationNameArabic = dt.Rows[0]["RegistrationNameArabic"].ToString(),
                    RegistrationNameEng = dt.Rows[0]["RegistrationName"].ToString(),
                    CustomerVATNo = VATNO,
                    ResponseMessage = "Data found",
                    ResponseCode = 0

                };

                return RET;
            }
            else
            {
                bp.log("customer data NOT found in db for vat:" + VATNO, 1, lg);

                ReturnB2BCustomer RET = new ReturnB2BCustomer()
                {
                    RegistrationNameArabic = "",
                    RegistrationNameEng = "",
                    CustomerVATNo = "",
                    ResponseMessage = "No data found in DB",
                    ResponseCode = 1

                };
                return RET;
            }



        }


        //private string[] CheckInvoiceQR_Exists(string xmlFilePath, BaseData.Root bd, LOGTYPE lg)
        //{
        //    string QR = "";

        //    if (xmlFilePath != "")
        //    {
        //        string uuid = Basepage.getXMLNodeValue("d:Invoice/cbc:UUID", xmlFilePath);
        //        string irn = Basepage.getXMLNodeValue("d:Invoice/cbc:ID", xmlFilePath);
        //        string InvoicHashEncoded = Basepage.getXMLNodeValue("*[local-name()='Invoice']//*[local-name()='UBLExtensions']//*[local-name()='UBLExtension']//*[local-name()='ExtensionContent']//*[local-name()='UBLDocumentSignatures']//*[local-name()='SignatureInformation']//*[local-name()='Signature']//*[local-name()='SignedInfo']//ds:Reference[@Id='invoiceSignedData']//*[local-name()='DigestValue']", xmlFilePath);


        //        string qq = "select QR from QR where irn='" + irn + "' and uuid='" + uuid + "'";
        //        DataTable dt = bp.getDataDBQuery(qq, false, lg);
        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            QR = dt.Rows[0]["QR"].ToString();
        //        }
        //        return new string[] { QR, irn, uuid, InvoicHashEncoded };
        //    }
        //    else
        //    {

        //        string qq = "select QR from QR where irn='" + bd.Invoice.EInvoice.ID.en + "' and uuid='" + bd.Invoice.InvoiceUUID + "'";
        //        DataTable dt = bp.getDataDBQuery(qq, false, lg);
        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            QR = dt.Rows[0]["QR"].ToString();
        //        }
        //        return new string[] { QR, bd.Invoice.EInvoice.ID.en.ToString(), bd.Invoice.InvoiceUUID, "" };
        //    }
        //}

        public void DB_UPDAte(DataReturn ret, string IssueDate, string IssueTime, string irn, string uuid, INVOICE_OPERATION op, SOURCE src, LOGTYPE typ, string XMLFileName, DOCTYPE doctype, int actionCount, PROCESS_TYPE ProcessType)
        {
            SqlParameter[] sqlpar = null;
            try
            {
                string datetim = IssueDate + " " + IssueTime;
                int errorcount = 0;
                int warningCount = 0;
                int infoCount = 0;
                string errors = "";
                string warnings = "";
                string info = "";
                if (ret != null)
                {
                    errorcount = ret?.Errors?.Count ?? 0;
                    warningCount = ret?.Warnings?.Count ?? 0;
                    infoCount = ret?.info?.Count ?? 0;
                    if (errorcount > 0)
                        errors = String.Join("<<>>", ret.Errors.ToArray());
                    if (warningCount > 0)
                        warnings = String.Join("<<>>", ret.Warnings.ToArray());
                    if (infoCount > 0)
                        info = String.Join("<<>>", ret.info.ToArray());
                }

                string certSerial_ICV = ret.EGS_X509SerialNumber + "_" + ret.ICV;
                certSerial_ICV = certSerial_ICV.Equals("_") ? "" : certSerial_ICV;

                bp.log("invtype :: " + ret.InvoiceType.ToString(), 0, typ);
                bp.log("datetim :: " + datetim, 0, typ);

                sqlpar = new SqlParameter[] {
                                new SqlParameter("@OPERATION", op.ToString()),
                                new SqlParameter("@IRN",irn),
                                new SqlParameter("@UUID", uuid),
                                new SqlParameter("@XMLFileName", XMLFileName.ToString()),
                                new SqlParameter("@TaxTotal", ret.TaxTotal??""),
                                new SqlParameter("@DocType",doctype.ToString()),
                                new SqlParameter("@Action",ret._action.ToString() ),
                                new SqlParameter("@ActionStatus",ret._actionStatus.ToString()),
                                new SqlParameter("@TransactionDate",datetim),
                                new SqlParameter("@StoreNo", ""),
                                new SqlParameter("@TerminalNo", ""),
                                new SqlParameter("@TrxNo", ""),
                                new SqlParameter("@TimeleftToReport", ret.InvoiceType.ToString()),
                                new SqlParameter("@ReportedIn", certSerial_ICV),
                                new SqlParameter("@Source", src.ToString()),
                                new SqlParameter("@ActionCount",actionCount.ToString()),
                                new SqlParameter("@ErrorCount", errorcount.ToString()),
                                new SqlParameter("@WarningCount", warningCount.ToString()),
                                new SqlParameter("@Errors",errors),
                                new SqlParameter("@Warnings", warnings),
                                new SqlParameter("@Info", info),
                                new SqlParameter("@FullResponse", ret.FullResponse??""),
                                new SqlParameter("@ProcessType", ProcessType.ToString()),
                                new SqlParameter("@HTTPResponseCode", ret.HTTPResponseCode??""),
                                new SqlParameter("@CustIdentifier",Basepage.CustIdentifier),
                                new SqlParameter("@InvoiceHash",ret.InvoiceHash??""),
                                new SqlParameter("@QR", ret.QR??"") 
                                ,new SqlParameter("@InvoiceTotal", ret.InvoiceTotal??"")
                                };
                DataSet ds = bp.ExecuteProcedure("SP_INVOICE", Basepage.conStr, sqlpar);

                DataTable dt = ds.Tables[0];
                string errorMsg = dt.Rows[0]["errorMsg"].ToString();
                string errorNo = dt.Rows[0]["errorNo"].ToString();

                bp.log("DB_UPDAte Status:: " + errorMsg + " errorNo:" + errorNo + " Oepration:" + op.ToString(), 1, typ);


                if (errorMsg.Equals("SUCCESS"))
                {
                }
                else throw new InvalidCastException(ERR.DB_Update_Error.ToString());
            }
            catch (Exception ex)
            {
                //If a trx comes thru API,DB update failure should not affect sending response to API, 
                //otherwise clearance/signing is successful, but invoice gets failed at POS/oracle, and there will be VAT total mismatch for the customer. 

                bp.log("DB_UPDAte exception:: " + ex.ToString(), 1, typ);
                string dberr = "DBUpdate error:" + irn + ":Parameters are-->";

                if (sqlpar != null)
                {
                    foreach (SqlParameter ppp in sqlpar)
                    {
                        dberr += ppp.Value.ToString() + ",";
                    }
                    bp.log(dberr, 1, LOGTYPE.DB_UPDATE_ERROR);
                }

            }
        }


        private DataReturn Send_to_Clearance_B2B(string xmlFilePath, FileNameFields dATA, LOGTYPE tYP, string uuid, SOURCE src)
        {
            Set_Accounting_Customer_Party_B2B(xmlFilePath, tYP);

            // xmlFilePath = @"C:\IDOL\ZATCA\files\invoiceXML\POS\B2B\UNSIGNED\u300465395500003_20230407T145000_0000523040714500110011.xml";
            //uuid = "22d31bac-0dcd-44aa-9e00-d89d1d3f0070";
            DataReturn RET = new DataReturn();
            RET.DocumentType = DOCTYPE.B2B;
            RET._action = ACTION.CLEARANCE;
            Result rrrr = _IHashingValidator.GenerateEInvoiceHashing(xmlFilePath);
            RET.InvoiceHash = rrrr.ResultedValue;
            RET.InvReferenceNumber = dATA.irn;

            bp.logWrite("Clearance B2B API:" + Basepage.B2B_Clearance_API, tYP);

            string xmlString = File.ReadAllText(xmlFilePath, Encoding.UTF8);
            string base64 = Basepage.Base64Encode(xmlString);





            //string body = "{  \"invoiceHash\": \"<invoiceHash>\",\"uuid\": \"<uuid>\",\"invoice\": \"<invoice>\"}";
            //body = body.Replace("<invoiceHash>", rrrr.ResultedValue);
            //body = body.Replace("<uuid>", uuid);
            //body = body.Replace("<invoice>", base64);

            dynamic oooo = new JObject();
            oooo.invoiceHash = rrrr.ResultedValue;
            oooo.uuid = uuid;
            oooo.invoice = base64;

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(oooo);

            bp.log("body:" + jsonString, 1, tYP);

            string auth = Basepage.GetCertData().RawDataStringB64 + ":" + Basepage.GetCertData().secret;

            bp.log("auth:" + auth, 1, tYP);
            auth = Basepage.Base64Encode(auth);


            var client = new RestClient(Basepage.B2B_Clearance_API);

            if (!Basepage.proxy.Equals(""))
                client.Proxy = new WebProxy(Basepage.proxy);
            //client.Timeout = -1;
            var request = new RestRequest(RestSharp.Method.POST);
            request.AddHeader("accept", "application/json");
            request.AddHeader("accept-language", "en");
            request.AddHeader("Clearance-Status", "1");
            request.AddHeader("Accept-Version", "V2");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Basic " + auth);

            request.AddParameter("application/json", jsonString, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);


            RET.HTTPResponseCode = response.StatusCode.ToString();


            bp.logWrite("ResponseCode:" + response.StatusCode, tYP);
            bp.logWrite("ResponseBody:" + response.Content, tYP);

            RET._actionStatus = ACTION_STATUS.ClearanceFailed;

            bool change_icv_pih = new Basepage().Get_ICV_Update(response.StatusCode, DOCTYPE.B2B);
            //setting the cases where ICV PIH change is not required/required.
            RET.FullResponse = response.Content;

            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Accepted)  // 200,202 ok,accepted with warning
            {
                var data = (JObject)JsonConvert.DeserializeObject(response.Content);
                string status = data["clearanceStatus"]?.Value<string>();

                if (status.Equals("CLEARED"))
                {
                    RET.RESPMSG = "SUCCESS";
                    RET.RC = 0;
                    RET.ClearedInvoice = data["clearedInvoice"].Value<string>();
                    RET._actionStatus = ACTION_STATUS.Cleared;

                    //save signed b2b xml
                    string xml = Basepage.Base64Decode(RET.ClearedInvoice);
                    string ff = Get_Folder(DOCTYPE.B2B, src, XMLSTATUS.SIGNED, "");
                    string filePath1 = SaveFileNormal(xml, Path.GetFileNameWithoutExtension(xmlFilePath), ff, tYP);
                    //File.Delete(xmlFilePath);
                    string ddd = DateTime.Now.ToString("ddMMyyHHmmss");
                    string newpath = xmlFilePath.Replace(Path.GetFileName(xmlFilePath), Path.GetFileNameWithoutExtension(xmlFilePath) + "_signed_" + ddd + Path.GetExtension(xmlFilePath));
                    File.Move(xmlFilePath, newpath);

                    //get InvoiceHash and QR from ClearedInvoice
                    RET = Get_Signed_Fields_XML(filePath1, tYP, RET);

                }
                else
                {
                    string ff = Get_Folder(DOCTYPE.B2B, src, XMLSTATUS.FAILED, "");
                    string newpath = ff + Path.GetFileName(xmlFilePath);

                    if (File.Exists(newpath)) File.Delete(newpath);
                    File.Move(xmlFilePath, newpath);


                    //Send email
                    string msg = "Clearance Failed.xml:" + xmlFilePath;
                    bp.SendAlertEmail(MailType.ClearanceError, msg, DOCTYPE.B2B, "");
                }

            }
            else if (response.StatusCode == System.Net.HttpStatusCode.SeeOther) //303
            {
                //as per zatca, 303 response indicates clearance disabled, in that case, report the B2B docs to Reporting API.
                RET = SignXML(xmlFilePath, dATA, tYP, src, DOCTYPE.B2B);
                //Check and confirm if B2B xml has to be signed and send for reporting, 
                //or direclt send to reporting api and whether reporting api will send the QR back.

                //Send email
                string msg = "Response 303 received from zatca.xml:" + xmlFilePath;
                bp.SendAlertEmail(MailType.ClearanceError, msg, DOCTYPE.B2B, "");

            }
            else    //all other failed cases
            {
                RET.RESPMSG = response.StatusCode.ToString() + "-" + response.Content;
                RET.RC = 1;
                RET.ClearedInvoice = "";
                RET.QR = "";
                RET._actionStatus = ACTION_STATUS.ClearanceFailed;

                string ff = Get_Folder(DOCTYPE.B2B, src, XMLSTATUS.FAILED, "");
                string newpath = ff + Path.GetFileName(xmlFilePath);

                if (File.Exists(newpath)) File.Delete(newpath);
                File.Move(xmlFilePath, newpath);

                //Send email
                string msg = "Clearance Failed! xml:" + xmlFilePath + "<br/>" + JobZatca.FormatJson(RET.FullResponse);
                bp.SendAlertEmail(MailType.ClearanceError, msg, DOCTYPE.B2B, "");

            }



            RET.change_icv_pih = change_icv_pih;
            RET.FullResponse = response.Content;

            var data1 = (JObject)JsonConvert.DeserializeObject(response.Content);
            JArray errr = (JArray)data1["validationResults"]["errorMessages"];
            JArray wwww = (JArray)data1["validationResults"]["warningMessages"];
            JArray innnn = (JArray)data1["validationResults"]["infoMessages"];


            if (errr.Count > 0)
            {
                List<string> ll = new List<string>();
                foreach (JObject obj in errr)
                {
                    ll.Add(obj["message"].ToString());
                }
                RET.Errors = ll;
            }
            if (wwww.Count > 0)
            {
                List<string> ll = new List<string>();

                foreach (JObject obj in wwww)
                {
                    ll.Add(obj["message"].ToString());
                }
                RET.Warnings = ll;
            }
            if (innnn.Count > 0)
            {
                List<string> ll = new List<string>();
                foreach (JObject obj in innnn)
                {
                    ll.Add(obj["message"].ToString());
                }
                RET.info = ll;
            }



            return RET;

        }
        internal XMLSTATUS getXML_STATUS(DOCTYPE doctype, ACTION_STATUS actionStatus)
        {

            XMLSTATUS stat;
            if (doctype == DOCTYPE.B2B && actionStatus == ACTION_STATUS.ClearanceFailed) stat = XMLSTATUS.FAILED;
            else if (doctype == DOCTYPE.B2B && actionStatus == ACTION_STATUS.Cleared) stat = XMLSTATUS.SIGNED;
            else if (doctype == DOCTYPE.B2C && actionStatus == ACTION_STATUS.Reported) stat = XMLSTATUS.REPORTED;
            else if (doctype == DOCTYPE.B2C && actionStatus == ACTION_STATUS.ReportingFailed) stat = XMLSTATUS.FAILED;
            else if (actionStatus == ACTION_STATUS.Signed) stat = XMLSTATUS.SIGNED;
            else if (actionStatus == ACTION_STATUS.SigningFailed) stat = XMLSTATUS.FAILED;
            else throw new InvalidCastException("invalid condition on getting XMLStatus");
            return stat;

        }
        public string getXMLPath_from_IRN(string irn, LOGTYPE LG)
        {

            string qq = "select doctype, XMLFileName,Action,ActionStatus,Source from Invoices where IRN='" +
                       irn + "'";
            DataTable dt = bp.getDataDBQuery(qq, false, LG);

            if (dt != null && dt.Rows.Count > 0)
            {
                string doctype = dt.Rows[0]["doctype"].ToString();
                string Action = dt.Rows[0]["Action"].ToString();
                string ActionStatus = dt.Rows[0]["ActionStatus"].ToString();
                string Source = dt.Rows[0]["Source"].ToString();
                string XMLFileName = dt.Rows[0]["XMLFileName"].ToString();

                Enum.TryParse(doctype, out DOCTYPE _doctype);
                Enum.TryParse(Source, out SOURCE _source);
                Enum.TryParse(ActionStatus, out ACTION_STATUS _ActionStatus);

                XMLSTATUS _xmlstatus = getXML_STATUS(_doctype, _ActionStatus);
                string folder = Get_Folder(_doctype, _source, _xmlstatus, XMLFileName);   //eg: C:\IDOL\ZATCA\files\invoiceXML\EXTERNAL\B2C\FAILED
                string newpath = folder + XMLFileName;
                return newpath;
            }
            else
            {
                return "";
            }
        }
        private FileNameFields SetFields_to_XML(string xmlFilepath, string iCV, string pih, string uuid, LOGTYPE TYP)
        {
            FileNameFields bd = new FileNameFields();
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
            // nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            //nsmgr.AddNamespace("xades", "http://uri.etsi.org/01903/v1.3.2#");
            // nsmgr.AddNamespace("sac", "urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2");
            //nsmgr.AddNamespace("sig", "urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2");
            nsmgr.AddNamespace("ext", NS_EXT);
            nsmgr.AddNamespace("cac", NS_CAC);


            xDoc.XPathSelectElement("d:Invoice/cac:AdditionalDocumentReference/cbc:UUID", nsmgr).Value = iCV;
            xDoc.XPathSelectElement("d:Invoice/cac:AdditionalDocumentReference/cac:Attachment/cbc:EmbeddedDocumentBinaryObject", nsmgr).Value = pih;
            xDoc.XPathSelectElement("d:Invoice/cbc:UUID", nsmgr).Value = uuid;

            string partyTaxSchemeCompanyId = xDoc.XPathSelectElement("d:Invoice/cac:AccountingSupplierParty/cac:Party/cac:PartyTaxScheme/cbc:CompanyID", nsmgr).Value;
            string IssueDate_YYYY_MM_DD = xDoc.XPathSelectElement("d:Invoice/cbc:IssueDate", nsmgr).Value;
            string IssueTime_HH_MM_SS = xDoc.XPathSelectElement("d:Invoice/cbc:IssueTime", nsmgr).Value;
            string irn = xDoc.XPathSelectElement("d:Invoice/cbc:ID", nsmgr).Value;


            bd.partyTaxSchemeCompanyId = partyTaxSchemeCompanyId;
            bd.IssueDate_YYYY_MM_DD = IssueDate_YYYY_MM_DD;
            bd.IssueTime_HH_MM_SS = IssueTime_HH_MM_SS;
            bd.irn = irn;

            XmlWriterSettings xws = new XmlWriterSettings { OmitXmlDeclaration = true };
            using (XmlWriter xw = XmlWriter.Create(xmlFilepath, xws))
                xDoc.Save(xw);


            return bd;

        }

        private FileNameFields SetFields_to_XML_NEW(string xmlFilepath, string iCV, string pih, string uuid, LOGTYPE TYP)
        {
            FileNameFields bd = new FileNameFields();

            string xmldata = File.ReadAllText(xmlFilepath, Encoding.UTF8);









            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.LoadXml(xmldata);
            var nsmgr = new XmlNamespaceManager(xmlDoc2.NameTable);


            nsmgr.AddNamespace("d", NS_XMLNS);
            nsmgr.AddNamespace("cbc", NS_CBC);
            nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            nsmgr.AddNamespace("xades", "http://uri.etsi.org/01903/v1.3.2#");
            nsmgr.AddNamespace("sac", "urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2");
            nsmgr.AddNamespace("sig", "urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2");
            nsmgr.AddNamespace("ext", NS_EXT);
            nsmgr.AddNamespace("cac", NS_CAC);


            // var nl = xmlDoc2.SelectNodes("//xsl:import/@href", nsmgr);


            //XmlNode node = xmlDoc2.SelectSingleNode("/Projects/Project[@ID=" + nodeId + "]");
            XmlNode node_uuid = xmlDoc2.SelectSingleNode("d:Invoice/cbc:UUID", nsmgr);
            XmlNode node_icv = xmlDoc2.SelectSingleNode("d:Invoice/cac:AdditionalDocumentReference/cbc:UUID", nsmgr);
            XmlNode node_pih = xmlDoc2.SelectSingleNode("d:Invoice/cac:AdditionalDocumentReference/cac:Attachment/cbc:EmbeddedDocumentBinaryObject", nsmgr);

            node_uuid.InnerText = uuid;
            node_icv.InnerText = iCV;
            node_pih.InnerText = pih;

            XmlNode node_partyTaxSchemeCompanyId = xmlDoc2.SelectSingleNode("d:Invoice/cac:AccountingSupplierParty/cac:Party/cac:PartyTaxScheme/cbc:CompanyID", nsmgr);
            XmlNode node_IssueDate_YYYY_MM_DD = xmlDoc2.SelectSingleNode("d:Invoice/cbc:IssueDate", nsmgr);
            XmlNode node_IssueTime_HH_MM_SS = xmlDoc2.SelectSingleNode("d:Invoice/cbc:IssueTime", nsmgr);
            XmlNode node_irn = xmlDoc2.SelectSingleNode("d:Invoice/cbc:ID", nsmgr);



            string partyTaxSchemeCompanyId = node_partyTaxSchemeCompanyId.InnerText;
            string IssueDate_YYYY_MM_DD = node_IssueDate_YYYY_MM_DD.InnerText;
            string IssueTime_HH_MM_SS = node_IssueTime_HH_MM_SS.InnerText;
            string irn = node_irn.InnerText;


            bd.partyTaxSchemeCompanyId = partyTaxSchemeCompanyId;
            bd.IssueDate_YYYY_MM_DD = IssueDate_YYYY_MM_DD;
            bd.IssueTime_HH_MM_SS = IssueTime_HH_MM_SS;
            bd.irn = irn;

            //node.Attributes["Name"].Value = "Project2_Update"; 
            xmlDoc2.Save(xmlFilepath);


            return bd;

        }


        private void Set_Accounting_Customer_Party_B2B(string xmlFilepath, LOGTYPE TYP)
        {


            //for virgin, b2b customers are saved in VATID DB and the customer details are entered thru idol webpage
            //so whenever any b2b trx comes from ANY system like pos, TBO, SAP or ecommerce,
            //it will check if the customer details are present in VATID db and populated from db.
            //For that it is mandatory that api trx json /or invoice xml from store must have PartyTaxSchemeCompanyID (VAT Number) populated in it.
            //from some system like SAP, they have all b2b customer data and thus they will be sending all fields of accountingCustomerParty in the api input,
            //so here it will check if the data is already populated, for that I am checking only one field. Registration name.
            //so if registration name is populated , it will assume the sender is already populating b2b customer info and
            //for such invocies the system will not check DB for customer info.
            //if registration name in the xml is empty and if the VATID db has customer info for that specific vat number,
            //then we will populate accountingCustomerParty tags from database.

            if (Basepage.CustIdentifier.Equals("2"))
            {
                bp.log("Colleting fields.. XML:: " + xmlFilepath, 1, TYP);

                BaseData.AccountingCustomerParty cust = new BaseData.AccountingCustomerParty();

                string xml_PartyTaxSchemeCompanyID = "";
                string xml_PartyIdentificationSchemeID = "";
                string xml_PartyIdentificationSchemeID_value = "";
                string xml_RegistrationName = "";
                try
                {
                    bp.log("getting xml_PartyTaxSchemeCompanyID..", 1, TYP);
                    xml_PartyTaxSchemeCompanyID = Basepage.getXMLNodeValue("d:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PartyTaxScheme/cbc:CompanyID", xmlFilepath);
                }
                catch (Exception ex)
                {
                    bp.log("getting xml_PartyTaxSchemeCompanyID err:: " + ex.Message, 1, TYP);
                }
                try
                {
                    bp.log("getting xml_RegistrationName..", 1, TYP);
                    xml_RegistrationName = Basepage.getXMLNodeValue("d:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PartyLegalEntity/cbc:RegistrationName", xmlFilepath);
                }
                catch (Exception ex)
                {
                    bp.log("getting xml_RegistrationName err:: " + ex.Message, 1, TYP);
                }
                try
                {
                    bp.log("getting xml_PartyIdentificationSchemeID_value..", 1, TYP);
                    string id_value = Basepage.getXMLNodeValue("d:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PartyIdentification/cbc:ID", xmlFilepath);

                    if (id_value.Length > 3)      //id_value=CRN;5900135656
                    {
                        xml_PartyIdentificationSchemeID = id_value.Split(new char[] { ';' })[0];
                        xml_PartyIdentificationSchemeID_value = id_value.Split(new char[] { ';' })[1];
                    }
                }
                catch (Exception ex)
                {
                    bp.log("getting xml_PartyIdentificationSchemeID_value err:: " + ex.Message, 1, TYP);
                }


                if (xml_RegistrationName.Trim().Equals(""))
                {
                    bp.log("No accountingCustomerParty details exist in the xml.Populating from VATDB... :: " + xmlFilepath, 1, TYP);
                    string qqq = "select * from Customers where PartyTaxSchemeCompanyID = '" + xml_PartyTaxSchemeCompanyID + "' or ( PartyIdentificationSchemeID = '" + xml_PartyIdentificationSchemeID + "' and PartyIdentificationID = '" + xml_PartyIdentificationSchemeID_value + "' )";
                    DataTable dt = bp.getDataDBQuery(qqq, false, Basepage.ConStrCustomerVAT, TYP);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        bp.log("Customer data found in DB...::" + xmlFilepath, 1, TYP);

                        cust.Party.PartyIdentification.ID.schemeID = dt.Rows[0]["PartyIdentificationSchemeID"].ToString();
                        cust.Party.PartyIdentification.ID.en = cust.Party.PartyIdentification.ID.ar = dt.Rows[0]["PartyIdentificationID"].ToString();
                        cust.Party.PostalAddress.StreetName.en = dt.Rows[0]["StreetName"].ToString();
                        cust.Party.PostalAddress.StreetName.ar = dt.Rows[0]["StreetNameArabic"].ToString();
                        cust.Party.PostalAddress.BuildingNumber.en = cust.Party.PostalAddress.BuildingNumber.ar = dt.Rows[0]["BuildingNumber"].ToString();
                        cust.Party.PostalAddress.PlotIdentification.en = cust.Party.PostalAddress.PlotIdentification.ar = dt.Rows[0]["PlotIdentification"].ToString();
                        cust.Party.PostalAddress.CitySubdivisionName.en = dt.Rows[0]["CitySubdivisionName"].ToString();
                        cust.Party.PostalAddress.CitySubdivisionName.ar = dt.Rows[0]["CitySubdivisionNameArabic"].ToString();
                        cust.Party.PostalAddress.CityName.en = dt.Rows[0]["CityName"].ToString();
                        cust.Party.PostalAddress.CityName.ar = dt.Rows[0]["CityNameArabic"].ToString();
                        cust.Party.PostalAddress.PostalZone = dt.Rows[0]["PostalZone"].ToString();
                        cust.Party.PostalAddress.Country.IdentificationCode = dt.Rows[0]["CountryIdentificationCode"].ToString();
                        cust.Party.PartyTaxScheme.TaxScheme.ID.en = cust.Party.PartyTaxScheme.TaxScheme.ID.ar = dt.Rows[0]["TaxScheme"].ToString();
                        cust.Party.PartyTaxScheme.CompanyID = dt.Rows[0]["PartyTaxSchemeCompanyID"].ToString();
                        cust.Party.PartyLegalEntity.RegistrationName.en = dt.Rows[0]["RegistrationName"].ToString();
                        cust.Party.PartyLegalEntity.RegistrationName.ar = dt.Rows[0]["RegistrationNameArabic"].ToString();


                        //reading and setting xml
                        string xmldata = File.ReadAllText(xmlFilepath, Encoding.UTF8);
                        XmlDocument xmlDoc2 = new XmlDocument();
                        xmlDoc2.LoadXml(xmldata);
                        var nsmgr = new XmlNamespaceManager(xmlDoc2.NameTable);
                        nsmgr.AddNamespace("d", NS_XMLNS);
                        nsmgr.AddNamespace("cbc", NS_CBC);
                        nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
                        nsmgr.AddNamespace("xades", "http://uri.etsi.org/01903/v1.3.2#");
                        nsmgr.AddNamespace("sac", "urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2");
                        nsmgr.AddNamespace("sig", "urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2");
                        nsmgr.AddNamespace("ext", NS_EXT);
                        nsmgr.AddNamespace("cac", NS_CAC);


                        XmlNode node_PartyIdentificationID = xmlDoc2.SelectSingleNode("d:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PartyIdentification/cbc:ID", nsmgr);
                        XmlNode node_StreetName = xmlDoc2.SelectSingleNode("d:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PostalAddress/cbc:StreetName", nsmgr);
                        XmlNode node_BuildingNumber = xmlDoc2.SelectSingleNode("d:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PostalAddress/cbc:BuildingNumber", nsmgr);
                        XmlNode node_PlotIdentification = xmlDoc2.SelectSingleNode("d:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PostalAddress/cbc:PlotIdentification", nsmgr);
                        XmlNode node_CitySubdivisionName = xmlDoc2.SelectSingleNode("d:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PostalAddress/cbc:CitySubdivisionName", nsmgr);
                        XmlNode node_CityName = xmlDoc2.SelectSingleNode("d:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PostalAddress/cbc:CityName", nsmgr);
                        XmlNode node_PostalZone = xmlDoc2.SelectSingleNode("d:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PostalAddress/cbc:PostalZone", nsmgr);
                        XmlNode node_CountryIdentificationCode = xmlDoc2.SelectSingleNode("d:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PostalAddress/cac:Country/cbc:CountryIdentificationCode", nsmgr);
                        XmlNode node_PartyTaxSchemeID = xmlDoc2.SelectSingleNode("d:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PartyTaxScheme/cac:TaxScheme/cbc:ID", nsmgr);
                        XmlNode node_PartyTaxSchemeCompanyID = xmlDoc2.SelectSingleNode("d:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PartyTaxScheme/cbc:CompanyID", nsmgr);
                        XmlNode node_RegistrationName = xmlDoc2.SelectSingleNode("d:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PartyLegalEntity/cbc:RegistrationName", nsmgr);

                        node_PartyIdentificationID.InnerText = cust.Party.PartyIdentification.ID.en;
                        node_PartyIdentificationID.Attributes["schemeID"].Value = cust.Party.PartyIdentification.ID.schemeID;
                        node_StreetName.InnerText = cust.Party.PostalAddress.StreetName.arb;
                        node_BuildingNumber.InnerText = cust.Party.PostalAddress.BuildingNumber.arb;
                        node_PlotIdentification.InnerText = cust.Party.PostalAddress.PlotIdentification.arb;
                        node_CitySubdivisionName.InnerText = cust.Party.PostalAddress.CitySubdivisionName.arb;
                        node_CityName.InnerText = cust.Party.PostalAddress.CityName.arb;
                        node_PostalZone.InnerText = cust.Party.PostalAddress.PostalZone;
                        node_CountryIdentificationCode.InnerText = cust.Party.PostalAddress.Country.IdentificationCode;
                        node_PartyTaxSchemeID.InnerText = cust.Party.PartyTaxScheme.TaxScheme.ID.en;
                        node_PartyTaxSchemeCompanyID.InnerText = cust.Party.PartyTaxScheme.CompanyID;
                        node_RegistrationName.InnerText = cust.Party.PartyLegalEntity.RegistrationName.arb;



                        xmlDoc2.Save(xmlFilepath);



                    }
                    else
                    {
                        bp.log("Customer details not present in Customers DB.Keeping AccountingCustomerParty as it is in the XML::" + xmlFilepath, 1, TYP);
                    }
                }
                else
                {
                    bp.log("accountingCustomerParty details present in the XML.Quitting database fetch..:: " + xmlFilepath, 1, TYP);
                }


            }
        }

        public DataReturn Get_Signed_Fields_XML(string xmlFilepath, LOGTYPE TYP, DataReturn RET)
        {

            XmlNameTable table = null;
            XDocument xDoc = null;
            try
            {
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

            }
            catch (Exception ex)
            {
                bp.log("Get_Signed_Fields_XML:: " + ex.ToString(), 1, TYP);
                throw new InvalidCastException("XML file not found.");
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


            string QR = xDoc.XPathSelectElement("*[local-name()='Invoice']//*[local-name()='AdditionalDocumentReference'][cbc:ID[text()='QR']]//*[local-name()='Attachment']", nsmgr)?.Value?.Trim();
            string InvoicHashEncoded = xDoc.XPathSelectElement("*[local-name()='Invoice']//*[local-name()='UBLExtensions']//*[local-name()='UBLExtension']//*[local-name()='ExtensionContent']//*[local-name()='UBLDocumentSignatures']//*[local-name()='SignatureInformation']//*[local-name()='Signature']//*[local-name()='SignedInfo']//ds:Reference[@Id='invoiceSignedData']//*[local-name()='DigestValue']", nsmgr)?.Value;

            string taxtotal = xDoc.XPathSelectElement("*[local-name()='Invoice']//*[local-name()='TaxTotal']//cbc:TaxAmount[@currencyID='SAR']", nsmgr)?.Value?.Trim();
            string InvoiceTotal = xDoc.XPathSelectElement("*[local-name()='Invoice']//*[local-name()='LegalMonetaryTotal']//cbc:TaxInclusiveAmount[@currencyID='SAR']", nsmgr)?.Value?.Trim();

            string X509SerialNumber = xDoc.XPathSelectElement("*[local-name()='Invoice']//*[local-name()='UBLExtensions']//*[local-name()='UBLExtension']//*[local-name()='ExtensionContent']//*[local-name()='UBLDocumentSignatures']//*[local-name()='SignatureInformation']//*[local-name()='Signature']//*[local-name()='Object']//*[local-name()='QualifyingProperties']//*[local-name()='SignedProperties']//*[local-name()='SigningCertificate']//*[local-name()='Cert']//*[local-name()='IssuerSerial']//*[local-name()='X509SerialNumber']", nsmgr)?.Value;
            string icv = xDoc.XPathSelectElement("*[local-name()='Invoice']//*[local-name()='AdditionalDocumentReference'][cbc:ID[text()='ICV']]//*[local-name()='UUID']", nsmgr)?.Value?.Trim();


            RET.QR = RET.QR == null || RET.QR.Equals("") ? QR : RET.QR;
            RET.InvoiceHash = RET.InvoiceHash == null || RET.InvoiceHash.Equals("") ? InvoicHashEncoded : RET.InvoiceHash;
            RET.TaxTotal = RET.TaxTotal == null || RET.TaxTotal.Equals("") ? taxtotal : RET.TaxTotal;
            RET.InvoiceTotal = RET.InvoiceTotal == null || RET.InvoiceTotal.Equals("") ? InvoiceTotal : RET.InvoiceTotal;

            RET.EGS_X509SerialNumber = X509SerialNumber;
            RET.ICV = Int64.Parse(icv);

            return RET;

        }

        //public List<string> readIcvPih_fromFile(LOGTYPE TYP)
        //{

        //    FileStream myFileStream = null;
        //    List<string> myList = new List<string>();
        //    string myLine;

        //    bp.log("Reading ICV_PIH file:", 1, TYP);

        //    try
        //    {
        //        while (true)
        //        {
        //            try
        //            {
        //                myFileStream = new FileStream(Basepage.ICV_PIH_file, FileMode.Open, FileAccess.Read, FileShare.None);
        //                break;
        //            }
        //            catch
        //            {
        //                Thread.Sleep(20);  //if the file is used by any other process, it waits for 20ms and tries again
        //            }
        //        }
        //        StreamReader myStreamReader = new StreamReader(myFileStream);
        //        while ((myLine = myStreamReader.ReadLine()) != null)
        //        {
        //            myList.Add(myLine.Substring(4));
        //        }

        //        myStreamReader.Close();
        //        myFileStream.Close();
        //        myFileStream.Dispose();

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidCastException("ICV PIH Read error.Try again." + ex.ToString());
        //    }
        //    finally
        //    {
        //        if (myFileStream != null)
        //        {
        //            myFileStream.Dispose();
        //        }
        //    }
        //    bp.log("ICV:" + myList[0], 1, LOGTYPE.COUNTER);
        //    bp.log("PIH:" + myList[1], 1, LOGTYPE.COUNTER);

        //    bp.log("ICV_PIH:" + myList[0] + " " + myList[1], 1, TYP);

        //    if (myList[0] == null || myList[0].Equals("")) throw new InvalidCastException("ICV error.Try again");
        //    if (myList[1] == null || myList[1].Equals("")) throw new InvalidCastException("PIH error.Try again");
        //    return myList;

        //}


        //public void writeIcvPih_File(int icv, string pih, LOGTYPE TYP)
        //{

        //    FileStream myFileStream = null;

        //    bp.log("writing ICV_PIH:icv=" + icv + ";PIH:" + pih, 1, LOGTYPE.COUNTER);

        //    try
        //    {
        //        while (true)
        //        {
        //            try
        //            {
        //                myFileStream = new FileStream(Basepage.ICV_PIH_file, FileMode.Open, FileAccess.Write, FileShare.None);
        //                break;
        //            }
        //            catch
        //            {
        //                Thread.Sleep(20);  //if the file is used by any other process, it waits for 20ms and tries again
        //            }
        //        }
        //        StreamWriter myStreamWriter = new StreamWriter(myFileStream);
        //        myFileStream.Seek(0, SeekOrigin.Begin);
        //        myFileStream.SetLength(0);

        //        myStreamWriter.WriteLine("ICV=" + icv);
        //        myStreamWriter.WriteLine("PIH=" + pih);

        //        myStreamWriter.Close();
        //        myFileStream.Close();
        //        myFileStream.Dispose();

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidCastException("ICV PIH write error.Try again." + ex.ToString());
        //    }
        //    finally
        //    {
        //        if (myFileStream != null)
        //        {
        //            myFileStream.Dispose();
        //        }
        //    }

        //}

        public DataReturn SignXML(string xmlFilePath, FileNameFields bd, LOGTYPE typ, SOURCE src, DOCTYPE docType)
        {
            ///  xmlFilePath = @"C:\IDOL\ZATCA\files\invoiceXML\POS\B2C\UNSIGNED\FARM B2C.xml";
            // xmlFilePath = @"D:\Desktop\_IDOL\ZATCA phase2\zatca-einvoicing-sdk-234-R3.2.0\zatca-einvoicing-sdk-234-R3.2.0\Data\Samples\Simplified\Invoice\Simplified_Invoice.xml";
            DataReturn RET = new DataReturn();
            RET._actionStatus = ACTION_STATUS.SigningFailed;
            RET.DocumentType = docType;
            RET._action = docType == DOCTYPE.B2C ? ACTION.SIGNING_B2C : ACTION.SIGNING_B2B;



            try
            {
                bp.log("Signing start", 0, typ);


                CertificateData certData = Basepage.GetCertData();
                //  EInvoiceSigningLogic eee = new EInvoiceSigningLogic();
                //  Result rrrr1 = eee.SignDocument(xmlFilePath, certData.RawDataString, certData.privateKeytext);

                bp.log("xmlFilePath:" + xmlFilePath, 0, typ);
                bp.log("cert:" + certData.RawDataString, 0, typ);
                bp.log("privatekey:" + certData.privateKeytext, 0, typ);
                ZATCA.signDLL.Result rrrr1 = new ZATCA.signDLL.signdll().SignDocument(xmlFilePath, certData.RawDataString, certData.privateKeytext);



                // Result rrrr1 = eee.SignDocument(xmlFilePath, certData.RawDataString, "TUlHTkFnRUFNQkFHQnlxR1NNNDlBZ0VHQlN1QkJBQUtCSFl3ZEFJQkFRUWcwcG45Z1VpaWluakozemxtcFl5YzRZbXJsNWZhaHVMN0tkUndoMHkvVTdxZ0J3WUZLNEVFQUFxaFJBTkNBQVJSc1piY2tEWThGSWR0anlyQWxsYm8rdW8yWjNKWVlBTjZKV3plT0FXRE1iTUpDdVZEdkZwK2J0aXN4YkZjNHIwOWd5NFJTTUtNWTMzNW9sTXJnTndV");
                //Result rrrr1 = eee.SignDocument(xmlFilePath, "MIID6zCCA5CgAwIBAgITbwAAf18FYT9MDlU3fgABAAB/XzAKBggqhkjOPQQDAjBjMRUwEwYKCZImiZPyLGQBGRYFbG9jYWwxEzARBgoJkiaJk/IsZAEZFgNnb3YxFzAVBgoJkiaJk/IsZAEZFgdleHRnYXp0MRwwGgYDVQQDExNUU1pFSU5WT0lDRS1TdWJDQS0xMB4XDTIyMDkwMTEwNTEzM1oXDTI0MDgzMTEwNTEzM1owTjELMAkGA1UEBhMCU0ExEzARBgNVBAoTCjMxMDIzMzM3NDYxDDAKBgNVBAsTA1RTVDEcMBoGA1UEAxMTVFNULTMxMDIzMzM3NDYwMDAwMzBWMBAGByqGSM49AgEGBSuBBAAKA0IABGGDDKDmhWAITDv7LXqLX2cmr6+qddUkpcLCvWs5rC2O29W/hS4ajAK4Qdnahym6MaijX75Cg3j4aao7ouYXJ9GjggI5MIICNTCBmgYDVR0RBIGSMIGPpIGMMIGJMTswOQYDVQQEDDIxLVRTVHwyLVRTVHwzLTFhZDZiZjAwLTA2ZWQtNGYxZS1iYWY3LTFkNjIxNGY1Njc3NjEfMB0GCgmSJomT8ixkAQEMDzMxMDIzMzM3NDYwMDAwMzENMAsGA1UEDAwEMTAwMDEMMAoGA1UEGgwDVFNUMQwwCgYDVQQPDANUU1QwHQYDVR0OBBYEFDuWYlOzWpFN3no1WtyNktQdrA8JMB8GA1UdIwQYMBaAFHZgjPsGoKxnVzWdz5qspyuZNbUvME4GA1UdHwRHMEUwQ6BBoD+GPWh0dHA6Ly90c3RjcmwuemF0Y2EuZ292LnNhL0NlcnRFbnJvbGwvVFNaRUlOVk9JQ0UtU3ViQ0EtMS5jcmwwga0GCCsGAQUFBwEBBIGgMIGdMG4GCCsGAQUFBzABhmJodHRwOi8vdHN0Y3JsLnphdGNhLmdvdi5zYS9DZXJ0RW5yb2xsL1RTWkVpbnZvaWNlU0NBMS5leHRnYXp0Lmdvdi5sb2NhbF9UU1pFSU5WT0lDRS1TdWJDQS0xKDEpLmNydDArBggrBgEFBQcwAYYfaHR0cDovL3RzdGNybC56YXRjYS5nb3Yuc2Evb2NzcDAOBgNVHQ8BAf8EBAMCB4AwHQYDVR0lBBYwFAYIKwYBBQUHAwIGCCsGAQUFBwMDMCcGCSsGAQQBgjcVCgQaMBgwCgYIKwYBBQUHAwIwCgYIKwYBBQUHAwMwCgYIKoZIzj0EAwIDSQAwRgIhAL7U7Mnk0MPG/EBShq5LZxx23ZfLnLSG3ufy5s8OKaTuAiEAtDtWqC3Xn0EeYLO+t7xd6ko/r0qU09N7VMd9Rea55dI="
                //   , "MHQCAQEEIDyLDaWIn/1/g3PGLrwupV4nTiiLKM59UEqUch1vDfhpoAcGBSuBBAAKoUQDQgAEYYMMoOaFYAhMO/steotfZyavr6p11SSlwsK9azmsLY7b1b+FLhqMArhB2dqHKboxqKNfvkKDePhpqjui5hcn0Q==");

                bp.log("Signing end", 0, typ);
                bool isok = rrrr1.IsValid;

                for (int i = 0; i < rrrr1.lstSteps.Count; i++)
                {
                    bp.log(rrrr1.lstSteps[i].Operation + ":" + rrrr1.lstSteps[i].IsValid, 1, typ);
                    isok = rrrr1.lstSteps[i].IsValid;
                }

                bp.log("IsValid:" + rrrr1.IsValid, 1, typ);

                string fname = GetFileName_withoutext(bd);

                string filePath1 = "";
                if (isok == true)  //sigining success
                {
                    string ff = Get_Folder(docType, src, XMLSTATUS.SIGNED, "");
                    filePath1 = SaveFileNormal(rrrr1.ResultedValue, fname, ff, typ);

                    Result res1 = _IQRValidator.ValidateEInvoiceQRCode(filePath1);
                    bp.log("QrCodeValidator isValid:" + res1.IsValid, 1, typ);

                    Result res = _IHashingValidator.ValidateEInvoiceHashing(filePath1);
                    bp.log("HashingValidator isValid:" + res.IsValid, 1, typ);

                    RET.RESPMSG = "SUCCESS";
                    RET._actionStatus = ACTION_STATUS.Signed;


                    //new JobZatca().Send_to_Reporting(filePath1, LOGTYPE.JOB);  //for testing, to be removed.

                }
                else       //sigining failed -rrrr1.ResultedValue is null
                {
                    // string ff = Get_Folder(docType, src, XMLSTATUS.FAILED);
                    //  filePath1 = SaveFileNormal(rrrr1.ResultedValue, fname, ff, typ);

                    RET.RESPMSG = RET.RESPMSG == "" ? "FAILED" : RET.RESPMSG;
                    RET._actionStatus = ACTION_STATUS.SigningFailed;

                    //Send email
                    string err = "Sigining failed for xml " + xmlFilePath;
                    bp.SendAlertEmail(docType == DOCTYPE.B2B ? MailType.ClearanceError : MailType.ReportingError, err, docType, "");

                }

                if (rrrr1.ResultedValue != null && rrrr1.ResultedValue != "")
                {
                    RET.change_icv_pih = true;
                    RET = Get_Signed_Fields_XML(filePath1, typ, RET);
                }







                RET.InvReferenceNumber = bd.irn;
                bp.log("Signing result=>IRN:" + bd.irn + "\r\nQR:" + RET.QR + "\r\nInvoiceHash:" + RET.InvoiceHash, 1, typ);


            }
            catch (Exception ex)
            {
                bp.log("SignXML method error:" + ex.ToString(), 1, typ);
                RET.RESPMSG = RET.RESPMSG == "" ? "FAILED" : RET.RESPMSG;
                RET.RC = 1;

            }
            return RET;
        }

        public static string SaveFileNormal(string xml, string filename, string folderpath, LOGTYPE lgtype)
        {

            try
            {
                var _IHashingValidator = new HashingValidator();
                var _IQRValidator = new QRValidator();
                var _IEInvoiceValidator = new EInvoiceValidator();
                var _IEInvoiceSigningLogic = new EInvoiceSigningLogic();


                string extension = ".xml";

                string filePath1 = folderpath + filename + extension;

                string ff = filePath1.Replace(extension, ".tmp");
                // Create a new file     
                using (FileStream fs = File.Create(ff))
                {
                    Byte[] title = new UTF8Encoding(true).GetBytes(xml);
                    fs.Write(title, 0, title.Length);

                }

                string tt = "file created. fileName:" + Path.GetFileName(ff);
                new Basepage().logWrite(tt, lgtype);

                if (File.Exists(filePath1))
                {
                    string s = DateTime.Now.ToString("mmss");
                    File.Move(filePath1, Path.ChangeExtension(filePath1, "." + s));
                }
                System.IO.File.Move(ff, filePath1);

                tt = "file renamed from .tmp to " + extension;
                new Basepage().logWrite(tt, lgtype);


                if (Basepage.logDebug == 1)
                {
                    Result res = _IQRValidator.ValidateEInvoiceQRCode(filePath1);
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


        public string GetFileName_withoutext(FileNameFields bd)
        {
            //Seller Identification + ”_” + Date + ”T” + Time + ”_” + IRN.xml

            string partyTaxSchemeCompanyId = bd?.partyTaxSchemeCompanyId ?? "";
            string IssueDate_YYYY_MM_DD = bd?.IssueDate_YYYY_MM_DD ?? "";
            string IssueTime_HH_MM_SS = bd?.IssueTime_HH_MM_SS ?? DateTime.Now.ToString("ddMMyyHHmmss");
            string irn = bd?.irn ?? "";


            string fname = partyTaxSchemeCompanyId + "_" +
                            IssueDate_YYYY_MM_DD.Replace("-", "") + "T" +
                            IssueTime_HH_MM_SS.Replace(":", "") + "_" +
                            irn;

            return fname;

        }



        public Generated_OUT generateXml(BaseData.Root baseData, LOGTYPE typ, FileNameFields ff, string validationError, SOURCE TrxSource)
        {

            try
            {
                Generated_OUT RET = new Generated_OUT();

                string xmlns = NS_XMLNS;
                string cbc = NS_CBC;
                string cac = NS_CAC;
                string ext = NS_EXT;
                // root XmlElement
                XmlDocument doc = new XmlDocument();
                XmlElement rootXmlElement = doc.CreateElement("Invoice");

                rootXmlElement.SetAttribute("xmlns", xmlns);
                rootXmlElement.SetAttribute("xmlns:cac", cac);
                rootXmlElement.SetAttribute("xmlns:cbc", cbc);
                rootXmlElement.SetAttribute("xmlns:ext", ext);

                doc.AppendChild(rootXmlElement);

                // staff XmlElements
                XmlElement profileId = doc.CreateElement("cbc", "ProfileID", cbc);
                profileId.AppendChild(doc.CreateTextNode("reporting:1.0"));
                rootXmlElement.AppendChild(profileId);

                XmlElement id = doc.CreateElement("cbc", "ID", cbc);
                id.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.ID.arb));
                rootXmlElement.AppendChild(id);

                XmlElement uuid = doc.CreateElement("cbc", "UUID", cbc);
                uuid.AppendChild(doc.CreateTextNode(baseData.Invoice.InvoiceUUID));
                rootXmlElement.AppendChild(uuid);

                XmlElement issueDate = doc.CreateElement("cbc", "IssueDate", cbc);
                issueDate.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.IssueDate));
                rootXmlElement.AppendChild(issueDate);

                XmlElement issueTime = doc.CreateElement("cbc", "IssueTime", cbc);
                issueTime.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.IssueTime));
                rootXmlElement.AppendChild(issueTime);

                XmlElement invoiceTypeCode = doc.CreateElement("cbc", "InvoiceTypeCode", cbc);
                invoiceTypeCode.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.InvoiceTypeCode.value));
                System.Xml.XmlAttribute nameAttr = doc.CreateAttribute("name");
                nameAttr.Value = (baseData.Invoice.EInvoice.InvoiceTypeCode.name);
                invoiceTypeCode.SetAttributeNode(nameAttr);
                rootXmlElement.AppendChild(invoiceTypeCode);

                XmlElement documentCurrencyCode = doc.CreateElement("cbc", "DocumentCurrencyCode", cbc);
                documentCurrencyCode.AppendChild(doc.CreateTextNode("SAR"));
                rootXmlElement.AppendChild(documentCurrencyCode);

                XmlElement taxCurrencyCode = doc.CreateElement("cbc", "TaxCurrencyCode", cbc);
                taxCurrencyCode.AppendChild(doc.CreateTextNode("SAR"));
                rootXmlElement.AppendChild(taxCurrencyCode);

                if (baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN)
                {
                    XmlElement billingReference = doc.CreateElement("cac", "BillingReference", cac);
                    for (int i = 0; i < baseData.Invoice.EInvoice.BillingReference.Count; i++)
                    {
                        BaseData.BillingReference bb = baseData.Invoice.EInvoice.BillingReference[i];
                        XmlElement invoiceDocumentReference = doc.CreateElement("cac", "InvoiceDocumentReference", cac);
                        XmlElement invoiceDocumentReferenceId = doc.CreateElement("cbc", "ID", cbc);
                        invoiceDocumentReferenceId.AppendChild(doc.CreateTextNode(bb.InvoiceDocumentReference.ID.arb));
                        invoiceDocumentReference.AppendChild(invoiceDocumentReferenceId);
                        billingReference.AppendChild(invoiceDocumentReference);
                    }

                    //if (!"".Equals(PropertyAccessor.valueOrDefault(() -> baseData.getBillingReference().getIssueDate(), ""))) {
                    //                     XmlElement invoiceDocumentReferenceIssueDate = doc.CreateElement("cbc","IssueDate");
                    //                     invoiceDocumentReferenceIssueDate.AppendChild(doc.CreateTextNode(baseData.getBillingReference().getIssueDate()));
                    //                     invoiceDocumentReference.AppendChild(invoiceDocumentReferenceIssueDate);
                    //}

                    rootXmlElement.AppendChild(billingReference);
                }

                XmlElement additionalDocumentReferenceICV = doc.CreateElement("cac", "AdditionalDocumentReference", cac);
                XmlElement additionalDocumentReferenceICVId = doc.CreateElement("cbc", "ID", cbc);
                additionalDocumentReferenceICVId.AppendChild(doc.CreateTextNode("ICV"));
                additionalDocumentReferenceICV.AppendChild(additionalDocumentReferenceICVId);
                XmlElement additionalDocumentReferenceICVUuid = doc.CreateElement("cbc", "UUID", cbc);
                additionalDocumentReferenceICVUuid.AppendChild(doc.CreateTextNode(baseData.Invoice.ICV));
                additionalDocumentReferenceICV.AppendChild(additionalDocumentReferenceICVUuid);
                rootXmlElement.AppendChild(additionalDocumentReferenceICV);

                XmlElement additionalDocumentReferencePIH = doc.CreateElement("cac", "AdditionalDocumentReference", cac);
                XmlElement additionalDocumentReferencePIHId = doc.CreateElement("cbc", "ID", cbc);
                additionalDocumentReferencePIHId.AppendChild(doc.CreateTextNode("PIH"));
                additionalDocumentReferencePIH.AppendChild(additionalDocumentReferencePIHId);
                XmlElement additionalDocumentReferencePIHAttachment = doc.CreateElement("cac", "Attachment", cac);
                XmlElement embeddedDocumentBinaryObjectPIH = doc.CreateElement("cbc", "EmbeddedDocumentBinaryObject", cbc);
                embeddedDocumentBinaryObjectPIH.AppendChild(doc.CreateTextNode(baseData.Invoice.PIH));
                System.Xml.XmlAttribute mimeCodePIH = doc.CreateAttribute("mimeCode");
                mimeCodePIH.Value = ("text/plain");
                embeddedDocumentBinaryObjectPIH.SetAttributeNode(mimeCodePIH);
                additionalDocumentReferencePIHAttachment.AppendChild(embeddedDocumentBinaryObjectPIH);
                additionalDocumentReferencePIH.AppendChild(additionalDocumentReferencePIHAttachment);
                rootXmlElement.AppendChild(additionalDocumentReferencePIH);

                // XmlElement additionalDocumentReferenceQR = doc.CreateElement("cac","AdditionalDocumentReference");
                // XmlElement additionalDocumentReferenceQRId = doc.CreateElement("cbc","ID");
                //  additionalDocumentReferenceQRId.AppendChild(doc.CreateTextNode("QR"));
                //additionalDocumentReferenceQR.AppendChild(additionalDocumentReferenceQRId);
                // XmlElement additionalDocumentReferenceQRAttachment = doc.CreateElement("cac","Attachment");
                //     XmlElement embeddedDocumentBinaryObjectQR = doc.CreateElement("cbc","EmbeddedDocumentBinaryObject");
                //     embeddedDocumentBinaryObjectQR.AppendChild(doc.CreateTextNode(baseData.getAdditionalDocumentReference().getEmbeddedDocumentBinaryObject()));
                //        Attr mimeCodeQR = doc.CreateAttribute("mimeCode");
                //        mimeCodeQR.Value=("text/plain");
                //        embeddedDocumentBinaryObjectPIH.SetAttributeNode(mimeCodeQR);
                //    additionalDocumentReferenceQRAttachment.AppendChild(embeddedDocumentBinaryObjectQR);
                // additionalDocumentReferenceQR.AppendChild(additionalDocumentReferenceQRAttachment);
                // rootXmlElement.AppendChild(additionalDocumentReferenceQR);

                XmlElement accountingSupplierParty = doc.CreateElement("cac", "AccountingSupplierParty", cac);
                XmlElement party = doc.CreateElement("cac", "Party", cac);
                // if (!"".Equals(baseData.getAccountingSupplierParty().getPartyIdentification().getId(), "")))
                {
                    XmlElement partyIdentification = doc.CreateElement("cac", "PartyIdentification", cac);
                    XmlElement partyIdentificationId = doc.CreateElement("cbc", "ID", cbc);
                    partyIdentificationId.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PartyIdentification.ID.value));
                    //    if (!"".Equals(baseData.getAccountingSupplierParty().getPartyIdentification().getIdSchemeId(), "")))
                    {
                        XmlAttribute schemeIDAttr = doc.CreateAttribute("schemeID");
                        schemeIDAttr.Value = (baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PartyIdentification.ID.schemeID);
                        partyIdentificationId.SetAttributeNode(schemeIDAttr);
                    }
                    partyIdentification.AppendChild(partyIdentificationId);
                    party.AppendChild(partyIdentification);
                }
                XmlElement postalAddress = doc.CreateElement("cac", "PostalAddress", cac);
                XmlElement streetName = doc.CreateElement("cbc", "StreetName", cbc);
                streetName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PostalAddress.StreetName.arb));
                postalAddress.AppendChild(streetName);
                XmlElement buildingNumber = doc.CreateElement("cbc", "BuildingNumber", cbc);
                buildingNumber.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PostalAddress.BuildingNumber.arb));
                postalAddress.AppendChild(buildingNumber);
                XmlElement plotIdentification = doc.CreateElement("cbc", "PlotIdentification", cbc);
                plotIdentification.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PostalAddress.PlotIdentification.arb));
                postalAddress.AppendChild(plotIdentification);
                XmlElement citySubdivisionName = doc.CreateElement("cbc", "CitySubdivisionName", cbc);
                citySubdivisionName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PostalAddress.CitySubdivisionName.arb));
                postalAddress.AppendChild(citySubdivisionName);
                XmlElement cityName = doc.CreateElement("cbc", "CityName", cbc);
                cityName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PostalAddress.CityName.arb));
                postalAddress.AppendChild(cityName);
                XmlElement postalZone = doc.CreateElement("cbc", "PostalZone", cbc);
                postalZone.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PostalAddress.PostalZone));
                postalAddress.AppendChild(postalZone);
                //             XmlElement countrySubentity = doc.CreateElement("cbc","CountrySubentity");
                //             countrySubentity.AppendChild(doc.CreateTextNode(baseData.getAccountingSupplierParty().getPostalAddress().getCountrySubentity()));
                //             postalAddress.AppendChild(countrySubentity);

                XmlElement country = doc.CreateElement("cac", "Country", cac);
                XmlElement identificationCode = doc.CreateElement("cbc", "IdentificationCode", cbc);
                identificationCode.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PostalAddress.Country.IdentificationCode));
                country.AppendChild(identificationCode);
                postalAddress.AppendChild(country);
                party.AppendChild(postalAddress);
                XmlElement partyTaxScheme = doc.CreateElement("cac", "PartyTaxScheme", cac);
                XmlElement partyTaxSchemePartyCompanyID = doc.CreateElement("cbc", "CompanyID", cbc);
                partyTaxSchemePartyCompanyID.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PartyTaxScheme.CompanyID));
                partyTaxScheme.AppendChild(partyTaxSchemePartyCompanyID);
                XmlElement taxScheme = doc.CreateElement("cac", "TaxScheme", cac);
                XmlElement taxSchemeID = doc.CreateElement("cbc", "ID", cbc);
                taxSchemeID.AppendChild(doc.CreateTextNode("VAT"));
                taxScheme.AppendChild(taxSchemeID);
                partyTaxScheme.AppendChild(taxScheme);
                party.AppendChild(partyTaxScheme);
                XmlElement partyLegalEntity = doc.CreateElement("cac", "PartyLegalEntity", cac);
                XmlElement partyLegalEntityRegistrationName = doc.CreateElement("cbc", "RegistrationName", cbc);
                partyLegalEntityRegistrationName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingSupplierParty.Party.PartyLegalEntity.RegistrationName.arb));
                partyLegalEntity.AppendChild(partyLegalEntityRegistrationName);
                party.AppendChild(partyLegalEntity);
                accountingSupplierParty.AppendChild(party);
                rootXmlElement.AppendChild(accountingSupplierParty);

                //needs to be present for all, but   pass empty fields.																	
                //if (baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE) 
                {
                    XmlElement accountingCustomerParty = doc.CreateElement("cac", "AccountingCustomerParty", cac);
                    XmlElement accountingCustomerPartyParty = doc.CreateElement("cac", "Party", cac);
                    if (!"".Equals(baseData?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PartyIdentification?.ID?.value ?? ""))
                    {
                        XmlElement partyIdentification = doc.CreateElement("cac", "PartyIdentification", cac);
                        XmlElement partyIdentificationId = doc.CreateElement("cbc", "ID", cbc);
                        partyIdentificationId.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyIdentification.ID.value));
                        if (!"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyIdentification.ID.schemeID))
                        {
                            XmlAttribute schemeIDAttr = doc.CreateAttribute("schemeID");
                            schemeIDAttr.Value = (baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyIdentification.ID.schemeID);
                            partyIdentificationId.SetAttributeNode(schemeIDAttr);
                        }
                        partyIdentification.AppendChild(partyIdentificationId);
                        accountingCustomerPartyParty.AppendChild(partyIdentification);
                    }
                    XmlElement accountingCustomerPartyPostalAddress = doc.CreateElement("cac", "PostalAddress", cac);
                    //if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.StreetName.arb ))
                    //     || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) &&
                    //     !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.StreetName.arb ))
                    //     || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)

                    if ((baseData?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.StreetName?.arb ?? "").Equals("") == false)
                    {
                        XmlElement accountingCustomerPartyStreetName = doc.CreateElement("cbc", "StreetName", cbc);
                        accountingCustomerPartyStreetName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.StreetName.arb));
                        accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyStreetName);
                    }
                    //if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.AdditionalStreetName.arb )) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.AdditionalStreetName.arb )) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
                    if ((baseData?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.AdditionalStreetName?.arb ?? "").Equals("") == false)
                    {
                        XmlElement accountingCustomerPartyAdditionalStreetName = doc.CreateElement("cbc", "AdditionalStreetName", cbc);
                        accountingCustomerPartyAdditionalStreetName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.AdditionalStreetName.arb));
                        accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyAdditionalStreetName);
                    }
                    ///  if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.BuildingNumber.arb )) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.BuildingNumber.arb )) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
                    if ((baseData?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.BuildingNumber?.arb ?? "").Equals("") == false)
                    {
                        XmlElement accountingCustomerPartyBuildingNumber = doc.CreateElement("cbc", "BuildingNumber", cbc);
                        accountingCustomerPartyBuildingNumber.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.BuildingNumber.arb));
                        accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyBuildingNumber);
                    }
                    //   if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CitySubdivisionName.arb )) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CitySubdivisionName.arb )) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
                    if ((baseData?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.CitySubdivisionName?.arb ?? "").Equals("") == false)
                    {
                        XmlElement accountingCustomerPartyCitySubdivisionName = doc.CreateElement("cbc", "CitySubdivisionName", cbc);
                        accountingCustomerPartyCitySubdivisionName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CitySubdivisionName.arb));
                        accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyCitySubdivisionName);
                    }
                    // if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.PlotIdentification.arb )) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.PlotIdentification.arb )) || ((baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.PlotIdentification.arb )))
                    if ((baseData?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.PlotIdentification?.arb ?? "").Equals("") == false)
                    {
                        XmlElement accountingCustomerPartyPlotIdentification = doc.CreateElement("cbc", "PlotIdentification", cbc);
                        accountingCustomerPartyPlotIdentification.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.PlotIdentification.arb));
                        accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyPlotIdentification);
                    }
                    // if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CityName.arb )) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CityName.arb )) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
                    if ((baseData?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.CityName?.arb ?? "").Equals("") == false)
                    {
                        XmlElement accountingCustomerPartyCityName = doc.CreateElement("cbc", "CityName", cbc);
                        accountingCustomerPartyCityName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CityName.arb));
                        accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyCityName);
                    }
                    // if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.PostalZone)) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.PostalZone)) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
                    if ((baseData?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.PostalZone ?? "").Equals("") == false)
                    {
                        XmlElement accountingCustomerPartyPostalZone = doc.CreateElement("cbc", "PostalZone", cbc);
                        accountingCustomerPartyPostalZone.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.PostalZone));
                        accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyPostalZone);
                    }
                    // if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CountrySubentity.arb )) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CountrySubentity)) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
                    if ((baseData?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.CountrySubentity?.arb ?? "").Equals("") == false)
                    {
                        XmlElement accountingCustomerPartyCountrySubentity = doc.CreateElement("cbc", "CountrySubentity", cbc);
                        accountingCustomerPartyCountrySubentity.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.CountrySubentity.arb));
                        accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyCountrySubentity);
                    }

                    //  if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.Country.IdentificationCode)) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.Country.IdentificationCode)) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
                    if ((baseData?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PostalAddress?.Country?.IdentificationCode ?? "").Equals("") == false)
                    {
                        XmlElement accountingCustomerPartyCountry = doc.CreateElement("cac", "Country", cac);
                        XmlElement accountingCustomerPartyIdentificationCode = doc.CreateElement("cbc", "IdentificationCode", cbc);
                        accountingCustomerPartyIdentificationCode.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PostalAddress.Country.IdentificationCode));
                        accountingCustomerPartyCountry.AppendChild(accountingCustomerPartyIdentificationCode);
                        accountingCustomerPartyPostalAddress.AppendChild(accountingCustomerPartyCountry);
                    }
                    accountingCustomerPartyParty.AppendChild(accountingCustomerPartyPostalAddress);






                    XmlElement accountingCustomerPartyPartyTaxScheme = doc.CreateElement("cac", "PartyTaxScheme", cac);

                    if ((baseData?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PartyTaxScheme?.CompanyID ?? "").Equals("") == false)
                    {
                        XmlElement accountingCustomerPartypartyTaxSchemePartyCompanyID = doc.CreateElement("cbc", "CompanyID", cbc);
                        accountingCustomerPartypartyTaxSchemePartyCompanyID.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyTaxScheme.CompanyID));
                        accountingCustomerPartyPartyTaxScheme.AppendChild(accountingCustomerPartypartyTaxSchemePartyCompanyID);
                    }


                    XmlElement accountingCustomerPartyTaxScheme = doc.CreateElement("cac", "TaxScheme", cac);
                    XmlElement accountingCustomerPartyTaxSchemeID = doc.CreateElement("cbc", "ID", cbc);
                    accountingCustomerPartyTaxSchemeID.AppendChild(doc.CreateTextNode("VAT"));
                    accountingCustomerPartyTaxScheme.AppendChild(accountingCustomerPartyTaxSchemeID);
                    accountingCustomerPartyPartyTaxScheme.AppendChild(accountingCustomerPartyTaxScheme);
                    accountingCustomerPartyParty.AppendChild(accountingCustomerPartyPartyTaxScheme);
                    //if (((baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyLegalEntity.RegistrationName)) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyLegalEntity.RegistrationName)) || ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_INVOICE) && !"".Equals(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyLegalEntity.RegistrationName)) || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE)
                    if ((baseData?.Invoice?.EInvoice?.AccountingCustomerParty?.Party?.PartyLegalEntity?.RegistrationName?.arb ?? "").Equals("") == false)
                    {
                        XmlElement accountingCustomerPartyLegalEntity = doc.CreateElement("cac", "PartyLegalEntity", cac);
                        XmlElement accountingCustomerPartyRegistrationName = doc.CreateElement("cbc", "RegistrationName", cbc);
                        accountingCustomerPartyRegistrationName.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.AccountingCustomerParty.Party.PartyLegalEntity.RegistrationName.arb));
                        accountingCustomerPartyLegalEntity.AppendChild(accountingCustomerPartyRegistrationName);
                        accountingCustomerPartyParty.AppendChild(accountingCustomerPartyLegalEntity);
                    }
                    accountingCustomerParty.AppendChild(accountingCustomerPartyParty);
                    rootXmlElement.AppendChild(accountingCustomerParty);
                }


                XmlElement delivery = doc.CreateElement("cac", "Delivery", cac);
                if (baseData.Invoice.EInvoice?.Delivery != null && baseData.Invoice.EInvoice?.Delivery?.Count > 0)
                {
                    bool append = false;

                    for (int i = 0; i < baseData.Invoice.EInvoice?.Delivery?.Count; i++)
                    {

                        BaseData.Delivery bb = baseData.Invoice.EInvoice.Delivery[i];
                        if (!bb.ActualDeliveryDate.Trim().Equals(""))
                        {
                            XmlElement actualDeliveryDate = doc.CreateElement("cbc", "ActualDeliveryDate", cbc);
                            actualDeliveryDate.AppendChild(doc.CreateTextNode(bb.ActualDeliveryDate));
                            delivery.AppendChild(actualDeliveryDate);

                            append = true;
                        }
                        if (!bb.LatestDeliveryDate.Trim().Equals(""))
                        {
                            XmlElement LatestDeliveryDate = doc.CreateElement("cbc", "LatestDeliveryDate", cbc);
                            LatestDeliveryDate.AppendChild(doc.CreateTextNode(bb.LatestDeliveryDate));
                            delivery.AppendChild(LatestDeliveryDate);

                            append = true;
                        }

                        if (append) rootXmlElement.AppendChild(delivery);
                    }
                }


                for (int i = 0; i < baseData.Invoice.EInvoice.PaymentMeans.Count; i++)
                {
                    BaseData.PaymentMean pm = baseData.Invoice.EInvoice.PaymentMeans[i];
                    XmlElement paymentMeans = doc.CreateElement("cac", "PaymentMeans", cac);
                    // if ((baseData.Invoice.InvoiceType == InvoiceType.SIMP_INVOICE && !"".Equals(PropertyAccessor.valueOrDefault(() -> baseData.getPaymentMeans().getPaymentMeansCode(), ""))) || baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE) {
                    XmlElement paymentMeansCode = doc.CreateElement("cbc", "PaymentMeansCode", cbc);
                    paymentMeansCode.AppendChild(doc.CreateTextNode(pm.PaymentMeansCode));
                    paymentMeans.AppendChild(paymentMeansCode);

                    //}

                    if (baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN)
                    {
                        XmlElement InstructionNote = doc.CreateElement("cbc", "InstructionNote", cbc);
                        InstructionNote.AppendChild(doc.CreateTextNode(pm.InstructionNote.arb));
                        paymentMeans.AppendChild(InstructionNote);
                    }


                    rootXmlElement.AppendChild(paymentMeans);
                }




                if (baseData.Invoice.EInvoice.AllowanceCharge.Count > 0)
                {
                    for (int i = 0; i < baseData.Invoice.EInvoice.AllowanceCharge.Count; i++)
                    {
                        BaseData.AllowanceCharge ac = baseData.Invoice.EInvoice.AllowanceCharge[i];
                        if (!"".Equals(ac?.Amount?.value ?? ""))
                        {
                            XmlElement allowanceCharge = doc.CreateElement("cac", "AllowanceCharge", cac);
                            if (!"".Equals(ac.ChargeIndicator))
                            {
                                XmlElement chargeIndicator = doc.CreateElement("cbc", "ChargeIndicator", cbc);
                                chargeIndicator.AppendChild(doc.CreateTextNode(ac.ChargeIndicator));
                                allowanceCharge.AppendChild(chargeIndicator);
                            }
                            if (ac.ChargeIndicator.ToLower().Equals("true"))  //add reason codes only for document level charges, not discounts
                            {
                                if (!"".Equals(ac.AllowanceChargeReasonCode))
                                {
                                    XmlElement allowanceChargeReasonCode = doc.CreateElement("cbc", "AllowanceChargeReasonCode", cbc);
                                    allowanceChargeReasonCode.AppendChild(doc.CreateTextNode(ac.AllowanceChargeReasonCode));
                                    allowanceCharge.AppendChild(allowanceChargeReasonCode);
                                }
                                if (!"".Equals(ac.AllowanceChargeReason))
                                {
                                    XmlElement allowanceChargeReason = doc.CreateElement("cbc", "AllowanceChargeReason", cbc);
                                    allowanceChargeReason.AppendChild(doc.CreateTextNode(ac.AllowanceChargeReason.arb));
                                    allowanceCharge.AppendChild(allowanceChargeReason);
                                }
                            }
                               
                            if (!"".Equals(ac.MultiplierFactorNumeric))
                            {
                                XmlElement multiplierFactorNumeric = doc.CreateElement("cbc", "MultiplierFactorNumeric", cbc);
                                multiplierFactorNumeric.AppendChild(doc.CreateTextNode(ac.MultiplierFactorNumeric));
                                allowanceCharge.AppendChild(multiplierFactorNumeric);
                            }
                            if (!"".Equals(ac.Amount))
                            {
                                XmlElement amount = doc.CreateElement("cbc", "Amount", cbc);
                                amount.AppendChild(doc.CreateTextNode(ac.Amount.value));
                                if (!"".Equals(ac.Amount.currencyID))
                                {
                                    XmlAttribute amountcurrencyIDAttr = doc.CreateAttribute("currencyID");
                                    amountcurrencyIDAttr.Value = (ac.Amount.currencyID);
                                    amount.SetAttributeNode(amountcurrencyIDAttr);
                                }
                                allowanceCharge.AppendChild(amount);
                            }
                            if (!"".Equals(ac.BaseAmount.value))
                            {
                                XmlElement baseAmount = doc.CreateElement("cbc", "BaseAmount", cbc);
                                baseAmount.AppendChild(doc.CreateTextNode(ac.BaseAmount.value));
                                if (!"".Equals(ac.BaseAmount.currencyID))
                                {
                                    XmlAttribute baseAmountcurrencyIDAttr = doc.CreateAttribute("currencyID");
                                    baseAmountcurrencyIDAttr.Value = (ac.BaseAmount.currencyID);
                                    baseAmount.SetAttributeNode(baseAmountcurrencyIDAttr);
                                }
                                allowanceCharge.AppendChild(baseAmount);
                            }

                            if (!"".Equals(ac.TaxCategory.Percent))
                            {
                                XmlElement taxCategoryElement = doc.CreateElement("cac", "TaxCategory", cac);
                                if (!"".Equals(ac.TaxCategory.ID.arb))
                                {
                                    XmlElement taxCategoryIdElement = doc.CreateElement("cbc", "ID", cbc);
                                    taxCategoryIdElement.AppendChild(doc.CreateTextNode(ac.TaxCategory.ID.arb));
                                    if (!"".Equals(ac.TaxCategory.ID.schemeAgencyID))
                                    {
                                        XmlAttribute schemeAgencyIDAttr = doc.CreateAttribute("schemeAgencyID");
                                        schemeAgencyIDAttr.Value = (ac.TaxCategory.ID.schemeAgencyID);
                                        taxCategoryIdElement.SetAttributeNode(schemeAgencyIDAttr);
                                    }
                                    if (!"".Equals(ac.TaxCategory.ID.schemeID))
                                    {
                                        XmlAttribute schemeIDAttr = doc.CreateAttribute("schemeID");
                                        schemeIDAttr.Value = (ac.TaxCategory.ID.schemeID);
                                        taxCategoryIdElement.SetAttributeNode(schemeIDAttr);
                                    }
                                    taxCategoryElement.AppendChild(taxCategoryIdElement);
                                }
                                if (!"".Equals(ac.TaxCategory.Percent))
                                {
                                    XmlElement percentXmlElement = doc.CreateElement("cbc", "Percent", cbc);
                                    percentXmlElement.AppendChild(doc.CreateTextNode(ac.TaxCategory.Percent));
                                    taxCategoryElement.AppendChild(percentXmlElement);
                                }
                                if (!"".Equals(ac.TaxCategory.TaxScheme.ID.arb))
                                {
                                    XmlElement taxSchemeXmlElement = doc.CreateElement("cac", "TaxScheme", cac);
                                    XmlElement taxSchemeIdXmlElement = doc.CreateElement("cbc", "ID", cbc);
                                    taxSchemeIdXmlElement.AppendChild(doc.CreateTextNode(ac.TaxCategory.TaxScheme.ID.arb));

                                    //if (!"".Equals(ac.TaxCategory.TaxScheme.ID.schemeAgencyID))
                                    //{
                                    //    XmlAttribute schemeAgencyIDAttr = doc.CreateAttribute("schemeAgencyID");
                                    //    schemeAgencyIDAttr.Value = (ac.TaxCategory.TaxScheme.ID.schemeAgencyID);
                                    //    taxSchemeIdXmlElement.SetAttributeNode(schemeAgencyIDAttr);
                                    //}
                                    //if (!"".Equals(ac.TaxCategory.TaxScheme.ID.schemeID))
                                    //{
                                    //    XmlAttribute schemeIDAttr = doc.CreateAttribute("schemeID");
                                    //    schemeIDAttr.Value = (ac.TaxCategory.TaxScheme.ID.schemeID);
                                    //    taxSchemeIdXmlElement.SetAttributeNode(schemeIDAttr);
                                    //}

                                    taxSchemeXmlElement.AppendChild(taxSchemeIdXmlElement);
                                    taxCategoryElement.AppendChild(taxSchemeXmlElement);
                                }
                                allowanceCharge.AppendChild(taxCategoryElement);
                            }
                            rootXmlElement.AppendChild(allowanceCharge);
                        }
                    }
                }

                //tax is not applicable for Export Invoice. So tax details will not be available.TaxTotal array will be empty only
                //if (baseData.Invoice.EInvoice.TaxTotal.Count != 2)
                //    throw new InvalidCastException(ERR.custerr_TaxTotal_array_must_contain_two_items_json.ToString());


                string taxtotal = "";
                for (int k = 0; k < baseData.Invoice.EInvoice.TaxTotal.Count; k++)
                {
                    BaseData.TaxTotal tt = baseData.Invoice.EInvoice.TaxTotal[k];
                    XmlElement taxTotalElement = doc.CreateElement("cac", "TaxTotal", cac);
                    XmlElement taxAmountElement = doc.CreateElement("cbc", "TaxAmount", cbc);
                    taxAmountElement.AppendChild(doc.CreateTextNode(tt.TaxAmount.value));
                    XmlAttribute taxAmountElementCurrencyIDAttr = doc.CreateAttribute("currencyID");
                    taxAmountElementCurrencyIDAttr.Value = (tt.TaxAmount.currencyID);
                    taxAmountElement.SetAttributeNode(taxAmountElementCurrencyIDAttr);
                    taxTotalElement.AppendChild(taxAmountElement);

                    if (tt.TaxSubtotal == null)
                    {
                        RET.TaxTotal = tt.TaxAmount.value;
                    }

                    if (tt.TaxSubtotal != null && tt.TaxSubtotal.Count > 0)
                    {

                        for (int j = 0; j < tt.TaxSubtotal.Count; j++)
                        {
                            BaseData.TaxSubtotal tst = tt.TaxSubtotal[j];
                            XmlElement taxSubtotal = doc.CreateElement("cac", "TaxSubtotal", cac);
                            XmlElement taxableAmount = doc.CreateElement("cbc", "TaxableAmount", cbc);
                            taxableAmount.AppendChild(doc.CreateTextNode(tst.TaxableAmount.value));
                            XmlAttribute taxableAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
                            taxableAmountCurrencyIDAttr.Value = (tst.TaxableAmount.currencyID);
                            taxableAmount.SetAttributeNode(taxableAmountCurrencyIDAttr);
                            taxSubtotal.AppendChild(taxableAmount);

                            XmlElement taxSubtotalTaxAmount = doc.CreateElement("cbc", "TaxAmount", cbc);
                            taxSubtotalTaxAmount.AppendChild(doc.CreateTextNode(tst.TaxAmount.value));
                            XmlAttribute taxSubtotalTaxAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
                            taxSubtotalTaxAmountCurrencyIDAttr.Value = (tst.TaxAmount.currencyID);
                            taxSubtotalTaxAmount.SetAttributeNode(taxSubtotalTaxAmountCurrencyIDAttr);
                            taxSubtotal.AppendChild(taxSubtotalTaxAmount);

                            XmlElement taxSubtotaltaxCategoryElement = doc.CreateElement("cac", "TaxCategory", cac);
                            XmlElement taxSubtotaltaxCategoryIdElement = doc.CreateElement("cbc", "ID", cbc);
                            taxSubtotaltaxCategoryIdElement.AppendChild(doc.CreateTextNode(tst.TaxCategory.ID.arb));
                            if (!"".Equals(tst.TaxCategory.ID.schemeAgencyID))
                            {
                                XmlAttribute schemeAgencyIDAttr = doc.CreateAttribute("schemeAgencyID");
                                schemeAgencyIDAttr.Value = (tst.TaxCategory.ID.schemeAgencyID);
                                taxSubtotaltaxCategoryIdElement.SetAttributeNode(schemeAgencyIDAttr);
                            }
                            if (!"".Equals(tst.TaxCategory.ID.schemeID))
                            {
                                XmlAttribute schemeIDAttr = doc.CreateAttribute("schemeID");
                                schemeIDAttr.Value = (tst.TaxCategory.ID.schemeID);
                                taxSubtotaltaxCategoryIdElement.SetAttributeNode(schemeIDAttr);
                            }
                            taxSubtotaltaxCategoryElement.AppendChild(taxSubtotaltaxCategoryIdElement);

                            XmlElement taxSubtotalPercentXmlElement = doc.CreateElement("cbc", "Percent", cbc);
                            taxSubtotalPercentXmlElement.AppendChild(doc.CreateTextNode(tst.TaxCategory.Percent));
                            taxSubtotaltaxCategoryElement.AppendChild(taxSubtotalPercentXmlElement);

                            if (!"".Equals(tst.TaxCategory?.TaxExemptionReasonCode ?? ""))
                            {
                                XmlElement taxExemptionReasonCodeXmlElement = doc.CreateElement("cbc", "TaxExemptionReasonCode", cbc);
                                taxExemptionReasonCodeXmlElement.AppendChild(doc.CreateTextNode(tst.TaxCategory.TaxExemptionReasonCode));
                                taxSubtotaltaxCategoryElement.AppendChild(taxExemptionReasonCodeXmlElement);
                            }

                            if (!"".Equals(tst.TaxCategory?.TaxExemptionReason?.arb ?? ""))
                            {
                                XmlElement taxExemptionReasonXmlElement = doc.CreateElement("cbc", "TaxExemptionReason", cbc);
                                taxExemptionReasonXmlElement.AppendChild(doc.CreateTextNode(tst.TaxCategory.TaxExemptionReason.arb));
                                taxSubtotaltaxCategoryElement.AppendChild(taxExemptionReasonXmlElement);
                            }

                            XmlElement taxSubtotalTaxSchemeXmlElement = doc.CreateElement("cac", "TaxScheme", cac);
                            XmlElement taxSubtotalTaxSchemeIdXmlElement = doc.CreateElement("cbc", "ID", cbc);
                            if (!"".Equals(tst.TaxCategory.TaxScheme.ID.schemeAgencyID))
                            {
                                XmlAttribute schemeAgencyIDAttr = doc.CreateAttribute("schemeAgencyID");
                                schemeAgencyIDAttr.Value = (tst.TaxCategory.TaxScheme.ID.schemeAgencyID);
                                taxSubtotalTaxSchemeIdXmlElement.SetAttributeNode(schemeAgencyIDAttr);
                            }
                            if (!"".Equals(tst.TaxCategory.TaxScheme.ID.schemeID))
                            {
                                XmlAttribute schemeIDAttr = doc.CreateAttribute("schemeID");
                                schemeIDAttr.Value = (tst.TaxCategory.TaxScheme.ID.schemeID);
                                taxSubtotalTaxSchemeIdXmlElement.SetAttributeNode(schemeIDAttr);
                            }
                            taxSubtotalTaxSchemeIdXmlElement.AppendChild(doc.CreateTextNode(tst.TaxCategory.TaxScheme.ID.arb));
                            taxSubtotalTaxSchemeXmlElement.AppendChild(taxSubtotalTaxSchemeIdXmlElement);
                            taxSubtotaltaxCategoryElement.AppendChild(taxSubtotalTaxSchemeXmlElement);


                            taxSubtotal.AppendChild(taxSubtotaltaxCategoryElement);
                            taxTotalElement.AppendChild(taxSubtotal);
                        }
                    }
                    rootXmlElement.AppendChild(taxTotalElement);

                }



                XmlElement legalMonetaryTotal = doc.CreateElement("cac", "LegalMonetaryTotal", cac);
                XmlElement lineExtensionAmount = doc.CreateElement("cbc", "LineExtensionAmount", cbc);
                lineExtensionAmount.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.LegalMonetaryTotal.LineExtensionAmount.value));
                XmlAttribute currencyIDAttr = doc.CreateAttribute("currencyID");
                currencyIDAttr.Value = (baseData.Invoice.EInvoice.LegalMonetaryTotal.LineExtensionAmount.currencyID);
                lineExtensionAmount.SetAttributeNode(currencyIDAttr);
                legalMonetaryTotal.AppendChild(lineExtensionAmount);

                XmlElement taxExclusiveAmount = doc.CreateElement("cbc", "TaxExclusiveAmount", cbc);
                taxExclusiveAmount.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.LegalMonetaryTotal.TaxExclusiveAmount.value));
                XmlAttribute taxExclusiveAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
                taxExclusiveAmountCurrencyIDAttr.Value = (baseData.Invoice.EInvoice.LegalMonetaryTotal.TaxExclusiveAmount.currencyID);
                taxExclusiveAmount.SetAttributeNode(taxExclusiveAmountCurrencyIDAttr);
                legalMonetaryTotal.AppendChild(taxExclusiveAmount);
                XmlElement taxInclusiveAmount = doc.CreateElement("cbc", "TaxInclusiveAmount", cbc);
                taxInclusiveAmount.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.LegalMonetaryTotal.TaxInclusiveAmount.value));
                XmlAttribute taxInclusiveAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
                taxInclusiveAmountCurrencyIDAttr.Value = (baseData.Invoice.EInvoice.LegalMonetaryTotal.TaxInclusiveAmount.currencyID);
                taxInclusiveAmount.SetAttributeNode(taxInclusiveAmountCurrencyIDAttr);
                legalMonetaryTotal.AppendChild(taxInclusiveAmount);

                RET.InvoiceTotal = baseData.Invoice.EInvoice.LegalMonetaryTotal.TaxInclusiveAmount.value;

                if (!"".Equals(baseData.Invoice.EInvoice.LegalMonetaryTotal.AllowanceTotalAmount.value))
                {
                    XmlElement allowanceTotalAmount = doc.CreateElement("cbc", "AllowanceTotalAmount", cbc);
                    allowanceTotalAmount.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.LegalMonetaryTotal.AllowanceTotalAmount.value));
                    if (!"".Equals(baseData.Invoice.EInvoice.LegalMonetaryTotal.AllowanceTotalAmount.currencyID))
                    {
                        XmlAttribute allowanceTotalAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
                        allowanceTotalAmountCurrencyIDAttr.Value = (baseData.Invoice.EInvoice.LegalMonetaryTotal.AllowanceTotalAmount.currencyID);
                        allowanceTotalAmount.SetAttributeNode(allowanceTotalAmountCurrencyIDAttr);
                    }
                    legalMonetaryTotal.AppendChild(allowanceTotalAmount);
                }
                if (!"".Equals(baseData.Invoice.EInvoice.LegalMonetaryTotal.ChargeTotalAmount.value))
                {
                    XmlElement chargeTotalAmount = doc.CreateElement("cbc", "ChargeTotalAmount", cbc);
                    chargeTotalAmount.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.LegalMonetaryTotal.ChargeTotalAmount.value));
                    if (!"".Equals(baseData.Invoice.EInvoice.LegalMonetaryTotal.ChargeTotalAmount.currencyID))
                    {
                        XmlAttribute chargeTotalAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
                        chargeTotalAmountCurrencyIDAttr.Value = (baseData.Invoice.EInvoice.LegalMonetaryTotal.ChargeTotalAmount.currencyID);
                        chargeTotalAmount.SetAttributeNode(chargeTotalAmountCurrencyIDAttr);
                    }
                    legalMonetaryTotal.AppendChild(chargeTotalAmount); 
                  
                }
                if (!"".Equals(baseData.Invoice.EInvoice.LegalMonetaryTotal.PrepaidAmount.value))
                {
                    XmlElement prepaidAmount = doc.CreateElement("cbc", "PrepaidAmount", cbc);
                    prepaidAmount.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.LegalMonetaryTotal.PrepaidAmount.value));
                    if (!"".Equals(baseData.Invoice.EInvoice.LegalMonetaryTotal.PrepaidAmount.currencyID))
                    {
                        XmlAttribute prepaidAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
                        prepaidAmountCurrencyIDAttr.Value = (baseData.Invoice.EInvoice.LegalMonetaryTotal.PrepaidAmount.currencyID);
                        prepaidAmount.SetAttributeNode(prepaidAmountCurrencyIDAttr);
                    }
                    legalMonetaryTotal.AppendChild(prepaidAmount);
                }
                XmlElement payableAmount = doc.CreateElement("cbc", "PayableAmount", cbc);
                payableAmount.AppendChild(doc.CreateTextNode(baseData.Invoice.EInvoice.LegalMonetaryTotal.PayableAmount.value));
                if (!"".Equals(baseData.Invoice.EInvoice.LegalMonetaryTotal.PayableAmount.currencyID))
                {
                    XmlAttribute payableAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
                    payableAmountCurrencyIDAttr.Value = (baseData.Invoice.EInvoice.LegalMonetaryTotal.PayableAmount.currencyID);
                    payableAmount.SetAttributeNode(payableAmountCurrencyIDAttr);
                }
                legalMonetaryTotal.AppendChild(payableAmount);
                rootXmlElement.AppendChild(legalMonetaryTotal);


                doc = addXmlElementForInvoiceLine(baseData, doc, rootXmlElement);


                XMLSTATUS stat = XMLSTATUS.UNSIGNED;
                if (!validationError.Equals(""))
                {
                    stat = XMLSTATUS.FAILED;
                }
                string foldername = Get_Folder(baseData.Invoice.documentType, TrxSource, stat, "");

                string fname = foldername + GetFileName_withoutext(ff) + ".xml";

                // doc.Normalize();
                doc.Save(fname);

                //var nsmgr = new XmlNamespaceManager(doc.NameTable);
                // nsmgr.AddNamespace("app", "http://www.weather.gov/forecasts/xml/OGC_services");

                // doc.getDocumentXmlElement().normalize();
                //  Transformer transformer=new service.HashingGenerationService().getTransformer();
                //DOMSource source = new DOMSource(doc);
                //    StreamResult result = new StreamResult(new File(fname));

                //    TransformerFactory transformerFactory = tra.newInstance();
                //    Transformer transformer = transformerFactory.newTransformer();
                //    //             transformer.setOutputProperty(OutputKeys.INDENT, "yes");
                //    //            // transformer.setOutputProperty("{http://xml.apache.org/xslt}indent-amount", "2");
                //    //             transformer.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "yes");
                //    //             transformer.setOutputProperty(OutputKeys.arb CODING, "UTF-8");
                //    transformer.setOutputProperty("encoding", "UTF-8");
                //    transformer.setOutputProperty("indent", "yes");
                //    transformer.setOutputProperty("{http://xml.apache.org/xslt}indent-amount", "4");
                //    transformer.setOutputProperty("omit-xml-declaration", "yes");
                //    transformer.transform(source, result);
                //    util.Util.log("XML file created successfully");


                //while returning xml and signing thexml string directly, it gives error  "hashedXml does not match with qr code hashedXml"
                //So doing the same way as SDK, saving the generated invoice xml and then load the xml from file for signing.

                //              StringWriter stringWriter = new StringWriter();
                //              transformer.transform(source, new StreamResult(stringWriter)); 
                //                 xml = stringWriter.toString();
                //                 xml=xml.trim();

                //string  xml = System.IO.File.ReadAllText(fname, Encoding.UTF8);

                //                                  byte [] bb=xml.getBytes(StandardCharsets.UTF_8);
                //                  xml= canonicalizeXml(bb);  // it is needed to make sure last new line is removed.
                //                    util.Util.logEx("xml canonicalized");
                RET.xmlFilePath = fname;
                return RET;

            }
            catch (Exception ex)
            {

                bp.log("generateXML error:" + ex.ToString(), 1, typ);
                
                throw new InvalidCastException(validationError + ".GenerateXML error:" + ex.ToString());
            }

        }

        private XmlDocument addXmlElementForInvoiceLine(BaseData.Root baseData, XmlDocument doc, XmlElement rootXmlElement)
        {
            string cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
            string cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
            string ext = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";

            // loop for each InvoiceLine
            for (int i = 0; i < baseData.Invoice.EInvoice.InvoiceLine.Count; i++)
            {
                BaseData.InvoiceLine invLine = baseData.Invoice.EInvoice.InvoiceLine[i];
                XmlElement invoiceLine = doc.CreateElement("cac", "InvoiceLine", cac);
                XmlElement idXmlElement = doc.CreateElement("cbc", "ID", cbc);
                idXmlElement.AppendChild(doc.CreateTextNode(invLine.ID));
                invoiceLine.AppendChild(idXmlElement);

                XmlElement invoicedQuantityXmlElement = doc.CreateElement("cbc", "InvoicedQuantity", cbc);
                invoicedQuantityXmlElement.AppendChild(doc.CreateTextNode(invLine.InvoicedQuantity.value));
                XmlAttribute unitCodeAttr = doc.CreateAttribute("unitCode");
                unitCodeAttr.Value = (invLine.InvoicedQuantity.unitCode);
                invoicedQuantityXmlElement.SetAttributeNode(unitCodeAttr);
                invoiceLine.AppendChild(invoicedQuantityXmlElement);

                XmlElement lineExtensionAmountXmlElement = doc.CreateElement("cbc", "LineExtensionAmount", cbc);
                lineExtensionAmountXmlElement.AppendChild(doc.CreateTextNode(invLine.LineExtensionAmount.value));
                XmlAttribute currencyIDAttr = doc.CreateAttribute("currencyID");
                currencyIDAttr.Value = (invLine.LineExtensionAmount.currencyID);
                lineExtensionAmountXmlElement.SetAttributeNode(currencyIDAttr);
                invoiceLine.AppendChild(lineExtensionAmountXmlElement);

                if (invLine.AllowanceCharge.Count > 0)
                {
                    for (int j = 0; j < invLine.AllowanceCharge.Count; j++)
                    {
                        BaseData.AllowanceCharge ac = invLine.AllowanceCharge[j];
                        if (!"".Equals(ac?.Amount?.value ?? ""))
                        {
                            XmlElement allowanceCharge = doc.CreateElement("cac", "AllowanceCharge", cac);
                            if (!"".Equals(ac.ChargeIndicator))
                            {
                                XmlElement chargeIndicator = doc.CreateElement("cbc", "ChargeIndicator", cbc);
                                chargeIndicator.AppendChild(doc.CreateTextNode(ac.ChargeIndicator));
                                allowanceCharge.AppendChild(chargeIndicator);
                            }
                            if (ac.ChargeIndicator.ToLower().Equals("true"))  //add reason codes only for document level charges, not discounts
                            {
                                if (!"".Equals(ac.AllowanceChargeReasonCode))
                                {
                                    XmlElement allowanceChargeReasonCode = doc.CreateElement("cbc", "AllowanceChargeReasonCode", cbc);
                                    allowanceChargeReasonCode.AppendChild(doc.CreateTextNode(ac.AllowanceChargeReasonCode));
                                    allowanceCharge.AppendChild(allowanceChargeReasonCode);
                                }
                                if (!"".Equals(ac.AllowanceChargeReason))
                                {
                                    XmlElement allowanceChargeReason = doc.CreateElement("cbc", "AllowanceChargeReason", cbc);
                                    allowanceChargeReason.AppendChild(doc.CreateTextNode(ac.AllowanceChargeReason.arb));
                                    allowanceCharge.AppendChild(allowanceChargeReason);
                                }
                            }

                            if (!"".Equals(ac.MultiplierFactorNumeric))
                            {
                                XmlElement multiplierFactorNumeric = doc.CreateElement("cbc", "MultiplierFactorNumeric", cbc);
                                multiplierFactorNumeric.AppendChild(doc.CreateTextNode(ac.MultiplierFactorNumeric));
                                allowanceCharge.AppendChild(multiplierFactorNumeric);
                            }
                            if (!"".Equals(ac.Amount))
                            {
                                XmlElement amount = doc.CreateElement("cbc", "Amount", cbc);
                                amount.AppendChild(doc.CreateTextNode(ac.Amount.value));
                                if (!"".Equals(ac.Amount.currencyID))
                                {
                                    XmlAttribute amountcurrencyIDAttr = doc.CreateAttribute("currencyID");
                                    amountcurrencyIDAttr.Value = (ac.Amount.currencyID);
                                    amount.SetAttributeNode(amountcurrencyIDAttr);
                                }
                                allowanceCharge.AppendChild(amount);
                            }
                            if (!"".Equals(ac.BaseAmount.value))
                            {
                                XmlElement baseAmount = doc.CreateElement("cbc", "BaseAmount", cbc);
                                baseAmount.AppendChild(doc.CreateTextNode(ac.BaseAmount.value));
                                if (!"".Equals(ac.BaseAmount.currencyID))
                                {
                                    XmlAttribute baseAmountcurrencyIDAttr = doc.CreateAttribute("currencyID");
                                    baseAmountcurrencyIDAttr.Value = (ac.BaseAmount.currencyID);
                                    baseAmount.SetAttributeNode(baseAmountcurrencyIDAttr);
                                }
                                allowanceCharge.AppendChild(baseAmount);
                            }

                            if (!"".Equals(ac?.TaxCategory?.Percent ?? ""))
                            {
                                XmlElement taxCategoryElement = doc.CreateElement("cac", "TaxCategory", cac);
                                if (!"".Equals(ac.TaxCategory.ID.arb))
                                {
                                    XmlElement taxCategoryIdElement = doc.CreateElement("cbc", "ID", cbc);
                                    taxCategoryIdElement.AppendChild(doc.CreateTextNode(ac.TaxCategory.ID.arb));
                                    if (!"".Equals(ac.TaxCategory.ID.schemeAgencyID))
                                    {
                                        XmlAttribute schemeAgencyIDAttr = doc.CreateAttribute("schemeAgencyID");
                                        schemeAgencyIDAttr.Value = (ac.TaxCategory.ID.schemeAgencyID);
                                        taxCategoryIdElement.SetAttributeNode(schemeAgencyIDAttr);
                                    }
                                    if (!"".Equals(ac.TaxCategory.ID.schemeID))
                                    {
                                        XmlAttribute schemeIDAttr = doc.CreateAttribute("schemeID");
                                        schemeIDAttr.Value = (ac.TaxCategory.ID.schemeID);
                                        taxCategoryIdElement.SetAttributeNode(schemeIDAttr);
                                    }
                                    taxCategoryElement.AppendChild(taxCategoryIdElement);
                                }
                                if (!"".Equals(ac?.TaxCategory?.Percent ?? ""))
                                {
                                   XmlElement percentXmlElement1 = doc.CreateElement("cbc", "Percent", cbc);
                                    percentXmlElement1.AppendChild(doc.CreateTextNode(ac.TaxCategory.Percent));
                                    taxCategoryElement.AppendChild(percentXmlElement1);
                                }
                                if (!"".Equals(ac?.TaxCategory?.TaxScheme?.ID?.arb ?? ""))
                                {
                                    XmlElement taxSchemeXmlElement1 = doc.CreateElement("cac", "TaxScheme", cac);
                                    XmlElement taxSchemeIdXmlElement1 = doc.CreateElement("cbc", "ID", cbc);
                                    taxSchemeIdXmlElement1.AppendChild(doc.CreateTextNode(ac.TaxCategory.TaxScheme.ID.arb));

                                    //if (!"".Equals(ac.TaxCategory.TaxScheme.ID.schemeAgencyID))
                                    //{
                                    //    XmlAttribute schemeAgencyIDAttr = doc.CreateAttribute("schemeAgencyID");
                                    //    schemeAgencyIDAttr.Value = (ac.TaxCategory.TaxScheme.ID.schemeAgencyID);
                                    //    taxSchemeIdXmlElement.SetAttributeNode(schemeAgencyIDAttr);
                                    //}
                                    //if (!"".Equals(ac.TaxCategory.TaxScheme.ID.schemeID))
                                    //{
                                    //    XmlAttribute schemeIDAttr = doc.CreateAttribute("schemeID");
                                    //    schemeIDAttr.Value = (ac.TaxCategory.TaxScheme.ID.schemeID);
                                    //    taxSchemeIdXmlElement.SetAttributeNode(schemeIDAttr);
                                    //}

                                    taxSchemeXmlElement1.AppendChild(taxSchemeIdXmlElement1);
                                    taxCategoryElement.AppendChild(taxSchemeXmlElement1);
                                }
                                allowanceCharge.AppendChild(taxCategoryElement);
                            }
                            invoiceLine.AppendChild(allowanceCharge);
                        }

                    }
                }


                if (baseData.Invoice.InvoiceType == InvoiceType.STD_CN || baseData.Invoice.InvoiceType == InvoiceType.STD_DN || baseData.Invoice.InvoiceType == InvoiceType.STD_INVOICE || baseData.Invoice.InvoiceType == InvoiceType.SIMP_CN || baseData.Invoice.InvoiceType == InvoiceType.SIMP_DN)
                {
                    XmlElement taxTotalXmlElement = doc.CreateElement("cac", "TaxTotal", cac);

                    XmlElement taxAmountXmlElement = doc.CreateElement("cbc", "TaxAmount", cbc);
                    taxAmountXmlElement.AppendChild(doc.CreateTextNode(invLine.TaxTotal.TaxAmount.value));
                    XmlAttribute taxAmountCurrencyIdAttr = doc.CreateAttribute("currencyID");
                    taxAmountCurrencyIdAttr.Value = (invLine.TaxTotal.TaxAmount.currencyID);
                    taxAmountXmlElement.SetAttributeNode(taxAmountCurrencyIdAttr);
                    taxTotalXmlElement.AppendChild(taxAmountXmlElement);


                    XmlElement roundingAmountXmlElement = doc.CreateElement("cbc", "RoundingAmount", cbc);
                    roundingAmountXmlElement.AppendChild(doc.CreateTextNode(invLine.TaxTotal.RoundingAmount.value));
                    XmlAttribute roundingAmountCurrencyIdAttr = doc.CreateAttribute("currencyID");
                    roundingAmountCurrencyIdAttr.Value = (invLine.TaxTotal.RoundingAmount.currencyID);
                    roundingAmountXmlElement.SetAttributeNode(roundingAmountCurrencyIdAttr);
                    taxTotalXmlElement.AppendChild(roundingAmountXmlElement);

                    invoiceLine.AppendChild(taxTotalXmlElement);
                }


                XmlElement itemXmlElement = doc.CreateElement("cac", "Item", cac);
                XmlElement itemNameXmlElement = doc.CreateElement("cbc", "Name", cbc);
                itemNameXmlElement.AppendChild(doc.CreateTextNode(invLine.Item.Name.arb));
                itemXmlElement.AppendChild(itemNameXmlElement);
                XmlElement classifiedtaxCategoryElement = doc.CreateElement("cac", "ClassifiedTaxCategory", cac);
                XmlElement classifiedtaxCategoryIdElement = doc.CreateElement("cbc", "ID", cbc);
                classifiedtaxCategoryIdElement.AppendChild(doc.CreateTextNode(invLine.Item.ClassifiedTaxCategory.ID.arb));
                classifiedtaxCategoryElement.AppendChild(classifiedtaxCategoryIdElement);
                XmlElement percentXmlElement = doc.CreateElement("cbc", "Percent", cbc);
                percentXmlElement.AppendChild(doc.CreateTextNode(invLine.Item.ClassifiedTaxCategory.Percent));
                classifiedtaxCategoryElement.AppendChild(percentXmlElement);
                XmlElement taxSchemeXmlElement = doc.CreateElement("cac", "TaxScheme", cac);
                XmlElement taxSchemeIdXmlElement = doc.CreateElement("cbc", "ID", cbc);
                taxSchemeIdXmlElement.AppendChild(doc.CreateTextNode(invLine.Item.ClassifiedTaxCategory.TaxScheme.ID.arb));
                taxSchemeXmlElement.AppendChild(taxSchemeIdXmlElement);
                classifiedtaxCategoryElement.AppendChild(taxSchemeXmlElement);
                itemXmlElement.AppendChild(classifiedtaxCategoryElement);
                invoiceLine.AppendChild(itemXmlElement);

                XmlElement priceXmlElement = doc.CreateElement("cac", "Price", cac);
                XmlElement priceAmountXmlElement = doc.CreateElement("cbc", "PriceAmount", cbc);
                priceAmountXmlElement.AppendChild(doc.CreateTextNode(invLine.Price.PriceAmount.value));
                XmlAttribute priceAmountCurrencyIDAttr = doc.CreateAttribute("currencyID");
                priceAmountCurrencyIDAttr.Value = (invLine.Price.PriceAmount.currencyID);
                priceAmountXmlElement.SetAttributeNode(priceAmountCurrencyIDAttr);
                priceXmlElement.AppendChild(priceAmountXmlElement);
                invoiceLine.AppendChild(priceXmlElement);

                
              
                rootXmlElement.AppendChild(invoiceLine);
            }

            return doc;
        }



        public static string Get_Folder_old(DOCTYPE doctype, SOURCE src, ACTION_STATUS stat)
        {


            string folderpath = Basepage.invoiceXML_folder;
            if (src == SOURCE.EXTERNAL)
            {
                if (doctype == DOCTYPE.B2B)
                {
                    if (stat == ACTION_STATUS.ClearanceFailed) folderpath += FOLDER.External_B2B_Failed.GetEnumDescription();
                    if (stat == ACTION_STATUS.Cleared) folderpath += FOLDER.External_B2B_Signed.GetEnumDescription();
                    if (stat == ACTION_STATUS.Signed) folderpath += FOLDER.External_B2B_Signed.GetEnumDescription();
                }
                if (doctype == DOCTYPE.B2C)
                {
                    if (stat == ACTION_STATUS.Reported) folderpath += FOLDER.External_B2C_Reported.GetEnumDescription();
                    if (stat == ACTION_STATUS.ReportingFailed) folderpath += FOLDER.External_B2C_Failed.GetEnumDescription();
                    if (stat == ACTION_STATUS.Signed) folderpath += FOLDER.External_B2C_Signed.GetEnumDescription();
                    if (stat == ACTION_STATUS.SigningFailed) folderpath += FOLDER.External_B2C_Failed.GetEnumDescription();
                }
            }

            if (src == SOURCE.POS)
            {
                if (doctype == DOCTYPE.B2B)
                {
                    if (stat == ACTION_STATUS.ClearanceFailed) folderpath += FOLDER.POS_B2B_Failed.GetEnumDescription();
                    if (stat == ACTION_STATUS.Cleared) folderpath += FOLDER.POS_B2B_Signed.GetEnumDescription();
                    if (stat == ACTION_STATUS.Signed) folderpath += FOLDER.POS_B2B_Signed.GetEnumDescription();
                }
                if (doctype == DOCTYPE.B2C)
                {
                    if (stat == ACTION_STATUS.Reported) folderpath += FOLDER.POS_B2C_Reported.GetEnumDescription();
                    if (stat == ACTION_STATUS.ReportingFailed) folderpath += FOLDER.POS_B2C_Failed.GetEnumDescription();
                    if (stat == ACTION_STATUS.Signed) folderpath += FOLDER.POS_B2C_Signed.GetEnumDescription();
                    if (stat == ACTION_STATUS.SigningFailed) folderpath += FOLDER.POS_B2C_Failed.GetEnumDescription();
                }
            }

            return folderpath;
        }

        public static string Get_Folder(DOCTYPE doctype, SOURCE src, XMLSTATUS stat, string xmlFilename)
        {


            string folderpath = Basepage.invoiceXML_folder;
            if (src == SOURCE.EXTERNAL)
            {
                if (doctype == DOCTYPE.B2B)
                {
                    if (stat == XMLSTATUS.UNSIGNED) folderpath += FOLDER.External_B2B_Unsigned.GetEnumDescription();
                    if (stat == XMLSTATUS.SIGNED) folderpath += FOLDER.External_B2B_Signed.GetEnumDescription();
                    if (stat == XMLSTATUS.FAILED) folderpath += FOLDER.External_B2B_Failed.GetEnumDescription();
                }
                if (doctype == DOCTYPE.B2C)
                {
                    if (stat == XMLSTATUS.UNSIGNED) folderpath += FOLDER.External_B2C_Unsigned.GetEnumDescription();
                    if (stat == XMLSTATUS.SIGNED) folderpath += FOLDER.External_B2C_Signed.GetEnumDescription();
                    if (stat == XMLSTATUS.FAILED) folderpath += FOLDER.External_B2C_Failed.GetEnumDescription();
                    if (stat == XMLSTATUS.REPORTED)
                    {
                        folderpath += FOLDER.External_B2C_Reported.GetEnumDescription();
                        folderpath = Basepage.getReported_Directory(xmlFilename, folderpath);
                    }
                }
            }

            if (src == SOURCE.POS)
            {
                if (doctype == DOCTYPE.B2B)
                {
                    if (stat == XMLSTATUS.UNSIGNED) folderpath += FOLDER.POS_B2B_Unsigned.GetEnumDescription();
                    if (stat == XMLSTATUS.SIGNED) folderpath += FOLDER.POS_B2B_Signed.GetEnumDescription();
                    if (stat == XMLSTATUS.FAILED) folderpath += FOLDER.POS_B2B_Failed.GetEnumDescription();
                }
                if (doctype == DOCTYPE.B2C)
                {
                    if (stat == XMLSTATUS.UNSIGNED) folderpath += FOLDER.POS_B2C_Unsigned.GetEnumDescription();
                    if (stat == XMLSTATUS.SIGNED) folderpath += FOLDER.POS_B2C_Signed.GetEnumDescription();
                    if (stat == XMLSTATUS.FAILED) folderpath += FOLDER.POS_B2C_Failed.GetEnumDescription();
                    if (stat == XMLSTATUS.REPORTED)
                    {
                        folderpath += FOLDER.POS_B2C_Reported.GetEnumDescription();
                        folderpath = Basepage.getReported_Directory(xmlFilename, folderpath);
                    }
                }
            }

            return folderpath;
        }


    }
    public class Generated_OUT
    {
        public string xmlFilePath { get; set; }
        public string TaxTotal { get; set; }
        public string InvoiceTotal { get; set; }
    }
    public class FileNameFields
    {
        public string partyTaxSchemeCompanyId { get; set; }
        public string IssueDate_YYYY_MM_DD { get; set; }
        public string IssueTime_HH_MM_SS { get; set; }
        public string irn { get; set; }
    }

    public enum INVOICE_OPERATION
    {
        INSERT_EGSconfig,
        INSERT_INVOICE,
        INSERT_STORE,
        SEARCH_INVOICES,
        GET_ERRORS_WARNINGS,
        GET_ALL_ACTIONS
    }
    public enum ERR
    {
        custerr_Document_type_mismatch = 90001,
        custerr_ICV_PIH_ERR = 90002,
        custerr_TaxTotal_array_must_contain_two_items_json = 90003,
        custerr_XML_Generation_Error = 90004,
        custerr_Doc_Level_AllowanceCharge_charge_indicator_missing = 90005,
        custerr_Amount_must_have_currencyID = 90006,
        custerr_B2B_document_not_allowed = 90007,
        custerr_Validation_Error = 90008,
        DB_Update_Error = 90009,

    }


    public enum PROCESS_TYPE
    {
        SYSTEM,
        SYSTEM_RETRY,
        MANUAL,
        API

    }
    public enum SOURCE
    {
        POS,
        EXTERNAL
    }
    public enum ACTION
    {
        REPORTING,
        CLEARANCE,
        SIGNING_B2C,
        SIGNING_B2B  //during clearance disabled, yet to confirm if this is needed or not.
    }
    public enum ACTION_STATUS
    {
        Reported,
        Cleared,
        Signed,
        SigningFailed,
        ReportingFailed,
        ClearanceFailed
    }
    public enum XMLSTATUS
    {

        UNSIGNED,
        SIGNED,
        FAILED,
        REPORTED

    }

    public enum FOLDER
    {

        [Description("External\\B2B\\UNSIGNED\\")]
        External_B2B_Unsigned,

        [Description("External\\B2B\\SIGNED\\")]
        External_B2B_Signed,

        [Description("External\\B2B\\FAILED\\")]
        External_B2B_Failed,

        [Description("External\\B2C\\UNSIGNED\\")]
        External_B2C_Unsigned,

        [Description("External\\B2C\\SIGNED\\")]
        External_B2C_Signed,

        [Description("External\\B2C\\FAILED\\")]
        External_B2C_Failed,

        [Description("External\\B2C\\RETRY\\")]   //files sent to retry folder and retried without changing ICV, PIH AND UUID.
        External_B2C_Retry,

        [Description("External\\B2C\\REPORTED\\")]
        External_B2C_Reported,

        [Description("POS\\B2B\\UNSIGNED\\")]
        POS_B2B_Unsigned,

        [Description("POS\\B2B\\SIGNED\\")]
        POS_B2B_Signed,

        [Description("POS\\B2B\\FAILED\\")]
        POS_B2B_Failed,

        [Description("POS\\B2C\\UNSIGNED\\")]
        POS_B2C_Unsigned,

        [Description("POS\\B2C\\SIGNED\\")]
        POS_B2C_Signed,

        [Description("POS\\B2C\\FAILED\\")]
        POS_B2C_Failed,

        [Description("POS\\B2C\\RETRY\\")]
        POS_B2C_Retry,

        [Description("POS\\B2C\\REPORTED\\")]
        POS_B2C_Reported,

        [Description("PORTAL\\PROCESSING\\")]
        Portal_Uploaded,

        [Description("PORTAL\\BACKUP\\")]
        Portal_Backup

    }
}

namespace ExtensionMethod
{
    public static class Extension
    {
        public static string GetEnumDescription(this Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                return attribute.Description;
            }
            throw new ArgumentException("Item not found.", nameof(enumValue));
        }
    }
}
