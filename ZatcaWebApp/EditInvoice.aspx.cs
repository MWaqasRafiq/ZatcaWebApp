using ExtensionMethod;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZatcaWebApp
{
    public partial class EditInvoice : System.Web.UI.Page
    {
        Basepage bp = new Basepage(); 
        //public static string CONFIG=""
        public bool isClientScrpiptActive = true; 
        public static string _UNSIGNED_PATH;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                this.Master.Page_heading = "Edit Invoice";

                this.Master.TextMessage.Text = "";
                btnDownload.Visible = false;

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
                        Session["PageId"] = "li_Editinvoice";

                        string qq = Request.QueryString["ViewXML"];
                        if (qq !=null && qq.Equals("true"))
                        {
                            if (qq != null && qq.Equals("true"))
                            {
                                txtXMLFileName.Text = Session["ViewXML_filename"].ToString();
                                Load();
                                btnSave.Visible = false;
                            }
                          
                        }
                        
                        else txtXMLFileName.Text = "";
                    }
                     
                  
                   
                }

            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }


        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
               

                if (lblPath.Text != string.Empty)
                { 

                    Response.ContentType = ContentType;
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(lblPath.Text));
                    Response.WriteFile( (lblPath.Text));
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                Load();

            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }
        }

        private void Load()
        {
            btnDownload.Visible = false;
            td_labels.Visible = false;
            string qq = "select doctype, XMLFileName,Action,ActionStatus,Source from Invoices where XMLFileName='" +
                         txtXMLFileName.Text.Trim() + "' or IRN='" + txtXMLFileName.Text.Trim() + "'";
            DataTable dt = bp.getDataDBQuery(qq);

            if (dt != null && dt.Rows.Count > 0)
            {
                string doctype = dt.Rows[0]["doctype"].ToString();
                string Action = dt.Rows[0]["Action"].ToString();
                string ActionStatus = dt.Rows[0]["ActionStatus"].ToString();
                string Source = dt.Rows[0]["Source"].ToString();
                string XMLFileName = dt.Rows[0]["XMLFileName"].ToString();

                Enum.TryParse(doctype, out DOCTYPE _doctype);
                Enum.TryParse(Source, out SOURCE _source);
                Enum.TryParse(ActionStatus, out ACTION_STATUS _ActionStatus);
                XMLSTATUS _xmlstatus = bp.getXML_STATUS(_doctype, _ActionStatus);

                //check all fodlers of eg: EXTERNAL\B2B
               // string[] itemNames = System.Enum.GetNames(typeof(XMLSTATUS));
               // for (int i = 0; i <= itemNames.Length - 1; i++)
                {
                   // Enum.TryParse(itemNames[i], out XMLSTATUS _xmlstatus);
                    string folder = Basepage.Get_Folder(_doctype, _source, _xmlstatus ,txtXMLFileName.Text.Trim(),"");   //eg: C:\IDOL\ZATCA\files\invoiceXML\EXTERNAL\B2C\FAILED

                  //  _UNSIGNED_PATH = Basepage.Get_Folder(_doctype, _source, XMLSTATUS.UNSIGNED,"");  // //eg: C:\IDOL\ZATCA\files\invoiceXML\EXTERNAL\B2C\UNSIGNED
                    string fpath = folder + XMLFileName;
                    if (File.Exists(fpath))
                    {

                        string xml = File.ReadAllText(fpath, Encoding.UTF8);
                        txtXML.Text = xml;

                        btnDownload.Visible = true;
                        btnSave.Visible = true;
                        lblPath.Text = fpath;
                        lblSource.Text = Source;
                        lblDocType.Text = doctype;
                        lblActionStatus.Text = ActionStatus;
                        td_labels.Visible = true;
                        btnSave.Text = _doctype == DOCTYPE.B2B ? "Send for Clearance" : btnSave.Text;



                    //    break;
                    }
                }

            }
            else
                this.Master.ShowMessage("No data found", Alert.error, isClientScrpiptActive);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                string folder = Basepage.invoiceXML_folder + FOLDER.Portal_Uploaded.GetEnumDescription();
                string path = folder + txtXMLFileName.Text.Trim();

                File.WriteAllText(path , txtXML.Text.Trim());
                this.Master.TextMessage.Text = "Uploaded Successful.Check the reports for the Status.";

            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }
        }
    }
}