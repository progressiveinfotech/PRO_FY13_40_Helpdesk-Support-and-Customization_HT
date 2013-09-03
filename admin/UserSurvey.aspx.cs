﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;

public partial class UserSurvey : System.Web.UI.Page
{
    UserLogin_mst objuser = new UserLogin_mst();
    BLLCollection<UserLogin_mst> coluser = new BLLCollection<UserLogin_mst>();
    ContactInfo_mst objusercontact = new ContactInfo_mst();
    BLLCollection<ContactInfo_mst> colcontact = new BLLCollection<ContactInfo_mst>();
    UserEmail objuseremail = new UserEmail();
    SentMailToUser objsentuseremail = new SentMailToUser();
    SqlDataProvider db = new SqlDataProvider();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindGrid();
            
        }
    }
    protected void BindGrid()
    {

        //coluser = objuser.Get_All();
        //grdvwSite.DataSource = coluser;
        //grdvwSite.DataBind();
        colcontact = objusercontact.Get_By_comandname("A");
        grdvwSite.DataSource = colcontact;
        grdvwSite.DataBind();
        ViewState["commandname"] = "a";

    }
    protected void grdvwSite_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {

        RememberOldValues();

        // Hndling GridView PageIndex Change Event for Paging  
        grdvwSite.PageIndex = e.NewPageIndex;
        // On Selected Page Index Bind Grid here

        BindGrid1();
        RePopulateValues();

    }
    protected void grdvwsite_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Footer)
        {

            TableCell cell = e.Row.Cells[0];

            cell.ColumnSpan = 6;

            for (int i = 65; i <= (65 + 25); i++)
            {

                LinkButton lb = new LinkButton();

                lb.Text = Char.ConvertFromUtf32(i) + "&nbsp;&nbsp;&nbsp;";

                lb.CommandArgument = Char.ConvertFromUtf32(i);

                lb.CommandName = "AlphaPaging";
                lb.Font.Size = FontUnit.Large;
                cell.Controls.Add(lb);

            }

        }
    }
    protected void grdvwSite_RowCommand(object sender, GridViewCommandEventArgs e)
    {



        if (e.CommandName.Equals("AlphaPaging"))
        {
            string commandname = e.CommandArgument.ToString();
            ViewState["commandname"] = e.CommandArgument.ToString();

            colcontact = objusercontact.Get_By_comandname(commandname);
            if (colcontact.Count != 0)
            {

                grdvwSite.DataSource = colcontact;
                grdvwSite.DataBind();

            }
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("userid");
                dt.Columns.Add("firstname");
                dt.Columns.Add("lastname");
                dt.Columns.Add("emailid");

                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);

                grdvwSite.DataSource = dt;
                grdvwSite.DataBind();

                //grdvwViewAsset.Rows[0].Cells[3].Visible = false;
                //grdvwViewAsset.Rows[0].Cells[5].Visible = false;


            }


        }

    }
    protected void BindGrid1()
    {
        string commandname;
        commandname = ViewState["commandname"].ToString();

        colcontact = objusercontact.Get_By_comandname(commandname);
        if (colcontact.Count != 0)
        {
            grdvwSite.DataSource = colcontact;
            grdvwSite.DataBind();
        }

    }

    private void RememberOldValues()
    {
        ArrayList categoryIDList = new ArrayList();
        int index = -1;
        foreach (GridViewRow row in grdvwSite.Rows)
        {
            index = (int)grdvwSite.DataKeys[row.RowIndex].Value;
            bool result = ((CheckBox)row.FindControl("CheckAll")).Checked;

            // Check in the Session
            if (Session["CHECKED_ITEMS"] != null)
                categoryIDList = (ArrayList)Session["CHECKED_ITEMS"];
            if (result)
            {
                if (!categoryIDList.Contains(index))
                    categoryIDList.Add(index);
            }
            else
                categoryIDList.Remove(index);
        }
        if (categoryIDList != null && categoryIDList.Count > 0)
            Session["CHECKED_ITEMS"] = categoryIDList;
    }

    private void RePopulateValues()
    {
        ArrayList categoryIDList = (ArrayList)Session["CHECKED_ITEMS"];
        if (categoryIDList != null && categoryIDList.Count > 0)
        {
            foreach (GridViewRow row in grdvwSite.Rows)
            {
                int index = (int)grdvwSite.DataKeys[row.RowIndex].Value;
                if (categoryIDList.Contains(index))
                {
                    CheckBox myCheckBox = (CheckBox)row.FindControl("CheckAll");
                    myCheckBox.Checked = true;
                }
            }
        }
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {

        DataTable dtMealTemplate = new DataTable();
        SqlDataProvider db = new SqlDataProvider();
        dtMealTemplate.Columns.Add("Userid", Type.GetType("System.Int32"));
        dtMealTemplate.Columns.Add("Emailid", Type.GetType("System.String"));
        foreach (GridViewRow gvr in grdvwSite.Rows)
        {
            ///DataRow drMT = new DataRow();

            DataRow drMT = dtMealTemplate.NewRow();
            drMT["Userid"] = gvr.Cells[1].Text;
            drMT["Emailid"] = gvr.Cells[4].Text;

            dtMealTemplate.Rows.Add(drMT);
            CheckBox myCheckBox = (CheckBox)gvr.FindControl("CheckAll");
            if (myCheckBox.Checked == true)
            {
                UserEmail obj1 = new UserEmail();
               
                objuseremail.Userid = Convert.ToInt32(drMT["Userid"]);
                objuseremail.Emailid = drMT["Emailid"].ToString();
                objsentuseremail.SentFeedbackmailToUser(objuseremail.Userid, objuseremail.Emailid);
                objuseremail.InsertFeedbackCustomer();
                
                               
            }
        }

    }



   
}
