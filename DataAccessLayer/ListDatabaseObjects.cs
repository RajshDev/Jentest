using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Configuration;

namespace DataAccessLayer
{

    public class ListDatabaseObjects
    {
        public DataTable GetCanaraBankDetails(string Id)
        {
            DataTable dtColumns = new DataTable();
            try
            {

                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_CanaraBankBulkDetails where TxnRefNo ='" + Id + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    //  dataset.Tables.Add("Debting Account No");
                    //dataset.Tables.Add("12345678");
                    //dataset.Tables.Add("Ordering Customer Name");
                    //dataset.Tables.Add("ICSR");
                    //dataset.Tables.Add("Address");
                    //dataset.Tables.Add("IITM");
                    // dtColumns= dataset.Tables[0].TableName = "Debting Account No";
                    //dtColumns=dataset.Tables[0].DataSet.Tables.Add("Debting Account No");
                    dtColumns = dataset.Tables[0];

                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetReceiptDetails(string ProjectNo)
        {
            DataTable dtColumns = new DataTable();
            try
            {

                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_ReceiptDetails where ProjectNumber ='" + ProjectNo + "' and status <> 'InActive'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    //  dataset.Tables.Add("Debting Account No");
                    //dataset.Tables.Add("12345678");
                    //dataset.Tables.Add("Ordering Customer Name");
                    //dataset.Tables.Add("ICSR");
                    //dataset.Tables.Add("Address");
                    //dataset.Tables.Add("IITM");
                    // dtColumns= dataset.Tables[0].TableName = "Debting Account No";
                    //dtColumns=dataset.Tables[0].DataSet.Tables.Add("Debting Account No");
                    dtColumns = dataset.Tables[0];

                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private SqlConnection getConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
        }

        public DataTable getAllProperties(string tableName, string objType = "")
        {
            DataTable dtColumns = new DataTable();
            if (tableName != "" && tableName != null)
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT c.column_id as ID, c.name FROM " + connection.Database + ".sys.tables T " +
                            "INNER JOIN sys.columns C ON C.object_id = T.object_id " +
                            "WHERE T.name = '" + tableName + "' Order by c.name";
                    if (objType == "view")
                    {
                        command.CommandText = "SELECT c.column_id as ID, c.name FROM " + connection.Database + ".sys.views V " +
                                "INNER JOIN sys.columns C ON C.object_id = V.object_id " +
                                "WHERE V.name = '" + tableName + "' Order by c.name";
                    }

                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
            }

            return dtColumns;
        }

        public DataTable getAllTables()
        {

            DataTable dtTables = new DataTable();
            //using (var connection = new System.Data.SqlClient.SqlConnection("GBN_DB"))
            using (var connection = getConnection())
            {
                connection.Open();
                var command = new System.Data.SqlClient.SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT object_id as ID, name FROM " + connection.Database + ".sys.tables";

                var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                var dataset = new DataSet();
                adapter.Fill(dataset);
                dtTables = dataset.Tables[0];
            }

            return dtTables;

        }

        public DataTable getAllViews()
        {

            DataTable dtViews = new DataTable();
            using (var connection = getConnection())
            {
                connection.Open();
                var command = new System.Data.SqlClient.SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT name, object_id as ID FROM " + connection.Database + ".sys.views where name like '%vw%'";

                var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                var dataset = new DataSet();
                adapter.Fill(dataset);
                dtViews = dataset.Tables[0];
            }

            return dtViews;

        }

        public DataTable getAllTablesAndViews()
        {

            DataTable dtTables = new DataTable();
            //using (var connection = new System.Data.SqlClient.SqlConnection("GBN_DB"))
            using (var connection = getConnection())
            {
                connection.Open();
                var command = new System.Data.SqlClient.SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT object_id as ID, name FROM " + connection.Database + ".sys.tables Union ALL SELECT object_id as ID, name FROM " + connection.Database + ".sys.views";

                var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                var dataset = new DataSet();
                adapter.Fill(dataset);
                dtTables = dataset.Tables[0];
            }

            return dtTables;

        }
        public string getObjectType(string objectName)
        {

            string type = "";
            //using (var connection = new System.Data.SqlClient.SqlConnection("GBN_DB"))
            using (var connection = getConnection())
            {
                connection.Open();
                var command = new System.Data.SqlClient.SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "select type_desc  from " + connection.Database + ".sys.objects where name = '" + objectName + "'";
                using (SqlDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        type = rdr.GetString(0);
                    }
                }
            }

            return type;

        }

        public DataTable getReportView(string tableName)
        {

            DataTable dtViews = new DataTable();
            using (var connection = getConnection())
            {
                connection.Open();
                var command = new System.Data.SqlClient.SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT * FROM " + connection.Database + ".dbo." + tableName;

                var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                var dataset = new DataSet();
                adapter.Fill(dataset);
                dtViews = dataset.Tables[0];
            }

            return dtViews;

        }

        public DataTable getReportView(int ReportID, string whereClause, string user)
        {

            try
            {
                DataSet dsReport = getReportDetails(ReportID);
                DataTable dtViews = new DataTable();
                string query = "";
                string fields = "";
                string groupBy = "";
                string orderBy = "";
                DataTable dtReport = dsReport.Tables[0];
                DataTable dtFields = new DataTable();
                if (dsReport.Tables.Count > 0)
                {
                    dtFields = dsReport.Tables[1];
                    string[] AggColumnNames = (from dr in dtFields.Rows.Cast<DataRow>()
                                               where dr["Aggregation"].ToString() != ""
                                               select dr["ReportField"].ToString()).ToArray();
                    string[] groupByFields = (from dr in dtFields.Rows.Cast<DataRow>()
                                              where dr["Aggregation"].ToString() == ""
                                              select dr["ReportField"].ToString()).ToArray();

                    if (AggColumnNames != null && AggColumnNames.Length > 0 && groupBy == "")
                    {
                        for (int item = 0; item < groupByFields.Length; item++)
                        {
                            if (groupBy == "")
                            {
                                groupBy = "[" + groupByFields[item] + "]";
                            }
                            else
                            {
                                groupBy = groupBy + ", " + "[" + groupByFields[item] + "]";
                            }

                        }

                    }
                    for (int i = 0; i < dtFields.Rows.Count; i++)
                    {
                        string dtype = dtFields.Rows[i]["DType"].ToString();
                        string dateField = "CONVERT(varchar(12),[" + dtFields.Rows[i]["ReportField"].ToString() + "], 103) as " + dtFields.Rows[i]["ReportField"].ToString();
                        var aggregation = dtFields.Rows[i]["Aggregation"].ToString();
                        var reportField = (dtype == "datetime") ? dateField : dtFields.Rows[i]["ReportField"].ToString();
                        if (fields == "")
                        {
                            fields = (aggregation != "") ? aggregation + "([" + reportField + "]) AS " + "[" + reportField + "]" : reportField;
                            //fields = (aggregation != "") ? aggregation + "(" + reportField + ") as sumOf_"+ reportField : reportField;
                        }
                        else
                        {
                            //var strTemp = (aggregation != "") ? aggregation + "(" + reportField + ") as sumOf_" + reportField : reportField;
                            var strTemp = (aggregation != "") ? aggregation + "([" + reportField + "]) AS " + "[" + reportField + "]" : reportField;
                            fields = fields + ", " + strTemp;
                        }

                        if (AggColumnNames == null || AggColumnNames.Length == 0)
                        {
                            if (groupBy == "" && Convert.ToBoolean(dtFields.Rows[i]["GroupBy"]) == true)
                            {
                                groupBy = "[" + dtFields.Rows[i]["ReportField"].ToString() + "]";
                            }
                            else if (Convert.ToBoolean(dtFields.Rows[i]["GroupBy"]) == true)
                            {
                                groupBy = groupBy + "," + "[" + dtFields.Rows[i]["ReportField"].ToString() + "]";
                            }
                        }

                        if (orderBy == "" && Convert.ToBoolean(dtFields.Rows[i]["OrderBy"]) == true)
                        {
                            orderBy = "[" + dtFields.Rows[i]["ReportField"].ToString() + "]";
                        }
                        else if (Convert.ToBoolean(dtFields.Rows[i]["OrderBy"]) == true)
                        {
                            orderBy = orderBy + "," + "[" + dtFields.Rows[i]["ReportField"].ToString() + "]";
                        }
                    }

                }
                if (fields != "")
                {
                    query = "SELECT " + fields + " FROM " + dtReport.Rows[0]["TableName"].ToString();
                    if (whereClause != "")
                    {
                        query = query + " where " + whereClause;
                    }
                    if (groupBy != "")
                    {
                        query = query + " Group By " + groupBy;
                    }
                    if (orderBy != "")
                    {
                        query = query + " Order By " + orderBy;
                    }

                }
                if (query != "")
                {
                    using (var connection = getConnection())
                    {
                        connection.Open();
                        var command = new System.Data.SqlClient.SqlCommand();
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = query;

                        var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                        adapter.SelectCommand.CommandTimeout = 1800;
                        var dataset = new DataSet();
                        adapter.Fill(dataset);
                        dtViews = dataset.Tables[0];
                    }
                }
                return dtViews;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public Tuple<DataTable, int> pagingReportView(int ReportID, string whereClause, string user, int pageIndex, int pageSize)
        {

            try
            {
                DataSet dsReport = getReportDetails(ReportID);
                DataTable dtViews = new DataTable();
                string query = "";
                string countQuery = "";
                string fields = "";
                string groupBy = "";
                string orderBy = "";
                int skiprec = 0;
                int totalRecord = 0;
                if (pageIndex == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (pageIndex - 1) * pageSize;
                }
                DataTable dtReport = dsReport.Tables[0];
                DataTable dtFields = new DataTable();
                if (dsReport.Tables.Count > 0)
                {
                    dtFields = dsReport.Tables[1];
                    string[] AggColumnNames = (from dr in dtFields.Rows.Cast<DataRow>()
                                               where dr["Aggregation"].ToString() != ""
                                               select dr["ReportField"].ToString()).ToArray();
                    string[] groupByFields = (from dr in dtFields.Rows.Cast<DataRow>()
                                              where dr["Aggregation"].ToString() == ""
                                              select dr["ReportField"].ToString()).ToArray();

                    if (AggColumnNames != null && AggColumnNames.Length > 0 && groupBy == "")
                    {
                        for (int item = 0; item < groupByFields.Length; item++)
                        {
                            if (groupBy == "")
                            {
                                groupBy = "[" + groupByFields[item] + "]";
                            }
                            else
                            {
                                groupBy = groupBy + ", " + "[" + groupByFields[item] + "]";
                            }

                        }

                    }

                    for (int i = 0; i < dtFields.Rows.Count; i++)
                    {
                        string dtype = dtFields.Rows[i]["DType"].ToString();
                        string dateField = "CONVERT(varchar(12),[" + dtFields.Rows[i]["ReportField"].ToString() + "], 103) as " + dtFields.Rows[i]["ReportField"].ToString();
                        var aggregation = dtFields.Rows[i]["Aggregation"].ToString();
                        var reportField = (dtype == "datetime") ? dateField : dtFields.Rows[i]["ReportField"].ToString();
                        if (fields == "")
                        {
                            fields = (aggregation != "") ? aggregation + "([" + reportField + "]) AS " + "[" + reportField + "]" : reportField;
                            //fields = (aggregation != "") ? aggregation + "(" + reportField + ") as sumOf_"+ reportField : reportField;
                        }
                        else
                        {
                            //var strTemp = (aggregation != "") ? aggregation + "(" + reportField + ") as sumOf_" + reportField : reportField;
                            var strTemp = (aggregation != "") ? aggregation + "([" + reportField + "]) AS " + "[" + reportField + "]" : reportField;
                            fields = fields + ", " + strTemp;
                        }

                        if (AggColumnNames == null || AggColumnNames.Length == 0)
                        {
                            if (groupBy == "" && Convert.ToBoolean(dtFields.Rows[i]["GroupBy"]) == true)
                            {
                                groupBy = "[" + dtFields.Rows[i]["ReportField"].ToString() + "]";
                            }
                            else if (Convert.ToBoolean(dtFields.Rows[i]["GroupBy"]) == true)
                            {
                                groupBy = groupBy + "," + "[" + dtFields.Rows[i]["ReportField"].ToString() + "]";
                            }
                        }

                        if (orderBy == "" && Convert.ToBoolean(dtFields.Rows[i]["OrderBy"]) == true)
                        {
                            orderBy = "[" + dtFields.Rows[i]["ReportField"].ToString() + "]";
                        }
                        else if (Convert.ToBoolean(dtFields.Rows[i]["OrderBy"]) == true)
                        {
                            orderBy = orderBy + "," + "[" + dtFields.Rows[i]["ReportField"].ToString() + "]";
                        }
                    }

                }
                if (fields != "")
                {
                    query = "SELECT " + fields + " FROM " + dtReport.Rows[0]["TableName"].ToString();
                    countQuery = "SELECT count(*) FROM " + dtReport.Rows[0]["TableName"].ToString();
                    if (whereClause != "")
                    {
                        query = query + " where " + whereClause;
                        countQuery = countQuery + " where " + whereClause;
                    }
                    if (groupBy != "")
                    {
                        query = query + " Group By " + groupBy;
                        countQuery = countQuery + " Group By " + groupBy;
                    }
                    if (orderBy != "")
                    {
                        query = query + " Order By " + orderBy;
                    }
                    else if (whereClause == "")
                    {
                        query = query + " Order By (SELECT NULL)";
                    }
                    else
                    {
                        string fFieldName = dtFields.Rows[0]["ReportField"].ToString();
                        query = query + " Order By " + fFieldName;
                    }

                }
                if (query != "")
                {
                    using (var connection = getConnection())
                    {
                        connection.Open();
                        var command = new System.Data.SqlClient.SqlCommand();
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = query + " OFFSET " + skiprec + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";

                        var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                        adapter.SelectCommand.CommandTimeout = 1800;
                        var dataset = new DataSet();
                        adapter.Fill(dataset);
                        dtViews = dataset.Tables[0];

                        command.CommandText = countQuery;
                        using (SqlDataReader rdr = command.ExecuteReader())
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                totalRecord = rdr.GetInt32(0);
                            }
                        }
                    }
                }
                return Tuple.Create(dtViews, totalRecord);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public DataTable getFieldDetails(string TableName)
        {
            var connection = getConnection();
            SqlParameter[] ReportParam = new SqlParameter[1];
            ReportParam[0] = new SqlParameter("@TableName", SqlDbType.VarChar, 50);
            ReportParam[0].Value = TableName;
            try
            {
                return SqlHelper.ExecuteDataTable(connection, CommandType.StoredProcedure, "GetFieldDetails", ReportParam);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public DataSet getReportDetails(int reportId)
        {
            var connection = getConnection();
            SqlParameter[] ReportParam = new SqlParameter[1];
            ReportParam[0] = new SqlParameter("@ReportID", SqlDbType.Int, 50);
            ReportParam[0].Value = reportId;

            try
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "GetDynamicReport", ReportParam);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public DataTable getReportDetailsById(int reportId)
        {
            var connection = getConnection();
            SqlParameter[] ReportParam = new SqlParameter[1];
            ReportParam[0] = new SqlParameter("@ReportID", SqlDbType.Int, 50);
            ReportParam[0].Value = reportId;

            try
            {
                DataSet dsResult = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "GetDynamicReport", ReportParam);
                return dsResult.Tables[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public DataSet getReportByUser(int reportId, string user, string ReportName)
        {
            var connection = getConnection();
            SqlParameter[] ReportParam = new SqlParameter[3];
            ReportParam[0] = new SqlParameter("@ReportID", SqlDbType.Int, 50);
            ReportParam[0].Value = reportId;
            ReportParam[1] = new SqlParameter("@User", SqlDbType.VarChar, 150);
            ReportParam[1].Value = user;
            ReportParam[2] = new SqlParameter("@ReportName", SqlDbType.VarChar, 150);
            ReportParam[2].Value = ReportName;
            try
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "GetDynamicReportByUser", ReportParam);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public DataTable getFilterDetails(int reportId, string reportFieldName)
        {
            var connection = getConnection();
            SqlParameter[] ReportParam = new SqlParameter[2];
            ReportParam[0] = new SqlParameter("@ReportID", SqlDbType.Int, 50);
            ReportParam[0].Value = reportId;
            ReportParam[1] = new SqlParameter("@ReportField", SqlDbType.NVarChar, 150);
            ReportParam[1].Value = reportFieldName;
            try
            {
                return SqlHelper.ExecuteDataTable(connection, CommandType.StoredProcedure, "GetFilterFieldDetails", ReportParam);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public int AddReportDetails(ReportsProfileHandler report)
        {

            var connection = getConnection();
            SqlParameter[] ReportParam = new SqlParameter[19];
            ReportParam[0] = new SqlParameter("@ReportID", SqlDbType.Int, 50);
            ReportParam[0].Value = report.ReportID;
            ReportParam[0].Direction = ParameterDirection.InputOutput;
            ReportParam[1] = new SqlParameter("@ReportName", SqlDbType.VarChar, 150);
            ReportParam[1].Value = report.ReportName;
            ReportParam[2] = new SqlParameter("@Description", SqlDbType.VarChar, 300);
            ReportParam[2].Value = report.ReportDescription;
            ReportParam[3] = new SqlParameter("@TableName", SqlDbType.VarChar, 300);
            ReportParam[3].Value = report.TableName;
            ReportParam[4] = new SqlParameter("@Fields", SqlDbType.VarChar, 300);
            ReportParam[4].Value = (report.Fields != null && report.Fields != "") ? report.Fields : " ";
            ReportParam[5] = new SqlParameter("@GroupByFields", SqlDbType.VarChar, 300);
            ReportParam[5].Value = (report.GroupByFields != null && report.GroupByFields != "") ? report.GroupByFields : " ";
            ReportParam[6] = new SqlParameter("@OrderByFields", SqlDbType.VarChar, 300);
            ReportParam[6].Value = (report.OrderByFields != null && report.OrderByFields != "") ? report.OrderByFields : " ";
            ReportParam[7] = new SqlParameter("@IsActive", SqlDbType.Bit);
            ReportParam[7].Value = report.IsActive;
            ReportParam[8] = new SqlParameter("@UserId", SqlDbType.Int);
            ReportParam[8].Value = report.UserId;
            ReportParam[9] = new SqlParameter("@RoleId", SqlDbType.Int);
            ReportParam[9].Value = report.RoleId;
            ReportParam[10] = new SqlParameter("@ModuleId", SqlDbType.Int);
            ReportParam[10].Value = report.ModuleId;
            ReportParam[11] = new SqlParameter("@IsDeleted", SqlDbType.Bit);
            ReportParam[11].Value = report.IsDeleted;
            ReportParam[12] = new SqlParameter("@dtSummary", SqlDbType.Structured);
            ReportParam[12].Value = report.dtReportFields;
            ReportParam[13] = new SqlParameter("@dtFilter", SqlDbType.Structured);
            ReportParam[13].Value = report.dtFilterFields;
            ReportParam[14] = new SqlParameter("@dtRoles", SqlDbType.Structured);
            ReportParam[14].Value = report.dtRoles;
            ReportParam[15] = new SqlParameter("@canExport", SqlDbType.Bit);
            ReportParam[15].Value = report.CanExport;
            ReportParam[16] = new SqlParameter("@toPdf", SqlDbType.Bit);
            ReportParam[16].Value = report.ToPDF;
            ReportParam[17] = new SqlParameter("@toExcel", SqlDbType.Bit);
            ReportParam[17].Value = report.ToExcel;

            ReportParam[18] = new SqlParameter("@Status", SqlDbType.VarChar, 100);
            ReportParam[18].Direction = ParameterDirection.Output;
            ReportParam[18].Value = "";
            try
            {
                SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "DynamicReportsIU", ReportParam);
                return Convert.ToInt32(ReportParam[0].Value);
            }
            catch (Exception ex)
            {
                return -1;
            }

        }

        public DataTable GetFinalOutstandigReport(DateTime FromDate, DateTime ToDate)
        {

            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_VendorDetails where Date >='" + Frm + "' and Date <= '" + Todate + "' ";
                    //command.CommandText += "select a.VendorId,f.ProjectNumber,g.FirstName,b.Name,a.InvoiceDate,a.InvoiceNumber,a.InvoiceAmount,a.InvoiceTaxAmount,b.GSTExempted from tblBillEntry as a join tblVendorMaster as b on a.VendorId = b.VendorId join tblBillCommitmentDetail  as c on a.BillId = c.BillId join tblCommitmentDetails as d on c.CommitmentDetailId = d.ComitmentDetailId join tblCommitment as e on d.CommitmentId = e.CommitmentId join tblProject as f on e.ProjectId = f.ProjectId join vwFacultyStaffDetails as g on f.PIName = g.UserId where b.GSTIN Is Not Null and b.GSTExempted in(0,Null)";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                    dataset.Tables[0].TableName = "Vendor Invoice Details";

                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetProjectTransaction(int Id)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT IsNull(cast(ROW_NUMBER() OVER(PARTITION BY a.Category ORDER BY a.Category ASC) as int), 0) as pKey, a.Code,a.FunctionName,a.Category, RefNo,b.ProjectNumber,a.ProjectId, a.CommitmentNumber,Amount,DateOfTransaction,RunningTotal,TransType,SUM(RunningTotal) OVER(PARTITION BY a.ProjectId order by DateOfTransaction) AS RunningAmtTotal FROM vw_ProjectTransactionReport as a join tblProject as b on a.ProjectId = b.ProjectId where a.ProjectId = " + Id;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetCanaraBankForSalary(int Id)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_CanaraBankForsalary where SalaryId=" + Id;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetNonCanaraBankForSalary(int Id)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_NonCanaraBankForsalary where SalaryId=" + Id;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetOverHeadPosting(int Id)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    //var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    //var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_OverHeadsPaymentProcess where RefNumber in (select b.RefNo from tblOverHeadPaymentProcess as a join tblOverHeadPaymentProcessDetail as b on a.OverHeadPaymentProcessId = b.OverHeadPaymentProcessId where a.Status = 'Active' and a.OverHeadPaymentProcessId =" + Id + ")";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetPCFOverHeadPosting(int Id)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    //var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    //var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_PCFOverHeadsPayment where RefNumber in (select ReferenceNumber from tblPCFDistribution where Status = 'Completed' and Id = " + Id + ")";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetImprestRecoupment(string Id)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    //var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    //var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_ImprestRecoupment where TxnRefNo ='" + Id + "'";
                    //command.CommandText = "select * from vw_ImprestRecoupment where RefNumber in (select ImprestBillRecoupNumber from tblImprestBillRecoupment where Status = 'Completed' and ImprestBillRecoupId= " + Id + ")";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetContraBankAdvice(string Id)
        {
            DataTable dtColumns = new DataTable();
            try
            {

                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_ContraBankAdvice where TxnRefNo ='" + Id + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];

                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetDirectFundTransferBankAdvice(string Id)
        {
            DataTable dtColumns = new DataTable();
            try
            {

                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_DirectFundTransferBankAdvice where TxnRefNo ='" + Id + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];

                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetAdminSalary()
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_Adminsalary";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet GetAgencyReport(DateTime FromDate, DateTime ToDate, int Category)
        {
            DataSet dataset = new DataSet();
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    if (Category == 1)
                    {
                        command.CommandText = "select * from [vw_AgencyReport] where  InvoiceType ='Exempted'   and InvoiceDate >='" + Frm + "' and InvoiceDate <= '" + Todate + "' ";
                        command.CommandText += "select * from [vw_AgencyReport] where InvoiceType='Export' and InvoiceDate >='" + Frm + "' and InvoiceDate <= '" + Todate + "' ";
                        command.CommandText += "select * from [vw_AgencyReport] where InvoiceType in('Indian SEZ','Indian Non SEZ Registered') and( InvoiceDate >='" + Frm + "' and InvoiceDate <= '" + Todate + "' ) ";
                        command.CommandText += "select * from [vw_AgencyReport] where InvoiceType ='Indian Non SEZ Un Registered'  and (InvoiceDate >='" + Frm + "' and InvoiceDate <= '" + Todate + "' )";
                    }
                    if (Category == 2)
                    {
                        command.CommandText = "select * from [vw_AgencyReport] where InvoiceType ='Exempted'and ReceiptDate >='" + Frm + "' and ReceiptDate <= '" + Todate + "' ";
                        command.CommandText += "select * from [vw_AgencyReport] where InvoiceType='Export' and ReceiptDate >='" + Frm + "' and ReceiptDate <= '" + Todate + "' ";
                        command.CommandText += "select * from [vw_AgencyReport] where InvoiceType in('Indian SEZ','Indian Non SEZ Registered')  and(ReceiptDate >='" + Frm + "' and ReceiptDate <= '" + Todate + "') ";
                        command.CommandText += "select * from [vw_AgencyReport] where InvoiceType='Indian Non SEZ Un Registered'   and (ReceiptDate >='" + Frm + "' and ReceiptDate <= '" + Todate + "' ) ";
                    }
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                    dtColumns = dataset.Tables[1];
                    dtColumns = dataset.Tables[2];
                    dtColumns = dataset.Tables[3];
                    dataset.Tables[0].TableName = "Exempted";
                    dataset.Tables[1].TableName = "Export";
                    dataset.Tables[2].TableName = "Registered";
                    dataset.Tables[3].TableName = "UnRegistered";
                }
                return dataset;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetAnnualAccounts(DateTime Fromdate, DateTime ToDate)
        {
            ToDate = ToDate.AddDays(1).AddTicks(-10001);
            var Frm = Fromdate.ToString("yyyy-MM-dd HH:mm");
            var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
            var connection = getConnection();
            SqlParameter[] ReportParam = new SqlParameter[2];
            ReportParam[0] = new SqlParameter("@Date", SqlDbType.DateTime);
            ReportParam[0].Value = Frm;
            ReportParam[1] = new SqlParameter("@Date2", SqlDbType.DateTime);
            ReportParam[1].Value = Todate;
            try
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "AnnualAccounts", ReportParam);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public DataTable GetReceiptVoucher(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            ToDate = ToDate.AddDays(1).AddTicks(-10001);
            try
            {
                var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                var To = ToDate.ToString("yyyy-MM-dd HH:mm");
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_ReceiptOverHeadReport where Date >='" + Frm + "' and Date <= '" + To + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetGSTOffsetInput(DateTime FromDate, DateTime ToDate)
        {
            DataSet dataset = new DataSet();
            DataTable dtColumns = new DataTable();
            ToDate = ToDate.AddDays(1).AddTicks(-10001);
            var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
            var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_ITC where PostedDate <'" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                return dtColumns;
            }
        }
        public DataTable GetGSTOffsetTDSReceivable(DateTime FromDate, DateTime ToDate)
        {
            DataSet dataset = new DataSet();
            DataTable dtColumns = new DataTable();
            ToDate = ToDate.AddDays(1).AddTicks(-10001);
            var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
            var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_GSTTDS where PostedDate >='" + Frm + "' and PostedDate <= '" + Todate + "' ";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                return dtColumns;
            }
        }

        public DataSet GetOverhead()
        {

            var connection = getConnection();
            try
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "ProjectwiseOverhead");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public DataTable GetITTDSPayment(DateTime FromDate, DateTime ToDate, int BankId, int AccId)
        {
            DataSet dataset = new DataSet();
            DataTable dtColumns = new DataTable();
            ToDate = ToDate.AddDays(1).AddTicks(-10001);
            var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
            var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_TDSPaymentReport where  AccountHeadId=" + AccId + "  and  BankHeadId=" + BankId + " and PostedDate >='" + Frm + "' and PostedDate <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                return dtColumns;
            }
        }
        public DataTable GetGSTTDSPayment(DateTime FromDate, DateTime ToDate, int BankId)
        {
            DataSet dataset = new DataSet();
            DataTable dtColumns = new DataTable();
            ToDate = ToDate.AddDays(1).AddTicks(-10001);
            var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
            var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_GSTTDSPaymentReport where  BankHeadId=" + BankId + "  and PostedDate >='" + Frm + "' and PostedDate <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                return dtColumns;
            }
        }

        public DataSet GetOHReversal(int Id)
        {
            DataSet dataset = new DataSet();
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {

                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_OHReversalStaffWelfare where TxnRefNo in (select REPLACE(b.RefNo, '/', '') from tblOverHeadPaymentProcess as a join tblOverHeadPaymentProcessDetail as b on a.OverHeadPaymentProcessId = b.OverHeadPaymentProcessId where a.Status = 'Active' and a.OverHeadPaymentProcessId = " + Id + ")";
                    command.CommandText += "select * from vw_OHReversalRMF where TxnRefNo in (select REPLACE(b.RefNo, '/', '') from tblOverHeadPaymentProcess as a join tblOverHeadPaymentProcessDetail as b on a.OverHeadPaymentProcessId = b.OverHeadPaymentProcessId where a.Status = 'Active' and a.OverHeadPaymentProcessId = " + Id + ")";
                    command.CommandText += "select * from vw_OHReversalPCF where TxnRefNo in (select REPLACE(b.RefNo, '/', '') from tblOverHeadPaymentProcess as a join tblOverHeadPaymentProcessDetail as b on a.OverHeadPaymentProcessId = b.OverHeadPaymentProcessId where a.Status = 'Active' and a.OverHeadPaymentProcessId = " + Id + ")";
                    command.CommandText += "select * from vw_OHReversalDDF where TxnRefNo in (select REPLACE(b.RefNo, '/', '') from tblOverHeadPaymentProcess as a join tblOverHeadPaymentProcessDetail as b on a.OverHeadPaymentProcessId = b.OverHeadPaymentProcessId where a.Status = 'Active' and a.OverHeadPaymentProcessId = " + Id + ")";
                    command.CommandText += "select * from vw_OHReversalCorpus where TxnRefNo in (select REPLACE(b.RefNo, '/', '') from tblOverHeadPaymentProcess as a join tblOverHeadPaymentProcessDetail as b on a.OverHeadPaymentProcessId = b.OverHeadPaymentProcessId where a.Status = 'Active' and a.OverHeadPaymentProcessId = " + Id + ")";
                    command.CommandText += "select * from vw_OHReversalICSROH where TxnRefNo in (select REPLACE(b.RefNo, '/', '') from tblOverHeadPaymentProcess as a join tblOverHeadPaymentProcessDetail as b on a.OverHeadPaymentProcessId = b.OverHeadPaymentProcessId where a.Status = 'Active' and a.OverHeadPaymentProcessId = " + Id + ")";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                    dtColumns = dataset.Tables[1];
                    dtColumns = dataset.Tables[2];
                    dtColumns = dataset.Tables[3];
                    dtColumns = dataset.Tables[4];
                    dtColumns = dataset.Tables[5];
                    dataset.Tables[0].TableName = "StaffWelfare";
                    dataset.Tables[1].TableName = "RMF";
                    dataset.Tables[2].TableName = "PCF";
                    dataset.Tables[3].TableName = "DDF";
                    dataset.Tables[4].TableName = "Corpus";
                    dataset.Tables[5].TableName = "ICSROH";
                }
                return dataset;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetPrevNegativeBalance(string ProjectStr)
        {
            var connection = getConnection();
            //SqlParameter[] ReportParam = new SqlParameter[1];
            //ReportParam[0] = new SqlParameter("@ProjectStr", SqlDbType.VarChar, 8000);
            //ReportParam[0].Value = ProjectStr;
            try
            {
                //return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "PrevNegativeBalance", ReportParam);
                DataSet ds = new DataSet();
                using (SqlConnection conn = getConnection())
                {
                    SqlCommand sqlComm = new SqlCommand("PrevNegativeBalance", conn);
                    sqlComm.Parameters.AddWithValue("@ProjectStr", ProjectStr);
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    sqlComm.CommandTimeout = 1000;
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = sqlComm;
                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public DataTable GetTDSPayment(string Id)
        {
            DataSet dataset = new DataSet();
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_TDSPayment where TDSPaymentNumber ='" + Id + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                return dtColumns;
            }
        }

        public DataTable GetICSRB2B(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_ICSRB2B where InvoiceDate >='" + Frm + "' and InvoiceDate <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetICSRB2CL(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_ICSRB2CL where InvoiceDate >='" + Frm + "' and InvoiceDate <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetICSRB2CS(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_ICSRB2CS where InvoiceDate >='" + Frm + "' and InvoiceDate <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetExport(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_Export where InvoiceDate >='" + Frm + "' and InvoiceDate <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetExempted(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from Vw_Exempted where InvoiceDate >='" + Frm + "' and InvoiceDate <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetCNreg(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_CNreg where CreditNoteDate >='" + Frm + "' and CreditNoteDate <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetCNunreg(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_CNunreg where CreditNoteDate >='" + Frm + "' and CreditNoteDate <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetSACfinal(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select a.TaxCode as HSN,'' as Description,count(a.TaxCode) as TotalQuantity,sum(a.TotalInvoiceValue) as TotalValue,sum(a.TaxableValue) as TaxableValue,sum(b.IGSTAmount) as IntegratedTaxAmount,sum(b.CGSTAmount) as CentralTaxAmount,sum(b.SGSTAmount) as StateTaxAmount,'' as CessAmount from tblProjectInvoice as a join tblInvoiceTaxDetails as b on a.InvoiceId = b.InvoiceId join tblProject as c on a.ProjectId = c.ProjectId where c.ProjectType=2 and a.Status != 'InActive'  and a.TaxCode is not null and  a.InvoiceDate >='" + Frm + "' and a.InvoiceDate <= '" + Todate + "' group by a.TaxCode ";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetRCMReceivable(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    FromDate = FromDate.AddMonths(-1);
                    ToDate = ToDate.AddMonths(-1);
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_RCM where PostedDate >='" + Frm + "' and PostedDate <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetEligibleITC(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_ELIGIBLEITC where PostedDate >='" + Frm + "' and PostedDate <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetRCMPayable(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_RCM where PostedDate >='" + Frm + "' and PostedDate <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Tuple<decimal, decimal, decimal, decimal, decimal, decimal> GetB2B(DateTime FromDate, DateTime ToDate)
        {
            int TaxableId = 0; int IGSTId = 0; int SGSTId = 0; int CGSTId = 0;
            decimal Taxable = 0; decimal IGST = 0; decimal SGST = 0; decimal CGST = 0;
            decimal Export = 0; decimal Exempted = 0;
            try
            {
                ToDate = ToDate.AddDays(1).AddTicks(-10001);
                var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                using (var connection = getConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"select isnull(sum(Taxablevalue),0) as Taxablevalue,isnull(sum(CGSTAmount),0) as CGSTAmount, isnull(sum(SGSTAmount),0) as SGSTAmount,isnull(sum(IGSTAmount),0) as IGSTAmount  from vw_ICSRB2B where InvoiceDate >='" + Frm + "' and InvoiceDate <= '" + Todate + "'", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Don't assume we have any rows.
                            {
                                TaxableId = reader.GetOrdinal("Taxablevalue");
                                IGSTId = reader.GetOrdinal("IGSTAmount");
                                SGSTId = reader.GetOrdinal("SGSTAmount");
                                CGSTId = reader.GetOrdinal("CGSTAmount");
                                Taxable += reader.GetDecimal(0);
                                CGST += reader.GetDecimal(1);
                                SGST += reader.GetDecimal(2);
                                IGST += reader.GetDecimal(3);
                                //   return reader.GetString(ord); // Handles nulls and empty strings.
                            }
                        }
                    }
                    using (var command1 = new SqlCommand(@"select isnull(sum(Taxablevalue),0) as Taxablevalue,isnull(sum(CGSTAmount),0) as CGSTAmount, isnull(sum(SGSTAmount),0) as SGSTAmount,isnull(sum(IGSTAmount),0) as IGSTAmount  from vw_ICSRB2CL where InvoiceDate >='" + Frm + "' and InvoiceDate <= '" + Todate + "'", connection))
                    {
                        using (var reader1 = command1.ExecuteReader())
                        {
                            if (reader1.Read()) // Don't assume we have any rows.
                            {
                                Taxable += reader1.GetDecimal(0);
                                CGST += reader1.GetDecimal(1);
                                SGST += reader1.GetDecimal(2);
                                IGST += reader1.GetDecimal(3);
                            }
                        }
                    }
                    using (var command2 = new SqlCommand(@"select isnull(sum(Taxablevalue),0) as Taxablevalue,isnull(sum(CGSTAmount),0) as CGSTAmount, isnull(sum(SGSTAmount),0) as SGSTAmount,isnull(sum(IGSTAmount),0) as IGSTAmount  from vw_ICSRB2CS where InvoiceDate >='" + Frm + "' and InvoiceDate <= '" + Todate + "'", connection))
                    {
                        using (var reader2 = command2.ExecuteReader())
                        {
                            if (reader2.Read()) // Don't assume we have any rows.
                            {
                                Taxable += reader2.GetDecimal(0);
                                CGST += reader2.GetDecimal(1);
                                SGST += reader2.GetDecimal(2);
                                IGST += reader2.GetDecimal(3);
                                //   return reader.GetString(ord); // Handles nulls and empty strings.
                            }
                        }
                    }
                    using (var command3 = new SqlCommand(@"select isnull(sum(Taxablevalue),0) as Taxablevalue,isnull(sum(CGST),0) as CGSTAmount, isnull(sum(SGST),0) as SGSTAmount,isnull(sum(IGST),0) as IGSTAmount  from vw_CNreg  where CreditNoteDate >='" + Frm + "' and CreditNoteDate <= '" + Todate + "'", connection))
                    {
                        using (var reader3 = command3.ExecuteReader())
                        {
                            if (reader3.Read()) // Don't assume we have any rows.
                            {
                                Taxable -= reader3.GetDecimal(0);
                                CGST -= reader3.GetDecimal(1);
                                SGST -= reader3.GetDecimal(2);
                                IGST -= reader3.GetDecimal(3);
                                //   return reader.GetString(ord); // Handles nulls and empty strings.
                            }
                        }
                    }
                    using (var command4 = new SqlCommand(@"select isnull(sum(Taxablevalue),0) as Taxablevalue,isnull(sum(CGST),0) as CGSTAmount, isnull(sum(SGST),0) as SGSTAmount,isnull(sum(IGST),0) as IGSTAmount  from vw_CNunreg   where CreditNoteDate >='" + Frm + "' and CreditNoteDate <= '" + Todate + "'", connection))
                    {
                        using (var reader4 = command4.ExecuteReader())
                        {
                            if (reader4.Read()) // Don't assume we have any rows.
                            {
                                Taxable -= reader4.GetDecimal(0);
                                CGST -= reader4.GetDecimal(1);
                                SGST -= reader4.GetDecimal(2);
                                IGST -= reader4.GetDecimal(3);
                                //   return reader.GetString(ord); // Handles nulls and empty strings.
                            }
                        }
                    }
                    using (var command5 = new SqlCommand(@"select isnull(sum(Taxablevalue),0) as Taxablevalue from vw_Export  where InvoiceDate >='" + Frm + "' and InvoiceDate <= '" + Todate + "'", connection))
                    {
                        using (var reader5 = command5.ExecuteReader())
                        {
                            if (reader5.Read()) // Don't assume we have any rows.
                            {
                                Export = reader5.GetDecimal(0);
                            }
                        }
                    }
                    using (var command6 = new SqlCommand(@"select isnull(sum(Exempted),0) as Taxablevalue from Vw_Exempted  where InvoiceDate >='" + Frm + "' and InvoiceDate <= '" + Todate + "'", connection))
                    {
                        using (var reader6 = command6.ExecuteReader())
                        {
                            if (reader6.Read()) // Don't assume we have any rows.
                            {
                                Exempted = reader6.GetDecimal(0);
                            }
                        }
                    }
                }
                return Tuple.Create(Taxable, IGST, CGST, SGST, Export, Exempted);
            }
            catch (Exception ex)
            {
                return Tuple.Create((decimal)0, (decimal)0, (decimal)0, (decimal)0, (decimal)0, (decimal)0);
            }
        }
        public Tuple<decimal, decimal, decimal, decimal, decimal, decimal, decimal> GetRCM(DateTime FromDate, DateTime ToDate)
        {
            int SGSTId = 0; int CGSTId = 0;
            decimal SGST = 0; decimal CGST = 0; decimal RecSGST = 0; decimal RecCGST = 0;
            decimal ITCSGST = 0; decimal ITCCGST = 0; decimal ITCIGST = 0;
            try
            {
                ToDate = ToDate.AddDays(1).AddTicks(-10001);
                var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                using (var connection = getConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"select isnull(sum(CGST),0) as CGSTAmount, isnull(sum(SGST),0) as SGSTAmount from vw_RCM where PostedDate >='" + Frm + "' and PostedDate <= '" + Todate + "'", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Don't assume we have any rows.
                            {
                                CGST = reader.GetDecimal(0);
                                SGST = reader.GetDecimal(1);

                            }
                        }
                    }
                    DateTime From = FromDate.AddMonths(-1);
                    DateTime To = ToDate.AddMonths(-1);
                    var Fr = From.ToString("yyyy-MM-dd HH:mm");
                    var ToD = To.ToString("yyyy-MM-dd HH:mm");
                    using (var command1 = new SqlCommand(@"select isnull(sum(CGST),0) as CGSTAmount, isnull(sum(SGST),0) as SGSTAmount from vw_RCM where PostedDate >='" + Fr + "' and PostedDate <= '" + ToD + "'", connection))
                    {
                        using (var reader1 = command1.ExecuteReader())
                        {
                            if (reader1.Read()) // Don't assume we have any rows.
                            {
                                RecCGST = reader1.GetDecimal(0);
                                RecSGST = reader1.GetDecimal(1);
                            }
                        }
                    }
                    using (var command2 = new SqlCommand(@"select isnull(sum(CGST),0) as CGSTAmount, isnull(sum(SGST),0) as SGSTAmount , isnull(sum(IGST),0) as IGSTAmount from vw_ELIGIBLEITC where PostedDate >='" + Frm + "' and PostedDate <= '" + Todate + "'", connection))
                    {
                        using (var reader2 = command2.ExecuteReader())
                        {
                            if (reader2.Read()) // Don't assume we have any rows.
                            {
                                ITCCGST = reader2.GetDecimal(0);
                                ITCSGST = reader2.GetDecimal(1);
                                ITCIGST = reader2.GetDecimal(2);
                            }
                        }
                    }
                    return Tuple.Create(CGST, SGST, RecCGST, RecSGST, ITCIGST, ITCCGST, ITCSGST);
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create((decimal)0, (decimal)0, (decimal)0, (decimal)0, (decimal)0, (decimal)0, (decimal)0);
            }
        }
        public Tuple<decimal, decimal, decimal, decimal> GetTDSRece(DateTime FromDate, DateTime ToDate)
        {

            decimal SGST = 0; decimal CGST = 0; decimal IGST = 0; decimal Taxable = 0;
            try
            {
                ToDate = ToDate.AddDays(1).AddTicks(-10001);
                var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                using (var connection = getConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"select isnull(sum(TaxableValue),0) as TaxableValue,isnull(sum(CGSTTDSReceivable),0) as CGSTAmount, isnull(sum(SGSTTDSReceivable),0) as SGSTAmount,isnull(sum(IGSTTDSReceivable),0) as IGST from vw_GSTTDS where PostedDate >='" + Frm + "' and PostedDate <= '" + Todate + "'", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Don't assume we have any rows.
                            {
                                Taxable = reader.GetDecimal(0);
                                CGST = reader.GetDecimal(1);
                                SGST = reader.GetDecimal(2);
                                IGST = reader.GetDecimal(3);

                            }
                        }
                    }
                    return Tuple.Create(Taxable, IGST, CGST, SGST);
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create((decimal)0, (decimal)0, (decimal)0, (decimal)0);
            }
        }

        public DataTable GetInvoicenonTN(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select PlaceOfSupply,sum(TaxableValue) as TaxableValue,sum(IGSTAmount) as IGSTAmount from vw_InvoicenonTN where InvoiceDate >='" + Frm + "' and InvoiceDate <= '" + Todate + "' group by PlaceOfSupply";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetProjectSchemeBalance(DateTime ToDate)
        {

            var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
            var connection = getConnection();
            SqlParameter[] ReportParam = new SqlParameter[1];
            ReportParam[0] = new SqlParameter("@Date", SqlDbType.DateTime);
            ReportParam[0].Value = Todate;
            try
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "SchemeCodeDetails", ReportParam);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public DataSet GetProjectScheme(DateTime Fromdate, DateTime ToDate)
        {

            ToDate = ToDate.AddDays(1).AddTicks(-10001);
            var Frm = Fromdate.ToString("yyyy-MM-dd HH:mm");
            var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
            var connection = getConnection();
            SqlParameter[] ReportParam = new SqlParameter[2];
            ReportParam[0] = new SqlParameter("@Date", SqlDbType.DateTime);
            ReportParam[0].Value = Frm;
            ReportParam[1] = new SqlParameter("@Date2", SqlDbType.DateTime);
            ReportParam[1].Value = Todate;
            try
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "ProjectSchemes", ReportParam);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public DataTable GetConsReceiptNIRF(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select ProjectNumber,Department,PIName,CoPi,Agency,ProjectTitle,Sum(Amount) as Amount,Type,ReceiptNumber from vw_ConsReceiptsNIRF where ReceiptDate >='" + Frm + "' and ReceiptDate <= '" + Todate + "' group  by ProjectNumber,Department,PIName,CoPi,Agency,ProjectTitle,Type ,ReceiptNumber order by  Type asc";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetSponReceiptNIRF(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select ProjectNumber,Department,PIName,CoPi,Agency,SanctionOrderDate,SanctionOrderNumber,ProjectTitle,ProjectType,Type,Sum(Amount) as Amount,ReceiptNumber from vw_SponReceiptsNIRF where ReceiptDate >='" + Frm + "' and ReceiptDate <= '" + Todate + "' group  by ProjectNumber,Department,PIName,CoPi,Agency,SanctionOrderDate,SanctionOrderNumber,ProjectTitle,ProjectType,Type,ReceiptNumber";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetConsReceiptNONNIRF(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select ProjectNumber,Department,PIName,CoPi,Agency,ProjectTitle,Sum(Amount) as Amount,Type,ReceiptNumber from vw_ConsReceiptsNONNIRF where ReceiptDate >='" + Frm + "' and ReceiptDate <= '" + Todate + "' group  by ProjectNumber,Department,PIName,CoPi,Agency,ProjectTitle,Type,ReceiptNumber  order by  Type asc";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetSponReceiptNONNIRF(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select ProjectNumber,Department,PIName,CoPi,Agency,SanctionOrderDate,SanctionOrderNumber,ProjectTitle,ProjectType,Type,Sum(Amount) as Amount,ReceiptNumber from vw_SponReceiptsNONNIRF where ReceiptDate >='" + Frm + "' and ReceiptDate <= '" + Todate + "' group  by ProjectNumber,Department,PIName,CoPi,Agency,SanctionOrderDate,SanctionOrderNumber,ProjectTitle,ProjectType,Type,ReceiptNumber";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetCapexList(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select BillNumber,ProjectNumber,ProjectType,CommitmentNumber,HeadName,AmountSpent,CommitmentDate as Date from vw_ProjectExpenditureReport where CommitmentDate >='" + Frm + "' and CommitmentDate <= '" + Todate + "' and AllocationHeadId in (5,7) and Posted_f=1";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetOpexList(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select BillNumber,ProjectNumber,ProjectType,CommitmentNumber,HeadName,Amount, Date from vw_NIRFopexlist where Date >='" + Frm + "' and Date <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.SelectCommand.CommandTimeout = 1800;
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetCapex(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select HeadName,sum(Spon) as Spon,sum(Cons) as Cons,sum(ICSROH) as ICSROH,sum(PCF) as PCF,sum(RMF) as RMF,(sum(Spon)+sum(Cons)+sum(ICSROH)+sum(PCF)+sum(RMF) ) as Total from vw_CopexNIRF where CommitmentDate >='" + Frm + "' and CommitmentDate <= '" + Todate + "' group by HeadName";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetOpex(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select HeadName,sum(Spon) as Spon,sum(Cons) as Cons,sum(ICSROH) as ICSROH,sum(PCF) as PCF,sum(RMF) as RMF,(sum(Spon)+sum(Cons)+sum(ICSROH)+sum(PCF)+sum(RMF) ) as Total from vw_opexNIRF where Date >='" + Frm + "' and Date <= '" + Todate + "' group by HeadName";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetReceiptsSummaryNIRF(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select Department,sum(IC) as IC,sum(RB) as RB,sum(RC) as RC,sum(TT) as TT,sum(ET) as ET,sum(IT) as IT,sum(CR) as CR,sum(CN) as CN,sum(CT) as CT,(sum(IC) + sum(RB) + sum(RC) + sum(TT) + sum(ET) + sum(IT) +sum(CR) + sum(CN) + sum(CT)) as ConsTotal,sum(PFMS) as PFMS,sum(NONPFMS) as NONPFMS,sum(Imprints) as Imprints,sum(UAY) as UAY,(sum(PFMS) + sum(NONPFMS) + sum(Imprints)  + sum(UAY)) as SponTotal,(sum(IC) + sum(RB) + sum(RC) + sum(TT) + sum(ET) + sum(IT) +sum(CR) + sum(CN) + sum(CT) + sum(PFMS) + sum(NONPFMS) + sum(Imprints) + sum(UAY)) as GrandTotal from  vw_NIRFcons where ReceiptDate >='" + Frm + "' and ReceiptDate <= '" + Todate + "' group by Department";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetReceiptsSummaryTotal(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select sum(IC) as IC,sum(RB) as RB,sum(RC) as RC,sum(TT) as TT,sum(ET) as ET,sum(IT) as IT,sum(CR) as CR,sum(CN) as CN,sum(CT) as CT,(sum(IC) + sum(RB) + sum(RC) + sum(TT) + sum(ET) + sum(IT) +sum(CR) + sum(CN) + sum(CT)) as ConsTotal,sum(PFMS) as PFMS,sum(NONPFMS) as NONPFMS,sum(Imprints) as Imprints,sum(UAY) as UAY,(sum(PFMS) + sum(NONPFMS) + sum(Imprints) +  sum(UAY)) as SponTotal,(sum(IC) + sum(RB) + sum(RC) + sum(TT) + sum(ET) + sum(IT) +sum(CR) + sum(CN) + sum(CT) + sum(PFMS) + sum(NONPFMS) + sum(Imprints) + sum(UAY)) as GrandTotal from  vw_NIRFcons where ReceiptDate >='" + Frm + "' and ReceiptDate <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetReceiptTransferList(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vwReceipttransfer where ReceiptDate >='" + Frm + "' and ReceiptDate <= '" + Todate + "' and ProjectBank != FundReceivedBank";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetReceiptTransfer(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select FROMBANK,TOBANK,sum(Amount) as AMOUNT from vw_Receipttransfer where ReceiptDate >='" + Frm + "' and ReceiptDate <= '" + Todate + "' group by FROMBANK,TOBANK";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                    DataView dv1 = dtColumns.DefaultView;
                    dv1.RowFilter = " AMOUNT >0";
                    dtColumns = dv1.ToTable();
                    // dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Tuple<decimal, decimal, decimal, decimal> GetLedgerBalAmt(int Accid, DateTime FromDate, DateTime ToDate)
        {

            decimal CurrDebit = 0; decimal CurrCredit = 0; decimal PrevDebit = 0; decimal PrevCredit = 0;

            try
            {
                ToDate = ToDate.AddDays(1).AddTicks(-10001);
                var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                using (var connection = getConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"select isnull(sum(Amount),0) from vw_LeadgerBalanceReport where AccountHeadId=" + Accid + "  and TransactionType='Credit' and  PostedDate >='" + Frm + "' and PostedDate <= '" + Todate + "'", connection))
                    {
                        command.CommandTimeout = 180;
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Don't assume we have any rows.
                            {
                                CurrCredit = reader.GetDecimal(0);
                            }
                        }
                    }

                    using (var command1 = new SqlCommand(@"select isnull(sum(Amount),0) from vw_LeadgerBalanceReport where AccountHeadId=" + Accid + "  and TransactionType='Debit' and  PostedDate >='" + Frm + "' and PostedDate <= '" + Todate + "'", connection))
                    {
                        command1.CommandTimeout = 180;
                        using (var reader1 = command1.ExecuteReader())
                        {
                            if (reader1.Read()) // Don't assume we have any rows.
                            {
                                CurrDebit = reader1.GetDecimal(0);
                            }
                        }
                    }
                    using (var command2 = new SqlCommand(@"select isnull(sum(Amount),0) from vw_LeadgerBalanceReport where AccountHeadId=" + Accid + "  and TransactionType='Credit' and  PostedDate <'" + Frm + "'", connection))
                    {
                        command2.CommandTimeout = 180;
                        using (var reader2 = command2.ExecuteReader())
                        {
                            if (reader2.Read()) // Don't assume we have any rows.
                            {
                                PrevCredit = reader2.GetDecimal(0);
                            }
                        }
                    }
                    using (var command3 = new SqlCommand(@"select isnull(sum(Amount),0) from vw_LeadgerBalanceReport where AccountHeadId=" + Accid + "  and TransactionType='Debit' and  PostedDate <'" + Frm + "'", connection))
                    {
                        command3.CommandTimeout = 180;
                        using (var reader3 = command3.ExecuteReader())
                        {
                            if (reader3.Read()) // Don't assume we have any rows.
                            {
                                PrevDebit = reader3.GetDecimal(0);
                            }
                        }
                    }
                    //using (var command4 = new SqlCommand(@"select isnull(sum(OpeningBalance),0) from tblHeadOpeningBalance where AccountHeadId=" + Accid + "  and TransactionType='Debit'", connection))
                    //{
                    //    using (var reader4 = command4.ExecuteReader())
                    //    {
                    //        if (reader4.Read()) // Don't assume we have any rows.
                    //        {
                    //            PrevDebit = PrevDebit + reader4.GetDecimal(0);
                    //        }
                    //    }
                    //}
                    //using (var command5 = new SqlCommand(@"select isnull(sum(OpeningBalance),0) from tblHeadOpeningBalance where AccountHeadId=" + Accid + "  and TransactionType='Credit'", connection))
                    //{
                    //    using (var reader5 = command5.ExecuteReader())
                    //    {
                    //        if (reader5.Read()) // Don't assume we have any rows.
                    //        {
                    //            PrevCredit = PrevCredit + reader5.GetDecimal(0);
                    //        }
                    //    }
                    //}
                    return Tuple.Create(CurrCredit, CurrDebit, PrevCredit, PrevDebit);
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create((decimal)0, (decimal)0, (decimal)0, (decimal)0);
            }
        }
        public DataTable GetLedgerBalDebit(int Accid, DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select PostedDate,FunctionName,RefNumber,Amount,Remarks from vw_LeadgerBalanceReport where  TransactionType ='Debit' and PostedDate >='" + Frm + "' and PostedDate <= '" + Todate + "' and AccountHeadId = " + Accid;
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetLedgerBalCredit(int Accid, DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select PostedDate,FunctionName,RefNumber,Amount,Remarks from vw_LeadgerBalanceReport where  TransactionType ='Credit' and PostedDate >='" + Frm + "' and PostedDate <= '" + Todate + "' and AccountHeadId = " + Accid;
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetProjectTransactionPayment(int ProjectId)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {

                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select  *  from vw_BOAExpenditure where Projectid =" + ProjectId;
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetProjectTransactionReceipt(int ProjectId)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {

                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select  ReceiptType,ReceiptNumber,InvoiceNumber,(ReceiptAmount - (isnull(IGST,0)+isnull(SGST,0)+isnull(CGST,0))) as Amount,AgencyName,Date,Description as Remarks  from vw_ReceiptReport where status= 'Completed' and Projectid =" + ProjectId;
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Tuple<decimal> GetBalanceSheetApp(int Accid, DateTime FromDate, DateTime ToDate)
        {
            decimal Amt = 0; string Head = "";
            try
            {

                ToDate = ToDate.AddDays(1).AddTicks(-10001);
                var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                using (var connection = getConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"select isnull((sum(Debit)-sum(Credit)),0) as Amount from vw_BalanceSheet  where AccountGroupId=" + Accid + " and PostedDate >='" + Frm + "' and PostedDate <= '" + Todate + "' ", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Don't assume we have any rows.
                            {

                                Amt = reader.GetDecimal(0);
                            }
                        }
                    }
                    return Tuple.Create(Amt);
                }

            }
            catch (Exception ex)
            {
                return Tuple.Create(Amt);
            }
        }
        public Tuple<decimal> GetBalanceSheetSource(int Accid, DateTime FromDate, DateTime ToDate)
        {
            decimal Amt = 0; string Head = "";
            try
            {

                ToDate = ToDate.AddDays(1).AddTicks(-10001);
                var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                using (var connection = getConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"select isnull((sum(Credit)-sum(Debit)),0) as Amount from vw_BalanceSheet  where AccountGroupId=" + Accid + " and PostedDate >='" + Frm + "' and PostedDate <= '" + Todate + "'", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Don't assume we have any rows.
                            {

                                Amt = reader.GetDecimal(0);
                            }
                        }
                    }
                    return Tuple.Create(Amt);
                }

            }
            catch (Exception ex)
            {
                return Tuple.Create(Amt);
            }
        }
        public Tuple<decimal> GetBalanceSheetReceipt(int Accid, DateTime FromDate, DateTime ToDate)
        {
            decimal Amt = 0; string Head = "";
            try
            {
                ToDate = ToDate.AddDays(1).AddTicks(-10001);
                var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                using (var connection = getConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"select isnull(sum(Amount),0) as Amount from vw_BalancesheetReceipt  where Receiptid=" + Accid + " and ReceiptDate >='" + Frm + "' and ReceiptDate <= '" + Todate + "'", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Don't assume we have any rows.
                            {

                                Amt = reader.GetDecimal(0);
                            }
                        }
                    }
                    return Tuple.Create(Amt);
                }

            }
            catch (Exception ex)
            {
                return Tuple.Create(Amt);
            }
        }
        public Tuple<decimal, decimal> GetBalanceSheetEXP(int Head, DateTime FromDate, DateTime ToDate)
        {
            decimal Amt = 0; decimal Misc = 0;
            try
            {

                ToDate = ToDate.AddDays(1).AddTicks(-10001);
                var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                using (var connection = getConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"select isnull(sum(AmountSpent),0) as Amount from dbo.vw_ProjectExpenditureReport where AllocationHeadId=" + Head + " and CommitmentDate >='" + Frm + "' and CommitmentDate <= '" + Todate + "'  ", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Don't assume we have any rows.
                            {

                                Amt = reader.GetDecimal(0);
                            }
                        }
                    }
                    using (var command1 = new SqlCommand(@"select isnull(sum(AmountSpent),0) as Amount from vw_ProjectExpenditureReport where AllocationHeadId not in(1,2,3,4,5,6,7,8) and  CommitmentDate >='" + Frm + "' and CommitmentDate <= '" + Todate + "'", connection))
                    {
                        using (var reader1 = command1.ExecuteReader())
                        {
                            if (reader1.Read()) // Don't assume we have any rows.
                            {
                                Misc = reader1.GetDecimal(0);
                            }
                        }
                    }
                    return Tuple.Create(Amt, Misc);
                }

            }
            catch (Exception ex)
            {
                return Tuple.Create(Amt, Misc);
            }
        }
        public DataTable GetBalanceSheetMisc(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select BillNumber,ProjectNumber,HeadName,AmountSpent,CommitmentDate from vw_ProjectExpenditureReport where AllocationHeadId not in(1,2,3,4,5,6,7,8) and  CommitmentDate >='" + Frm + "' and CommitmentDate <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Tuple<decimal, decimal> GetTrailBalanceTotal(DateTime ToDate)
        {
            decimal Debit = 0; decimal Credit = 0;
            try
            {

                ToDate = ToDate.AddDays(1).AddTicks(-10001);
                //var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                using (var connection = getConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"select Isnull(Sum(Debit),0) as Debit,Isnull(Sum(Credit),0) as Credit from vw_trailbalance where PostedDate <= '" + Todate + "'", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Don't assume we have any rows.
                            {

                                Debit = reader.GetDecimal(0);
                                Credit = reader.GetDecimal(1);
                            }
                        }
                    }

                    return Tuple.Create(Debit, Credit);
                }

            }
            catch (Exception ex)
            {
                return Tuple.Create(Debit, Credit);
            }
        }
        public DataTable GetTrailBalance(DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    //ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    //var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select Accounts,Groups,AccountHead ,case when Sum(Debit)> Sum(Credit) then Sum(Debit)-Sum(Credit) else 0 end as Debit,case when Sum(Credit)> Sum(Debit) then Sum(Credit)-Sum(Debit) else 0 end  as Credit from vw_trailbalance where PostedDate <= '" + Todate + "' group by Accounts,Groups,AccountHead order by Accounts asc";
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetICSROHincome(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_ICSROHincome where Date >='" + Frm + "' and Date <= '" + Todate + "'";
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetICSROHexp(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_ICSROHExp where Date >='" + Frm + "' and Date <= '" + Todate + "'";
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetICSROHLedgerexp(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_ICSROHLedgerExp where RefNumber in ( select BillNumber from vw_ICSROHExp where Date >='" + Frm + "' and Date <= '" + Todate + "')";
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int CheckInterestRefundRunning(int FinYear, DateTime FromDate)
        {
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    int Count = 0;
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    using (var command = new SqlCommand(@" SELECT count( p.ProjectId) as ProjectCount FROM tblProject p JOIN tblAgencyMaster a ON p.SponsoringAgency = a.AgencyId WHERE p.TentativeCloseDate > '" + Frm + "' AND p.ProjectClassification != 14 AND p.Status != 'InActive' AND (p.ProjectType IS NULL OR p.projecttype = 1) AND p.ProjectId NOT IN (SELECT ie.ProjectId FROM tblInterestEarned  ie where ie.FinYear=" + FinYear + ")", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Count = reader.GetInt32(0);
                            }
                        }
                    }
                    if (Count > 0)
                    {
                        using (var command1 = new SqlCommand(@"SELECT object_name(b.objectid) as  SpName ,  b.objectid FROM sys.dm_exec_requests a CROSS APPLY sys.dm_exec_sql_text(sql_handle) AS b where object_name(b.objectid) ='InterestRefund'", connection))
                        {

                            using (var reader1 = command1.ExecuteReader())
                            {
                                if (reader1.Read())
                                {
                                    if ("InterestRefund" == reader1.GetString(0))
                                        return -1;
                                    else
                                        return 2;
                                }
                                else
                                    return 2;
                            }
                        }
                    }
                    else
                    {
                        return 1;
                    }


                }
                return 1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public void ExceuteInterestRefund(int FinYear, int Userid)
        {
            //var connection = getConnection();
            //SqlParameter[] ReportParam = new SqlParameter[2];
            //ReportParam[0] = new SqlParameter("@FinYear", SqlDbType.Int, 50);
            //ReportParam[0].Value = FinYear;
            //ReportParam[1] = new SqlParameter("@Userid", SqlDbType.Int, 50);
            //ReportParam[1].Value = Userid;
            //SqlHelper.voidExecuteDataset(connection, CommandType.StoredProcedure, "InterestRefund", ReportParam);

            using (SqlConnection connection = getConnection())
            {
                SqlCommand sqlComm = new SqlCommand("InterestRefund", connection);
                sqlComm.Parameters.AddWithValue("@FinYear", FinYear);
                sqlComm.Parameters.AddWithValue("@Userid", Userid);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.CommandTimeout = 18000;
                connection.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);

                DataSet ds = new DataSet();
                SqlDataAdapter da1 = new SqlDataAdapter();
                da1.SelectCommand = sqlComm;
                da1.Fill(ds);
                //sqlComm.Parameters.Clear();
            }
        }
        public DataTable GetInterestRefund(int FinYear)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from tblInterestEarned  where FinYear=" + FinYear;
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int CheckInterestRefundCount(int FinYear, DateTime FromDate)
        {
            int Count = 0;
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();

                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    using (var command = new SqlCommand(@" SELECT count( p.ProjectId) as ProjectCount FROM tblProject p JOIN tblAgencyMaster a ON p.SponsoringAgency = a.AgencyId WHERE p.TentativeCloseDate > '" + Frm + "' AND p.ProjectClassification != 14 AND p.Status != 'InActive' AND (p.ProjectType IS NULL OR p.projecttype = 1) AND p.ProjectId NOT IN (SELECT ie.ProjectId FROM tblInterestEarned  ie where ie.FinYear=" + FinYear + ")", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Count = reader.GetInt32(0);
                            }
                        }
                    }
                }
                return Count;
            }
            catch (Exception ex)
            {
                return Count;
            }
        }
        public DataTable GetBRSReconcile(DateTime FromDate, DateTime ToDate, int BankId)
        {
            DataSet dataset = new DataSet();
            DataTable dtColumns = new DataTable();
            ToDate = ToDate.AddDays(1).AddTicks(-10001);
            var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
            var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select ReferenceNumber,Amount,Remarks,TransactionType,VoucherDate,BankDate,BankDescription from vw_GetBRSReconciledReport where BankHeadId=" + BankId + "  and VoucherDate >='" + Frm + "' and VoucherDate <= '" + Todate + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                return dtColumns;
            }
        }
        public DataSet GetProjectExpExceed()
        {
            var connection = getConnection();
            SqlParameter[] ReportParam = new SqlParameter[0];
            try
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "GetProjectExpExceed", ReportParam);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public DataTable GetProfitandLossIncome(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    // var Frm = Date.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select a.Groups,a.AccountHead,(isnull(sum(Credit), 0) - isnull(sum(Debit), 0)) as Amount from tblAccountHeadandgroup as a left join vw_ProfitandLoss as b on a.AccountHeadId = b.AccountHeadId where b.PostedDate>='" + FromDate + "'  and b.PostedDate>='2020-04-01 00:00:00.000' and  b.PostedDate <= '" + Todate + "' and a.Accounts = 'Income' group by a.Groups,a.AccountHead order by a.Groups,a.AccountHead";
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetProfitandLossExpense(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    //   var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select a.Groups,a.AccountHead,(isnull(sum(Debit), 0) - isnull(sum(Credit), 0)) as Amount from tblAccountHeadandgroup as a left join vw_ProfitandLoss as b on a.AccountHeadId = b.AccountHeadId  where b.PostedDate>='" + FromDate + "' and b.PostedDate>='2020-04-01 00:00:00.000' and  b.PostedDate <= '" + Todate + "' and a.Accounts = 'Expense' group by a.Groups,a.AccountHead order by a.Groups,a.AccountHead";
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public DataTable GetReceiptsForReceiptsandPayment(DateTime Date)
        //{
        //    DataTable dtColumns = new DataTable();
        //    try
        //    {
        //        using (var connection = getConnection())
        //        {
        //            Date = Date.AddDays(1).AddTicks(-10001);
        //            // var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
        //            var Todate = Date.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
        //            connection.Open();
        //            var command = new System.Data.SqlClient.SqlCommand();
        //            command.Connection = connection;
        //            command.CommandType = CommandType.Text;
        //            command.CommandText = " select a.AccountHead,(isnull(sum(a.Credit), 0) - isnull(sum(a.Debit), 0)) as Amount from vw_ReceiptsandPayments as a where  a.AccountHead in('Interest Credit','Project Receipts','Internal Receipt')  and   a.PostedDate>='2020-04-01 00:00:00.000' and a.PostedDate <= '" + Todate + "' group by a.AccountHead,a.AccountHeadId order by a.AccountHeadId";
        //            command.CommandTimeout = 180;
        //            var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
        //            var dataset = new DataSet();
        //            adapter.Fill(dataset);
        //            dtColumns = dataset.Tables[0];
        //        }
        //        return dtColumns;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public DataTable GetPayementForReceiptsandPayment(DateTime Date)
        //{
        //    DataTable dtColumns = new DataTable();
        //    try
        //    {
        //        using (var connection = getConnection())
        //        {
        //            Date = Date.AddDays(1).AddTicks(-10001);
        //            //var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
        //            var Todate = Date.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
        //            connection.Open();
        //            var command = new System.Data.SqlClient.SqlCommand();
        //            command.Connection = connection;
        //            command.CommandType = CommandType.Text;
        //            // command.CommandText = "select AccountHead,(isnull(sum(Debit), 0) - isnull(sum(Credit), 0)) as Amount  from vw_PaymentsandReceipt  where  PostedDate <= '" + Todate + "' group by AccountHead,AccountHeadId order by AccountHeadId";
        //            command.CommandText = "select HeadName as AccountHead,Sum(AmountSpent) as Amount  from vw_ProjectExpenditureReportDuplicate  where  ProjectClassification not in (4,6) and BillNumber in  (select b.RefNumber from tblBOA  as b  where b.Status = 'Posted' and b.PostedDate >= '2020-04-01 00:00:00.000' and b. PostedDate <= '" + Todate + "') group by HeadName order by HeadName";

        //            command.CommandTimeout = 180;
        //            var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
        //            var dataset = new DataSet();
        //            adapter.Fill(dataset);
        //            dtColumns = dataset.Tables[0];
        //        }
        //        return dtColumns;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public DataTable GetPayementForReceiptsandPaymentForRandP(DateTime FromDate, DateTime ToDate)
        //{
        //    DataTable dtColumns = new DataTable();
        //    try
        //    {
        //        using (var connection = getConnection())
        //        {
        //            ToDate = ToDate.AddDays(1).AddTicks(-10001);
        //            //var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
        //            var Fromdate = FromDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
        //            var Todate = ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
        //            connection.Open();
        //            var command = new System.Data.SqlClient.SqlCommand();
        //            command.Connection = connection;
        //            command.CommandType = CommandType.Text;
        //            // command.CommandText = "select AccountHead,(isnull(sum(Debit), 0) - isnull(sum(Credit), 0)) as Amount  from vw_PaymentsandReceipt  where  PostedDate <= '" + Todate + "' group by AccountHead,AccountHeadId order by AccountHeadId";
        //            command.CommandText = "select HeadName as AccountHead,Sum(AmountSpent) as Amount  from vw_ProjectExpenditureReportDuplicate  where  ProjectClassification not in (4,6) and TransactionTypeCode !='Receipt OH' and BillNumber in  (select b.RefNumber from tblBOA  as b  where b.Status = 'Posted' and b.PostedDate >= '2020-04-01 00:00:00.000' and  b.PostedDate >= '" + Fromdate + "' and b. PostedDate <= '" + Todate + "') group by HeadName order by HeadName";

        //            command.CommandTimeout = 180;
        //            var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
        //            var dataset = new DataSet();
        //            adapter.Fill(dataset);
        //            dtColumns = dataset.Tables[0];
        //        }
        //        return dtColumns;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public Tuple<decimal, decimal> GetReceiptsForReceiptsandPayment(DateTime FromDate, DateTime ToDate)
        {
            decimal Amt1 = 0; decimal Amt2 = 0;
            try
            {
                ToDate = ToDate.AddDays(1).AddTicks(-10001);
                // var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                var Todate = ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                using (var connection = getConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(@" select isnull(sum(a.Amount), 0) as Amount from vw_ReceiptsandPayments as a where  a.Type ='ICSROH'  and   a.Date>=" + FromDate + " and a.Date <= '" + Todate + "'", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Don't assume we have any rows.
                            {

                                Amt1 = reader.GetDecimal(0);
                            }
                        }
                    }
                    using (var command1 = new SqlCommand(@" select isnull(sum(a.Amount), 0) as Amount from vw_ReceiptsandPayments as a where  a.Type ='NON-ICSROH'  and   a.Date>=" + FromDate + " and a.Date <= '" + Todate + "'", connection))
                    {
                        using (var reader1 = command1.ExecuteReader())
                        {
                            if (reader1.Read()) // Don't assume we have any rows.
                            {
                                Amt2 = reader1.GetDecimal(0);
                            }
                        }
                    }
                    return Tuple.Create(Amt1, Amt2);
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create(Amt1, Amt2);
            }
        }
        public DataSet GetPayementForReceiptsandPayment(DateTime FromDate, DateTime Date)
        {
            DataSet set = new DataSet();
            DataTable dtCopy = new DataTable("NON-ICSR");
            DataTable dtCopy2 = new DataTable("ICSR");
            DataTable dtColumns1 = new DataTable("NON-ICSR");
            DataTable dtColumns2 = new DataTable("ICSR");
            try
            {
                using (var connection = getConnection())
                {
                    Date = Date.AddDays(1).AddTicks(-10001);
                    var Todate = Date.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    connection.Open();

                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select HeadName as AccountHead,Sum(AmountSpent) as Amount  from vw_ProjectExpenditureReportDuplicate  where  ProjectClassification not in (4,6) and ExpenditureDate >='" + FromDate + "' and ExpenditureDate <= '" + Todate + "' group by HeadName order by HeadName";
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns1 = dataset.Tables[0];
                    dtCopy = dtColumns1.Copy();
                    set.Tables.Add(dtCopy);

                    var command1 = new System.Data.SqlClient.SqlCommand();
                    command1.Connection = connection;
                    command1.CommandType = CommandType.Text;
                    command1.CommandText = "select HeadName as AccountHead,Sum(AmountSpent) as Amount  from vw_ProjectExpenditureReportDuplicate  where  ProjectClassification  in (4,6) and ExpenditureDate >= '" + FromDate + "' and ExpenditureDate <= '" + Todate + "' group by HeadName order by HeadName";
                    command1.CommandTimeout = 180;
                    var adapter1 = new System.Data.SqlClient.SqlDataAdapter(command1);
                    var dataset1 = new DataSet();
                    adapter1.Fill(dataset1);
                    dtColumns2 = dataset1.Tables[0];
                    dtCopy2 = dtColumns2.Copy();
                    dtCopy2.TableName = "ICSR";
                    set.Tables.Add(dtCopy2);
                }
                return set;
            }
            catch (Exception ex)
            {
                return set;
            }
        }
        public DataTable GetPayementForReceiptsandPaymentForRandP(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    //var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Fromdate = FromDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    // command.CommandText = "select AccountHead,(isnull(sum(Debit), 0) - isnull(sum(Credit), 0)) as Amount  from vw_PaymentsandReceipt  where  PostedDate <= '" + Todate + "' group by AccountHead,AccountHeadId order by AccountHeadId";
                    command.CommandText = "select HeadName as AccountHead,Sum(AmountSpent) as Amount  from vw_ProjectExpenditureReportDuplicate  where  ProjectClassification not in (4,6) and TransactionTypeCode !='Receipt OH' and BillNumber in  (select b.RefNumber from tblBOA  as b  where b.Status = 'Posted' and b.PostedDate >= '2020-04-01 00:00:00.000' and  b.PostedDate >= '" + Fromdate + "' and b. PostedDate <= '" + Todate + "') group by HeadName order by HeadName";

                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetPrevYearAdvanceandSett()
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {

                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = " select AdvanceReferenceNumber as RefNumber , AdvanceValue as Amount from vw_PrevYearAdvanceandSettlNumber";
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetcurrentyearAdvanceOustanding()
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {

                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = " select AdvanceReferenceNumber as RefNumber , AdvanceValue as Amount from vw_currentyearAdvanceOustanding";
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public Tuple<decimal, decimal> GetPrevDataForBalanceSheet()
        //{
        //    decimal Income = 0; decimal Expense = 0;
        //    decimal Receipts = 0; decimal Payment = 0;
        //    try
        //    {


        //        DateTime StartDate = new DateTime(2020, 4, 01);
        //        // StartDate  = StartDate.AddDays(1).AddTicks(-10001);
        //        //   var Todate = StartDate.ToString("yyyy-MM-dd HH:mm");
        //        //   var Todate = StartDate.ToString("yyyy-MM-dd HH:mm");
        //        //   var Todate = StartDate.ToString("yyyy-MM-dd HH:mm");
        //        //   var Todate = StartDate.ToString("yyyy-MM-dd HH:mm");
        //        var Todate = "2020-04-01 00:00:00.000";
        //        using (var connection = getConnection())
        //        {
        //            connection.Open();
        //            using (var command = new SqlCommand(@"select (isnull(sum(Credit), 0) - isnull(sum(Debit), 0)) as Amount from tblAccountHeadandgroup as a left join vw_ProfitandLoss as b on a.AccountHeadId = b.AccountHeadId and  b.PostedDate < '" + Todate + "' where a.Accounts = 'Income'", connection))
        //            {
        //                using (var reader = command.ExecuteReader())
        //                {
        //                    if (reader.Read()) // Don't assume we have any rows.
        //                    {
        //                        Income = reader.GetDecimal(0);
        //                    }
        //                }
        //            }

        //            using (var command1 = new SqlCommand(@"select (isnull(sum(Debit), 0) - isnull(sum(Credit), 0)) as Amount from tblAccountHeadandgroup as a left join vw_ProfitandLoss as b on a.AccountHeadId = b.AccountHeadId  and  b.PostedDate <'" + Todate + "' where a.Accounts = 'Expense' ", connection))
        //            {
        //                using (var reader1 = command1.ExecuteReader())
        //                {
        //                    if (reader1.Read()) // Don't assume we have any rows.
        //                    {
        //                        Expense = reader1.GetDecimal(0);
        //                    }
        //                }
        //            }
        //            using (var command2 = new SqlCommand(@" select (isnull(sum(a.Credit), 0) - isnull(sum(a.Debit), 0)) as Amount from vw_ReceiptsandPayments as a where      a.PostedDate <'" + Todate + "'", connection))
        //            {
        //                using (var reader2 = command2.ExecuteReader())
        //                {
        //                    if (reader2.Read()) // Don't assume we have any rows.
        //                    {
        //                        Receipts = reader2.GetDecimal(0);
        //                    }
        //                }
        //            }
        //            using (var command3 = new SqlCommand(@"select (isnull(sum(a.Debit), 0) - isnull(sum(a.Credit), 0)) as Amount from vw_PaymentsandReceipt as a where      a.PostedDate < '" + Todate + "'", connection))
        //            {
        //                using (var reader3 = command3.ExecuteReader())
        //                {
        //                    if (reader3.Read()) // Don't assume we have any rows.
        //                    {
        //                        Payment = reader3.GetDecimal(0);
        //                    }
        //                }
        //            }

        //            return Tuple.Create((Income - Expense), (Receipts - Payment));
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return Tuple.Create((Income - Expense), (Receipts - Payment));
        //    }
        //}
        public Tuple<decimal, decimal> GetPrevDataForBalanceSheet(DateTime FromDate)
        {
            decimal Income = 0; decimal Expense = 0;
            decimal Receipts = 0; decimal Payment = 0;
            try
            {

                var Todate = FromDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);

                using (var connection = getConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"select (isnull(sum(Credit), 0) - isnull(sum(Debit), 0)) as Amount from tblAccountHeadandgroup as a left join vw_ProfitandLoss as b on a.AccountHeadId = b.AccountHeadId and  b.PostedDate < '" + Todate + "' where a.Accounts = 'Income'", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Don't assume we have any rows.
                            {
                                Income = reader.GetDecimal(0);
                            }
                        }
                    }

                    using (var command1 = new SqlCommand(@"select (isnull(sum(Debit), 0) - isnull(sum(Credit), 0)) as Amount from tblAccountHeadandgroup as a left join vw_ProfitandLoss as b on a.AccountHeadId = b.AccountHeadId  and  b.PostedDate <'" + Todate + "' where a.Accounts = 'Expense' ", connection))
                    {
                        using (var reader1 = command1.ExecuteReader())
                        {
                            if (reader1.Read()) // Don't assume we have any rows.
                            {
                                Expense = reader1.GetDecimal(0);
                            }
                        }
                    }
                    using (var command2 = new SqlCommand(@" select (isnull(sum(a.Credit), 0) - isnull(sum(a.Debit), 0)) as Amount from vw_trailbalance as a where  a.AccountHeadId in (388,136)  and   a.PostedDate <'" + Todate + "'", connection))
                    {
                        using (var reader2 = command2.ExecuteReader())
                        {
                            if (reader2.Read()) // Don't assume we have any rows.
                            {
                                Receipts = reader2.GetDecimal(0);
                            }
                        }
                    }
                    using (var command3 = new SqlCommand(@"select (isnull(sum(a.Debit), 0) - isnull(sum(a.Credit), 0)) as Amount from vw_PaymentsandReceipt as a where      a.PostedDate < '" + Todate + "'", connection))
                    {
                        using (var reader3 = command3.ExecuteReader())
                        {
                            if (reader3.Read()) // Don't assume we have any rows.
                            {
                                Payment = reader3.GetDecimal(0);
                            }
                        }
                    }

                    return Tuple.Create((Income - Expense), (Receipts - Payment));
                }

            }
            catch (Exception ex)
            {
                return Tuple.Create((Income - Expense), (Receipts - Payment));
            }
        }

        public DataTable GetCashFlowNotes(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select Label,Note,AccountHead,sum(Amount) as Amount from vw_CashFlowDetails where PostedDate >= '" + Frm + "' and PostedDate <= '" + Todate + "'  group by Label,Note,AccountHead  order by Note";
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetCashFlow(DateTime FromDate, DateTime ToDate, string Note)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Frm = FromDate.ToString("yyyy-MM-dd HH:mm");
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select Label,Note,sum(Amount) as Amount from vw_CashFlowDetails where Note in (" + Note + ") and PostedDate >= '" + Frm + "' and PostedDate <= '" + Todate + "'  group by Label,Note  order by Note";
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal GetBankBalance(DateTime ToDate)
        {
            decimal Amt = 0;
            try
            {

                ToDate = ToDate.AddDays(1).AddTicks(-10001);
                var Todate = ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                using (var connection = getConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"select (Isnull(Sum(Debit),0)-Isnull(Sum(Credit),0)) as Amount from vw_trailbalance as a join tblAccountHead as b on a.AccountHeadId=b.AccountHeadId where b.AccountGroupId  in (38,61) and  PostedDate <= '" + Todate + "'", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Don't assume we have any rows.
                                Amt = reader.GetDecimal(0);

                        }
                    }

                    return Amt;
                }

            }
            catch (Exception ex)
            {
                return Amt;
            }
        }
        public DataTable GetCommonBankBalance(DateTime ToDate)
        {

            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select  a.AccountHead ,(Isnull(Sum(Debit),0)-Isnull(Sum(Credit),0)) as Amount from vw_trailbalance as a join tblAccountHead as b on a.AccountHeadId=b.AccountHeadId where b.AccountGroupId  in (38) and  PostedDate <= '" + Todate + "' group by a.AccountHead";
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                return dtColumns;
            }
        }

        public DataTable GetRFExpenditure(int FinYear)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_ResearchFundBreakUp where ExpYear<=" + FinYear;
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetRFProject(int FinYear)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from vw_ResearchFundProjectBreakUp where ProjectFinYear<=" + FinYear;
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet getCashBook(string FromDate, string ToDate, int BankId)
        {
            var connection = getConnection();
            //SqlParameter[] ReportParam = new SqlParameter[3];
            //ReportParam[0] = new SqlParameter("@FromDate", SqlDbType.DateTime, 50);
            //ReportParam[0].Value = FromDate;
            //ReportParam[1] = new SqlParameter("@ToDate", SqlDbType.DateTime, 150);
            //ReportParam[1].Value = ToDate;
            //ReportParam[2] = new SqlParameter("@BankId", SqlDbType.Int, 150);
            //ReportParam[2].Value = BankId;

            try
            {
                //return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "CashBook", ReportParam);
                DataSet ds = new DataSet();
                using (SqlConnection conn = getConnection())
                {
                    SqlCommand sqlComm = new SqlCommand("CashBook", conn);
                    sqlComm.Parameters.AddWithValue("@FromDate", FromDate);
                    sqlComm.Parameters.AddWithValue("@ToDate", ToDate);
                    sqlComm.Parameters.AddWithValue("@BankId", BankId);
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    sqlComm.CommandTimeout = 2000;
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = sqlComm;
                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public DataTable InterestRefund(int FinYear)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select * from tblInterestEarned where FinYear=" + FinYear;
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet getRandP(DateTime FromDate, DateTime ToDate)
        {
            var From = FromDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);

            ToDate = ToDate.AddDays(1).AddTicks(-10001);
            var To = ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            var connection = getConnection();
            SqlParameter[] ReportParam = new SqlParameter[2];
            ReportParam[0] = new SqlParameter("@FromDate", SqlDbType.DateTime, 50);
            ReportParam[0].Value = From;
            ReportParam[1] = new SqlParameter("@ToDate", SqlDbType.DateTime, 150);
            ReportParam[1].Value = To;
            try
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "ReceiptandPayment", ReportParam);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public decimal GetImprestBankBalance(DateTime ToDate)
        {
            decimal Amt = 0;
            try
            {

                ToDate = ToDate.AddDays(1).AddTicks(-10001);
                var Todate = ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                using (var connection = getConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"select (Isnull(Sum(Debit),0)-Isnull(Sum(Credit),0)) as Amount from vw_trailbalance as a join tblAccountHead as b on a.AccountHeadId=b.AccountHeadId where b.AccountGroupId  in (61) and  PostedDate <= '" + Todate + "'", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Don't assume we have any rows.
                                Amt = reader.GetDecimal(0);

                        }
                    }

                    return Amt;
                }

            }
            catch (Exception ex)
            {
                return Amt;
            }
        }
        public DataTable GetICSROH_2(DateTime FromDate, DateTime ToDate, string Type)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    var Fromdate = FromDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    if (Type == "Income")
                        command.CommandText = "select b.Groups,b.AccountHead,(isnull(sum(Credit), 0) - isnull(sum(Debit), 0)) as Amount from vw_ICSROH_2  as a right join vw_ICSROHMaster as b on a.AccountHeadId =b.AccountHeadId and  a.PostedDate >= '" + Fromdate + "' and a.PostedDate <= '" + Todate + "'   where  b.Type = '" + Type + "' group by b.Groups,b.AccountHead,b.TypeId order by b.TypeId";
                    else
                        command.CommandText = "select b.Groups,b.AccountHead,(isnull(sum(Debit), 0) - isnull(sum(Credit), 0)) as Amount from vw_ICSROH_2  as a right join vw_ICSROHMaster as b on a.AccountHeadId =b.AccountHeadId and  a.PostedDate >= '" + Fromdate + "' and a.PostedDate <= '" + Todate + "'   where  b.Type = '" + Type + "' group by b.Groups,b.AccountHead,b.TypeId order by b.TypeId";
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetICSROHList_2(DateTime FromDate, DateTime ToDate, string Type)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Fromdate = FromDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    if (Type == "Income")
                        command.CommandText = "select b.Type,b.Groups,(isnull(sum(Credit), 0) - isnull(sum(Debit), 0)) as Amount from vw_ICSROH_2 as a right join vw_ICSROHMaster as b on a.AccountHeadId =b.AccountHeadId and  a.PostedDate >= '" + Fromdate + "' and a.PostedDate <= '" + Todate + "'   where  b.Type = '" + Type + "' group by b.Type,b.TypeId, b.Groups order by b.TypeId";
                    else
                        command.CommandText = "select b.Type,b.Groups,(isnull(sum(Debit), 0) - isnull(sum(Credit), 0)) as Amount from vw_ICSROH_2 as a right join vw_ICSROHMaster as b on a.AccountHeadId =b.AccountHeadId and  a.PostedDate >= '" + Fromdate + "' and a.PostedDate <= '" + Todate + "'   where  b.Type = '" + Type + "' group by b.Type,b.TypeId, b.Groups order by b.TypeId";

                    //if (Type == "Income")
                    //    command.CommandText = "select b.Groups,b.AccountHead,(isnull(sum(Credit), 0) - isnull(sum(Debit), 0)) as Amount from vw_ICSROH_2  as a right join vw_ICSROHMaster as b on a.AccountHeadId =b.AccountHeadId and  a.PostedDate >= '" + Fromdate + "' and a.PostedDate <= '" + Todate + "'  and a.Type='" + Type + "' group by b.Groups,b.AccountHead order by b.Groups";
                    //else
                    //    command.CommandText = "select b.Groups,b.AccountHead,(isnull(sum(Debit), 0) - isnull(sum(Credit), 0)) as Amount from vw_ICSROH_2  as a right join vw_ICSROHMaster as b on a.AccountHeadId =b.AccountHeadId and  a.PostedDate >= '" + Fromdate + "' and a.PostedDate <= '" + Todate + "'  and a.Type='" + Type + "' group by b.Groups,b.AccountHead order by b.Groups";

                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetICSROHBreakUp_2(DateTime FromDate, DateTime ToDate)
        {
            DataTable dtColumns = new DataTable();
            try
            {
                using (var connection = getConnection())
                {
                    ToDate = ToDate.AddDays(1).AddTicks(-10001);
                    var Fromdate = FromDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    var Todate = ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;

                    command.CommandText = "select * from vw_ICSROH_2  where PostedDate >= '" + Fromdate + "' and PostedDate <= '" + Todate + "'";

                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet GetExpenditureReportwithBankAmount(DateTime Fromdate, DateTime ToDate)
        {
            ToDate = ToDate.AddDays(1).AddTicks(-10001);
            var Frm = Fromdate.ToString("yyyy-MM-dd HH:mm");
            var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
            var connection = getConnection();
            SqlParameter[] ReportParam = new SqlParameter[2];
            ReportParam[0] = new SqlParameter("@Date", SqlDbType.DateTime);
            ReportParam[0].Value = Frm;
            ReportParam[1] = new SqlParameter("@Date2", SqlDbType.DateTime);
            ReportParam[1].Value = Todate;
            try
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "ExpenditureReportwithBankAmount", ReportParam);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public DataTable GetDaywiseTapal(DateTime Date)
        {
            DataSet dataset = new DataSet();
            DataTable dtColumns = new DataTable();
            // ToDate = ToDate.AddDays(1).AddTicks(-10001);
            var Frm = Date.ToString("yyyy-MM-dd");
            //var Todate = ToDate.ToString("yyyy-MM-dd HH:mm");
            try
            {
                using (var connection = getConnection())
                {
                    connection.Open();
                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select Name,TapalReceived,Processed ,Paid ,Pending from vw_DaywiseTapal where  Cast(Date AS DATE)='" + Frm + "'";
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    adapter.Fill(dataset);
                    dtColumns = dataset.Tables[0];
                }
                return dtColumns;
            }
            catch (Exception ex)
            {
                return dtColumns;
            }
        }

        public DataSet GetDaywiseTapal(DateTime FromDate, DateTime ToDate, DateTime Date)
        {
            DataSet set = new DataSet();
            DataTable dtCopy = new DataTable("Current");
            DataTable dtCopy2 = new DataTable("Previous");
            DataTable dtColumns1 = new DataTable("Current");
            DataTable dtColumns2 = new DataTable("Previous");
            try
            {
                using (var connection = getConnection())
                {
                    Date = Date.AddDays(1).AddTicks(-10001);
                    var frmdate = FromDate.ToString("yyyy-MM-dd");
                    var Todate = ToDate.ToString("yyyy-MM-dd");
                    var date = Date.ToString("yyyy-MM-dd");
                    connection.Open();

                    var command = new System.Data.SqlClient.SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select Name,TapalReceived,Processed ,Paid ,Pending from vw_DaywiseTapal where  Cast(Date AS DATE)='" + date + "'";
                    command.CommandTimeout = 180;
                    var adapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    var dataset = new DataSet();
                    adapter.Fill(dataset);
                    dtColumns1 = dataset.Tables[0];
                    dtCopy = dtColumns1.Copy();
                    set.Tables.Add(dtCopy);

                    var command1 = new System.Data.SqlClient.SqlCommand();
                    command1.Connection = connection;
                    command1.CommandType = CommandType.Text;
                    command1.CommandText = "select Name,count(TapalReceived) as T2TapalReceived ,(count(Processed)-count(Paid)) as T2Processed,count(Paid)  as  T2Paid,(count(TapalReceived)-count(Processed))  as T2Pending  from vw_DaywiseTapal where    Cast(Date AS DATE)>= '" + FromDate + "' and  Cast(Date AS DATE)<= '" + Todate + "' group by  Name";
                    command1.CommandTimeout = 180;
                    var adapter1 = new System.Data.SqlClient.SqlDataAdapter(command1);
                    var dataset1 = new DataSet();
                    adapter1.Fill(dataset1);
                    dtColumns2 = dataset1.Tables[0];
                    dtCopy2 = dtColumns2.Copy();
                    dtCopy2.TableName = "Previous";
                    set.Tables.Add(dtCopy2);
                }
                return set;
            }
            catch (Exception ex)
            {
                return set;
            }
        }
    }

    public class ReportsProfileHandler
    {

        #region Private Variables
        private int reportID;
        private string reportName;
        private string reportDescription;
        private string fields;
        private string groupByFields;
        private string orderByFields;
        private string tableName;
        private bool isActive;
        private bool isDeleted;
        private int userId;
        private int roleId;
        private int moduleId;
        private DataTable _dtReportFields;
        private DataTable _dtFilterFields;
        private DataTable _dtRoles;
        #endregion

        #region Public Properties

        public int ReportID
        {
            get { return reportID; }
            set { reportID = value; }
        }
        public String ReportName
        {
            get { return reportName; }
            set { reportName = value; }
        }
        public String ReportDescription
        {
            get { return reportDescription; }
            set { reportDescription = value; }
        }
        public String Fields
        {
            get { return fields; }
            set { fields = value; }
        }
        public String GroupByFields
        {
            get { return groupByFields; }
            set { groupByFields = value; }
        }
        public String OrderByFields
        {
            get { return orderByFields; }
            set { orderByFields = value; }
        }
        public String TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; }
        }
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        public int RoleId
        {
            get { return roleId; }
            set { roleId = value; }
        }

        public int ModuleId
        {
            get { return moduleId; }
            set { moduleId = value; }
        }

        public DataTable dtReportFields
        {
            get { return _dtReportFields; }
            set { _dtReportFields = value; }
        }

        public DataTable dtFilterFields
        {
            get { return _dtFilterFields; }
            set { _dtFilterFields = value; }
        }

        public DataTable dtRoles
        {
            get { return _dtRoles; }
            set { _dtRoles = value; }
        }

        public bool CanExport { get; set; }
        public bool ToExcel { get; set; }
        public bool ToPDF { get; set; }

        #endregion

    }
}
