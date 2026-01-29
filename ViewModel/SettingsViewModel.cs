using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Application = Microsoft.Maui.Controls.Application;

namespace Conda_AI.ViewModel
{
    /// <summary>
    /// ViewModel für die Einstellungsseite der App.
    /// Verwaltet Benutzerpräferenzen wie Benachrichtigungen, Dark Mode, Benutzername
    /// sowie zugehörige Befehle zur Änderung und Rücksetzung.
    /// </summary>
    public class SettingsViewModel : BindableObject
    {
        private bool notificationsEnabled;
        private bool isDarkMode;
        private string userName;

        // Der Pfad zur Datei, die gelöscht werden soll genrell für Maui
        private static readonly string NameFilePath = Path.Combine(FileSystem.AppDataDirectory, "CondaAIUser.json");

        // Binding für den Benutzernamen
        /// <summary>
        /// Benutzername des aktuell eingeloggten Nutzers.
        /// Wird in den App-Einstellungen gespeichert und aktualisiert die UI bei Änderung.
        /// </summary>
        public string UserName
        {
            get => userName;
            set
            {
                if (userName != value)
                {
                    userName = value;
                    Preferences.Set("UserName", value);
                    OnPropertyChanged();
                }
            }
        }

        // Der KI-Modellname ist fest
        /// <summary>
        /// Gibt den fest definierten Namen des verwendeten KI-Modells zurück.
        /// </summary>
        public string ModelName => "llama3";

        // Binding für Benachrichtigungen
        /// <summary>
        /// Gibt an, ob Benachrichtigungen aktiviert sind.
        /// Änderungen werden automatisch gespeichert und an die UI weitergeleitet.
        /// </summary>
        public bool NotificationsEnabled
        {
            get => notificationsEnabled;
            set
            {
                if (notificationsEnabled != value)
                {
                    notificationsEnabled = value;
                    Preferences.Set("Notifications", value);
                    OnPropertyChanged();
                }
            }
        }

        // Binding für den Dark-Mode-Schalter
        /// <summary>
        /// Gibt an, ob der Dark Mode aktiv ist.
        /// Änderungen speichern den Zustand und setzen das App-Theme entsprechend.
        /// </summary>
        public bool IsDarkMode
        {
            get => isDarkMode;
            set
            {
                if (isDarkMode != value)
                {
                    isDarkMode = value;
                    Preferences.Set("IsDarkMode", value);

                    // Das Design sofort anwenden
                    App.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
                    OnPropertyChanged();
                }
            }
        }

        // Befehl zum Löschen des Caches
        /// <summary>
        /// Befehl zum Löschen aller gespeicherten Einstellungen (App-Cache).
        /// Zeigt anschließend eine Bestätigungsmeldung.
        /// </summary>
        public ICommand ClearCacheCommand => new Command(() =>
        {
            Preferences.Clear();
            Application.Current.MainPage.DisplayAlert("Info", "Cache wurde geleert", "OK");
        });

        // Befehl zum Zurücksetzen der App-Einstellungen
        /// <summary>
        /// Befehl zum Zurücksetzen aller Einstellungen auf die Standardwerte.
        /// Dark Mode wird deaktiviert, Benachrichtigungen aktiviert.
        /// </summary>
        public ICommand ResetAppCommand => new Command(() =>
        {
            Preferences.Clear();
            NotificationsEnabled = true;
            IsDarkMode = false;
            Application.Current.MainPage.DisplayAlert("Info", "App wurde zurückgesetzt", "OK");
        });

        // Befehl, um den Namen des Benutzers zu ändern
        /// <summary>
        /// Befehl zum Löschen der lokalen Namensdatei und Zurücksetzen des Benutzernamens.
        /// Fragt den Nutzer vorher um Bestätigung.
        /// </summary>
        public ICommand ChangeNameCommand => new Command(async () => await ChangeNameAsync());

        /// <summary>
        /// Fragt den Nutzer, ob er seinen Benutzernamen zurücksetzen möchte.
        /// Löscht bei Zustimmung die entsprechende Datei und leert den gespeicherten Namen.
        /// Zeigt Erfolg oder Fehler via Dialogfenster.
        /// </summary>
        private async Task ChangeNameAsync()
        {
            // Frage den Benutzer, ob er seinen Namen ändern möchte
            var result = await Application.Current.MainPage.DisplayAlert(
                "Name ändern",
                "Möchtest du deinen aktuellen Namen löschen und ihn neu initialisieren?",
                "Ja",
                "Nein");

            if (result)
            {
                try
                {
                    // Überprüfen, ob die Datei existiert
                    if (File.Exists(NameFilePath))
                    {
                        // Lösche die Datei
                        File.Delete(NameFilePath);
                        // Zeige eine Bestätigungsmeldung an
                        await Application.Current.MainPage.DisplayAlert(
                            "Name neu initialisiert",
                            "Dein Name wird beim nächsten Start der App neu initialisiert.",
                            "OK");

                        // Setze den Benutzernamen im Preferences zurück, falls gewünscht
                        UserName = string.Empty; // Dies setzt den Namen in der App zurück.
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            "Fehler",
                            "Keine Namensdatei zum Löschen gefunden.",
                            "OK");
                    }
                }
                catch (Exception ex)
                {
                    // Fehlerbehandlung, falls beim Löschen ein Fehler auftritt
                    await Application.Current.MainPage.DisplayAlert(
                        "Fehler",
                        $"Ein Fehler ist aufgetreten: {ex.Message}",
                        "OK");
                }
            }
        }

        // Initialisierung der Einstellungen
        /// <summary>
        /// Konstruktor, der die Benutzereinstellungen aus dem lokalen Speicher lädt.
        /// Wendet das Theme gemäß Einstellung automatisch an.
        /// </summary>
        public SettingsViewModel()
        {
            NotificationsEnabled = Preferences.Get("Notifications", true);
            IsDarkMode = Preferences.Get("IsDarkMode", false);
            UserName = Preferences.Get("UserName", string.Empty);

            // Design aus den Benutzereinstellungen übernehmen
            App.Current.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;
        }
    }
}
