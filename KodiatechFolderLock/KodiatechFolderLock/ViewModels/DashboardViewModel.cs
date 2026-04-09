using System.Windows;
using System.Windows.Input;
using KodiatechFolderLock.ViewModels;

namespace KodiatechFolderLock.ViewModels
{
    public class DashboardViewModel
    {
        public ICommand LockCommand { get; }

        public DashboardViewModel()
        {
            LockCommand = new RelayCommand(OnLock);
        }

        private void OnLock(object? parameter)
        {
            if (parameter is Window win)
            {
                // call existing window handler by invoking the button click through code-behind equivalent
                var window = win as KodiatechFolderLock.DashboardWindow;
                window?.ToggleFolderLock();
            }
        }
    }
}
