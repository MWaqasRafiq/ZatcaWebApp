using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZatcaWebApp
{
    public partial class ViewEGSConfigs : System.Web.UI.Page
    {
        Basepage bp = new Basepage();
        //public static string CONFIG=""
        public bool isClientScrpiptActive = true;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                this.Master.Page_heading = "EGS Configuration";

                this.Master.TextMessage.Text = "";
                if (!IsPostBack)
                {
                    if (Session["userName"] == null)
                    {
                        Session.Clear();
                        Session.Abandon();
                        Response.Redirect("Login.aspx");
                    }
                    else
                    {
                        Session["PageId"] = "li_ViewEGSConfig";


                    }



                }

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
                System.Data.DataTable dt=((System.Data.DataView)this.SqlDataSource1.Select(DataSourceSelectArguments.Empty)).ToTable();
                bp.ExportDirectToCSV_NEW(dt, "EGS_", this.Context.Response);
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }
        }
        protected void GridView1_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            if (e.Exception != null)
            {
               
                // I imagine by firing javascript on the page
                this.Master.TextMessage.Text = e.Exception.Message; 
                e.ExceptionHandled = true; //important not forgetting this
            }
            else
            {
                this.Master.TextMessage.Text = "";
            }

        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            
           try     
            {
                if (e.CommandName.Equals("downloadConfig"))
                { 

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