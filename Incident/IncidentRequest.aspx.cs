﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Incident_IncidentRequest : System.Web.UI.Page
{
    #region Definition of Objects of various classes
    SentMailToUser objSentmailtoUser = new SentMailToUser();
    UserToSiteMapping ObjUserToSite = new UserToSiteMapping();
    RoleInfo_mst objRole = new RoleInfo_mst();
    Site_mst objSite = new Site_mst();
    UserLogin_mst objUser = new UserLogin_mst();
    Organization_mst objOrganization = new Organization_mst();
    BLLCollection<Site_mst> colSite = new BLLCollection<Site_mst>();
    BLLCollection<UserLogin_mst> colUser = new BLLCollection<UserLogin_mst>();
    BLLCollection<UserToSiteMapping> colUserToSite = new BLLCollection<UserToSiteMapping>();
    BLLCollection<Department_mst> colDept = new BLLCollection<Department_mst>();
    Department_mst objDept = new Department_mst();
    BLLCollection<Category_mst> colCategory = new BLLCollection<Category_mst>();
    Category_mst objCategory = new Category_mst();
    Impact_mst objImpact = new Impact_mst();
    BLLCollection<Impact_mst> colImpact = new BLLCollection<Impact_mst>();
    Status_mst objStatus = new Status_mst();
    BLLCollection<Status_mst> colStatus = new BLLCollection<Status_mst>();
    Mode_mst objMode = new Mode_mst();
    BLLCollection<Mode_mst> colMode = new BLLCollection<Mode_mst>();
    Priority_mst objPriority = new Priority_mst();
    BLLCollection<Priority_mst> colPriority = new BLLCollection<Priority_mst>();
    Incident_mst objIncident = new Incident_mst();
    IncidentStates objIncidentStates = new IncidentStates();
    Subcategory_mst objSubCategory = new Subcategory_mst();
    BLLCollection<Subcategory_mst> colSubCategory = new BLLCollection<Subcategory_mst>();
    IncidentHistory objIncidentHistory = new IncidentHistory();
    RequestType_mst objRequestType = new RequestType_mst();
    BLLCollection<RequestType_mst> colRequesttype = new BLLCollection<RequestType_mst>();
    UserToAssetMapping objusertoasset = new UserToAssetMapping();
    IncidentToAssetMapping objincidenttoasset = new IncidentToAssetMapping();
    Asset_mst objassetmst = new Asset_mst();
    Customer_mst objCustomer = new Customer_mst();
    BLLCollection<Customer_mst> colCust = new BLLCollection<Customer_mst>();
    CustomerToSiteMapping objCustToSite = new CustomerToSiteMapping();
    BLLCollection<CustomerToSiteMapping> colCustToSite = new BLLCollection<CustomerToSiteMapping>();
    Title_mst objTitle = new Title_mst();
    BLLCollection<Title_mst> colTitle = new BLLCollection<Title_mst>();
    TechnicianSLAStop objtechslastop = new TechnicianSLAStop();
    IncidentHistoryDiff objIncidentHistoryDiff = new IncidentHistoryDiff();
    //ContactInfo_mst objConInfo = new ContactInfo_mst();
    //ContactInfo_mst objcnt = new ContactInfo_mst();
   
    #endregion
    int assetid=0;
    string compname;
    string username;
    string priorityid;
    protected void Page_Load(object sender, EventArgs e)
    { /////Add Exception handilng try catch change by vishal 21-05-2012
        try
        {
            this.ClientScript.GetPostBackEventReference(this, "arg");

            // Values for SelectAsset Page after select the asset for a particular user
            int flag = Convert.ToInt32(Session["flag"]);
            if (flag == 1)
            {

                assetid = Convert.ToInt32(Session["assetid"]);
                compname = (string)(Session["compname"]);
                txtassignasset.Text = compname.ToString();
                username = (string)(Session["username"]);
                txtUsername.Text = username.ToString();
                txtEmail.Text = Session["Email"].ToString();
                flag = 0;
            }

            if (!IsPostBack)
            {
                //txtUsername.Text = "";
                //txtassignasset.Text = "";

                #region Declaration of Binding Drop Down Function
                BindDropCustomer();
                BindDropSite();
                BindDropDept();
                BindDropCategory();
                BindDropStatus();
                BindDropMode();
                //BindDropPriority();
                BindDropTechnician();
                BindDropSubCategory();
                BindDropRequestType();
                //   BindTitle();

                #endregion

            }
            #region Page is Postback by Javascript,and get eventArgument to check user is to be created or not
            if (IsPostBack)
            {
                string eventTarget = this.Request["__EVENTTARGET"];
                string eventArgument = this.Request["__EVENTARGUMENT"];
                if (eventTarget != string.Empty && eventTarget == "callPostBack")
                {
                    if (eventArgument != string.Empty)
                    {
                        Session["UserCreate"] = eventArgument.ToString();


                    }
                }
            }
            #endregion
        }
        catch (Exception ex)
        {
            string myScript;
            myScript = "<script language=javascript>alert('Exception - '" + ex + "');</script>";
            Page.RegisterClientScriptBlock("MyScript", myScript);
            return;
        }

    }
    public void BindDropCustomer()
    {
        //colCust = objCustomer.Get_All();
        //drpCustomer.DataTextField = "Customer_Name";
        //drpCustomer.DataValueField = "CustId";
        //drpCustomer.DataSource = colCust;
        //drpCustomer.DataBind();
        //ListItem item = new ListItem();
        //item.Text = "------------Select-------------";
        //item.Value = "0";
        //drpCustomer.Items.Add(item);
        //drpCustomer.SelectedValue = "0";


        BLLCollection<Customer_mst> colCtS = new BLLCollection<Customer_mst>();
        string userName = "";
        MembershipUser User = Membership.GetUser();
        if (User != null)
        {
            userName = User.UserName.ToString();
        }
        if (userName != "")
        {
            int userid;
            int Flagcount = 0;
            objOrganization = objOrganization.Get_Organization();
            objUser = objUser.Get_UserLogin_By_UserName(userName, objOrganization.Orgid);
            if (objUser.Userid != 0)
            {
                userid = objUser.Userid;
                colUserToSite = ObjUserToSite.Get_All_By_userid(userid);
                foreach (UserToSiteMapping obj in colUserToSite)
                {
                    int siteid;
                    Site_mst objSite1 = new Site_mst();
                    siteid = obj.Siteid;
                    objSite1 = objSite1.Get_By_id(siteid);
                    if (objSite1.Siteid != 0)
                    {
                        colCustToSite = objCustToSite.Get_All_By_siteid(objSite1.Siteid);


                        foreach (CustomerToSiteMapping objcts in colCustToSite)
                        {
                            Customer_mst objC = new Customer_mst();
                            int FlagStatus = 0;
                            objC = objC.Get_By_id(objcts.Custid);
                            if (Flagcount == 0)
                            {
                                colCtS.Add(objC);
                            }
                            else
                            {
                                foreach (Customer_mst objCus in colCtS)
                                {
                                    if (objC.Custid == objCus.Custid)
                                    {
                                        FlagStatus = 1;
                                    }

                               }
                                if (FlagStatus == 0)
                                {
                                    colCtS.Add(objC);
                                }

                            }
                            Flagcount = Flagcount + 1;
                        }
                    }
                }
            }
        }
        drpCustomer.DataTextField = "Customer_Name";
        drpCustomer.DataValueField = "CustId";
        drpCustomer.DataSource = colCtS;
        drpCustomer.DataBind();
        if (colCtS.Count == 0)
        {
            ListItem item = new ListItem();
            item.Text = "-------------Select-------------";
            item.Value = "0";
            drpCustomer.Items.Add(item);
        }

    }
    #region Functions to Bind Drop Down

    protected void BindDropRequestType()
    {
        colRequesttype = objRequestType.Get_All();
        drpRequestType.DataTextField = "requesttypename";
        drpRequestType.DataValueField = "requesttypeid";
        drpRequestType.DataSource = colRequesttype;
        drpRequestType.DataBind();
    
    }
    protected void BindDropPriority()
    {
        
            colPriority = objPriority.Get_All();
            drpPriority.DataTextField = "name";
            drpPriority.DataValueField = "priorityid";
            drpPriority.DataSource = colPriority;
            drpPriority.DataBind();
            ListItem item = new ListItem();
            item.Text = "-------------Select-------------";
            item.Value = "0";
            drpPriority.Items.Add(item);
            drpPriority.SelectedValue = "0";
        }
    
    protected void BindDropMode()
    {
        colMode = objMode.Get_All();
        drpMode.DataTextField = "modename";
        drpMode.DataValueField = "modeid";
        drpMode.DataSource = colMode;
        drpMode.DataBind();
        ListItem item = new ListItem();
        item.Text = "-------------Select-------------";
        item.Value = "0";
        drpMode.Items.Add(item);
        drpMode.SelectedValue = "0";

            
    }
    protected void BindDropStatus()
    {
        colStatus = objStatus.Get_All_By_OpenStatus();
        drpStatus.DataTextField = "statusname";
        drpStatus.DataValueField = "statusid";
        drpStatus.DataSource = colStatus;
        drpStatus.DataBind();
           
    }
    

    protected void BindDropSite()
    {
        string userName="";
        MembershipUser User = Membership.GetUser();
        if (User != null)
        {
            userName = User.UserName.ToString();
        }
       

        if (userName!="")
        {
            int userid;
            objOrganization = objOrganization.Get_Organization();
            objUser = objUser.Get_UserLogin_By_UserName(userName, objOrganization.Orgid);
            if (objUser.Userid !=0)
            {
                userid = objUser.Userid;
                colUserToSite = ObjUserToSite.Get_All_By_userid(userid);
                foreach (UserToSiteMapping obj in colUserToSite)
                {
                    int siteid;
                    Site_mst objSite1 = new Site_mst();
                    siteid = obj.Siteid;
                    objSite1 = objSite1.Get_By_id(siteid);
                    if (objSite1.Siteid!=0)
                    {
                        int custid = Convert.ToInt32(drpCustomer.SelectedValue);
                        int flag = objCustToSite.Get_By_Id(custid, objSite1.Siteid);
                        if (flag == 1)
                        {
                            colSite.Add(objSite1);
                        }
                        
                        
                    }
                
                }

            
            }
            drpSite.DataTextField = "sitename";
            drpSite.DataValueField = "siteid";
            drpSite.DataSource = colSite;
            drpSite.DataBind();
            ListItem item = new ListItem();
            item.Text = "-------------Select-------------";
            item.Value = "0";
            drpSite.Items.Add(item);
            drpSite.SelectedValue = "0";

        }
       

    
    
    }
    protected void BindDropTechnician()
    {
       
        objRole = objRole.Get_RoleInfo_By_RoleName("technician");
        if (objRole.Roleid != 0)
        {
            int siteid=0;
            int roleid;
            if (drpSite.SelectedValue != "") { siteid = Convert.ToInt32(drpSite.SelectedValue); }
           
            roleid = objRole.Roleid;
            if (siteid != 0 && roleid != 0)
            {
                colUser = objUser.Get_All_By_Role_Site(roleid, siteid);
                drpTechnician.DataTextField = "username";
                drpTechnician.DataValueField = "userid";
                drpTechnician.DataSource = colUser;
                drpTechnician.DataBind();
                ListItem item = new ListItem();
                item.Text = "-------------Select-------------";
                item.Value = "0";
                drpTechnician.Items.Add(item);
                drpTechnician.SelectedValue = "0";

            }
            else
            {

                colUser = objUser.Get_All_By_Role_Site(roleid, siteid);
                drpTechnician.DataTextField = "username";
                drpTechnician.DataValueField = "userid";
                drpTechnician.DataSource = colUser;
                drpTechnician.DataBind();
                ListItem item = new ListItem();
                item.Text = "-------------Select-------------";
                item.Value = "0";
                drpTechnician.Items.Add(item);
                drpTechnician.SelectedValue = "0";
            
            
            }
        
        }
     

    
    }
    protected void drpSite_SelectedIndexChanged(object sender, EventArgs e)
    { /////Add Exception handilng try catch change by vishal 21-05-2012
        try
        {
            BindDropTechnician();
            BindDropDept();
        }
        catch (Exception ex)
        {
            string myScript;
            myScript = "<script language=javascript>alert('Exception - '" + ex + "');</script>";
            Page.RegisterClientScriptBlock("MyScript", myScript);
            return;
        }

    }
    protected void BindDropDept()
    {
        int siteid=0;
        if (drpSite.SelectedValue!="")
        {
        siteid = Convert.ToInt32(drpSite.SelectedValue);
        colDept = objDept.Get_All_By_SiteId(siteid);
        drpDepartment.DataTextField = "departmentname";
        drpDepartment.DataValueField = "deptid";
        drpDepartment.DataSource = colDept;
        drpDepartment.DataBind();
        ListItem item = new ListItem();
        item.Text = "-------------Select-------------";
        item.Value = "0";
        drpDepartment.Items.Add(item);
        drpDepartment.SelectedValue = "0";
        }
    
    
    }
    protected void BindDropCategory()
    {
        colCategory = objCategory.Get_All();
        drpCategory.DataTextField = "categoryname";
        drpCategory.DataValueField = "categoryid";
        drpCategory.DataSource = colCategory;
        drpCategory.DataBind();
        ListItem item = new ListItem();
        item.Text = "-------------Select-------------";
        item.Value = "0";
        drpCategory.Items.Add(item);
        drpCategory.SelectedValue = "0";


    }
    #endregion

    protected void btnAdd_Click(object sender, EventArgs e)
    {

        /////Add Exception handilng try catch change by vishal 21-05-2012
        try
        {
            #region Declaration of Local Variables
            int siteid, priorityid;
            int SLAid = 0;
            int createdbyid = 0;
            int requesterid = 0;
            int FlagInsert;
            int requesttypeid;
            string userName;
            bool FlagUserStatus;
            FlagUserStatus = true;
            FlagInsert = 0;
            #endregion

            #region Fetch Current User
            MembershipUser User = Membership.GetUser();
            userName = User.UserName.ToString();
            #endregion
            #region Get Current Site and Priority id
            siteid = Convert.ToInt32(drpSite.SelectedValue);
            priorityid = Convert.ToInt32(drpPriority.SelectedValue);
            #endregion


            #region Get SLAid on the basis of siteid and Priority id

            if (siteid != 0 && priorityid != 0)
            {

                SLAid = objIncident.Get_By_SLAid(siteid, priorityid);
                requesttypeid = Convert.ToInt32(Resources.MessageResource.strRequestTypeId.ToString());
                if (requesttypeid == Convert.ToInt32(drpRequestType.SelectedValue))
                {
                    SLAid = 0;
                }
            }
            #endregion
            if (userName != "")
            {
                #region Find Userid of User who Created this Request
                objOrganization = objOrganization.Get_Organization();
                objUser = objUser.Get_UserLogin_By_UserName(userName, objOrganization.Orgid);
                if (objUser.Userid != 0)
                {
                    createdbyid = objUser.Userid;
                }

                #endregion



                #region Find Userid of User who is Requesting to log a call

                #region If User Already Exist
                if (Session["UserCreate"].ToString() == "Exist")
                {
                    objUser = objUser.Get_UserLogin_By_UserName_Ex(txtUsername.Text.ToString().Trim(), objOrganization.Orgid);
                    if (objUser.Userid != 0)
                    {
                        requesterid = objUser.Userid;
                    }
                }
                #endregion
                #region If New User is to be Created
                else if (Session["UserCreate"].ToString() == "create")
                {
                    string varEmail = "";
                    string varRoleName = Resources.MessageResource.BasicUserRole.ToString();
                    if (txtEmail.Text == "")
                    {
                        varEmail = Resources.MessageResource.errMemshipCreateUserEmail.ToString();
                    }
                    else
                    {
                        varEmail = txtEmail.Text;
                    }


                    int roleid = objRole.Get_By_RoleName(varRoleName);
                    int status;
                    objOrganization = objOrganization.Get_Organization();
                    UserLogin_mst objUserLogin = new UserLogin_mst();
                    objUserLogin.Username = txtUsername.Text.ToString();
                    objUserLogin.Password = Resources.MessageResource.strDefaultPassword.ToString();
                    objUserLogin.Roleid = roleid;
                    objUserLogin.Orgid = objOrganization.Orgid;
                    objUserLogin.ADEnable = false;
                    objUserLogin.Enable = true;
                    objUserLogin.Createdatetime = DateTime.Now.ToString();
                    status = objUserLogin.Insert();
                    if (status == 1)
                    {
                        // Create Mstatus field to send in Membership.CreateUser function as Out Variable for creating Membership User database
                        MembershipCreateStatus Mstatus = default(MembershipCreateStatus);
                        // Call Membership.CreateUser function to create Membership user 
                        Membership.CreateUser(txtUsername.Text.ToString().Trim(), Resources.MessageResource.strDefaultPassword.ToString(), varEmail, "Project Name", "Helpdesk", true, out Mstatus);
                        // Call Roles.AddUserToRole Function to Add User To Role
                        Roles.AddUserToRole(txtUsername.Text.ToString().Trim(), varRoleName);
                        // Declare Local Variable Userid to fetch userid of newly created user

                        // Create Object objUserLogin of UserLogin_mst()Class
                        objUserLogin = new UserLogin_mst();
                        // Fetch userid of Newly created user and assign to local variable userid by calling function objUserLogin.Get_By_UserName
                        requesterid = objUserLogin.Get_By_UserName(txtUsername.Text.ToString().Trim(), objOrganization.Orgid);
                        // If userid not equal to 0 then we get userid of Newly created user otherwise error Occured

                        ContactInfo_mst objContactInfo = new ContactInfo_mst();
                        objContactInfo.Userid = requesterid;
                        objContactInfo.Emailid = varEmail;
                        objContactInfo.Firstname = txtUsername.Text.ToString();
                        objContactInfo.Lastname = txtUsername.Text.ToString();
                        objContactInfo.Insert();
                        /// added by vishal 18-05-2012
                       Session["UserCreate"] = "";

                    }

                }
                #endregion
                #region If User is Not to be Created
                else if (Session["UserCreate"].ToString() == "notcreate")
                {
                    FlagUserStatus = false;

                }
                #endregion

                #endregion

            }

            objIncident.Title = txtTitle.Text.Trim();
            //objIncident.Title = drpTitle.SelectedItem.Text;
            // objIncident.Title = txtTitle.Text.Trim() + "-" + Txtextension.Text.Trim();
            //objIncident.Title = txtTitle.Text.Trim();
            if (Txtextension.Text != "")
            {
                string extension = Txtextension.Text;
                Int64 vOut = Convert.ToInt64(extension);
                //int vOut = Convert.ToInt32(extension);
                objIncident.Extension = vOut;
            }
            //start changed by prachi-19thmarch
            if (drpAMCcall.SelectedItem.Text == "NO")
            {
                objIncident.AMCCall = false;
            }
            else if (drpAMCcall.SelectedItem.Text == "YES")
            {
                objIncident.AMCCall = true;
            }
            //end
            objIncident.Slaid = SLAid;
            objIncident.Createdbyid = createdbyid;
            objIncident.Requesterid = requesterid;
            objIncident.Siteid = siteid;
            objIncident.Description = txtDescription.Text.ToString().Trim();
            objIncident.Deptid = Convert.ToInt32(drpDepartment.SelectedValue);
            objIncident.Createdatetime = DateTime.Now.ToString();
            objIncident.Modeid = Convert.ToInt32(drpMode.SelectedValue);
            //objIncident.ExternalTicketNo = txtExternalTicket.Text.ToString().Trim();
            if (FlagUserStatus == true)
            {
                FlagInsert = objIncident.Insert();

                #region Save Assetid and incident id in incidenttoassetmaaping

                // Get Asset and Incident Id for incidenttoassetmaaping
                objOrganization = objOrganization.Get_Organization();
                objUser = objUser.Get_UserLogin_By_UserName_Ex(txtUsername.Text.ToString().Trim(), objOrganization.Orgid);
                int userid = Convert.ToInt32(objUser.Userid);
                int tempuser1 = Convert.ToInt32(Session["tempuser1"]);
                if (tempuser1 == 1)
                {
                    assetid = Convert.ToInt32(Session["assetid"]);
                }
                else
                {
                    assetid = Convert.ToInt32(objusertoasset.Get_AssetId_By_UserId(userid));
                }
                int incid = Convert.ToInt32(objIncident.Get_TopIncidentId());
                if (txtassignasset.Text != "")
                {
                    //assetid = Convert.ToInt32(txtassignasset.Text);
                    if (assetid != 0)
                    {
                        objincidenttoasset.Insert(incid, assetid);
                        objusertoasset.Insert(userid, assetid, objUser.City, objUser.Company);
                        Session.Abandon();

                    }
                }

                #endregion

            }

            if (FlagInsert == 1)
            {
                int FlagIncdStatesInsert;
                int incidentid;
                incidentid = objIncident.Get_Current_Incidentid();
                objIncidentStates.Incidentid = incidentid;
                objIncidentStates.Priorityid = Convert.ToInt32(drpPriority.SelectedValue);
                objIncidentStates.Categoryid = Convert.ToInt32(drpCategory.SelectedValue);
                objIncidentStates.Statusid = Convert.ToInt32(drpStatus.SelectedValue);
                objIncidentStates.Subcategoryid = Convert.ToInt32(drpSubcategory.SelectedValue);
                objIncidentStates.Technicianid = Convert.ToInt32(drpTechnician.SelectedValue);
                if (Convert.ToInt32(drpTechnician.SelectedValue) != 0)
                {
                    objIncidentStates.AssignedTime = DateTime.Now.ToString();
                }
                objIncidentStates.Requesttypeid = Convert.ToInt32(drpRequestType.SelectedValue);
                FlagIncdStatesInsert = objIncidentStates.Insert();
                if (FlagIncdStatesInsert == 1)
                {

                    objIncidentHistory.Incidentid = incidentid;
                    objIncidentHistory.Operation = "create";
                    objIncidentHistory.Operationownerid = createdbyid;
                    objIncidentHistory.Insert();



                    objSentmailtoUser.SentmailUser(requesterid, incidentid, "open");
                    // change code/////////////////////////////////////////////////////////////////
                    if (FileUpload1.HasFile)
                    {
                        string filepath = Server.MapPath("~/FileAttach/");
                        string[] filenameupd = FileUpload1.FileName.Split(new char[] { '.' });
                        string filenew = Convert.ToString(incidentid) + "." + filenameupd[1];
                        FileUpload1.PostedFile.SaveAs(filepath + filenew);
                    }
                    ///////////////////////////////////////////////////////////////////////////////


                    if (Convert.ToInt32(drpTechnician.SelectedValue) != 0)
                    {

                        objSentmailtoUser.SentmailTechnician(Convert.ToInt32(drpTechnician.SelectedValue), incidentid);


                    }

                    Response.Redirect("~/Incident/IncidentRequestUpdate.aspx?incidentid=" + incidentid + "");

                }

            }
        }
        catch (Exception ex)
        {
            string myScript;
            myScript = "<script language=javascript>alert('Exception - '" + ex + "');</script>";
            Page.RegisterClientScriptBlock("MyScript", myScript);
            return;
        }



    }
    protected void drpCategory_SelectedIndexChanged(object sender, EventArgs e)
    { /////Add Exception handilng try catch change by vishal 21-05-2012
        try
        {
            BindDropSubCategory();
        }
        catch (Exception ex)
        {
            string myScript;
            myScript = "<script language=javascript>alert('Exception - '" + ex + "');</script>";
            Page.RegisterClientScriptBlock("MyScript", myScript);
            return;
        }
       
    }
    protected void BindDropSubCategory()
    {
        int categoryid = Convert.ToInt32(drpCategory.SelectedValue);
        
            colSubCategory = objSubCategory.Get_All_By_Categoryid(categoryid);
            drpSubcategory.DataTextField = "subcategoryname";
            drpSubcategory.DataValueField = "subcategoryid";
            drpSubcategory.DataSource = colSubCategory;
            drpSubcategory.DataBind();
            ListItem item = new ListItem();
            item.Text = "-------------Select-------------";
            item.Value = "0";
            drpSubcategory.Items.Add(item);
            drpSubcategory.SelectedValue = "0";
    
    }

    protected void txtUsername_TextChanged(object sender, EventArgs e)
    { /////Add Exception handilng try catch change by vishal 21-05-2012
        try
        {
            lblerrmsg.Text = "";
            BindDropSite();
            BindDropDept();


            #region Find Userid of User who is Requesting to log a call
            objOrganization = objOrganization.Get_Organization();
            objUser = objUser.Get_UserLogin_By_UserName_Ex(txtUsername.Text.ToString().Trim(), objOrganization.Orgid);
            int usrid = Convert.ToInt32(objUser.Userid);
            int tempuser = 0;
            if (objUser.Userid == 0)
            {
                string myScript;
                tempuser = 1;
                myScript = "<script language=javascript>function __doPostBack(eventTarget, eventArgument){var theForm = document.forms['aspnetForm'];  if (theForm){  theForm.__EVENTTARGET.value = eventTarget;theForm.__EVENTARGUMENT.value = eventArgument;theForm.submit(); }} var Flag= confirm('User Does not Exist,Do You Want to Create User');if(Flag==true){ __doPostBack('callPostBack', 'create');}if(Flag==false){ __doPostBack('callPostBack', 'notcreate');}</script>";
                Page.RegisterClientScriptBlock("MyScript", myScript);
                Session["tempuser"] = tempuser;
                txtassignasset.Text = "";
                BindDropPriority();



            }
            else
            {
                IncidentToAsset(usrid);
                ContactInfo_mst objConInfo = new ContactInfo_mst();
                objConInfo = objConInfo.Get_By_id(usrid);
                txtEmail.Text = objConInfo.Emailid;
                //txtassignasset.Text = assetid.ToString();
                txtassignasset.Text = compname;

                Session["assignassetid"] = assetid;
                Session["UserCreate"] = "Exist";
                //changed by shrikant

                colPriority = objPriority.Get_All();
                drpPriority.DataTextField = "name";
                drpPriority.DataValueField = "priorityid";
                drpPriority.DataSource = colPriority;
                drpPriority.DataBind();
                ListItem item = new ListItem();
                item.Text = "-------------Select-------------";
                if (objConInfo.userpriority == 1)
                {
                    item.Value = "0";
                    drpPriority.Items.Add(item);
                    drpPriority.SelectedValue = "8";
                }
                else
                {
                    item.Value = "0";
                    drpPriority.Items.Add(item);
                    drpPriority.SelectedValue = "0";
                }

                //end
            }
            string username1 = txtUsername.Text.ToString();
            Session["Username"] = username1;
            Session["Email"] = txtEmail.Text;
            // change by meenakshi





            #endregion
        }
        catch (Exception ex)
        {
            string myScript;
            myScript = "<script language=javascript>alert('Exception - '" + ex + "');</script>";
            Page.RegisterClientScriptBlock("MyScript", myScript);
            return;
        }
    }

    protected void IncidentToAsset(int userid)
    {
        
        objOrganization = objOrganization.Get_Organization();
        objUser = objUser.Get_UserLogin_By_UserName(txtUsername.Text.ToString().Trim(), objOrganization.Orgid);
        userid = Convert.ToInt32(objUser.Userid);
        assetid=Convert.ToInt32(objusertoasset.Get_AssetId_By_UserId(userid));
        objassetmst = objassetmst.Get_By_id(assetid);
        compname = objassetmst.Computername;

    }

    //protected void imgselectasset_click(object sender, ImageClickEventArgs e)
    //{  
    //    //if (txtUsername.Text == "")
    //    //{
    //    //    lblerrmsg.Text = "Enter the User name";
    //    //}
    //    //else if (txtassignasset.Text == "")
    //    //{
    //    //    string username = txtUsername.Text.ToString();
    //    //    Response.Redirect("~/Incident/SelectAsset.aspx?" + username + "");
    //    //}
    //    //else
    //    //{
    //    //    lblerrmsg.Text = "Asset already assigned";
    //    //}

    //}
    protected void btnReset_Click(object sender, EventArgs e)
    { /////Add Exception handilng try catch change by vishal 21-05-2012
        try
        {
            Session.Abandon();
            txtassignasset.Text = "";
            txtUsername.Text = "";
            lblerrmsg.Text = "";
            txtEmail.Text = "";
            BindDropSite();
            BindDropDept();
            BindDropCategory();
            BindDropStatus();
            BindDropMode();
            //BindDropPriority();
            BindDropTechnician();
            BindDropSubCategory();
            BindDropRequestType();
            //  BindTitle();
            txtTitle.Text = "";
            txtDescription.Text = "";
        }
        catch (Exception ex)
        {
            string myScript;
            myScript = "<script language=javascript>alert('Exception - '" + ex + "');</script>";
            Page.RegisterClientScriptBlock("MyScript", myScript);
            return;
        }
    }

    protected void drpCustomer_SelectedIndexChanged(object sender, EventArgs e)
    { /////Add Exception handilng try catch change by vishal 21-05-2012
        try
        {
            BindDropSite();
        }
        catch (Exception ex)
        {
            string myScript;
            myScript = "<script language=javascript>alert('Exception - '" + ex + "');</script>";
            Page.RegisterClientScriptBlock("MyScript", myScript);
            return;
        }
    }

    //protected void BindTitle()
    //{

    //    int categoryid = Convert.ToInt32(drpCategory.SelectedValue);
    //    int subcategoryid = Convert.ToInt32(drpSubcategory.SelectedValue);
    //    colTitle = objTitle.Get_All_By_Categoryid(categoryid,subcategoryid);
    //    drpTitle.DataTextField = "title";
    //    drpTitle.DataValueField = "title";
    //    drpTitle.DataSource = colTitle;
    //    drpTitle.DataBind();
    //    ListItem item = new ListItem();
    //    item.Text = "-----------------------------------------------Select-----------------------------------------------";
    //    item.Value = "0";
    //    drpTitle.Items.Add(item);
    
    
    //}
    protected void drpSubcategory_SelectedIndexChanged(object sender, EventArgs e)
    {
//        BindTitle();
    }
    protected int IsSLANeedsToBeStopped(string technician)
    {
        int technician1 = Convert.ToInt32(technician);
        int tech = objtechslastop.GetTechnicianId(technician1);
        return tech;
    }

    protected void drpAMCcall_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}
