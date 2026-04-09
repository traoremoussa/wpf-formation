using System.IO;
using System.Windows;
using System.Security.AccessControl;
using System.Security.Principal;

namespace KodiatechFolderLock
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.DashboardViewModel();
        }
        private void BtnDoSomething_Click(object sender, RoutedEventArgs e)
        {
            ToggleFolderLock();
        }

        // Exposed method for ViewModel to call
        


        //***************** CLEANUP *****************//



        public void ToggleFolderLock()
        {
            using var dlg = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Sélectionnez le dossier à verrouiller / déverrouiller",
                UseDescriptionForTitle = true
            };

            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK ||
                string.IsNullOrWhiteSpace(dlg.SelectedPath))
                return;

            var folderPath = dlg.SelectedPath;

            try
            {
                var dirInfo = new DirectoryInfo(folderPath);

                // Stockage centralisé (SAFE)
                var appData = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "KodiatechFolderLock"
                );
                Directory.CreateDirectory(appData);

                var markerFile = Path.Combine(appData, GetSafeFileName(folderPath) + ".sddl");

                if (File.Exists(markerFile))
                {
                    // 🔓 UNLOCK
                    UnlockFolder(dirInfo, markerFile);
                }
                else
                {
                    // 🔒 LOCK
                    LockFolder(dirInfo, markerFile);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void LockFolder(DirectoryInfo dirInfo, string markerFile)
        {
            try
            {
                // 1. Sauvegarde ACL (robuste)
                var currentSec = dirInfo.GetAccessControl();
                var originalSddl = currentSec.GetSecurityDescriptorSddlForm(AccessControlSections.All);

                var tempFile = markerFile + ".tmp";
                File.WriteAllText(tempFile, originalSddl);
                File.Move(tempFile, markerFile, true);

                // 2. Nouvelle sécurité
                var newSec = new DirectorySecurity();
                newSec.SetAccessRuleProtection(true, false);

                // SYSTEM
                newSec.AddAccessRule(new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null),
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                // ADMIN
                newSec.AddAccessRule(new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                // ❌ PAS d’accès pour l’utilisateur courant → vrai lock

                dirInfo.SetAccessControl(newSec);

                System.Windows.MessageBox.Show("Dossier verrouillé 🔒",
                    "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (UnauthorizedAccessException)
            {
                System.Windows.MessageBox.Show("Lance l'application en ADMIN.",
                    "Accès refusé", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UnlockFolder(DirectoryInfo dirInfo, string markerFile)
        {
            try
            {
                if (!File.Exists(markerFile))
                    throw new Exception("Fichier de restauration introuvable.");

                var sddl = File.ReadAllText(markerFile);

                if (string.IsNullOrWhiteSpace(sddl))
                    throw new Exception("SDDL invalide.");

                var sec = new DirectorySecurity();
                sec.SetSecurityDescriptorSddlForm(sddl, AccessControlSections.All);

                dirInfo.SetAccessControl(sec);

                File.Delete(markerFile);

                System.Windows.MessageBox.Show("Dossier déverrouillé 🔓",
                    "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur unlock : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private string GetSafeFileName(string path)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                path = path.Replace(c, '_');
            }
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(path));
        }

    }
}