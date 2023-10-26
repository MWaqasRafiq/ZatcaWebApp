using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;

namespace ZatcaWebApp
{
    public partial class Change_pw : System.Web.UI.Page
    {
        Basepage bp = new Basepage();
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                if (!IsPostBack)
                {
                    // string sdd = txtNewPass.Text;
                    if (Session["user"] == null && Session["role"] == null)
                    {
                        Response.Redirect("Login.aspx");
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script language='javascript'>alert('Process is not successfull. You have some error in the system please try again.')</script>");

            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Session["userName"] = null;
            Session["user"] = null;
            Session["role"] = null;
            Response.Redirect("Login.aspx");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string uname = Session["user"].ToString();
                string query = "select Password from Z_Users where UserName='" + uname + "'";
                System.Data.DataTable dt = bp.getDataDBQuery(query);

                string pw_Entered = Convert.ToBase64String(Basepage.Encryption(Current_pw.Text));

                string pw_fromDB = dt.Rows[0][0].ToString();


                if (pw_Entered.Equals(pw_fromDB))
                {

                    string n = Convert.ToBase64String(Basepage.Encryption(new_pw.Text));

                    string q = "UPDATE Z_Users SET Password='" + n + "' where UserName='" + uname + "'";
                    int s = bp.getDataDBQuery_Update(q);
                    if (s == 1)
                    {
                        //this.Master.TextMessage.Text = "Password updated successfully..!!";
                        //this.Master.TextMessage.ForeColor = System.Drawing.Color.Green;
                        //Current_pw.Text = "";
                        //new_pw.Text = "";
                        //confirm_pw.Text = "";
                        this.Master.TextMessage.Text = "";

                       // bp.LogOperation(Session["user"].ToString(), DateTime.Now, "Changed password.");
                        Session.Clear();
                        Session.Abandon();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('Password updated successfully..!!'); window.location='" +
                             Request.ApplicationPath + "Login.aspx';", true);

                    }
                    else
                    {
                        this.Master.TextMessage.Text = "Error..!! Invalid fields..!!";
                        this.Master.TextMessage.ForeColor = System.Drawing.Color.Red;
                        Current_pw.Text = "";
                        new_pw.Text = "";
                        confirm_pw.Text = "";

                    }
                }

                else
                {
                    this.Master.TextMessage.Text = "Error..!! Current password is incorrect..!!";
                    this.Master.TextMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex) { Response.Write(ex.Message); }

        }
    }
}