using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ZatcaWebApp
{
    public partial class Home : System.Web.UI.Page
    {
        public bool isClientScrpiptActive = true;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadVATDDL(false);
                LoadChart();
            }
        }

        public void LoadChart()
        {
            try
            {
                string connSring = ddlVAT.SelectedValue.ToString();
                SqlDataSource1.ConnectionString = connSring;
                SqlDataSource2.ConnectionString = connSring;
                SqlDataSource3.ConnectionString = connSring;

                string qq = @"select  
                (select sum(taxtotal) from invoices where LTRIM(RTRIM(ActionStatus))  in('Cleared','Reported')) as TotalVatSubmitted,
                (select sum(taxtotal) from invoices where LTRIM(RTRIM(ActionStatus)) not  in('Cleared','Reported')) as TotalVatFailed,
                '0' as VATPending,
                (select count(id) from invoices) as CountProcessed,
                (select sum(taxtotal) from invoices) as VATProcessed,
                (select sum(taxtotal) from invoices where DocType='B2B') as VatB2B,
                (select sum(taxtotal) from invoices where DocType='B2C') as VatB2C";

                DataTable dt = new Basepage().getDataDBQuery(qq);
                lblTotalVatSubmitted.Text = dt.Rows[0]["TotalVatSubmitted"].ToString();
                lblVatFailed.Text = dt.Rows[0]["TotalVatFailed"].ToString();
                // lblVATPending.Text = dt.Rows[0]["VATPending"].ToString();
                lblProcessed.Text = dt.Rows[0]["CountProcessed"].ToString();
                lblVatB2B.Text = dt.Rows[0]["VatB2B"].ToString();
                lblVatB2C.Text = dt.Rows[0]["VatB2C"].ToString();

                string path = Basepage.Get_Folder(DOCTYPE.B2C, SOURCE.POS, XMLSTATUS.SIGNED, "", "");
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
                int count = dir.GetFiles().Length;
                lblPending.Text = count.ToString();
                SqlDataSource2.DataBind();
            }
            catch (Exception ex)
            {
                new Basepage().logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);
            }
        }
        public void LoadVATDDL(bool enableAll)
        { 
            ddlVAT.Items.Clear();

            if(enableAll)
                ddlVAT.Items.Add(new ListItem("ALL", "ALL"));

            DataTable dt = new Basepage().getDataDBQuery("select * from VATIDs where status=1");

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ListItem item = new ListItem(dt.Rows[i]["VATNo"].ToString(), dt.Rows[i]["ConnectionString"].ToString());
                    ddlVAT.Items.Add(item);
                }
            }
        }

        protected void ddlVAT_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadChart();
            }
            catch (Exception ex)
            {
                new Basepage().logWrite(ex.ToString(), LOGTYPE.LG);
                this.Master.ShowMessage(ex.Message, Alert.error, isClientScrpiptActive);

            }
        }
    }
     
}