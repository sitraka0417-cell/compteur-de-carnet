# Compteur carnet (Windows)

Application de bureau Windows (WPF / .NET 8) portée depuis le projet Android
d'origine ("NewProject34" / Sketchware). Elle permet de :

- importer des carnets depuis des fichiers JSON ;
- afficher la liste des carnets disponibles et les supprimer ;
- répartir les carnets entre plusieurs "onglets" (agents) et suivre
  l'avancement feuillet par feuillet ("Vita") ;
- consulter l'historique des carnets terminés.

Les données sont sauvegardées automatiquement dans
`%AppData%\CompteurCarnet\data.json`.

## Compiler depuis GitHub

### Option A — GitHub Actions (recommandé, aucune installation requise)

1. Créez un dépôt GitHub et poussez ce dossier dedans :
   ```
   git init
   git add .
   git commit -m "Compteur carnet - version Windows"
   git branch -M main
   git remote add origin <URL_DE_VOTRE_DEPOT>
   git push -u origin main
   ```
2. Le workflow `.github/workflows/build.yml` se déclenche automatiquement
   (`windows-latest`) et compile l'application.
3. Allez dans l'onglet **Actions** du dépôt, ouvrez la dernière exécution,
   puis téléchargez l'artefact **CompteurCarnet-windows** : il contient
   `CompteurCarnet.exe`, un exécutable autonome (aucune installation de
   .NET nécessaire sur l'ordinateur cible).
4. Pour obtenir un lien de téléchargement permanent, créez un tag de version
   (`git tag v1.0.0 && git push origin v1.0.0`) : une **Release** GitHub sera
   créée automatiquement avec `CompteurCarnet-windows.zip`.

### Option B — Compiler localement sous Windows

Prérequis : [.NET 8 SDK](https://dotnet.microsoft.com/download) avec la
charge de travail de développement de bureau (`desktop` / WPF).

```powershell
git clone <URL_DE_VOTRE_DEPOT>
cd CompteurCarnet
dotnet restore
dotnet build -c Release
```

Pour obtenir un `.exe` autonome à distribuer :

```powershell
dotnet publish CompteurCarnet\CompteurCarnet.csproj -c Release -r win-x64 `
  --self-contained true -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true -o publish
```

L'exécutable se trouve alors dans `publish\CompteurCarnet.exe`.

## Installer sur Windows

Il n'y a pas besoin d'un installeur classique : `CompteurCarnet.exe` est un
exécutable autonome.

1. Copiez `CompteurCarnet.exe` où vous voulez sur l'ordinateur (par exemple
   `C:\Program Files\CompteurCarnet\` ou simplement le Bureau).
2. Double-cliquez dessus pour le lancer. Windows peut afficher un
   avertissement SmartScreen la première fois car l'exécutable n'est pas
   signé numériquement : cliquez sur **Informations complémentaires** puis
   **Exécuter quand même**.
3. (Optionnel) Créez un raccourci vers l'exécutable sur le Bureau ou dans le
   menu Démarrer.

## Structure du projet

```
CompteurCarnet.sln
CompteurCarnet/
  CompteurCarnet.csproj
  MainWindow.xaml(.cs)        - menu principal
  ListeWindow.xaml(.cs)       - importation et liste des carnets
  TachesWindow.xaml(.cs)      - onglets / suivi de la progression
  HistoriqueWindow.xaml(.cs)  - historique des carnets terminés
  SelectCarnetWindow.xaml(.cs)- boîte de sélection d'un carnet
  Models/                     - Carnet, HistoriqueEntry, AppData, import JSON
  Services/
    DataStore.cs              - sauvegarde/chargement (remplace SharedPreferences)
    CommunesData.cs           - données communes/fokontany (région Itasy)
.github/workflows/build.yml   - compilation automatique via GitHub Actions
```

## Format du fichier JSON à importer

```json
{
  "communes": [
    {
      "code_commune": "130101",
      "fokontany": [
        {
          "code_fokontany": "13010101",
          "carnets": [
            {
              "num_carnet": "001",
              "feuillets": [
                { "num_feuillet": "0001", "statut": "" },
                { "num_feuillet": "0002", "statut": "annulé" }
              ]
            }
          ]
        }
      ]
    }
  ]
}
```

Un feuillet est considéré comme "valide" quand son champ `statut` est vide
ou absent (comme dans l'application Android d'origine).
