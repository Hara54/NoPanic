using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;

namespace NoPanic
{
    static class Program
    {
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
            string ConfigPath = "";
            string[] args = Environment.GetCommandLineArgs();
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
                        if (!File.Exists(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath)) { Properties.Settings.Default.Port_Alerte = 15000; Properties.Settings.Default.Save(); }
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
                    } catch { }
                }
            }
            if (Properties.Settings.Default.Veille_Desactive == true) { PreventSleep(); }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmPrincipale myForm = new frmPrincipale();
            Application.Run();
        }
    }
}