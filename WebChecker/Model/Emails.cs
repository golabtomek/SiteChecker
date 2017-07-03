using System;
using System.Collections.Generic;
using System.IO;

namespace WebChecker.Model
{
    public class Emails
    {
        private List<string> EmailAddresses;
        private string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "emails.xml");

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
