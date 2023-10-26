using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SDKNETFrameWorkLib.BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Xml;

namespace ZatcaWebApp
{
    public partial class Onboard : System.Web.UI.Page
    {
        Basepage bp = new Basepage();
        public bool isClientScrpiptActive = true;

        public static string NS_XMLNS = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
        public static string NS_CBC = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        public static string NS_CAC = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
        public static string NS_EXT = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";

        static string  proxy = ConfigurationManager.AppSettings["proxy"].ToString();
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                this.Master.Page_heading = "Onboarding";

                this.Master.TextMessage.Text = "";
                if (!IsPostBack)
                {
                    if (Session["userName"] == null)
                    {
                       /// Session.Clear();
                       // Session.Abandon();
                      //  Response.Redirect("Login.aspx");
                    }
                    else
                    {
                        Session["PageId"] = "li_onboarding";


                    }

                    LoadFields();

                }

            }
            catch (Exception ex)
            {
                //  bp.logWrite(ex.ToString(), LOGTYPE.CMPL);
                //  this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
                Response.Write(ex.ToString());
            }


        }

        private void LoadFields()
        {
            txtCSRLocation.Text = Basepage.CSR_Path;
            txtPrivKeyLocation.Text = Basepage.private_key_path;
            txtComplInvLocation.Text = Basepage.invoices_for_compliance_Check_folder;
            txtCertLocation.Text = Basepage.cert_pem_path;
            txtComplianceCSIDapi.Text = Basepage.Compliance_CSID_API;
            txtComplianceInvoiceAPI.Text = Basepage.Compliance_Invoice_API;
            txtProdCSIDapi.Text = Basepage.Production_CSID_API; 
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";

                onboard_EGS();

            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.CMPL);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }

        }

        public void onboard_EGS()
        {
            //steps
            //load OTP from portal to config.ini
            //generate CSR and base64 encode if any paremeter in cnf changed, in our case the csr and privte key is getting generated on each onboarding.
            //send base64 encoded CSR for compliance csid
            //send invoices for compliance checks
            //request prod csid

            //in case renewal of csid , if no change in cnf file, send the existing csr directly to renew
            //for renewal of csid, if there is change in cnf file, then all the steps need to be followed, ie, complaince csid, compliance checks and request prod csid.

            //I am sending xmls for compliance checks in both the cases above to be on the safe side.

            //after getting binarySecurityToken, decoding it to get compliance cert. 
            //after getting compliance csid, I am signing the xml using previously generated private key and compliance cert. *****

            try
            {

                bp.logWrite("Onboarding Started....", LOGTYPE.CMPL);

                string[] ret1;
                //if (chk_loadauthtext.Checked == false)
                {
                    ret1 = Compliance_CSID_API();
                }
                // else ret1 = ReadFromFile();

                if (ret1 == null)
                {
                    bp.logWrite("Process ended.", LOGTYPE.CMPL);
                }
                else
                {
                    string requestID = ret1[0];
                    string binarySecurityToken = ret1[1];
                    string auth_encoded = ret1[2];
                    //               
                    //                string auth=binarySecurityToken+":"+secret;
                    //                 string auth_encoded = Base64.getEncoder().encodeTostring(auth.getBytes());

                    bp.logWrite("auth_encoded:" + auth_encoded, LOGTYPE.CMPL);

                    string ret = Compliance_Invoice_API(binarySecurityToken, auth_encoded);

                    if (ret.Equals("success"))
                    {
                         string[] ret2 = Production_CSID_request_API(requestID, auth_encoded);
                     //   string requestID_prod = "1152";
                     //   string binarySecurityToken_prod = "TUlJRXlqQ0NCSENnQXdJQkFnSVRiUUFBQklDeXFmV21qWTcreGdBQUFBQUVnREFLQmdncWhrak9QUVFEQWpCaU1SVXdFd1lLQ1pJbWlaUHlMR1FCR1JZRmJHOWpZV3d4RXpBUkJnb0praWFKay9Jc1pBRVpGZ05uYjNZeEZ6QVZCZ29Ka2lhSmsvSXNaQUVaRmdkbGVIUm5ZWHAwTVJzd0dRWURWUVFERXhKUVJWcEZTVTVXVDBsRFJWTkRRVEl0UTBFd0hoY05Nak13TkRBNU1UUXpOekU0V2hjTk1qTXdPREE0TVRJeU5qUTJXakJMTVFzd0NRWURWUVFHRXdKVFFURVBNQTBHQTFVRUNoTUdVa1ZVUVVsTU1SVXdFd1lEVlFRTEV3eFNhWGxoWkNCQ2NtRnVZMmd4RkRBU0JnTlZCQU1UQzBWQk1USXpORGc0TnpnNU1GWXdFQVlIS29aSXpqMENBUVlGSzRFRUFBb0RRZ0FFMDIrUmdGWWdiUTMzS0NWc1BkSTZsMHFTeERESWsxREI5UXp1TnhzZ0RTaTlGS3E4dVJnUHdhVW5pMy95VnNlY0Y2Y0o5V3V1aTJvendveEphQWJsUDZPQ0F4MHdnZ01aTUNjR0NTc0dBUVFCZ2pjVkNnUWFNQmd3Q2dZSUt3WUJCUVVIQXdJd0NnWUlLd1lCQlFVSEF3TXdQQVlKS3dZQkJBR0NOeFVIQkM4d0xRWWxLd1lCQkFHQ054VUlnWWFvSFlUUSt4S0c3WjBraDg3N0dkUEFWV2FCbk5ndGcrWEZYUUlCWkFJQkV6Q0J6UVlJS3dZQkJRVUhBUUVFZ2NBd2diMHdnYm9HQ0NzR0FRVUZCekFDaG9HdGJHUmhjRG92THk5RFRqMVFSVnBGU1U1V1QwbERSVk5EUVRJdFEwRXNRMDQ5UVVsQkxFTk9QVkIxWW14cFl5VXlNRXRsZVNVeU1GTmxjblpwWTJWekxFTk9QVk5sY25acFkyVnpMRU5PUFVOdmJtWnBaM1Z5WVhScGIyNHNSRU05WlhoMFoyRjZkQ3hFUXoxbmIzWXNSRU05Ykc5allXdy9ZMEZEWlhKMGFXWnBZMkYwWlQ5aVlYTmxQMjlpYW1WamRFTnNZWE56UFdObGNuUnBabWxqWVhScGIyNUJkWFJvYjNKcGRIa3dIUVlEVlIwT0JCWUVGTXk3N0d0cUhWWVMvMEwyb2phOUVsSmRqdCtnTUE0R0ExVWREd0VCL3dRRUF3SUhnRENCakFZRFZSMFJCSUdFTUlHQnBIOHdmVEVwTUNjR0ExVUVCQXdnTVMxQlkyMWxTVzVqZkRJdFJWTkhWVzVwZEh3ekxUSXlPRGc0TmpjeU9Ea3hIekFkQmdvSmtpYUprL0lzWkFFQkRBOHpNREEwTmpVek9UVTFNREF3TURNeERUQUxCZ05WQkF3TUJERXhNREF4RHpBTkJnTlZCQm9NQmxKcGVXRmthREVQTUEwR0ExVUVEd3dHVWtWVVFVbE1NSUhoQmdOVkhSOEVnZGt3Z2RZd2dkT2dnZENnZ2MyR2djcHNaR0Z3T2k4dkwwTk9QVkJGV2tWSlRsWlBTVU5GVTBOQk1pMURRU3hEVGoxUVJWcEZhVzUyYjJsalpYTmpZVElzUTA0OVEwUlFMRU5PUFZCMVlteHBZeVV5TUV0bGVTVXlNRk5sY25acFkyVnpMRU5PUFZObGNuWnBZMlZ6TEVOT1BVTnZibVpwWjNWeVlYUnBiMjRzUkVNOVpYaDBaMkY2ZEN4RVF6MW5iM1lzUkVNOWJHOWpZV3cvWTJWeWRHbG1hV05oZEdWU1pYWnZZMkYwYVc5dVRHbHpkRDlpWVhObFAyOWlhbVZqZEVOc1lYTnpQV05TVEVScGMzUnlhV0oxZEdsdmJsQnZhVzUwTUI4R0ExVWRJd1FZTUJhQUZJZWwyd0s5ZDgxSGIya0JlM2d0R3ZYdXg4QlJNQjBHQTFVZEpRUVdNQlFHQ0NzR0FRVUZCd01DQmdnckJnRUZCUWNEQXpBS0JnZ3Foa2pPUFFRREFnTklBREJGQWlBVDRobWp0VGVhdjU5Zjd6VFdhTWJWZUh5U0NXM1U2VXcyeDhzbWF1MjlhZ0loQU1NelF4M090MmNsNGk4UTZFZFFFYVV6aDVKNDdXVXdJOWE1SXVCam1NcjE=";
                    //    string secret_prod = "uablSpj92jVmIDnI0tZkN8FbPB72txjTms4vRL5/mdc=";
                     //   string[] ret2 = new string[] { requestID_prod , binarySecurityToken_prod , secret_prod };

                        if (ret2 == null)
                        {
                            bp.logWrite("Process ended.!", LOGTYPE.CMPL);
                        }
                        else
                        {
                            string requestID_prod = ret2[0];
                            string binarySecurityToken_prod = ret2[1];
                            string secret_prod = ret2[2];

                            //save new CSID as cert in the cert.pem
                           

                            Save_cert(binarySecurityToken_prod, secret_prod);

                            //reseting ICV_PIH file
                            reset_ICV_PIH();
                        }
                    }
                }

                this.Master.ShowMessage("OnBoarding Successful.Verify log files", Alert.error, isClientScrpiptActive);

            }
            catch (Exception ex)
            {

                bp.logWrite("onboard_EGS method error:" + ex.ToString(), LOGTYPE.CMPL);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }
        }

        public string[] Compliance_CSID_API()
        {
            string[]
            ret = null;



            if (chk_loadauthtext.Checked == true)
            {
                bp.logWrite("taking auth_encoded,binarySecurityToken,requestID from file...", LOGTYPE.CMPL);

                ret = ReadFromFile();
            }
            else {  


            IRestResponse response1 = null;


            bp.logWrite("Compliance_CSID_API Started....", LOGTYPE.CMPL);

            //Reading CSR file
            string csr = System.IO.File.ReadAllText(txtCSRLocation.Text, Encoding.UTF8);
            string base64encoded_csr = Basepage.Base64Encode(csr);

            bp.logWrite("base64encoded_csr:" + base64encoded_csr, LOGTYPE.CMPL);


            string body = "{ \"csr\": \"<CSR>\"}";
            body = body.Replace("<CSR>", base64encoded_csr);

            bp.logWrite("body:" + body, LOGTYPE.CMPL);

            bp.logWrite("Compliance_CSID_API:" + txtComplianceCSIDapi.Text.Trim(), LOGTYPE.CMPL);

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                //not using HttpClient because it is not giving response body when error code is 400 bad request
                var client = new RestClient(txtComplianceCSIDapi.Text.Trim());

              
                
                if(!proxy.Trim().Equals(""))
                client.Proxy = new WebProxy(proxy);

            //client.Timeout = -1;
            var request = new RestRequest(Method.POST);

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                request.AddHeader("accept", "application/json");
            request.AddHeader("OTP", txtOTP.Text.Trim());
            request.AddHeader("Accept-Version", "V2");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            response1 = client.Execute(request);

            bp.logWrite("ErrorException :" + response1.ErrorException, LOGTYPE.CMPL);
            bp.logWrite("ErrorMessage :" + response1.ErrorMessage, LOGTYPE.CMPL);
            bp.logWrite("ResponseCode:" + response1.StatusCode, LOGTYPE.CMPL);
            bp.logWrite("ResponseBody:" + response1.Content, LOGTYPE.CMPL);



            if (response1.StatusCode == System.Net.HttpStatusCode.OK)  //200
            {
                var data = (JObject)JsonConvert.DeserializeObject(response1.Content);

                string requestID = data["requestID"]?.Value<string>();
                string dispositionMessage = data["dispositionMessage"]?.Value<string>();
                string binarySecurityToken = data["binarySecurityToken"]?.Value<string>();
                string secret = data["secret"]?.Value<string>();

                //JSONObject errors =obj.has("errors")? obj.getJSONObject("errors"):null;

                if (dispositionMessage.Equals("ISSUED"))
                {

                    string auth = binarySecurityToken + ":" + secret;
                    string auth_encoded = Basepage.Base64Encode(auth);

                    string pathhh = txtCSRLocation.Text.Trim().Replace(Path.GetFileName(txtCSRLocation.Text.Trim()), "auth_encoded.txt");

                    File.WriteAllText(pathhh, auth_encoded, Encoding.UTF8);


                    pathhh = txtCSRLocation.Text.Trim().Replace(Path.GetFileName(txtCSRLocation.Text.Trim()), "binarySecurityToken.txt");
                    File.WriteAllText(pathhh, binarySecurityToken, Encoding.UTF8);

                    pathhh = txtCSRLocation.Text.Trim().Replace(Path.GetFileName(txtCSRLocation.Text.Trim()), "requestID.txt");
                    File.WriteAllText(pathhh, requestID, Encoding.UTF8);

                    ret = new string[] { requestID, binarySecurityToken, auth_encoded };
                }

            }
            else if (response1.StatusCode == System.Net.HttpStatusCode.Conflict)
            {

                if (response1.Content.Contains("Compliance transaction was submitted"))
                {
                    //means that csr was already submitted and we already have requestDI, and other fields saved in file.
                    //so get these fields from files

                    bp.logWrite("taking auth_encoded,binarySecurityToken,requestID from file...", LOGTYPE.CMPL);

                    ret = ReadFromFile();


                }

            }
            else
            {
                throw new InvalidCastException("Compliance_CSID_API Failed : HTTP error code : "
                        + response1.StatusCode + " - " + response1.Content);
            }
            //               for (Map.Entry<string, List<string>> header : conn.getHeaderFields().entrySet()) {
            //    util.Util.log(header.getKey() + "=" + header.getValue());
            // }

        }     

            return ret;
        }
        public List<string> readIcvPih_fromFile(LOGTYPE TYP)
        {

            FileStream myFileStream = null;
            List<string> myList = new List<string>();
            string myLine;

            bp.log("Reading ICV_PIH file:", 1, TYP);

            try
            {
                while (true)
                {
                    try
                    {
                        myFileStream = new FileStream(Basepage.ICV_PIH_file, FileMode.Open, FileAccess.Read, FileShare.None);
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
                    myList.Add(myLine.Substring(4));
                }

                myStreamReader.Close();
                myFileStream.Close();
                myFileStream.Dispose();

            }
            finally
            {
                if (myFileStream != null)
                {
                    myFileStream.Dispose();
                }
            }
            bp.log("ICV:" + myList[0], 1, TYP);
            bp.log("PIH:" + myList[1], 1, TYP);

            if (myList[0] == null || myList[0].Equals("")) throw new InvalidCastException("ICV error.Try again");
            if (myList[1] == null || myList[1].Equals("")) throw new InvalidCastException("PIH error.Try again");
            return myList;

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


        public string Compliance_Invoice_API(string binarySecurityToken, string auth_encoded)
        {

            string ret = "success";



            string compliance_cert = Basepage.Base64Decode(binarySecurityToken);

            
          //  CertificateData dd = GetCertData_ONBOARD();

            ///bp.logWrite("Signing using below private key and compliance_cert...", LOGTYPE.CMPL);
           // bp.logWrite("private_key:" + dd.privateKeytext, LOGTYPE.CMPL);
            bp.logWrite("compliance_cert:" + compliance_cert, LOGTYPE.CMPL);


            bp.logWrite("Compliance_Invoice_API Started....", LOGTYPE.CMPL);
            bp.logWrite("Reading invoices XML files for compliace check....", LOGTYPE.CMPL);
            bp.logWrite("XML file folder:" +txtComplInvLocation.Text.Trim(), LOGTYPE.CMPL);


            string[] files = Directory.GetFiles(txtComplInvLocation.Text.Trim(), "*.xml", SearchOption.TopDirectoryOnly);

            bp.logWrite("fileCount:" + files.Length, LOGTYPE.CMPL);
            if (files.Length < 3)
                throw new InvalidCastException("3 invoices must be present for compliance checks");



            foreach (string file in files)
            {

                try
                {

                    string fileName = Path.GetFileName(file);
                    string currentFilePath = txtComplInvLocation.Text.Trim() + fileName;
                    bp.logWrite("Reading " + currentFilePath, LOGTYPE.CMPL);

                    // string str = File.ReadAllText(currentFilePath);
                    //util.Util.log("invoiceXML:\n" + str,Main.logType.CMPL);


                    string uuid = util.getXMLNodeValue("//d:Invoice/cbc:UUID", currentFilePath);




                    //List<string> result = readIcvPih_fromFile(LOGTYPE.CMPL);
                   // string ICV = result[0];
                   // string pih = result[1];

                    //  SetFields_to_XML_NEW(currentFilePath, ICV, pih, uuid, LOGTYPE.CMPL);

                    //signing the invoice
                    //CertificateData certData = GetCertData_ONBOARD();
                    EInvoiceSigningLogic eee = new EInvoiceSigningLogic();
                     
                    string ppp = File.ReadAllText(txtPrivKeyLocation.Text.ToLowerInvariant());
                    string privateKeytext = ppp;
                    privateKeytext = privateKeytext.Replace("-----BEGIN EC PRIVATE KEY-----","");
                    privateKeytext = privateKeytext.Replace("-----END EC PRIVATE KEY-----","");
                    //remove new line

                    //privateKeytext = "MHQCAQEEIMuKQyOv9ce3dIlk0w/U8KE0Jb3xQHW5nJ5TpvsTG+aNoAcGBSuBBAAKoUQDQgAE02+RgFYgbQ33KCVsPdI6l0qSxDDIk1DB9QzuNxsgDSi9FKq8uRgPwaUni3/yVsecF6cJ9Wuui2ozwoxJaAblPw==";
                    privateKeytext = privateKeytext.Replace(System.Environment.NewLine, "");
                    string[] arr =new  util().SignXML_Compliance(currentFilePath, fileName, compliance_cert, privateKeytext);

                    string invocie_encoded = Basepage.Base64Encode(arr[0]);
                    // invocie_encoded="dsfsgdfgfd\+";
                    //create request body.
                    string body = "{  \"invoiceHash\": \"<invoiceHash>\",  \"uuid\": \"<uuid>\", \"invoice\": \"<invoice>\"}";
                    body = body.Replace("<invoiceHash>", arr[1]);
                    body = body.Replace("<uuid>", uuid);
                    body = body.Replace("<invoice>", invocie_encoded);

                    bp.logWrite("body:" + body, LOGTYPE.CMPL);
                    string reportingStatus = Send_Compliant_invoice(body, auth_encoded);
                    if (reportingStatus.Equals("SUCCESS") == false)
                    {
                        ret = "failed";
                        break;   //****** should it continue if one invocie is not cleared?
                    }

                }
                catch (Exception ex)
                {
                    ret = "failed";
                    bp.logWrite("errr1:" + ex.ToString(), LOGTYPE.CMPL);
                }

            }






            return ret;
        }

        public  CertificateData GetCertData_ONBOARD( )
        {
            string cert_pem_path = txtCSRLocation.Text.Trim().Replace(Path.GetFileName(txtCSRLocation.Text.Trim()), "cert.pem");
            string private_key_path = txtCSRLocation.Text.Trim().Replace(Path.GetFileName(txtCSRLocation.Text.Trim()), "pk.pem");


            System.Security.Cryptography.X509Certificates.X509Certificate2 cert = new X509Certificate2(cert_pem_path, private_key_path,
                            X509KeyStorageFlags.MachineKeySet
                          | X509KeyStorageFlags.PersistKeySet);

            string certificate = Convert.ToBase64String(cert.RawData);

            CertificateData cdata = new CertificateData();
            cdata.RawDataString = certificate;
            cdata.Certificate_issuer_name = cert.IssuerName.ToString();
            cdata.X509SerialNumber = cert.SerialNumber;
            cdata.RawDataByte = cert.RawData;
            cdata.PublicKey = cert.PublicKey.ToString();

            string ppp = File.ReadAllText(private_key_path);
            cdata.privateKeytext = ppp;

            return cdata;

        }
        private string Send_Compliant_invoice(string body, string auth_encoded)
        {

            string ret = "";
            IRestResponse response1 = null;

            bp.logWrite("Compliance_Invoice_API:" + txtComplianceInvoiceAPI.Text.Trim(), LOGTYPE.CMPL);

            bp.logWrite("body:" + body, LOGTYPE.CMPL);


            //not using HttpClient because it is not giving response body when error code is 400 bad request
            var client = new RestClient(txtComplianceInvoiceAPI.Text.Trim());
            if (!proxy.Trim().Equals(""))
                client.Proxy = new WebProxy(proxy);

            //client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Accept-Language", "en");
            request.AddHeader("Authorization", "Basic " + auth_encoded); 
            //   client1.DefaultRequestHeaders.Add("Accept-Charset", "UTF-8"); 
            request.AddHeader("Accept-Version", "V2");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            response1 = client.Execute(request);

            bp.logWrite("ResponseCode:" + response1.StatusCode, LOGTYPE.CMPL);
            bp.logWrite("ResponseBody:" + response1.Content, LOGTYPE.CMPL);

            string resp = response1.Content;


                if (response1.StatusCode != System.Net.HttpStatusCode.OK)  //200
                {
                    throw new InvalidCastException("Send_Compliant_invoice Failed : HTTP error code : "
                                + response1.StatusCode + " - " + resp);

                }

                var data = (JObject)JsonConvert.DeserializeObject(resp);
                string reportingStatus = data["reportingStatus"]?.Value<string>();
                string clearanceStatus = data["clearanceStatus"]?.Value<string>();

                if (reportingStatus !=null && reportingStatus.Equals("REPORTED"))
                {
                    ret = "SUCCESS";
                }
            if (clearanceStatus != null && clearanceStatus.Equals("CLEARED"))
            {
                ret = "SUCCESS";
            }

            return ret;

             

        }

        public string[] Production_CSID_request_API(string requestID, string auth_encoded)
        {
            string[]
            ret = null;

            IRestResponse response1 = null;

            bp.logWrite("Production_CSID_request_API Started....", LOGTYPE.CMPL);

            string body = "{  \"compliance_request_id\": \"<compliance_request_id>\"}";
            body = body.Replace("<compliance_request_id>", requestID);


            bp.logWrite("Production_CSID_API:" + txtProdCSIDapi.Text.Trim(), LOGTYPE.CMPL);

            bp.logWrite("body:" + body, LOGTYPE.CMPL);



            
            string reqDate = DateTime.Now.ToString();


            //not using HttpClient because it is not giving response body when error code is 400 bad request
            var client = new RestClient(txtProdCSIDapi.Text.Trim());
            if (!proxy.Trim().Equals(""))
                client.Proxy = new WebProxy(proxy);
            //client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Accept-Language", "en");
            request.AddHeader("Authorization", "Basic " + auth_encoded);
            //   client1.DefaultRequestHeaders.Add("Accept-Charset", "UTF-8"); 
            request.AddHeader("Accept-Version", "V2");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            response1 = client.Execute(request);

            bp.logWrite("ResponseCode:" + response1.StatusCode, LOGTYPE.CMPL);
            bp.logWrite("ResponseBody:" + response1.Content, LOGTYPE.CMPL);

            string resp = response1.Content;
            
               
             

                if (response1.StatusCode != System.Net.HttpStatusCode.OK)  //200
                {

                    throw new InvalidCastException("Production_CSID_request_API Failed : HTTP error code : "
                                + response1.StatusCode + " - " + resp);

                }

                var data = (JObject)JsonConvert.DeserializeObject(resp);
                string request_ID = data["requestID"]?.Value<string>();
                string dispositionMessage = data["dispositionMessage"]?.Value<string>();
                string binarySecurityToken = data["binarySecurityToken"]?.Value<string>();
                string secret = data["secret"]?.Value<string>();


                if (dispositionMessage.Equals("ISSUED"))
                {
                    ret = new string[] { request_ID, binarySecurityToken, secret };
                }

           

            return ret;
        }

        public void Save_cert(string binarySecurityToken_prod, string secret_prod)
        {
            string certpath = txtCSRLocation.Text.Trim().Replace(Path.GetFileName(txtCSRLocation.Text.Trim()), "cert.pem");
            //taking the backup existing cert
            bp.logWrite("taking the backup existing certif exists", LOGTYPE.CMPL);
            string ddd = DateTime.Now.ToString("yyMMddHHmmss");
            string backupname = certpath + ddd;

            if(File.Exists(certpath))
            System.IO.File.Move(certpath, backupname);


            bp.logWrite("decoding binarySecurityToken_prod..", LOGTYPE.CMPL);
            string cert = Basepage.Base64Decode(binarySecurityToken_prod);

            //Wrting new cert string to the cert file
            bp.logWrite("Wrting new cert string to the cert file " + Path.GetFileName(certpath), LOGTYPE.CMPL);
            File.WriteAllText(certpath, cert, Encoding.UTF8);


            //Wrting new secret_prod string to the secret_prod file

            //creating path in the same folder for secret file
            string scret_file = txtCSRLocation.Text.Trim().Replace(Path.GetFileName(txtCSRLocation.Text.Trim()), "secret.txt");  
            if (File.Exists(scret_file))
            {
                string secr_old = scret_file.Replace("secret.txt", "secret_" + ddd + ".txt");
                File.Move(scret_file, secr_old);
            }
            bp.logWrite("Wrting new secret_prod string to   file " + scret_file, LOGTYPE.CMPL);
            File.WriteAllText(scret_file, secret_prod, Encoding.UTF8);



        }
        public void reset_ICV_PIH()
        {

            //taking the backup existing  
            bp.logWrite("taking the backup existing ICV_PIH", LOGTYPE.CMPL);
            string ddd = DateTime.Now.ToString("ddMMyyHHmmss");
            string backupname = Basepage.ICV_PIH_file + ddd;
            
            if(File.Exists(Basepage.ICV_PIH_file))
            System.IO.File.Move(Basepage.ICV_PIH_file, backupname);




            bp.logWrite("SAVING:" + "ICV=1\n" +
            "PIH=NWZlY2ViNjZmZmM4NmYzOGQ5NTI3ODZjNmQ2OTZjNzljMmRiYzIzOWRkNGU5MWI0NjcyOWQ3M2EyN2ZiNTdlOQ==", LOGTYPE.CMPL);
            string cert = "ICV=1\n" +
            "PIH=NWZlY2ViNjZmZmM4NmYzOGQ5NTI3ODZjNmQ2OTZjNzljMmRiYzIzOWRkNGU5MWI0NjcyOWQ3M2EyN2ZiNTdlOQ==";

            //Writing org icv and pih to the icv_pih file
            bp.logWrite("Writing org icv and pih to the icv_pih file " + Path.GetFileName(Basepage.ICV_PIH_file), LOGTYPE.CMPL);
            File.WriteAllText(Basepage.ICV_PIH_file, cert, Encoding.UTF8);

            bp.logWrite("PIH_ICV file reset.Check and confirm the file " + Path.GetFileName(Basepage.ICV_PIH_file), LOGTYPE.CMPL);

        }

        string[] ReadFromFile()
        {
            string pathhh = txtCSRLocation.Text.Trim().Replace(Path.GetFileName(txtCSRLocation.Text.Trim()), "auth_encoded.txt");  
            string auth_encoded = File.ReadAllText(pathhh);

            pathhh = txtCSRLocation.Text.Trim().Replace(Path.GetFileName(txtCSRLocation.Text.Trim()), "binarySecurityToken.txt"); 
            string binarySecurityToken = File.ReadAllText(pathhh);

            pathhh = txtCSRLocation.Text.Trim().Replace(Path.GetFileName(txtCSRLocation.Text.Trim()), "requestID.txt");
            string requestID = File.ReadAllText(pathhh);

            return new string[] { requestID, binarySecurityToken, auth_encoded };

        }
    }
}