# Azure Storage CLI Tool

Simple and cross platform Azure Storage CLI Tool

## Getting Started

Install Azure Storage CLI Tool with dotnet global tool

```
dotnet tool install azure-storage-cli --global
```

|Name|Parameters|Description|Example|
|--------------|-------------|-------------|-------------------------|
|login| Azure Storage Account Name, Azure Storage Account Key | Add Azure Storage Account | `as-cli login barisceviz accountKey`
|list| [Optional] Container Name | List containers or blob files from Azure Storage | `as-cli list`, `as-cli list uploadsContainer`
|upload| Filename, Container Name | Upload file to Container | `as-cli upload music.mp3 uploadsContainer --public=true`
|download| Filename | Download file from Azure Storage | `as-cli music.mp3 uploadContainers downloadedMusic.mp3` 


## How to use

### Login

You can add multiple Azure Storage account to CLI

***Paramters***  

|Name|Required|Example|Description|
|--------------|-------------|-------------|-------------------------|
|accountName| True | `"5O7V8l4SeXTymVp3IesT9C"` | Azure Storage account name 
|accountKey| True | `"5O7V8l4SeXTymVp3IesT9C"` | Azure Storage account key 

***Usage***  

```bash

as-cli login accountName accountKey

```

### List

Get containers and blob files from Azure Storage

***Paramters***  

|Name|Required|Description|
|--------------|-----------|-------------------------|
|Container Name|False| List blob files from Azure Storage


***Options***  

|Name|Description|
|--------------|-------------------------|
|--name="barisceviz"|Account Name


***Usage***  
```bash
as-cli list
```

Returns containers

```bash
 | Container Name          | Last Modified Date |
 |----------------------------------------------|
 | aktuellistesi           | 01/11/2018         |
 | azure-jobs-host-archive | 04/14/2018         |
 | azure-jobs-host-output  | 04/14/2018         |
 | azure-webjobs-dashboard | 04/14/2018         |
 | azure-webjobs-hosts     | 01/23/2018         |
 | backups                 | 07/04/2017         |
 | blog                    | 03/05/2017         |
 | chatbotimages           | 05/06/2017         |
 | cryptocurrency-jpg      | 03/13/2018         |
 | cryptocurrency-png      | 03/13/2018         |
 | havadis                 | 05/09/2017         |
 | items                   | 07/06/2018         |
 | posts                   | 03/23/2017         |
 | sinem                   | 03/09/2018         |
 | trainings               | 09/08/2017         |
 | uploads                 | 02/23/2017         |
 | videos                  | 09/23/2018         |
```

Returns blob files

```bash
 | File Name      | Url                                                            |
 |---------------------------------------------------------------------------------|
 | 02032017.json  | https://barisceviz.blob.core.windows.net/havadis/02032017.json |
 | 04032017.json  | https://barisceviz.blob.core.windows.net/havadis/04032017.json |
 | 09052017.json  | https://barisceviz.blob.core.windows.net/havadis/09052017.json |
 | 17022017.json  | https://barisceviz.blob.core.windows.net/havadis/17022017.json |
 | 18022017.json  | https://barisceviz.blob.core.windows.net/havadis/18022017.json |
 | 19022017.json  | https://barisceviz.blob.core.windows.net/havadis/19022017.json |
 | 20022017.json  | https://barisceviz.blob.core.windows.net/havadis/20022017.json |
 | 22022017.json  | https://barisceviz.blob.core.windows.net/havadis/22022017.json |
 | 23022017.json  | https://barisceviz.blob.core.windows.net/havadis/23022017.json |
```

### Download

Download file from container on Azure Storage

***Paramters***  

|Name|Required|Description|
|--------------|-------|-------------------------|
|blobName|True|Blob file name
|containerName|True| Container name | `"TR"`
|filePath|False| Spesific download file name

***Options***  

|Name|Description|
|--------------|-------------------------|
|--name="barisceviz"|Account Name


***Usage***  
```bash
as-cli download 23022017.json havadis 23022017.json
```

### Upload

Upload file to Azure Storage container

***Paramters***  

|Name|Required|Description|
|--------------|-------|-------------------------|
|filePath|True| Spesific download file name
|containerName|True| Container name | `"TR"`

***Options***  

|Name|Description|
|--------------|-------------------------|
|--name="barisceviz"|Account Name

***Usage***  
```bash
as-cli upload 23022017.json uploads
```

## Contribution

* If you want to contribute to codes, create pull request
* If you find any bugs or error, create an issue

## License

This project is licensed under the MIT License
