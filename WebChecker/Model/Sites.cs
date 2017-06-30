using System.Collections.Generic;
using System.ComponentModel;

namespace WebChecker.Model
{
    public class Sites
    {
        public List<Site> SitesList { get; set; }
        string path = "sites.xml";
        
        public Sites()
        {
            SitesList = XmlFiles.LoadSitesFromFile(path);
        }

        public void AddSite(string url)
        {
            SitesList.Add(new Site(url));
        }

        public void RemoveSite(Site site)
        {
            SitesList.Remove(site);
        }
       
        public void SaveSites()
        {
            XmlFiles.SaveSitesToFile(path, SitesList);
        }
    }
}
