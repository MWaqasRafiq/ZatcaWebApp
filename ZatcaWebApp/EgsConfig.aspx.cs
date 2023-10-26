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
    public partial class EgsConfig : System.Web.UI.Page
    {
        Basepage bp = new Basepage();
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
                        Session["PageId"] = "li_EGSconfig";


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
            try
            {
                string SEPR = "anddd";
                string sssssss = txtEGS.Text.Trim() + SEPR +
                               txtcsr_common_name.Text.Trim() + SEPR +
                               txtcsr_serial_number.Text.Trim() + SEPR +
                               txtcsr_organization_identifier.Text.Trim() + SEPR +
                               txtcsr_organization_unit_name.Text.Trim() + SEPR +
                               txtcsr_organization_name.Text.Trim() + SEPR +
                               txtcsr_country_name.Text.Trim() + SEPR +
                               txtcsr_invoice_type.Text.Trim() + SEPR +
                               txtcsr_location_address.Text.Trim() + SEPR +
                               txtcsr_industry_business_category.Text.Trim() + SEPR +
                               txtseller_identification_schemeID.Text.Trim() + SEPR +
                               txtseller_identification_ID.Text.Trim() + SEPR +
                               txtstreetName.Text.Trim() + SEPR +
                               txtbuildingNumber.Text.Trim() + SEPR +
                               txtPlotIdentification.Text.Trim() + SEPR +
                               txtcitySubdivisionName.Text.Trim() + SEPR +
                               txtcityName.Text.Trim() + SEPR +
                               txtpostalZone.Text.Trim() + SEPR +
                               txtcountryIdentificationCode.Text.Trim() + SEPR +
                               txtpartyTaxSchemeCompanyId.Text.Trim() + SEPR +
                               txtpartyLegalEntityRegistrationName.Text.Trim();
                sssssss = sssssss.Replace("&", "<<<>>>");  //if any field has already _ in it, replace it, not to be considered in split.
                sssssss = sssssss.Replace(SEPR, "&");

                DataSet ds = bp.ExecuteProcedure("SP_INVOICE", Basepage.conStr, new SqlParameter[] {
                                new SqlParameter("@OPERATION", INVOICE_OPERATION.INSERT_EGSconfig.ToString()),
                                new SqlParameter("@IRN", sssssss),
                                new SqlParameter("@UUID", ""),
                                new SqlParameter("@XMLFileName", ""),
                                new SqlParameter("@TaxTotal", ""),
                                new SqlParameter("@DocType",""),
                                new SqlParameter("@Action","" ),
                                new SqlParameter("@ActionStatus",""),
                                new SqlParameter("@TransactionDate", ""),
                                new SqlParameter("@StoreNo", ""),
                                new SqlParameter("@TerminalNo", ""),
                                new SqlParameter("@TrxNo", ""),
                                new SqlParameter("@TimeleftToReport",""),
                                new SqlParameter("@ReportedIn", ""),
                                new SqlParameter("@Source", ""),
                                new SqlParameter("@ActionCount",""),
                                new SqlParameter("@ErrorCount",""),
                                new SqlParameter("@WarningCount", ""),
                                new SqlParameter("@Errors",""),
                                new SqlParameter("@Warnings", ""),
                                new SqlParameter("@Info", ""),
                                new SqlParameter("@FullResponse", ""),
                                new SqlParameter("@ProcessType", ""),
                                new SqlParameter("@HTTPResponseCode",""),
                                new SqlParameter("@CustIdentifier",Basepage.CustIdentifier),
                                new SqlParameter("@InvoiceHash", ""),
                                new SqlParameter("@QR","")
                                });

                DataTable dt = ds.Tables[0];
                string errorMsg = dt.Rows[0]["errorMsg"].ToString();
                string errorNo = dt.Rows[0]["errorNo"].ToString();

                bp.log("DB_UPDAte Status:: " + errorMsg + " errorNo:" + errorNo + " txt:" + sssssss, 1, LOGTYPE.LG);


                if (errorMsg.Equals("SUCCESS"))
                {
                }
                else throw new InvalidCastException("err:" + errorMsg);
            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }
        }

        

    }
}