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
    internal static class Program
    {
        /* MUTEX */
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_RESTORE = 9;
        [DllImport("user32.dll")]
        private static extern IntPtr GetLastActivePopup(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool IsWindowEnabled(IntPtr hWnd);
#pragma warning disable IDE0052 // Supprimer les membres privés non lus
        private static Mutex mutex = null;
#pragma warning restore IDE0052 // Supprimer les membres privés non lus

        [Flags]
        private enum EXECUTION_STATE : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
        }
        private static class SleepUtil
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
        }
        private static void PreventSleep()
        {
            if (SleepUtil.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS
                | EXECUTION_STATE.ES_DISPLAY_REQUIRED
                | EXECUTION_STATE.ES_SYSTEM_REQUIRED
                | EXECUTION_STATE.ES_AWAYMODE_REQUIRED) == 0)
            {
                _ = SleepUtil.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS
                    | EXECUTION_STATE.ES_DISPLAY_REQUIRED
                    | EXECUTION_STATE.ES_SYSTEM_REQUIRED);
            }
        }

        [STAThread]
        private static void Main()
        {
            try
            {
                mutex = new Mutex(false, Application.ProductName, out bool createdNew);
                if (!createdNew)
                {
                    SetFocusToPreviousInstance(Application.ProductName);
                    return;
                }
            }
            catch { }

            string ConfigPath = "";
            string[] args = Environment.GetCommandLineArgs();
            bool Config = args.Contains("/config", StringComparer.OrdinalIgnoreCase);
            if (Config)
            {
                Config = false;
                try
                {
                    ConfigPath = args[2];
                    if (File.Exists(ConfigPath)) { Config = true; }
                }
                catch { Config = false; }
                if (Config == true)
                {
                    Properties.Settings appSettings = Properties.Settings.Default;
                    try
                    {
                        if (!File.Exists(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath)) { Properties.Settings.Default.Alerte_Touche_Modifier = 2; Properties.Settings.Default.Save(); }
                        Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                        string appSettingsXmlName = appSettings.Context["GroupName"].ToString();
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
                    }
                    catch { }
                }
            }
            if (Properties.Settings.Default.Veille_Desactive == true) { PreventSleep(); }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmPrincipale myForm = new frmPrincipale();
            Application.Run();
        }
        private static void SetFocusToPreviousInstance(string windowCaption)
        {
            IntPtr hWnd = FindWindow(null, windowCaption);

            if (hWnd != null)
            {
                IntPtr hPopupWnd = GetLastActivePopup(hWnd);

                if (hPopupWnd != null && IsWindowEnabled(hPopupWnd))
                {
                    hWnd = hPopupWnd;
                }
                if (IsIconic(hWnd))
                {
                    _ = ShowWindow(hWnd, SW_RESTORE);
                }
                _ = SetForegroundWindow(hWnd);
            }
        }
    }
}