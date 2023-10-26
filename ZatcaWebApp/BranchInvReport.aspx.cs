 

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZatcaWebApp
{
    public partial class BranchInvReport : System.Web.UI.Page
    {
        Basepage bp = new Basepage();
        //public static string CONFIG=""
        public bool isClientScrpiptActive = true;
        public static DataTable DT1_STATIC;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                this.Master.Page_heading = "Branch Invoice Report";

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
                        Session["PageId"] = "li_BranchInvReport";


                    }

                    LoadDDL();


                     
                    btnExport.Visible = false;
                }

            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }


        }

        private void LoadDDL()
        {
      

            //loading  store_ddl----------------------------------------
            StoreList.Items.Clear(); 
            DataTable dt = bp.getDataDBQuery("select   storeno from Stores where StoreNo is not null and StoreNo !='' and status=1 order by storeno ");
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string storeno = dt.Rows[i]["storeno"].ToString();
                    StoreList.Items.Add(storeno);
                }
                StoreList.DataBind();
            }

            //loading  store_default datetime----------------------------------------
            string[] dates = bp.getdates();
           // txtFrom.Text = dates[0];
            txtTo.Text = dates[1];

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string valuesSelected =  string.Join(",", StoreList.Items.OfType<ListItem>().Where(i => i.Selected).Select(i => i.Text).ToArray());
                if(valuesSelected.Equals("") )
                    valuesSelected= string.Join(",", StoreList.Items.OfType<ListItem>().Select(i => i.Text).ToArray());

                this.Master.TextMessage.Text = "";
                string CURRENT_DATE = Convert.ToDateTime(DateTime.Now.ToString()).ToString("yyyy-MM-dd");

                //string _fromdate = txtFrom.Text != "" ? DateTime.ParseExact(txtFrom.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : CURRENT_DATE;
                //  string _todate = txtTo.Text != "" ? DateTime.ParseExact(txtTo.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : CURRENT_DATE;

                string _fromdate = txtFrom.Text != "" ? DateTime.ParseExact(txtFrom.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd 00:00:00") : "";
                  string _todate = txtTo.Text != "" ? DateTime.ParseExact(txtTo.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd 23:59:59") : "";

                

                DataSet ds = bp.ExecuteProcedure("SP_INVOICE", Basepage.conStr, new SqlParameter[] {
                                new SqlParameter("@OPERATION", INVOICE_OPERATION.GET_BRANCH_INVOICE_REPORT.ToString()),
                                new SqlParameter("@IRN", ""),
                                new SqlParameter("@UUID", ""),
                                new SqlParameter("@XMLFileName", ""),
                                new SqlParameter("@TaxTotal", ""),
                                new SqlParameter("@DocType",""),
                                new SqlParameter("@Action","" ),
                                new SqlParameter("@ActionStatus",""),
                                new SqlParameter("@TransactionDate", _todate),
                                new SqlParameter("@StoreNo", valuesSelected),
                                new SqlParameter("@TerminalNo", txtTermNo.Text.Trim()),
                                new SqlParameter("@TrxNo", ""),
                                new SqlParameter("@TimeleftToReport",""),
                                new SqlParameter("@ReportedIn", ""),
                                new SqlParameter("@Source", ""),
                                new SqlParameter("@ActionCount",""),
                                new SqlParameter("@ErrorCount",""),
                                new SqlParameter("@WarningCount", ""),
                                new SqlParameter("@Errors",""),
                                new SqlParameter("@Warnings",""),
                                new SqlParameter("@Info", ""),
                                new SqlParameter("@FullResponse", _fromdate),
                                new SqlParameter("@ProcessType", ""),
                                new SqlParameter("@HTTPResponseCode",""),
                                new SqlParameter("@CustIdentifier",Basepage.CustIdentifier),
                                new SqlParameter("@InvoiceHash", ""),
                                new SqlParameter("@QR", "")
                                });

                DataTable dt = ds.Tables[1];
                string errorMsg = dt.Rows[0]["errorMsg"].ToString();
                string errorNo = dt.Rows[0]["errorNo"].ToString();

                if (errorMsg.Equals("SUCCESS"))
                {
                    DT1_STATIC = ds.Tables[0]; 
                    GridView1.DataBind();

                    if (DT1_STATIC.Rows.Count <= 0)
                        this.Master.ShowMessage("No data found", Alert.warning, isClientScrpiptActive);

                     
                }
                else this.Master.ShowMessage(errorMsg, Alert.warning, isClientScrpiptActive);



                btnExport.Visible = DT1_STATIC.Rows == null ? false : DT1_STATIC.Rows.Count > 0;

                btnExport.Visible = DT1_STATIC.Rows == null ? false : DT1_STATIC.Rows.Count > 0;
                
                
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }
        }

        public DataTable ODS_DATA(string table_required)
        {
            if (table_required == "DT1_STATIC")
                return DT1_STATIC;
            else return null;
        }

      
        protected void btnExport_Click(object sender, EventArgs e)
        {
            try {  
            bp.ExportDirectToCSV_NEW(DT1_STATIC, "BranchReport_", this.Context.Response);
        }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }
}
      
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
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

     
    }
    } 