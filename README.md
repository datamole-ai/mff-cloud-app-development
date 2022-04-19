# MFF/NSWI152 - Cloud Application Development - IoT

There will be 3 lessons in total. During the first 2 lessons, specifics of IoT cloud solutions will be discussed and end-to-end system design for an example IoT case study will be presented in an incremental way. In the end of the 2nd lesson, the assignment for the semestral project will be presented. 

# Semestral project

The assignment will require students to prepare a system design for a simple IoT case study. Students are expected to deliver the solution for the assignment in form of text documents and diagrams.

The documents and diagrams representing the solution should be stored in **private GitHub repository** and pushed to the branch `feature/solution`. After that following actions are required:

* Create a  **Pull Request** from `feature/solution` branch to `main` branch 
* Add `Read` permissions to following GitHub users:
  * [tomas-pajurek](https://github.com/tomas-pajurek)
  * [jmasek-dtml](https://github.com/jmasek-dtml)
  * [mzatloukal-dtml](https://github.com/mzatloukal-dtml)
* Send email to `mff-nswi152@datamole.ai`

This process will enable smooth delivery of feedback via comments.

Formal assignment can be found in the [semestral-project-assignment.md](./semestral-project-assignment.md) file.

# Lesson Outline

* **Lesson 1** (12. 04. 2022)
  * Overview of relevant Azure resources - Storage (Blobs, Tables), Functions with HTTP Trigger, Application Insights, ARM templates. 
  * Specifics of cloud development for IoT
  * Example IoT case study (basic solution)
  * Voluntary homework I. 
  * For more details visit [lesson-1.md](./lesson-1/lesson-1.md).

* **Lesson 2** (19. 04. 2022)
  * Overview of relevant Azure resources - Event Hub, Functions with Event Hub Trigger.
  * Example IoT case study (advanced solution)
  * Voluntary homework II.
  * Assignment of semestral project.
  * For more details visit [lesson-2.md](./lesson-2/lesson-2.md).

* **Lesson 3** (26. 04. 2022)
  * Deadline for semestral project (till the start of the lesson). 
  * Students' presentations of semestral projects.
  * Discussion of reference solution.
  * Advanced topics.
  * For more details visit [semestral-project-assignment.md](./semestral-project-assignment.md).

# Prerequisites 

## Knowledge

* Concepts and usage of basic Azure resources
  * Azure Storage (blobs, tables)
  * Azure App Insights
  * Azure App Service (Functions)
- Programming in .NET/C#

## Tooling

* IDE/text editor for writing & debugging C# code.
  * Visual Studio
  * Visual Studio Code
  * Rider
* Azure subscription
* .NET 6 SDK
* [Azure Function Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local#v2)
* [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
* [Azure Storage Explorer (if you want read a table by yourself)](https://azure.microsoft.com/en-us/features/storage-explorer/)

Tooling test:

* Log in via `az login` to your azure account.

* `func --version` to check that the Azure Functions Core Tools.
* `az --version` to check that the Azure CLI version is 2.34 or later.
* `az account list` 
* `dotnet --list-sdks` to check that .NET Core SDK 6 is installed. 

## Recommended resources

* [Cloud Architecture Patterns](https://docs.microsoft.com/en-us/azure/architecture/patterns/)
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
* Kleppmann, M. (2017). **Designing Data-Intensive Applications**. O'Reilly Media, Inc. ISBN: 978-1-4493-7332-0 
