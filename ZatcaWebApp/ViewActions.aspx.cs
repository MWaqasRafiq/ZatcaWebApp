


using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZatcaWebApp
{
    public partial class ViewActions : System.Web.UI.Page
    {
        Basepage bp = new Basepage();
        //public static string CONFIG=""
        public bool isClientScrpiptActive = true;
        public static DataTable DT1_STATIC;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                this.Master.Page_heading = "View All Reporting & Clearance";

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
                        Session["PageId"] = "li_viewActions";


                    }

                    LoadDDL();



                    DT1_STATIC = null;
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
                chk_show_errors_only.Checked = false; 
                BindGrid();


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
        private void BindGrid()
        {
            string CURRENT_DATE = Convert.ToDateTime(DateTime.Now.ToString()).ToString("yyyy-MM-dd");

            //string _fromdate = txtFrom.Text != "" ? DateTime.ParseExact(txtFrom.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : CURRENT_DATE;
            //  string _todate = txtTo.Text != "" ? DateTime.ParseExact(txtTo.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : CURRENT_DATE;


            string erroronly = chk_show_errors_only.Checked ? "TRUE" : "FALSE";

            string valuesSelected = string.Join(",", StoreList.Items.OfType<ListItem>().Where(i => i.Selected).Select(i => i.Text).ToArray());

            string _fromdate = txtFrom.Text != "" ? DateTime.ParseExact(txtFrom.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd 00:00:00") : "";
            string _todate = txtTo.Text != "" ? DateTime.ParseExact(txtTo.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd 23:59:59") : "";
            DataSet ds = bp.ExecuteProcedure("SP_INVOICE", Basepage.conStr, new SqlParameter[] {
                                new SqlParameter("@OPERATION", INVOICE_OPERATION.GET_ALL_ACTIONS.ToString()),
                                new SqlParameter("@IRN", txtIRN.Text.Trim()),
                                new SqlParameter("@UUID", ""),
                                new SqlParameter("@XMLFileName", ""),
                                new SqlParameter("@TaxTotal", ""),
                                new SqlParameter("@DocType",docType_ddl.SelectedValue.Trim()),
                                new SqlParameter("@Action","" ),
                                new SqlParameter("@ActionStatus",status_ddl.SelectedValue.Trim()),
                                new SqlParameter("@TransactionDate", _todate),
                                new SqlParameter("@StoreNo",valuesSelected),
                                new SqlParameter("@TerminalNo", txtTermNo.Text.Trim()),
                                new SqlParameter("@TrxNo", txtTrxNo.Text.Trim()),
                                new SqlParameter("@TimeleftToReport",""),
                                new SqlParameter("@ReportedIn", ""),
                                new SqlParameter("@Source", ""),
                                new SqlParameter("@ActionCount",""),
                                new SqlParameter("@ErrorCount",""),
                                new SqlParameter("@WarningCount", ""),
                                new SqlParameter("@Errors",erroronly),
                                new SqlParameter("@Warnings", ""),
                                new SqlParameter("@Info", ""),
                                new SqlParameter("@FullResponse", _fromdate),
                                new SqlParameter("@ProcessType", ""),
                                new SqlParameter("@HTTPResponseCode",""),
                                new SqlParameter("@CustIdentifier",Basepage.CustIdentifier),
                                new SqlParameter("@InvoiceHash", ""),
                                new SqlParameter("@QR","")
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
                
                    chk_show_errors_only.Visible = DT1_STATIC.Rows.Count>0;
                
                 
                 
            }
            else this.Master.ShowMessage(errorMsg, Alert.warning, isClientScrpiptActive);



            btnExport.Visible = DT1_STATIC.Rows == null ? false : DT1_STATIC.Rows.Count > 0; 
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                bp.ExportDirectToCSV_NEW(DT1_STATIC, "errors_", this.Context.Response);
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
                if (e.CommandName.Equals("view_Details"))
                {
                }
                else if (e.CommandName.Equals("lnk_viewErrors") || e.CommandName.Equals("lnk_viewWarnings") || e.CommandName.Equals("lnk_viewInfo"))
                {
                    TextBox1.Visible = false;
                    string type = "";
                    if (e.CommandName.Equals("lnk_viewErrors"))
                        type = "ERROR";
                    else if (e.CommandName.Equals("lnk_viewWarnings"))
                        type = "WARNING";
                    else if (e.CommandName.Equals("lnk_viewWarnings"))
                        type = "INFO";
                    string fkInvoices = e.CommandArgument.ToString();
                    string qq = "select IRN, Type,Message,CreatedDate from Errors where FkInvocies=" + fkInvoices + " and IsLatest=1 and Type in('" + type + "','INFO')";
                    DataTable dt = bp.getDataDBQuery(qq);
                    gverr.DataSource = dt;
                    gverr.DataBind();
                    mp_err.Show();
                }
                else if (e.CommandName.Equals("lnk_viewFullResponse"))
                {
                    string id = e.CommandArgument.ToString();
                    string qq = "select FullResponse from Actions where id= " + id;

                    DataTable dt = bp.getDataDBQuery(qq);
                    
                     JsonDocument document = JsonDocument.Parse(dt.Rows[0]["FullResponse"].ToString());
                     var stream = new MemoryStream();
                     var writer = new Utf8JsonWriter(stream, new JsonWriterOptions() { Indented = true });
                    document.WriteTo(writer);
                    writer.Flush();
                    string formatted=Encoding.UTF8.GetString(stream.ToArray());



                    gverr.DataSource = null;
                    gverr.DataBind();

                    TextBox1.Text = formatted;
                    TextBox1.Visible = true;
                    mp_err.Show();

                }
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }
        }

      

    

        protected void chk_show_errors_only_CheckedChanged(object sender, EventArgs e)
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

         
    }
}