 



using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls; 

namespace ZatcaWebApp
{
    public partial class CustomersReport : System.Web.UI.Page
    {
        Basepage bp = new Basepage();
        public static string conStr = ConfigurationManager.ConnectionStrings["ConStr"]?.ConnectionString;
        protected string ArrayStore = "";
        public bool isClientScrpiptActive = true;

        static string OLD_DB_PASSWORD;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {


                this.Master.TextMessage.Text = "";
                if (!IsPostBack)
                {
                    if (Session["UserID"] == null)
                    {
                        Session.Clear();
                        Session.Abandon();
                        Response.Redirect("Login.aspx");
                    }
                    else
                    {
                        Session["PageId"] = "li_B2BCustomerReport"; 
                       

                    }

                     

                }
                btnExport.Visible = datagrid1.Rows.Count > 0;

            }
            catch (Exception ex)
            {
                // bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }


        }
         
         
         
        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";


            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
                 }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt= ((DataView)this.SqlDataSource1.Select(DataSourceSelectArguments.Empty)).ToTable();
                bp.ExportDirectToCSV_NEW(dt, "B2BCustomers_", this.Context.Response);
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }
        }
    }
}