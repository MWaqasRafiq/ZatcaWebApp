using ExtensionMethod;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZatcaWebApp
{
    public partial class Viewinvoices : System.Web.UI.Page
    {
        Basepage bp = new Basepage();
        //public static string CONFIG=""
        public bool isClientScrpiptActive = true;
        public static DataTable DT1_STATIC;
        public static DataTable DT1_INFO;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                this.Master.Page_heading = "View Invoices";

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
                        Session["PageId"] = "li_viewInvoices";


                    }

                    LoadDDL();



                    DT1_STATIC = null;
                    btnExport.Visible = false;
                    LoadVATDDL();
                }




            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }


        }
        public void LoadVATDDL( )
        {
            listVAT.Items.Clear();

            

            DataTable dt = new Basepage().getDataDBQuery("select * from VATIDs where status=1");

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ListItem item = new ListItem(dt.Rows[i]["VATNo"].ToString(), dt.Rows[i]["ConnectionString"].ToString());
                    listVAT.Items.Add(item);
                }
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
                DT1_STATIC?.Clear();
                DT1_INFO?.Clear();

                string CURRENT_DATE = Convert.ToDateTime(DateTime.Now.ToString()).ToString("yyyy-MM-dd");

                //string _fromdate = txtFrom.Text != "" ? DateTime.ParseExact(txtFrom.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : CURRENT_DATE;
                //  string _todate = txtTo.Text != "" ? DateTime.ParseExact(txtTo.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : CURRENT_DATE;

                string _fromdate = txtFrom.Text != "" ? DateTime.ParseExact(txtFrom.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd 00:00:00") : "";
                string _todate = txtTo.Text != "" ? DateTime.ParseExact(txtTo.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd 23:59:59") : "";

                string valuesSelected = string.Join(",", StoreList.Items.OfType<ListItem>().Where(i => i.Selected).Select(i => i.Text).ToArray());


                string[] vatSelectedValues = listVAT.Items.OfType<ListItem>().Where(i => i.Selected).Select(i => i.Value).ToArray();

                if (vatSelectedValues == null || vatSelectedValues.Length<=0)
                    vatSelectedValues = listVAT.Items.OfType<ListItem>().Select(i => i.Value).ToArray();



                for (int i = 0; i < vatSelectedValues.Length; i++)
                {
                    string constr = vatSelectedValues[i];
                    // string qq = "update dbo.Invoices   set TimeleftToReport=( SELECT cast( FORMAT(CAST((SELECT DATEDIFF(SECOND, GETDATE(), TransactionDate) AS DateDiff)/86400.000 AS datetime), 'HH:mm:ss') as time(7)))  where DocType = 'B2C' and ActionStatus != 'Reported' ";
                    // bp.getDataDBQuery_Update(qq, constr); 


                    DataSet ds = bp.ExecuteProcedure("SP_INVOICE", constr, new SqlParameter[] {
                                new SqlParameter("@OPERATION", INVOICE_OPERATION.SEARCH_INVOICES.ToString()),
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
                                new SqlParameter("@Source",  ddlSource.SelectedValue.Trim()),
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

                    DataTable dt = ds.Tables[2];
                    string errorMsg = dt.Rows[0]["errorMsg"].ToString();
                    string errorNo = dt.Rows[0]["errorNo"].ToString();

                    if (errorMsg.Equals("SUCCESS"))
                    {
                        if (DT1_STATIC == null) DT1_STATIC = ds.Tables[0];
                        else DT1_STATIC.Merge(ds.Tables[0]);

                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            if (DT1_INFO == null) DT1_INFO = ds.Tables[1];
                            else DT1_INFO.Merge(ds.Tables[1]);
                        }
                    }
                    else this.Master.ShowMessage(errorMsg + "  constr:" + constr, Alert.warning, isClientScrpiptActive);
                }



                GridView1.DataBind();

                if (DT1_STATIC.Rows.Count <= 0)
                    this.Master.ShowMessage("No data found", Alert.warning, isClientScrpiptActive);

                if (DT1_STATIC.Rows.Count > 0)
                {
                    table_info.Visible = true;
                   
                    sp_Total.InnerText = DT1_INFO.AsEnumerable().Sum(row => row.Field<int>("Total")).ToString();   
                    sp_Cleared.InnerText = DT1_INFO.AsEnumerable().Sum(row => row.Field<int>("Cleared")).ToString(); 
                    sp_Reported.InnerText = DT1_INFO.AsEnumerable().Sum(row => row.Field<int>("Reported")).ToString();
                    sp_ClearedWithWarning.InnerText = DT1_INFO.AsEnumerable().Sum(row => row.Field<int>("ClearedWithWarning")).ToString();
                    sp_ReportedWithWarning.InnerText = DT1_INFO.AsEnumerable().Sum(row => row.Field<int>("ReportedWithWarning")).ToString();
                    sp_ClearanceFailed.InnerText = DT1_INFO.AsEnumerable().Sum(row => row.Field<int>("ClearanceFailed")).ToString();
                    sp_ReportingFailed.InnerText = DT1_INFO.AsEnumerable().Sum(row => row.Field<int>("ReportingFailed")).ToString();
                    sp_TotalFailed.InnerText = DT1_INFO.AsEnumerable().Sum(row => row.Field<int>("TotalFailed")).ToString();  
                     
                }
                else table_info.Visible = false;



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
            try
            {
                bp.ExportDirectToCSV_NEW(DT1_STATIC, "invoices_", this.Context.Response);
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
                    string xmfilename = e.CommandArgument.ToString();
                    Session["ViewXML_filename"] = xmfilename;

                    Response.Redirect("searchInvoice.aspx?ViewXML=true");
                }
                else if (e.CommandName.Equals("report"))
                {
                    string[] arr = e.CommandArgument.ToString().Split(new char[] { ',' });
                    string xmfilename = arr[0];
                    string doctype = arr[1];
                    string Source = arr[2];
                    string ActionStatus = arr[3];


                    Enum.TryParse(doctype, out DOCTYPE _doctype);
                    Enum.TryParse(Source, out SOURCE _source);
                    Enum.TryParse(ActionStatus, out ACTION_STATUS _ActionStatus);

                    XMLSTATUS _xmlstatus = bp.getXML_STATUS(_doctype, _ActionStatus);
                    string folder = Basepage.Get_Folder(_doctype, _source, _xmlstatus, xmfilename, "");   //eg: C:\IDOL\ZATCA\files\invoiceXML\EXTERNAL\B2C\FAILED

                    string fpath = folder + xmfilename;
                    if (File.Exists(fpath))
                    {
                        string destFile = Basepage.invoiceXML_folder;
                        if (_source == SOURCE.EXTERNAL)
                        {
                            destFile += FOLDER.External_B2C_Signed.GetEnumDescription() + xmfilename;
                        }
                        else if (_source == SOURCE.POS)
                        {
                            destFile += FOLDER.POS_B2C_Signed.GetEnumDescription() + xmfilename;
                        }

                        if (File.Exists(destFile))
                        {
                            throw new InvalidCastException("File already being processed.Please wait.. ");
                        }
                        else
                        {
                            File.Move(fpath, destFile);

                            bp.logWrite("File rereporting:file moved from " + fpath + " to " + destFile, LOGTYPE.LG);
                            this.Master.ShowMessage("File is getting processed. Please check the status after some time.", Alert.error, isClientScrpiptActive);
                        }
                    }
                    else {
                        throw new InvalidCastException("File not found:"+fpath);
                    }
                }
                else if (e.CommandName.Equals("lnk_viewErrors") || e.CommandName.Equals("lnk_viewWarnings"))
                {
                    string type = "";
                    if (e.CommandName.Equals("lnk_viewErrors"))
                        type = "ERROR";
                    else if (e.CommandName.Equals("lnk_viewWarnings"))
                        type = "WARNING";
                    string fkInvoices = e.CommandArgument.ToString();
                    string qq = "select IRN, Type,Message,CreatedDate from Errors where FkInvocies=" + fkInvoices + " and IsLatest=1 and Type in('" + type + "','INFO')";
                    DataTable dt = bp.getDataDBQuery(qq);
                    gverr.DataSource = dt;
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


    }
}