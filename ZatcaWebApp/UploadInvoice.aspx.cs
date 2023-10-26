using ExtensionMethod;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZatcaWebApp
{
    public partial class UploadInvoice : System.Web.UI.Page
    {
        Basepage bp = new Basepage();
        //public static string CONFIG=""
        public bool isClientScrpiptActive = true;
        public static string _UNSIGNED_PATH;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                this.Master.Page_heading = "Upload Invoice";

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
                        Session["PageId"] = "li_uplodInvoice";


                    }


                   
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
            try {
                this.Master.TextMessage.Text = "";

               // Enum.TryParse(rd_doctype.SelectedValue, out DOCTYPE _doctype);
                //Enum.TryParse(rd_source.SelectedValue, out SOURCE _source);

                string folder = Basepage.invoiceXML_folder + FOLDER.Portal_Uploaded.GetEnumDescription();
                string path = folder + FileUpload1.FileName;
                if (Path.GetExtension(path).ToLower().Equals(".xml") == false)
                {
                    throw new InvalidCastException("Upload XML file");

                }
                if (File.Exists(path))
                    File.Delete(path);

                FileUpload1.SaveAs(path);

                this.Master.TextMessage.Text = "File saved for processing.";

            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }
        }
    }
}
 