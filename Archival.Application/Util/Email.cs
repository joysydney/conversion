using Archival.Core.Configuration;
using Archival.Core.Interfaces;
using Archival.Core.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Application.Util
{
    public class Email : IEmail
    {
        private readonly EmailConfiguration _config;
        private readonly ILogManagerCustom _log;
        public Email(IOptions<EmailConfiguration> config, ILogManagerCustom log)
        {
            _config = config.Value;
            _log = log;
        }
        public void sendEmailNotification(Summary summary)
        {
            try
            {
                string email_host = _config.email_host;
                string email_to = _config.email_to;
                string email_from = _config.email_from;
                string email_cc = _config.email_cc;

                SmtpClient smtpClient = new SmtpClient(email_host, 25);
                smtpClient.EnableSsl = false;

                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.From = new MailAddress(email_from);

                // Multiple Recpients
                foreach (string email in email_to.Split(';'))
                {
                    mail.To.Add(new MailAddress(email.Trim()));
                }

                string datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                string title = "";
                if (summary.conversionType == "DAILYCONVERSION")
                {
                    title = "DailyConversion";
                }
                else if (summary.conversionType == "MAIN_A")
                {
                    title = "Document Due for Archiving";
                }
                else if (summary.conversionType == "MAIN_NAS")
                {
                    title = "Transfer to NAS";
                }
                mail.Subject = "[" + _config.subject_header + "] - " + title + " Summary Report.[" + datetime + "]";

                long TotalProcessed = summary.TotalProcessed, TotalSuccess = summary.TotalSuccess, TotalFailed = summary.TotalProcessed - summary.TotalSuccess;

                var body = new StringBuilder();
                body.AppendFormat("<b>" + title + " Summary</b>");
                body.AppendLine("<br/>");
                body.AppendLine("<br/>");
                body.AppendLine("Total number of documents processed : " + TotalProcessed);
                body.AppendLine("<br/>");
                body.AppendLine("<br/>");
                body.AppendLine("Total number of documents passed : " + TotalSuccess);
                body.AppendLine("<br/>");
                body.AppendLine("<br/>");
                body.AppendLine("Total number of documents failed : " + TotalFailed);
                body.AppendLine("<br/>");
                body.AppendLine("<br/>");

                mail.Body = body.ToString();
                try
                {
                    string reportFilePath = "", folderCsvFilePath = "";
                    if (summary.conversionType == "MAIN_A")
                    {
                        reportFilePath = summary.recordReportPath;
                    }
                    else if (summary.conversionType == "MAIN_NAS")
                    {
                        reportFilePath = summary.recordReportPath;
                        folderCsvFilePath = summary.folderReportPath;
                    }
                    else if (summary.conversionType == "DAILYCONVERSION")
                    {
                        reportFilePath = summary.recordReportPath;
                    }
                    string filePath = reportFilePath;
                    _log.debug("filePath: " + filePath);
                    Attachment csvAttach = new Attachment(filePath);

                    int index = reportFilePath.LastIndexOf("\\");
                    int trim = reportFilePath.Length - index;
                    string attachmentFileName = reportFilePath.Substring(index + 1, trim - 1);
                    csvAttach.Name = attachmentFileName;
                    mail.Attachments.Add(csvAttach);

                    if (!String.IsNullOrEmpty(folderCsvFilePath))
                    {
                        int start = folderCsvFilePath.LastIndexOf("\\");
                        int end = folderCsvFilePath.Length - start;
                        string folderAttachmentName = folderCsvFilePath.Substring(start + 1, end - 1);
                        Attachment folderCsv = new Attachment(folderCsvFilePath);
                        folderCsv.Name = folderAttachmentName;
                        mail.Attachments.Add(folderCsv);
                    }
                }
                catch (Exception e)
                {
                    _log.debug("error adding attachment to email: " + e);
                }
                _log.info("Sending Email");
                smtpClient.Send(mail);
                _log.info("Email sent successfully");
            }
            catch (Exception e)
            {
                _log.debug("There is an error in sending email. Error: " + e.Message);
            }
        }
        public void sendFailedEmailNotification(Summary summary)
        {
            try
            {
                List<(string, string, string)> errormsgs = new List<(string, string, string)>();
                foreach (var rec in summary.failedRecords)
                {
                    errormsgs.Add((rec.error_msg, rec.name, rec.recLocation));
                }
                string email_host = _config.email_host;
                string email_to = _config.email_to;
                string email_from = _config.email_from;
                string email_cc = _config.email_cc;

                SmtpClient smtpClient = new SmtpClient(email_host, 25);
                smtpClient.EnableSsl = false;

                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.From = new MailAddress(email_from);
                //mail.To.Add(new MailAddress(email.Trim()));  

                // Multiple Recpients
                foreach (string email in email_to.Split(';'))
                {
                    mail.To.Add(new MailAddress(email.Trim()));
                }

                string datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                string title = "";
                if (summary.conversionType == "DAILYCONVERSION")
                {
                    title = "DailyConversion";
                }
                else if (summary.conversionType == "MAIN_A")
                {
                    title = "Document Due for Archiving";
                }
                else if (summary.conversionType == "MAIN_NAS")
                {
                    title = "Transfer to NAS";
                }
                mail.Subject = "[" + _config.subject_header + "] - " + title + " Failed To Convert File.[" + datetime + "]";

                var body = new StringBuilder();
                body.AppendFormat("<b>Conversion failed for the following files: </b>");
                body.AppendLine("<br/>");
                body.AppendLine("<br/>");
                body.AppendLine("List of Documents Failed to Convert: ");
                body.AppendLine("<br/>");
                body.AppendLine("<table border='1' cellspacing='0'>");
                body.AppendLine("<tr>");
                body.AppendLine("<th>Document Name</th>");
                body.AppendLine("<th>Error Message</th>");
                body.AppendLine("<th>File Location</th>");
                body.AppendLine("</tr>");
                foreach (var msg in errormsgs)
                {
                    body.AppendLine("<tr>");
                    body.AppendLine("<td style='padding-left: 15px; padding-right: 15px; text-align:center; vertical-align: middle;'>" + msg.Item2 + "</td>");
                    body.AppendLine("<td  style='padding-left: 15px; padding-right: 15px; text-align:center; vertical-align: middle;''>" + msg.Item1 + "</td>");
                    body.AppendLine("<td  style='padding-left: 15px; padding-right: 15px; text-align:center; vertical-align: middle;''>" + msg.Item3 + "</td>");
                    body.AppendLine("</tr>");
                }
                body.AppendLine("</table>");
                body.AppendLine("<br/>");
                body.AppendLine("<br/>");

                mail.Body = body.ToString();

                _log.debug("Sending Failed Email");
                smtpClient.Send(mail);
                _log.debug("Failed email sent successfully");
            }
            catch (Exception e)
            {
                _log.debug("There is an Error in sending failed email.Error: " + e.Message);
            }
        }
    }
}
