using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace WebChecker.Model
{
    public static class XmlFiles
    {
        public static List<Site> LoadSitesFromFile(string path)
        {
            List<Site> SitesList = new List<Site>();
            if (File.Exists(path))
            {
                try
                {
                    XDocument xml = XDocument.Load(path);
                    List<string> sitesData = xml.Root.Elements("Site").
                        Select(element => element.Value).ToList();
                    foreach (string site in sitesData)
                    {
                        SitesList.Add(new Site(site));
                    }
                    return SitesList;
                }
                catch (Exception)
                {
                    return SitesList;
                }
            }
            else
            {
                return SitesList;
            }
        }

        public static void SaveSitesToFile(string path, List<Site> sites)
        {
            try
            {
                XDocument xml = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XComment("Saving date: " + DateTime.Now.ToString(CultureInfo.InvariantCulture)),
                    new XElement("Sites", from Site site in sites
                                          select new XElement("Site", site.address)));
                xml.Save(path);
            }
            catch (Exception e)
            {
                throw new Exception("XML Saving File Error", e);
            }
        }

        public static List<string> LoadEmailsFromFile(string path)
        {
            List<string> Emails = new List<string>();
            if (File.Exists(path))
            {
                try
                {
                    XDocument xml = XDocument.Load(path);
                    Emails = xml.Root.Elements("Email").
                        Select(element => element.Value).ToList();
                    
                    return Emails;
                }
                catch (Exception)
                {
                    return Emails;
                }
            }
            else
            {
                return Emails;
            }
        }

        public static void SaveEmailsToFile(string path, List<string> Emails)
        {
            try
            {
                XDocument xml = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XComment("Saving date: " + DateTime.Now.ToString(CultureInfo.InvariantCulture)),
                    new XElement("Emails", from string email in Emails
                                          select new XElement("Email", email)));
                xml.Save(path);
            }
            catch (Exception e)
            {
                throw new Exception("XML Saving File Error", e);
            }
        }
    }
}
