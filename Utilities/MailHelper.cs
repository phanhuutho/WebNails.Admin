using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace WebNails.Admin.Utilities
{
    public class MailHelper
    {
        public static void SendMail(string strEmailTo, string Subject, string Body, bool IsBug = true)
        {
            using (MailMessage mail = new MailMessage(new MailAddress(ConfigurationManager.AppSettings["EmailSystem"], ConfigurationManager.AppSettings["EmailName"], System.Text.Encoding.UTF8), new MailAddress(strEmailTo)))
            {
                mail.HeadersEncoding = System.Text.Encoding.UTF8;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = bool.Parse(ConfigurationManager.AppSettings["IsBodyHtmlEmailSystem"]);
                mail.Subject = Subject;
                mail.Body = Body;

                SmtpClient mySmtpClient = new SmtpClient(ConfigurationManager.AppSettings["HostEmailSystem"], int.Parse(ConfigurationManager.AppSettings["PortEmailSystem"]));
                NetworkCredential networkCredential = new NetworkCredential(ConfigurationManager.AppSettings["EmailSystem"], ConfigurationManager.AppSettings["PasswordEmailSystem"]);
                mySmtpClient.UseDefaultCredentials = false;
                mySmtpClient.Credentials = networkCredential;
                mySmtpClient.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSslEmailSystem"]);
                mySmtpClient.Send(mail);
            }   
        }
    }
}