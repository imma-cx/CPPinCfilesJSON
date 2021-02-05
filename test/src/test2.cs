using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.DataAccess.Client;
using System.Data;


public partial class AuditResultsPopUp : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string strHTML = string.Empty;
        string reci_seq = Request.QueryString["reci_seq"].ToString();
        string sdate = Request.QueryString["sdate"].ToString();
        string edate = Request.QueryString["edate"].ToString();
        strHTML = GetAuditResultdData(Convert.ToInt32(reci_seq), sdate, edate);
        Response.Clear();
        Response.ContentType = "application/vnd.ms-excel";
        Response.AddHeader("content-disposition", " filename=WEISExcelReport.xls");
	Response.Header.Set("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
        Response.Write(strHTML);
        Response.End();
    }
    #region [GetAuditResultdData]
    /// <Summary>
    /// <Description>This method will fetch Audit results data from the database.</Description>
    /// <MethodName>GetAuditResultdData</MethodName>
    /// <RetunType>Void</RetunType>
    /// <InputParams>
    /// <param1>int reci_seq</param1>
    /// <param1>string sdate</param1>
    /// <param1>string edate</param1>
    /// 
    /// </InputParams>
    /// </Summary>
    public string GetAuditResultdData(int reci_seq, string sdate, string edate)
    {
        DBResults objDb = new DBResults();
        WEIS.AuditResults objAuditResults = new WEIS.AuditResults();
        DataSet dsAuditResults = new DataSet();
        string abc = string.Empty;

        try
        {
            objDb = objAuditResults.GetAuditResultsData(reci_seq, sdate, edate);
            dsAuditResults = objDb.oraResultSet;

            //if (dsAuditResults.Tables.Count > 0)
            //{
            abc = PrepareHTML(dsAuditResults, sdate, edate);
            //}
            dsAuditResults.Dispose();
            dsAuditResults = null;
            objDb = null;
            objAuditResults = null;
        }
        catch (Exception ex)
        {
            Utilities.StopDisplayAndLogMessage(Resources.Resource.AuditResults_Title.ToString(),
                   Resources.Resource.AuditResults_GetDataError.ToString(), 1, ex.StackTrace.ToString(), WEIS.SessionManager.Current.UserID,
                   Resources.Resource.Error_Details.ToString() + ex.Message, "");
        }
        return abc;

    }
    #endregion
    #region [PrepareHTML]
    public string PrepareHTML(DataSet dsAuditResults, string sdate, string edate)
    {
        string abc = string.Empty;
        try
        {

            string strHeader = "";
            string atable = "";
            string strFooter = "";
            int i = 0;
            string strComments = "";
            string strcolor = "";

            DataTable dt1 = new DataTable();

            DataTable dt2 = new DataTable();

            dt1 = dsAuditResults.Tables[0];

            dt2 = dsAuditResults.Tables[1];

            strHeader = "";

            strHeader = "<table cellspacing = '1' cellpadding='1' border='1' name = 'results'>";
            if (dt1.Rows.Count > 0)
            {
                strHeader = strHeader + "<tr><td colspan =8>Audit Results for '" + dt1.Rows[0]["ri"].ToString() + "' from " + sdate + " to " + edate + "</td></tr>";
                strHeader = strHeader + "<tr ><td bgcolor=#FFFFCC><b>Time Period</b></td><td bgcolor=#FFFFCC><b>Value (" + dt1.Rows[0]["uom"].ToString() + ")</b>";
            }
            else // no rows
            {
                strHeader = strHeader + "<tr><td colspan =8>Audit Results for from " + sdate + " to " + edate + "</td></tr>";
                strHeader = strHeader + "<tr ><td bgcolor=#FFFFCC><b>Time Period</b></td><td bgcolor=#FFFFCC><b>Value (  )</b>";
            }
            strHeader = strHeader + "</td><td bgcolor=#FFFFCC><b>Comments</b></td><td bgcolor=#FFFFCC><b>Test Date</b></td><td bgcolor=#FFFFCC>";
            strHeader = strHeader + "<b>Status</b></td><td bgcolor=#FFFFCC><b>Audit User</b></td><td bgcolor=#FFFFCC><b>Audit Date</b></td><td bgcolor=#FFFFCC><b>Updated</b></td><td bgcolor=#FFFFCC><b>Update/Delete</b></td></tr>";

            if (dt2.Rows.Count > 0)
            {

                atable = "";
                for (i = 0; i < dt2.Rows.Count; i++)
                {
                    strComments = dt2.Rows[i]["CUR_COMMENTS"].ToString() != "" ? dt2.Rows[i]["CUR_COMMENTS"].ToString() : dt2.Rows[i]["DEL_COMMENTS"].ToString();
                    strcolor = strComments == "" ? "#CCFFCC" : "#FFCC99";

                    atable = atable + "<tr ><td bgcolor=#CCFFCC>" + dt2.Rows[i]["start_date"] + " -- <br>" + dt2.Rows[i]["end_date"] + "&nbsp;</td><td bgcolor=#CCFFCC>";
                    atable = atable + dt2.Rows[i]["value"] + "</td><td bgcolor=" + strcolor + ">" + strComments + "</td><td bgcolor=#CCFFCC>" + dt2.Rows[i]["created"];
                    atable = atable + "</td><td bgcolor=#CCFFCC>" + dt2.Rows[i]["status"] + "</td><td bgcolor=#CCFFCC>" + dt2.Rows[i]["audit_user"];
                    atable = atable + "</td><td bgcolor=#CCFFCC>" + dt2.Rows[i]["audit_date"] + "</td><td bgcolor=#CCFFCC>&nbsp;</td><td bgcolor=#CCFFCC>&nbsp;</td></tr>";

                    //'find all audit records for current result line 141 asp
                    // SQLStatement2 = "Select * from audit_results where rslt_seq='"&rslt_seq&"' order by datestamp desc"
                    atable = atable + getAuditRecordsForCurrentResults(dt2.Rows[i]["RSLT_SEQ"].ToString(), dt2.Rows[i]["audit_date"].ToString());
                    //between rs.movement and wend of asp page
                    atable = atable + "<tr><td colspan=9 bgcolor=#808080>&nbsp;</td></tr>";
                }

            }

            else
            {

                atable = atable + "<tr><td colspan = 9>No results found</td></tr>";

            }

            strFooter = strFooter + "<tr><td colspan=9 align=center><font color=red><b><i>~~ End of Audit Report ~~</b></i></font></td></tr></table>";


            dt1 = null;
            dt2 = null;
            abc = strHeader + atable + strFooter;
        }

        catch (Exception ex)
        {
            Utilities.StopDisplayAndLogMessage(Resources.Resource.AuditResults_Title.ToString(),
                  Resources.Resource.AuditResults_GetDataError.ToString(), 1, ex.StackTrace.ToString(), WEIS.SessionManager.Current.UserID,
                  Resources.Resource.Error_Details.ToString() + ex.Message, "");
        }
        return abc;
    }
    #endregion
    #region [getAuditRecordsForCurrentResults]
    //
    public string getAuditRecordsForCurrentResults(string RSLT_SEQ, string aDate)
    {
        string strAudit = string.Empty;
        DBResults objDb = new DBResults();
        WEIS.AuditResults objAuditResults = new WEIS.AuditResults();
        DataSet dsAuditResults = new DataSet();
        DataTable dt1 = new DataTable();
        int i = 0;
        string strDate1 = string.Empty;
        string strDate2 = string.Empty;
        try
        {
            objDb = objAuditResults.GetAuditRecordsForCurrentResults(RSLT_SEQ, aDate);
            dsAuditResults = objDb.oraResultSet;


            dt1 = dsAuditResults.Tables[0];

            if (dt1.Rows.Count > 0) // if not rs2.eof of ASP
            {
                for (i = 0; i < dt1.Rows.Count; i++) // while rs2.eof of ASP
                {
                    //calling database function to look up comments between current result and and most recent audited result
                    if (i == 0)
                    {
                        strAudit = strAudit + getCommentsForCurrentResults(RSLT_SEQ, aDate, dt1.Rows[i]["audit_date"].ToString());
                    }

                    strAudit = strAudit + "<tr><td>&nbsp;</td><td bgcolor=#99CCFF>";
                    strAudit = strAudit + dt1.Rows[i]["value"] + "</td><td bgcolor=#99CCFF>&nbsp;</td><td bgcolor=#99CCFF>";
                    strAudit = strAudit + dt1.Rows[i]["created"] + "</td><td bgcolor=#99CCFF>";
                    strAudit = strAudit + dt1.Rows[i]["status"] + "</td><td bgcolor=#99CCFF>";
                    strAudit = strAudit + dt1.Rows[i]["audit_user"] + "</td><td bgcolor=#99CCFF>";
                    strAudit = strAudit + dt1.Rows[i]["audit_date"] + "</td><td bgcolor=#99CCFF>";
                    strAudit = strAudit + dt1.Rows[i]["datestamp"] + "</td><td bgcolor=#99CCFF>";
                    strAudit = strAudit + dt1.Rows[i]["action"] + "</td></tr>";


                    //'save dates for looking up comments
                    strDate1 = dt1.Rows[i]["audit_date"].ToString();
                    if (i <= (dt1.Rows.Count))
                    {
                        strDate2 = dt1.Rows[i + 1]["audit_date"].ToString();
                    }
                    else
                    {
                        strDate2 = "1/1/1999 12:00:00 AM";
                    }
                    //'display all comments between current result and previous result

                    strAudit = strAudit + getCommentsForCurrentResults(RSLT_SEQ, strDate1, strDate2);
                }

            }
            else
            {
                //if there are no audit records, search for any comments saved before current result
                strAudit = strAudit + GetCommentsWhenNoAudit(RSLT_SEQ, aDate);

            }
            dt1 = null;
            dsAuditResults.Dispose();
            dsAuditResults = null;
            objDb = null;
            objAuditResults = null;
        }
        catch (Exception ex)
        {
            Utilities.StopDisplayAndLogMessage(Resources.Resource.AuditResults_Title.ToString(),
                   Resources.Resource.AuditResults_GetDataError.ToString(), 1, ex.StackTrace.ToString(), WEIS.SessionManager.Current.UserID,
                   Resources.Resource.Error_Details.ToString() + ex.Message, "");
        }
        return strAudit;
    }
    #endregion
    #region [getCommentsForCurrentResults]
    //look up comments between current result and and most recent audited result
    public string getCommentsForCurrentResults(string RSLT_SEQ, string fDate, string tDate)
    {
        string strCurrentComments = string.Empty;
        string strDeletedComments = string.Empty;
        string strComments = string.Empty;
        WEIS.AuditResults objAuditResults = new WEIS.AuditResults();


        try
        {
            strCurrentComments = objAuditResults.GetCommentsForCurrentResults(RSLT_SEQ, fDate, tDate, "NO");
            strDeletedComments = objAuditResults.GetCommentsForCurrentResults(RSLT_SEQ, fDate, tDate, "YES");
            if (strCurrentComments != "")
            {
                //calling database function to look up comments between current result and and most recent audited result
                strComments = strCurrentComments;
            }
            else if (strDeletedComments != "")
            {
                strComments = strDeletedComments;
            }
            objAuditResults = null;
        }
        catch (Exception ex)
        {
            Utilities.StopDisplayAndLogMessage(Resources.Resource.AuditResults_Title.ToString(),
                   Resources.Resource.AuditResults_GetDataError.ToString(), 1, ex.StackTrace.ToString(), WEIS.SessionManager.Current.UserID,
                   Resources.Resource.Error_Details.ToString() + ex.Message, "");
        }
        return strComments;
    }
    #endregion
    #region [GetCommentsWhenNoAudit]
    //if there are no audit records, search for any comments saved before current result
    public string GetCommentsWhenNoAudit(string RSLT_SEQ, string fDate)
    {
        string strComments = string.Empty;
        WEIS.AuditResults objAuditResults = new WEIS.AuditResults();
        try
        {
            strComments = objAuditResults.GetCommentsWhenNoAudit(RSLT_SEQ, fDate);
            objAuditResults = null;
        }
        catch (Exception ex)
        {
            Utilities.StopDisplayAndLogMessage(Resources.Resource.AuditResults_Title.ToString(),
                   Resources.Resource.AuditResults_GetDataError.ToString(), 1, ex.StackTrace.ToString(), WEIS.SessionManager.Current.UserID,
                   Resources.Resource.Error_Details.ToString() + ex.Message, "");
        }
        return strComments;
    }
    #endregion
}
