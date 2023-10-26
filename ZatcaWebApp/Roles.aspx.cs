 


using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZatcaWebApp
{
    public partial class Roles : System.Web.UI.Page
    {
        Basepage bp = new Basepage();
        public static bool isClientScrpiptActive = true;
        static int id;


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                txt_name.ReadOnly = false;


                this.Master.Page_heading = "Roles";
                lbl_homeId.Text = WebConfigurationManager.AppSettings["HomePageId"];
                this.Master.TextMessage.Text = "";
                if (!IsPostBack)
                {
                    if (Session["user"] == null && Session["role"] == null)
                    {
                        Session.Clear();
                        Session.Abandon();
                        Response.Redirect("Login.aspx");
                    }
                    else
                    {
                        Session["PageID"] = "li_Roles";
                        SelectRole();
                    }
                }


            }
            catch (Exception ex)
            { this.Master.TextMessage.Text = ex.Message; this.Master.TextMessage.ForeColor = System.Drawing.Color.Red; }



        }

        private void SelectRole()
        {
            try
            {
                DataTable dt = bp.getDataDBQuery("Select * from [Z_Roles]");

                int c = 1;  //role code
                if (dt.Rows.Count > 0)
                {
                    //get the last role code
                    string code = "999";

                    foreach (DataRow dr in dt.Rows)
                    {
                        code = dr["Code"].ToString();
                    }

                    if (int.TryParse(code, out c) == false)
                    {
                        this.Master.TextMessage.Text = ("Error on loading role codes.");
                    }
                    else
                    {
                        c = c + 1;  //new role code

                    }

                    // check whether the new role code alreasy exists.
                    for (; ; )
                    {
                        DataTable dt1 = bp.getDataDBQuery("select * from [z_Roles] where [Code]=" + c);
                        if (dt1.Rows.Count > 0)
                        {
                            c = c + 1;
                        }
                        else
                        {
                            break;
                        }
                    }


                    txt_roleCode.Text = c.ToString();
                }
                else
                {
                    txt_roleCode.Text = "1";
                }

            }
            catch (Exception ex)
            { this.Master.TextMessage.Text = ex.Message; this.Master.TextMessage.ForeColor = System.Drawing.Color.Red; }

        }


        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            try
            {
                this.Master.TextMessage.Text = "";
                if (e.CommandName.Equals("edit_role"))
                {


                    btb_add.Text = "Update";
                    hdf_btnText.Value = "Update";
                    txt_name.Attributes.Add("readonly", "readonly");

                    // BulletedList1.Items.Clear();
                    CheckBoxList1.ClearSelection();
                    string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });

                    string AccessPgeIds = commandArgs[0];
                    string Id = commandArgs[1];
                    string Code = commandArgs[2];
                    string Description = commandArgs[3];
                    id = int.Parse(Id);
                    txt_name.Text = Description;


                    string[] Ids = AccessPgeIds.Split(' ');
                    foreach (string id1 in Ids)
                    {
                        int n;
                        if (int.TryParse(id1, out n))
                        {
                            DataTable dt = bp.getDataDBQuery("Select PageName from Z_PagesList where Id=" + n);
                            string ss = dt.Rows[0]["PageName"].ToString();
                            // BulletedList1.Items.Add(ss);
                            if (ss != "Home")
                            {
                                CheckBoxList1.Items.FindByText(ss).Selected = true;
                            }
                        }

                    }

                    mpe_role.Show();
                }
                else
                {
                    btb_add.Text = "Add";
                }

            }
            catch (Exception ex)
            { this.Master.TextMessage.Text = ex.Message; this.Master.TextMessage.ForeColor = System.Drawing.Color.Red; }


        }
        protected void btb_add_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                string btnText = hdf_btnText.Value;
                if (btnText == "Add")
                {
                    AddData();
                }
                else
                {
                    UpdateData();
                }
            }
            catch (Exception ex)
            { this.Master.TextMessage.Text = ex.Message; this.Master.TextMessage.ForeColor = System.Drawing.Color.Red; }

        }

        private void UpdateData()
        {
            //get access page Ids
            string s = WebConfigurationManager.AppSettings["HomePageId"] + " ";  //  Id of the Home Page in databalse table Pagelist. Which is not allowed to change or delete.
            foreach (ListItem li in CheckBoxList1.Items)
            {
                if (li.Selected)
                {
                    s = s + li.Value + " ";
                }
            }
            string qu = "Update  [Z_Roles] set AccessPageIds='" + s + "' where Id=" + id;
            int n = bp.getDataDBQuery_Update(qu);
            if (n == 1)
            {

                this.Master.TextMessage.Text = ("Role is updated successfully.");
                //bp.LogOperation_UsersDB(Session["user"].ToString(), DateTime.Now, "Role  with Id " + id + " is updated");
                txt_name.Text = "";
                CheckBoxList1.ClearSelection();
                SelectRole();
                GridView1.DataBind();

            }
            else
            {


                this.Master.TextMessage.Text = ("Process failed.");
            }
            btb_add.Text = "Add";
            txt_name.Attributes.Remove("readonly");
        }

        private void AddData()
        {
            //get access page Ids
            string s = WebConfigurationManager.AppSettings["HomePageId"] + " ";  //  Id of the Home Page in databalse table Pagelist. Which is not allowed to change or delete.
            foreach (ListItem li in CheckBoxList1.Items)
            {
                if (li.Selected)
                {
                    s = s + li.Value + " ";
                }
            }

            //check role name already exists
            DataTable dd = bp.getDataDBQuery("SELECT * FROM [z_Roles] WHERE [Description]='" + txt_name.Text + "'");
            if (dd.Rows.Count > 0)
            {
                string errorMsg = "Role Name already exists.Enter another name.";
                this.Master.TextMessage.Text = (errorMsg);
            }
            else
            {
                this.Master.TextMessage.Text = "";

                string Description = txt_name.Text;
                int code = int.Parse(txt_roleCode.Text);
                string qu = "Insert into [z_Roles] (Code,Description,AccessPageIds) VALUES(" + code + ",'" + Description + "','" + s + "')";
                int n = bp.getDataDBQuery_Update(qu);
                if (n == 1)
                {
                    string errorMsg = "Created new role successfully.";
                    this.Master.TextMessage.Text = (errorMsg);


                    //  bp.LogOperation_UsersDB(Session["user"].ToString(), DateTime.Now, "Role " + Description + " with code " + code + " is added");
                    txt_name.Text = "";
                    CheckBoxList1.ClearSelection();
                    SelectRole();
                    GridView1.DataBind();
                }
                else
                {
                    this.Master.TextMessage.Text = ("Process failed");
                }

            }
        }

    }
}