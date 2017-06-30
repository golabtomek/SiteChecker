using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WebChecker.Model
{
    public class Checker : INotifyPropertyChanged
    {
        public Sites sites { get; set; } = new Sites();

        public ViewModel()
        {
            modelSync();
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            timer.Interval = interval;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        private void modelSync()
        {
            sites.SitesList = new ObservableCollection<Model.Site>(sites.SitesList);
            RaisePropertyChanged("sitesList");
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!worker.IsBusy)
                worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            refreshList();
        }

        void refreshList()
        {
            foreach (Model.Site site in sites.SitesList)
            {
                site.status = Model.Status.GetStatus(site.address);
                modelSync();
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
