using System;
using System.IO;
using System.Configuration;
using context = System.Web.HttpContext;
using System.Net;
using System.Diagnostics;
/// <summary>  
/// Summary description for ExceptionLogging  
/// </summary>  
namespace IOAS.Infrastructure
{
    public class ErrorHandler
    {
        private static String ErrorlineNo, Errormsg, extype, exurl, hostIp, ErrorLocation, HostAdd;

        public void SendErrorToText(Exception ex)
        {
            var line = Environment.NewLine + Environment.NewLine;
            var st = new StackTrace(ex, true);
            string InnerException = ex.InnerException != null ? ex.InnerException.ToString() : "";
            string Method = ex.StackTrace;
            // Get the top stack frame
            const string lineSearch = ":line ";
            var index = ex.StackTrace.LastIndexOf(lineSearch);
            ErrorlineNo = index != -1 ? ex.StackTrace.Substring(index + lineSearch.Length) : "No line Number";
            Errormsg = ex.GetType().Name.ToString();
            extype = ex.GetType().ToString();
            exurl = context.Current.Request.Url.ToString();
            ErrorLocation = ex.Message.ToString();
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            // Get the IP  
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            Console.WriteLine("My IP Address is :" + myIP);
            try
            {
                string filepath = context.Current.Server.MapPath("~/ExceptionDetailsFile");  //Text File Path
                string fileName = context.Current.Server.MapPath("~/ExceptionDetailsFile/ErrorException.txt");

                if (!Directory.Exists(filepath))
                    Directory.CreateDirectory(filepath);

                if (!File.Exists(fileName))
                    File.Create(fileName).Dispose();
                using (StreamWriter sw = File.AppendText(fileName))
                {
                    string error = "Error Line No :" + " " + ErrorlineNo + line + "Error Message:" + " " + Errormsg + line + "Exception Type:" + " " + extype + line + "Inner Exception :" + " " + InnerException + line + "Method:" + " " + Method + line + "Error Location :" + " " + ErrorLocation + line + "Error Page Url:" + " " + exurl + line + "User Host IP:" + " " + myIP + line + "User Host:" + " " + hostName + line;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    sw.WriteLine(line);
                    sw.WriteLine(error);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }      
    }
}