# MFF/NSWI152 - Cloud Application Development - IoT

There will be three lessons in total. During the first 2 lessons, specifics of IoT cloud solutions will be discussed and end-to-end system design for an example IoT use-case will be presented in an incremental way. In the end of the 2nd lesson, the semestral project assignment will be presented. 

# Semestral project

The assignment will be require students to prepare a system design for a different IoT use case. Students are expected to deliver the solution for the assignment in form of text documents and diagrams.

The documents and diagrams representing the solution should be stored in **private GitHub repository** and pushed to the branch `feature/solution`. After that following actions are required:

* Create a  **Pull Request** from `feature/solution` branch to `main` branch 
* Add `Read` permissions to following GitHub users:
  * [tomas-pajurek](https://github.com/tomas-pajurek)
  * [jmasek-dtml](https://github.com/jmasek-dtml)
  * [mzatloukal-dtml](https://github.com/mzatloukal-dtml)
* Send email to `mff-nswi152@datamole.ai`

This process will enable smooth delivery of feedback via comments.

[Assignment](./docs/assignment.md)

# Contents



## Lesson 1 - Use Case (12. 04. 2022)

The situation is as follows. The client has one warehouse in which objects are moved from one location to another. The client wants to keep track of the object movement and get daily reports. 

For more details visit [this document](./docs/walkthrough_day1.md).

## Lesson 2 - Use Case (19. 04. 2022)

The situation has changed in the following way: 
- The business is thriving and the number of smart warehouses is about to increase from one to a hundred.
- The client request a simple anomaly detection tool.

The architecture would need to adjust at multiple levels.

For more details visit [this document](./docs/walkthrough_day2.md).

## Lesson 3: Semestral project presentations, Bonus topics (26. 04. 2022)

* Students will present solutions for their semestral project.
* Reference solution will be provided.



Web: https://www.ksi.mff.cuni.cz/teaching/nswi152-web/

# Prerequisites 

- Basic Azure resources knowledge
- C# basics

## Tooling


* TODO: describe C#/.NET Tooling - nothing is needed and supported

- Azure account
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

# Recommended resources

* [Cloud Architecture Patterns](https://docs.microsoft.com/en-us/azure/architecture/patterns/)
* Kleppmann, M. (2017). **Designing Data-Intensive Applications**. O'Reilly Media, Inc. ISBN: 978-1-4493-7332-0 
* Azure Storage Documentation
  * [Blobs](https://azure.microsoft.com/en-us/services/storage/blobs/)
  * [Tables - Design](https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-design-guidelines)
  * [Tables - Patterns](https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-design-patterns)
  * [Tables - Query](https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-design-for-query)
  * [Tables - Writes/Updates](https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-design-for-modification)
  * [Tables - Modelling](https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-design-modeling)
* Azure Event Hubs Documentation
  * [Terminology](https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-features)
  * [Scaling](https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-scalability)
  * [Availability/Consistency](https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-availability-and-consistency?tabs=dotnet)

