using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;

namespace ZatcaWebApp
{
    public partial class Login : System.Web.UI.Page
    {
        public bool isClientScrpiptActive = true;
        Basepage bp = new Basepage();
        protected void Page_Load(object sender, EventArgs e)
        {

            serialize();

            
        }

        public string serialize()
        {
            string ret = "";

           // string tt = File.ReadAllText(@"D:\Desktop\_IDOL\Egypt Tax\sample.json");
           // string tt1 = JsonConvert.SerializeObject(tt, Formatting.None);


          // string rr= jObject.ToString(Formatting.None);

            return ret;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {

               // string qq = "update dbo.Invoices   set TimeleftToReport=( SELECT cast( FORMAT(CAST((SELECT DATEDIFF(SECOND, GETDATE(), TransactionDate) AS DateDiff)/86400.000 AS datetime), 'HH:mm:ss') as time(7)))  where DocType = 'B2C' and ActionStatus != 'Reported' ";

              //  bp.getDataDBQuery_Update(qq);


                this.Master.TextMessage.Text = "";
                string pw = Convert.ToBase64String(Basepage.Encryption(txtPassword.Text));



                string query = "select * from Z_Users where UserName='" + txtUserName.Text + "' and password='" + pw + "'";
                DataTable dt = bp.getDataDBQuery(query);

                if (dt.Rows.Count == 0)
                {
                    this.Master.TextMessage.Text = "Invalid user name or password !";
                }

                else
                {
                    DataRow dr = dt.Rows[0];

                    string roleName = dr[1].ToString();
                    string role = dr["Role"].ToString();
                    string userName = dr["UserName"].ToString();
                    //  string currency = dr["Currency"].ToString();
                    string userid = dr["id"].ToString();

                    Session["roleName"] = roleName;
                    Session["role"] = role;
                    Session["user"] = userName;
                    Session["userName"] = userName;
                    Session["userid"] = userid;
                    Session["storeNo"] = dr["Unit_Id"].ToString();

                    this.Master.USERID.Text = userid;

                    string url = "~/Home.aspx";

                    bp.logWrite("User login" + Session["userName"].ToString(), LOGTYPE.LG);
                    Session["PageID"] = "li_Home";

                    //bp.Load_ALL_DDLDATA();
                    Response.Redirect(url, false);
                }
               


            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }
        }


    }
}