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
    public partial class StoresList : System.Web.UI.Page
    {
        Basepage bp = new Basepage();
        //public static string CONFIG=""
        public bool isClientScrpiptActive = true;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                this.Master.Page_heading = "View/Add Stores";

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
                        Session["PageId"] = "li_store";


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

                txtfromHOUR.Text = txtfromHOUR.Text.Trim() == "" ? "00" : int.Parse(txtfromHOUR.Text).ToString("D2");
                txtfromMINUTE.Text = txtfromMINUTE.Text.Trim() == "" ? "00" : int.Parse(txtfromMINUTE.Text).ToString("D2");
                txttoHOUR.Text = txttoHOUR.Text.Trim() == "" ? "00" : int.Parse(txttoHOUR.Text).ToString("D2");
                txttoMINUTE.Text = txttoMINUTE.Text.Trim() == "" ? "00" : int.Parse(txttoMINUTE.Text).ToString("D2");

                string _fromtime = txtfromHOUR.Text + ":" + txtfromMINUTE.Text + ":00";
                string _totime = txttoHOUR.Text + ":" + txttoMINUTE.Text + ":00";


                DataSet ds = bp.ExecuteProcedure("SP_Stores", Basepage.conStr, new SqlParameter[] {
                                new SqlParameter("@OPERATION", INVOICE_OPERATION.INSERT_STORE.ToString()),
                                new SqlParameter("@StoreNo", txtStoreNo.Text.Trim()),
                                new SqlParameter("@StoreName", txtStoreName.Text.Trim()),
                                new SqlParameter("@StoreIP", txtStoreIP.Text.Trim()),
                                new SqlParameter("@OpenHoursStart",_fromtime),
                                new SqlParameter("@OpenHoursEnd",_totime ),
                                new SqlParameter("@Status",status_Ddl.SelectedValue.Trim())
                                });

                DataTable dt = ds.Tables[0];
                string errorMsg = dt.Rows[0]["errorMsg"].ToString();
                string errorNo = dt.Rows[0]["errorNo"].ToString();

                bp.log("SP_Stores Status:: " + errorMsg + " errorNo:" + errorNo + " StoreIP:" + txtStoreIP.Text.Trim(), 1, LOGTYPE.LG);


                if (errorMsg.Equals("SUCCESS"))
                {
                    this.Master.ShowMessage("Successful", Alert.error, isClientScrpiptActive);
                    SqlDataSource1.DataBind();
                    GridView1.DataBind();
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