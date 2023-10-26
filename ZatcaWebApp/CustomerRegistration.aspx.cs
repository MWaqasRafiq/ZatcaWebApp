using AjaxControlToolkit.HTMLEditor.ToolbarButton;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static java.util.jar.Attributes;

namespace ZatcaWebApp
{
    public partial class CustomerRegistration : System.Web.UI.Page
    {
        Basepage bp = new Basepage();
        //public static string CONFIG=""
        public bool isClientScrpiptActive = true;
       
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                this.Master.Page_heading = "B2B Customer Registration";

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
                        Session["PageId"] = "li_B2BCustomerRegistration";


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
                this.Master.TextMessage.Text = ""; 
                 

            DataSet ds = bp.ExecuteProcedure("CUSTOMER", Basepage.ConStrVATDB,  new SqlParameter[]  { new SqlParameter("OPERATION","INSERT_CUSTOMER"),
                                                                                new SqlParameter("PartyIdentificationSchemeID",txtPartyIdentificationSchemeID.Text.Trim()),
                                                                                new SqlParameter("PartyIdentificationID",txtPartyIdentificationID.Text.Trim()),
                                                                                new SqlParameter("StreetName",txtStreetName.Text),
                                                                                new SqlParameter("StreetNameArabic",txtStreetNameArabic.Text.Trim()),
                                                                                new SqlParameter("BuildingNumber",txtBuildingNumber.Text.Trim()), 
                                                                                new SqlParameter("PlotIdentification",txtPlotIdentification.Text.Trim()),
                                                                                new SqlParameter("CitySubdivisionName",txtCitySubdivisionName.Text.Trim()),
                                                                                new SqlParameter("CitySubdivisionNameArabic",txtCitySubdivisionNameArabic.Text.Trim()),
                                                                                new SqlParameter("CityName",txtCityName.Text.Trim()),
                                                                                new SqlParameter("CityNameArabic",txtCityNameArabic.Text.Trim()),
                                                                                new SqlParameter("PostalZone",txtPostalZone.Text.Trim()),
                                                                                new SqlParameter("CountryIdentificationCode",txtCountryIdentificationCode.Text.Trim()),
                                                                                new SqlParameter("TaxScheme",txtTaxScheme.Text.Trim()),
                                                                                new SqlParameter("PartyTaxSchemeCompanyID",txtPartyTaxSchemeCompanyID.Text.Trim()),
                                                                                new SqlParameter("RegistrationName",txtRegistrationName.Text.Trim()),
                                                                                new SqlParameter("RegistrationNameArabic",txtStreetNameArabic.Text.Trim()),
                                                                                new SqlParameter("CreatedUser", Session["user"]),
                                                                                new SqlParameter("custom_param_1",""),
                                                                                new SqlParameter("custom_param_2",""),
                                                                                new SqlParameter("custom_param_3",""),
                                                                                new SqlParameter("custom_param_4",""),
                                                                                new SqlParameter("custom_param_5","") });
                DataTable dt = ds.Tables[0];
                string errorMsg = dt.Rows[0]["errorMsg"].ToString();
                string errorNo = dt.Rows[0]["errorNo"].ToString();

                string msg = errorNo != "0" ? "error" : "success";
                this.Master.ShowMessage(errorMsg, errorNo != "0" ?  Alert.error : Alert.success, isClientScrpiptActive);

            }
            catch (Exception ex)
            {
                bp.logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }
        }
    }
}