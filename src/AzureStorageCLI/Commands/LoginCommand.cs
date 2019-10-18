using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace AzureStorageCLI.Commands
{
    [Command(Name = "login", Description = "Login to Azure Storage")]
    [HelpOption("-h|--help")]
    public class LoginCommand
    {
        [Argument(0, "accountName", "Azure Storage Account Name")]
        public string Name { get; set; }

        [Argument(1, "accountKey", "Azure Storage Account Key")]
        public string Key { get; set; }

        public int OnExecute(CommandLineApplication app)
        {
            try
            {
                if (Settings.IsExist(Name))
                {
                    Console.WriteLine($"Your {Name} already exists");
                    return (int) StatusCodes.UnknownError;
                }

                var credentials = new StorageCredentials(Name, Key);
                var account = new CloudStorageAccount(credentials, true);
                account.CreateCloudBlobClient();
                var settings = new Settings()
                {
                    AccountName = Name,
                    AccountKey = Key
                };
                settings.Save();
                Console.WriteLine($"Your {Name} is added");
            }
            catch
            {
                Console.WriteLine("Unhandled exception");
            }

            return (int) StatusCodes.Success;
        }
    }
}