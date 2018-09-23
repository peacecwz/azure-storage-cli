using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureStorageCLI.Extensions;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Core.Util;
using ShellProgressBar;

namespace AzureStorageCLI.Commands
{
    [Command(Name = "upload", Description = "Upload file to Azure Storage")]
    [HelpOption("-h|--help")]
    public class UploadCommand
    {
        [Required]
        [Argument(0, "filePath", "Upload file path")]
        public string FilePath { get; set; }

        [Argument(1, "containerName", "Container Name")]
        public string Container { get; set; }

        [Option("-n|--name", CommandOptionType.SingleValue, Description = "Choose different account")]
        public string Name { get; set; }

        [Option("--public", CommandOptionType.SingleValue, Description = "Set access level")]
        public bool IsPublic { get; set; }

        public async Task<int> OnExecute(CommandLineApplication cmd)
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                Console.WriteLine("You have to enter your file path for uploading");
                return (int) StatusCodes.UnknownError;
            }

            var fileInfo = new FileInfo(FilePath);
            if (!fileInfo.Exists)
            {
                Console.WriteLine("File not found");
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
            var container = !string.IsNullOrWhiteSpace(Container)
                ? blobClient.GetContainerReference(Container)
                : blobClient.GetRootContainerReference();
            await container.SetPermissionsAsync(new BlobContainerPermissions()
            {
                PublicAccess = (IsPublic)
                    ? BlobContainerPublicAccessType.Blob
                    : BlobContainerPublicAccessType.Off
            });

            string uploadFileName = fileInfo.Name;
            var blob = container.GetBlockBlobReference(uploadFileName);
            int tryCount = 1;
            while (await blob.ExistsAsync())
            {
                if (tryCount == 1)
                {
                    var result = Prompt.GetYesNo($"{uploadFileName} already exist. Would you like to upload continue?",
                        true,
                        ConsoleColor.Yellow);
                    if (!result)
                    {
                        return (int) StatusCodes.UnknownError;
                    }
                }

                uploadFileName = String.Format("{0}-{1}{2}",
                    Path.GetFileNameWithoutExtension(fileInfo.Name), tryCount.ToString(),
                    Path.GetExtension(fileInfo.Name));
                blob = container.GetBlockBlobReference(uploadFileName);
                tryCount++;
            }

            try
            {
                var options = new ProgressBarOptions
                {
                    ForegroundColor = ConsoleColor.Green,
                    ForegroundColorDone = ConsoleColor.DarkGreen,
                    BackgroundColor = ConsoleColor.DarkGray,
                    BackgroundCharacter = '\u2593'
                };
                using (var pbar = new ProgressBar((int) fileInfo.Length,
                    $"{uploadFileName} is uploading 0/{fileInfo.Length.ToSizeString()}", options))
                {
                    await blob.UploadFromFileAsync(fileInfo.FullName, null, null, null, new Progress<StorageProgress>(
                            progress =>
                            {
                                pbar.Tick((int) progress.BytesTransferred,
                                    $"{uploadFileName} is uploading {progress.BytesTransferred.ToSizeString()}/{fileInfo.Length.ToSizeString()}");
                            }),
                        CancellationToken.None);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (int) StatusCodes.UnknownError;
            }

            Console.WriteLine($"Your file has been uploaded: {blob.Uri.ToString()}");

            return (int) StatusCodes.Success;
        }
    }
}