using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WebChecker.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        private Model.Sites sites = new Model.Sites();
        public ObservableCollection<Model.Site> sitesList { get; set; }

        public Model.Notify notify { get; set; }
        public ObservableCollection<string> Emails { get; set; }
        private Model.Emails emails = new Model.Emails();
        public string smtpStatus { get; set; }
        public string currentPassword { get; set; }

        private BackgroundWorker worker;
        private double interval;
        Timer timer = new Timer();
        public DateTime updatedDate { get; set; }
        public int refreshInterval
        {
            get { return (int)interval / 1000 / 60; }
            set
            {
                interval = value * 1000 * 60;
                Properties.Settings.Default.interval = interval;
                timerSet();
                RaisePropertyChanged("refreshInterval");
            }
        }

        private Model.Site _selectedItem;
        public Model.Site selectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged("selectedItem");
            }
        }

        private bool isNotifyEnabled;
        public bool IsNotifyEnabled
        {
            get { return isNotifyEnabled; }
            set
            {
                isNotifyEnabled = value;
                Properties.Settings.Default.isNotifyEnabled = value;
                RaisePropertyChanged("IsNotifyEnabled");
            }
        }
        


        public ViewModel()
        {
            setVisualProperties();
            IsNotifyEnabled = Properties.Settings.Default.isNotifyEnabled;
            SecureString password = Model.Secure.DecryptString(Properties.Settings.Default.emailPassword);
            notify = new Model.Notify(Properties.Settings.Default.emailAddress, password, Properties.Settings.Default.smtpPort,
                Properties.Settings.Default.enableSsl, Properties.Settings.Default.smtpServer);
            refreshList();
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            interval = Properties.Settings.Default.interval;
            timerSet();
            smtpStatus = "No data";
        }

        private void timerSet()
        {
            timer.Interval = interval;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        private void modelSync()
        {
            sitesList = new ObservableCollection<Model.Site>(sites.SitesList);
            RaisePropertyChanged("sitesList");
            Emails = new ObservableCollection<string>(emails.GetEmails());
            RaisePropertyChanged("Emails");
        }
                
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!worker.IsBusy)
                worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            refreshList();
        }

        private void refreshList()
        {
            foreach (Model.Site site in sites.SitesList)
            {
                Model.Site newSite = Model.Status.GetInfo(site);
                site.status = newSite.status;
                modelSync();
                if (IsNotifyEnabled == true)
                {
                    try
                    {
                        if (!(site.status == "OK") && !(site.status == "Checking"))
                        {
                            notify.email_send(site.address, site.status, emails.GetEmails());
                            smtpStatus = "OK";
                            RaisePropertyChanged("smtpStatus");
                        }
                    }
                    catch (WebException e)
                    {
                        using (var stream = e.Response.GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {
                            smtpStatus = e.ToString();
                        }
                        
                        RaisePropertyChanged("smtpStatus");
                    }
                }
            }
            updatedDate = DateTime.Now;
            RaisePropertyChanged("updatedDate");
        }

        #region VisualProperties
        private void setVisualProperties()
        {
            NotifySettingsVisibility = Visibility.Hidden;
            ToolsVisibility = Visibility.Visible;
        }

        private Visibility notifySettingsVisibility;
        public Visibility NotifySettingsVisibility
        {
            get { return notifySettingsVisibility; }
            set
            {
                notifySettingsVisibility = value;
                RaisePropertyChanged("NotifySettingsVisibility");
            }
        }

        private Visibility toolsVisibility;
        public Visibility ToolsVisibility
        {
            get { return toolsVisibility; }
            set
            {
                toolsVisibility = value;
                RaisePropertyChanged("ToolsVisibility");
            }
        }

        #endregion


        #region Commands
        private ICommand addSite;
        public ICommand AddSite
        {
            get
            {
                if (addSite == null)
                    addSite = new RelayCommand(
                        o =>
                        {
                            string address = (string)o;
                            sites.AddSite(address);
                            if (!worker.IsBusy)
                                worker.RunWorkerAsync();
                            modelSync();
                        },
                        o =>
                        {
                            string address = (string)o;
                            Uri uriResult;
                            bool result = Uri.TryCreate(address, UriKind.Absolute, out uriResult)
                                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                            return result;
                        });
                return addSite;
            }
        }

        private ICommand deleteSite;
        public ICommand DeleteSite
        {
            get
            {
                if (deleteSite == null)
                    deleteSite = new RelayCommand(
                        o =>
                        {
                            Model.Site site = (Model.Site)o;
                            sites.SitesList.Remove(site);
                            modelSync();
                        },
                        o =>
                        {
                            Model.Site site = (Model.Site)o;
                            if (sites.SitesList.Contains(site))
                                return true;
                            else return false;
                        });
                return deleteSite;
            }
        }

        private ICommand closeApp;
        public ICommand CloseApp
        {
            get
            {
                if (closeApp == null)
                    closeApp = new RelayCommand(
                        o =>
                        {
                            sites.SaveSites();
                            emails.SaveToXml();
                            Properties.Settings.Default.Save();
                        });
                return closeApp;
            }
        }

        private ICommand refresh;
        public ICommand Refresh
        {
            get
            {
                if (refresh == null)
                    refresh = new RelayCommand(
                        o =>
                        {
                            if (!worker.IsBusy)
                                worker.RunWorkerAsync();
                        });
                return refresh;
            }
        }

        private ICommand addEmail;
        public ICommand AddEmail
        {
            get
            {
                if (addEmail == null)
                    addEmail = new RelayCommand(
                        o =>
                        {
                            string email = (string)o;
                            emails.AddEmail(email);
                            modelSync();
                        });
                return addEmail;
            }
        }

        private ICommand removeEmail;
        public ICommand RemoveEmail
        {
            get
            {
                if (removeEmail == null)
                    removeEmail = new RelayCommand(
                        o =>
                        {
                            string email = (string)o;
                            emails.RemoveEmail(email);
                            modelSync();
                        },
                        o =>
                        {
                            string email = (string)o;
                            if (emails.GetEmails().Contains(email)) return true;
                            else return false;
                        });
                return removeEmail;
            }
        }

        private ICommand showNotifySettings;
        public ICommand ShowNotifySettings
        {
            get
            {
                if (showNotifySettings == null)
                    showNotifySettings = new RelayCommand(
                        o =>
                        {
                            NotifySettingsVisibility = Visibility.Visible;
                            ToolsVisibility = Visibility.Hidden;
                        });
                return showNotifySettings;
            }
        }

        private ICommand hideNotifySettings;
        public ICommand HideNotifySettings
        {
            get
            {
                if (hideNotifySettings == null)
                    hideNotifySettings = new RelayCommand(
                        o =>
                        {
                            NotifySettingsVisibility = Visibility.Hidden;
                            ToolsVisibility = Visibility.Visible;
                        });
                return hideNotifySettings;
            }
        }

        private ICommand saveSMTPSettings;
        public ICommand SaveSMTPSettings
        {
            get
            {
                if (saveSMTPSettings == null)
                    saveSMTPSettings = new RelayCommand(
                        o =>
                        {
                            Model.Notify smtpSettings = (Model.Notify)o;
                            notify = smtpSettings; 
                            Properties.Settings.Default.emailAddress = smtpSettings.address;
                            Properties.Settings.Default.smtpPort = smtpSettings.smtpPort;
                            Properties.Settings.Default.smtpServer = smtpSettings.smtpServer;
                            Properties.Settings.Default.enableSsl = smtpSettings.enableSsl;
                            Properties.Settings.Default.emailPassword = Model.Secure.EncryptString(smtpSettings.password);
                            RaisePropertyChanged("notify");
                            currentPassword = Model.Secure.SecurePasswordToString(smtpSettings.password);
                            RaisePropertyChanged("currentPassword");
                        });
                return saveSMTPSettings;
            }
        }
        

        public string savedText { get; set; }
        
        #endregion

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
