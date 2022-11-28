using System;
using System.Web;
using System.IO;
using System.Web.Configuration;
using System.Net;
using System.Text;

namespace IOAS.Infrastructure
{
    public static class ExtensionMethods
    {
        public static byte[] GetFileData(this string fileName, string filePath)
        {
            var filenameNew = HttpUtility.UrlPathEncode(fileName);
            var fullFilePath = string.Format("{0}/{1}", filePath, fileName);
            if (!File.Exists(fullFilePath))
                throw new FileNotFoundException("The file does not exist.",
                    fullFilePath);
            return File.ReadAllBytes(fullFilePath);
        }

        public static bool UploadFile(this HttpPostedFileBase file, string dirName, string filename)
        {
            try
            {
                string rootPath = WebConfigurationManager.AppSettings["ftpRootPath"];
                string ftpUser = WebConfigurationManager.AppSettings["ftpUser"];
                string ftpPassword = WebConfigurationManager.AppSettings["ftpPassword"];
                if (!FtpDirectoryExists(rootPath, ftpUser, ftpPassword, dirName))
                {
                    FindAndCreate(dirName);
                }
                if (!string.IsNullOrEmpty(filename))
                    return FtpUploadFile(rootPath + dirName, ftpUser, ftpPassword, file, filename);
                else
                    return false;
            }
            catch(Exception exx)
            {
               
                    string Message = exx.Message ?? "";
                    string InnerException = exx.InnerException.ToString() ?? "";
                    string Exception = Message + InnerException;

                    Infrastructure.IOASException.Instance.HandleMe(
    (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName + " Exception -------------- " 
    + Exception +"-----" + filename + "------"+ dirName +"---"+ file + " ----------------------  ", exx.InnerException);
                    return false;
                
            }
            

        }
        public static bool UploadByteFile(this byte[] file, string dirName, string filename)
        {
            string rootPath = WebConfigurationManager.AppSettings["ftpRootPath"];
            string ftpUser = WebConfigurationManager.AppSettings["ftpUser"];
            string ftpPassword = WebConfigurationManager.AppSettings["ftpPassword"];
            if (!FtpDirectoryExists(rootPath, ftpUser, ftpPassword, dirName))
            {
                FindAndCreate(dirName);
            }
            if (!string.IsNullOrEmpty(filename))
                return FtpUploadFile(rootPath + dirName, ftpUser, ftpPassword, file, filename);
            else
                return false;

        }
        static bool FtpUploadFile(string ftpPath, string ftpUser, string ftpPassword, byte[] fileBytes, string fileName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("{0}/{1}", ftpPath, fileName));
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = new NetworkCredential(ftpUser, ftpPassword);

            request.ContentLength = fileBytes.Length;
            request.UsePassive = true;
            request.UseBinary = true;
            request.ServicePoint.ConnectionLimit = fileBytes.Length;
            request.EnableSsl = false;
            try
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                    requestStream.Close();
                    return true;
                }
            }
            catch (WebException ex)
            {
                //message = "file Not Uploaded";
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    //Does not exist
                }
                return false;
            }
        }

        public static byte[] DownloadFile(this string filename, string dirName)
        {
            string rootPath = WebConfigurationManager.AppSettings["ftpRootPath"];
            string ftpUser = WebConfigurationManager.AppSettings["ftpUser"];
            string ftpPassword = WebConfigurationManager.AppSettings["ftpPassword"];
            string fileLoc = string.Format("{0}{1}/{2}", rootPath, dirName.TrimEnd('/'), filename);
            return FtpGetFileData(fileLoc, ftpUser, ftpPassword);

        }
        public static byte[] WFProposalDownloadFile(this string filename, string dirName)
        {
            try
            {
                string rootPath = WebConfigurationManager.AppSettings["WFProposalftpRootPath"];
                string ftpUser = WebConfigurationManager.AppSettings["WFProposalftpUser"];
                string ftpPassword = WebConfigurationManager.AppSettings["WFProposalftpPassword"];
                string fileLoc = string.Format("{0}{1}/{2}", rootPath, dirName.TrimEnd('/'), filename);
   //             Infrastructure.IOASException.Instance.HandleMe(
   //rootPath + ",,"+ftpUser + ",," + ftpPassword + ",," + fileLoc, new Exception ());
                return FtpGetFileData(fileLoc, ftpUser, ftpPassword);
               
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                return null;
            }
            

        }
        public static Stream GetFileStream(this string filename, string dirName)
        {
            string rootPath = WebConfigurationManager.AppSettings["ftpRootPath"];
            string ftpUser = WebConfigurationManager.AppSettings["ftpUser"];
            string ftpPassword = WebConfigurationManager.AppSettings["ftpPassword"];
            string fileLoc = string.Format("{0}{1}/{2}", rootPath, dirName.TrimEnd('/'), filename);
            return FtpGetFileStream(fileLoc, ftpUser, ftpPassword);

        }
        static void FindAndCreate(string path)
        {
            string[] patharr = path.Split('/');
            string rootPath = WebConfigurationManager.AppSettings["ftpRootPath"];
            string ftpUser = WebConfigurationManager.AppSettings["ftpUser"];
            string ftpPassword = WebConfigurationManager.AppSettings["ftpPassword"];
            if (patharr.Length > 0)
            {
                foreach (var folder in patharr)
                {
                    if (!FtpDirectoryExists(rootPath, ftpUser, ftpPassword, folder))
                    {
                        FtpCreateDirectory(rootPath, ftpUser, ftpPassword, folder);
                    }
                }
            }
        }
        static bool FtpDirectoryExists(string ftproot, string ftpUser, string ftpPassword, string foldername)
        {
            bool IsExists = true;
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("{1}/{0}/", foldername, ftproot.TrimEnd('/')));
                request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch (WebException ex)
            {
                IsExists = false;
            }
            return IsExists;
        }
        static bool FtpCreateDirectory(string ftproot, string ftpUser, string ftpPassword, string foldername)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("{1}/{0}", foldername, ftproot));
                request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                using (var resp = request.GetResponse())
                {
                    return true;
                }
            }
            catch (WebException ex)
            {
                return false;
            }
        }
        static bool FtpUploadFile(string ftpPath, string ftpUser, string ftpPassword, HttpPostedFileBase file, string fileName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("{0}/{1}", ftpPath, fileName));
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = new NetworkCredential(ftpUser, ftpPassword);

            var fileBytes = new byte[file.ContentLength];
            file.InputStream.Read(fileBytes, 0, fileBytes.Length);

            request.ContentLength = fileBytes.Length;
            request.UsePassive = true;
            request.UseBinary = true;
            request.ServicePoint.ConnectionLimit = fileBytes.Length;
            request.EnableSsl = false;
            try
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                    requestStream.Close();
                    return true;
                }
            }
            catch (WebException exx)
            {
                
                    string Message = exx.Message ?? "";
                    string InnerException = exx.InnerException.ToString() ?? "";
                    string Exception = Message + InnerException;
                    Infrastructure.IOASException.Instance.HandleMe(
     (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName + " Exception -------------- " + Exception + " ----------------------  ", exx.InnerException);

                    //message = "file Not Uploaded";
                    FtpWebResponse response = (FtpWebResponse)exx.Response;
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        //Does not exist
                    }
                    return false;
                
            }
        }
        static byte[] FtpGetFileData(string ftpFilePath, string ftpUser, string ftpPassword)
        {
            byte[] buffer = new byte[1024];
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilePath);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
            request.UsePassive = true;
            request.UseBinary = true;
            request.EnableSsl = false;
            try
            {
                using (Stream ftpStream = request.GetResponse().GetResponseStream())
                {
                   return ToByteArray(ftpStream);
                  
                }
            }
            catch (WebException ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(
      (object)System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName, ex);
                throw new Exception(ex.Message);
            }
        }
        static Stream FtpGetFileStream(string ftpFilePath, string ftpUser, string ftpPassword)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilePath);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
            request.UsePassive = true;
            request.UseBinary = true;
            request.EnableSsl = false;
            MemoryStream ms = new MemoryStream();
            try
            {
                using (Stream str = request.GetResponse().GetResponseStream())
                {                    
                        str.CopyTo(ms);
                        return ms;
                }
            }
            catch (WebException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (ms != null) ms.Dispose();
            }
        }
        static bool FtpDownloadFile(string ftpFilePath, string appFilePath, string ftpUser, string ftpPassword)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilePath);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
            request.UsePassive = true;
            request.UseBinary = true;
            request.EnableSsl = false;
            try
            {
                using (Stream ftpStream = request.GetResponse().GetResponseStream())
                {
                    using (Stream fileStream = File.Create(appFilePath))
                    {
                        ftpStream.CopyTo(fileStream);
                    }
                }
                return true;
            }
            catch (WebException ex)
            {
                //message = "file Not Uploaded";
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    //Does not exist
                }
                return false;
            }
            
        }
        public static Byte[] ToByteArray(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            byte[] chunk = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(chunk, 0, chunk.Length)) > 0)
            {
                ms.Write(chunk, 0, bytesRead);
            }

            return ms.ToArray();
        }
        public class MemoryPostedFile : HttpPostedFileBase
        {
            private readonly byte[] fileBytes;

            public MemoryPostedFile(byte[] fileBytes, string fileName = null)
            {
                this.fileBytes = fileBytes;
                this.FileName = fileName;
                this.InputStream = new MemoryStream(fileBytes);
            }

            public override int ContentLength => fileBytes.Length;

            public override string FileName { get; }

            public override Stream InputStream { get; }
        }

      
    }
}