using GatewayService;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ZATCA
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {


                ////Timer jobs create ---------------------
                Thread t = new Thread(new ThreadStart(this.InitializeTimer));
                t.Start();
                ////-------------------------------


                //Web service ----------------------

                string apiurl = ConfigurationManager.AppSettings["APIURL"].ToString();

                //StartOptions options = new StartOptions();
                //options.Urls.Add("http://localhost:7777");
                //options.Urls.Add("http://127.0.0.1:7777");
                //options.Urls.Add(string.Format("http://{0}:7777", Environment.MachineName));

                //var server = WebApp.Start<Startup>(options);
                var server = WebApp.Start<Startup>(url: apiurl);
                //-------------------------------

                // new Basepage().log(1,"------------- Service Started. --------------");


                string serv = Basepage.is_localServer.Equals("NO") ? "central Cloud Server" : "Store Server:" + Basepage.is_localServer;
                string msg = "Notification:ZATCA Windows service started on " + serv;
                new Basepage().SendAlertEmail(MailType.Notification, msg, DOCTYPE.B2B, "");

            }
            catch (Exception ex)
            {
                // EventLog.WriteEntry(ex.Message);
                new Basepage().log(  "Service Error OnStart:" + ex.ToString(),1,LOGTYPE.MAIN);
                // base.OnStop();
                // throw;'
                 
                string server = Basepage.is_localServer.Equals("NO") ? "central Cloud Server" : "Store Server:" + Basepage.is_localServer;
                string msg = "Critical Error:Error on starting ZATCA Windows service on " + server;
                new Basepage().SendAlertEmail(MailType.SystemError, msg, DOCTYPE.B2B, "");
            }
        }

        protected void InitializeTimer()
        {
            try
            {
                JobZatca jid = new JobZatca();
                Thread jthread2 = new Thread(jid.JobStart);
                jthread2.Start();
                new Basepage().log("InitializeTimer JobZatca...", 1, LOGTYPE.JOB);
                
            }
            catch (Exception ex)
            {
                new Basepage().log("Error InitializeTimer:" + ex.Message, 1, LOGTYPE.JOB); 
            }

        }
        //        private void test()
        //        {

        //            string ret = "";
        //            HttpResponseMessage response1 = null;

        //            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        //            // Client Certificate Handler
        //            WebRequestHandler handler = new WebRequestHandler();

        //            using (var client1 = new HttpClient(handler))
        //            {
        //                client1.BaseAddress = new Uri("https://posintegration.everseen.com/toshiba/001/0034/transaction/start");

        //                string tt = "{ " +
        //"    \"a\":\"b\"" +
        //"}";

        //                response1 = client1.PostAsJsonAsync("https://posintegration.everseen.com/toshiba/001/0034/transaction/start", tt).Result;
        //                string resp = response1.Content.ReadAsStringAsync().Result;


        //                new Basepage().log("resp:" + resp, 1, LOGTYPE.MAIN);


        //            }



        //        }



        public class Startup
        {
            // This code configures Web API. The Startup class is specified as a type
            // parameter in the WebApp.Start method.
            public void Configuration(IAppBuilder appBuilder)
            {
                try
                {
                    // Configure Web API for self-host. 
                    HttpConfiguration config = new HttpConfiguration();
                    config.Routes.MapHttpRoute(
                        name: "DefaultApi",
                        routeTemplate: "api/{controller}/{action}/{id}",  // "api/{controller}/{action}/{id}"   for GetString method in controller api/{controller}
                          defaults: new { id = RouteParameter.Optional, Action = RouteParameter.Optional }
                    );

                    appBuilder.UseWebApi(config);

                    new Basepage().log ( "------------- Service Started. --------------",1,LOGTYPE.MAIN);


                }
                catch (Exception ex)
                {
                    new Basepage().log(  "Error Startup:" + ex.ToString(),1,LOGTYPE.MAIN);
                }
            }
        }

        //protected void InitializeTimer()
        //{

        //    JobSTC ji2 = new JobSTC();
        //    Thread jthread2 = new Thread(ji2.JobStart);
        //    jthread2.Start();
        //}

        protected override void OnStop()
        {
            new Basepage().log( "------------- Service Stopped. --------------",1,LOGTYPE.MAIN);
            string server = Basepage.is_localServer.Equals( "NO" )? "central Cloud Server" : "Store Server:" + Basepage.is_localServer;
            string msg = "Critical Error:ZATCA Windows service stopped on " + server;
            new Basepage().SendAlertEmail(MailType.SystemError, msg, DOCTYPE.B2B, "");
        }

       
        public void MyServiceOnStart()
        {
            string[] c = new string[0];
            this.OnStart(c);
            while (true)
            {
                Console.ReadLine();
            }

        }
    }

    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        private const string Realm = "My Realm";
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //If the Authorization header is empty or null
            //then return Unauthorized
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request
                    .CreateResponse(HttpStatusCode.Unauthorized);
                // If the request was unauthorized, add the WWW-Authenticate header 
                // to the response which indicates that it require basic authentication
                //if (actionContext.Response.StatusCode == HttpStatusCode.Unauthorized)
                //{
                //    actionContext.Response.Headers.Add("WWW-Authenticate",
                //        string.Format("Basic realm=\"{0}\"", Realm));
                //}
            }
            else
            {
                //Get the authentication token from the request header
                string authenticationToken = actionContext.Request.Headers
                    .Authorization.Parameter;
                //Decode the string
                string decodedAuthenticationToken = Encoding.UTF8.GetString(
                    Convert.FromBase64String(authenticationToken));
                //Convert the string into an string array
                string[] usernamePasswordArray = decodedAuthenticationToken.Split(':');
                //First element of the array is the username
                string username = usernamePasswordArray[0];
                //Second element of the array is the password
                string password = usernamePasswordArray[1];
                //call the login method to check the username and password
                if (UserValidate.Login(username, password))
                {
                    var identity = new GenericIdentity(username);
                    IPrincipal principal = new GenericPrincipal(identity, null);
                    Thread.CurrentPrincipal = principal;
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.User = principal;
                    }
                }
                else
                {
                    actionContext.Response = actionContext.Request
                        .CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
        }
    }

    public class UserValidate
    {
        //This method is used to check the   credentials
        public static bool Login(string username, string password)
        {
            UsersBL userBL = new UsersBL();
            var UserLists = userBL.GetUsers();
            return UserLists.Any(user =>
                user.UserName.Equals(username, StringComparison.OrdinalIgnoreCase)
                && user.Password == password);
        }
    }
    public class UsersBL
    {
        public List<User> GetUsers()
        {
            // In Real-time you need to get the data from any persistent storage
            // For Simplicity of this demo and to keep focus on Basic Authentication
            // Here we are hardcoded the data
            List<User> userList = new List<User>();
            userList.Add(new User()              //b2b& b2c
            {
                ID = 101,
                UserName = "eInvoiceGen",
                Password = "Zatca@f@rM#"
            });

            userList.Add(new User()              //b2b& b2c
            {
                ID = 101,
                UserName = "eInvoiceGen",
                Password = "Zatca@Vgn#"
            });
            userList.Add(new User()              //b2b& b2c
            {
                ID = 101,
                UserName = "eInvoiceGen",
                Password = "Zatca@star#"
            });

            return userList;
        }
    }
    public class User
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}