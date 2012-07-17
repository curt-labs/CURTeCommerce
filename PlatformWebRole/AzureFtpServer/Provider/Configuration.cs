using System;
using System.Configuration;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace AzureFtpServer.Provider
{
    public enum Modes
    {
        Live,
        Debug,
        Development
    }

    public class StorageProviderConfiguration
    {
        
        public static string StorageProviderName
        {
            get {
                return RoleEnvironment.IsAvailable ? RoleEnvironment.GetConfigurationSettingValue("StorageProviderName") : ConfigurationManager.AppSettings["StorageProviderName"];
            }
        }

        public static string AccountKey
        {
            get
            {
                return RoleEnvironment.IsAvailable ? RoleEnvironment.GetConfigurationSettingValue("AccountKey") : ConfigurationManager.AppSettings["AccountKey"];
            }
        }

        public static string AccountName
        {
            get
            {
                return RoleEnvironment.IsAvailable ? RoleEnvironment.GetConfigurationSettingValue("AccountName") : ConfigurationManager.AppSettings["AccountName"];
            }
        }

        public static Modes Mode
        {
            get
            {
                return RoleEnvironment.IsAvailable ? (Modes)Enum.Parse(typeof(Provider.Modes), RoleEnvironment.GetConfigurationSettingValue("Mode")) : (Modes)Enum.Parse(typeof(Provider.Modes),ConfigurationManager.AppSettings["Mode"]);
            }
        }

        public static string BaseUri
        {
            get
            {
                return RoleEnvironment.IsAvailable ? RoleEnvironment.GetConfigurationSettingValue("BaseUri") : ConfigurationManager.AppSettings["BaseUri"];
            }
        }

        public static bool UseHttps
        {
            get
            {
                return RoleEnvironment.IsAvailable ? bool.Parse(RoleEnvironment.GetConfigurationSettingValue("UseHttps")) : bool.Parse(ConfigurationManager.AppSettings["UseHttps"]);
            }
        }

        public static bool UseAsynchMethods
        {
            get
            {
                return RoleEnvironment.IsAvailable ? bool.Parse(RoleEnvironment.GetConfigurationSettingValue("UseAsynchMethods")) : bool.Parse(ConfigurationManager.AppSettings["UseAsynchMethods"]);
            }
        }

    }
}