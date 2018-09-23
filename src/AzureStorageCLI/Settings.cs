using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Newtonsoft.Json;

namespace AzureStorageCLI
{
    public class Settings
    {
        public string AccountName { get; set; }
        public string AccountKey { get; set; }

        public void Save()
        {
            var settings = new List<Settings>();
            if (!File.Exists("settings.json"))
            {
                File.WriteAllText("settings.json", "[]");
            }

            var json = File.ReadAllText("settings.json");
            settings.AddRange(JsonConvert.DeserializeObject<List<Settings>>(json));

            if (!IsExist(AccountName))
            {
                settings.Add(new Settings()
                {
                    AccountName = AccountName,
                    AccountKey = AccountKey
                });
            }

            File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
        }

        public static List<Settings> GetAll()
        {
            if (!File.Exists("settings.json"))
            {
                File.WriteAllText("settings.json", "[]");
            }

            var json = File.ReadAllText("settings.json");
            return JsonConvert.DeserializeObject<List<Settings>>(json);
        }

        public static CloudStorageAccount GetAccountByName(string name)
        {
            var settings = GetAll();
            var credentials = settings.FirstOrDefault(x => x.AccountName == name);
            if (credentials!=null)
            {
                return new CloudStorageAccount(new StorageCredentials(credentials.AccountName, credentials.AccountKey),
                    true);
            }

            return null;
        }

        public static bool IsExist(string name)
        {
            var settings = GetAll();
            return settings.FirstOrDefault(x => x.AccountName == name) != null;
        }
    }
}