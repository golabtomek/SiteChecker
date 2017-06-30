using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebChecker.Model
{
    public class Emails
    {
        private List<string> EmailAddresses;
        private string path = "emails.xml";

        public Emails()
        {
            EmailAddresses = XmlFiles.LoadEmailsFromFile(path);
        }
        
        public void AddEmail(string email)
        {
            EmailAddresses.Add(email);
        }

        public void RemoveEmail(string email)
        {
            EmailAddresses.Remove(email);
        }

        public List<string> GetEmails()
        {
            return EmailAddresses;
        }

        public void SaveToXml()
        {
            XmlFiles.SaveEmailsToFile(path, EmailAddresses);
        }
    }
}
