
//using Microsoft.Office.Interop.Excel;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Timers;
using System.Web; 
using SDKNETFrameWorkLib.BLL;
using SDKNETFrameWorkLib.GeneralLogic; 
using System.Security.Cryptography.X509Certificates; 
using System.Threading.Tasks;
using System.Xml;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Net;
using GatewayService;
using System.Net.Mail;

namespace ZATCA
{
    public enum LOGTYPE
    { MAIN,
        API,
         JOB,
        JOB_REP,
        JOB_RETRY_REP,
        JOB_SIGNING,
        JOB_B2C_FTP,
        JOB_STR_FILES,
        COUNTER,
        TXTREN,
        RPTED_REN,
        DB_BACKUP,
        DB_UPDATE_ERROR


    }

    public class Basepage
    {

         public static int logDebug = int.Parse(ConfigurationManager.AppSettings["logDebug"]);
         public static string LogFilePath = ConfigurationManager.AppSettings["LogFilePath"];
        //public static string onboarding = ConfigurationManager.AppSettings["onboarding"];
        //public static string OTP = ConfigurationManager.AppSettings["OTP"];
        //public static string CSR_Path = ConfigurationManager.AppSettings["CSR_Path"];
         public static string private_key_path = ConfigurationManager.AppSettings["private_key_path"];
         public static string cert_pem_path = ConfigurationManager.AppSettings["cert_pem_path"];
        //public static string invoices_for_compliance_Check_folder = ConfigurationManager.AppSettings["invoices_for_compliance_Check_folder"];
        //public static string Compliance_CSID_API = ConfigurationManager.AppSettings["Compliance_CSID_API"];
        //public static string Compliance_Invoice_API = ConfigurationManager.AppSettings["Compliance_Invoice_API"];
        //public static string Production_CSID_API = ConfigurationManager.AppSettings["Production_CSID_API"];
        public static string B2B_Clearance_API = ConfigurationManager.AppSettings["B2B_Clearance_API"];
        public static string Reporting_API = ConfigurationManager.AppSettings["Reporting_API"];
        public static string invoiceXML_folder = ConfigurationManager.AppSettings["invoiceXML_folder"];
         public static string partyTaxSchemeCompanyId = ConfigurationManager.AppSettings["partyTaxSchemeCompanyId"];
         public static string ICV_PIH_file = ConfigurationManager.AppSettings["ICV_PIH_file"];

        public static int JobZatca_reporting_timer_inSec = int.Parse(ConfigurationManager.AppSettings["JobZatca_reporting_timer_inSec"]??"0");
        public static int JobZatca_reporting_timer_FileCount = int.Parse(ConfigurationManager.AppSettings["JobZatca_reporting_timer_FileCount"] ?? "0");
        public static int JobZatca_check_files_from_Stores_minutes = int.Parse(ConfigurationManager.AppSettings["JobZatca_check_files_from_Stores_minutes"] ??"0");
        public static int JobZatca_RETRY_reporting_minutes = int.Parse(ConfigurationManager.AppSettings["JobZatca_RETRY_reporting_minutes"] ??"0");
        public static int JobZatca_B2C_FTP_inSec = int.Parse(ConfigurationManager.AppSettings["JobZatca_B2C_FTP_inSec"]);
        public static string xml_upload_ftp_ip_port = (ConfigurationManager.AppSettings["xml_upload_ftp_ip_port"]);
        public static string xml_upload_ftp_username = (ConfigurationManager.AppSettings["xml_upload_ftp_username"]);
        public static string xml_upload_ftp_password = (ConfigurationManager.AppSettings["xml_upload_ftp_password"]);
        public static string xml_upload_ftp__destination_fpath = (ConfigurationManager.AppSettings["xml_upload_ftp__destination_fpath"]);
        public static string is_localServer = (ConfigurationManager.AppSettings["is_localServer"]);
        public static string CustIdentifier = (ConfigurationManager.AppSettings["CustIdentifier"]);
        public static string resources_path = ConfigurationManager.AppSettings["resources_path"];

        public static string conStr = ConfigurationManager.ConnectionStrings["ConStr"]?.ConnectionString;
        public static string ConStrCustomerVAT = ConfigurationManager.ConnectionStrings["ConStrCustomerVAT"]?.ConnectionString;


        public static int clearlogfiledays = int.Parse(ConfigurationManager.AppSettings["clearlogfiledays"]);  

        //mail config
        public static string EnableAlertEmail = ConfigurationManager.AppSettings["EnableAlertEmail"];
        public static string fromEmail = ConfigurationManager.AppSettings["fromEmail"];
        public static string toEmails = ConfigurationManager.AppSettings["toEmails"];
        public static string EmailPassword = ConfigurationManager.AppSettings["EmailPassword"];
        public static string MailHost = ConfigurationManager.AppSettings["MailHost"];
        public static int MailPort = int.Parse(ConfigurationManager.AppSettings["MailPort"]); 


        public static string proxy = ConfigurationManager.AppSettings["proxy"];

        //public static  string PG_ConnectionString= ConfigurationManager.AppSettings["PG_ConnectionString"]?.ToString();


        public static string Environment = ConfigurationManager.AppSettings["Environment"].ToString();



        public static string LocalFolder = ConfigurationManager.AppSettings["LocalFolder"]?.ToString();


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

        //public   string[] get_ICV_PIH_from_DB(string OPERATION, string neWPIH,LOGTYPE typ)
        //{

        //    DataSet ds = ExecuteProcedure("ICV_PIH",Basepage.conStr, new SqlParameter[]{

        //                                                           new SqlParameter("OPERATION", OPERATION), 
        //                                                           new SqlParameter("PIH_NEW", neWPIH) 
        //                                                           });


        //    DataTable dt = ds.Tables[0];
        //    string errorMsg = dt.Rows[0]["errorMsg"].ToString();
        //    string errorNo = dt.Rows[0]["errorNo"].ToString(); 

        //    log("DB ICV_PIH Status:: " + errorMsg + " errorNo:" + errorNo+" Oepration:"+OPERATION+" New PIH:"+neWPIH,1,typ);

        //    if (errorMsg.Equals("SUCCESS"))
        //    {
        //          dt = ds.Tables[1];
        //        string ICV = dt.Rows[0]["ICV"].ToString();
        //        string PIH = dt.Rows[0]["PIH"].ToString();
        //        return new string[] { ICV, PIH };
        //    }
        //    else throw new InvalidCastException(ERR.custerr_ICV_PIH_ERR.ToString());
        //}

        public void SendAlertEmail(MailType type, string msg, DOCTYPE doctyp,string IRN)
        {
            try
            {


                if (Basepage.EnableAlertEmail.ToLower().Equals("true"))
                {
                   
                    string subject = "ZATCA:" + type.ToString();
                    string body = msg;


                    string msg1 = ("Sending mail : " + subject);

                    string[] arr = toEmails.Split(new char[] { ';' }).ToArray();

                    string sentEmails = "";

                    foreach (string to in arr)
                    {
                        string[] arr1 = to.Split(new char[] { ' ' }).ToArray();
                        string prefix = arr1[0];  //B2B ,B2C, or ALL
                        string email = arr1[1];

                        //if prefix is B2B, sending emails related to B2B only to that email user. and so on.
                        //atleast one ALL email must be present, else technical failures will not be alerted.
                        //MailType=SystemError,Notification will be sent to all.

                        if (prefix.Equals("ALL") || prefix.ToString().ToUpper().Equals(doctyp.ToString()) || type == MailType.SystemError || type == MailType.Notification)
                        {
                            //send email
                            if(!sentEmails.Contains(email))   //dont send email again in case already sent.
                            {  

                            //Create the msg object to be sent
                            MailMessage mailMsg = new MailMessage();
                            //Add your email address to the recipients
                            mailMsg.To.Add(email);
                            //Configure the address we are sending the mail from
                            MailAddress address = new MailAddress(fromEmail);
                            mailMsg.From = address;
                            mailMsg.Subject = subject;
                            mailMsg.Body = body;
                                
                                mailMsg.IsBodyHtml = true;
                                if (type == MailType.ReportingError) mailMsg.IsBodyHtml = false;
                                if (type == MailType.ClearanceError) mailMsg.IsBodyHtml = false;

                                    log("Sending mail...", 1, LOGTYPE.MAIN);
                                log("from:"+ fromEmail, 1, LOGTYPE.MAIN);
                                log("to:"+ email, 1, LOGTYPE.MAIN);
                                log("subject:" + subject, 1, LOGTYPE.MAIN);
                                log("body:" + body, 1, LOGTYPE.MAIN); 

                                if(IRN !=null && !IRN.Equals(""))
                                { 
                                    log("IRN/XML File Name:" + IRN, 1, LOGTYPE.MAIN);
                                    body = body + "<br/>" + IRN;
                                }
                                // mailMsg.IsBodyHtml = true;
                                //SmtpClient smtp = new SmtpClient(MailHost, 587);
                                // smtp.EnableSsl = true;
                                // smtp.UseDefaultCredentials = false;
                                // smtp.Credentials = new System.Net.NetworkCredential(fromEmail, EmailPassword);
                                // smtp.Send(mailMsg);

                                //Configure an SmtpClient to send the mail.            
                                SmtpClient client = new SmtpClient();
                                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                                client.EnableSsl = true;
                                client.Host = MailHost;
                                client.Port = MailPort;
                                client.Timeout = 50000;


                                //Setup credentials to login to our sender email address ("UserName", "Password")
                                client.UseDefaultCredentials = false;
                                NetworkCredential credentials = new NetworkCredential(fromEmail, EmailPassword);
                                client.Credentials = credentials;


                                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, 
                                    System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };
                           
                                if(body !=null)
                            client.Send(mailMsg);

                            sentEmails = sentEmails + ";" + email;
                                log("sentEmails:"+ sentEmails, 1, LOGTYPE.MAIN);
                            }
                        }

                    }

                }

                
            }
            catch (Exception ex)
            {
                string msg1=("Sending mail error: " + ex.ToString());
                log(msg1, 1,LOGTYPE.MAIN);
                // SendMail(ex.ToString());

            }

        }
        public static XmlNamespaceManager get_nsmanager(XmlDocument xmlDocument)
        {

            var nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("d", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");

            nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            nsmgr.AddNamespace("xades", "http://uri.etsi.org/01903/v1.3.2#");
            nsmgr.AddNamespace("sac", "urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2");
            nsmgr.AddNamespace("sig", "urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2");
            nsmgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            return nsmgr;
        }
        public static string getXMLNodeValue(string xpath, string xmlFilePath)
        {
            FileNameFields bd = new FileNameFields();
            XmlNameTable table = null;
            XDocument xDoc = null;

            if (xDoc == null)
            {
                 

                using (var sr = new StreamReader(xmlFilePath))
                {
                    XmlTextReader reader = new XmlTextReader(xmlFilePath);
                    xDoc = XDocument.Load(reader);
                    table = reader.NameTable;
                    reader.Close();
                    reader.Dispose();
                }
                    
              
            }


            var nsmgr = new XmlNamespaceManager(table);
            nsmgr.AddNamespace("d",invoice.NS_XMLNS);
            nsmgr.AddNamespace("cbc", invoice.NS_CBC);
            // nsmgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            //nsmgr.AddNamespace("xades", "http://uri.etsi.org/01903/v1.3.2#");
            // nsmgr.AddNamespace("sac", "urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2");
            //nsmgr.AddNamespace("sig", "urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2");
            nsmgr.AddNamespace("ext", invoice.NS_EXT);
            nsmgr.AddNamespace("cac", invoice.NS_CAC);

            var nodd = xDoc.XPathSelectElement(xpath, nsmgr);

            if (nodd == null) throw new InvalidCastException("Value not found for :" + xpath);
            string value= nodd.Value;

            if(xpath.Equals("d:Invoice/cbc:InvoiceTypeCode"))
                value= nodd.Attribute("name").Value+""+ nodd.Value;

            if (xpath.Equals("d:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PartyIdentification/cbc:ID"))
                value = nodd.Attribute("schemeID").Value + ";" + nodd.Value;

            if (value == null) throw new InvalidCastException("XPATH VALUE EMPTY");
            return value;

            
             
        }
        public static INV_TYPE getInvoiceTypeCode(string invType)
        {

            if (invType.Equals("383")) return INV_TYPE.DEBIT;
            else if (invType.Equals("381")) return INV_TYPE.CREDIT;
            else if (invType.Equals("386")) return INV_TYPE.PREPAYMENT;
            else return INV_TYPE.INVOICE;   //388 default

        }
        public bool Get_ICV_Update(HttpStatusCode statusCode,DOCTYPE doctyp)
        {
            bool change_icv_pih;
            
            switch (statusCode)
            {
                //ICV PIH Update required
                case System.Net.HttpStatusCode.OK: { change_icv_pih = true; break; }   //200
                case System.Net.HttpStatusCode.Accepted: { change_icv_pih = true; break; }  //202
                
                //Conditional
                case System.Net.HttpStatusCode.BadRequest: {   change_icv_pih = doctyp == DOCTYPE.B2C?  true : false; break; }  //400
                case System.Net.HttpStatusCode.SeeOther: {     change_icv_pih = doctyp == DOCTYPE.B2C ? true : false; break; } //303
                case System.Net.HttpStatusCode.Unauthorized: { change_icv_pih = doctyp == DOCTYPE.B2C ? true : false; break; }  //401

                //ICV PIH Update NOT required
                case System.Net.HttpStatusCode.RequestEntityTooLarge: { change_icv_pih = false; break; }  //413
                case System.Net.HttpStatusCode.InternalServerError: { change_icv_pih = false; break; }  //500
                case System.Net.HttpStatusCode.ServiceUnavailable: { change_icv_pih = false; break; }  //503
                case System.Net.HttpStatusCode.GatewayTimeout: { change_icv_pih = false; break; }  //504
                case System.Net.HttpStatusCode.NotFound: { change_icv_pih = false; break; }  //404 

                


                default: { change_icv_pih = false; break; }
            }

            return change_icv_pih;
        }


        public static CertificateData GetCertData()
        {

            string ppp = File.ReadAllText(Basepage.private_key_path);
            string privateKeytext = ppp;
            privateKeytext = privateKeytext.Replace("-----BEGIN EC PRIVATE KEY-----", "");
            privateKeytext = privateKeytext.Replace("-----END EC PRIVATE KEY-----", "");
            //remove new line
            privateKeytext = privateKeytext.Replace(System.Environment.NewLine, "");

            
            string certificate = File.ReadAllText(Basepage.cert_pem_path);

            CertificateData cdata = new CertificateData();
            cdata.RawDataString = certificate;
            cdata.RawDataStringB64 = Basepage.Base64Encode(certificate); 
            cdata.privateKeytext = privateKeytext;






            //X509Certificate2 cert = new X509Certificate2(Basepage.cert_pem_path, Basepage.private_key_path,
            //                X509KeyStorageFlags.MachineKeySet
            //              | X509KeyStorageFlags.PersistKeySet);

            //string certificate = Convert.ToBase64String(cert.RawData);

            //CertificateData cdata = new CertificateData();
            //cdata.RawDataString = certificate;
            //cdata.RawDataStringB64 = Basepage.Base64Encode(certificate);
            //cdata.Certificate_issuer_name = cert.IssuerName.ToString();
            //cdata.X509SerialNumber = cert.SerialNumber;
            //cdata.RawDataByte = cert.RawData;
            //cdata.PublicKey = cert.PublicKey.ToString();

            //string ppp = File.ReadAllText(Basepage.private_key_path);
            //cdata.privateKeytext = ppp;

            //  string key = ppp;
            // string  privateKeyString = "-----BEGIN EC PRIVATE KEY-----\n" + key.Replace("\n", "").Replace("\t", "") + "\n-----END EC PRIVATE KEY-----";










            //  java.io.Reader rdr = new java.io.InputStreamReader(new java.io.ByteArrayInputStream(Encoding.UTF8.GetBytes(privateKeyString)));
            // Object parsed = (new  org.​bouncycastle.​openssl.​PEMParser(rdr)).readObject();
            // KeyPair pair = (new JcaPEMKeyConverter()).getKeyPair((PEMKeyPair)parsed);
            // Main.Private_Key = pair.getPrivate();



            //StringWriter stringWriter = new StringWriter();
            //Org.BouncyCastle.Utilities.IO.Pem.PemWriter pemWriter = new Org.BouncyCastle.Utilities.IO.Pem.PemWriter(stringWriter);
            //pemWriter.WriteObject((Org.BouncyCastle.Utilities.IO.Pem.PemObjectGenerator)new Org.BouncyCastle.Utilities.IO.Pem.PemObject("PRIVATE KEY", File.ReadAllBytes(private_key_path))); // SEC1: EC PRIVATE KEY, X.509: PUBLIC KEY
            //string pp = (stringWriter.ToString());

            //var fileStream = System.IO.File.OpenText(private_key_path);
            //var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(fileStream);
            //var KeyParameter =  pemReader.ReadObject();

            //byte[] byteArray = Encoding.UTF8.GetBytes(privateKeyString); 
            //MemoryStream stream = new MemoryStream(byteArray); 

            //TextReader reader = File.OpenText(private_key_path); 
            //// Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters key;

            //using (reader = new StreamReader(stream))
            //{
            //   var key1 = new PemReader(reader).ReadObject();
            //}

            //string myPrivateKey = System.IO.File.ReadAllText(private_key_path); 
            //AsymmetricCipherKeyPair keyPair = null;

            //using (var sr = new StringReader(myPrivateKey))
            //{
            //    PemReader pr = new PemReader(sr);
            //    keyPair = pr.ReadObject() as AsymmetricCipherKeyPair;
            //}


            string scret_file = Basepage.cert_pem_path.Replace(Path.GetFileName(Basepage.cert_pem_path), "secret.txt");
            string secret=File.ReadAllText(scret_file);
            cdata.secret = secret;

            return cdata;

        }

        internal  SOURCE getXMLSource(string fPath, LOGTYPE typ)
        {
            //check if the store number in IRN is available in the database, set as POS if yes else external

            string irn = Basepage.getXMLNodeValue("d:Invoice/cbc:ID", fPath);

            //sample Farm : 0002223052521421030391
            string store = irn.Substring(0, 5);
            string qq = "select * from Stores where cast(StoreNo as bigint)=" + int.Parse(store);
            DataTable dt = getDataDBQuery(qq, false,typ);
            if (dt != null && dt.Rows.Count > 0)
                return SOURCE.POS;
            else return SOURCE.EXTERNAL; 


        }

        internal   DOCTYPE getXMLDocType(string fPath,LOGTYPE typ)
        {
            string InvoiceTypeCode = Basepage.getXMLNodeValue("d:Invoice/cbc:InvoiceTypeCode", fPath);
            if (InvoiceTypeCode.StartsWith("01"))
                return DOCTYPE.B2B;
            else return DOCTYPE.B2C;         //  InvoiceTypeCode.StartsWith("02")
            
        }

        public void log(string msg, int ex, LOGTYPE lg)
        {
            if (logDebug == 1)
            {
                logWrite(msg, lg);
            }
            else
            {
                if (ex == 1)
                {
                    logWrite(msg, lg);
                }
            }
        }

        public void logWrite(string msg, LOGTYPE lg)
        {
            string format = "ddMMyyyy";

            string fff =( lg == LOGTYPE.COUNTER|| lg == LOGTYPE.DB_UPDATE_ERROR) ? "" : DateTime.Now.ToString(format);  //ICV ,PIH logs,DB_UPDATE_ERROR  should be wrtitten in one file for all dates.
             string filePath = LogFilePath + lg.ToString() + fff + ".log";
            string version = "1.29";
            if (!File.Exists(filePath))
            {
                StreamWriter log;
                log = new StreamWriter(filePath);
                //var version = Assembly.GetExecutingAssembly().GetName().Version;

                log.WriteLine("[" + version.ToString() + "] " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff tt") + "==> " + msg);
                log.Close();
            }
            else
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter StreamWriters = new StreamWriter(fs))
                    {
                        StreamWriters.WriteLine("[" + version.ToString() + "] " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff tt") + "==> " + msg);
                        StreamWriters.Close();
                    }
                }


            }
        }







        public DataSet ExecuteProcedure(string procedureName, string Constr, params SqlParameter[] parameter)
        {
            return Execute(CommandType.StoredProcedure, Constr, procedureName, parameter);
        }
        private DataSet Execute(CommandType type, string constr, string procedureName, SqlParameter[] parameter)
        {
            DataSet dt = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(procedureName, constr);

            da.SelectCommand.CommandType = type;
            da.SelectCommand.CommandTimeout = 300000;
            foreach (SqlParameter p in parameter)
                da.SelectCommand.Parameters.Add(p);
            da.Fill(dt);
            da.Dispose();
            return dt;

        }
        public System.Data.DataTable getDataDBQuery(string query, bool write_query_to_log, LOGTYPE lg)
        {
            if (write_query_to_log == true)
                logWrite("Query:::" + query, lg);


            System.Data.DataTable dt = new System.Data.DataTable();

            using (SqlConnection conn = new SqlConnection(conStr))
            {

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.CommandTimeout = 50;
                da.Fill(dt);
                conn.Close();
            }
            return dt;
             
        }
        public System.Data.DataTable getDataDBQuery(string query, bool write_query_to_log,string constr, LOGTYPE lg)
        {
            if (write_query_to_log == true)
                logWrite("Query:::" + query, lg);

            if (constr == null || constr.Trim(). Equals(""))
                log("database connection string missing", 1, lg);


            System.Data.DataTable dt = new System.Data.DataTable();

            using (SqlConnection conn = new SqlConnection(constr))
            {

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.CommandTimeout = 50;
                da.Fill(dt);
                conn.Close();
            }
            return dt;

        }
        public System.Data.DataTable ExcecuteReader(string query)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            {

                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = query;
                    var dataReader = cmd.ExecuteReader();
                    var dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    conn.Close();
                }
                return dt;
            }


        }




        public int getDataDBQuery_Update(string query)
        {
            {
                SqlConnection conn = new SqlConnection(Basepage.conStr);
                conn.Open();
                SqlCommand sc = new SqlCommand(query, conn);
                int n = sc.ExecuteNonQuery();
                conn.Close();
                return n;
            }


        }

        internal static string getReported_Directory(string xmlFilename,string parentFolder)
        {
            string trxdate = xmlFilename.Split(new char[] { '_' })[1];   //getting date from 300465395500003_20230813T195200_0002023081319521080176to 
            trxdate = trxdate.Substring(0, 4) + "-" + trxdate.Substring(4, 2) + "-" + trxdate.Substring(6, 2);
            parentFolder = parentFolder + trxdate;
            if (!Directory.Exists(parentFolder)) Directory.CreateDirectory(parentFolder);

            return parentFolder + @"\";
        }
    }

     

}