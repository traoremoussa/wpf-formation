using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KodiatechFolderLock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AuthService _auth = new AuthService();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonAddName_Click(object sender, RoutedEventArgs e)
        {
            // Récupère les valeurs depuis l'interface
            var username = txtUsername?.Text?.Trim() ?? string.Empty;
            var password = pwdPassword?.Password ?? string.Empty;

            // Auth via AuthService (simple SHA-256 hash stored in AppData)
            var stored = _auth.LoadHash();
            if (string.IsNullOrEmpty(stored))
            {
                // first run: create password
                var hash = _auth.Hash(password);
                _auth.SaveHash(hash);
                System.Windows.MessageBox.Show("Mot de passe enregistré. Vous êtes connecté.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                var dashboard = new DashboardWindow();
                dashboard.Show();
                this.Close();
                return;
            }

            if (_auth.Verify(password, stored))
            {
                var dashboard = new DashboardWindow();
                dashboard.Show();
                this.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("Nom d'utilisateur ou mot de passe invalide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}