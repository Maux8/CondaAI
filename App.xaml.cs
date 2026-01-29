using System.Globalization;
using Microsoft.Maui.Storage;

namespace Conda_AI
{
    /// <summary>
    /// Haupteinstiegspunkt der MAUI-Anwendung.
    /// Initialisiert die App, lädt Lizenzen und setzt die bevorzugte Sprache.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Erstellt eine neue Instanz der App-Klasse.
        /// Registriert ggf. die SyncFusion-Lizenz und stellt die Sprache basierend auf gespeicherten Einstellungen ein.
        /// </summary>
        public App()
        {
            // Load and register SyncFusion license
            var syncFusionKey = DotNetEnv.Env.GetString("SYNC_FUSION_KEY");
            if (!string.IsNullOrEmpty(syncFusionKey))
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncFusionKey);

            // Set the saved app language (default: de)
            var lang = Preferences.Get("AppLanguage", "de");
            var culture = new CultureInfo(lang);

            InitializeComponent();
        }

        /// <summary>
        /// Erstellt das Hauptfenster der App und lädt die AppShell.
        /// </summary>
        /// <param name="activationState">Aktivierungszustand, z. B. für Deep-Linking oder App-Reaktivierung.</param>
        /// <returns>Ein neues Fenster mit dem AppShell als Startseite.</returns>
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
