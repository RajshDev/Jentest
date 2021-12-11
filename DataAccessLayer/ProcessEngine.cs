using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Configuration;

namespace DataAccessLayer
{
    public class ProcessEngine
    {
        private SqlConnection ConnectDB()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
        }

        public DataSet GetProcessFlowByUser(int ProcessGuidelineId, int userId, int refId)
        {
            var connection = ConnectDB();
            SqlParameter[] ReportParam = new SqlParameter[3];
            ReportParam[0] = new SqlParameter("@ProcessGuidelineId", SqlDbType.Int);
            ReportParam[0].Value = ProcessGuidelineId;
            ReportParam[1] = new SqlParameter("@UserID", SqlDbType.VarChar, 150);
            ReportParam[1].Value = userId;
            //ReportParam[2] = new SqlParameter("@TransactionId", SqlDbType.Int);
            //ReportParam[2].Value = transactionId;
            ReportParam[2] = new SqlParameter("@RefId", SqlDbType.Int);
            ReportParam[2].Value = refId;

            try
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "GetProcessFlowByUser", ReportParam);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        
        public DataSet GetPendingTransactionByUser(int ProcessGuidelineId, int userId)
        {
            var connection = ConnectDB();
            SqlParameter[] ReportParam = new SqlParameter[2];
            ReportParam[0] = new SqlParameter("@ProcessGuidelineId", SqlDbType.Int);
            ReportParam[0].Value = ProcessGuidelineId;
            ReportParam[1] = new SqlParameter("@UserID", SqlDbType.Int);
            ReportParam[1].Value = userId;

            try
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "GetPendingTransactionByUser", ReportParam);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
