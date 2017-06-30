using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WebChecker.ViewModel;

namespace WebChecker
{
    public abstract class DialogBox : FrameworkElement, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string nazwaWłasności)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nazwaWłasności));
        }
        #endregion
        protected Action<object> execute = null;
        public string Caption { get; set; }
        protected ICommand show;
        public virtual ICommand Show
        {
            get
            {
                if (show == null) show = new RelayCommand(execute);
                return show;
            }
        }
    }

    public abstract class CommandDialogBox : DialogBox
    {
        public override ICommand Show
        {
            get
            {
                if (show == null) show = new RelayCommand(
               o =>
               {
                   ExecuteCommand(CommandBefore, CommandParameter);
                   execute(o);
                   ExecuteCommand(CommandAfter, CommandParameter);
               });
                return show;
            }
        }
        public static DependencyProperty CommandParameterProperty =
        DependencyProperty.Register("CommandParameter", typeof(object),
        typeof(CommandDialogBox));
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }
        protected static void ExecuteCommand(ICommand command, object commandParameter)
        {
            if (command != null)
                if (command.CanExecute(commandParameter))
                    command.Execute(commandParameter);
        }
        public static DependencyProperty CommandBeforeProperty =
        DependencyProperty.Register("CommandBefore", typeof(ICommand),
        typeof(CommandDialogBox));
        public ICommand CommandBefore
        {
            get
            {
                return (ICommand)GetValue(CommandBeforeProperty);
            }
            set
            {
                SetValue(CommandBeforeProperty, value);
            }
        }
        public static DependencyProperty CommandAfterProperty =
        DependencyProperty.Register("CommandAfter", typeof(ICommand),
        typeof(CommandDialogBox));
        public ICommand CommandAfter
        {
            get
            {
                return (ICommand)GetValue(CommandAfterProperty);
            }
            set
            {
                SetValue(CommandAfterProperty, value);
            }
        }
    }
    public class NotificationDialogBox : CommandDialogBox
    {
        public NotificationDialogBox()
        {
            execute =
            o =>
             {
                 MessageBox.Show((string)o, Caption, MessageBoxButton.OK,
                 MessageBoxImage.Information);
             };
        }
    }
}
