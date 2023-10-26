
//using Microsoft.Office.Interop.Excel;

using ExtensionMethod;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;


namespace ZatcaWebApp
{

    public class FileNameFields
    {
        public string partyTaxSchemeCompanyId { get; set; }
        public string IssueDate_YYYY_MM_DD { get; set; }
        public string IssueTime_HH_MM_SS { get; set; }
        public string irn { get; set; }
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
    public enum DOCTYPE
    {
        B2B,
        B2C
    }
    public enum SOURCE
    {
        POS,
        EXTERNAL
    }
    public enum XMLSTATUS
    {

        UNSIGNED,
        SIGNED,
        FAILED,
        REPORTED

    }
    public enum LOGTYPE
    {
        LG,
        CMPL
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
    public enum INVOICE_OPERATION
    {
        INSERT_EGSconfig,
        INSERT_INVOICE,
        INSERT_STORE,
        SEARCH_INVOICES,
        GET_ERRORS_WARNINGS_SUMMARY,
        GET_ERRORS_WARNINGS_DETAIL,
        GET_ALL_ACTIONS,
        GET_BRANCH_INVOICE_REPORT,
        GET_INVOICE_COUNTER_REPORT,
        GET_MISSINGE_DUPLICATE_INVOICE_COUNTER_REPORT
    }
    public enum PROCESS_TYPE
    {
        SYSTEM,
        SYSTEM_RETRY,
        MANUAL,
        API

    }

    public class Basepage
    {

        public static int logDebug = int.Parse(ConfigurationManager.AppSettings["logDebug"]);
        public static string CSR_Path = ConfigurationManager.AppSettings["CSR_Path"];



        public static string private_key_path = ConfigurationManager.AppSettings["private_key_path"];
        public static string cert_pem_path = ConfigurationManager.AppSettings["cert_pem_path"];
        public static string invoices_for_compliance_Check_folder = ConfigurationManager.AppSettings["invoices_for_compliance_Check_folder"];

        internal static string getReported_Directory(string xmlFilename, string parentFolder)
        {
            string trxdate = xmlFilename.Split(new char[] { '_' })[1];   //getting date from 300465395500003_20230813T195200_0002023081319521080176to 
            trxdate = trxdate.Substring(0, 4) + "-" + trxdate.Substring(4, 2) + "-" + trxdate.Substring(6, 2);
            parentFolder = parentFolder + trxdate;
            if (!Directory.Exists(parentFolder)) Directory.CreateDirectory(parentFolder);

            return parentFolder + @"\";
        }
        internal string[] getdates()
        {

            string fromdate = Convert.ToDateTime(DateTime.Today.AddDays(-7).ToString()).ToString("dd/MM/yyyy");
            string todate = Convert.ToDateTime(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
            return new string[] { fromdate, todate };
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

        public static string Compliance_CSID_API = ConfigurationManager.AppSettings["Compliance_CSID_API"];
        public static string Compliance_Invoice_API = ConfigurationManager.AppSettings["Compliance_Invoice_API"];
        public static string Production_CSID_API = ConfigurationManager.AppSettings["Production_CSID_API"];
        public static string invoiceXML_folder = ConfigurationManager.AppSettings["invoiceXML_folder"];
        public static string ICV_PIH_file = ConfigurationManager.AppSettings["ICV_PIH_file"];
        public static string CustIdentifier = ConfigurationManager.AppSettings["CustIdentifier"];
        public static string resources_path = ConfigurationManager.AppSettings["resources_path"];

        public static string conStr = ConfigurationManager.ConnectionStrings["ConStr"]?.ConnectionString;
        public static string ConStrVATDB = ConfigurationManager.ConnectionStrings["ConStrVATDB"]?.ConnectionString;



        public static string Get_Folder(DOCTYPE doctype, SOURCE src, XMLSTATUS stat, string xmlFilename,string InvoiceXMLFolder)
        {


            string folderpath = InvoiceXMLFolder.Trim().Equals("")? Basepage.invoiceXML_folder: InvoiceXMLFolder;
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



        public void ExportDirectToCSV_NEW(DataTable dt, string FileName, HttpResponse Response)
        {
            FileName = FileName.ToUpper();
            StringBuilder csv = new StringBuilder(10 * dt.Rows.Count * dt.Columns.Count);



            for (int c = 0; c < dt.Columns.Count; c++)
            {
                if (c > 0)
                    csv.Append(",");
                DataColumn dc = dt.Columns[c];
                string columnTitleCleaned = CleanCSVString(dc.ColumnName);
                csv.Append(columnTitleCleaned);
            }
            csv.Append(Environment.NewLine);
            foreach (DataRow dr in dt.Rows)
            {
                StringBuilder csvRow = new StringBuilder();
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (c != 0)
                        csvRow.Append(",");

                    object columnValue = dr[c];
                    if (columnValue == null)
                        csvRow.Append("");
                    else
                    {
                        string columnStringValue = columnValue.ToString();


                        string cleanedColumnValue = CleanCSVString(columnStringValue);

                        if (columnValue.GetType() == typeof(string) && !columnStringValue.Contains(","))
                        {
                            cleanedColumnValue = "=" + cleanedColumnValue; // Prevents a number stored in a string from being shown as 8888E+24 in Excel.
                        }
                        csvRow.Append(cleanedColumnValue);
                    }
                }
                csv.AppendLine(csvRow.ToString());
            }


            // HttpResponseBase response = context.HttpContext.Response;
            Response.Clear();
            Response.Buffer = true;

            // Response.AddHeader("password", "1234");
            // Response.AddHeader("content-disposition", "attachment; filename=\"" + FileName + ".csv\"");
            Response.AddHeader("content-disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(FileName + ".csv", Encoding.UTF8) + "");
            // Response.Cookies.Add(new System.Web.HttpCookie("fileDownload", "true"));
            Response.Charset = "";
            Response.ContentType = "application/csv";
            Response.ContentEncoding = Encoding.UTF8;
            Response.BinaryWrite(Encoding.UTF8.GetPreamble());
            Response.Write(csv.ToString()); // Information which you want in .csv file


            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();  //for resdponse.end() since response.end gives exception on exel

        }


        //public static  string PG_ConnectionString= ConfigurationManager.AppSettings["PG_ConnectionString"]?.ToString();
        public Dictionary<string, string> PERIODS = new Dictionary<string, string>();

        public static string IsTestingMode = ConfigurationManager.AppSettings["IsTestingMode"]?.ToString();



        public static string LocalFolder = ConfigurationManager.AppSettings["LocalFolder"].ToString();



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
            string filePath = LocalFolder + lg.ToString() + DateTime.Now.ToString(format) + ".log";
            string version = "1.2";
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
        public int getDataDBQuery_Update(string query, string conStr)
        {
            {
                SqlConnection conn = new SqlConnection(conStr);
                conn.Open();
                SqlCommand sc = new SqlCommand(query, conn);
                int n = sc.ExecuteNonQuery();
                conn.Close();
                return n;
            }


        }
        public void OperationLog(Int64 fk_user, OPERATION_TYPE optype, string operation_description, LOGTYPE lg)
        {
            string QUERY_SQL = " INSERT INTO GENIUS3.OPERATION_LOG( FK_USER, OPERATION_TYPE, OPERATION_DESCRIPTION, CREATE_DATE) " +
                                    "VALUES   (  {FK_USER},'{OPERATION_TYPE}','{OPERATION_DESCRIPTION}',GETDATE())";


            QUERY_SQL = QUERY_SQL.Replace("{FK_USER}", fk_user.ToString());
            QUERY_SQL = QUERY_SQL.Replace("{OPERATION_TYPE}", optype.ToString());
            QUERY_SQL = QUERY_SQL.Replace("{OPERATION_DESCRIPTION}", operation_description);


            getDataDBQuery(QUERY_SQL, false, lg);
        }
        public string getNumQuery(string query)
        {
            {
                string statu = "0";
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    SqlCommand com = new SqlCommand(query, conn);
                    conn.Open();
                    using (SqlDataReader reader = com.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            statu = reader.GetValue(0).ToString();


                        }

                        reader.Close();
                    }
                    conn.Close();
                }
                return statu;
            }


        }





        protected string CleanCSVString(string input)
        {
            string output = "\"" + input.Replace("\"", "\"\"").Replace("\r\n", " ").Replace("\r", " ") + "\"";
            return output;
        }

        public System.Data.DataTable getDataDBQuery(string query)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            using (SqlConnection conn = new SqlConnection(conStr))
            {

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.CommandTimeout = 50;
                da.Fill(dt);
                da.Dispose();
                conn.Close();
            }

            //getSICs();
            return dt;

        }
        public System.Data.DataTable getDataDBQuery(string query, string constrString)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            using (SqlConnection conn = new SqlConnection(constrString))
            {

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.CommandTimeout = 50;
                da.Fill(dt);
                da.Dispose();
                conn.Close();
            }

            //getSICs();
            return dt;

        }
        public bool CheckAccessPermission(string role, string pageId)
        {

            bool res = false;
            if (role != "0")
            {
                string q = "Select Id from [Z_PagesList] where PageId='" + pageId + "'";
                DataTable dt = new Basepage().getDataDBQuery(q);
                if (dt.Rows.Count > 0)  //can not be zero.
                {
                    if (dt.Rows[0]["Id"] != null) //can not be null.
                    {
                        int r;
                        if (int.TryParse(role, out r))
                        {
                            string q1 = "	SELECT REPLACE( (select concat(r.AccessPageIds,' ', (SELECT Stuff((SELECT ', ' + Z_PagesList.ConnectedPageIds" +
                                        " FROM Z_PagesList where Z_PagesList.Id in(select item from dbo.SplitString1(r.AccessPageIds, ' '))  FOR XML PATH('')" +
                                         "), 1, 2, '')  )) as fsdf from Z_Roles r where r.Code =" + r + "),',',' ') as AccessPageIds";
                            DataTable dt1 = new Basepage().getDataDBQuery(q1);
                            if (dt.Rows.Count > 0)  //can not be zero.
                            {
                                if (dt1.Rows[0]["AccessPageIds"].ToString() != null) //can not be null.
                                {
                                    string AccessPageIds = dt1.Rows[0]["AccessPageIds"].ToString();
                                    string[] Ids = AccessPageIds.Split(' ');
                                    int currenct_page_id = int.Parse(dt.Rows[0]["Id"].ToString());  //id of the currenct page.
                                    int flag = 0;  //0 implies return false(page not accessible) 1 return true
                                    foreach (string id in Ids)
                                    {
                                        int k; int.TryParse(id, out k);
                                        if (k.Equals(currenct_page_id))  //pageId is in the accesspageIds
                                        {
                                            flag = 1;
                                        }
                                        else { res = false; }


                                    }

                                    if (flag == 0) { res = false; } else { res = true; }

                                }
                                else { res = false; }
                            }
                            else { res = false; }
                        }
                        else { res = false; }

                    }
                    else { res = false; }
                }

            }
            else
            {
                res = true; //for admin
            }
            return res;


        }





        public static byte[] Encryption(string PlainText)
        {
            string key = "far_m_pand_a_id0l_encripttionKEY";
            TripleDES des = CreateDES(key);
            ICryptoTransform ct = des.CreateEncryptor();
            byte[] input = Encoding.Unicode.GetBytes(PlainText);
            return ct.TransformFinalBlock(input, 0, input.Length);
        }

        static TripleDES CreateDES(string key)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            TripleDES des = new TripleDESCryptoServiceProvider();
            des.Key = md5.ComputeHash(Encoding.Unicode.GetBytes(key));
            des.IV = new byte[des.BlockSize / 8];
            return des;
        }
        public static string Decryption(string CypherText)
        {
            string key = "far_m_pand_a_id0l_encripttionKEY";
            byte[] b = Convert.FromBase64String(CypherText);
            TripleDES des = CreateDES(key);
            ICryptoTransform ct = des.CreateDecryptor();
            byte[] output = ct.TransformFinalBlock(b, 0, b.Length);
            return Encoding.Unicode.GetString(output);
        }
        public string RollBackTransactions(string[] queries)
        {
            {
                string ret = "";
                using (SqlConnection connection1 = new SqlConnection(conStr))
                {
                    SqlTransaction sqlTran = null;
                    try
                    {
                        connection1.Open();
                        sqlTran = connection1.BeginTransaction();
                        SqlCommand command = connection1.CreateCommand();
                        command.Transaction = sqlTran;


                        foreach (string query in queries)
                        {
                            if (query == "" || query == null)
                            {
                                break;
                            }
                            command.CommandText = query;
                            command.ExecuteScalar();
                        }


                        sqlTran.Commit();
                        ret = "success";
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            sqlTran.Rollback();
                            ret = "Error occured.Txn Rolled back. error:" + ex.Message;
                        }
                        catch (Exception exRollback)
                        {
                            ret = "error on rolling back transaction." + exRollback.ToString();

                        }
                    }
                    return ret;
                }

            }


        }




    }

    public enum OPERATION_TYPE
    {

        USER_LOGIN,
        USER_LOGOUT
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