using AzureStorageCLI.Commands;
using McMaster.Extensions.CommandLineUtils;

namespace AzureStorageCLI
{
    [Command(Name = "azure-cli", Description = "Azure Storage CLI Tool")]
    [HelpOption("-h|--help")]
    [Subcommand("login", typeof(LoginCommand))]
    [Subcommand("upload", typeof(UploadCommand))]
    [Subcommand("download", typeof(DownloadCommand))]
    [Subcommand("list", typeof(ListCommand))]
    public class App
    {
        public void OnExecute(CommandLineApplication app)
        {
            
        }
    }
}