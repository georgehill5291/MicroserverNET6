using System.Net.Mail;

namespace SendEmailRabbitMQ.Utility
{
    public class EmailUtil
    {
        public static void SendTestMail(string toEmail, string fromEmail, string bodyEmail, string subjectEmail, string ccEmail, string bccEmail)
        {
            try
            {
                string email = toEmail;
                string[] emails = email.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (emails != null && emails.Length > 0)
                {
                    string mailBody = bodyEmail;
                    string subject = subjectEmail;
                    string cc = ccEmail;
                    string bcc = bccEmail;

                    foreach (string address in emails)
                    {
                        EmailUtil.SendTestEmailTemplate(address, fromEmail, mailBody, subject, null, "", bcc, cc, "");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static bool SendTestEmailTemplate(string sendToAddress, string fromEmail, string mailBody, string mailSubject, Dictionary<string, string> contextDictionary, string from = "", string bccs = "", string ccs = "", string displayNameFrom = "")
        {
            try
            {
                var fromEmailAdress = new MailAddress(StaticConfigurationManager.AppSetting["SMTP:Username"], StaticConfigurationManager.AppSetting["SMTP:Username"]);
                var toAddress = new MailAddress(sendToAddress);

                var smtp = new SmtpClient(StaticConfigurationManager.AppSetting["SMTP:Host"]);
                smtp.Port = int.Parse(StaticConfigurationManager.AppSetting["SMTP:Port"]);
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(StaticConfigurationManager.AppSetting["SMTP:Username"], StaticConfigurationManager.AppSetting["SMTP:Password"]);
                var message = new MailMessage(fromEmailAdress, toAddress)
                {
                    Subject = mailSubject,
                    Body = mailBody,
                    IsBodyHtml = true
                };
                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
