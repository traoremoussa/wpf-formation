# KodiatechFolderLock - Mini application WPF

Ce projet est une petite application WPF (.NET 10) qui montre une page de connexion et, après authentification réussie, ouvre une fenêtre "dashboard".

## Structure principale

- `App.xaml` / `App.xaml.cs`
  - Point d'entrée WPF. Configure les ressources applicatives et démarrage.

- `MainWindow.xaml` / `MainWindow.xaml.cs`
  - Fenêtre de connexion (UI) avec les contrôles : `TextBox` pour le nom d'utilisateur (`txtUsername`) et `PasswordBox` pour le mot de passe (`pwdPassword`).
  - Le gestionnaire `ButtonAddName_Click` valide les identifiants (implémentation actuelle simple et codée en dur).
  - En cas de succès, la fenêtre de connexion se ferme et la `DashboardWindow` s'ouvre.

- `DashboardWindow.xaml` / `DashboardWindow.xaml.cs`
  - Fenêtre affichée après connexion réussie.
  - Contient un `Button` (`btnDoSomething`) qui déclenche une action (actuellement affiche un `MessageBox`).
  - Contient un `Button` (`btnDoSomething`) qui permet de verrouiller ou déverrouiller un dossier sélectionné.
    - Le mécanisme actuel : création/suppression d'un fichier marqueur `.kodiatech_locked` dans le dossier et modification des ACL (contrôle d'accès) du dossier pour restreindre l'accès.
    - Si le fichier marqueur existe, l'action déverrouille le dossier en réactivant l'héritage des ACL et en restaurant les règles héritées. Sinon elle verrouille le dossier en désactivant l'héritage et en autorisant uniquement l'utilisateur courant et les Administrateurs.

- `AssemblyInfo.cs`
  - Métadonnées d'assemblage (version, informations). Remarque : avec les SDK modernes, certaines infos peuvent être dans le fichier projet.

## Comment ça fonctionne

1. L'application démarre et affiche `MainWindow` (la page de connexion).
2. L'utilisateur saisit `txtUsername` et `pwdPassword` puis clique sur le bouton "Se connecter".
3. Le code-behind vérifie les identifiants. Exemple actuel : `username == "admin" && password == "password"`.
4. Si la validation réussit, la `DashboardWindow` est instanciée et affichée via `dashboard.Show()` puis `this.Close()` ferme la fenêtre de connexion.

## Exécution

- Ouvrir la solution dans Visual Studio (ou dotnet CLI) et exécuter le projet WPF ciblant `.NET 10`.
- Identifiants de test : utilisateur `admin`, mot de passe `password`.

## Améliorations recommandées

- Remplacer la validation codée en dur par une vérification sécurisée (base de données, service web, fichier chiffré).
- Implémenter la gestion des erreurs, verrouillage après plusieurs échecs, et messages d'aide côté UI.
- Transmettre le nom d'utilisateur ou d'autres données au `DashboardWindow` pour personnaliser l'affichage.

## Verrouillage de dossier (implémentation actuelle)

- L'utilisateur clique sur le bouton du dashboard, choisit un dossier via un dialogue.
- L'application crée un fichier `.kodiatech_locked` dans le dossier pour marquer qu'il est verrouillé, puis modifie les ACL du dossier : désactivation de l'héritage et assignation d'un accès `FullControl` uniquement à l'utilisateur courant et au groupe Administrators.
- Pour déverrouiller, l'application supprime le fichier marqueur et réactive l'héritage des ACL (en conservant les règles héritées).

Remarques de sécurité et limites :

- Ce mécanisme est simple et ne remplace pas une véritable fonctionnalité de chiffrement ou de gestion des permissions. Les utilisateurs expérimentés peuvent afficher les fichiers cachés ou modifier les attributs manuellement.
- Les opérations peuvent nécessiter des droits administratifs selon l'emplacement du dossier. Des erreurs d'accès peuvent se produire.
- Pour une protection réelle, envisager le chiffrement (EFS, AES côté application) et une gestion stricte des permissions.

Si vous voulez, j'ajoute la navigation avec passage de paramètres ou je sécurise l'authentification.
