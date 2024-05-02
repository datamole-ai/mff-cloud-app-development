# Lesson 1

# Horizontal vs. vertical scaling

TODO

# Partitioning & replication

TODO

## Replication (storage)

![Replication (storage)](./imgs/replication-storage.png)

## Partitioning (storage)

![Partitioning (storage)](./imgs/partitioning-storage.png)

## Replication (compute)

![Replication (compute)](./imgs/replication-compute.png)

## Partitioning (compute)

![Partitioning (compute)](./imgs/partitioning-compute.png)

## Partitioning & Replication combined (storage)

![Partitioning & Replication combined (storage)](./imgs/partitioning-replication-combined-storage.png)

# CAP / PACELC theorem

TODO

![CAP/PACELC](./imgs/cap-pacelc.png)

# Overview of relevant Azure resources

TODO

* [Azure Storage](https://learn.microsoft.com/en-us/azure/storage/)
  * [Blobs](https://learn.microsoft.com/en-us/azure/storage/blobs/)
  * [Tables](https://learn.microsoft.com/en-us/azure/storage/tables/)
* [Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/)

# Case study problem statement

The business of your client is thriving! 

Now, they have hundreds of sorting facilities and the projection is to store tens of TBs of data. They already hit the storage limits of the general-purpose Azure SQL and the price gets very high.

They want to present transportation data via a web application accessible to their technicians for troubleshooting within the facilities. Initially, the application will provide data from our existing APIs alongside additional device information from the client's "Device Inventory Service". It is expected that the app will include data from more services, both ours and those of our client.

The data access patterns stay the same.

**Our task:**

* Optimize the storage for higer traffic and higher amounts of data stored
* Design the changes needed to add the web application to the system


## The Original Design
![Design](./imgs/diagram_1.png)

## Ideas - discussion


## Resulting design

![Design](./imgs/diagram_2.png)


## Components

- **Event Consumer**, Azure Function with HTTP trigger
- **Stats Reporter**, Azure Function with HTTP trigger
- **Backend for Frontend**, Azure App Service
- **Storage**, Azure Tables

## Storage Design

Azure Table

### Transport Table Entity

We should ideally use one table per day, so we can efficiently delete the data according to our retention policy.


```
PartitionKey: <TransportedDate>_<FacilityId>
RowKey: <ParcelId>

FacilityId: string
ParcelId: string
TransportedAt: DateTime
LocationFrom: string
LocationTo: string
TimeSpentSeconds: int
DeviceId: string
TransportId: string
```


## Implementation

### Deployment

Prerequisites: [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)

Clone the repository

```
git clone https://github.com/datamole-ai/mff-cloud-app-development.git
```
Navigate to the `lesson-2/arm` directory and see the [ARM template](/lesson-1/arm/resources.azrm.json). It contains
* Function App
* Storage Account with a precreated table
* App Insights
* App Service

First thing is to create a new resource group (or reuse the one from the previous lesson). In the following command, replace the `<resource-group>` with your own name (e.g. "mff-iot-{name}-{surname}").

```pswh
az group create --location 'WestEurope' -n <resource-group>
```

Then, deploy the infrastructure defined in `lesson-2/arm/resources.azrm.json` with the following command. Define the suffix parameter, e.g. your surname. The suffix is used in the names of the resources, so they are unique.

```pwsh
cd lesson-2/arm
az deployment group create `
  --name "deploy-mff-task-components" `
  --resource-group "<resource-group>" `
  --template-file "resources.azrm.json" `
  --parameters suffix="<suffix>"
```

For local development, you need to copy the output values. It should appear in the output as follows:

```json
"outputs": {
      "functionsHostKey": {
        "type": "String",
        "value": "<functionHostKey>"
      },
      "functionsHostUrl": {
        "type": "String",
        "value": "<functionHostUrl>"
      },
      "storageAccountConnectionString": {
        "type": "String",
        "value": "<storageAccountConnectionString>"
      }
    }
```

### Deploy the Functions
Deploy the new versions of the functions. You'll find the name of the function app in the output of the deployment command (it is defined as `mff-iot-fa-<suffix>`).

```
cd lesson-2/sln/AzureFunctions
func azure functionApp publish "<name-of-the-functionapp>" --show-keys
```

### Deploy the Web App

Go to `lesson-2/sln/WebAppBackend`

It is possible to deploy the app directly from your IDE:
- Visual Studio, Rider (with Azure plugin) - Right-click on the project -> Publish

#### Deployment with az cli

Publish the project:
```shell
dotnet publish
```

Create a zip archive from the publish artifacts:

```pwsh
# Powershell
Compress-Archive -Path bin\Release\net8.0\publish\* -DestinationPath deploy.zip
```

Upload the zip file to Azure via az cli:

```shell
az webapp deploy --src-path deploy.zip -n <web-app-name> --resource-group <resource-group-name>
```


## Test

### Backend for Frontend

#### Individual Transport

Powershell
```pwsh
Invoke-WebRequest -Uri "https://<name-of-the-functionapp>.azurewebsites.net/api/gettransport?code=/<func_code>&date=2022-04-05&facilityId=prague&parcelId=123"
```

cUrl
```sh
curl "https://<name-of-the-functionapp>.azurewebsites.net/api/gettransport?code=/<func_code>&date=2022-04-05&facilityId=prague&parcelId=123"
```

Log output of each function can be read via Portal -> Function App `<name-of-the-functionapp>` -> Functions -> Select the Function -> Monitor -> Logs tab

### Local Test

#### Functions

**NOTE:** Only http triggered function can be tested locally.

Add the connection string from arm deployment to `/sln/AzureFunctions/local.settings.json`. They will be accessible to the function as enviromental variables and also automatically loaded as configuration.

```json
{
  "IsEncrypted": false,
  "Values": {
      "AzureWebJobsStorage": "UseDevelopmentStorage=true",
      "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
      // Add this:
      "TransportsStorageConnectionsString":"<the-connection-string-from-arm>"
  }
}
```

Then navigate to `<project-root>/sln/AzureFunctions` and [run](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=windows%2Ccsharp%2Cbash#start)

```pwsh
func start
```

#### Backend for Frontend

Create file `/sln/WebAppBackend/appsettings.Development.json` with the function app host key and the url. You can use the url from the output of the deployment. Alternatively, you can use a url of the locally executed functions. It will appear in the output of `func start` (somthing like this: `http://localhost:7071/`).

```json
{
    "StatsReporter": {
        "FunctionHostKey": "<function-host-key>",
        "FunctionHostUrl": "<function-host-url>"
    }
}
```

Run the project in your IDE, or using `dotnet run` in the `/sln/WebAppBackend/` directory.

### Storage Check

Open [Azure Portal](https://portal.azure.com/) in you browser.

Find the Storage Account resource.

Go to the "Storage browser" in the left panel.

Click on Tables and view the "transports" table.

