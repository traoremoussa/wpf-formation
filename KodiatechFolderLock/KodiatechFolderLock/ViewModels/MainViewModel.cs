using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using KodiatechFolderLock.ViewModels;

namespace KodiatechFolderLock.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
        private readonly AuthService _auth = new AuthService();

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        // Note: PasswordBox binding is not direct; we pass the Window as CommandParameter and read PasswordBox in code-behind helper
        public RelayCommand LoginCommand { get; }

        public MainViewModel()
        {
            LoginCommand = new RelayCommand(OnLogin);
        }

        private void OnLogin(object? parameter)
        {
            if (parameter is Window win)
            {
                var pwdBox = win.FindName("pwdPassword") as System.Windows.Controls.PasswordBox;
                var password = pwdBox?.Password ?? string.Empty;

                var stored = _auth.LoadHash();
                if (string.IsNullOrEmpty(stored))
                {
                    var hash = _auth.Hash(password);
                    _auth.SaveHash(hash);
                    System.Windows.MessageBox.Show("Mot de passe enregistré. Vous êtes connecté.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    var dashboard = new DashboardWindow();
                    dashboard.Show();
                    win.Close();
                    return;
                }

                if (_auth.Verify(password, stored))
                {
                    var dashboard = new DashboardWindow();
                    dashboard.Show();
                    win.Close();
                }
                else
                {
                    System.Windows.MessageBox.Show("Nom d'utilisateur ou mot de passe invalide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
