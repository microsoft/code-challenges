# Deployment Guide for Azure Cosmos DB Lab

## Azure Resources

To deploy the azure resouces needed for this lab, click the button below

![https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoft%2Fcode-challenges%2Fmaster%2FLabs%2FAzure%20SQL%20Database%2Fdeployment%2Fazuredeploy.json](http://azuredeploy.net/deploybutton.png)

Enter a resource group and a suitable (unique) server name for your SQL server. Enter a username and password of your choosing. Remember these details.

When it has fully deployed, navigate to your SQL Server in the Azure portal and your I.P. Address to the SQL firewall by clicking add Client I.P. and pressing save.

## Seed Data

Open the ShardMapPrep.sln in the databases/ShardMapPrep directory.

Open the program.cs in the Prepare project.

Replace the following lines

```csharp
private const string Username = "<username>";
private const string Password = "<password>";
private const string ShardServer = "<server>.database.windows.net";
```
with the details that you entered above e.g.

```csharp
private const string Username = "adminuser";
private const string Password = "Password1";
private const string ShardServer = "codechallenges.database.windows.net";
```

Press F5 / Press play and wait until it finishes. Your database is ready.

## Set up Lab project

Open the Azure SQL DB Lab.sln in the src directory.

Open the Config.cs file and find the following lines

```csharp
public const string ServerDomainName = "ENTER YOUR AZURE SQL DATABASE (LOGICAL SERVER NAME)";
public const string Username = "ENTER YOUR AZURE SQL DATABASE LOGICAL SERVER USERNAME";
public const string Password = "ENTER YOUR AZURE SQL DATABASE LOGICAL SERVER PASSWORD";
```

and replace with the inputs that you used to deploy your SQL server in the Azure Resources step. e.g.

```csharp
public const string ServerDomainName = "codechallenges.database.windows.net";
public const string Username = "adminuser";
public const string Password = "Password1";
```

The lab can now be followed.

