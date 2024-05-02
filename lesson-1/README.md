# Lesson 1

## Overview of relevant Azure resources

* [Azure SQL Database](https://learn.microsoft.com/en-us/azure/azure-sql/database)
* [Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/)
  * HTTP Trigger
* [Azure Resource Manager (ARM) Templates](https://docs.microsoft.com/en-us/azure/templates/)

## Cloud-based development of IoT solutions

### Benefits of cloud solutions compared to on-premises solutions

#### Prototyping & Incremental development

* New ideas can be tested immediately without up-front investments and delays.
* Infrastructure for the applications can be added just-in-time.
  * Including e.g. GPUs, NVMes, SGX/SEV.

#### System architecture changes

* Cost of provisioning new infrastructure does not have to be considered.
* ROI of existing infrastructure does not have to be considered.

#### Cost management

* Infrastructure over-provisioning can be avoided with pay-as-you-go pricing.
* For predictable workloads, cost can be optimized with capacity reservations.

#### System load changes

* Capacity of the system can be adjusted/scaled to current demand.
* The scaling is very flexible and can be even on the less-than-hourly basis.

### Drawbacks of cloud solutions compared to on-premises solutions

#### Price

* Renting bare-metal HW can be significantly cheaper.

#### Data residency & security

### Many point of views on IoT solutions

#### Hardware + Embedded Software

* Performance.
* Reliability.
* Power usage.
* Real-time.

#### Integration

* Deployment of devices into field
* Networking.
* Security.
* Updates.

#### Cloud/Backend

* (Stateful) connection of many devices.
* Storing of data.
* Processing of data.
* Cloud-to-device communication.

### Challenges for cloud services

* Unbalanced load in time - autoscaling.
* IoT devices are frequently geodistributed.
    * Network reliability and latency challenges.
    * Might lead to geodistributed system architecture.
* In-order processing of data from a single device.
    * Sequential.
    * Not trivial to achieve in distributed systems.
        * Retries, failovers.
        * Tradeoff with high-availability.
    * Batching- throughput vs latency.
* Parallel processing of data from multiple devices.
* Extremely unreliable clocks on devices.
* Huge amounts of relatively low-value transactions compared to typical business applications.
    * Logging/tracing every single transaction (e.g. collecting one sensor reading) is not feasible.
        * Designing logging/tracing strategy is complex.
        * Debugging is harder.

## Case study problem statement

Your client operates a delivery company with five sorting facilities. In these facilities, robots retrieve parcels from the inbound zones and transport them to the outbound zones, where they are prepared for the next stages of delivery. The client wants to keep track of the parcel movement within the facilities and get daily reports.

**Example:**

Robot R-1 moves parcel 4242 from inbound zone I-12 to outbound zone O-25 in 40 seconds .

**The client needs answers for the following:**

- What was the daily volume of parcels transported within the facilities?
- What was the daily average transportation time?
- How did the parcel 4242 move within a facility F-1 on a day 20-04-2024?

They want to consume the data via HTTP API from their auditing service.

### Ideas - discussion

```




 









 
 
``` 
 

### Resulting design

![Design](./imgs/diagram_1.png)


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
  "transportId": "15asd55cvgh",
  "parcelId": "sf546ad465asd",
  "facilityId": "prague-e12",
  "transportedAt": "2022-04-05T15:01:02Z",
  "locationFrom": "in-25",
  "locationTo": "out-35",
  "transportDurationSec": 31,
  "deviceId": "sorter-1654345"
}
```

Response Code:

- `201 Created` - Event was successfully stored.
- `400 Bad Request` - Body is not in the correct form.

Response Body: None

### Reporter

#### Daily Statistics

Request Method: `GET`

Request Query Parameters: 
- `date`- a day for which the statistics are calculated in form of `yyyy-MM-dd`

Request Body: None

Response Code
- `200 OK` - Statistics calculated and returned in the body
- `204 No Content` - No events for the given day exists
- `400 Bad Request` - The query parameter `day` is not correct

Response Body:

```json
{
  "day": "20220405",
  "totalTransported": 42,
  "avgDurationOfTransportationSec": 40.2,
}
```

#### Transport Information

Request Method: `GET`

Request Query Parameters: 
- `date`- a day of transportation in form of `yyyy-MM-dd`
- `facilityId`- name of the sorting facility
- `parcelId`- id of the parcel

Request Body: None

Response Code
- `200 OK` - Statistics calculated and returned in the body
- `204 No Content` - No events for the given day exists
- `400 Bad Request` - The query parameter `day` is not correct

Response Body:

```json
{
  "transportedDate": "2022-04-05",
  "facilityId": "prague",
  "parcelId": "123",
  "transportedAt": "2022-04-05T15:01:02+00:00",
  "locationFrom": "in-25",
  "locationTo": "out-35",
  "timeSpentSeconds": 31,
  "deviceId": "sorter-1654345",
  "transportId": "15asd55cvgh"
}
```

## Implementation


### Deploy the infrastructure

Prerequisites: [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)

Clone the repository

```
git clone https://github.com/datamole-ai/mff-cloud-app-development.git
```
Navigate to the `lesson-1/arm` directory and see the [ARM template](/lesson-1/arm/resources.azrm.json). It contains
* Function App
* Storage Account
* App Insights
* Azure SQL

First thing is to create a new resource group. In the following command, replace the `<resource-group>` with your own name (e.g. "mff-iot-{name}-{surname}").

```pswh
az group create --location 'WestEurope' -n <resource-group>
```

Edit the values in the `lesson-1/arm/resources.azrm.parameters.json` so they are unique.

Then, deploy the infrastructure defined in `lesson-1/arm/resources.azrm.json` with the following command.

```pwsh
cd lesson-1/arm
az deployment group create `
  --name "deploy-mff-task-components" `
  --resource-group "mff-lectures" `
  --template-file "resources.azrm.json" `
  --parameters "resources.azrm.parameters.json" `
  --parameters adminPassword=<password-to-sql-server>
```

You should copy the Connection String to the database for local development. It should appear in the output as follows:

```json
"outputs": {
  "sqlConnectionString": {
    "type": "String",
    "value": "Server=tcp:mff-iot-sql-...database.windows.net,1433;Initial Catalog=transports;User ID=mffAdmin;Password=<password-to-sql-server>;"
  }
}
```

## Create Azure Functions from a template

Prerequisites: [Azure Functions Core](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=v4%2Cwindows%2Ccsharp%2Cportal%2Cbash#install-the-azure-functions-core-tools)

Create Azure Function .NET project ([docs here](https://docs.microsoft.com/en-us/azure/azure-functions/create-first-function-cli-csharp?tabs=azure-cli%2Cbrowser#create-a-local-function-project))

```pwsh
mkdir ./iot-usecase-1
cd ./iot-usecase-1
func init "AzureFunctions" --worker-runtime "dotnet-isolated" --target-framework "net8.0"
```

Create the individual Azure Functions
  
```pwsh
cd ./iot-usecase-1/AzureFunctions

func new --name "Reporter" --template "HTTP trigger" --authlevel "function"
func new --name "GetDailyStatistics" --template "HTTP trigger" --authlevel "function"
func new --name "GetTransport" --template "HTTP trigger" --authlevel "function"

```

## Publish Azure Functions

```pwsh
cd iot-usecase-1/AzureFunctions
func azure functionApp publish "<name-of-the-functionapp>" --show-keys
```

In the output, you will receive URIs of each azure function. Put them down.

```
    EventConsumer - [httpTrigger]
        Invoke url: https://<name-of-the-functionapp>.azurewebsites.net/api/eventconsumer?code=<code>

    GetDailyStatistics - [httpTrigger]
        Invoke url: https://<name-of-the-functionapp>.azurewebsites.net/api/getdailystatistics?code=<code>

    GetTransport - [httpTrigger]
        Invoke url: https://<name-of-the-functionapp>.azurewebsites.net/api/gettransport?code=<code>
```

You can test the function with your HTTP client of choice:

```
curl "https://<name-of-the-functionapp>.azurewebsites.net/api/gettransport?code=<code>"
```

## Actual Implementation

### Set up Database Schema

Open [Azure Portal](https://portal.azure.com/) in you browser.

Find the SQL Database resource.

Go to Query Editor in the left panel.

Login using the admin credentials.

Execute the following query:

```sql
CREATE TABLE [Transports] (
    [TransportedDate] date NOT NULL,
    [FacilityId] nvarchar(255) NOT NULL,
    [ParcelId] nvarchar(255) NOT NULL,
    [TransportedAt] datetimeoffset NOT NULL,
    [LocationFrom] nvarchar(max) NOT NULL,
    [LocationTo] nvarchar(max) NOT NULL,
    [TimeSpentSeconds] bigint NOT NULL,
    [DeviceId] nvarchar(max) NOT NULL,
    [TransportId] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Transports] PRIMARY KEY ([TransportedDate], [FacilityId], [ParcelId])
);
```

### Set up the Code

You can find the reference implementation in `sln/AzureFunction`. 

Note that it is definitely not production-ready for many reasons (missing error-handling, validations, observability).
It should rather serve as a minimal example on how to glue the Azure resources and code together.

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

Add the connection string from arm deployment to `local.settings.json`. They will be accessible to the function as enviromental variables and also automatically loaded as configuration.

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

Find the SQL Database resource.

Go to Query Editor in the left panel.

Login using the admin credentials.

Find the table: Tables -> dbo.Transports. 

Right-click and select "Select Top 1000 Rows".

