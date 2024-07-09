using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace NoPanic
{
    static class Program
    {
        /* MUTEX */
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_RESTORE = 9;
        [DllImport("user32.dll")]
        static extern IntPtr GetLastActivePopup(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool IsWindowEnabled(IntPtr hWnd);
        private static Mutex mutex = null;

        [Flags]
        public enum EXECUTION_STATE : uint {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
        }
        public static class SleepUtil
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
        }
        public static void PreventSleep()
        {
            if (SleepUtil.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS
                | EXECUTION_STATE.ES_DISPLAY_REQUIRED
                | EXECUTION_STATE.ES_SYSTEM_REQUIRED
                | EXECUTION_STATE.ES_AWAYMODE_REQUIRED) == 0)
                SleepUtil.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS
                    | EXECUTION_STATE.ES_DISPLAY_REQUIRED
                    | EXECUTION_STATE.ES_SYSTEM_REQUIRED);
        }

        [STAThread]
        static void Main() {
            try {
                bool createdNew = false;
                mutex = new Mutex(false, Application.ProductName, out createdNew);
                if (!createdNew) {
                    SetFocusToPreviousInstance(Application.ProductName);
                    return;
                }
            } catch { }

            String ConfigPath = "";
            String[] args = Environment.GetCommandLineArgs();
            bool Config = args.Contains("/config", StringComparer.OrdinalIgnoreCase);
            if (Config) {
                Config = false;
                try {
                    ConfigPath = args[2];
                    if (File.Exists(ConfigPath)) { Config = true; }
                } catch { Config = false; }
                if (Config == true) {
                    var appSettings = Properties.Settings.Default;
                    try {
                        if (!File.Exists(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath)) { Properties.Settings.Default.Alerte_Touche = 2; Properties.Settings.Default.Save(); }
                        Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                        String appSettingsXmlName = appSettings.Context["GroupName"].ToString();
                        XDocument import = XDocument.Load(configuration.FilePath);
                        XDocument importFile = XDocument.Load(ConfigPath);
                        IEnumerable<XElement> combinedUnique = import.Descendants("setting").Concat(importFile.Descendants("setting")).GroupBy(x => (string)x.Attribute("name")).Select(g => g.Last());
                        configuration.GetSectionGroup("userSettings")
                            .Sections[appSettingsXmlName]
                            .SectionInformation
                            .SetRawXml("<" + appSettingsXmlName + ">" + string.Concat(combinedUnique) + "</" + appSettingsXmlName + ">");
                        configuration.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("userSettings");
                        appSettings.Reload();
                    } catch { }
                }
            }
            if (Properties.Settings.Default.Veille_Desactive == true) { PreventSleep(); }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmPrincipale myForm = new frmPrincipale();
            Application.Run();
        }
        private static void SetFocusToPreviousInstance(string windowCaption) {
            IntPtr hWnd = FindWindow(null, windowCaption);

            if (hWnd != null) {
                IntPtr hPopupWnd = GetLastActivePopup(hWnd);

                if (hPopupWnd != null && IsWindowEnabled(hPopupWnd)) {
                    hWnd = hPopupWnd;
                }
                if (IsIconic(hWnd)) {
                    ShowWindow(hWnd, SW_RESTORE);
                }
                SetForegroundWindow(hWnd);
            }
        }
    }
}