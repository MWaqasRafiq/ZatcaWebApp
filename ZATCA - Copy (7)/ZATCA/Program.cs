  
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ZATCA
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //  SonyVirgin.Decrypt("sLt1PmaeDo5LQgyMBhAMmQ==", true);




            bool install_service = true;  //true for installutil.

            //// ---- Debud mode for test  use install_service = false ------

            if (install_service == false)
            {
                Service1 MyService = new Service1();
                     MyService.MyServiceOnStart();
                //string xmlFilePath = @"C:\Users\Rigesh\Desktop\u300465395500003_20230609T090900_0000123060909091040024.xml";
                //string RawDataString = "MIIE1TCCBHqgAwIBAgITbQAACAjF5TDbBc4lyQAAAAAICDAKBggqhkjOPQQDAjBiMRUwEwYKCZImiZPyLGQBGRYFbG9jYWwxEzARBgoJkiaJk/IsZAEZFgNnb3YxFzAVBgoJkiaJk/IsZAEZFgdleHRnYXp0MRswGQYDVQQDExJQRVpFSU5WT0lDRVNDQTItQ0EwHhcNMjMwNjAxMDgwMzE0WhcNMjMwODA4MTIyNjQ2WjBVMQswCQYDVQQGEwJTQTEPMA0GA1UEChMGUkVUQUlMMRUwEwYDVQQLEwxSaXlhZCBCcmFuY2gxHjAcBgNVBAMTFUVBZmFybXRlc3RzdG9yZVNlcnYwMTBWMBAGByqGSM49AgEGBSuBBAAKA0IABCDcy6ftgkUfqYmd3iahHSKF4e+tOKVS0Wy0tPzXNSa7MXnukcuIwf8PKRcll+6m5QKe29zm/RScbvvBnJv3gf6jggMdMIIDGTAnBgkrBgEEAYI3FQoEGjAYMAoGCCsGAQUFBwMCMAoGCCsGAQUFBwMDMDwGCSsGAQQBgjcVBwQvMC0GJSsGAQQBgjcVCIGGqB2E0PsShu2dJIfO+xnTwFVmgZzYLYPlxV0CAWQCARMwgc0GCCsGAQUFBwEBBIHAMIG9MIG6BggrBgEFBQcwAoaBrWxkYXA6Ly8vQ049UEVaRUlOVk9JQ0VTQ0EyLUNBLENOPUFJQSxDTj1QdWJsaWMlMjBLZXklMjBTZXJ2aWNlcyxDTj1TZXJ2aWNlcyxDTj1Db25maWd1cmF0aW9uLERDPWV4dGdhenQsREM9Z292LERDPWxvY2FsP2NBQ2VydGlmaWNhdGU/YmFzZT9vYmplY3RDbGFzcz1jZXJ0aWZpY2F0aW9uQXV0aG9yaXR5MB0GA1UdDgQWBBR09SJa2vsx6IpZE5uisDbqxtPD6jAOBgNVHQ8BAf8EBAMCB4AwgYwGA1UdEQSBhDCBgaR/MH0xKTAnBgNVBAQMIDEtSURPTHwyLUVHU1Rlc3RTZXJ2fDMtMTIwNDU3Nzg4MR8wHQYKCZImiZPyLGQBAQwPMzAwNDY1Mzk1NTAwMDAzMQ0wCwYDVQQMDAQxMTAwMQ8wDQYDVQQaDAZSaXlhZGgxDzANBgNVBA8MBlJFVEFJTDCB4QYDVR0fBIHZMIHWMIHToIHQoIHNhoHKbGRhcDovLy9DTj1QRVpFSU5WT0lDRVNDQTItQ0EsQ049UEVaRWludm9pY2VzY2EyLENOPUNEUCxDTj1QdWJsaWMlMjBLZXklMjBTZXJ2aWNlcyxDTj1TZXJ2aWNlcyxDTj1Db25maWd1cmF0aW9uLERDPWV4dGdhenQsREM9Z292LERDPWxvY2FsP2NlcnRpZmljYXRlUmV2b2NhdGlvbkxpc3Q/YmFzZT9vYmplY3RDbGFzcz1jUkxEaXN0cmlidXRpb25Qb2ludDAfBgNVHSMEGDAWgBSHpdsCvXfNR29pAXt4LRr17sfAUTAdBgNVHSUEFjAUBggrBgEFBQcDAgYIKwYBBQUHAwMwCgYIKoZIzj0EAwIDSQAwRgIhAKFEmnMSn+Ukg/LQSZ67yMwM4DnVafld9icoTSP9rAIGAiEA0UThEYiUI6HExkjhhADqDGFCCbTo9CCfUiIMmX+S/TA=";
                //string privateKeytext = "MHQCAQEEIHto/Bui0MY1CSBxX3y8LFIlkzIReIASqktPQxL/g5GzoAcGBSuBBAAKoUQDQgAEINzLp+2CRR+piZ3eJqEdIoXh7604pVLRbLS0/Nc1Jrsxee6Ry4jB/w8pFyWX7qblAp7b3Ob9FJxu+8Gcm/eB/g==";
                //  ZATCA.signDLL.Result rrrr1 = new ZATCA.signDLL.signdll().SignDocument(xmlFilePath, RawDataString, privateKeytext);

               //   new Basepage().SendAlertEmail(GatewayService.MailType.ReportingError,"test mail ",GatewayService.DOCTYPE.B2C);


              
            }

            //// ----  Use installUtil Windows Service use install_service = true ----
            else
            {

               // new invoice().Get_Signed_Fields_XML(@"C:\IDOL\ZATCA\files\invoiceXML\POS\B2C\REPORTED\300465395500003_20230517T153000_0002223051715301030155.xml", LOGTYPE.API, new GatewayService.DataReturn());

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
            new Service1()
                };
                ServiceBase.Run(ServicesToRun);

            }

        }
    }
}

