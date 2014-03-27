using System.ComponentModel;
using System.Net.Mail;
using CarbonKnown.MVC.Properties;

namespace CarbonKnown.MVC.Code
{
    public class EmailManager : IEmailManager
    {
        private static string ReplaceTokens(string template, object data)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(data))
            {
                var token = string.Format("{{{0}}}", property.Name);
                var value = string.Format("{0}", property.GetValue(data));

                template = template.Replace(token, value);
            }
            return template;
        }

        public void SendMail<T>(T model, string template, params string[] addresses)
        {
            var emailTemplate = string.IsNullOrWhiteSpace(template) ? Settings.Default.EmailTemplate : template;
            var mailBody = WebFormMvcUtil.RenderHtml(emailTemplate, model);
            var subject = ReplaceTokens(Settings.Default.EmailSubject, model);
            var mail = new MailMessage();

            foreach (var toAddress in addresses)
            {
                mail.To.Add(toAddress);
            }
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = mailBody;

            using (var client = new SmtpClient())
            {
                client.Send(mail);
            }
        }
    }
}