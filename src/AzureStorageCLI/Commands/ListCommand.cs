using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using CTable;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace AzureStorageCLI.Commands
{
    [Command(Name = "list", Description = "List of containers")]
    [HelpOption("-h|--help")]
    public class ListCommand
    {
        [Argument(0, "containerName", "Azure storage container name")]
        public string ContainerName { get; set; }

        [Option("-n|--name", CommandOptionType.SingleValue, Description = "Account Name")]
        public string Name { get; set; }

        public async Task<int> OnExecute(CommandLineApplication cmd)
        {
            try
            {
                if (!Settings.GetAll().Any())
                {
                    Console.WriteLine("No valid azure storage");
                    return (int)StatusCodes.UnknownError;
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
                    return (int)StatusCodes.UnknownError;
                }

                var blobClient = account.CreateCloudBlobClient();
                if (!string.IsNullOrWhiteSpace(ContainerName))
                {
                    var container = blobClient.GetContainerReference(ContainerName);
                    var blobItems = await GetBlobs(container);
                    var containerTable = blobItems.Select(x =>
                            new
                            {
                                Name = x.Name,
                                Url = x.Uri.ToString()
                            })
                        .ToStringTable(new string[] {"Container Name", "Url"}, x => x.Name, x => x.Url);
                    Console.WriteLine(containerTable);

                    return (int)StatusCodes.Success;
                }

                var containers = await GetContainers(blobClient);

                string table = containers.Select(x => new
                {
                    Name = x.Name,
                    LastModifiedOn = x.Properties.LastModified.Value.DateTime.ToShortDateString()
                }).ToStringTable(new string[] {"Container Name", "Last Modified Date"}, x => x.Name,
                    x => x.LastModifiedOn);

                Console.WriteLine(table);

                return (int)StatusCodes.Success;
            }
            catch (StorageException e)
            {
                switch (e.RequestInformation.ErrorCode)
                {
                        case "ContainerNotFound":
                            Console.WriteLine($"{ContainerName} is not found");
                            return (int)StatusCodes.UnknownError;
                        default:
                            Console.WriteLine(e);
                            return (int)StatusCodes.UnknownError;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (int)StatusCodes.UnknownError;
            }
        }

        private async Task<List<CloudBlobContainer>> GetContainers(CloudBlobClient blobClient)
        {
            BlobContinuationToken continuationToken = null;
            var containers = new List<CloudBlobContainer>();

            do
            {
                ContainerResultSegment response = await blobClient.ListContainersSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                containers.AddRange(response.Results);
            } while (continuationToken != null);

            return containers;
        }

        private async Task<List<CloudBlockBlob>> GetBlobs(CloudBlobContainer container)
        {
            BlobContinuationToken continuationToken = null;
            var containers = new List<IListBlobItem>();

            do
            {
                var response = await container.ListBlobsSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                containers.AddRange(response.Results);
            } while (continuationToken != null);

            return containers.Cast<CloudBlockBlob>().ToList();
        }
    }
}