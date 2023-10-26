 

 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ZatcaWebApp
{
    public partial class CreateUser : System.Web.UI.Page
    {
        Basepage bp = new Basepage();

        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
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
                        Session["PageID"] = "li_users";

                        Master.Page_heading = "Users";



                        DataTable dt3 = bp.getDataDBQuery("select * from [Z_Roles]");
                        role_ddl.Items.Clear();

                        role_ddl.AppendDataBoundItems = true;
                        role_ddl.DataTextField = "Description";
                        role_ddl.DataValueField = "Code";
                        role_ddl.DataSource = dt3;
                        role_ddl.DataBind();
                        role_ddl.SelectedIndex = 1;

                        role_ddl1.Items.Clear();
                        role_ddl1.Items.Insert(0, "");
                        role_ddl1.AppendDataBoundItems = true;
                        role_ddl1.DataTextField = "Description";
                        role_ddl1.DataValueField = "Code";
                        role_ddl1.DataSource = dt3;
                        role_ddl1.DataBind();
                        role_ddl1.SelectedIndex = 1;


                    }
                }

            }
            catch (Exception ex)
            { this.Master.TextMessage.Text = "Error occured."; this.Master.TextMessage.ForeColor = System.Drawing.Color.Red; }





        }

        protected void gvReport_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";
                if (e.CommandName.Equals("edit_user"))
                {
                    string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
                    lbl_id_update.Text = commandArgs[0];
                    string fName = commandArgs[1];
                    string uName = commandArgs[2];
                    string role = commandArgs[3];
                    string roleCode = commandArgs[4];
                    string password_encrypted = commandArgs[5];


                    name1.Text = fName;
                    uname1.Text = uName; //username
                    role_ddl1.SelectedValue = roleCode; //role
                    role_ddl1.DataBind();



                    string p = Basepage.Decryption(password_encrypted);  //password decryption
                    password1.Attributes["value"] = p;
                    repeat_pw1.Attributes["value"] = p;
                    //password1.Text = p;  //password
                    //repeat_pw1.Text = p; //repaet assword


                    mpe_updateuser.Show();

                }

                if (e.CommandName.Equals("Deactivate"))
                {


                    string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
                    int id_to_update = int.Parse(commandArgs[0]);

                    DataSet ds = bp.ExecuteProcedure("Z_CreateUser", Basepage.conStr, new SqlParameter[]  { new SqlParameter("operation","deactivate_user"),
                                                                                                     new SqlParameter("FirstName",""),
                                                                                                   new SqlParameter("LastName",""),
                                                                                                   new SqlParameter("Email",""),
                                                                                                   new SqlParameter("UserName",""),
                                                                                                   new SqlParameter("Password",""),
                                                                                                   new SqlParameter("Role",9999),  //not used
                                                                                                   new SqlParameter("Status",9999),  //not used  
                                                                                                   new SqlParameter("id_to_update",id_to_update),
                                                                                                   new SqlParameter("LoggedIn_UserName", Session["user"])});

                    DataTable dt = ds.Tables[0];
                    string errorMsg = dt.Rows[0]["errorMsg"].ToString();
                    string errorNo = dt.Rows[0]["errorNo"].ToString();
                    string msg = errorNo != "0" ? "error" : "success";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "call_toaster('" + errorMsg + "','" + msg + "')", true);
                    //ClientScript.RegisterStartupScript(this.GetType(), "yourMessage", "alert('" + errorMsg + "');", true);

                    GridView1.DataBind();



                }
                if (e.CommandName.Equals("Activate"))
                {

                    string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
                    int id_to_update = int.Parse(commandArgs[0]);

                    DataSet ds = bp.ExecuteProcedure("Z_CreateUser", Basepage.conStr, new SqlParameter[]  { new SqlParameter("operation","activate_user"),
                                                                                                     new SqlParameter("FirstName",""),
                                                                                                   new SqlParameter("LastName",""),
                                                                                                   new SqlParameter("Email",""),
                                                                                                   new SqlParameter("UserName",""),
                                                                                                   new SqlParameter("Password",""),
                                                                                                   new SqlParameter("Role",9999),  //not used
                                                                                                   new SqlParameter("Status",9999),  //not used  
                                                                                                   new SqlParameter("id_to_update",id_to_update),
                                                                                                   new SqlParameter("LoggedIn_UserName", Session["user"])});
                    DataTable dt = ds.Tables[0];
                    string errorMsg = dt.Rows[0]["errorMsg"].ToString();
                    string errorNo = dt.Rows[0]["errorNo"].ToString();
                    string msg = errorNo != "0" ? "error" : "success";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "call_toaster('" + errorMsg + "','" + msg + "')", true);
                    // ClientScript.RegisterStartupScript(this.GetType(), "yourMessage", "alert('" + errorMsg + "');", true);

                    GridView1.DataBind();

                }
            }
            catch (Exception ex) { this.Master.TextMessage.Text = ex.Message; this.Master.TextMessage.ForeColor = System.Drawing.Color.Red; }
        }

        protected string IsStatusActive(String status)
        {
            if (status.Equals("Active")) //returned from 'case when' in the sql datasourse
            {
                return "Deactivate";
            }
            else if (status == "Not Active")
            {
                return "Activate";

            }
            else
            {
                return "";
            }
        }



        protected void create_btn_Click(object sender, EventArgs e)
        {
            try
            {
                this.Master.TextMessage.Text = "";

                string firstName = name.Text;
                string userName = uname.Text;

                string pw = Convert.ToBase64String(Basepage.Encryption(password.Text));
                int role = int.Parse(role_ddl.SelectedItem.Value);

                DataSet ds = bp.ExecuteProcedure("Z_CreateUser", Basepage.conStr, new SqlParameter[]  {  new SqlParameter("operation","create_user"),
                                                                                                   new SqlParameter("FirstName",firstName),
                                                                                                   new SqlParameter("LastName",""),
                                                                                                   new SqlParameter("Email",""),
                                                                                                   new SqlParameter("UserName",userName),
                                                                                                   new SqlParameter("Password",pw),
                                                                                                   new SqlParameter("Role",role),
                                                                                                   new SqlParameter("Status",1),
                                                                                                   new SqlParameter("id_to_update",9999) ,//valid only while update upser process
                                                                                                   new SqlParameter("LoggedIn_UserName", Session["user"])});
                DataTable dt = ds.Tables[0];
                string errorMsg = dt.Rows[0]["errorMsg"].ToString();
                string errorNo = dt.Rows[0]["errorNo"].ToString();

                string msg = errorNo != "0" ? "error" : "success";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "call_toaster('" + errorMsg + "','" + msg + "')", true);
                GridView1.DataBind();
            }
            catch (Exception ex) { this.Master.TextMessage.Text = ex.Message; this.Master.TextMessage.ForeColor = System.Drawing.Color.Red; }

        }
        protected void update_Click(object sender, EventArgs e)
        {
            try
            {
                int id_to_update = int.Parse(lbl_id_update.Text);
                string pw = Convert.ToBase64String(Basepage.Encryption(password1.Text));



                DataSet ds = bp.ExecuteProcedure("Z_CreateUser", Basepage.conStr, new SqlParameter[]  { new SqlParameter("operation","update_user"),
                                                                                                     new SqlParameter("FirstName",name1.Text),
                                                                                                   new SqlParameter("LastName",""),
                                                                                                   new SqlParameter("Email",""),
                                                                                                   new SqlParameter("UserName",uname1.Text),
                                                                                                   new SqlParameter("Password",pw),
                                                                                                   new SqlParameter("Role",int.Parse(role_ddl1.SelectedValue)),
                                                                                                   new SqlParameter("Status",9999),  //not used in sp
                                                                                                   new SqlParameter("id_to_update",id_to_update),
                                                                                                   new SqlParameter("LoggedIn_UserName", Session["user"])});
                DataTable dt = ds.Tables[0];
                string errorMsg = dt.Rows[0]["errorMsg"].ToString();
                string errorNo = dt.Rows[0]["errorNo"].ToString();


                string msg = errorNo != "0" ? "error" : "success";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "call_toaster('" + errorMsg + "','" + msg + "')", true);
                GridView1.DataBind();

            }
            catch (Exception ex) { this.Master.TextMessage.Text = ex.Message; this.Master.TextMessage.ForeColor = System.Drawing.Color.Red; }
        }


    }
}