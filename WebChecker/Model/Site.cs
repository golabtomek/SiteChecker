namespace WebChecker.Model
{
    public class Site
    {
        public string address { get; set; }
        public string status { get; set; }
        public int responseTime { get; set; }

        public Site(string address)
        {
            this.address = address;
            status = "Checking";
        }
    }
}
