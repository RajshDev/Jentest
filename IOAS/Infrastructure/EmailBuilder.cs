using IOAS.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;

namespace IOAS.Infrastructure
{
    public class EmailBuilder
    {
        public Tuple<bool, string, string> RunCompile(string templatename, string templatekey, object model, Type modelType = null)
        {
            string result = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(templatename) || model == null)
                    return Tuple.Create(false, result, result);

                string templateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/EmailTemplate/", templatename);

                if (File.Exists(templateFilePath))
                {
                    string template = File.ReadAllText(templateFilePath);

                    if (string.IsNullOrEmpty(templatekey))
                    {
                        templatekey = Guid.NewGuid().ToString();
                    }
                    result = Engine.Razor.RunCompile(template, templatekey, modelType, model);
                    return Tuple.Create(true, result, templatekey);
                }
                return Tuple.Create(false, result, result);
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, result, result);
            }
        }

        public bool ForeignRemitEmail(EmailModel model, string eBody, string[] pdf)        {            try            {                bool enableSSL = true;                string mail = WebConfigurationManager.AppSettings["fromMail"];                string mailpassword = WebConfigurationManager.AppSettings["fromMailPassword"];                string smtpAddress = WebConfigurationManager.AppSettings["smtpAddress"];                int portNumber = Convert.ToInt32(WebConfigurationManager.AppSettings["portNumber"]);                using (MailMessage mm = new MailMessage(mail, model.toMail))                {

                    mm.Subject = "Foreign Remittance Payment Confirmation Mail";                    mm.Body = eBody;                    mm.IsBodyHtml = true;                    foreach (var cc in model.cc ?? new List<string>())                        mm.CC.Add(cc);                    foreach (var bc in model.bcc ?? new List<string>())                        mm.Bcc.Add(bc);
                    // mm.Attachments.Add(new Attachment(new MemoryStream(pdf), "Payslip.pdf"));
                    for (int i = 0; i < pdf.Count(); i++)                    {
                        //string url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + pdf[i];
                        //mm.Attachments.Add(new Attachment(pdf[i]));
                        if (pdf[i] != null)                        {                            var strem = pdf[i].DownloadFile("OtherDocuments");                            mm.Attachments.Add(new Attachment(new MemoryStream(strem), pdf[i]));                        }                    }                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))                    {                        smtp.Credentials = new NetworkCredential(mail, mailpassword);                        smtp.EnableSsl = enableSSL;                        smtp.Send(mm);                    }                }                return true;            }            catch (Exception ex)            {                return false;            }        }

        //public bool SendEmail(EmailModel model, string eBody)
        //{
        //    try
        //    {

        //        bool enableSSL = true;
        //        string mail = WebConfigurationManager.AppSettings["fromMail"];
        //        string mailpassword = WebConfigurationManager.AppSettings["fromMailPassword"];
        //        string smtpAddress = WebConfigurationManager.AppSettings["smtpAddress"];
        //        int portNumber = Convert.ToInt32(WebConfigurationManager.AppSettings["portNumber"]);
        //        using (MailMessage mm = new MailMessage(mail, model.toMail))
        //        {
        //            // string url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + "/Account/Login";
        //            mm.Subject = model.subject;
        //            mm.Body = eBody;
        //            mm.IsBodyHtml = true;
        //            foreach (var cc in model.cc ?? new List<string>())
        //                mm.CC.Add(cc);
        //            foreach (var bc in model.bcc ?? new List<string>())
        //                mm.Bcc.Add(bc);
        //            foreach(var attach in model.attachment ?? new List<string>())
        //                mm.Attachments.Add(new Attachment(attach));
        //            using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
        //            {
        //                smtp.Credentials = new NetworkCredential(mail, mailpassword);
        //                smtp.EnableSsl = enableSSL;
        //                smtp.Send(mm);
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Infrastructure.IOASException.Instance.HandleMe(this, ex);
        //        return false;
        //    }
        //}


        public bool SendEmail(EmailModel model, string eBody)
        {
            try
            {

                bool enableSSL = true;
                string mail = WebConfigurationManager.AppSettings["fromMail"];
                string mailpassword = WebConfigurationManager.AppSettings["fromMailPassword"];
                string smtpAddress = WebConfigurationManager.AppSettings["smtpAddress"];
                int portNumber = Convert.ToInt32(WebConfigurationManager.AppSettings["portNumber"]);
                using (MailMessage mm = new MailMessage(mail, model.toMail))
                {
                    // string url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + "/Account/Login";
                    mm.Subject = model.subject;
                    mm.Body = eBody;
                    mm.IsBodyHtml = true;
                    foreach (var cc in model.cc ?? new List<string>())
                        if (!string.IsNullOrEmpty(cc))
                            mm.CC.Add(cc);
                    foreach (var bc in model.bcc ?? new List<string>())
                        if (!string.IsNullOrEmpty(bc))
                            mm.Bcc.Add(bc);
                    foreach (var attach in model.attachment ?? new List<string>())
                        mm.Attachments.Add(new Attachment(attach));
                    if (model.attachmentByte != null)
                    {
                        if (model.attachmentByte.Count > 0)
                        {
                            foreach (var item in model.attachmentByte)
                                mm.Attachments.Add(new Attachment(new MemoryStream(item.dataByte), item.actualName));
                        }
                    }
                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(mail, mailpassword);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mm);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Infrastructure.IOASException.Instance.HandleMe(this, ex);
                return false;
            }
        }

        public bool ReceiptEmail(EmailModel model, string eBody, byte[] pdf)
        {
            try
            {

                bool enableSSL = true;
                string mail = WebConfigurationManager.AppSettings["fromMail"];
                string mailpassword = WebConfigurationManager.AppSettings["fromMailPassword"];
                string smtpAddress = WebConfigurationManager.AppSettings["smtpAddress"];
                int portNumber = Convert.ToInt32(WebConfigurationManager.AppSettings["portNumber"]);
                using (MailMessage mm = new MailMessage(mail, model.toMail))
                {

                    mm.Subject = "Confirmation of Receipt of Fund";
                    mm.Body = eBody;
                    mm.IsBodyHtml = true;
                    foreach (var cc in model.cc ?? new List<string>())
                        mm.CC.Add(cc);
                    foreach (var bc in model.bcc ?? new List<string>())
                        mm.Bcc.Add(bc);
                    mm.Attachments.Add(new Attachment(new MemoryStream(pdf), "Receipt.pdf"));
                    //for (int i = 0; i < pdf.Count(); i++)
                    //{
                    //    //string url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + pdf[i];
                    //    mm.Attachments.Add(new Attachment(pdf));

                    //}
                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(mail, mailpassword);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mm);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ForeignRemitBOEEmail(EmailModel model, string eBody, string[] fileName)
        {
            try
            {

                bool enableSSL = true;
                string mail = WebConfigurationManager.AppSettings["fromMail"];
                string mailpassword = WebConfigurationManager.AppSettings["fromMailPassword"];
                string smtpAddress = WebConfigurationManager.AppSettings["smtpAddress"];
                int portNumber = Convert.ToInt32(WebConfigurationManager.AppSettings["portNumber"]);
                using (MailMessage mm = new MailMessage(mail, model.toMail))
                {

                    mm.Subject = "Submission of Bill of Entry against import remittances Mail";
                    mm.Body = eBody;
                    mm.IsBodyHtml = true;
                    foreach (var cc in model.cc ?? new List<string>())
                        mm.CC.Add(cc);
                    foreach (var bc in model.bcc ?? new List<string>())
                        mm.Bcc.Add(bc);
                    // mm.Attachments.Add(new Attachment(new MemoryStream(pdf), "Payslip.pdf"));
                    for (int i = 0; i < fileName.Count(); i++)
                    {
                        //string url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + pdf[i];
                        if (fileName[i] != null)
                        {
                            var strem = fileName[i].DownloadFile("OtherDocuments");
                            mm.Attachments.Add(new Attachment(new MemoryStream(strem), fileName[i]));
                        }

                    }
                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(mail, mailpassword);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mm);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RCTSendEmail(EmailModel model, string eBody)        {            try            {                bool enableSSL = true;                string mail = WebConfigurationManager.AppSettings["RCTfromMail"];                string mailpassword = WebConfigurationManager.AppSettings["RCTfromMailPassword"];                string smtpAddress = WebConfigurationManager.AppSettings["smtpAddress"];                int portNumber = Convert.ToInt32(WebConfigurationManager.AppSettings["portNumber"]);                var tomail = model.toMail;
                using (MailMessage mm = new MailMessage(mail, tomail))                {
                    // string url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath + "/Account/Login";
                    mm.Subject = model.subject;                    mm.Body = eBody;                    mm.IsBodyHtml = true;
                    foreach (var cc in model.cc ?? new List<string>())
                        if (!string.IsNullOrEmpty(cc))
                            mm.CC.Add(cc);                    foreach (var bc in model.bcc ?? new List<string>())                        if (!string.IsNullOrEmpty(bc))
                            mm.Bcc.Add(bc);                    if (model.attachmentByte != null)                    {                        if (model.attachmentByte.Count > 0)                        {                            foreach (var item in model.attachmentByte)                                mm.Attachments.Add(new Attachment(new MemoryStream(item.dataByte), item.displayName));                        }                    }                    foreach (var attach in model.attachment ?? new List<string>())                        mm.Attachments.Add(new Attachment(attach));                    foreach (var item in model.attachmentlist ?? new List<EmailAttachmentModel>())                    {                        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(item.actualName);                        attachment.Name = item.displayName;                        mm.Attachments.Add(attachment);                    }                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))                    {                        smtp.Credentials = new NetworkCredential(mail, mailpassword);                        smtp.EnableSsl = enableSSL;                        smtp.Send(mm);                    }                }                return true;            }            catch (Exception ex)            {
                ErrorHandler WriteLog = new ErrorHandler();
                WriteLog.SendErrorToText(ex);
                return false;            }        }    }
}