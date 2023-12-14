using ExtensionMethod;
using GatewayService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Renci.SshNet;
using RestSharp;
using SDKNETFrameWorkLib.BLL;
using SDKNETFrameWorkLib.GeneralLogic;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZATCA
{
    class JobZatca
    {
        System.Timers.Timer Timer_rename_rpted;


        System.Timers.Timer Timer_Reporting;
        System.Timers.Timer Timer_Reporting7;
        System.Timers.Timer Timer_Reporting8;
        System.Timers.Timer Timer_Reporting9;
        public static Boolean ENAMBLE_MULTIPLE_REPORTING_TIMERS = false;

        System.Timers.Timer Timer_Retry_Reporting;
        System.Timers.Timer Timer_Sigining;
        System.Timers.Timer Check_if_no_file_from_stores;
        System.Timers.Timer Timer_B2C_FTP;
        System.Timers.Timer Timer_ClearLogFiles;
        System.Timers.Timer Timer_BackUp_delete_DB_DATA;

        HashingValidator _IHashingValidator = new HashingValidator();
        QRValidator _IQRValidator = new QRValidator();
        EInvoiceValidator _IEInvoiceValidator = new EInvoiceValidator();


        int timeInterval = (Basepage.JobZatca_reporting_timer_inSec);
        int timeIntervalRetry_Reporting = (Basepage.JobZatca_RETRY_reporting_minutes);
        int timeInterval_B2Cftp = (Basepage.JobZatca_B2C_FTP_inSec); 

        LOGTYPE TYPP = LOGTYPE.JOB;
        Basepage bp = new Basepage();
        public void JobStart()
        {

            try
            {
                Thread.Sleep(10000);
                bp.log("JobZatca started", 1, TYPP);

                Thread t = new Thread(new ThreadStart(this.InitializeTimer));
                t.Start();
            }
            catch (Exception ex)
            {
                bp.log("JobZatca::" + ex.ToString(), 1, TYPP);

                //Send email
                bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");

            }
        }

        protected void InitializeTimer()
        {
            try
            {
                if (Basepage.is_localServer.Equals("NO"))
                { //reporting only from HO server
                    if (Timer_rename_rpted == null)
                    {
                        Timer_rename_rpted = new System.Timers.Timer();
                        Timer_rename_rpted.Enabled = true;
                        Timer_rename_rpted.AutoReset = true;
                        Timer_rename_rpted.Interval = timeInterval * 1000;
                        Timer_rename_rpted.Enabled = true;
                        Timer_rename_rpted.Start();
                        Timer_rename_rpted.Elapsed += timer_rename_rpted_Elapsed;

                    }
                    if (Timer_Reporting == null)
                    {
                        Timer_Reporting = new System.Timers.Timer();
                        Timer_Reporting.Enabled = true;
                        Timer_Reporting.AutoReset = true;
                        Timer_Reporting.Interval = timeInterval * 1000;
                        Timer_Reporting.Enabled = true;
                        Timer_Reporting.Start();
                        Timer_Reporting.Elapsed += timer_reporting_Elapsed;

                    }
                    if (Timer_Retry_Reporting == null)
                    {
                        Timer_Retry_Reporting = new System.Timers.Timer();
                        Timer_Retry_Reporting.Enabled = true;
                        Timer_Retry_Reporting.AutoReset = true;
                        Timer_Retry_Reporting.Interval = timeIntervalRetry_Reporting * 60 * 1000;
                        Timer_Retry_Reporting.Enabled = true;
                        Timer_Retry_Reporting.Start();
                        Timer_Retry_Reporting.Elapsed += timer_retry_reporting_Elapsed;

                    }
                    if (ENAMBLE_MULTIPLE_REPORTING_TIMERS == true)
                    {
                        if (Timer_Reporting7 == null)
                        {
                            Timer_Reporting7 = new System.Timers.Timer();
                            Timer_Reporting7.Enabled = true;
                            Timer_Reporting7.AutoReset = true;
                            Timer_Reporting7.Interval = (timeInterval + 3) * 1000;
                            Timer_Reporting7.Enabled = true;
                            Timer_Reporting7.Start();
                            Timer_Reporting7.Elapsed += timer_reporting7_Elapsed;

                        }
                        if (Timer_Reporting8 == null)
                        {
                            Timer_Reporting8 = new System.Timers.Timer();
                            Timer_Reporting8.Enabled = true;
                            Timer_Reporting8.AutoReset = true;
                            Timer_Reporting8.Interval = (timeInterval + 5) * 1000;
                            Timer_Reporting8.Enabled = true;
                            Timer_Reporting8.Start();
                            Timer_Reporting8.Elapsed += timer_reporting8_Elapsed;

                        }
                        if (Timer_Reporting9 == null)
                        {
                            Timer_Reporting9 = new System.Timers.Timer();
                            Timer_Reporting9.Enabled = true;
                            Timer_Reporting9.AutoReset = true;
                            Timer_Reporting9.Interval = (timeInterval + 7) * 1000;
                            Timer_Reporting9.Enabled = true;
                            Timer_Reporting9.Start();
                            Timer_Reporting9.Elapsed += timer_reporting9_Elapsed;

                        }
                    }

                    if (Timer_Sigining == null)
                    {

                        Timer_Sigining = new System.Timers.Timer();
                        Timer_Sigining.Enabled = true;
                        Timer_Sigining.AutoReset = true;
                        Timer_Sigining.Interval = 10 * 1000;
                        Timer_Sigining.Enabled = true;
                        Timer_Sigining.Start();
                        Timer_Sigining.Elapsed += timer_signing_Elapsed;
                    }
                    if (Check_if_no_file_from_stores == null)
                    {

                        Check_if_no_file_from_stores = new System.Timers.Timer();
                        Check_if_no_file_from_stores.Enabled = true;
                        Check_if_no_file_from_stores.AutoReset = true;
                        Check_if_no_file_from_stores.Interval = 120 * 60 * 1000;  //runs every 120 minutes
                        Check_if_no_file_from_stores.Enabled = true;
                        Check_if_no_file_from_stores.Start();
                        Check_if_no_file_from_stores.Elapsed += timer_check_files_from_Stores_Elapsed;
                    }

                    if (Timer_ClearLogFiles == null)
                    {
                        Timer_ClearLogFiles = new System.Timers.Timer();
                        Timer_ClearLogFiles.Enabled = true;
                        Timer_ClearLogFiles.AutoReset = true;
                        Timer_ClearLogFiles.Interval = 8 * 60 * 60 * 1000;  //runs every 8 hours
                        Timer_ClearLogFiles.Enabled = true;
                        Timer_ClearLogFiles.Start();
                        Timer_ClearLogFiles.Elapsed += timer_clearLogs_Elapsed;

                    }
                    //if (Timer_BackUp_delete_DB_DATA == null)
                    //{
                    //    Timer_BackUp_delete_DB_DATA = new System.Timers.Timer();
                    //    Timer_BackUp_delete_DB_DATA.Enabled = true;
                    //    Timer_BackUp_delete_DB_DATA.AutoReset = true;
                    //    Timer_BackUp_delete_DB_DATA.Interval =24 * 60 * 60 * 1000;  //runs every day
                    //    Timer_BackUp_delete_DB_DATA.Enabled = true;
                    //    Timer_BackUp_delete_DB_DATA.Start();
                    //    Timer_BackUp_delete_DB_DATA.Elapsed += timer_BackUp_delete_DB_DATA_Elapsed;

                    //}
                }
                if (Basepage.is_localServer.Equals("NO") == false)
                { //doing FTP only from store server.
                    if (Timer_B2C_FTP == null)
                    {
                        Timer_B2C_FTP = new System.Timers.Timer();
                        Timer_B2C_FTP.Enabled = true;
                        Timer_B2C_FTP.AutoReset = true;
                        Timer_B2C_FTP.Interval = timeInterval_B2Cftp * 1000;
                        Timer_B2C_FTP.Enabled = true;
                        Timer_B2C_FTP.Start();
                        Timer_B2C_FTP.Elapsed += timer_B2C_FTP_Elapsed;
                    }
                    if (Timer_ClearLogFiles == null)
                    {
                        Timer_ClearLogFiles = new System.Timers.Timer();
                        Timer_ClearLogFiles.Enabled = true;
                        Timer_ClearLogFiles.AutoReset = true;
                        Timer_ClearLogFiles.Interval = 8 * 60 * 60 * 1000;  //runs every 8 hours
                        Timer_ClearLogFiles.Enabled = true;
                        Timer_ClearLogFiles.Start();
                        Timer_ClearLogFiles.Elapsed += timer_clearLogs_Elapsed;

                    }
                }



            }
            catch (Exception ex)
            {
                bp.log("JobZatca InitializeTimer::" + ex.ToString(), 1, TYPP);

                //Send email
                bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");
            }
            finally
            {
            }
        }
        protected void timer_BackUp_delete_DB_DATA_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            LOGTYPE ttt = LOGTYPE.DB_BACKUP;
            Timer_BackUp_delete_DB_DATA.Stop();
            try
            {  

                //clearing DB Errors, actions table eg:3 months old.

                //to be written.

            }
            catch (Exception ex)
            {
                bp.log("JobZatca timer_BackUp_delete_DB_DATA_Elapsed::" + ex.ToString(), 1, ttt);

                //Send email
                bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");

            }

            Timer_BackUp_delete_DB_DATA.Start();
        }

        protected void timer_clearLogs_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            LOGTYPE ttt = LOGTYPE.TXTREN;
            Timer_ClearLogFiles.Stop();
            try
            {
                //rename txt files for failed rename files
                FOLDER[] folders = Basepage.is_localServer.Equals("NO") ?
                    new FOLDER[] { FOLDER.External_B2C_Signed,FOLDER.POS_B2C_Signed }
                    : new FOLDER[] {  FOLDER.POS_B2C_Signed };   //External_B2C_Signed folder is not available for store servers.

                for (int i = 0; i < folders.Length; i++)
                {
                    string singedXMLPath = folders[i].GetEnumDescription();

                    bp.log("path::" + Basepage.invoiceXML_folder + singedXMLPath, 1, ttt);
                    DirectoryInfo myDir = new DirectoryInfo(Basepage.invoiceXML_folder + singedXMLPath);
                    string[] filestxt = Directory.GetFiles(Basepage.invoiceXML_folder + singedXMLPath, "*.txt");

                    foreach (string filetxt in filestxt)
                    {
                        FileInfo fi = new FileInfo(filetxt);
                        if (fi.LastAccessTime < DateTime.Now.AddMinutes(30))
                        {
                            bp.log("renaming txt to xml ::" + fi.FullName, 1, ttt);
                            File.Move(fi.FullName, Path.ChangeExtension(fi.FullName, ".xml"));
                            bp.log("renaming txt to xml Success for file::" + fi.FullName, 1, ttt);
                        }

                    }

                }




                ttt = LOGTYPE.MAIN;


                //clearing HO server and store serv er log files

                string[] files = Directory.GetFiles(Basepage.LogFilePath);

                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastAccessTime < DateTime.Now.AddDays(-Basepage.clearlogfiledays))
                    {
                        bp.log("deleting ::" + fi.FullName, 1, ttt);
                        fi.Delete();
                    }

                }

            }
            catch (Exception ex)
            {
                bp.log("JobZatca Timer_ClearLogFiles::" + ex.ToString(), 1, ttt);

                //Send email
                bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");

            }

            Timer_ClearLogFiles.Start();
        }

        protected void timer_check_files_from_Stores_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            LOGTYPE ttt = LOGTYPE.JOB_STR_FILES;
            Check_if_no_file_from_stores.Stop();
            try
            {
                //Check if files are getting recieved from all stores regularly, if not send email.

                string qq = @" 
			select A.storeno as Store,A.Last_Transaction_Recieved  as [Last Transaction Recieved]
			from 
			( 
			select storeno, max(TransactionDate) as Last_Transaction_Recieved
			from Invoices
			group by StoreNo 
			) A   inner join Stores S on S.StoreNo=A.StoreNo
			  where 
			  --checking if the no xmls recived in last 30 minutes during working hours, if no working hours set, checking between 10 am to 6 PM
			  (A.Last_Transaction_Recieved < DATEADD(minute,-" + Basepage.JobZatca_check_files_from_Stores_minutes + @",getdate())  
			  and cast(getdate() as time ) >cast(isnull(S.OpenHoursStart,'10:00:00') as time )
			  and cast(getdate() as time ) <cast(isnull(S.OpenHoursEnd,'17:59:59') as time )
			  )
			  order by a.Last_Transaction_Recieved";

                DataTable dt = bp.getDataDBQuery(qq, true, ttt);

                if (dt.Rows.Count > 0)
                {

                    bp.log("rowcount:"+dt.Rows.Count,1,ttt);

                    //send email
                    string htmlString = getHtml(dt, ttt);

                    if (htmlString.Equals("") == false)
                    {
                        bp.SendAlertEmail(MailType.NoStoreData, htmlString, DOCTYPE.B2C, "");
                    }
                    


                }



            }
            catch (Exception ex)
            {
                bp.log("timer_check_files_from_Stores_Elapsed::" + ex.ToString(), 1, ttt);

                //Send email
                bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");

            }

            Check_if_no_file_from_stores.Start();
        }
        public string getHtml(DataTable dt, LOGTYPE ttt)
        {
            try
            {
                string messageBody = "<font>ZATCA-Critical issue<br><br>XMLs are not sent to ZATCA for a while from below stores: </font><br><br>";

                string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                string htmlTableEnd = "</table>";
                string htmlHeaderRowStart = "<tr style =\"background-color:#6FA1D2; color:#ffffff;\">";
                string htmlHeaderRowEnd = "</tr>";
                string htmlTrStart = "<tr style =\"color:#555555;\">";
                string htmlTrEnd = "</tr>";
                string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdEnd = "</td>";

                messageBody += htmlTableStart;
                messageBody += htmlHeaderRowStart;
                messageBody += htmlTdStart + "Store " + htmlTdEnd;
                messageBody += htmlTdStart + "Last Transaction Recieved Date " + htmlTdEnd;
                messageBody += htmlHeaderRowEnd;

                foreach (DataRow Row in dt.Rows)
                {
                    messageBody = messageBody + htmlTrStart;
                    messageBody = messageBody + htmlTdStart + Row["Store"] + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + Row["Last Transaction Recieved"] + htmlTdEnd;
                    messageBody = messageBody + htmlTrEnd;
                }
                messageBody = messageBody + htmlTableEnd;


                return messageBody;
            }
            catch (Exception ex)
            {
                bp.log("getHtml Error::" + ex.ToString(), 1, ttt);
                bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");
                return "";
            }
        }
        protected void timer_B2C_FTP_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            LOGTYPE ttt = LOGTYPE.JOB_B2C_FTP;
            Timer_B2C_FTP.Stop();
            try
            {
                //start process to send files from store windows server(pc) to HO server-14 stores with old controllers
                // startProcess();
                FTP_Process(ttt);


            }
            catch (Exception ex)
            {
                bp.log("JobZatca timer_B2C_FTP_Elapsed::" + ex.ToString(), 1, ttt);

                //Send email
                bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");

            }

            Timer_B2C_FTP.Start();
        }

        private void FTP_Process(LOGTYPE TYP1)
        {
            // testFTP();

            FOLDER[] folders = new FOLDER[] {
            FOLDER.POS_B2C_Signed };

            for (int i = 0; i < folders.Length; i++)
            {
                string singedXMLPath = folders[i].GetEnumDescription();
                string path = Basepage.invoiceXML_folder + singedXMLPath;
                bp.log("path::" + path, 0, TYP1);

                List<string> listStores = new List<string>();
                DirectoryInfo myDir = new DirectoryInfo(path);
                var Files = myDir.GetFiles("*.xml");
                if (Files.Count() > 0)
                {
                    bp.log("singedXMLPath::" + singedXMLPath, 1, TYP1);
                    bp.log("XmlFileCount::" + Files.Count(), 1, TYP1);

                    foreach (FileInfo fil in Files)
                    {

                        string filepath = myDir.ToString() + fil.ToString();
                        bp.log("Sending file via SFTP ::" + filepath, 1, TYP1);

                        //Sending FTP--------------------------------- 


                        string fname = Path.GetFileNameWithoutExtension(filepath);
                        string fname_temp = fname + ".tmp";


                        //sending temp file via FTP..................................................................................
                        string ip = Basepage.xml_upload_ftp_ip_port.Split(new char[] { ':' })[0];
                        int port = int.Parse(Basepage.xml_upload_ftp_ip_port.Split(new char[] { ':' })[1].Trim());


                        bp.log("ip: " + ip + " port: " + port + "  username:" + Basepage.xml_upload_ftp_username + " password:" + Basepage.xml_upload_ftp_password, 1, TYP1);
                        bp.log("fromFilePath: " + filepath, 1, TYP1);
                        bp.log("toFilePath: " + Basepage.xml_upload_ftp__destination_fpath, 1, TYP1);


                        string pathFTP = "ftp://" + Basepage.xml_upload_ftp_ip_port + "//" + Basepage.xml_upload_ftp__destination_fpath.Trim() + fname_temp;
                        bp.log("  pathFTP::" + pathFTP, 1, LOGTYPE.JOB_B2C_FTP);


                        FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(pathFTP);   //example :ftp://192.168.2.90:123//LYL/
                        uploadRequest.KeepAlive = false;
                        uploadRequest.Credentials = new NetworkCredential(Basepage.xml_upload_ftp_username, Basepage.xml_upload_ftp_password);
                        uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;

                        string upload_status = "";

                        using (Stream fileStream = File.OpenRead(filepath)) //@"C:\temp\store\LYL.BAT
                        using (Stream ftpStream = uploadRequest.GetRequestStream())
                        {
                            fileStream.CopyTo(ftpStream);
                        }
                        using (FtpWebResponse RESP = (FtpWebResponse)uploadRequest.GetResponse())
                        {
                            upload_status = RESP.StatusDescription;

                        }
                        bp.log("FTPUpload stattus: " + upload_status, 1, TYP1);




                        //renaming temp file back to csv from sftp location..................................................................................
                        bp.log("renaming temp file back to xml: " + fname_temp, 1, TYP1);

                        FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(pathFTP);   //example :ftp://192.168.2.90//CV/
                        ftpRequest.KeepAlive = false;
                        ftpRequest.Credentials = new NetworkCredential(Basepage.xml_upload_ftp_username, Basepage.xml_upload_ftp_password);
                        ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                        ftpRequest.RenameTo = Basepage.xml_upload_ftp__destination_fpath.Trim() + Path.GetFileName(filepath);
                        using (FtpWebResponse RESP = (FtpWebResponse)ftpRequest.GetResponse())
                        {
                            string rename_Status = RESP.StatusDescription;
                            bp.log("rename_Status: " + rename_Status.Trim(), 1, TYP1);

                        }


                        bp.log("moving file to REPORTED folder >>" + fname, 1, TYP1);

                        try
                        {
                            FileInfo fi = new FileInfo(filepath);
                          string reportedPath=  invoice.Get_Folder(DOCTYPE.B2C, SOURCE.POS, XMLSTATUS.REPORTED, fname) + Path.GetFileName(filepath);
                            bp.log("FTP backup path: "+ reportedPath, 1, TYP1);

                            fi.MoveTo(reportedPath);
                            bp.log("file move finished " , 1, TYP1);
                        }
                        catch (Exception ex)
                        {
                            bp.log("FTP_Process file move errr::" + ex.ToString(), 1, TYP1);
                            if (ex.Message.Contains("Cannot create a file when that file already exists"))
                            {
                                //rename the file extension
                                //otherwise the file will keep on trying to get moved from the source folder.
                                string s = DateTime.Now.ToString("mmss");
                                File.Move(path, Path.ChangeExtension(path, "." + s));

                                bp.log("File saved after renaming the extension:" + Path.ChangeExtension(path, "." + s), 1, TYP1);
                            }
                            bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");
                        }

                       

                        //deleting th efile after FTP..................................................................................
                        //bp.log("Deleting the file..", 1, TYP1);
                        //File.Delete(filepath);
                        

                        bp.log("ftp finished for the file.: " + filepath + "......................................................", 1, TYP1);



                    }

                }


            }

        }
        public void testFTP()
        {
            try
            {
                //ftp://10.217.109.10//300465395500003_20230609T100500_0000123060910051040049.tmp
                string filepath = @"C:\ZATCA\files\invoiceXML\POS\B2C\SIGNED\300465395500003_20230609T125100_0000123060912511040113.xml";
                string fname_temp = "300465395500003_20230609T125100_0000123060912511040113.tmp";
                string pathFTP = "ftp://" + Basepage.xml_upload_ftp_ip_port + "//" + Basepage.xml_upload_ftp__destination_fpath.Trim() + fname_temp;
                bp.log("  pathFTP::" + pathFTP, 1, LOGTYPE.JOB_B2C_FTP);

                FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(pathFTP);   //example :ftp://192.168.2.90:123//LYL/
                uploadRequest.Credentials = new NetworkCredential(Basepage.xml_upload_ftp_username, Basepage.xml_upload_ftp_password);
                uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;

                string upload_status = "";

                using (Stream fileStream = File.OpenRead(filepath)) //@"C:\temp\store\LYL.BAT
                using (Stream ftpStream = uploadRequest.GetRequestStream())
                {
                    fileStream.CopyTo(ftpStream);
                }
                using (FtpWebResponse RESP = (FtpWebResponse)uploadRequest.GetResponse())
                {
                    upload_status = RESP.StatusDescription;
                    bp.log("upload_status testFTP::" + upload_status, 1, LOGTYPE.JOB_B2C_FTP);

                }
            }
            catch (Exception ex)
            {
                bp.log("JobZatca testFTP::" + ex.ToString(), 1, LOGTYPE.JOB_B2C_FTP);
            }

        }

        protected void timer_reporting7_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            LOGTYPE TYP2 = LOGTYPE.JOB_REP;
            Timer_Reporting7.Stop();
            try
            {


                //start process to send files to POS
                // startProcess();

                bp.log("Reporting7_Process started", 1, TYP2);
                Reporting_Process(TYP2, "7.xml",false);    //files ending with 7.xml


            }
            catch (Exception ex)
            {
                bp.log("JobZatca timer_reporting7_Elapsed::" + ex.ToString(), 1, TYP2);
                //Send email
                bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");
            }

            Timer_Reporting7.Start();
        }
        protected void timer_reporting8_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            LOGTYPE TYP2 = LOGTYPE.JOB_REP;
            Timer_Reporting8.Stop();
            try
            {


                //start process to send files to POS
                // startProcess();

                bp.log("Reporting8_Process started", 1, TYP2);
                Reporting_Process(TYP2, "8.xml",false);    //files ending with 8.xml


            }
            catch (Exception ex)
            {
                bp.log("JobZatca timer_reporting8_Elapsed::" + ex.ToString(), 1, TYP2);
                //Send email
                bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");
            }

            Timer_Reporting8.Start();
        }
        protected void timer_reporting9_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            LOGTYPE TYP2 = LOGTYPE.JOB_REP;
            Timer_Reporting9.Stop();
            try
            {


                //start process to send files to POS
                // startProcess();

                bp.log("Reporting9_Process started", 1, TYP2);
                Reporting_Process(TYP2, "9.xml",false);    //files ending with 9.xml


            }
            catch (Exception ex)
            {
                bp.log("JobZatca timer_reporting9_Elapsed::" + ex.ToString(), 1, TYP2);
                //Send email
                bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");
            }

            Timer_Reporting9.Start();
        }
        protected void timer_reporting_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            LOGTYPE TYP2 = LOGTYPE.JOB_REP;
            Timer_Reporting.Stop();
            try
            {



                //start process to send files to POS
                // startProcess();

                bp.log("Reporting_Process started", 1, TYP2);
                Reporting_Process(TYP2, "",false);


            }
            catch (Exception ex)
            {
                bp.log("JobZatca timer_reporting_Elapsed::" + ex.ToString(), 1, TYP2);
                //Send email
                bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");
            }

            Timer_Reporting.Start();
        }

        protected void timer_retry_reporting_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            LOGTYPE TYP2 = LOGTYPE.JOB_RETRY_REP;
            Timer_Retry_Reporting.Stop();
            try
            {

                 

                bp.log("retry_reporting started", 1, TYP2);
                Reporting_Process(TYP2, "",true);


            }
            catch (Exception ex)
            {
                bp.log("JobZatca timer_retry_reporting_Elapsed::" + ex.ToString(), 1, TYP2);
                //Send email
                bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");
            }

            Timer_Retry_Reporting.Start();
        }

        protected void timer_rename_rpted_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            LOGTYPE ttt = LOGTYPE.RPTED_REN;
            Timer_rename_rpted.Stop();
            try
            {

                //move .RPTED files  from SIGNED folder to REPORTED folder, this is done in a seperate process because moving the file 
                //from one folder to other takes time. and thus reporting gets lagged.

                FOLDER[] folders = new FOLDER[] {
                  FOLDER.External_B2C_Signed,FOLDER.POS_B2C_Signed,FOLDER.POS_B2C_Retry,FOLDER.External_B2C_Retry };

                for (int i = 0; i < folders.Length; i++)
                {
                    string singedXMLPath = folders[i].GetEnumDescription();

                    bp.log("path::" + Basepage.invoiceXML_folder + singedXMLPath, 1, ttt);
                    DirectoryInfo myDir = new DirectoryInfo(Basepage.invoiceXML_folder + singedXMLPath);
                    string[] filesReported = Directory.GetFiles(Basepage.invoiceXML_folder + singedXMLPath, "*.RPTED");

                    SOURCE src = SOURCE.POS;
                    if (singedXMLPath.Contains("External")) src = SOURCE.EXTERNAL;

                   
                    
                    foreach (string filerep in filesReported)
                    {
                        FileInfo fi = new FileInfo(filerep);
                        if (fi.LastAccessTime < DateTime.Now.AddSeconds(-10))
                        {
                            string folderDate = invoice.Get_Folder(DOCTYPE.B2C, src, XMLSTATUS.REPORTED, fi.Name);

                            string path = folderDate + fi.Name.Replace(".RPTED", ".xml");
                            bp.log("moving .RPTED to  ::" + path, 1, ttt);

                            try
                            {
                                fi.MoveTo(path);
                                bp.log("file move success " + fi.Name.Replace(".RPTED", ".xml"), 1, ttt);
                            }
                            catch (Exception ex)
                            {
                                bp.log("JobZatca file move err::" + ex.ToString(), 1, ttt);
                                if (ex.Message.Contains("Cannot create a file when that file already exists"))
                                {
                                    //rename the file extension
                                    //otherwise the file will keep on trying to get moved from the source folder.
                                    string s = DateTime.Now.ToString("mmss");
                                    File.Move(path, Path.ChangeExtension(path, "." + s));

                                    bp.log("File saved after renaming the extension:" + Path.ChangeExtension(path, "." + s), 1, ttt);
                                } 
                                bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");
                            }
                           
                        }

                    }

                }




            }
            catch (Exception ex)
            {
                bp.log("JobZatca timer_rename_rpted_Elapsed::" + ex.ToString(), 1, ttt);

                //Send email
                bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");

            }


            Timer_rename_rpted.Start();
        }

        protected void timer_signing_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            //for B2B/B2C files that are uploaded from portal
            LOGTYPE TYP3 = LOGTYPE.JOB_SIGNING;
            Timer_Sigining.Stop();
            try
            {
                //start process to send files to POS
                // startProcess();
                bp.log("Signing_Process started", 1, TYP3);
                Signing_Process(TYP3);


            }
            catch (Exception ex)
            {
                bp.log("JobZatca timer_signing_Elapsed::" + ex.ToString(), 1, TYP3);

                //Send email
                bp.SendAlertEmail(MailType.SystemError, ex.ToString().Replace("Rigesh", "systname"), DOCTYPE.B2C, "");

            }

            Timer_Sigining.Start();
        }

        private void Signing_Process(LOGTYPE TYP3)
        {
            //getting all files(b2b and b2c) uploaded from portal
            //finding the doctype and source and put it in the correct unsigned folder for POS or external
            //and then send for reporting or clearance
            //then move the uploaded file to backup folder, not to get it processed again and again.

            FOLDER[] folders = new FOLDER[] { FOLDER.Portal_Uploaded };

            for (int i = 0; i < folders.Length; i++)
            {

                string unsingedXMLPath = folders[i].GetEnumDescription();
                //  DOCTYPE doctyp = unsingedXMLPath.Contains("B2B") ? DOCTYPE.B2B : DOCTYPE.B2C;
                //  SOURCE source1 = unsingedXMLPath.ToLower().Contains("external") ? SOURCE.EXTERNAL : SOURCE.POS;

                bp.log("path::" + Basepage.invoiceXML_folder + unsingedXMLPath, 0, TYP3);

                DirectoryInfo myDir = new DirectoryInfo(Basepage.invoiceXML_folder + unsingedXMLPath);
                var Files = myDir.GetFiles("*.xml");
                if (Files.Count() > 0)
                {


                    bp.log("FilePath::" + unsingedXMLPath, 1, TYP3);
                    bp.log("XmlFileCount::" + Files.Count(), 1, TYP3);

                    foreach (FileInfo fil in Files)
                    {
                        try
                        {
                            string fPath = myDir.ToString() + fil.ToString();
                            bp.log("Processing " + fPath, 1, TYP3);

                            string backupFile = Basepage.invoiceXML_folder + FOLDER.Portal_Backup.GetEnumDescription() + fil.ToString();
                            bp.log("File copying to backup folder..", 1, TYP3);
                            File.Copy(fPath, backupFile, true);


                            DOCTYPE doctyp = bp.getXMLDocType(fPath, TYP3);
                            bp.log("doctyp::" + doctyp, 1, TYP3);

                            SOURCE source1 = bp.getXMLSource(fPath, TYP3);
                            bp.log("source::" + source1, 1, TYP3);

                            //Check if the invoice is already reported/cleared successfully, if yes, do not do it again.



                            bp.log("Finding correct folder...", 1, TYP3);
                            string righFolder = invoice.Get_Folder(doctyp, source1, XMLSTATUS.UNSIGNED,"");
                            bp.log("righFolder::" + righFolder, 1, TYP3);

                            string destFile = righFolder + fil.ToString();
                            bp.log("File Moving..", 1, TYP3);
                            bp.log("From::" + fPath, 1, TYP3);
                            bp.log("To::" + destFile, 1, TYP3);

                            if (File.Exists(destFile)) File.Delete(destFile);
                            File.Move(fPath, destFile);



                            DataReturn RET = new invoice().Process_POS(destFile, doctyp, TYP3, source1);

                            bp.log("Signing RESPMSG::" + RET.RESPMSG, 1, TYP3);
                            bp.log("Signing RC::" + RET.RC, 1, TYP3);
                            bp.log("Signing change_icv_pih::" + RET.change_icv_pih, 1, TYP3);

                        }
                        catch (Exception ex)
                        {
                            bp.log("Signing_Process err::" + ex.ToString(), 1, TYP3);
                        }
                    }

                }
                else
                {
                    // bp.log("Nof file found", 0, TYP3);
                }



            }

        }

        //private void Signing_Process_old(LOGTYPE TYP3)
        //{

        //    FOLDER[] folders = new FOLDER[] {
        //        FOLDER.External_B2C_Unsigned,
        //        FOLDER.POS_B2C_Unsigned,
        //        FOLDER.External_B2B_Unsigned,
        //        FOLDER.POS_B2B_Unsigned };

        //    for (int i = 0; i < folders.Length; i++)
        //    {
        //        string unsingedXMLPath = folders[i].GetEnumDescription();
        //        DOCTYPE doctyp = unsingedXMLPath.Contains("B2B") ? DOCTYPE.B2B : DOCTYPE.B2C;    
        //        SOURCE source1= unsingedXMLPath.ToLower().Contains("external") ? SOURCE.EXTERNAL : SOURCE.POS;

        //        bp.log("path::" + Basepage.invoiceXML_folder + unsingedXMLPath, 1, TYP3);

        //        DirectoryInfo myDir = new DirectoryInfo(Basepage.invoiceXML_folder + unsingedXMLPath);
        //        var Files = myDir.GetFiles("*.xml");
        //        if (Files.Count() > 0)
        //        {
        //            bp.log("unsingedXMLPath::" + unsingedXMLPath, 1, TYP3);
        //            bp.log("XmlFileCount::" + Files.Count(), 1, TYP3);

        //            foreach (FileInfo fil in Files)
        //            {
        //                bp.log("doctyp::" + doctyp, 1, TYP3);
        //                string fPath = myDir.ToString() + fil.ToString();
        //                bp.log("Signing ::" + fPath, 1, TYP3);

        //                DataReturn RET = new invoice().Process_POS(fPath, doctyp, TYP3, source1);

        //                bp.log("Signing RESPMSG::" + RET.RESPMSG, 1, TYP3);
        //                bp.log("Signing RC::" + RET.RC, 1, TYP3);
        //                bp.log("Signing change_icv_pih::" + RET.change_icv_pih, 1, TYP3);



        //                bp.log("Deleting File ::" + fPath, 1, TYP3);
        //                File.Delete(fPath);




        //            }

        //        }
        //        else bp.log("Nof file found", 0, TYP3);


        //    }

        //}


        private void Reporting_Process(LOGTYPE TYP2, string ext,bool isRetry)
        {

            FOLDER[] folders = new FOLDER[] {
            FOLDER.External_B2C_Signed,FOLDER.POS_B2C_Signed };
           
            if(isRetry==true)
                folders = new FOLDER[] {
            FOLDER.External_B2C_Retry,FOLDER.POS_B2C_Retry };

            for (int i = 0; i < folders.Length; i++)
            {
                try
                {

                    string singedXMLPath = folders[i].GetEnumDescription();

                    bp.log("path::" + Basepage.invoiceXML_folder + singedXMLPath + "   ext: " + ext, 1, TYP2);
                    DirectoryInfo myDir = new DirectoryInfo(Basepage.invoiceXML_folder + singedXMLPath);

                    string[] Files = null;
                    if (ext.Equals(""))

                    {
                        if (ENAMBLE_MULTIPLE_REPORTING_TIMERS == true)
                        {
                            Files = Directory.GetFiles(Basepage.invoiceXML_folder + singedXMLPath).Where(name => name.EndsWith(".xml")
                                                                                                       && !name.EndsWith("7.xml")
                                                                                                       && !name.EndsWith("8.xml")
                                                                                                       && !name.EndsWith("9.xml")).ToArray();
                        }
                        else
                        {
                            Files = Directory.GetFiles(Basepage.invoiceXML_folder + singedXMLPath).Where(name => name.EndsWith(".xml")).ToArray();
                        }

                    }

                    else
                        Files = Directory.GetFiles(Basepage.invoiceXML_folder + singedXMLPath).Where(name => name.EndsWith(ext)).ToArray();

                    if (Files.Count() > 0)
                    {
                        bp.log("singedXMLPath::" + singedXMLPath + "   ext: " + ext, 1, TYP2);
                        bp.log("XmlFileCount::" + Files.Count() + "   ext: " + ext, 1, TYP2);

                        int countt = Files.Count() < Basepage.JobZatca_reporting_timer_FileCount ? Files.Count() : Basepage.JobZatca_reporting_timer_FileCount;

                        for (int k = 0; k < countt; k++)
                        {
                            try
                            {
                                ProceessAsync(new FileInfo(Files[k]), myDir, folders[i], TYP2, ext);
                            }
                            catch (Exception ex)
                            {
                                bp.log("ProceessAsync error::" + ex.ToString() + "   ext: " + ext, 1, TYP2); 
                                bp.SendAlertEmail(MailType.ReportingError, ex.ToString(), DOCTYPE.B2C, new FileInfo(Files[k]).Name);
                            }
                           
                        }

                        bp.log("Processed " + countt + " files." + "   ext: " + ext, 1, TYP2);
                    }
                    else
                    {
                        bp.log("Nof file found" + "   ext: " + ext, 0, TYP2);
                    }
                }
                catch (Exception ex)
                {
                    bp.log("Reporting ext_Process error::" + ex.ToString() + "   ext: " + ext, 1, TYP2);
                    bp.SendAlertEmail(MailType.ReportingError, ex.ToString(), DOCTYPE.B2C, "");
                }

            }


        }


        private void ProceessAsync(FileInfo fil, DirectoryInfo myDir, FOLDER foldr, LOGTYPE TYP2, string ext)
        {
            //await Task.Run(() =>
            //{
            //string fPath = myDir.ToString() + fil.ToString();
            string fPath = fil.ToString();
            bp.log("Reporting ::" + fPath + "   ext: " + ext, 1, TYP2);

            DataReturn RET = Send_to_Reporting(fPath, TYP2);

            bp.log("Reporting RESPMSG for " + fil.Name + "::" + RET.RESPMSG + " RC::" + RET.RC + "   ext: " + ext, 1, TYP2);
            //bp.log("Reporting change_icv_pih in case of error::" + RET.change_icv_pih, 1, TYP2);

            

            //Moving file to reported folder.

            FOLDER reportedFolder;
            SOURCE src = SOURCE.POS;
            if (foldr == FOLDER.External_B2C_Signed || foldr==FOLDER.External_B2C_Retry)  //file for reporting can come from these folders.
            {
                reportedFolder = FOLDER.External_B2C_Reported;
                if (RET.RESPMSG != "SUCCESS")
                {
                    //files moved to failed folder are resent with new icv, pih and uuid.
                    //files moved to retry folder are resent without any change.
                    if (RET.change_icv_pih == true) reportedFolder = FOLDER.External_B2C_Failed;
                    else reportedFolder = FOLDER.External_B2C_Retry;


                    //Send email
                    string msg = "Reporting ::" + fPath + "\r\n" +RET.RESPMSG+ "\r\n" + FormatJson(RET.FullResponse);
                    bp.SendAlertEmail(MailType.ReportingError, msg, RET.DocumentType, RET.InvReferenceNumber);
                }


                src = SOURCE.EXTERNAL;
            }
            else if (foldr == FOLDER.POS_B2C_Signed || foldr == FOLDER.POS_B2C_Retry)  //file for reporting can come from these folders.
            {
                reportedFolder = FOLDER.POS_B2C_Reported;
                if (RET.RESPMSG != "SUCCESS")
                {
                    //files moved to failed folder are resent with new icv, pih and uuid.
                    //files moved to retry folder are resent without any change.
                    if (RET.change_icv_pih == true) reportedFolder = FOLDER.POS_B2C_Failed;
                    else reportedFolder = FOLDER.POS_B2C_Retry;

                    //Send email
                    string msg = "Reporting ::" + fPath + "\r\n" + RET.RESPMSG + "\r\n" + FormatJson(RET.FullResponse);
                    bp.SendAlertEmail(MailType.ReportingError, msg, RET.DocumentType, RET.InvReferenceNumber);
                }

            }
            else throw new InvalidCastException("invalid folder");

           
            string fname = Path.GetFileName(fPath);
            try
            {
                string IssueDate = Basepage.getXMLNodeValue("d:Invoice/cbc:IssueDate", fPath);
                string IssueTime = Basepage.getXMLNodeValue("d:Invoice/cbc:IssueTime", fPath);
                //put in try catch, otherwise throwing error like UNIQUE IRN  and then proessing same file again and again, since file moving to failed folder doesnt happen.
                new invoice().DB_UPDAte(RET, IssueDate, IssueTime, RET.InvReferenceNumber, RET.UUID, INVOICE_OPERATION.INSERT_INVOICE, src, TYP2, fname, DOCTYPE.B2C, 1, PROCESS_TYPE.SYSTEM);
            }
            catch (Exception ex)
            {
                bp.log("DB_UPDAte errr: " + ex.ToString(), 1, TYP2);
                bp.SendAlertEmail(MailType.DBUpdateError, ex.ToString().Replace("Rigesh", "systname"), RET.DocumentType, RET.InvReferenceNumber);
            }

          
            //  MoveFile(destFile, fil, TYP2);

            //fil.MoveTo(destFile);
            // bp.log("Moving file Successful " + fil.Name + "", 1, TYP2);

            //failed files also getting renamed as .RPTED  to avoid this , below if.
            if (reportedFolder == FOLDER.POS_B2C_Reported || reportedFolder == FOLDER.External_B2C_Reported)
            {
                //for successfully reported files instaed of moving files immediately from signed folder to Reported folder, 
                //renaming them as .RPTED, and another service is taking these files to corresponding REPORTED folders.
                //this is done, because of the reporting slowness and files getting accummulated issue.
                File.Move(fil.ToString(), fil.ToString().Replace(".xml", ".RPTED"));
                bp.log("File rename success to .RPTED for file " + fil.Name + "", 1, TYP2);
            }
            else
            {
                if (foldr != reportedFolder)  //moving to failed/retry folder.
                {
                    string ff = Basepage.invoiceXML_folder + reportedFolder.GetEnumDescription();
                    bp.log("Moving file to reporting folder " + fil.Name + "::" + ff + "   ext: " + ext, 1, TYP2);

                    string destFile = ff + Path.GetFileName(fPath);
                    if (File.Exists(destFile))
                    {
                        string s = DateTime.Now.ToString("mmss");
                        File.Move(destFile, Path.ChangeExtension(destFile, "." + s));
                    }

                    bool moved = false;
                    int count = 0;
                    while(moved==false && count<5 )
                    {
                        try
                        {
                            count = count + 1;
                            fil.MoveTo(destFile);
                            bp.log("Moving file Successful " + fil.Name + "", 1, TYP2);
                            moved = true;
                            break;
                        }
                        catch (Exception ex)
                        {
                            bp.log("Moving file errr: " + ex.ToString() + "", 1, TYP2);
                            bp.log("Sleeping for 25 seconds: ", 1, TYP2);
                            Thread.Sleep(25000);
                        }
                    }
                    
                    
                }
                else {
                    bp.log("Moving ignonred.Source folder=destination folder.src: " + foldr.GetEnumDescription() + "   dest folder=" + reportedFolder.GetEnumDescription() , 1, TYP2);
                }
                
            }

            // });


        }

        async Task MoveFile(string destFile, FileInfo fil, LOGTYPE TYP2)
        {
            // File.Move(fPath, destFile); takes time
            fil.MoveTo(destFile);
            // fil.CopyTo(destFile);
            // fil.Delete();

            bp.log("Moving file Successful " + fil.Name + "", 1, TYP2);
        }
        public static string FormatJson(string json)
        {
            if (json !=null && json.Equals("") == false)
            {
                dynamic parsedJson = JsonConvert.DeserializeObject(json);
                return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            }
            else return "";
           
        }



        internal DataReturn Send_to_Reporting(string xmlFilePath, LOGTYPE TYP)
        {
            DataReturn RET = new DataReturn();
            RET.change_icv_pih = false;
            RET.DocumentType = DOCTYPE.B2C;
            RET._action = ACTION.REPORTING;
            RET._actionStatus = ACTION_STATUS.ReportingFailed;

            bool continuee = false;
            
            string xpath = "d:Invoice/cbc:UUID";
            try
            {
                string uuid = Basepage.getXMLNodeValue(xpath, xmlFilePath);
                string irn = Basepage.getXMLNodeValue("d:Invoice/cbc:ID", xmlFilePath);

                string invType = Basepage.getXMLNodeValue("d:Invoice/cbc:InvoiceTypeCode", xmlFilePath);    //0200000388
                invType = invType.Substring(7, 3); 
                RET.InvoiceType =Basepage.getInvoiceTypeCode(invType);


                RET = new invoice().Get_Signed_Fields_XML(xmlFilePath, TYP, RET);

                bp.log("InvoiceHash::" + RET.InvoiceHash, 1, TYP);

                RET.InvReferenceNumber = irn;
                RET.UUID = uuid;
                continuee = true;
            }
            catch (Exception ex)
            {
                RET.RESPMSG = "XML parsing failed:" + ex.ToString().Replace("Rigesh", "systname");
                RET.RC = 1;
                RET.ClearedInvoice = "";
                RET.FullResponse = "";
            }

            if(continuee)
            {
                Result rrrr = _IHashingValidator.GenerateEInvoiceHashing(xmlFilePath);
                RET.InvoiceHash = rrrr.ResultedValue; 

                IRestResponse response1 = null;

                // bp.log("Reporting API:" + Basepage.Reporting_API, 0, TYP);
                 //  bp.log("InvoiceHash:" + RET.InvoiceHash, 1, TYP);

                string xmlString = File.ReadAllText(xmlFilePath, Encoding.UTF8);
                string base64 = Basepage.Base64Encode(xmlString);

                string body = "{  \"invoiceHash\": \"<invoiceHash>\",\"uuid\": \"<uuid>\",\"invoice\": \"<invoice>\"}";
                body = body.Replace("<invoiceHash>", rrrr.ResultedValue);
                body = body.Replace("<uuid>", RET.UUID);
                body = body.Replace("<invoice>", base64);

                //  bp.log("body:" + body, 0, TYP);

                string auth = Basepage.GetCertData().RawDataStringB64 + ":" + Basepage.GetCertData().secret;
                bp.log("auth:" + auth,1, TYP);
                auth = Basepage.Base64Encode(auth);

                //not using HttpClient because it is not giving response body when error code is 400 bad request
                var client = new RestClient(Basepage.Reporting_API);
                if (!Basepage.proxy.Equals(""))
                    client.Proxy = new WebProxy(Basepage.proxy);

                //client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("accept", "application/json");
                request.AddHeader("Accept-Language", "en");
                request.AddHeader("Authorization", "Basic " + auth);
                //   client1.DefaultRequestHeaders.Add("Accept-Charset", "UTF-8"); 
                request.AddHeader("Accept-Version", "V2");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                response1 = client.Execute(request);

                bp.log("ResponseCode:" + response1.StatusCode, 1, TYP);
                // bp.log("ResponseBody:" + response1.Content, 1, TYP);

                RET.FullResponse = response1.Content;
                RET.HTTPResponseCode = response1.StatusCode.ToString();



                bool change_icv_pih = new Basepage().Get_ICV_Update(response1.StatusCode, DOCTYPE.B2C);
                RET.change_icv_pih = change_icv_pih;

                if (response1.StatusCode != System.Net.HttpStatusCode.OK && response1.StatusCode != System.Net.HttpStatusCode.Accepted)  //other than 200,202
                {
                    bp.log("ResponseBody:" + response1.Content, 1, TYP);
                    RET.RESPMSG = response1.StatusCode.ToString() + "-" + response1.Content;
                    RET.RC = 1;
                    RET.ClearedInvoice = "";
                    RET._actionStatus = ACTION_STATUS.ReportingFailed;

                }

                var data = (JObject)JsonConvert.DeserializeObject(response1.Content);
                if (data != null)
                {
                   // RET.InvoiceHash = data["invoiceHash"]?.Value<string>();
                    string status = data["reportingStatus"]?.Value<string>();

                    if (status.ToUpper().Equals("REPORTED"))
                    {
                        RET.RESPMSG = "SUCCESS";
                        RET.RC = 0;
                        RET._actionStatus = ACTION_STATUS.Reported;
                    }


                    JArray errr = (JArray)data["validationResults"]["errorMessages"];
                    JArray wwww = (JArray)data["validationResults"]["warningMessages"];
                    JArray innnn = (JArray)data["validationResults"]["infoMessages"];


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
                }



            }



            return RET;





        }

   
    }
}
