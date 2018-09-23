using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureStorageCLI.Extensions;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Core.Util;
using ShellProgressBar;

namespace AzureStorageCLI.Commands
{
    [Command(Name = "download", Description = "Download file from Azure Storage")]
    [HelpOption("-h|--help")]
    public class DownloadCommand
    {
        [Argument(0, "blobName", "Blob file name")]
        public string BlobName { get; set; }

        [Argument(1, "containerName", "Container Name")]
        public string ContainerName { get; set; }

        [Argument(2, "filePath", "Download target file path")]
        public string FilePath { get; set; }

        [Option("-n|--name", CommandOptionType.SingleValue, Description = "Choose different account")]
        public string Name { get; set; }

        public async Task<int> OnExecute(CommandLineApplication cmd)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(BlobName))
                {
                    Console.WriteLine("Blob file name is required");
                    return (int) StatusCodes.UnknownError;
                }

                if (!Settings.GetAll().Any())
                {
                    Console.WriteLine("No valid azure storage");
                    return (int) StatusCodes.UnknownError;
                }

                var account = Settings.GetAccountByName(Name);
                if (account == null)
                {
                    var firstCredential = Settings.GetAll().FirstOrDefault();
                    account = Settings.GetAccountByName(firstCredential?.AccountName);
                }

                if (account == null)
                {
                    Console.WriteLine("Invalid Azure Storage account");
                    return (int) StatusCodes.UnknownError;
                }

                var blobClient = account.CreateCloudBlobClient();
                var container = !string.IsNullOrWhiteSpace(ContainerName)
                    ? blobClient.GetContainerReference(ContainerName)
                    : blobClient.GetRootContainerReference();

                var result = await DownloadFile(container, BlobName, FilePath);

                var resultMessage = result
                    ? $"Your file is downloaded successfully."
                    : "Your file cannot downloaded";

                Console.WriteLine(resultMessage);

                return (int) StatusCodes.Success;
            }
            catch (Exception e)
            {
                bool showError = Prompt.GetYesNo("Something an exception happend. Would you like to see detail?", true,
                    ConsoleColor.Red);

                if (showError)
                {
                    Console.WriteLine(e);
                }

                return (int) StatusCodes.UnknownError;
            }
        }

        private string GetBlobName(string filePath)
        {
            return new FileInfo(filePath).Name;
        }

        private async Task<bool> DownloadFile(CloudBlobContainer container, string blobName, string filePath = "")
        {
            try
            {
                var blob = container.GetBlobReference(GetBlobName(filePath));

                var options = new ProgressBarOptions
                {
                    ForegroundColor = ConsoleColor.Green,
                    ForegroundColorDone = ConsoleColor.DarkGreen,
                    BackgroundColor = ConsoleColor.DarkGray,
                    BackgroundCharacter = '\u2593'
                };
                using (var pbar = new ProgressBar((int) blob.Properties.Length,
                    $"{blobName} is downloading", options))
                {
                    string targetPath = !string.IsNullOrWhiteSpace(filePath)
                        ? filePath
                        : blobName;
                    await blob.DownloadToFileAsync(targetPath, FileMode.OpenOrCreate, null, null, null,
                        new Progress<StorageProgress>(
                            progress =>
                            {
                                if (pbar.MaxTicks == 0)
                                {
                                    pbar.MaxTicks = (int) blob.Properties.Length;
                                }

                                pbar.Tick((int) progress.BytesTransferred,
                                    $"{blobName} is downloading {progress.BytesTransferred.ToSizeString()}/{blob.Properties.Length.ToSizeString()}");
                            }), CancellationToken.None);
                    return true;
                }
            }
            catch (Exception e)
            {
                bool showError = Prompt.GetYesNo("Something an exception happend. Would you like to see detail?", true,
                    ConsoleColor.Red);
                if (showError)
                {
                    Console.WriteLine(e);
                }

                return false;
            }
        }
    }
}