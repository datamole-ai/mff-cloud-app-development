MFF - NSWI 152 - Cloud Application Development - IoT Solution
---

Web: https://www.ksi.mff.cuni.cz/teaching/nswi152-web/

# Requirements 

- Basic Azure resources knowledge
- Azure account
- C# basics
- .NET 5 SDK
- Tools:
  - [Azure Function Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local#v2)
  - [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
  - [Azure Storage Explorer (if you want read a table by yourself)](https://azure.microsoft.com/en-us/features/storage-explorer/)

Log in via `az login` to your azure account.

Check that it works by typing
- `func --version` to check that the Azure Functions Core Tools are version 3.x.
- `az --version` to check that the Azure CLI version is 2.4 or later.
- `az account list` 
- `dotnet --list-sdks` to check that .NET Core SDK 5 is installed

<!-- az account set --subscription <subscriptionId> -->

# Internet of Things

It's a network of interconnected devices and services that together collect, store and process data. The devices are usually some sensors sending events or measurements -> single purpose and simple.

## Practicals 1 - Use Case

The situation is as follows. The client has one warehouse in which objects are moved from one location to another. The client wants to keep track of the object movement and get daily reports. 

For more details visit [this document](./docs/walkthrough_day1.md)

## Practicals 2 - Use Case

The situation has changed in the following way: 
- The business is thriving and the number of smart warehouses is about to increase from one to a hundred.
- The client request a simple anomaly detection tool.

The architecture would need to adjust at multiple levels

### API

The clients need to send an id of the warehouse they belong to and our Event Collector, Reporter and transports table need to be adjusted. 

### Reporting

We can expect increase demand of requests not only for the separate warehouse but even for the same warehouse (more people need the same data). The Report endpoint can no longer calculate the statistics on-the-fly, it should either precompute them or cache them in some storage.

### Anomaly Detection

This feature is developed by a different team but we need to integrate with them.

## Practicals 3 - Use Case

TBA

# References

* [Assignment](./docs/assignment.md)
* https://docs.microsoft.com/en-us/azure/architecture/patterns/
* https://www.nejlevnejsi-knihy.cz/kniha/designing-data-intensive-applications.html


