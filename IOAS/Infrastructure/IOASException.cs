using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Collections;
using System.Text;


namespace IOAS.Infrastructure
{
    public class IOASException
    {
        private object logWriteLock = new object();
        private FileStream fileStream = null;
        private StreamWriter streamWriter = null;
        private static IOASException _utException;
        private IOASException()
        { }
        public static IOASException Instance
        {
            get
            {
                if (_utException == null)
                {
                    _utException = new IOASException();
                }
                return _utException;
            }
        }

        public void HandleMe(object sender, Exception ex)
        {
            try
            {

                if (!(ex is System.Threading.ThreadAbortException))
                {
                    WriteLog(sender.ToString(), ex.Message.ToString() + " - " + ex.StackTrace);
                    if (sender is UserControl && ConfigurationManager.AppSettings["IsExceptionNeedTodisplay"].ToString() == "true")
                    {
                        try
                        {
                            Int32 RequestLength = HttpContext.Current.Request.RawUrl.Split('/').GetUpperBound(0);
                            if (RequestLength > 2)
                            {
                                RequestLength = RequestLength - 2;
                            }
                            string strErrorPath = @"..\";
                            StringBuilder strErrorUrl = new StringBuilder();
                            strErrorUrl.Append("");
                            for (int i = 1; i <= RequestLength; i++)
                            {
                                strErrorUrl.Append(strErrorPath);
                            }
                            strErrorUrl.Append("Error.aspx?ErrorMessage= ");
                            HttpContext.Current.Response.Redirect(strErrorUrl.ToString() + ex.Message.ToString());

                        }
                        catch (Exception InnerEx)
                        {
                            WriteLog("Exception", InnerEx.Message.ToString());
                        }
                    }
                }
            }
            catch (Exception outex)
            {
                WriteLog("Exception_outer", outex.Message.ToString());
            }
        }
        private void WriteLog(string module, string message)
        {
            lock (logWriteLock)
            {
                string strLogFilePath = ConfigurationManager.AppSettings["ApplicationLogPath"].ToString();
                string curDir = HttpContext.Current.Server.MapPath(strLogFilePath);
                strLogFilePath = curDir;
                if (!(System.IO.Directory.Exists(strLogFilePath)))
                {
                    System.IO.Directory.CreateDirectory(strLogFilePath);
                }
                string LogFileName = strLogFilePath + "Errors.txt";
                fileStream = new FileStream(LogFileName, FileMode.Append, FileAccess.Write);
                streamWriter = new StreamWriter(fileStream);
                string ErrorMsg = GetIPAddress() + " - " +
                                   DateTime.Now.ToString() + " " +
                                   module + " - " + message;
                streamWriter.WriteLine(ErrorMsg);
                streamWriter.Flush();
                fileStream.Close();
            }
        }
        private string GetIPAddress()
        {
            string sIPAddress = string.Empty;
            sIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (sIPAddress == null || sIPAddress == "")
            {
                sIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return sIPAddress;
        }
    }
}
