using SolidWorksTools;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace AxionFrame
{
    public partial class SwAddin
    {
        private const string AddinsRegistryPathTemplate = "SOFTWARE\\SolidWorks\\Addins\\{{{0}}}";
        private const string AddinsStartupRegistryPathTemplate = "Software\\SolidWorks\\AddInsStartup\\{{{0}}}";
        private const string RegistryValueDescription = "Description";
        private const string RegistryValueTitle = "Title";

        #region SolidWorks Registration
        [ComRegisterFunctionAttribute]
        public static void RegisterFunction(Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            try
            {
                SwAddinAttribute addinAttributes = ResolveAddinAttribute();

                string addinsKeyPath = string.Format(AddinsRegistryPathTemplate, t.GUID.ToString());
                using (RegistryKey addinsKey = Registry.LocalMachine.CreateSubKey(addinsKeyPath))
                {
                    if (addinsKey == null)
                    {
                        throw new InvalidOperationException("SolidWorks add-in registry key could not be created.");
                    }

                    addinsKey.SetValue(null, 0);
                    addinsKey.SetValue(RegistryValueDescription, addinAttributes.Description);
                    addinsKey.SetValue(RegistryValueTitle, addinAttributes.Title);
                }

                string startupKeyPath = string.Format(AddinsStartupRegistryPathTemplate, t.GUID.ToString());
                using (RegistryKey startupKey = Registry.CurrentUser.CreateSubKey(startupKeyPath))
                {
                    if (startupKey == null)
                    {
                        throw new InvalidOperationException("SolidWorks startup registry key could not be created.");
                    }

                    startupKey.SetValue(null, Convert.ToInt32(addinAttributes.LoadAtStartup), RegistryValueKind.DWord);
                }
            }
            catch (Exception ex)
            {
                ReportRegistrationFailure("registering", ex);
            }
        }

        [ComUnregisterFunctionAttribute]
        public static void UnregisterFunction(Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            try
            {
                string addinsKeyPath = string.Format(AddinsRegistryPathTemplate, t.GUID.ToString());
                Registry.LocalMachine.DeleteSubKey(addinsKeyPath, false);

                string startupKeyPath = string.Format(AddinsStartupRegistryPathTemplate, t.GUID.ToString());
                Registry.CurrentUser.DeleteSubKey(startupKeyPath, false);
            }
            catch (Exception ex)
            {
                ReportRegistrationFailure("unregistering", ex);
            }

        }

        private static SwAddinAttribute ResolveAddinAttribute()
        {
            Type addinType = typeof(SwAddin);
            foreach (Attribute attr in addinType.GetCustomAttributes(false))
            {
                SwAddinAttribute addinAttribute = attr as SwAddinAttribute;
                if (addinAttribute != null)
                {
                    return addinAttribute;
                }
            }

            throw new InvalidOperationException("SwAddinAttribute is not defined on SwAddin.");
        }

        private static void ReportRegistrationFailure(string action, Exception ex)
        {
            string message = "There was a problem " + action + " the add-in." + Environment.NewLine + "\"" + ex.Message + "\"";
            Console.WriteLine(message);
            System.Windows.Forms.MessageBox.Show(message);
        }

        #endregion
    }
}
