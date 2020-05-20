using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyMIDIController
{
    /// <summary>
    /// Runs program at startup by writing a registry value into 
    /// HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run
    /// </summary>
    /// <remarks>
    /// Source: https://stackoverflow.com/questions/5089601/how-to-run-a-c-sharp-application-at-windows-startup
    /// </remarks>
    static class RunAtStartup
    {
        public static bool IsEnabled
        {
            get
            {
                using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false))
                {
                    return (registryKey.GetValue(APP_NAME, null) != null);
                }
                    
            }
            set
            {
                using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (value)
                    {
                        registryKey.SetValue(APP_NAME, Application.ExecutablePath);
                    }
                    else
                    {
                        if (registryKey.GetValue(APP_NAME, null) != null)
                            registryKey.DeleteValue(APP_NAME);
                    }
                }
            }
        }

        const string APP_NAME = "MyMIDIController";
    }
}
