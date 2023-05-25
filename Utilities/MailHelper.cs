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
            if (IsBug)
            {
                try
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["EmailSystem"], ConfigurationManager.AppSettings["EmailName"], System.Text.Encoding.Unicode);
                    mail.To.Add(strEmailTo);

                    mail.HeadersEncoding = System.Text.Encoding.Unicode;
                    mail.SubjectEncoding = System.Text.Encoding.Unicode;
                    mail.BodyEncoding = System.Text.Encoding.Unicode;
                    mail.IsBodyHtml = bool.Parse(ConfigurationManager.AppSettings["IsBodyHtmlEmailSystem"]);

                    mail.Subject = Subject;
                    mail.Body = Body;

                    SmtpClient mySmtpClient = new SmtpClient(ConfigurationManager.AppSettings["HostEmailSystem"], int.Parse(ConfigurationManager.AppSettings["PortEmailSystem"]));
                    mySmtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EmailSystem"], ConfigurationManager.AppSettings["PasswordEmailSystem"]);
                    mySmtpClient.UseDefaultCredentials = false;
                    mySmtpClient.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSslEmailSystem"]);

                    mySmtpClient.Send(mail);
                }
                catch (Exception ex) { ex.Source = ex.Source; }
            }
            else
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(ConfigurationManager.AppSettings["EmailSystem"], ConfigurationManager.AppSettings["EmailName"], System.Text.Encoding.Unicode);
                mail.To.Add(strEmailTo);

                mail.HeadersEncoding = System.Text.Encoding.Unicode;
                mail.SubjectEncoding = System.Text.Encoding.Unicode;
                mail.BodyEncoding = System.Text.Encoding.Unicode;
                mail.IsBodyHtml = bool.Parse(ConfigurationManager.AppSettings["IsBodyHtmlEmailSystem"]);

                mail.Subject = Subject;
                mail.Body = Body;

                SmtpClient mySmtpClient = new SmtpClient(ConfigurationManager.AppSettings["HostEmailSystem"], int.Parse(ConfigurationManager.AppSettings["PortEmailSystem"]));
                mySmtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EmailSystem"], ConfigurationManager.AppSettings["PasswordEmailSystem"]);
                mySmtpClient.UseDefaultCredentials = false;
                mySmtpClient.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSslEmailSystem"]);

                mySmtpClient.Send(mail);
            }
        }
    }
}