using System.Collections.Generic;
using System.Net.Mail;
using System.Security;

namespace WebChecker.Model
{
    public class Notify
    {
        public string address { get; set; }
        public int smtpPort { get; set; }
        public bool enableSsl { get; set; }
        public string smtpServer { get; set; }
        public SecureString password { get; set; }


        public Notify(string address, SecureString password, int smtpPort, bool enableSsl, string smtpServer)
        {
            this.address = address;
            this.password = password;
            this.smtpPort = smtpPort;
            this.enableSsl = enableSsl;
            this.smtpServer = smtpServer;
        }

        public void email_send(string website, string status, List<string> emails)
        {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(smtpServer);
                mail.From = new MailAddress(address);

                foreach (string email in emails)
                {
                    mail.To.Add(new MailAddress(email));
                }

                mail.Subject = "Website " + website + "is unreachable";
                mail.Body = "Something goes wrong and website " + website + " returns status: \n" + status;

                SmtpServer.Port = smtpPort;
                SmtpServer.Credentials = new System.Net.NetworkCredential(address, Secure.SecurePasswordToString(password));
                SmtpServer.EnableSsl = enableSsl;

                SmtpServer.Send(mail);
        }

        
    }
}
