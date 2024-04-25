# Lesson 1

## Overview of relevant Azure resources
* [Azure Storage](https://learn.microsoft.com/en-us/azure/storage/)
  * [Blobs](https://learn.microsoft.com/en-us/azure/storage/blobs/)
  * [Tables](https://learn.microsoft.com/en-us/azure/storage/tables/)
* [Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/)

## Case study problem statement

The business of your client is thriving! 

Now, they have hundreds of sorting facilities and they need to store tens of TBs of data. The already hit the storage limits of the general-purpose Azure SQL. They want to increase the traffic from their Auditing Service, but the price of the SQL server gets very high.

They want to present transportation data via a web application accessible to their technicians for troubleshooting within the facilities. Initially, the application will provide data from our existing APIs alongside additional device information from the client's "Device Inventory Service". It is expected that the app will include data from more services, both ours and client's. It is also planned to develop a real-time dashboard using websockets or similar technology.

The data access patterns stay the same.

**Our task:**

* Optimize the storage for higer traffic and storage
* Design the changes needed to add the web application to the system


### The Original Design
![Design](./imgs/diagram_1.png)

### Ideas - discussion

```




 









 
 
``` 
 

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


```
PartitionKey: <TransportedDate>_<FacilityId>
RowKey: <ParcelId>

TransportedDate: string
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

Edit the values in the `lesson-2/arm/resources.azrm.parameters.json` so they are unique.

Then, deploy the infrastructure defined in `lesson-2/arm/resources.azrm.json` with the following command.

```pwsh
cd lesson-1/arm
az deployment group create `
  --name "deploy-mff-task-components" `
  --resource-group "mff-lectures" `
  --template-file "resources.azrm.json" `
  --parameters "resources.azrm.parameters.json"
```

You should copy the Connection String to the storage and API key for the function host for local development. It should appear in the output as follows:

```json
"outputs": {
      "functionsHostKey": {
        "type": "String",
        "value": "<key>"
      },
      "storageAccountConnectionString": {
        "type": "String",
        "value": "<storageAccountConnectionString>"
      }
}
```

### Deploy the Functions
Deploy the new versions of the functions

```
cd lesson-2/sln/AzureFunctions
func azure functionApp publish "<name-of-the-functionapp>" --show-keys
```

### Deploy the Web App

Go to `lesson-2/sln/WebAppBackend`

It is possible to deploy the app directly from your IDE:
- VS, Rider (with Azure plugin) - Right-click on the project -> Publish

#### Deployment with az cli

Publish the project:
```shell
dotnet publish
```

Create a zip archive from the artifacts:

```pwsh
# Powershell
Compress-Archive -Path bin\Release\net8.0\publish\* -DestinationPath deploy.zip
```

Upload the zip file to Azure:

```shell
az webapp deploy --src-path deploy.zip -n <web-app-name> --resource-group <resource-group-name>
```


## Test

### Event Consumer

Powershell
```pwsh
$body = @{
  locationFrom="a";
  locationTo="b";
  transportDurationSec=30;
  parcelId="1";
  transportedAt="2022-04-05T15:01:02Z";
  deviceId="sorter-123";
  facilityId="facility-123";
  transportId="t-4156";
} | ConvertTo-Json

 Invoke-WebRequest -Uri <event-consumer-uri> -Method Post -Body $body -ContentType "application/json"
```

cURL

```sh
curl -X POST -H "Content-Type: application/json" \
    -d '{"parcelId": "12345","facilityId": "prague","transportedAt": "2022-04-05T15:01:02Z", "locationFrom": "in-25",  "locationTo": "out-35",  "transportDurationSec": 50,  "deviceId": "sorter-1654345", "transportId": "t-4156"
}' \
    <URI>
```

### Reporter

#### Daily Statistics

Powershell
```pwsh
Invoke-WebRequest -Uri "https://<name-of-the-functionapp>.azurewebsites.net/api/getdailystatistics?code=/<func_code>&date=2022-04-05"
```

cUrl
```sh
curl "https://<name-of-the-functionapp>.azurewebsites.net/api/getdailystatistics?code=/<func_code>&date=2022-04-05"
```

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

**NOTE:** Only http triggered function can be tested locally.

Add the connection string from arm deployment to `/sln/AzureFunctions/local.settings.json`. They will be accessible to the function as enviromental variables and also automatically loaded as configuration.

```json
{
  "IsEncrypted": false,
  "Values": {
      "AzureWebJobsStorage": "UseDevelopmentStorage=true",
      "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
      // Add this:
      "TransportsDbConnectionString":"<the-connection-string-from-arm>"
  }
}
```

Then navigate to `<project-root>/sln/AzureFunctions` and [run](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=windows%2Ccsharp%2Cbash#start)

```pwsh
func start
```

Then use the requests from the section [Test](#Test).

### Storage Check


Open [Azure Portal](https://portal.azure.com/) in you browser.

Find the Storage Account resource.

Go to the "Storage browser" in the left panel.

Click on Tables and view the "transports" table.

