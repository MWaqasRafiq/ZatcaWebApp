 

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
    public partial class ViewErrorsWarnings : System.Web.UI.Page
    {
        Basepage bp = new Basepage();
        //public static string CONFIG=""
        public bool isClientScrpiptActive = true;
        public static DataTable DT1_STATIC;
        public static DataTable DT2_STATIC;
        public static DataTable DT3_STATIC;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                this.Master.Page_heading = "View Errors and Warnings";

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
                        Session["PageId"] = "li_ViewErrorsWarnings";


                    }

                    LoadDDL();



                    DT1_STATIC = null;
                    DT2_STATIC = null;
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
            //loading  status_ddl----------------------------------------
            status_ddl.Items.Clear();
            status_ddl.Items.Add(new ListItem("ALL", "ALL", true));
            string[] itemNames = System.Enum.GetNames(typeof(ACTION_STATUS));

            for (int i = 0; i <= itemNames.Length - 1; i++)
            {
                ListItem item = new ListItem(itemNames[i], itemNames[i]);
                status_ddl.Items.Add(item);
            }

            //loading  store_ddl----------------------------------------
            StoreList.Items.Clear();
            DataTable dt = bp.getDataDBQuery("select   storeno from Stores where StoreNo is not null and StoreNo !='' order by storeno ");
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
            txtFrom.Text = dates[0];
            txtTo.Text = dates[1];
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
               // chk_show_errors.Checked = true;
               // chk_show_info.Checked = true;
               // chk_show_warnings.Checked = true;
                BindGrid();


            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }
        }

    
        private void BindGrid()
        {
            string CURRENT_DATE = Convert.ToDateTime(DateTime.Now.ToString()).ToString("yyyy-MM-dd");

            //string _fromdate = txtFrom.Text != "" ? DateTime.ParseExact(txtFrom.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : CURRENT_DATE;
            //  string _todate = txtTo.Text != "" ? DateTime.ParseExact(txtTo.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : CURRENT_DATE;

            string error = chk_show_errors.Checked ? "TRUE" : "FALSE";
            string info = chk_show_info.Checked ? "TRUE" : "FALSE";
            string warning = chk_show_warnings.Checked ? "TRUE" : "FALSE";

            string operation = "";
            if (DDLReportType.SelectedValue == "SUMMARY")
            {
                operation = INVOICE_OPERATION.GET_ERRORS_WARNINGS_SUMMARY.ToString();
            }
            else {
                operation = INVOICE_OPERATION.GET_ERRORS_WARNINGS_DETAIL.ToString();
            }

            string valuesSelected = string.Join(",", StoreList.Items.OfType<ListItem>().Where(i => i.Selected).Select(i => i.Text).ToArray());


            string _fromdate = txtFrom.Text != "" ? DateTime.ParseExact(txtFrom.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd 00:00:00") : "";
            string _todate = txtTo.Text != "" ? DateTime.ParseExact(txtTo.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd 23:59:59") : "";
            DataSet ds = bp.ExecuteProcedure("SP_INVOICE", Basepage.conStr, new SqlParameter[] {
                                new SqlParameter("@OPERATION", operation),
                                new SqlParameter("@IRN", txtIRN.Text.Trim()),
                                new SqlParameter("@UUID", ""),
                                new SqlParameter("@XMLFileName", ""),
                                new SqlParameter("@TaxTotal", ""),
                                new SqlParameter("@DocType",docType_ddl.SelectedValue.Trim()),
                                new SqlParameter("@Action","" ),
                                new SqlParameter("@ActionStatus",status_ddl.SelectedValue.Trim()),
                                new SqlParameter("@TransactionDate", _todate),
                                new SqlParameter("@StoreNo", valuesSelected),
                                new SqlParameter("@TerminalNo", txtTermNo.Text.Trim()),
                                new SqlParameter("@TrxNo", txtTrxNo.Text.Trim()),
                                new SqlParameter("@TimeleftToReport",""),
                                new SqlParameter("@ReportedIn", ""),
                                new SqlParameter("@Source", ""),
                                new SqlParameter("@ActionCount",""),
                                new SqlParameter("@ErrorCount",""),
                                new SqlParameter("@WarningCount", ""),
                                new SqlParameter("@Errors",error),
                                new SqlParameter("@Warnings", warning),
                                new SqlParameter("@Info", info),
                                new SqlParameter("@FullResponse", _fromdate),
                                new SqlParameter("@ProcessType", ""),
                                new SqlParameter("@HTTPResponseCode",""),
                                new SqlParameter("@CustIdentifier",Basepage.CustIdentifier),
                                new SqlParameter("@InvoiceHash", ""),
                                new SqlParameter("@QR", "")
                                });

            DataTable dt = ds.Tables[2];
            string errorMsg = dt.Rows[0]["errorMsg"].ToString();
            string errorNo = dt.Rows[0]["errorNo"].ToString();

            if (errorMsg.Equals("SUCCESS"))
            {
                if (DDLReportType.SelectedValue == "SUMMARY")
                {
                    DT1_STATIC = ds.Tables[1];
                    GridView1.DataBind();

                    DT2_STATIC = null;
                    GridView2.DataBind();
                }
                else
                {
                    DT2_STATIC = ds.Tables[1];
                    GridView2.DataBind();

                    DT1_STATIC = null;
                    GridView1.DataBind();
                }

                if (DDLReportType.SelectedValue == "SUMMARY")
                {
                    if (DT1_STATIC.Rows.Count <= 0) this.Master.ShowMessage("No data found", Alert.warning, isClientScrpiptActive);
                }
                else
                {
                    if (DT2_STATIC.Rows.Count <= 0) this.Master.ShowMessage("No data found", Alert.warning, isClientScrpiptActive);
                }
                 

               

                if ((DT1_STATIC !=null && DT1_STATIC.Rows.Count > 0) || (DT2_STATIC != null && DT2_STATIC.Rows.Count>0 ))
                {
                    table_info.Visible = true;
                    DataTable dttt = ds.Tables[0];

                    sp_TotalErrors.InnerText = dttt.Rows[0]["TotalErrors"].ToString();
                    sp_TotalWarnings.InnerText = dttt.Rows[0]["TotalWarnings"].ToString();
                    sp_CurrentErrors.InnerText = dttt.Rows[0]["CurrentErrors"].ToString();
                    sp_currentwarnings.InnerText = dttt.Rows[0]["CurrentWarnings"].ToString();
                }
                else table_info.Visible = false;
            }
            else this.Master.ShowMessage(errorMsg, Alert.warning, isClientScrpiptActive);


            if (DDLReportType.SelectedValue == "SUMMARY")
            {
                btnExport.Visible = DT1_STATIC?.Rows == null ? false : DT1_STATIC.Rows.Count > 0;
            }
            else
            {
                btnExport.Visible = DT2_STATIC?.Rows == null ? false : DT2_STATIC.Rows.Count > 0;
            }
           
          
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (DDLReportType.SelectedValue == "SUMMARY")
                    bp.ExportDirectToCSV_NEW(DT1_STATIC, "invOperations_", this.Context.Response);
                else
                    bp.ExportDirectToCSV_NEW(DT2_STATIC, "invOperations_", this.Context.Response);
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
            else if (table_required == "DT2_STATIC")
                return DT2_STATIC;
            else if (table_required == "DT3_STATIC")
                return DT3_STATIC;

            else return null;
        }
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";

                if (e.CommandName.Equals("lnk_viewClearance") || (e.CommandName.Equals("lnk_viewReporting")))
                {
                    string []arr = e.CommandArgument.ToString().Split(new string[] { "<sep>" }, StringSplitOptions.None);
                   // bp.logWrite(e.CommandArgument.ToString(), LOGTYPE.LG);
                    string message = arr[0];
                    string type = arr[1];
                    string fkInvoices = arr[2];
                    string doctype = e.CommandName.Equals("lnk_viewClearance") ? "B2B" : "B2C";
                    message = message.Replace("'", "''");
                    string qq = @"select distinct i.XMLFileName,i.IRN,i.DocType,i.Source,i.ActionStatus,i.TransactionDate,e.IsLatest from Errors e INNER JOIN Invoices i ON i.id=e.FkInvocies
                     where Message = '" + message + "' and type = '" + type + "' and i.DocType='"+doctype+"' and i.id in ("+fkInvoices+")"                      ;

                    DT3_STATIC = bp.getDataDBQuery(qq); 
                    gverr.DataBind();
                    mp_err.Show();

                }
                else if (e.CommandName.Equals("lnk_viewLatestClearance") || (e.CommandName.Equals("lnk_viewLatestReporting")))
                {
                    string[] arr = e.CommandArgument.ToString().Split(new string[] { "<sep>" }, StringSplitOptions.None);
                    // bp.logWrite(e.CommandArgument.ToString(), LOGTYPE.LG);
                    string message = arr[0];
                    string type = arr[1];
                    string fkInvoices = arr[2];
                    string doctype = e.CommandName.Equals("lnk_viewLatestClearance") ? "B2B" : "B2C";
                    message = message.Replace("'", "''");
                    string qq = @"select distinct i.XMLFileName,i.IRN,i.DocType,i.Source,i.ActionStatus,i.TransactionDate,e.IsLatest from Errors e INNER JOIN Invoices i ON i.id=e.FkInvocies and e.Islatest=1 
                     where Message = '" + message + "' and type = '" + type + "' and i.DocType='" + doctype + "' and i.id in (" + fkInvoices + ")";

                    DT3_STATIC = bp.getDataDBQuery(qq);
                    gverr.DataBind();
                    mp_err.Show();

                }


            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }
        }

        

     

        protected void chk_show_errors_CheckedChanged(object sender, EventArgs e)
        {
            try {
                this.Master.TextMessage.Text = "";
                BindGrid();
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }

        }

        protected void chk_show_warnings_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                BindGrid();
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }
        }

        protected void chk_show_info_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                BindGrid();
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }
        }

        protected void gverr_PageIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                mp_err.Show();
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }
        }

        protected void gverr_Sorted(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                mp_err.Show();
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }
        }

        protected void gverr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                if (e.CommandName.Equals("view_Details"))
                {
                    string xmfilename = e.CommandArgument.ToString();
                    Session["ViewXML_filename"] = xmfilename;

                    Response.Redirect("searchInvoice.aspx?ViewXML=true");
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