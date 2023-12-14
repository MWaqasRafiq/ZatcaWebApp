
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using org.w3c.dom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ZATCA;

namespace GatewayService
{



    public class einvoiceController : ApiController
    {

        Basepage bp = new Basepage();

        [HttpGet]
        //// call http://localhost:8871/api/einvoice/zget in postman.Headers ->key:postedData value :any string
        [ActionName("zget")]
        public HttpResponseMessage getTest(HttpRequestMessage postedData)
        {

            HttpResponseMessage response = null;

            //  bp.log(1, "------------------------API Call------------------------");
            try
            {
                var resp = new HttpResponseMessage()
                {
                    Content = new StringContent("TEST SUCCESS!!")
                };

                response = resp;
            }
            catch (Exception ex)
            {
                string ret = " {\"code\":\"{code}\"} ";

                ret = ret.Replace("{code}", "8888");

                var resp = new HttpResponseMessage()
                {
                    Content = new StringContent(ret)
                };
                resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = resp;

            }


            return response;

        }


        [HttpPost]
        //// call http://localhost:8871/api/einvoice/getqr 
        ///For b2b and b2c xml creation, signing and sending QR back
        [ActionName("getqr")]
        [BasicAuthentication]
        public DataReturnAPI signInvoice(HttpRequestMessage postedData)
        {
            //request and response in json format
           

            bp.log("------------------------API Call------------------------", 1, LOGTYPE.API);
            invoice VV = new invoice();
            try
            {
                var sss = postedData.Content.ReadAsStringAsync().Result;
                bp.log("Request::\r\n" + sss, 0, LOGTYPE.API);


                //validating header-------------------    
                IEnumerable<string> headerValues;
                //X-IDL-RequestedInvType
                string doctype = null;
                if (postedData.Headers.TryGetValues("X-IDL-RequestedDocType", out headerValues))
                {
                    doctype = headerValues.FirstOrDefault();
                    bp.log("X-IDL-RequestedDocType::" + doctype, 1, LOGTYPE.API);
                }

                if (string.IsNullOrEmpty(doctype))
                {
                    throw new InvalidCastException("RequestedDocType required");
                }

                if (doctype == "B2B" || doctype == "B2C")
                {

                }
                else throw new InvalidCastException("Invalid request");

                string strrr = null;

                if (postedData.Headers.TryGetValues("PartyTaxSchemeCompanyId", out headerValues))
                {
                    strrr = headerValues.FirstOrDefault();
                    bp.log("PartyTaxSchemeCompanyId::" + strrr, 1, LOGTYPE.API);
                }
                if (strrr.Equals(Basepage.partyTaxSchemeCompanyId) == false)
                {
                    throw new InvalidCastException("Invalid request paramter");
                }



                Enum.TryParse(doctype, out DOCTYPE ddd);


                DataReturn retData = new invoice().Process(sss, ddd, LOGTYPE.API, SOURCE.EXTERNAL);
                //return retData;

                DataReturnAPI rett = new DataReturnAPI();
                rett.ClearedInvoice = retData.ClearedInvoice;
                rett.DocType = retData.DocumentType.ToString();
                rett.QR = retData.QR;
                rett.UUID = retData.UUID;
                rett.TaxTotal = retData.TaxTotal;
                rett.RC = retData.RC;
                rett.RESPMSG = retData.RESPMSG;
                rett.info = retData.info;
                rett.Errors = retData.Errors;
                rett.Warnings = retData.Warnings;
                rett.InvoiceHash = retData.InvoiceHash;
                rett.InvReferenceNumber = retData.InvReferenceNumber;
                rett.Process = retData._action.ToString();
                rett.ProcessStatus = retData._actionStatus.ToString();

                //return Newtonsoft.Json.JsonConvert.SerializeObject(retData);
                return rett;




            }
            catch (Exception ex)
            {
                bp.log("Error::\r\n" + ex.ToString(), 1, LOGTYPE.API);

                string msg = ex.Message;
                int code = 99999;

                if (msg.Contains(" custerr_"))
                {
                    msg = ex.Message.Replace("custerr_", "").Replace("_", " ");

                }

                DataReturn retData = new DataReturn()
                {
                    RESPMSG = msg,
                    RC = code
                };

                // return retData;
                //return Newtonsoft.Json.JsonConvert.SerializeObject(retData);

                DataReturnAPI rett = new DataReturnAPI();
                rett.ClearedInvoice = retData.ClearedInvoice;
                rett.DocType = retData.DocumentType.ToString();
                rett.QR = retData.QR;
                rett.UUID = retData.UUID;
                rett.TaxTotal = retData.TaxTotal;
                rett.RC = retData.RC;
                rett.RESPMSG = retData.RESPMSG.Replace("C:\\Users\\Rigesh\\Documents\\Visual Studio 2015\\Projects\\ZATCA\\ZATCA", "");
                rett.info = retData.info;
                rett.Errors = retData.Errors;
                rett.Warnings = retData.Warnings;
                rett.InvoiceHash = retData.InvoiceHash;
                rett.InvReferenceNumber = retData.InvReferenceNumber;
                rett.Process = "";  //retData._action.ToString()
                rett.ProcessStatus = ""; //retData._actionStatus.ToString();
                return rett;
            }

        }



        [HttpPost]
        //// call http://localhost:8871/api/einvoice/xmlfileupload  
        ///For POS XMLs to be signed and send the QR back
        [ActionName("xmlfileupload")]
        [BasicAuthentication]
        public async Task<DataReturnAPI> UploadFileAsync(HttpRequestMessage request)
        {
            bp.log("------------------------File Upload API Call------------------------", 1, LOGTYPE.API);
            try
            {
                DOCTYPE _DOCTYPE;

                DataReturn retData = new DataReturn();

                if (!request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                IEnumerable<string> headerValues;
                //X-IDL-RequestedInvType
                string doctype = null;
                if (request.Headers.TryGetValues("X-IDL-RequestedDocType", out headerValues))
                {
                    doctype = headerValues.FirstOrDefault();
                    bp.log("X-IDL-RequestedDocType::" + doctype, 1, LOGTYPE.API);
                }

                string source = null;
                if (request.Headers.TryGetValues("X-IDL-Source", out headerValues))
                {
                    source = headerValues.FirstOrDefault();
                    bp.log("X-IDL-Source::" + source, 1, LOGTYPE.API);
                }

                if (string.IsNullOrEmpty(doctype))
                {
                    throw new InvalidCastException("RequestedDocType required");
                }

                if (doctype == "B2B" || doctype == "B2C")
                {

                }
                else throw new InvalidCastException("Invalid request");

                string strrr = null;

                if (request.Headers.TryGetValues("PartyTaxSchemeCompanyId", out headerValues))
                {
                    strrr = headerValues.FirstOrDefault();
                    bp.log("PartyTaxSchemeCompanyId::" + strrr, 1, LOGTYPE.API);
                }
                if (strrr.Equals(Basepage.partyTaxSchemeCompanyId) == false)
                {
                    throw new InvalidCastException("Invalid request paramter");
                }


                Enum.TryParse(doctype, out DOCTYPE ddd);
                Enum.TryParse(source, out SOURCE src);

                var provider = new MultipartMemoryStreamProvider();
                var content = await request.Content.ReadAsMultipartAsync(provider);
                var body = await content.Contents.Single(x => x.Headers.ContentDisposition.Name == "\"file\"").ReadAsStringAsync();


                var fileName = "";
                string contentTypeValue = "";
                foreach (var file in provider.Contents)
                {
                    fileName = file.Headers.ContentDisposition.FileName;
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        fileName = fileName.Trim('"');
                    }
                }
                bp.log("FileName:" + fileName, 1, LOGTYPE.API);

                fileName = Path.GetFileName(fileName);

                var filePath = invoice.Get_Folder(ddd, src, XMLSTATUS.UNSIGNED, "");
                bool exists = System.IO.Directory.Exists(filePath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(filePath);

                filePath = filePath + fileName;

                if (File.Exists(filePath))  
                {
                    //if unsigned xml exists already in file, that means the file already came and signed, thus dont save and sign again,
                    //it will cause same invoice with multiple ICV generated,
                    //but second one overrwriten by first one, thus causing missing ICV issue. 

                    string[] arr = new invoice().CheckInvoiceQR_Exists(filePath, null, LOGTYPE.API);
                    string qr = arr[0];
                    string irn = arr[1];
                    string uuid1 = arr[2];
                    string invHash = arr[3];
                    if (qr != "")
                    {
                        retData = new DataReturn()
                        {
                            QR = qr,
                            InvReferenceNumber = irn,
                            UUID = uuid1,
                            DocumentType = ddd,
                            InvoiceHash = invHash,
                            RESPMSG = "SUCCESS",
                            Warnings = new List<string>() { "QR already generated for the specified IRN." },
                            RC = 0,
                            change_icv_pih = false
                        };
                    }
                }
                else
                {
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.Write(body);

                    }

                    string InvoiceTypeCode = Basepage.getXMLNodeValue("d:Invoice/cbc:InvoiceTypeCode", filePath);
                    //string InvoiceTypeCode = Basepage.getXMLNodeValue("*[local-name()='Invoice']//*[local-name()='InvoiceTypeCode']/[@name]", filePath);

                    if (ddd == DOCTYPE.B2B && InvoiceTypeCode.StartsWith("01") == false)
                    {
                        File.Delete(filePath);
                        bp.log(ddd + " " + InvoiceTypeCode + "mismatch", 1, LOGTYPE.API);
                        throw new InvalidCastException(ddd.ToString() + "," + InvoiceTypeCode + ": document type and InvoiceTypeCode.name are in mismatch!");
                    }
                    if (ddd == DOCTYPE.B2C && InvoiceTypeCode.StartsWith("02") == false)
                    {
                        File.Delete(filePath);
                        bp.log(ddd + " " + InvoiceTypeCode + "mismatch", 1, LOGTYPE.API);
                        throw new InvalidCastException(ddd.ToString() + "," + InvoiceTypeCode + ": document type and InvoiceTypeCode.name  are in mismatch!");
                    }




                      retData = new invoice().Process_POS(filePath, ddd, LOGTYPE.API, src);
                    //    return retData;

                   
                }

                DataReturnAPI rett = new DataReturnAPI();
                rett.ClearedInvoice = retData.ClearedInvoice;
                rett.DocType = retData?.DocumentType.ToString();
                rett.QR = retData.QR;
                rett.UUID = retData.UUID;
                rett.TaxTotal = retData?.TaxTotal;
                rett.RC = retData.RC;
                rett.RESPMSG = retData?.RESPMSG?.Replace("C:\\Users\\Rigesh\\Documents\\Visual Studio 2015\\Projects\\ZATCA\\ZATCA", "");
                rett.info = retData.info;
                rett.Errors = retData.Errors;
                rett.Warnings = retData.Warnings;
                rett.InvoiceHash = retData.InvoiceHash;
                rett.InvReferenceNumber = retData.InvReferenceNumber;
                rett.Process = retData?._action.ToString();
                rett.ProcessStatus = retData?._actionStatus.ToString();
                return rett;





            }
            catch (Exception ex)
            {
                bp.log("Error::\r\n" + ex.ToString(), 1, LOGTYPE.API);

                string msg = ex.Message;
                int code = 99999;

                if (msg.Contains(" custerr_"))
                {
                    msg = ex.Message.Replace("custerr_", "").Replace("_", " ");

                }



                DataReturn retData = new DataReturn()
                {
                    RESPMSG = msg,
                    RC = code
                };


                DataReturnAPI rett = new DataReturnAPI();
                rett.ClearedInvoice = retData.ClearedInvoice;
                rett.DocType = retData.DocumentType.ToString();
                rett.QR = retData.QR;
                rett.UUID = retData.UUID;
                rett.TaxTotal = retData.TaxTotal;
                rett.RC = retData.RC;
                rett.RESPMSG = retData.RESPMSG.Replace("C:\\Users\\Rigesh\\Documents\\Visual Studio 2015\\Projects\\ZATCA\\ZATCA", "");
                rett.info = retData.info;
                rett.Errors = retData.Errors;
                rett.Warnings = retData.Warnings;
                rett.InvoiceHash = retData.InvoiceHash;
                rett.InvReferenceNumber = retData.InvReferenceNumber;
                rett.Process = "";  //retData._action.ToString()
                rett.ProcessStatus = ""; //retData._actionStatus.ToString();
                return rett;
            }
        }


        [HttpPost]
        //// call http://localhost:8871/api/einvoice/responsezatca
        ///For b2b and b2c xml creation, signing and sending QR back
        [ActionName("responsezatca")]
        [BasicAuthentication]
        public ZATCAReturnAPI ZatcaResponse(HttpRequestMessage postedData)
        {
            //request and response in json format
            invoice VV = new invoice();

            bp.log("------------------------API Call------------------------", 1, LOGTYPE.API);

            DOCTYPE ddd = DOCTYPE.B2B; 
            try
            {
                var sss = postedData.Content.ReadAsStringAsync().Result;
                bp.log("Request::\r\n" + sss, 0, LOGTYPE.API);


                //validating header-------------------    
                IEnumerable<string> headerValues;
                //X-IDL-RequestedInvType
                string doctype = null;
                if (postedData.Headers.TryGetValues("X-IDL-RequestedDocType", out headerValues))
                {
                    doctype = headerValues.FirstOrDefault();
                    bp.log("X-IDL-RequestedDocType::" + doctype, 1, LOGTYPE.API);
                }

                if (string.IsNullOrEmpty(doctype))
                {
                    throw new InvalidCastException("RequestedDocType required");
                }

                if (doctype == "B2B" || doctype == "B2C")
                {

                }
                else throw new InvalidCastException("Invalid request");

                string strrr = null;

                if (postedData.Headers.TryGetValues("PartyTaxSchemeCompanyId", out headerValues))
                {
                    strrr = headerValues.FirstOrDefault();
                    bp.log("PartyTaxSchemeCompanyId::" + strrr, 1, LOGTYPE.API);
                }
                if (strrr.Equals(Basepage.partyTaxSchemeCompanyId) == false)
                {
                    throw new InvalidCastException("Invalid request paramter");
                }



                Enum.TryParse(doctype, out   ddd);


                ZATCAReturnAPI rett = new invoice().GetZatcaResponse(sss, ddd, LOGTYPE.API, SOURCE.EXTERNAL);


                return rett;



            }
            catch (Exception ex)
            {
                bp.log("Error::\r\n" + ex.ToString(), 1, LOGTYPE.API);

                string msg = ex.Message;
                int code = 99999;

                if (msg.Contains(" custerr_"))
                {
                    msg = ex.Message.Replace("custerr_", "").Replace("_", " ");

                }

                DataReturn retData = new DataReturn()
                {
                    RESPMSG = msg,
                    RC = code
                };

                ZATCAReturnAPI rett = new ZATCAReturnAPI();
                rett.DocType = ddd.ToString();
                rett.QR = retData.QR;
                rett.UUID = retData.UUID;
                rett.TaxTotal = retData.TaxTotal;
                rett.RC = retData.RC;
                rett.RESPMSG = retData.RESPMSG.Replace("C:\\Users\\Rigesh\\Documents\\Visual Studio 2015\\Projects\\ZATCA\\ZATCA", "");
                rett.InvoiceHash = retData.InvoiceHash;
                rett.InvReferenceNumber = retData.InvReferenceNumber;
                return rett;

            }

        }

        [HttpPost]
        //// call http://localhost:8871/api/einvoice/responsezatca
        ///For b2b and b2c xml creation, signing and sending QR back
        [ActionName("getB2BCustomer")]
        [BasicAuthentication]
        public ReturnB2BCustomer GetB2BCustomer(HttpRequestMessage postedData)
        {
            //request and response in json format
            invoice VV = new invoice();

            bp.log("------------------------API Call------------------------", 1, LOGTYPE.API);

            DOCTYPE ddd = DOCTYPE.B2B;
            try
            {
                var sss = postedData.Content.ReadAsStringAsync().Result;
                bp.log("Request::\r\n" + sss, 0, LOGTYPE.API);


                //validating header-------------------    
                IEnumerable<string> headerValues;
                //X-IDL-RequestedInvType
                string doctype = null;
                if (postedData.Headers.TryGetValues("X-IDL-RequestedDocType", out headerValues))
                {
                    doctype = headerValues.FirstOrDefault();
                    bp.log("X-IDL-RequestedDocType::" + doctype, 1, LOGTYPE.API);
                }

                if (string.IsNullOrEmpty(doctype))
                {
                    throw new InvalidCastException("RequestedDocType required");
                }

                if (doctype == "B2B"  )
                {

                }
                else throw new InvalidCastException("Invalid request doctype");

                string strrr = null;

                if (postedData.Headers.TryGetValues("PartyTaxSchemeCompanyId", out headerValues))
                {
                    strrr = headerValues.FirstOrDefault();
                    bp.log("PartyTaxSchemeCompanyId::" + strrr, 1, LOGTYPE.API);
                }
                if (strrr.Equals(Basepage.partyTaxSchemeCompanyId) == false)
                {
                    throw new InvalidCastException("Invalid request paramter");
                }



                Enum.TryParse(doctype, out ddd);


                ReturnB2BCustomer rett = new invoice().getB2BCustomer_from_DB(sss, ddd, LOGTYPE.API, SOURCE.EXTERNAL);


                return rett;



            }
            catch (Exception ex)
            {
                bp.log("Error::\r\n" + ex.ToString(), 1, LOGTYPE.API);

                string msg = ex.Message;
                int code = 99999;

                if (msg.Contains(" custerr_"))
                {
                    msg = ex.Message.Replace("custerr_", "").Replace("_", " ");

                }

                 

                ReturnB2BCustomer rett = new ReturnB2BCustomer();
                rett.RegistrationNameArabic = "";
                rett.RegistrationNameEng = ""; 
               
                rett.ResponseMessage =msg.Replace("C:\\Users\\Rigesh\\Documents\\Visual Studio 2015\\Projects\\ZATCA\\ZATCA", "");
                rett.ResponseCode = 1;
                rett.CustomerVATNo = ""; 
                return rett;

            }

        }

    }
    public class DataReturn
    {
        public DOCTYPE DocumentType { get; set; }
        public string QR { get; set; }
        public string InvReferenceNumber { get; set; }
        public string InvoiceHash { get; set; }
        public string ClearedInvoice { get; set; }
        public string UUID { get; set; }
        public string TaxTotal { get; set; }
        public int RC { get; set; }   //response code
        public string RESPMSG { get; set; }
        public string HTTPResponseCode { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> info { get; set; }
        public string FullResponse { get; set; }
        public ACTION _action { get; set; }
        public ACTION_STATUS _actionStatus { get; set; }
        public bool change_icv_pih { get; set; }
        public Int64 ICV { get; set; }
        public string EGS_X509SerialNumber { get; set; }
        public INV_TYPE InvoiceType { get; set; }
        public string InvoiceTotal { get; set; }
    }


    public class DataReturnAPI
    {
        public string DocType { get; set; }
        public string QR { get; set; }
        public string InvReferenceNumber { get; set; }
        public string InvoiceHash { get; set; }
        public string ClearedInvoice { get; set; }
        public string UUID { get; set; }
        public string TaxTotal { get; set; }
        public int RC { get; set; }   //response code
        public string RESPMSG { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> info { get; set; }
        public string Process { get; set; }  //_action
        public string ProcessStatus { get; set; } // _actionStatus 

    }

    public class ZATCAReturnAPI
    {
        public string DocType { get; set; }
        public string QR { get; set; }
        public string InvReferenceNumber { get; set; }
        public string InvoiceHash { get; set; }
        public string UUID { get; set; }
        public string TaxTotal { get; set; }
        public int RC { get; set; }   //response code
        public string RESPMSG { get; set; }
        public string FullResponse { get; set; }

    }

    public class ReturnB2BCustomer
    {
        public string RegistrationNameEng { get; set; }
        public string RegistrationNameArabic { get; set; } 
        public string CustomerVATNo { get; set; } 
        public string ResponseMessage { get; set; } 
        public int ResponseCode { get; set; } 

    }
    public enum DOCTYPE
    {
        B2B,
        B2C
    }
    public enum INV_TYPE
    {
        DEBIT,
        CREDIT,
        INVOICE,
        PREPAYMENT
    }
    public enum MailType
    {
        ServiceError,
        ReportingError,
        ClearanceError,
        NoStoreData,
        ReportUrgent,
        SystemError,
        Notification,
        DBUpdateError

    }
}
