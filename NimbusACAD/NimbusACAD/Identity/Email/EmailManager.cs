using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Threading.Tasks;
using System.Net.Mime;

namespace NimbusACAD.Identity.Email
{
    public class EmailMessage
    {
        public string To;
        public string Subject;
        public string Body;

        public EmailMessage(string _to, string _subject, string _body)
        {
            this.To = _to;
            this.Subject = _subject;
            this.Body = _body;
        }
    }

    public class EmailService
    {
        private const string cKey_SmtpServer = "SmtpServer";
        private const string cKey_SmtpPort = "SmtpPort";
        private const string cKey_SmtpUsername = "SmtpUsername";
        private const string cKey_SmtpPassword = "SmtpPassword";
        private const string cKey_SmtpEmailFrom = "SmtpEmailFrom";
        private const string cKey_SmtpNetworkDeliveryMethodEnabled = "SmtpNetwokDeliveryMethod";

        private readonly string m_Server;
        private readonly int m_Port;
        private readonly string m_Username;
        private readonly string m_Password;
        private readonly string m_EmailFrom;
        private readonly bool m_IsSmtpNetworkDeliveryMethodEnabled;

        public EmailService()
        {
            this.m_Server = ExtendedMethods.GetConfigSetting(cKey_SmtpServer);
            this.m_Port = ExtendedMethods.GetConfigSettingAsInt(cKey_SmtpPort);
            this.m_Username = ExtendedMethods.GetConfigSetting(cKey_SmtpUsername);
            this.m_Password = ExtendedMethods.GetConfigSetting(cKey_SmtpPassword);
            this.m_EmailFrom = ExtendedMethods.GetConfigSetting(cKey_SmtpEmailFrom);
            this.m_IsSmtpNetworkDeliveryMethodEnabled = ExtendedMethods.GetConfigSettingAsBool(cKey_SmtpNetworkDeliveryMethodEnabled);
        }

        public async Task Send(EmailMessage message)
        {
            Send2MailAccount(message);
            //return;

            //Configure Email Client
            SmtpClient client = new SmtpClient(this.m_Server);
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            //Create credentials
            NetworkCredential credential = new NetworkCredential(this.m_Username, this.m_Password);
            client.EnableSsl = true;
            client.Credentials = credential;

            //WARNING: Switches of certification validation!
            ServicePointManager.ServerCertificateValidationCallback = delegate (object o, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

            //Create message
            var mail = new MailMessage(new MailAddress(this.m_Username, "Administrator"), new MailAddress(message.To, message.To));
            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.IsBodyHtml = true;

            //Send:
            if (mail != null)
            {
                await client.SendMailAsync(mail);
            }
            else
            {
                await Task.FromResult(0);
            }
        }

        private bool Send2MailAccount(EmailMessage message)
        {
            bool _retVal = false;
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(this.m_EmailFrom);
                    mail.To.Add(message.To);
                    mail.Subject = message.Subject;
                    mail.Body = message.Body;
                    mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message.Body, null, MediaTypeNames.Text.Plain));
                    mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message.Body, null, MediaTypeNames.Text.Html));

                    var smtp = new SmtpClient(this.m_Server, this.m_Port);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(this.m_Username, this.m_Password);
                    smtp.EnableSsl = true;

                    if (this.m_IsSmtpNetworkDeliveryMethodEnabled)
                    {
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    }

                    smtp.Send(mail);

                    //Email was accepted by the SMTP Server
                    _retVal = true;
                }
            }
            catch
            {
                return false;
            }
            return _retVal;
        }
    }
}