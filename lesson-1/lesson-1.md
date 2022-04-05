# Lecture 1

## Overview of relevant Azure resources

* Azure Storage (Blobs, Tables)
* Event Hub
* Azure Functions (HTTP Trigger + Event Hub Trigger)
* Application Insights

## Specifics of cloud development for Internet of Things

TODO Tom P.

## Case study problem statement


We will start simple.

The client has one warehouse in which objects are moved from one location to another. The client wants to keep track of the object movement and get daily reports. 

### Examples
* Electronics box number 1 moved from Rack 25 to Rack 35.
* Clothes box number 45 moved from Rack 34 to Rack 20.
* How many objects were moved during the day and what was an average transportation time?

### Ideas ?

### Resulting design

![Design](./imgs/diagram_1.drawio.png)


## Components

- HTTP API
  - Event Consumer
  - Stats Reporter
- Storage

## API

### Event Consumer

Request Method: `POST`

Request Query Parameters: None

Request Body: 

```json
{
  "objectId": "electronics-box-1",
  "transportedDateTime": "2022-04-05T15:01:02Z",
  "locationFrom": "rack-25",
  "locationTo": "rack-35",
  "transportDurationSec": 31
}
```

Response Code:

- `202 Accepted` - Event was successfully stored.
- `400 Bad Request` - Body is not in the correct form.

Response Body: None

### Reporter

Request Method: `GET`

Request Query Parameters: 
- `day`- a day for which the statistics are calculated in form of `yyyy-MM-dd`

Request Body: None

Response Code
- `200 OK` - Statistics calculated and returned in the body
- `204 No Content` - No events for the given day exists
- `400 Bad Request` - The query parameter `day` is not correct

Response Body:

```json
{
  "day": "2022-04-05",
  "totalTransported": 42,
  "avgDurationOfTransportationSec": 40.2,
}
```

# Implementation

* HTTP API - [Azure functions](https://docs.microsoft.com/en-us/azure/azure-functions/)
* Storage - [Azure table](https://docs.microsoft.com/en-us/azure/storage/tables/)

Deployed using [Azure Resource Manager](https://docs.microsoft.com/en-us/azure/templates/) templates.

## Steps

### Repository

Clone the repository

```
git clone https://github.com/datamole-ai/mff-cloud-app-development.git
```

**NOTE**: The root of the repository will be always referred to as `/`. Don't confuse it with root of file system.

It has the following directories

* `/lesson-1`
  * `/arm` - ARM template to create the infrastructure
  * `/sln` - .NET projects
  * lesson-1.md
* `/lesson-2`
  * `/arm` - ARM template to create the infrastructure
  * `/sln` - .NET projects
  * lesson-2.md

During this lesson, only the directory `lesson-1` is relevant.

## Deploy the infrastructure

Prerequisites: Azure CLI

Navigate to the `/arm` directory

See [arm template](/arm/resources.azrm.json). It contains
- function app - Azure function resource
- storage account - required to store the data and the azure function
- app insights - monitoring for azure function 

First thing is to create a new resource group. In the following command, replace the `<resource-group>` with your own name (e.g. "mff-iot-<name>-<surname>").

```pswh
az group create --location 'WestEurope' -n <resource-group>
```

Edit the `storageAccountName` value in the `arm/resources.azrm.parameters.json`.

Then, deploy the infrastructure defined in `arm/resources.azrm.json` with the following command.

```pwsh
cd arm
az deployment group create \
  --name "deploy-mff-task-components" \
  --resource-group <resource-group> \
  --template-file "resources.azrm.json" `
  --parameters "resources.azrm.parameters.json"
```

After it's created, you will see the following lines in the output
```
storageAccountConnectionString  String
  DefaultEndpointsProtocol=https;AccountName=<your-unique-storage-account-name>;EndpointSuffix=core.windows.net;AccountKey=...       
```
Remember it, we will need it for local debugging later.

## Create Azure Functions from a template

Prerequisites: [Azure Functions Core](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=v4%2Cwindows%2Ccsharp%2Cportal%2Cbash#install-the-azure-functions-core-tools)

Create Azure Function .NET project ([docs here](https://docs.microsoft.com/en-us/azure/azure-functions/create-first-function-cli-csharp?tabs=azure-cli%2Cbrowser#create-a-local-function-project))

```pwsh
cd sln
func init "AzureFunctions" --worker-runtime "dotnetIsolated"
```

Create the individual Azure Functions
  
```pwsh
cd sln/AzureFunctions

func new --name "Reporter" --template "HTTP trigger" --authlevel "function"
func new --name "EventConsumer" --template "HTTP trigger" --authlevel "function"
```

When prompted, choose `dotnet` runtime.

After running those commands, change the Azure functions version value in .csproj file from:
```
<AzureFunctionsVersion>v3</AzureFunctionsVersion>
```
to
```
<AzureFunctionsVersion>v4</AzureFunctionsVersion>
```

## Publish Azure Functions

```pwsh
cd sln/AzureFunctions
func azure functionApp publish "mff-iot-example-fa"
```

In the output, you will receive URIs of each azure function. Put them down.

```
Functions in mff-iot-example-fa:
    Reporter - [HttpTrigger]
        Invoke url: https://mff-iot-example-fa.azurewebsites.net/api/reporter?code=<some-code>
    EventConsumer - [HttpTrigger]
        Invoke url: https://mff-iot-example-fa.azurewebsites.net/api/eventconsumer?code=<some-code>
```

## Smoke Test

You can use the uri here. If you didn't put them down, you can get them with the command

```pwsh
func azure functionapp list-functions "mff-iot-example-fa" --show-keys
```

### Event Consumer

Powershell
```pwsh
$body = @{
  locationFrom="a";
  locationTo="b";
  transportDurationSec=1;
  objectId="1";
  transportedDateTime="2021-04-03T12:34:56"
} | ConvertTo-Json

 Invoke-WebRequest -Uri <event-consumer-uri> -Method Post -Body $body
```

cURL

```sh
curl -X POST -H "Content-Type: application/json" \
    -d '{"locationFrom": "a", "locationTo":"b", "transportDurationSec":1, "objectId":"1", "transportedDateTime": "2021-04-03T12:34:56"}' \
    <URI>
```

### Reporter 

Powershell
```pwsh
Invoke-WebRequest -Uri "https://mff-iot-example-fa.azurewebsites.net/api/reporter?code=/<func_code>&day=20210405"
```

cUrl
```sh
curl -X GET "https://mff-iot-example-fa.azurewebsites.net/api/reporter?code=/<func_code>&day=20210405"
```

Log output of each function can be read via Portal -> Function App `mff-iot-example-fa` -> Functions -> Select `Reporter` -> Monitor -> Logs tab

## Check App Insights

Open browser, type in `portal.azure.com` and login.

Navigate to the app insights resource `mff-iot-example-ai`

Click at "Server Requests" metrics on the right side of the page.

See the requests that came to your function. 

In production, App Insights are critical, you can create rules that fire alert and notifies you if something goes wrong.
- Clients stopped sending data
- Significant increase of BAD REQUEST responses after API upgrade -> it was not as backwards compatible as expected.
- Some Internal Server Error -> reveals bugs

## Homework - Implement the HTTP API functions

For manipulating azure table, add package
`dotnet add package WindowsAzure.Storage`

Create model of the HTTP Request 
```cs
public class Transport
{
  [JsonPropertyName("objectId")]
  public string ObjectId { get; set; }
  
  [JsonPropertyName("transportedDateTime")]
  public DateTimeOffset TranportedDateTime{get;set;}
 
  [JsonPropertyName("locationFrom")]
  public string LocationFrom { get; set; }

  [JsonPropertyName("locationTo")]
  public string LocationTo { get; set; }

  [JsonPropertyName("transportDurationSec")]
  public double? TransportDurationSec { get; set; }
}
```
Parse the incoming request in the `/sln/AzureFunctions/EventConsumer.cs` file created by the azure function template.
```cs

public async Task<HttpResponseData> Run(
  [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
  FunctionContext executionContext)
{
  var model = req.ReadFromJsonAsync<Transport>();
}
```
Create a storage client. The environment property will be added later.

```cs
var storageConnectionString = Environment.GetEnvironmentVariable("StorageAccountConnectionString");
var tableName = "transports";

var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
var tableClient = storageAccount.CreateCloudTableClient();

var table = tableClient.GetTableReference(tableName);
```

Create an TableEntity

```cs
public class TransportEntity : TableEntity
{
  public TransportEntity()
  {

  }

  public TransportEntity(
      string transportId,
      string objectId,
      DateTimeOffset transportedDateTimeOffset,
      double transportDurationSec,
      string locationFrom,
      string locationTo
      ) :
      base(transportedDateTimeOffset.ToString("yyyyMMdd"), transportId)
  {
      TransportedDateTime = transportedDateTimeOffset.ToString("o");
      ObjectId = objectId;
      TransportDurationSec = transportDurationSec;
      LocationFrom = locationFrom;
      LocationTo = locationTo;
  }


  [IgnoreProperty]
  public string TransportId => RowKey;
  public string ObjectId { get; set; } = null;
  public double TimeSpentSeconds { get; set; }
  public string LocationFrom { get; set; } = null;

  public string LocationTo { get; set; } = null;

  [IgnoreProperty]
  public DateTimeOffset TransportedDateTimeOffset => DateTimeOffset.Parse(TransportedDateTime, CultureInfo.InvariantCulture);

  public string TransportedDateTime { get; set; } = null!;
}
```

Add transport to the table
```cs
var transportId = Guid.NewGuid().ToString();
var entity = new TransportEntity(
    transportId,
    transport.ObjectId,
    transport.TranportedDateTime,
    transport.TimeSpentSeconds.Value,
    transport.LocationFrom,
    transport.LocationTo
);

var operation = TableOperation.InsertOrMerge(entity);
await _table.ExecuteAsync(operation);
```

And return response
```
return req.CreateResponse(HttpStatusCode.Accepted);
```

**NOTE**: The code above can be used for prototyping but it is not ready for production for the following reasons:

- No requests checks what so ever
  - Body can be empty, contain invalid elements or the elements can have invalid values
- No exception handling
- Creating new instances of the table client with each requests

### Implement Client

Using the commands above
- create table client
- read all required transport record as transport entities 
- map them to Transport models
- return them to the client

You can get inspired how to address the issue in the example solution that will be presented the next time.

### Local Test

**NOTE:** Only http triggered function can be tested locally.

Add the connection string from arm deployment to `local.settings.json`. They will be accessible to the function as enviromental properties.

```json
{
  "IsEncrypted": false,
  "Values": {
      "AzureWebJobsStorage": "UseDevelopmentStorage=true",
      "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
      // add this
      "StorageAccountConnectionString":"<the-connection-string-from-arm>"
  }
}
```

Then navigate to `<project-root>/sln/AzureFunctions` and [run](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=windows%2Ccsharp%2Cbash#start)

```pwsh
func start --build
```

Then use the requests from the section [Smoke Test](#Smoke-Test)

### Storage Check

Open Azure Storage Explorer and log in.

Find the table under your subscription -> Storage Accounts -> `<your-storage-account-name>` -> Tables -> `transports`

Check that all the records are there.

