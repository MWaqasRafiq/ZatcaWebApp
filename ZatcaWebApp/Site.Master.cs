using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZatcaWebApp
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        Basepage bp = new Basepage();




        public string Page_heading { get { return TB.Text; } set { TB.Text = value; } }


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GC.Collect();

                
                 
                if (Session["user"] == null && Session["role"] == null)
                {
                    lblUser.Text = "";
                    linkBtnLogin.Text = "Login";
                    menu.Visible = false;

                    change_pw_link.Text = "";
                    TB.Visible = false;


                }
                else if (Session["user"] != null)
                {
                    if (bp.CheckAccessPermission(Session["role"].ToString(), Session["PageID"].ToString()) == false)
                    {
                        Session.Clear();
                        Session.Abandon();
                        Response.Redirect("Login.aspx");
                    }

                   
                    SetAccessibility(int.Parse(Session["role"].ToString()));

                    menu.Visible = true;
                    linkBtnLogin.Text = "Logout";
                    lblUser.Text = "Welcome " + Session["user"].ToString();
                    lblRole.Text = Session["roleName"].ToString();
                    TB.Visible = true;

                    //USERID.Text = Session["userid"].ToString();
                    USERID.Text = "Welcome " + Session["user"].ToString();
                     
                }
                else
                {
                    Session.Clear(); Session.Abandon();
                    Response.Redirect("~/Login.aspx");
                }



            }

            catch (Exception ex)
            {
                this.TextMessage.Text = ex.Message;
                //Response.Write("<script language='javascript'>alert('Process is not successfull. You have some error in the system please try again.')</script>");
            }

        }

       

        public int SessionLengthMinutes
        {
            get { return Session.Timeout; }
        }
        public string SessionExpireDestinationUrl
        {
            get { return "/Login.aspx"; }
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.PageHead.Controls.Add(new LiteralControl(
                String.Format("<meta http-equiv='refresh' content='{0};url={1}'>",
                SessionLengthMinutes * 60, SessionExpireDestinationUrl)));
        }

        public string ErrorMessage
        {
            get
            {
                return lblMessage.Text;
            }
            set
            {
                lblMessage.Text = value;
            }
        }

        public Label TextMessage
        {
            get
            {
                return lblMessage;
            }
            set
            {
                lblMessage = value;
            }
        }

       



        public Label USERID
        {
            get
            {
                return lblUser;
            }
            set
            {
                lblUser = value;
            }
        }
        protected void linkBtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (linkBtnLogin.Text == "Logout")
                {
                    bp.logWrite("UserID " + Session["userID"].ToString() + " User logout.", LOGTYPE.LG);
                    lblUser.Text = "";
                    linkBtnLogin.Text = "Login";
                    Session["userName"] = null;
                    Session["userID"] = null;

                    Session.Clear();
                    Session.Abandon();

                }
                Response.Redirect("~/Login.aspx");
            }
            catch (Exception ex)
            {
                lblUser.Text = "";
                linkBtnLogin.Text = "Login";
                bp.logWrite("linkBtnLogin_Click:" + ex.ToString(), LOGTYPE.LG);
                Response.Redirect("~/Login.aspx");

            }

        }

        protected void linkBtnLoginNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (linkBtnLogin.Text == "Logout")
                {
                    bp.logWrite("UserID " + Session["userID"].ToString() + " User logout.", LOGTYPE.LG);
                    lblUser.Text = "";
                    linkBtnLogin.Text = "Login";
                    Session["userName"] = null;
                    Session["userID"] = null;

                    Session.Clear();
                    Session.Abandon();

                }
                Response.Redirect("~/Login.aspx");
            }
            catch (Exception ex)
            {
                lblUser.Text = "";
                linkBtnLogin.Text = "Login";
                bp.logWrite("linkBtnLoginNew_Click:" + ex.ToString(), LOGTYPE.LG);
                Response.Redirect("~/Login.aspx");

            }

        }






        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Change_pw.aspx");
        }


        private void AllVisibility(bool b)
        {
            string q = "Select [PageId] from [Z_PagesList]";
            DataTable dt = bp.getDataDBQuery(q);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string pageId = dr["PageId"].ToString();
                    Control myControl1 = FindControl(pageId);
                    myControl1.Visible = b;

                }
            }
            else
            {
                TextMessage.Text = "error occured."; TextMessage.ForeColor = System.Drawing.Color.Red;

            }

        }


        public void SetAccessibility(int roleCode)
        {
            if (roleCode != 0)
            {
                AllVisibility(false);
                string[] Ids;
                string q = "Select [AccessPageIds] from [Z_Roles] where [Code]=" + roleCode;
                DataTable dt = bp.getDataDBQuery(q);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string AccessPageIds = dr["AccessPageIds"].ToString();
                        if (!AccessPageIds.Equals(null))
                        {
                            Ids = AccessPageIds.Split(' ');
                            foreach (string id in Ids)
                            {
                                int a;
                                if (int.TryParse(id, out a))
                                {   //getting each pageId and MainMenuId
                                    string qr = " select a.PageId,(Select b.PageId from [Z_PagesList] b where b.Id=a.MainMenuId) as MainMenuId " +
                                        " from [Z_PagesList] a   where a.Id =" + a;
                                    DataTable tb = bp.getDataDBQuery(qr);
                                    if (tb.Rows.Count > 0)
                                    {
                                        foreach (DataRow dr1 in tb.Rows)
                                        {
                                            string pageId = dr1["PageId"].ToString();
                                            Control myControl1 = FindControl(pageId);
                                            myControl1.Visible = true;

                                            string MainMenuId = dr1["MainMenuId"].ToString();
                                            if (!MainMenuId.Equals(""))
                                            {
                                                Control myControl2 = FindControl(MainMenuId);
                                                myControl2.Visible = true;
                                            }


                                        }

                                    }
                                    else
                                    {
                                        TextMessage.Text = "Error:";
                                    }
                                }
                            }
                        }
                    }

                }

            }
            else
            {
                AllVisibility(true);
            }
        }
        public static string val;
        public string getVal
        {
            get { return val; }
            set { val = value; }
        }
        public void ShowMessage(string ex, Alert alertType, bool isClientScrpiptActive)
        {
            ex = ex.Replace("'", " ");

            ex = ex.Replace('"', ' ');
            ex = ex.Replace("\r\n", " ");
            ex = ex.Replace("/", " ");
            if (isClientScrpiptActive == true)
                this.TextMessage.Text = ex;                //Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "call_toaster('" + ex + "','" + alertType.ToString() + "')", true);

            else this.TextMessage.Text = ex;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            val = "Add_Customer_onclick";
            Response.Redirect("CustomerForm.aspx?editID=addForm");
        }

        protected void lnkHome_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("Home.aspx");
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, Alert.error, false);
            }
        }

        protected void lnkExit_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(Request.RawUrl);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, Alert.error, false);
            }
        }


    }

    public enum Alert
    {
        error,
        warning,
        info,
        success
    }
}

